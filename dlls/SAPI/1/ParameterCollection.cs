// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ParameterCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

#nullable disable
namespace SapiLayer1;

public sealed class ParameterCollection : LateLoadReadOnlyCollection<Parameter>, IDisposable
{
  private Dictionary<string, Parameter> cache;
  private Dictionary<string, Parameter> combinedCache;
  private Channel channel;
  private float progress;
  private VcpHelper vcpHelper;
  private bool disposed;
  private bool haveBeenReadFromEcu;
  private StringCollection accumulatorPrefixes;
  private StringDictionary groupCodingStrings;
  private StringDictionary originalGroupCodingStrings;
  private Dictionary<string, CodingStringState> groupCodingStringsState = new Dictionary<string, CodingStringState>();
  private object groupCodingStringsStateLock = new object();
  private bool verifyAfterWrite;
  private bool verifyAfterCommit;
  private bool serializeGroupNames;

  internal ParameterCollection(Channel c)
  {
    this.channel = c;
    this.verifyAfterWrite = this.channel.Ecu.Properties.GetValue<bool>(nameof (VerifyAfterWrite), true);
    this.verifyAfterCommit = this.channel.Ecu.Properties.GetValue<bool>(nameof (VerifyAfterCommit), false);
    this.serializeGroupNames = this.channel.Ecu.Properties.GetValue<bool>("SerializeParameterGroupNames", false);
    this.AutoReadSummaryParameters = true;
    this.vcpHelper = new VcpHelper(this);
    this.accumulatorPrefixes = new StringCollection();
    if (c.Ecu.Xml != null)
    {
      XmlNodeList xmlNodeList = c.Ecu.Xml.SelectNodes("Ecu/AccumulatorGroups/AccumulatorGroup");
      if (xmlNodeList != null)
      {
        for (int index = 0; index < xmlNodeList.Count; ++index)
        {
          XmlNode xmlNode = xmlNodeList.Item(index);
          if (xmlNode != null)
            this.accumulatorPrefixes.Add(xmlNode.Attributes.GetNamedItem("Qualifier").InnerText);
        }
      }
    }
    this.groupCodingStrings = new StringDictionary();
    this.originalGroupCodingStrings = new StringDictionary();
  }

  protected override void AcquireList()
  {
    if (this.channel.McdEcuHandle != null)
    {
      uint nParameterIndex = 0;
      Dictionary<string, List<McdDBService>> dictionary = this.channel.McdEcuHandle.DBServices.Where<McdDBService>((Func<McdDBService, bool>) (s => s.Semantic == "VARIANTCODINGREAD" || s.Semantic == "VARIANTCODINGWRITE")).ToList<McdDBService>().GroupBy<McdDBService, string>((Func<McdDBService, string>) (s => s.Semantic)).ToDictionary<IGrouping<string, McdDBService>, string, List<McdDBService>>((Func<IGrouping<string, McdDBService>, string>) (g => g.Key), (Func<IGrouping<string, McdDBService>, List<McdDBService>>) (g => g.ToList<McdDBService>()));
      if (!dictionary.ContainsKey("VARIANTCODINGREAD"))
        return;
      foreach (McdDBService mcdDbService in dictionary["VARIANTCODINGREAD"])
      {
        McdDBService readPrimitive = mcdDbService;
        McdDBService writePrimitive = dictionary["VARIANTCODINGWRITE"].FirstOrDefault<McdDBService>((Func<McdDBService, bool>) (s => s.DefaultPdu.Take<byte>(readPrimitive.DefaultPdu.Count<byte>()).Skip<byte>(1).SequenceEqual<byte>(readPrimitive.DefaultPdu.Skip<byte>(1))));
        string domainQualifier = McdCaesarEquivalence.GetDomainQualifier(readPrimitive.Name);
        if (readPrimitive != null && writePrimitive != null)
        {
          this.groupCodingStrings.Add(domainQualifier, (string) null);
          this.groupCodingStringsState.Add(domainQualifier, CodingStringState.None);
          this.CheckAndAddParameterGroup(domainQualifier, ref nParameterIndex, readPrimitive, writePrimitive);
        }
        else
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"Unable to locate all necessary services for varcoding group {this.channel.Ecu.Name}.{domainQualifier}");
      }
    }
    else if (this.channel.EcuHandle != null)
    {
      ServiceCollection serviceCollection = new ServiceCollection(this.channel, ServiceTypes.ReadVarCode | ServiceTypes.WriteVarCode);
      uint nParameterIndex = 0;
      lock (this.channel.OfflineVarcodingHandleLock)
      {
        Varcode offlineVarcodingHandle = this.channel.OfflineVarcodingHandle;
        if (offlineVarcodingHandle != null)
        {
          StringCollection varCodeDomains = this.channel.EcuHandle.VarCodeDomains;
          for (int index = 0; index < varCodeDomains.Count; ++index)
          {
            string str = varCodeDomains[index];
            this.groupCodingStrings.Add(str, (string) null);
            this.groupCodingStringsState.Add(str, CodingStringState.None);
            using (CaesarDIVarCodeDom varcodeDom = this.channel.EcuHandle.OpenVarCodeDomain(str))
            {
              if (varcodeDom != null)
              {
                Service readService = serviceCollection[varcodeDom.ReadDefaultStringService];
                Service writeService = serviceCollection[varcodeDom.WriteCodingStringService];
                this.CheckAndAddParameterGroup(offlineVarcodingHandle, varcodeDom, str, ref nParameterIndex, readService, writeService);
              }
            }
          }
        }
      }
      XmlNode xml = this.channel.Ecu.Xml;
      if (xml == null)
        return;
      XmlNodeList xmlNodeList = xml.SelectNodes("Ecu/ServicesAsParameters/ServiceAsParameter");
      if (xmlNodeList == null)
        return;
      Parameter parameter = (Parameter) null;
      for (int index = 0; index < xmlNodeList.Count; ++index)
      {
        XmlNode xmlNode = xmlNodeList.Item(index);
        string innerText1 = xmlNode.Attributes.GetNamedItem("Qualifier").InnerText;
        string innerText2 = xmlNode.Attributes.GetNamedItem("Name").InnerText;
        XmlNode namedItem1 = xmlNode.Attributes.GetNamedItem("WriteReferenceQualifier");
        XmlNode namedItem2 = xmlNode.Attributes.GetNamedItem("ReadReferenceQualifier");
        XmlNode namedItem3 = xmlNode.Attributes.GetNamedItem("WriteReferenceIndex");
        XmlNode namedItem4 = xmlNode.Attributes.GetNamedItem("Hide");
        XmlNode namedItem5 = xmlNode.Attributes.GetNamedItem("Accumulator");
        string str1 = string.Empty;
        string str2 = string.Empty;
        int result = -1;
        bool flag = false;
        Service writeService = (Service) null;
        Service readService = (Service) null;
        bool hide = false;
        bool persistable = true;
        if (namedItem1 != null)
        {
          str1 = namedItem1.InnerText;
          writeService = this.channel.Services[str1];
          if (writeService != (Service) null)
          {
            if (namedItem3 != null)
            {
              if (int.TryParse(namedItem3.InnerText, out result))
              {
                if (result < 0 || result > writeService.InputValues.Count - 1)
                {
                  flag = true;
                  Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service as VCP parameter {0} definition error - write reference index out of range {1}", (object) innerText2, (object) result));
                }
              }
              else
              {
                flag = true;
                Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service as VCP parameter {0} definition error - write reference index could not be parsed.", (object) innerText2));
              }
            }
            else
            {
              flag = true;
              Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service as VCP parameter {0} definition error - write reference index does not exist, when write service specified", (object) innerText2));
            }
          }
          else
          {
            flag = true;
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service as VCP parameter definition error - write service {0} does not exist", (object) str1));
          }
        }
        if (namedItem2 != null)
        {
          str2 = namedItem2.InnerText;
          readService = this.channel.Services[str2];
          if (readService == (Service) null)
          {
            EcuInfo ecuInfo = this.channel.EcuInfos[str2] ?? this.channel.EcuInfos.GetItemContaining(str2);
            if (ecuInfo != null && ecuInfo.Services.Count > 0)
              readService = ecuInfo.Services[0];
          }
          if (readService == (Service) null)
          {
            flag = true;
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service as VCP parameter definition error - read service {0} does not exist", (object) str2));
          }
        }
        if (namedItem4 != null)
          hide = Convert.ToBoolean(namedItem4.InnerText, (IFormatProvider) CultureInfo.InvariantCulture);
        if (namedItem5 != null)
          persistable = !Convert.ToBoolean(namedItem5.InnerText, (IFormatProvider) CultureInfo.InvariantCulture);
        if (!flag)
        {
          parameter = new Parameter(this.channel, nParameterIndex++, "SVC_ServicesAsParameters", "Services", persistable, this.Count);
          parameter.Acquire(innerText1, innerText2, writeService, str1, result, readService, str2, hide);
          this.Items.Add(parameter);
        }
      }
      if (parameter == null)
        return;
      parameter.LastInGroup = true;
    }
    else
    {
      if (!this.vcpHelper.HasExternalVcp)
        return;
      TextFieldParser defParser = this.channel.Ecu.GetDefParser();
      if (defParser == null)
        return;
      string groupName = this.channel.Ecu.Properties.GetValue<string>("VcpGroupName", "VCP");
      string groupQualifier = this.channel.Ecu.Properties.GetValue<string>("VcpGroupQualifier", "VCP");
      uint num = 0;
      while (!defParser.EndOfData)
      {
        string[] fields = defParser.ReadFields();
        if (fields[0] == "P" && fields.Length >= 10)
        {
          Parameter parameter = new Parameter(this.channel, num++, groupQualifier, groupName, true, this.Count);
          parameter.Acquire(fields);
          this.Items.Add(parameter);
        }
      }
    }
  }

  private void InternalRead(Varcode varcode, ref CaesarException ce)
  {
    string str = string.Empty;
    int startIndex = -1;
    int num = 0;
    for (int index = 0; index < this.Count && ce == null && this.channel.ChannelRunning && !this.channel.Closing; ++index)
    {
      Parameter parameter = this[index];
      if (!string.Equals(parameter.GroupQualifier, str, StringComparison.Ordinal))
      {
        str = parameter.GroupQualifier;
        startIndex = index;
        num = 0;
      }
      if (!parameter.HasBeenReadFromEcu && parameter.Marked)
        ++num;
      if (num > 0 && parameter.LastInGroup)
      {
        this.UpdateProgress(Convert.ToSingle((object) index, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToSingle((object) this.Count, (IFormatProvider) CultureInfo.InvariantCulture));
        try
        {
          if (this.InternalReadGroup(str, startIndex, index, false, varcode))
            this.channel.CodingParameterGroups[str]?.AcquireDefaultStringandFragmentChoicesForCoding(this.groupCodingStrings[str]);
        }
        catch (CaesarException ex)
        {
          ce = ex;
        }
      }
    }
    this.UpdateHaveBeenReadFromEcuFlag();
  }

  internal void InternalRead()
  {
    CaesarException ce = (CaesarException) null;
    if (this.channel.ChannelHandle != null || this.channel.McdChannelHandle != null)
    {
      using (Varcode varcode = this.channel.VCInit())
        this.InternalRead(varcode, ref ce);
    }
    else if (this.vcpHelper.HasExternalVcp)
    {
      if (this.Count > 0)
      {
        try
        {
          this.vcpHelper.ProcessExternalRead();
        }
        catch (CaesarException ex)
        {
          ce = ex;
        }
        this.UpdateHaveBeenReadFromEcuFlag();
      }
    }
    this.channel.SetCommunicationsState(CommunicationsState.Online);
    this.RaiseParameterCommunicationCompleteEvent((Exception) ce, false);
  }

  internal void InternalReadGroupVcp(string groupQualifier)
  {
    Parameter itemFirstInGroup = this.GetItemFirstInGroup(groupQualifier);
    Parameter itemLastInGroup = this.GetItemLastInGroup(groupQualifier);
    if (itemFirstInGroup == null || itemLastInGroup == null)
      return;
    this.InternalReadGroup(groupQualifier, itemFirstInGroup.Index, itemLastInGroup.Index, false);
  }

  public bool AutoReadSummaryParameters { get; set; }

  internal void InternalReadGroup(Parameter parameter, bool explicitRead)
  {
    CaesarException e = (CaesarException) null;
    Parameter itemLastInGroup = this.GetItemLastInGroup(parameter.GroupQualifier);
    try
    {
      this.InternalReadGroup(parameter.GroupQualifier, parameter.Index, itemLastInGroup.Index, true);
    }
    catch (CaesarException ex)
    {
      e = ex;
    }
    this.UpdateHaveBeenReadFromEcuFlag();
    if (!explicitRead)
      return;
    this.channel.SetCommunicationsState(CommunicationsState.Online);
    this.RaiseParameterCommunicationCompleteEvent((Exception) e, false);
  }

  internal void InternalWriteVcp()
  {
    Varcode varcode = (Varcode) null;
    CaesarException caesarException = (CaesarException) null;
    List<Tuple<string, int, int>> tupleList = new List<Tuple<string, int, int>>();
    if (this.channel.ChannelHandle != null || this.channel.McdChannelHandle != null)
    {
      int num1 = -1;
      List<Parameter> parameterList = new List<Parameter>();
      string str = string.Empty;
      bool flag = false;
      int num2 = 0;
      for (int index = 0; index < this.Count; ++index)
      {
        Parameter parameter = this[index];
        if (!parameter.ServiceAsParameter && this.IsCodingStringAssignedByClient(parameter.GroupQualifier) || parameter.Marked && !object.Equals(parameter.OriginalValue, parameter.Value))
          ++num2;
      }
      int num3 = 0;
      for (int index = 0; index < this.Count && caesarException == null && this.channel.ChannelRunning && !this.channel.Closing; ++index)
      {
        Parameter p = this[index];
        if (!string.Equals(p.GroupQualifier, str, StringComparison.Ordinal))
        {
          str = p.GroupQualifier;
          num1 = index;
          parameterList.Clear();
          flag = p.ServiceAsParameter;
        }
        if (!flag && this.IsCodingStringAssignedByClient(str) || p.Marked && !object.Equals(p.OriginalValue, p.Value))
        {
          if (parameterList.Count == 0 && !flag)
          {
            byte? responseSession;
            if (this.channel.IntendedSession.HasValue && this.channel.GetActiveDiagnosticInformation(out responseSession, out uint? _) && !responseSession.Equals((object) this.channel.IntendedSession))
              throw new CaesarException(SapiError.EcuFailedToRemainInDiagnosticSession);
            if (varcode == null)
              varcode = this.channel.VCInit();
            bool communicatedViaJob = this.channel.ParameterGroups[str].CommunicatedViaJob;
            string groupCodingString = this.IsCodingStringState(str, CodingStringState.Incomplete) || communicatedViaJob ? (string) null : this.groupCodingStrings[str];
            if (!string.IsNullOrEmpty(groupCodingString))
            {
              varcode.EnableReadCodingStringFromEcu(false);
              varcode.SetCurrentCodingString(p.GroupQualifier, new Dump(groupCodingString).Data.ToArray<byte>());
            }
            else
              varcode.EnableReadCodingStringFromEcu(true);
          }
          ++num3;
          this.UpdateProgress(Convert.ToSingle((object) num3, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToSingle((object) num2, (IFormatProvider) CultureInfo.InvariantCulture));
          if (varcode != null | flag)
          {
            p.InternalWrite(varcode);
            if (p.Exception == null)
              this.RaiseParameterUpdateEvent(p, (Exception) null);
          }
          parameterList.Add(p);
        }
        if (parameterList.Count > 0 && p.LastInGroup)
        {
          if (varcode != null)
          {
            try
            {
              varcode.DoCoding();
            }
            catch (NullReferenceException ex)
            {
              Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Intentional catch of exception from VCDoCoding. Comms failure?");
            }
            if (varcode.IsErrorSet)
            {
              CaesarException exception = varcode.Exception;
              varcode.Dispose();
              varcode = (Varcode) null;
              if (this.channel.ChannelRunning)
              {
                foreach (Parameter parameter in parameterList)
                  parameter.Exception = (Exception) exception;
              }
              else
                caesarException = exception;
            }
          }
          else if (this.channel.IsChannelErrorSet)
            caesarException = new CaesarException(this.channel.ChannelHandle);
          if (this.channel.ChannelRunning)
            tupleList.Add(new Tuple<string, int, int>(str, num1, index));
        }
      }
      varcode?.Dispose();
      if (this.channel.ChannelRunning)
      {
        if (num2 > 0)
        {
          if ((this.channel.ChannelOptions & ChannelOptions.ExecuteParameterWriteInitializeCommitServices) != ChannelOptions.None && this.verifyAfterCommit)
            this.channel.Services.InternalDereferencedExecute("CommitToPermanentMemoryService");
          if (this.channel.ChannelRunning && (this.verifyAfterWrite || this.verifyAfterCommit))
          {
            using (Varcode vh = this.channel.VCInit())
            {
              foreach (Tuple<string, int, int> tuple in tupleList)
              {
                if (this.channel.ChannelRunning)
                {
                  if (!this.channel.Closing)
                  {
                    try
                    {
                      this.InternalReadGroup(tuple.Item1, tuple.Item2, tuple.Item3, false, vh);
                    }
                    catch (CaesarException ex)
                    {
                      if (caesarException == null)
                        caesarException = ex;
                    }
                  }
                }
              }
            }
          }
          if ((this.channel.ChannelOptions & ChannelOptions.ExecuteParameterWriteInitializeCommitServices) != ChannelOptions.None && this.verifyAfterWrite)
            this.channel.Services.InternalDereferencedExecute("CommitToPermanentMemoryService");
        }
      }
      else if (caesarException == null)
        caesarException = new CaesarException(SapiError.CommunicationsCeasedDuringVarcoding);
    }
    else if (this.vcpHelper.HasExternalVcp)
    {
      if (this.Count > 0)
      {
        try
        {
          this.vcpHelper.ProcessExternalWrite();
        }
        catch (CaesarException ex)
        {
          caesarException = ex;
        }
      }
    }
    if (caesarException != null)
      throw caesarException;
  }

  internal void InternalWrite()
  {
    CaesarException e = (CaesarException) null;
    if ((this.channel.ChannelOptions & ChannelOptions.ExecuteParameterWriteInitializeCommitServices) != ChannelOptions.None)
      this.Channel.Services.InternalDereferencedExecute("ParameterWriteInitializeService");
    try
    {
      this.InternalWriteVcp();
    }
    catch (CaesarException ex)
    {
      e = ex;
    }
    this.channel.SetCommunicationsState(CommunicationsState.Online);
    this.RaiseParameterCommunicationCompleteEvent((Exception) e, true);
  }

  internal void RaiseParameterUpdateEvent(Parameter p, Exception e)
  {
    FireAndForget.Invoke((MulticastDelegate) this.ParameterUpdateEvent, (object) p, (EventArgs) new ResultEventArgs(e));
  }

  internal void RaiseParameterCommunicationCompleteEvent(Exception e, bool write)
  {
    if (write)
      FireAndForget.Invoke((MulticastDelegate) this.ParametersWriteCompleteEvent, (object) this, (EventArgs) new ResultEventArgs(e));
    else
      FireAndForget.Invoke((MulticastDelegate) this.ParametersReadCompleteEvent, (object) this, (EventArgs) new ResultEventArgs(e));
    this.channel.SyncDone(e);
  }

  internal Parameter GetParameter(string parameterName, int hintIndex)
  {
    string alternateName;
    bool alternateAvailable = this.channel.Ecu.AlternateQualifiers.TryGetValue(parameterName, out alternateName);
    parameterName = ParameterCollection.StripUnderscores(parameterName);
    if (alternateAvailable)
      alternateName = ParameterCollection.StripUnderscores(alternateName);
    bool parameterNameIsCombined = parameterName.Contains<char>('.');
    return this.Skip<Parameter>(hintIndex).Concat<Parameter>(this.Take<Parameter>(hintIndex)).Select(item => new
    {
      item = item,
      itemName = ParameterCollection.StripUnderscores(parameterNameIsCombined ? item.CombinedQualifier : item.Qualifier)
    }).Where(_param1 =>
    {
      if (_param1.itemName.CompareNoCase(parameterName))
        return true;
      return alternateAvailable && _param1.itemName.CompareNoCase(alternateName);
    }).Select(_param1 => _param1.item).FirstOrDefault<Parameter>();
  }

  internal void UpdateProgress(float nominator, float denominator)
  {
    this.progress = (float) ((double) nominator / (double) denominator * 100.0);
  }

  internal void ResetExceptions()
  {
    foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) this)
      parameter.ResetException();
  }

  internal void ResetEcuReadFlags()
  {
    foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) this)
      parameter.ResetHasBeenReadFromEcu();
    this.haveBeenReadFromEcu = false;
    this.originalGroupCodingStrings.Clear();
  }

  internal void InternalProcessVcp()
  {
    this.vcpHelper.Process();
    this.channel.SetCommunicationsState(CommunicationsState.Online);
    FireAndForget.Invoke((MulticastDelegate) this.ParametersProcessVcpCompleteEvent, (object) this, (EventArgs) new ResultEventArgs((Exception) null));
    this.channel.SyncDone((Exception) null);
  }

  internal void SynchronousCheckFailure(object sender, CaesarException exception)
  {
    FireAndForget.Invoke((MulticastDelegate) this.ParametersWriteCompleteEvent, (object) this, (EventArgs) new ResultEventArgs((Exception) exception));
  }

  internal void ResetGroupCodingString(string groupQualifier)
  {
    lock (this.groupCodingStringsStateLock)
    {
      if (!this.groupCodingStringsState.ContainsKey(groupQualifier))
        return;
      this.groupCodingStringsState[groupQualifier] |= CodingStringState.NeedsUpdate;
    }
  }

  internal void SetGroupCodingString(string groupQualifier, string newValue)
  {
    lock (this.groupCodingStringsStateLock)
    {
      this.groupCodingStringsState[groupQualifier] |= CodingStringState.AssignedByClient;
      this.groupCodingStringsState[groupQualifier] &= ~CodingStringState.Incomplete;
    }
    this.SetGroupCodingString(groupQualifier, newValue, this.Where<Parameter>((Func<Parameter, bool>) (p => string.Equals(p.GroupQualifier, groupQualifier, StringComparison.OrdinalIgnoreCase))));
  }

  internal void SetGroupCodingString(
    string groupQualifier,
    string newValue,
    IEnumerable<Parameter> parametersToRead)
  {
    CaesarException caesarException = (CaesarException) null;
    byte[] array = new Dump(newValue).Data.ToArray<byte>();
    lock (this.Channel.OfflineVarcodingHandleLock)
    {
      Varcode offlineVarcodingHandle = this.Channel.OfflineVarcodingHandle;
      if (offlineVarcodingHandle != null)
      {
        offlineVarcodingHandle.SetCurrentCodingString(groupQualifier, array);
        if (offlineVarcodingHandle.IsErrorSet)
        {
          caesarException = offlineVarcodingHandle.Exception;
        }
        else
        {
          this.groupCodingStrings[groupQualifier] = newValue;
          foreach (Parameter parameter in parametersToRead)
            parameter.InternalRead(offlineVarcodingHandle, false);
        }
      }
    }
    if (caesarException != null)
      throw caesarException;
  }

  public void Read(bool synchronous)
  {
    this.ResetExceptions();
    this.channel.QueueAction((object) CommunicationsState.ReadParameters, synchronous);
  }

  public void ReadAll(bool synchronous)
  {
    this.ResetEcuReadFlags();
    this.Read(synchronous);
  }

  public void ReadGroup(string groupQualifier, bool fromCache, bool synchronous)
  {
    this.ResetExceptions();
    Parameter itemFirstInGroup = this.GetItemFirstInGroup(groupQualifier);
    if (itemFirstInGroup == null)
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Parameter group {0} not valid", (object) groupQualifier), nameof (groupQualifier));
    if (!fromCache)
    {
      for (int index = itemFirstInGroup.Index; index < this.Count; ++index)
      {
        Parameter parameter = this[index];
        if (string.Equals(groupQualifier, parameter.GroupQualifier, StringComparison.Ordinal))
          parameter.ResetHasBeenReadFromEcu();
        else
          break;
      }
    }
    this.channel.QueueAction((object) itemFirstInGroup, synchronous);
  }

  public void Write(bool synchronous)
  {
    this.ResetExceptions();
    this.channel.QueueAction((object) CommunicationsState.WriteParameters, synchronous, new SynchronousCheckFailureHandler(this.SynchronousCheckFailure));
  }

  public void Load(
    string path,
    ParameterFileFormat parameterFileFormat,
    Collection<string> unknownList)
  {
    using (StreamReader inputStream = new StreamReader(path))
      this.Load(inputStream, parameterFileFormat, unknownList, true);
  }

  public void Load(
    string path,
    ParameterFileFormat parameterFileFormat,
    StringDictionary unknownList)
  {
    StreamReader inputStream = new StreamReader(path);
    try
    {
      this.Load(inputStream, parameterFileFormat, unknownList, true);
    }
    finally
    {
      inputStream.Close();
    }
  }

  public void Save(string path, ParameterFileFormat parameterFileFormat)
  {
    StreamWriter outputStream = new StreamWriter(path);
    this.Save(outputStream, parameterFileFormat, true);
    outputStream.Close();
  }

  public void SaveAccumulator(string path, ParameterFileFormat parameterFileFormat)
  {
    StreamWriter outputStream = new StreamWriter(path);
    this.SaveAccumulator(outputStream, parameterFileFormat, true);
    outputStream.Close();
  }

  public void Load(
    StreamReader inputStream,
    ParameterFileFormat parameterFileFormat,
    Collection<string> unknownList,
    bool respectAccessLevels)
  {
    StringDictionary unknownList1 = new StringDictionary();
    this.Load(inputStream, parameterFileFormat, unknownList1, respectAccessLevels);
    if (unknownList == null)
      return;
    foreach (DictionaryEntry dictionaryEntry in unknownList1)
      unknownList.Add(dictionaryEntry.Key.ToString());
  }

  public void Load(
    StreamReader inputStream,
    ParameterFileFormat parameterFileFormat,
    StringDictionary unknownList,
    bool respectAccessLevels)
  {
    this.ResetExceptions();
    this.vcpHelper.LoadFromStream(inputStream, parameterFileFormat, unknownList, respectAccessLevels);
  }

  public void Save(
    StreamWriter outputStream,
    ParameterFileFormat parameterFileFormat,
    bool respectAccessLevels)
  {
    if (parameterFileFormat != ParameterFileFormat.ParFile && parameterFileFormat != ParameterFileFormat.VerFile)
      throw new ArgumentException("File format not supported");
    this.vcpHelper.SaveToStream(outputStream, parameterFileFormat, respectAccessLevels, false);
  }

  public void SaveAccumulator(
    StreamWriter outputStream,
    ParameterFileFormat parameterFileFormat,
    bool respectAccessLevels)
  {
    if (parameterFileFormat != ParameterFileFormat.ParFile && parameterFileFormat != ParameterFileFormat.VerFile)
      throw new ArgumentException("File format not supported");
    this.vcpHelper.SaveToStream(outputStream, parameterFileFormat, respectAccessLevels, true);
  }

  public void ProcessVcp(string inputFile, string outputFile, bool synchronous)
  {
    if (this.channel.Extension != null)
      this.channel.Extension.PrepareVcp();
    this.vcpHelper.SetVcpStreams(new StreamReader(inputFile), new StreamWriter(outputFile));
    this.channel.QueueAction((object) CommunicationsState.ProcessVcp, synchronous);
  }

  public void ProcessVcp(Stream inputStream, Stream outputStream, bool synchronous)
  {
    this.vcpHelper.SetVcpStreams(new StreamReader(inputStream), new StreamWriter(outputStream));
    this.channel.QueueAction((object) CommunicationsState.ProcessVcp, synchronous);
  }

  public Channel Channel => this.channel;

  public Parameter this[string qualifier]
  {
    get
    {
      if (this.cache == null)
        this.cache = this.DistinctBy<Parameter, string>((Func<Parameter, string>) (p => p.Qualifier.ToUpperInvariant())).ToDictionary<Parameter, string>((Func<Parameter, string>) (p => p.Qualifier), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.combinedCache == null)
        this.combinedCache = this.DistinctBy<Parameter, string>((Func<Parameter, string>) (p => p.CombinedQualifier.ToUpperInvariant())).ToDictionary<Parameter, string>((Func<Parameter, string>) (p => p.CombinedQualifier), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string key;
      bool flag = this.channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out key);
      Dictionary<string, Parameter> dictionary = qualifier.Contains<char>('.') ? this.combinedCache : this.cache;
      Parameter parameter;
      return dictionary.TryGetValue(qualifier, out parameter) || flag && dictionary.TryGetValue(key, out parameter) ? parameter : (Parameter) null;
    }
  }

  public Parameter GetItemByName(string index)
  {
    return this.Where<Parameter>((Func<Parameter, bool>) (item => string.Equals(item.Name, index, StringComparison.Ordinal))).FirstOrDefault<Parameter>();
  }

  public Parameter GetItemFirstInGroup(string groupQualifier)
  {
    return this.Where<Parameter>((Func<Parameter, bool>) (parameter => string.Equals(groupQualifier, parameter.GroupQualifier, StringComparison.Ordinal))).FirstOrDefault<Parameter>();
  }

  public Parameter GetItemLastInGroup(string groupQualifier)
  {
    return this.Where<Parameter>((Func<Parameter, bool>) (parameter => string.Equals(groupQualifier, parameter.GroupQualifier, StringComparison.Ordinal))).LastOrDefault<Parameter>();
  }

  public float Progress => this.progress;

  public bool HaveBeenReadFromEcu => this.haveBeenReadFromEcu;

  public bool ValuesLoadedFromLog { internal set; get; }

  public VcpComponentError VcpComponentError => this.vcpHelper.ComponentError;

  public bool VcpHadParameterError => this.vcpHelper.HadParameterError;

  public StringDictionary GroupCodingStrings
  {
    get
    {
      bool flag;
      lock (this.groupCodingStringsStateLock)
        flag = this.groupCodingStringsState.Any<KeyValuePair<string, CodingStringState>>((Func<KeyValuePair<string, CodingStringState>, bool>) (kv => (kv.Value & CodingStringState.NeedsUpdate) != 0));
      if (flag)
        this.UpdateCodingStrings();
      return this.groupCodingStrings;
    }
  }

  public StringDictionary OriginalGroupCodingStrings => this.originalGroupCodingStrings;

  public bool IsCodingStringAssignedByClient(string groupQualifier)
  {
    return this.IsCodingStringState(groupQualifier, CodingStringState.AssignedByClient);
  }

  internal bool IsCodingStringState(string groupQualifier, CodingStringState targetState)
  {
    lock (this.groupCodingStringsStateLock)
      return (this.groupCodingStringsState[groupQualifier] & targetState) != 0;
  }

  public bool VerifyAfterWrite
  {
    get => this.verifyAfterWrite;
    set => this.verifyAfterWrite = value;
  }

  public bool VerifyAfterCommit => this.verifyAfterCommit;

  public bool SerializeGroupNames => this.serializeGroupNames;

  public event ParameterUpdateEventHandler ParameterUpdateEvent;

  public event ParametersReadCompleteEventHandler ParametersReadCompleteEvent;

  public event ParametersWriteCompleteEventHandler ParametersWriteCompleteEvent;

  public event ParametersProcessVcpCompleteEventHandler ParametersProcessVcpCompleteEvent;

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  public static TargetEcuDetails GetTargetEcuDetails(
    string path,
    ParameterFileFormat parameterFileFormat)
  {
    StreamReader streamReader = new StreamReader(path);
    try
    {
      return ParameterCollection.GetTargetEcuDetails(streamReader, parameterFileFormat);
    }
    finally
    {
      streamReader.Close();
    }
  }

  public static TargetEcuDetails GetTargetEcuDetails(
    StreamReader streamReader,
    ParameterFileFormat parameterFileFormat)
  {
    string targetEcu = VcpHelper.GetIdentificationRecordValue("ECU", streamReader);
    string identificationRecordValue = VcpHelper.GetIdentificationRecordValue("DIAGNOSISVARIANT", streamReader);
    string empty = string.Empty;
    int assumedUnknownCount = int.MaxValue;
    Sapi sapi = Sapi.GetSapi();
    Ecu ecu = sapi.Ecus[targetEcu];
    if (ecu != null)
    {
      DiagnosisVariant diagnosisVariant1 = ecu.DiagnosisVariants[identificationRecordValue];
      Ecu ecu1 = (Ecu) null;
      if (diagnosisVariant1 == null)
      {
        ecu1 = sapi.Ecus.FirstOrDefault<Ecu>((Func<Ecu, bool>) (e => e.Name == targetEcu && e != ecu));
        if (ecu1 != null)
          diagnosisVariant1 = ecu1.DiagnosisVariants[identificationRecordValue];
      }
      if (diagnosisVariant1 == null)
      {
        StringDictionary parameters = new StringDictionary();
        VcpHelper.LoadDictionaryFromStream(streamReader, parameterFileFormat, parameters);
        List<string> source = new List<string>();
        foreach (XmlNode xmlNode in ((IEnumerable<Ecu>) new Ecu[2]
        {
          ecu,
          ecu1
        }).Where<Ecu>((Func<Ecu, bool>) (e => e != null && e.Xml != null)).Select<Ecu, XmlNode>((Func<Ecu, XmlNode>) (e => e.Xml)))
        {
          XmlNodeList xmlNodeList = xmlNode.SelectNodes("Ecu/ServicesAsParameters/ServiceAsParameter");
          if (xmlNodeList != null)
          {
            for (int index = 0; index < xmlNodeList.Count; ++index)
            {
              string innerText = xmlNodeList.Item(index).Attributes.GetNamedItem("Qualifier").InnerText;
              source.Add(innerText);
            }
          }
        }
        List<DiagnosisVariant> diagnosisVariants = new List<DiagnosisVariant>(ecu.DiagnosisVariants.Where<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (v => !v.IsBase)));
        if (ecu1 != null)
          diagnosisVariants.AddRange(ecu1.DiagnosisVariants.Where<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (adv => !adv.IsBase && !diagnosisVariants.Any<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (dv =>
          {
            long? diagnosticVersionLong1 = dv.DiagnosticVersionLong;
            long? diagnosticVersionLong2 = adv.DiagnosticVersionLong;
            return diagnosticVersionLong1.GetValueOrDefault() == diagnosticVersionLong2.GetValueOrDefault() && diagnosticVersionLong1.HasValue == diagnosticVersionLong2.HasValue;
          })))));
        foreach (DiagnosisVariant diagnosisVariant2 in diagnosisVariants.Reverse<DiagnosisVariant>())
        {
          int num = 0;
          foreach (DictionaryEntry dictionaryEntry in parameters)
          {
            DictionaryEntry entry = dictionaryEntry;
            if (!diagnosisVariant2.ParameterQualifiers.Select<Tuple<string, string>, string>((Func<Tuple<string, string>, string>) (vq =>
            {
              if (((string) entry.Key).IndexOf('.') == -1)
                return vq.Item2;
              return string.Join(".", vq.Item1, vq.Item2);
            })).Contains<string>((string) entry.Key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && !source.Contains<string>((string) entry.Key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
              ++num;
          }
          Sapi.GetSapi().RaiseDebugInfoEvent((object) ecu, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Variant {0} has {1} mismatched parameters", (object) diagnosisVariant2.Name, (object) num));
          if (num < assumedUnknownCount)
          {
            assumedUnknownCount = num;
            empty = diagnosisVariant2.ToString();
            if (num == 0)
              break;
          }
        }
      }
    }
    return new TargetEcuDetails(targetEcu, identificationRecordValue, empty, assumedUnknownCount);
  }

  public static void LoadDictionaryFromStream(
    StreamReader stream,
    ParameterFileFormat parameterFileFormat,
    StringDictionary parameters)
  {
    VcpHelper.LoadDictionaryFromStream(stream, parameterFileFormat, parameters);
  }

  public static string GetIdentificationRecordValue(string recordName, StreamReader stream)
  {
    return VcpHelper.GetIdentificationRecordValue(recordName, stream);
  }

  public static string GetIdentificationRecordValue(string recordName, string path)
  {
    StreamReader stream = new StreamReader(path);
    try
    {
      return VcpHelper.GetIdentificationRecordValue(recordName, stream);
    }
    finally
    {
      stream.Close();
    }
  }

  private void InternalReadGroup(
    string groupQualifier,
    int startIndex,
    int endIndex,
    bool ignoreMarkedFlag)
  {
    if (!this[startIndex].ServiceAsParameter)
    {
      using (Varcode vh = this.channel.VCInit())
        this.InternalReadGroup(groupQualifier, startIndex, endIndex, ignoreMarkedFlag, vh);
    }
    else
      this.InternalReadGroup(groupQualifier, startIndex, endIndex, ignoreMarkedFlag, (Varcode) null);
  }

  private bool InternalReadGroup(
    string groupQualifier,
    int startIndex,
    int endIndex,
    bool ignoreMarkedFlag,
    Varcode vh)
  {
    bool serviceAsParameter = this[startIndex].ServiceAsParameter;
    if (vh != null | serviceAsParameter)
    {
      CaesarException e = (CaesarException) null;
      if (!serviceAsParameter)
        e = this.UpdateCodingString(groupQualifier, vh, CodingStringSource.FromEcu);
      for (int index = startIndex; index <= endIndex; ++index)
      {
        Parameter parameter = this[index];
        if (!parameter.HasBeenReadFromEcu && (ignoreMarkedFlag || parameter.Marked))
        {
          if (e == null)
            parameter.InternalRead(vh, true);
          else
            parameter.RaiseParameterUpdateEvent((Exception) e);
        }
      }
      if (e != null && !this.channel.ChannelRunning)
        throw e;
      return e == null;
    }
    if (this.channel.IsChannelErrorSet)
      throw new CaesarException(this.channel.ChannelHandle);
    return false;
  }

  private void UpdateHaveBeenReadFromEcuFlag()
  {
    this.haveBeenReadFromEcu = true;
    for (int index = 0; index < this.Count; ++index)
    {
      Parameter parameter = this[index];
      if ((Sapi.GetSapi().HardwareAccess >= parameter.ReadAccess || Sapi.GetSapi().InitState == InitState.Offline) && !parameter.HasBeenReadFromEcu && parameter.Exception == null)
      {
        this.haveBeenReadFromEcu = false;
        break;
      }
    }
  }

  internal CaesarException UpdateCodingString(
    string groupQualifier,
    Varcode varcode,
    CodingStringSource from)
  {
    string source = (string) null;
    CaesarException caesarException = (CaesarException) null;
    try
    {
      source = new Dump((IEnumerable<byte>) varcode.GetCurrentCodingString(groupQualifier)).ToString();
    }
    catch (NullReferenceException ex)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Intentional catch of exception from VCCurrentCodingString. Comms failure?");
    }
    if (varcode.IsErrorSet)
      caesarException = varcode.Exception;
    this.groupCodingStrings[groupQualifier] = source;
    switch (from)
    {
      case CodingStringSource.FromParameterUpdate:
      case CodingStringSource.FromIncompleteParameterUpdate:
      case CodingStringSource.FromDefaultString:
        lock (this.groupCodingStringsStateLock)
        {
          this.groupCodingStringsState[groupQualifier] &= ~CodingStringState.NeedsUpdate;
          if (from == CodingStringSource.FromIncompleteParameterUpdate)
          {
            this.groupCodingStringsState[groupQualifier] |= CodingStringState.Incomplete;
            break;
          }
          this.groupCodingStringsState[groupQualifier] &= ~CodingStringState.Incomplete;
          if (from == CodingStringSource.FromDefaultString)
          {
            this.groupCodingStringsState[groupQualifier] |= CodingStringState.AssignedByClient;
            break;
          }
          break;
        }
      case CodingStringSource.FromEcu:
      case CodingStringSource.FromLogFile:
        this.originalGroupCodingStrings[groupQualifier] = source;
        lock (this.groupCodingStringsStateLock)
          this.groupCodingStringsState[groupQualifier] &= ~(CodingStringState.AssignedByClient | CodingStringState.Incomplete);
        if (from == CodingStringSource.FromEcu)
        {
          this.channel.ParameterGroups[groupQualifier].CodingStringValues.Add(new CodingStringValue(new Dump(source), Sapi.Now));
          break;
        }
        break;
    }
    return caesarException;
  }

  private void UpdateCodingStrings()
  {
    lock (this.channel.OfflineVarcodingHandleLock)
    {
      Varcode offlineVarcodingHandle = this.channel.OfflineVarcodingHandle;
      if (offlineVarcodingHandle == null)
        return;
      foreach (IGrouping<string, Parameter> source in this.Where<Parameter>((Func<Parameter, bool>) (p => !p.ServiceAsParameter)).GroupBy<Parameter, string>((Func<Parameter, string>) (p => p.GroupQualifier)))
      {
        bool flag;
        lock (this.groupCodingStringsStateLock)
          flag = (this.groupCodingStringsState[source.Key] & CodingStringState.NeedsUpdate) != 0;
        if (flag)
        {
          string groupCodingString = !this.IsCodingStringState(source.Key, CodingStringState.Incomplete) ? this.groupCodingStrings[source.Key] : (string) null;
          if (!string.IsNullOrEmpty(groupCodingString))
            offlineVarcodingHandle.SetCurrentCodingString(source.Key, new Dump(groupCodingString).Data.ToArray<byte>());
          foreach (Parameter parameter in (IEnumerable<Parameter>) source)
          {
            if (parameter.Value != null)
              parameter.InternalWrite(offlineVarcodingHandle);
          }
          this.UpdateCodingString(source.Key, offlineVarcodingHandle, !string.IsNullOrEmpty(groupCodingString) || source.All<Parameter>((Func<Parameter, bool>) (p => p.Value != null)) && this.channel.ParameterGroups[source.Key].ParametersCoverGroup ? CodingStringSource.FromParameterUpdate : CodingStringSource.FromIncompleteParameterUpdate);
        }
      }
    }
  }

  internal void UpdateGroupCodingStringFromLogFile(ParameterGroup group)
  {
    this.groupCodingStrings[group.Qualifier] = this.originalGroupCodingStrings[group.Qualifier] = (string) null;
    lock (this.channel.OfflineVarcodingHandleLock)
    {
      Varcode offlineVarcodingHandle = this.channel.OfflineVarcodingHandle;
      if (offlineVarcodingHandle == null || group.ServiceAsParameter)
        return;
      int? groupLength = group.GroupLength;
      if (!groupLength.HasValue)
        return;
      Varcode varcode = offlineVarcodingHandle;
      string qualifier = group.Qualifier;
      CodingStringValue current = group.CodingStringValues.Current;
      byte[] content;
      if (current == null)
      {
        content = (byte[]) null;
      }
      else
      {
        Dump dump = current.Value;
        if (dump == null)
        {
          content = (byte[]) null;
        }
        else
        {
          IList<byte> data = dump.Data;
          content = data != null ? data.ToArray<byte>() : (byte[]) null;
        }
      }
      if (content == null)
      {
        groupLength = group.GroupLength;
        content = new byte[groupLength.Value];
      }
      varcode.SetCurrentCodingString(qualifier, content);
      foreach (Parameter parameter in group.Parameters.Where<Parameter>((Func<Parameter, bool>) (p => p.Value != null)))
        parameter.InternalWrite(offlineVarcodingHandle, false);
      this.UpdateCodingString(group.Qualifier, offlineVarcodingHandle, CodingStringSource.FromLogFile);
      this.channel.CodingParameterGroups[group.Qualifier]?.AcquireDefaultStringandFragmentChoicesForCoding(this.groupCodingStrings[group.Qualifier]);
    }
  }

  private void Dispose(bool disposing)
  {
    if (!this.disposed && disposing && this.vcpHelper != null)
    {
      this.vcpHelper.Dispose();
      this.vcpHelper = (VcpHelper) null;
    }
    this.disposed = true;
  }

  private bool IsAccumulatorGroupQualifier(string groupQualifierString)
  {
    for (int index = 0; index < this.accumulatorPrefixes.Count; ++index)
    {
      if (Regex.Match(groupQualifierString, this.accumulatorPrefixes[index]).Success)
        return true;
    }
    return false;
  }

  private void CheckAndAddParameterGroup(
    string groupQualifier,
    ref uint nParameterIndex,
    McdDBService readPrimitive,
    McdDBService writePrimitive)
  {
    Parameter parameter = (Parameter) null;
    bool persistable = !this.IsAccumulatorGroupQualifier(groupQualifier);
    string caesarEquivalentName = McdCaesarEquivalence.GetCaesarEquivalentName((McdDBDiagComPrimitive) readPrimitive);
    bool flag = this.channel.Ecu.IgnoreQualifier(groupQualifier);
    Service readService = new Service(this.channel, ServiceTypes.ReadVarCode, "RVC_" + McdCaesarEquivalence.MakeQualifier(readPrimitive.Name));
    readService.Acquire(readPrimitive.Name, (McdDBDiagComPrimitive) readPrimitive, (IEnumerable<McdDBResponseParameter>) null, readPrimitive.SpecialData);
    Service writeService = new Service(this.channel, ServiceTypes.WriteVarCode, "WVC_" + McdCaesarEquivalence.MakeQualifier(writePrimitive.Name));
    writeService.Acquire(writePrimitive.Name, (McdDBDiagComPrimitive) writePrimitive, (IEnumerable<McdDBResponseParameter>) null, writePrimitive.SpecialData);
    Dictionary<string, int> existingSet = new Dictionary<string, int>();
    foreach (ServiceInputValue inputValue in (ReadOnlyCollection<ServiceInputValue>) writeService.InputValues)
    {
      string str1 = inputValue.Name;
      int num = str1.IndexOf('[');
      if (num != -1 && (num % 2 == 1 || num % 2 == 5))
      {
        string[] strArray = str1.Substring(0, num).Split(" ".ToCharArray());
        if (strArray.Length == 2)
        {
          string str2 = !strArray[0].StartsWith("e2p_", StringComparison.Ordinal) || strArray[1].StartsWith("E2P_", StringComparison.Ordinal) ? strArray[0] : strArray[0].Substring(4);
          if (str2.Length == strArray[1].Length && str2.ToUpperInvariant() == strArray[1])
            str1 = strArray[1] + str1.Substring(num);
        }
      }
      string str3 = McdCaesarEquivalence.MakeQualifier(str1, existingSet, true);
      if (!inputValue.IsReserved && !this.channel.Ecu.IgnoreQualifier(str3) && !flag && inputValue.Type != (Type) null)
      {
        parameter = new Parameter(this.channel, nParameterIndex++, groupQualifier, caesarEquivalentName, persistable, this.Count);
        parameter.Acquire(str3, inputValue, str1, readService, writeService);
        this.Items.Add(parameter);
      }
      else
        ++nParameterIndex;
    }
    if (parameter == null)
      return;
    parameter.LastInGroup = true;
  }

  private void CheckAndAddParameterGroup(
    Varcode offlineVarcoding,
    CaesarDIVarCodeDom varcodeDom,
    string groupQualifierString,
    ref uint nParameterIndex,
    Service readService,
    Service writeService)
  {
    Parameter parameter = (Parameter) null;
    bool persistable = !this.IsAccumulatorGroupQualifier(groupQualifierString);
    bool flag = this.channel.Ecu.IgnoreQualifier(groupQualifierString);
    int defaultStringCount = (int) varcodeDom.DefaultStringCount;
    byte[] numArray = (byte[]) null;
    if ((uint) defaultStringCount > 0U)
      numArray = varcodeDom.GetDefaultString(0U);
    string longName = varcodeDom.LongName;
    uint varCodeFragCount = varcodeDom.VarCodeFragCount;
    if (offlineVarcoding != null && numArray != null)
      offlineVarcoding.SetCurrentCodingString(groupQualifierString, numArray);
    for (uint index = 0; index < varCodeFragCount; ++index)
    {
      using (CaesarDIVarCodeFrag varcodeFragment = varcodeDom.OpenVarCodeFrag(index))
      {
        if (varcodeFragment != null)
        {
          if (!this.channel.Ecu.IgnoreQualifier(varcodeFragment.Qualifier) && !flag)
          {
            parameter = new Parameter(this.channel, nParameterIndex++, groupQualifierString, longName, persistable, this.Count);
            parameter.Acquire(offlineVarcoding, varcodeFragment, numArray, readService, writeService);
            this.Items.Add(parameter);
          }
          else
            ++nParameterIndex;
        }
      }
    }
    if (parameter == null)
      return;
    parameter.LastInGroup = true;
  }

  private static string StripUnderscores(string source) => source.Replace("_", string.Empty);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_ItemByName is deprecated, please use GetItemByName(string) instead.")]
  public Parameter get_ItemByName(string index) => this.GetItemByName(index);

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_ItemFirstInGroup is deprecated, please use GetItemFirstInGroup(string) instead.")]
  public Parameter get_ItemFirstInGroup(string groupQualifier)
  {
    return this.GetItemFirstInGroup(groupQualifier);
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("get_ItemLastInGroup is deprecated, please use GetItemFirstInGroup(string) instead.")]
  public Parameter get_ItemLastInGroup(string groupQualifier)
  {
    return this.GetItemLastInGroup(groupQualifier);
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("Load(StreamReader, ParameterFileFormat, ArrayList, bool) is deprecated, please use Load(StreamReader, ParameterFileFormat, Collection<string>, bool) instead.")]
  public void Load(
    StreamReader inputStream,
    ParameterFileFormat parameterFileFormat,
    ArrayList unknownList,
    bool respectAccessLevels)
  {
    StringDictionary unknownList1 = new StringDictionary();
    this.Load(inputStream, parameterFileFormat, unknownList1, respectAccessLevels);
    if (unknownList == null)
      return;
    foreach (DictionaryEntry dictionaryEntry in unknownList1)
      unknownList.Add(dictionaryEntry.Key);
  }

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("Load(string, ParameterFileFormat, ArrayList) is deprecated, please use Load(string, ParameterFileFormat, Collection<string>) instead.")]
  public void Load(string path, ParameterFileFormat parameterFileFormat, ArrayList unknownList)
  {
    using (StreamReader inputStream = new StreamReader(path))
      this.Load(inputStream, parameterFileFormat, unknownList, true);
  }
}
