// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.DataHub.DataPageRequest
// Assembly: DataHub, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 89346980-C6E7-48B1-88DD-CF29796E810E
// Assembly location: C:\Users\petra\Downloads\Архив (2)\DataHub.dll

using SapiLayer1;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable
namespace DetroitDiesel.DataHub;

internal class DataPageRequest
{
  public const string DataPageReadService = "223E";
  public const string DataPagePasswordService = "31013E00";
  public const string DataPageClearService = "31013E";
  public const string DataPageChangePasswordService = "2E3E060D0006100000000000";
  private readonly Channel channel;
  private DataPageRequestType pendingRequestType;

  public DataPage Page { get; private set; }

  public event EventHandler<DataPageRequestEventArgs> DataPageRequestCompleteEvent;

  public DataPageRequest(Channel channel)
  {
    this.channel = channel;
    this.Page = (DataPage) null;
  }

  public void RequestDataPageRead(DataPageType pageType)
  {
    this.pendingRequestType = pageType != DataPageType.SupportRequestPage0 ? DataPageRequestType.DataPage : DataPageRequestType.Page0;
    this.Page = (DataPage) null;
    Dump readRequestMessage = DataPageRequest.GetDataPageReadRequestMessage(pageType);
    this.channel.ByteMessageCompleteEvent += new ByteMessageCompleteEventHandler(this.channel_ByteMessageCompleteEvent);
    this.channel.SendByteMessage(readRequestMessage, false);
  }

  private static Dump GetDataPageReadRequestMessage(DataPageType pageType)
  {
    return new Dump(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1:X2}", (object) "223E", (object) (int) pageType));
  }

  public void RequestPasswordChange(string newDataPagePassword)
  {
    this.pendingRequestType = DataPageRequestType.ChangePassword;
    this.channel.ByteMessageCompleteEvent += new ByteMessageCompleteEventHandler(this.channel_ByteMessageCompleteEvent);
    this.channel.SendByteMessage(DataPageRequest.GetDataPageChangePasswordRequestMessage(newDataPagePassword), false);
  }

  public void RequestDataPageClear(DataPageType pageType)
  {
    this.pendingRequestType = DataPageRequestType.Clear;
    Dump clearRequestMessage = DataPageRequest.GetDataPageClearRequestMessage(pageType);
    this.channel.ByteMessageCompleteEvent += new ByteMessageCompleteEventHandler(this.channel_ByteMessageCompleteEvent);
    this.channel.SendByteMessage(clearRequestMessage, false);
  }

  private static Dump GetDataPageChangePasswordRequestMessage(string newDataPagePassword)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("2E3E060D0006100000000000");
    foreach (byte buildPasswordByte in DataPageRequest.BuildPasswordBytes(newDataPagePassword))
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:X2}", (object) buildPasswordByte);
    return new Dump(stringBuilder.ToString());
  }

  private static Dump GetDataPageClearRequestMessage(DataPageType pageType)
  {
    return new Dump(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1:X2}", (object) "31013E", (object) (int) pageType));
  }

  public void RequestDataPageUnlock(string password)
  {
    this.pendingRequestType = DataPageRequestType.Unlock;
    this.channel.ByteMessageCompleteEvent += new ByteMessageCompleteEventHandler(this.channel_ByteMessageCompleteEvent);
    this.channel.SendByteMessage(DataPageRequest.GetPasswordUnlockMessage(password), true);
  }

  private static byte[] BuildPasswordBytes(string password)
  {
    if (string.IsNullOrEmpty(password))
      return new byte[6];
    byte[] numArray = new byte[6]
    {
      (byte) 32 /*0x20*/,
      (byte) 32 /*0x20*/,
      (byte) 32 /*0x20*/,
      (byte) 32 /*0x20*/,
      (byte) 32 /*0x20*/,
      (byte) 32 /*0x20*/
    };
    for (int index = 0; index < password.Length && index < 6; ++index)
      numArray[index] = (byte) password[index];
    return numArray;
  }

  private static Dump GetPasswordUnlockMessage(string password)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("31013E00");
    foreach (byte buildPasswordByte in DataPageRequest.BuildPasswordBytes(password))
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:X2}", (object) buildPasswordByte);
    return new Dump(stringBuilder.ToString());
  }

  private static bool ParseRequestResponse(byte[] response)
  {
    return response.Length >= 1 && (response[0] == (byte) 113 || response[0] == (byte) 110);
  }

  private void channel_ByteMessageCompleteEvent(object sender, ResultEventArgs resultEventArgs)
  {
    this.channel.ByteMessageCompleteEvent -= new ByteMessageCompleteEventHandler(this.channel_ByteMessageCompleteEvent);
    ByteMessage byteMessage = sender as ByteMessage;
    bool succeeded = false;
    if (resultEventArgs.Succeeded && byteMessage != null && byteMessage.Response.Data != null)
    {
      switch (this.pendingRequestType)
      {
        case DataPageRequestType.Clear:
        case DataPageRequestType.Unlock:
          succeeded = DataPageRequest.ParseRequestResponse(byteMessage.Response.Data.ToArray<byte>());
          this.Page = (DataPage) null;
          break;
        case DataPageRequestType.Page0:
        case DataPageRequestType.DataPage:
          this.Page = new DataPage(byteMessage.Response.Data.ToArray<byte>());
          succeeded = this.Page != null && this.Page.Valid;
          break;
        case DataPageRequestType.ChangePassword:
          succeeded = DataPageRequest.ParseRequestResponse(byteMessage.Response.Data.ToArray<byte>());
          break;
      }
    }
    if (this.DataPageRequestCompleteEvent != null)
      this.DataPageRequestCompleteEvent((object) this, new DataPageRequestEventArgs(succeeded, this.pendingRequestType, this.Page));
    this.pendingRequestType = DataPageRequestType.None;
  }
}
