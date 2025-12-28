using System;
using System.Globalization;
using System.Linq;
using System.Text;
using SapiLayer1;

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
		Page = null;
	}

	public void RequestDataPageRead(DataPageType pageType)
	{
		if (pageType == DataPageType.SupportRequestPage0)
		{
			pendingRequestType = DataPageRequestType.Page0;
		}
		else
		{
			pendingRequestType = DataPageRequestType.DataPage;
		}
		Page = null;
		Dump dataPageReadRequestMessage = GetDataPageReadRequestMessage(pageType);
		channel.ByteMessageCompleteEvent += channel_ByteMessageCompleteEvent;
		channel.SendByteMessage(dataPageReadRequestMessage, synchronous: false);
	}

	private static Dump GetDataPageReadRequestMessage(DataPageType pageType)
	{
		return new Dump(string.Format(CultureInfo.InvariantCulture, "{0}{1:X2}", "223E", (int)pageType));
	}

	public void RequestPasswordChange(string newDataPagePassword)
	{
		pendingRequestType = DataPageRequestType.ChangePassword;
		channel.ByteMessageCompleteEvent += channel_ByteMessageCompleteEvent;
		channel.SendByteMessage(GetDataPageChangePasswordRequestMessage(newDataPagePassword), synchronous: false);
	}

	public void RequestDataPageClear(DataPageType pageType)
	{
		pendingRequestType = DataPageRequestType.Clear;
		Dump dataPageClearRequestMessage = GetDataPageClearRequestMessage(pageType);
		channel.ByteMessageCompleteEvent += channel_ByteMessageCompleteEvent;
		channel.SendByteMessage(dataPageClearRequestMessage, synchronous: false);
	}

	private static Dump GetDataPageChangePasswordRequestMessage(string newDataPagePassword)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("2E3E060D0006100000000000");
		byte[] array = BuildPasswordBytes(newDataPagePassword);
		foreach (byte b in array)
		{
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
		}
		return new Dump(stringBuilder.ToString());
	}

	private static Dump GetDataPageClearRequestMessage(DataPageType pageType)
	{
		return new Dump(string.Format(CultureInfo.InvariantCulture, "{0}{1:X2}", "31013E", (int)pageType));
	}

	public void RequestDataPageUnlock(string password)
	{
		pendingRequestType = DataPageRequestType.Unlock;
		channel.ByteMessageCompleteEvent += channel_ByteMessageCompleteEvent;
		channel.SendByteMessage(GetPasswordUnlockMessage(password), synchronous: true);
	}

	private static byte[] BuildPasswordBytes(string password)
	{
		if (string.IsNullOrEmpty(password))
		{
			return new byte[6];
		}
		byte[] array = new byte[6] { 32, 32, 32, 32, 32, 32 };
		for (int i = 0; i < password.Length && i < 6; i++)
		{
			array[i] = (byte)password[i];
		}
		return array;
	}

	private static Dump GetPasswordUnlockMessage(string password)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("31013E00");
		byte[] array = BuildPasswordBytes(password);
		foreach (byte b in array)
		{
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
		}
		return new Dump(stringBuilder.ToString());
	}

	private static bool ParseRequestResponse(byte[] response)
	{
		if (response.Length >= 1 && (response[0] == 113 || response[0] == 110))
		{
			return true;
		}
		return false;
	}

	private void channel_ByteMessageCompleteEvent(object sender, ResultEventArgs resultEventArgs)
	{
		channel.ByteMessageCompleteEvent -= channel_ByteMessageCompleteEvent;
		ByteMessage byteMessage = sender as ByteMessage;
		bool succeeded = false;
		if (resultEventArgs.Succeeded && byteMessage != null && byteMessage.Response.Data != null)
		{
			switch (pendingRequestType)
			{
			case DataPageRequestType.Clear:
			case DataPageRequestType.Unlock:
				succeeded = ParseRequestResponse(byteMessage.Response.Data.ToArray());
				Page = null;
				break;
			case DataPageRequestType.ChangePassword:
				succeeded = ParseRequestResponse(byteMessage.Response.Data.ToArray());
				break;
			case DataPageRequestType.Page0:
			case DataPageRequestType.DataPage:
				Page = new DataPage(byteMessage.Response.Data.ToArray());
				succeeded = Page != null && Page.Valid;
				break;
			}
		}
		if (this.DataPageRequestCompleteEvent != null)
		{
			this.DataPageRequestCompleteEvent(this, new DataPageRequestEventArgs(succeeded, pendingRequestType, Page));
		}
		pendingRequestType = DataPageRequestType.None;
	}
}
