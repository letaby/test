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
using Microsoft.VisualBasic.FileIO;

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

	internal VcpComponentError ComponentError => componentError;

	internal bool HadParameterError => hadParameterError;

	internal bool HasExternalVcp => externalVcpPath != null;

	internal VcpHelper(ParameterCollection parent)
	{
		parameters = parent;
		componentErrorText = string.Empty;
		componentError = VcpComponentError.NoError;
		if (parameters.Channel.IsRollCall)
		{
			string value = parameters.Channel.Ecu.Properties.GetValue<string>("VcpPath", null);
			if (value != null)
			{
				externalVcpPath = Environment.ExpandEnvironmentVariables(value);
				externalVcpAdditionalArguments = parameters.Channel.Ecu.Properties.GetValue<string>("VcpAdditionalArguments", null);
			}
		}
	}

	internal void LoadFromStream(StreamReader streamReader, ParameterFileFormat parameterFileFormat, StringDictionary unknownList, bool respectAccessLevels)
	{
		int hintIndex = 0;
		string identificationRecordValue = GetIdentificationRecordValue("ECU", streamReader);
		if (string.Equals(identificationRecordValue, parameters.Channel.Ecu.Name, StringComparison.Ordinal))
		{
			while (streamReader.Peek() != -1)
			{
				string text = streamReader.ReadLine();
				if (!text.StartsWith("P", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				string parameterQualifier = null;
				VcpParameterAccess parameterAccess = VcpParameterAccess.Error;
				string parameterValue = null;
				if (ValidateParameterRecord(text, out parameterQualifier, out parameterAccess, out parameterValue, parameterFileFormat))
				{
					if (parameterAccess != VcpParameterAccess.Both && parameterAccess != VcpParameterAccess.Write)
					{
						continue;
					}
					Parameter parameter = parameters.GetParameter(parameterQualifier, hintIndex);
					if (parameter != null)
					{
						parameter.InternalSetValue(parameterValue, respectAccessLevels);
						hintIndex = parameter.Index + 1;
					}
					else if (unknownList != null)
					{
						if (unknownList.ContainsKey(parameterQualifier))
						{
							unknownList[parameterQualifier] = parameterValue;
						}
						else
						{
							unknownList.Add(parameterQualifier, parameterValue);
						}
					}
					continue;
				}
				throw new DataException(string.Format(CultureInfo.InvariantCulture, "Invalid parameter record (not enough data) {0}", parameterQualifier));
			}
			return;
		}
		throw new DataException(string.Format(CultureInfo.InvariantCulture, "Target Ecu ({0}) does not match Channel Ecu {1}", identificationRecordValue, parameters.Channel.Ecu.Name));
	}

	internal void SaveToStream(StreamWriter stream, ParameterFileFormat parameterFileFormat, bool respectAccessLevels, bool saveAccumulator)
	{
		outputFile = stream;
		parameterOutput = new StringBuilder();
		hadParameterError = false;
		componentError = VcpComponentError.NoError;
		componentErrorText = string.Empty;
		for (int i = 0; i < parameters.Count; i++)
		{
			Parameter parameter = parameters[i];
			if (parameter.Marked && (!saveAccumulator || !parameter.Persistable) && (saveAccumulator || parameter.Persistable) && (!respectAccessLevels || parameter.ReadAccess <= Sapi.GetSapi().ReadAccess) && parameter.Value != null && parameter.Value != ChoiceCollection.InvalidChoice)
			{
				if (parameterFileFormat == ParameterFileFormat.VerFile)
				{
					AddParameterRecord(parameter);
				}
				else
				{
					parameterOutput.AppendFormat(CultureInfo.InvariantCulture, "P,{0},B,{1}\r\n", parameters.SerializeGroupNames ? parameter.CombinedQualifier : parameter.Qualifier, parameter.InternalGetValue());
				}
			}
		}
		StreamFile(parameterFileFormat);
		outputFile = null;
	}

	internal void SetVcpStreams(StreamReader input, StreamWriter output)
	{
		inputFile = input;
		outputFile = output;
	}

	internal void Process()
	{
		List<Parameter> list = new List<Parameter>();
		string arg = string.Empty;
		componentError = VcpComponentError.NoError;
		componentErrorText = string.Empty;
		long length = inputFile.BaseStream.Length;
		parameterOutput = new StringBuilder();
		hadParameterError = false;
		try
		{
			parameters.ResetExceptions();
			for (int i = 0; i < parameters.Count; i++)
			{
				Parameter parameter = parameters[i];
				parameter.ResetHasBeenReadFromEcu();
				parameter.Marked = true;
			}
			if (string.Equals(GetIdentificationRecordValue("ECU", inputFile), parameters.Channel.Ecu.Name, StringComparison.Ordinal))
			{
				while (inputFile.Peek() != -1 && componentError == VcpComponentError.NoError)
				{
					string text = inputFile.ReadLine();
					parameters.UpdateProgress(Convert.ToSingle(inputFile.BaseStream.Position, CultureInfo.InvariantCulture), Convert.ToSingle(length, CultureInfo.InvariantCulture));
					if (text.StartsWith("P", StringComparison.OrdinalIgnoreCase))
					{
						string parameterQualifier = null;
						VcpParameterAccess parameterAccess = VcpParameterAccess.Error;
						string parameterValue = null;
						if (ValidateParameterRecord(text, out parameterQualifier, out parameterAccess, out parameterValue, ParameterFileFormat.ParFile))
						{
							if (parameterAccess != VcpParameterAccess.Error)
							{
								Parameter parameter2 = parameters.GetParameter(parameterQualifier, 0);
								if (parameter2 != null)
								{
									if (!parameter2.HasBeenReadFromEcu)
									{
										arg = string.Format(CultureInfo.InvariantCulture, "Read {0}", parameter2.GroupQualifier);
										parameters.InternalReadGroupVcp(parameter2.GroupQualifier);
									}
									if (parameterAccess == VcpParameterAccess.Both || parameterAccess == VcpParameterAccess.Write)
									{
										parameter2.InternalSetValue(parameterValue, respectAccessLevel: false);
									}
									list.Add(parameter2);
								}
								else
								{
									AddParameterRecord(parameterQualifier, 3, string.Empty, string.Empty);
								}
							}
							else
							{
								AddParameterRecord(parameterQualifier, 4, string.Empty, string.Empty);
							}
						}
						else
						{
							AddParameterRecord(parameterQualifier, 7, string.Empty, string.Empty);
						}
					}
					else if (text.IndexOfAny("SsRrHhFf".ToCharArray()) != 0 && text.Trim().Length > 0)
					{
						AddParameterRecord(string.Empty, 7, string.Empty, string.Empty);
					}
					if (!parameters.Channel.ChannelRunning && componentError == VcpComponentError.NoError)
					{
						componentError = VcpComponentError.ToolFailure;
						componentErrorText = "Connection to device failed";
					}
				}
			}
			else
			{
				componentError = VcpComponentError.NoDefinitionFile;
				componentErrorText = "Parameter record but no valid setup record";
			}
			if (componentError == VcpComponentError.NoError)
			{
				arg = "Write";
				parameters.InternalWriteVcp();
				for (int j = 0; j < list.Count; j++)
				{
					Parameter parameter3 = list[j];
					AddParameterRecord(parameter3);
				}
				parameters.Channel.ClearCache();
				parameters.Channel.EcuInfos.InternalRead(explicitread: false);
			}
		}
		catch (CaesarException ex)
		{
			componentError = VcpComponentError.ToolFailure;
			componentErrorText = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", arg, ex.Message);
		}
		finally
		{
			StreamFile(ParameterFileFormat.VerFile);
			inputFile.Close();
			outputFile.Close();
		}
	}

	internal static string GetIdentificationRecordValue(string recordName, StreamReader stream)
	{
		stream.BaseStream.Seek(0L, SeekOrigin.Begin);
		stream.DiscardBufferedData();
		while (stream.Peek() != -1)
		{
			string text = stream.ReadLine();
			if (text.StartsWith("S", StringComparison.OrdinalIgnoreCase) || text.StartsWith("H", StringComparison.OrdinalIgnoreCase))
			{
				string[] array = text.Split(",".ToCharArray());
				if (array.Length > 2 && string.Equals(array[1], recordName, StringComparison.OrdinalIgnoreCase))
				{
					return array[2];
				}
			}
		}
		return string.Empty;
	}

	internal static void LoadDictionaryFromStream(StreamReader stream, ParameterFileFormat format, StringDictionary parameters)
	{
		stream.BaseStream.Seek(0L, SeekOrigin.Begin);
		stream.DiscardBufferedData();
		while (stream.Peek() != -1)
		{
			string text = stream.ReadLine();
			if (!text.StartsWith("P", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			string parameterQualifier = null;
			VcpParameterAccess parameterAccess = VcpParameterAccess.Error;
			string parameterValue = null;
			if (ValidateParameterRecord(text, out parameterQualifier, out parameterAccess, out parameterValue, format))
			{
				if (parameters.ContainsKey(parameterQualifier))
				{
					parameters.Remove(parameterQualifier);
				}
				parameters.Add(parameterQualifier, parameterValue);
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			if (inputFile != null)
			{
				inputFile.Close();
				inputFile = null;
			}
			if (outputFile != null)
			{
				outputFile.Close();
				outputFile = null;
			}
		}
		disposed = true;
	}

	private void StreamFile(ParameterFileFormat parameterFileFormat)
	{
		if (parameterFileFormat == ParameterFileFormat.VerFile)
		{
			outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "S,CERROR,{0},{1}", (ushort)componentError, componentErrorText));
			if (hadParameterError)
			{
				outputFile.WriteLine("S,PERROR,T");
			}
			else
			{
				outputFile.WriteLine("S,PERROR,F");
			}
		}
		string arg = ((parameterFileFormat != ParameterFileFormat.ParFile) ? "H" : "S");
		outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0},ECU,{1}", arg, parameters.Channel.Ecu.Name));
		outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0},DIAGNOSISVARIANT,{1}", arg, parameters.Channel.DiagnosisVariant.Name));
		outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "H,APPNAME,{0}", Assembly.GetEntryAssembly().GetName().Name.ToString()));
		outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "H,APPVERSION,{0}", Assembly.GetEntryAssembly().GetName().Version.ToString()));
		outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "H,SAPIVERSION,{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
		outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "H,CBFVERSION,{0}", parameters.Channel.Ecu.DescriptionDataVersion));
		outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "H,TIMESTAMP,{0}", Sapi.TimeToString((parameters.Channel.LogFile != null) ? parameters.Channel.LogFile.CurrentTime : Sapi.Now)));
		outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "H,UTCOFFSET,{0}", TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString()));
		if (parameterFileFormat == ParameterFileFormat.VerFile && componentError == VcpComponentError.NoError)
		{
			for (int i = 0; i < parameters.Channel.EcuInfos.Count; i++)
			{
				EcuInfo ecuInfo = parameters.Channel.EcuInfos[i];
				if (ecuInfo.Value.Length > 0 && ecuInfo.Common)
				{
					outputFile.WriteLine(string.Format(CultureInfo.InvariantCulture, "H,{0},{1}", ecuInfo.Qualifier, ecuInfo.Value));
				}
			}
		}
		outputFile.Write(parameterOutput.ToString());
	}

	private void AddParameterRecord(string parameterQualifier, ushort errorCode, string parameterValue, string remark)
	{
		object[] args = new object[4] { parameterQualifier, errorCode, parameterValue, remark };
		parameterOutput.AppendFormat(CultureInfo.InvariantCulture, "P,{0},{1:0000},{2},{3}\r\n", args);
		if (errorCode != 0)
		{
			hadParameterError = true;
		}
	}

	private void AddParameterRecord(Parameter parameter)
	{
		bool flag = false;
		string parameterQualifier = (parameters.SerializeGroupNames ? parameter.CombinedQualifier : parameter.Qualifier);
		if (parameter.Exception != null)
		{
			flag = true;
			if (parameter.Exception.GetType() == typeof(CaesarException))
			{
				CaesarException ex = (CaesarException)parameter.Exception;
				if (ex.ErrorNumber == 6059)
				{
					AddParameterRecord(parameterQualifier, 2, string.Empty, string.Empty);
				}
				else if (ex.ErrorNumber == 6602)
				{
					flag = false;
				}
				else
				{
					AddParameterRecord(parameterQualifier, 6, string.Empty, parameter.Exception.Message);
				}
			}
			else if (parameter.Exception.GetType() == typeof(OverflowException))
			{
				AddParameterRecord(parameterQualifier, 2, string.Empty, parameter.Exception.Message);
			}
			else
			{
				AddParameterRecord(parameterQualifier, 6, string.Empty, parameter.Exception.Message);
			}
			parameter.ResetException();
		}
		if (!flag)
		{
			if (parameter.Value != null && parameter.Value != ChoiceCollection.InvalidChoice)
			{
				AddParameterRecord(parameterQualifier, 0, parameter.InternalGetValue(), string.Empty);
			}
			else
			{
				AddParameterRecord(parameterQualifier, 0, string.Empty, string.Empty);
			}
		}
	}

	private static string[] SplitParameterRecord(string parameterRecord)
	{
		using StringReader reader = new StringReader(parameterRecord);
		using TextFieldParser textFieldParser = new TextFieldParser(reader);
		textFieldParser.HasFieldsEnclosedInQuotes = true;
		textFieldParser.SetDelimiters(",");
		textFieldParser.TrimWhiteSpace = false;
		try
		{
			return textFieldParser.ReadFields();
		}
		catch (MalformedLineException)
		{
			return parameterRecord.Split(",".ToCharArray());
		}
	}

	private static bool ValidateParameterRecord(string parameterRecord, out string parameterQualifier, out VcpParameterAccess parameterAccess, out string parameterValue, ParameterFileFormat parameterFileFormat)
	{
		string[] array = SplitParameterRecord(parameterRecord);
		parameterQualifier = string.Empty;
		parameterValue = string.Empty;
		parameterAccess = VcpParameterAccess.Error;
		if (array.Length > 1)
		{
			parameterQualifier = array[1].Trim();
			if (parameterFileFormat == ParameterFileFormat.ParFile)
			{
				if (array.Length > 2)
				{
					parameterAccess = GetParameterAccess(array[2].Trim());
					if (array.Length > 3)
					{
						if (array.Length > 4)
						{
							parameterValue = string.Join(",", array.Skip(3).ToArray());
						}
						else
						{
							parameterValue = array[3];
						}
					}
					return true;
				}
			}
			else
			{
				if (array.Length > 3)
				{
					if (Convert.ToUInt16(array[2], CultureInfo.InvariantCulture) == 0)
					{
						parameterAccess = VcpParameterAccess.Both;
						if (array.Length > 5)
						{
							parameterValue = string.Join(",", array.Skip(3).Take(array.Length - 4).ToArray());
						}
						else
						{
							parameterValue = array[3];
						}
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
			case 'R':
				return VcpParameterAccess.Read;
			case 'W':
				return VcpParameterAccess.Write;
			case 'B':
				return VcpParameterAccess.Both;
			}
		}
		return VcpParameterAccess.Error;
	}

	internal void ProcessExternalRead()
	{
		IEnumerable<Parameter> enumerable = parameters.Where((Parameter parameter) => parameter.ReadAccess <= Sapi.GetSapi().ReadAccess && parameter.Marked);
		ProcessExternal(enumerable, (Parameter parameter) => "P," + parameter.Qualifier + ",R,,");
	}

	internal void ProcessExternalWrite()
	{
		IEnumerable<Parameter> enumerable = parameters.Where((Parameter parameter) => parameter.WriteAccess <= Sapi.GetSapi().WriteAccess && parameter.Marked && !object.Equals(parameter.OriginalValue, parameter.Value));
		ProcessExternal(enumerable, (Parameter parameter) => string.Concat("P,", parameter.Qualifier, ",B,", parameter.Value, ","));
	}

	private void ProcessExternal(IEnumerable<Parameter> parameters, Func<Parameter, string> recordFunc)
	{
		if (!File.Exists(externalVcpPath))
		{
			throw new CaesarException(SapiError.ExternalVcpNotFound);
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("S,COMSWINT,XXXX,");
		stringBuilder.AppendLine("S,ECU," + this.parameters.Channel.Ecu.Name + ",");
		foreach (Parameter parameter in parameters)
		{
			stringBuilder.AppendLine(recordFunc(parameter));
		}
		string text = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString(), ".PAR"));
		string text2 = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString(), ".VER"));
		File.WriteAllText(text, stringBuilder.ToString(), Encoding.UTF8);
		ProcessExternal(text, text2);
		if (File.Exists(text2))
		{
			TextFieldParser textFieldParser = new TextFieldParser(text2);
			textFieldParser.SetDelimiters(",");
			textFieldParser.HasFieldsEnclosedInQuotes = true;
			while (!textFieldParser.EndOfData)
			{
				string[] array = textFieldParser.ReadFields();
				string text3 = array[0];
				if (!(text3 == "S"))
				{
					if (text3 == "P" && array.Length >= 5)
					{
						string qualifier = array[1];
						this.parameters[qualifier]?.InternalRead(array);
					}
					continue;
				}
				text3 = array[1];
				if (!(text3 == "PERROR"))
				{
					if (text3 == "CERROR")
					{
						string text4 = array[2];
						if (text4 != "1000")
						{
							throw new CaesarException(SapiError.ExternalVcpComponentError, text4 + " - " + ((array.Length > 2) ? array[3] : string.Empty));
						}
					}
				}
				else if (!(array[2] == "T"))
				{
				}
			}
			return;
		}
		throw new CaesarException(SapiError.ExternalVcpVerFileNotCreated);
	}

	private void ProcessExternal(string tempInPath, string tempOutPath)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = externalVcpPath;
		processStartInfo.Arguments = "-p " + tempInPath + " -v " + tempOutPath;
		if (externalVcpAdditionalArguments != null)
		{
			if (externalVcpAdditionalArguments.StartsWith("extension:", StringComparison.OrdinalIgnoreCase))
			{
				string method = externalVcpAdditionalArguments.Substring(10);
				string text = (string)parameters.Channel.Extension.Invoke(method, new object[0]);
				processStartInfo.Arguments = processStartInfo.Arguments + " " + text;
			}
			else
			{
				processStartInfo.Arguments = processStartInfo.Arguments + " " + externalVcpAdditionalArguments;
			}
		}
		try
		{
			System.Diagnostics.Process.Start(processStartInfo).WaitForExit();
		}
		catch (Win32Exception ex)
		{
			throw new CaesarException(SapiError.ExternalVcpExecutionError, ex.Message + Environment.NewLine + processStartInfo.FileName);
		}
	}
}
