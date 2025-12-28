// Decompiled with JetBrains decompiler
// Type: SapiLayer1.VcpHelper
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#nullable disable
namespace SapiLayer1;

internal class VcpHelper : IDisposable
{
  private ParameterCollection parameters;
  private bool hadParameterError;
  private VcpComponentError componentError;
  private string componentErrorText;
  private StringBuilder parameterOutput;
  private StreamReader inputFile;
  private StreamWriter outputFile;
  private bool disposed;
  private string externalVcpPath;
  private string externalVcpAdditionalArguments;

  internal VcpHelper(ParameterCollection parent)
  {
    this.parameters = parent;
    this.componentErrorText = string.Empty;
    this.componentError = VcpComponentError.NoError;
    if (!this.parameters.Channel.IsRollCall)
      return;
    string name = this.parameters.Channel.Ecu.Properties.GetValue<string>("VcpPath", (string) null);
    if (name == null)
      return;
    this.externalVcpPath = Environment.ExpandEnvironmentVariables(name);
    this.externalVcpAdditionalArguments = this.parameters.Channel.Ecu.Properties.GetValue<string>("VcpAdditionalArguments", (string) null);
  }

  internal void LoadFromStream(
    StreamReader streamReader,
    ParameterFileFormat parameterFileFormat,
    StringDictionary unknownList,
    bool respectAccessLevels)
  {
    int hintIndex = 0;
    string identificationRecordValue = VcpHelper.GetIdentificationRecordValue("ECU", streamReader);
    if (!string.Equals(identificationRecordValue, this.parameters.Channel.Ecu.Name, StringComparison.Ordinal))
      throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Target Ecu ({0}) does not match Channel Ecu {1}", (object) identificationRecordValue, (object) this.parameters.Channel.Ecu.Name));
    while (streamReader.Peek() != -1)
    {
      string parameterRecord = streamReader.ReadLine();
      if (parameterRecord.StartsWith("P", StringComparison.OrdinalIgnoreCase))
      {
        string parameterQualifier = (string) null;
        VcpParameterAccess parameterAccess = VcpParameterAccess.Error;
        string parameterValue = (string) null;
        if (!VcpHelper.ValidateParameterRecord(parameterRecord, out parameterQualifier, out parameterAccess, out parameterValue, parameterFileFormat))
          throw new DataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid parameter record (not enough data) {0}", (object) parameterQualifier));
        if (parameterAccess == VcpParameterAccess.Both || parameterAccess == VcpParameterAccess.Write)
        {
          Parameter parameter = this.parameters.GetParameter(parameterQualifier, hintIndex);
          if (parameter != null)
          {
            parameter.InternalSetValue(parameterValue, respectAccessLevels);
            hintIndex = parameter.Index + 1;
          }
          else if (unknownList != null)
          {
            if (unknownList.ContainsKey(parameterQualifier))
              unknownList[parameterQualifier] = parameterValue;
            else
              unknownList.Add(parameterQualifier, parameterValue);
          }
        }
      }
    }
  }

  internal void SaveToStream(
    StreamWriter stream,
    ParameterFileFormat parameterFileFormat,
    bool respectAccessLevels,
    bool saveAccumulator)
  {
    this.outputFile = stream;
    this.parameterOutput = new StringBuilder();
    this.hadParameterError = false;
    this.componentError = VcpComponentError.NoError;
    this.componentErrorText = string.Empty;
    for (int index = 0; index < this.parameters.Count; ++index)
    {
      Parameter parameter = this.parameters[index];
      if (parameter.Marked && (!saveAccumulator || !parameter.Persistable) && (saveAccumulator || parameter.Persistable) && (!respectAccessLevels || parameter.ReadAccess <= Sapi.GetSapi().ReadAccess) && parameter.Value != null && parameter.Value != (object) ChoiceCollection.InvalidChoice)
      {
        if (parameterFileFormat == ParameterFileFormat.VerFile)
          this.AddParameterRecord(parameter);
        else
          this.parameterOutput.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "P,{0},B,{1}\r\n", this.parameters.SerializeGroupNames ? (object) parameter.CombinedQualifier : (object) parameter.Qualifier, (object) parameter.InternalGetValue());
      }
    }
    this.StreamFile(parameterFileFormat);
    this.outputFile = (StreamWriter) null;
  }

  internal void SetVcpStreams(StreamReader input, StreamWriter output)
  {
    this.inputFile = input;
    this.outputFile = output;
  }

  internal void Process()
  {
    List<Parameter> parameterList = new List<Parameter>();
    string str = string.Empty;
    this.componentError = VcpComponentError.NoError;
    this.componentErrorText = string.Empty;
    long length = this.inputFile.BaseStream.Length;
    this.parameterOutput = new StringBuilder();
    this.hadParameterError = false;
    try
    {
      this.parameters.ResetExceptions();
      for (int index = 0; index < this.parameters.Count; ++index)
      {
        Parameter parameter = this.parameters[index];
        parameter.ResetHasBeenReadFromEcu();
        parameter.Marked = true;
      }
      if (string.Equals(VcpHelper.GetIdentificationRecordValue("ECU", this.inputFile), this.parameters.Channel.Ecu.Name, StringComparison.Ordinal))
      {
        while (this.inputFile.Peek() != -1 && this.componentError == VcpComponentError.NoError)
        {
          string parameterRecord = this.inputFile.ReadLine();
          this.parameters.UpdateProgress(Convert.ToSingle((object) this.inputFile.BaseStream.Position, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToSingle((object) length, (IFormatProvider) CultureInfo.InvariantCulture));
          if (parameterRecord.StartsWith("P", StringComparison.OrdinalIgnoreCase))
          {
            string parameterQualifier = (string) null;
            VcpParameterAccess parameterAccess = VcpParameterAccess.Error;
            string parameterValue = (string) null;
            if (VcpHelper.ValidateParameterRecord(parameterRecord, out parameterQualifier, out parameterAccess, out parameterValue, ParameterFileFormat.ParFile))
            {
              if (parameterAccess != VcpParameterAccess.Error)
              {
                Parameter parameter = this.parameters.GetParameter(parameterQualifier, 0);
                if (parameter != null)
                {
                  if (!parameter.HasBeenReadFromEcu)
                  {
                    str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Read {0}", (object) parameter.GroupQualifier);
                    this.parameters.InternalReadGroupVcp(parameter.GroupQualifier);
                  }
                  if (parameterAccess == VcpParameterAccess.Both || parameterAccess == VcpParameterAccess.Write)
                    parameter.InternalSetValue(parameterValue, false);
                  parameterList.Add(parameter);
                }
                else
                  this.AddParameterRecord(parameterQualifier, (ushort) 3, string.Empty, string.Empty);
              }
              else
                this.AddParameterRecord(parameterQualifier, (ushort) 4, string.Empty, string.Empty);
            }
            else
              this.AddParameterRecord(parameterQualifier, (ushort) 7, string.Empty, string.Empty);
          }
          else if (parameterRecord.IndexOfAny("SsRrHhFf".ToCharArray()) != 0 && parameterRecord.Trim().Length > 0)
            this.AddParameterRecord(string.Empty, (ushort) 7, string.Empty, string.Empty);
          if (!this.parameters.Channel.ChannelRunning && this.componentError == VcpComponentError.NoError)
          {
            this.componentError = VcpComponentError.ToolFailure;
            this.componentErrorText = "Connection to device failed";
          }
        }
      }
      else
      {
        this.componentError = VcpComponentError.NoDefinitionFile;
        this.componentErrorText = "Parameter record but no valid setup record";
      }
      if (this.componentError != VcpComponentError.NoError)
        return;
      str = "Write";
      this.parameters.InternalWriteVcp();
      for (int index = 0; index < parameterList.Count; ++index)
        this.AddParameterRecord(parameterList[index]);
      this.parameters.Channel.ClearCache();
      this.parameters.Channel.EcuInfos.InternalRead(false);
    }
    catch (CaesarException ex)
    {
      this.componentError = VcpComponentError.ToolFailure;
      this.componentErrorText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", (object) str, (object) ex.Message);
    }
    finally
    {
      this.StreamFile(ParameterFileFormat.VerFile);
      this.inputFile.Close();
      this.outputFile.Close();
    }
  }

  internal static string GetIdentificationRecordValue(string recordName, StreamReader stream)
  {
    stream.BaseStream.Seek(0L, SeekOrigin.Begin);
    stream.DiscardBufferedData();
    while (stream.Peek() != -1)
    {
      string str = stream.ReadLine();
      if (str.StartsWith("S", StringComparison.OrdinalIgnoreCase) || str.StartsWith("H", StringComparison.OrdinalIgnoreCase))
      {
        string[] strArray = str.Split(",".ToCharArray());
        if (strArray.Length > 2 && string.Equals(strArray[1], recordName, StringComparison.OrdinalIgnoreCase))
          return strArray[2];
      }
    }
    return string.Empty;
  }

  internal static void LoadDictionaryFromStream(
    StreamReader stream,
    ParameterFileFormat format,
    StringDictionary parameters)
  {
    stream.BaseStream.Seek(0L, SeekOrigin.Begin);
    stream.DiscardBufferedData();
    while (stream.Peek() != -1)
    {
      string parameterRecord = stream.ReadLine();
      if (parameterRecord.StartsWith("P", StringComparison.OrdinalIgnoreCase))
      {
        string parameterQualifier = (string) null;
        VcpParameterAccess parameterAccess = VcpParameterAccess.Error;
        string parameterValue = (string) null;
        if (VcpHelper.ValidateParameterRecord(parameterRecord, out parameterQualifier, out parameterAccess, out parameterValue, format))
        {
          if (parameters.ContainsKey(parameterQualifier))
            parameters.Remove(parameterQualifier);
          parameters.Add(parameterQualifier, parameterValue);
        }
      }
    }
  }

  internal VcpComponentError ComponentError => this.componentError;

  internal bool HadParameterError => this.hadParameterError;

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private void Dispose(bool disposing)
  {
    if (!this.disposed && disposing)
    {
      if (this.inputFile != null)
      {
        this.inputFile.Close();
        this.inputFile = (StreamReader) null;
      }
      if (this.outputFile != null)
      {
        this.outputFile.Close();
        this.outputFile = (StreamWriter) null;
      }
    }
    this.disposed = true;
  }

  private void StreamFile(ParameterFileFormat parameterFileFormat)
  {
    if (parameterFileFormat == ParameterFileFormat.VerFile)
    {
      this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "S,CERROR,{0},{1}", (object) (ushort) this.componentError, (object) this.componentErrorText));
      if (this.hadParameterError)
        this.outputFile.WriteLine("S,PERROR,T");
      else
        this.outputFile.WriteLine("S,PERROR,F");
    }
    string str = parameterFileFormat != ParameterFileFormat.ParFile ? "H" : "S";
    this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},ECU,{1}", (object) str, (object) this.parameters.Channel.Ecu.Name));
    this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},DIAGNOSISVARIANT,{1}", (object) str, (object) this.parameters.Channel.DiagnosisVariant.Name));
    this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "H,APPNAME,{0}", (object) Assembly.GetEntryAssembly().GetName().Name.ToString()));
    this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "H,APPVERSION,{0}", (object) Assembly.GetEntryAssembly().GetName().Version.ToString()));
    this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "H,SAPIVERSION,{0}", (object) Assembly.GetExecutingAssembly().GetName().Version.ToString()));
    this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "H,CBFVERSION,{0}", (object) this.parameters.Channel.Ecu.DescriptionDataVersion));
    this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "H,TIMESTAMP,{0}", (object) Sapi.TimeToString(this.parameters.Channel.LogFile != null ? this.parameters.Channel.LogFile.CurrentTime : Sapi.Now)));
    this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "H,UTCOFFSET,{0}", (object) TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString()));
    if (parameterFileFormat == ParameterFileFormat.VerFile && this.componentError == VcpComponentError.NoError)
    {
      for (int index = 0; index < this.parameters.Channel.EcuInfos.Count; ++index)
      {
        EcuInfo ecuInfo = this.parameters.Channel.EcuInfos[index];
        if (ecuInfo.Value.Length > 0 && ecuInfo.Common)
          this.outputFile.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "H,{0},{1}", (object) ecuInfo.Qualifier, (object) ecuInfo.Value));
      }
    }
    this.outputFile.Write(this.parameterOutput.ToString());
  }

  private void AddParameterRecord(
    string parameterQualifier,
    ushort errorCode,
    string parameterValue,
    string remark)
  {
    this.parameterOutput.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "P,{0},{1:0000},{2},{3}\r\n", (object) parameterQualifier, (object) errorCode, (object) parameterValue, (object) remark);
    if (errorCode == (ushort) 0)
      return;
    this.hadParameterError = true;
  }

  private void AddParameterRecord(Parameter parameter)
  {
    bool flag = false;
    string parameterQualifier = this.parameters.SerializeGroupNames ? parameter.CombinedQualifier : parameter.Qualifier;
    if (parameter.Exception != null)
    {
      flag = true;
      if (parameter.Exception.GetType() == typeof (CaesarException))
      {
        CaesarException exception = (CaesarException) parameter.Exception;
        if (exception.ErrorNumber == 6059L)
          this.AddParameterRecord(parameterQualifier, (ushort) 2, string.Empty, string.Empty);
        else if (exception.ErrorNumber == 6602L)
          flag = false;
        else
          this.AddParameterRecord(parameterQualifier, (ushort) 6, string.Empty, parameter.Exception.Message);
      }
      else if (parameter.Exception.GetType() == typeof (OverflowException))
        this.AddParameterRecord(parameterQualifier, (ushort) 2, string.Empty, parameter.Exception.Message);
      else
        this.AddParameterRecord(parameterQualifier, (ushort) 6, string.Empty, parameter.Exception.Message);
      parameter.ResetException();
    }
    if (flag)
      return;
    if (parameter.Value != null && parameter.Value != (object) ChoiceCollection.InvalidChoice)
      this.AddParameterRecord(parameterQualifier, (ushort) 0, parameter.InternalGetValue(), string.Empty);
    else
      this.AddParameterRecord(parameterQualifier, (ushort) 0, string.Empty, string.Empty);
  }

  private static string[] SplitParameterRecord(string parameterRecord)
  {
    using (StringReader reader = new StringReader(parameterRecord))
    {
      using (TextFieldParser textFieldParser = new TextFieldParser((TextReader) reader))
      {
        textFieldParser.HasFieldsEnclosedInQuotes = true;
        textFieldParser.SetDelimiters(",");
        textFieldParser.TrimWhiteSpace = false;
        try
        {
          return textFieldParser.ReadFields();
        }
        catch (MalformedLineException ex)
        {
          return parameterRecord.Split(",".ToCharArray());
        }
      }
    }
  }

  private static bool ValidateParameterRecord(
    string parameterRecord,
    out string parameterQualifier,
    out VcpParameterAccess parameterAccess,
    out string parameterValue,
    ParameterFileFormat parameterFileFormat)
  {
    string[] source = VcpHelper.SplitParameterRecord(parameterRecord);
    parameterQualifier = string.Empty;
    parameterValue = string.Empty;
    parameterAccess = VcpParameterAccess.Error;
    if (source.Length > 1)
    {
      parameterQualifier = source[1].Trim();
      if (parameterFileFormat == ParameterFileFormat.ParFile)
      {
        if (source.Length > 2)
        {
          parameterAccess = VcpHelper.GetParameterAccess(source[2].Trim());
          if (source.Length > 3)
            parameterValue = source.Length <= 4 ? source[3] : string.Join(",", ((IEnumerable<string>) source).Skip<string>(3).ToArray<string>());
          return true;
        }
      }
      else
      {
        if (source.Length > 3)
        {
          if (Convert.ToUInt16(source[2], (IFormatProvider) CultureInfo.InvariantCulture) == (ushort) 0)
          {
            parameterAccess = VcpParameterAccess.Both;
            parameterValue = source.Length <= 5 ? source[3] : string.Join(",", ((IEnumerable<string>) source).Skip<string>(3).Take<string>(source.Length - 4).ToArray<string>());
          }
          return true;
        }
        parameterAccess = VcpParameterAccess.Error;
      }
    }
    return false;
  }

  private static VcpParameterAccess GetParameterAccess(string parameterAccess)
  {
    if (parameterAccess != null && parameterAccess.Length > 0)
    {
      parameterAccess = parameterAccess.ToUpper(CultureInfo.CurrentCulture);
      switch (parameterAccess[0])
      {
        case 'B':
          return VcpParameterAccess.Both;
        case 'R':
          return VcpParameterAccess.Read;
        case 'W':
          return VcpParameterAccess.Write;
      }
    }
    return VcpParameterAccess.Error;
  }

  internal bool HasExternalVcp => this.externalVcpPath != null;

  internal void ProcessExternalRead()
  {
    this.ProcessExternal(this.parameters.Where<Parameter>((System.Func<Parameter, bool>) (parameter => parameter.ReadAccess <= Sapi.GetSapi().ReadAccess && parameter.Marked)), (System.Func<Parameter, string>) (parameter => $"P,{parameter.Qualifier},R,,"));
  }

  internal void ProcessExternalWrite()
  {
    this.ProcessExternal(this.parameters.Where<Parameter>((System.Func<Parameter, bool>) (parameter => parameter.WriteAccess <= Sapi.GetSapi().WriteAccess && parameter.Marked && !object.Equals(parameter.OriginalValue, parameter.Value))), (System.Func<Parameter, string>) (parameter => $"P,{parameter.Qualifier},B,{parameter.Value},"));
  }

  private void ProcessExternal(
    IEnumerable<Parameter> parameters,
    System.Func<Parameter, string> recordFunc)
  {
    if (!File.Exists(this.externalVcpPath))
      throw new CaesarException(SapiError.ExternalVcpNotFound);
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("S,COMSWINT,XXXX,");
    stringBuilder.AppendLine($"S,ECU,{this.parameters.Channel.Ecu.Name},");
    foreach (Parameter parameter in parameters)
      stringBuilder.AppendLine(recordFunc(parameter));
    string tempPath1 = Path.GetTempPath();
    Guid guid = Guid.NewGuid();
    string path2_1 = Path.ChangeExtension(guid.ToString(), ".PAR");
    string str1 = Path.Combine(tempPath1, path2_1);
    string tempPath2 = Path.GetTempPath();
    guid = Guid.NewGuid();
    string path2_2 = Path.ChangeExtension(guid.ToString(), ".VER");
    string str2 = Path.Combine(tempPath2, path2_2);
    File.WriteAllText(str1, stringBuilder.ToString(), Encoding.UTF8);
    this.ProcessExternal(str1, str2);
    TextFieldParser textFieldParser = File.Exists(str2) ? new TextFieldParser(str2) : throw new CaesarException(SapiError.ExternalVcpVerFileNotCreated);
    textFieldParser.SetDelimiters(",");
    textFieldParser.HasFieldsEnclosedInQuotes = true;
    while (!textFieldParser.EndOfData)
    {
      string[] fields = textFieldParser.ReadFields();
      switch (fields[0])
      {
        case "S":
          switch (fields[1])
          {
            case "PERROR":
              if (!(fields[2] == "T"))
                continue;
              continue;
            case "CERROR":
              string str3 = fields[2];
              if (str3 != "1000")
                throw new CaesarException(SapiError.ExternalVcpComponentError, $"{str3} - {(fields.Length > 2 ? fields[3] : string.Empty)}");
              continue;
            default:
              continue;
          }
        case "P":
          if (fields.Length >= 5)
          {
            Parameter parameter = this.parameters[fields[1]];
            if (parameter != null)
            {
              parameter.InternalRead(fields);
              continue;
            }
            continue;
          }
          continue;
        default:
          continue;
      }
    }
  }

  private void ProcessExternal(string tempInPath, string tempOutPath)
  {
    ProcessStartInfo startInfo = new ProcessStartInfo();
    startInfo.FileName = this.externalVcpPath;
    startInfo.Arguments = $"-p {tempInPath} -v {tempOutPath}";
    if (this.externalVcpAdditionalArguments != null)
    {
      if (this.externalVcpAdditionalArguments.StartsWith("extension:", StringComparison.OrdinalIgnoreCase))
      {
        string str = (string) this.parameters.Channel.Extension.Invoke(this.externalVcpAdditionalArguments.Substring(10), new object[0]);
        ProcessStartInfo processStartInfo = startInfo;
        processStartInfo.Arguments = $"{processStartInfo.Arguments} {str}";
      }
      else
      {
        ProcessStartInfo processStartInfo = startInfo;
        processStartInfo.Arguments = $"{processStartInfo.Arguments} {this.externalVcpAdditionalArguments}";
      }
    }
    try
    {
      System.Diagnostics.Process.Start(startInfo).WaitForExit();
    }
    catch (Win32Exception ex)
    {
      throw new CaesarException(SapiError.ExternalVcpExecutionError, ex.Message + Environment.NewLine + startInfo.FileName);
    }
  }
}
