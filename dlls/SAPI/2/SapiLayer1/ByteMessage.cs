using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class ByteMessage
{
	private Channel channel;

	private Dump request;

	private Dump response;

	private Dump requiredResponse;

	public Channel Channel => channel;

	public Dump Request => request;

	public Dump Response => response;

	public Dump RequiredResponse => requiredResponse;

	internal ByteMessage(Channel channel, Dump request)
	{
		this.channel = channel;
		this.request = request;
	}

	internal ByteMessage(Channel channel, Dump request, Dump requiredResponse)
	{
		this.channel = channel;
		this.request = request;
		this.requiredResponse = requiredResponse;
	}

	internal void InternalDoMessage()
	{
		InternalDoMessage(internalRequest: false);
	}

	internal void InternalDoMessage(bool internalRequest)
	{
		//IL_0050: Expected O, but got Unknown
		CaesarException e = null;
		if (channel.ChannelHandle != null)
		{
			try
			{
				byte[] data = channel.ChannelHandle.CCDoMessage(request.Data.ToArray());
				response = new Dump(data);
			}
			catch (CaesarErrorException ex)
			{
				e = new CaesarException(ex, null, null);
			}
		}
		else if (channel.McdChannelHandle != null)
		{
			try
			{
				McdDiagComPrimitive hexService = channel.McdChannelHandle.GetHexService();
				hexService.RequestMessage = request.Data.ToArray();
				hexService.Execute(0);
				response = new Dump(hexService.ResponseMessage);
			}
			catch (McdException mcdError)
			{
				e = new CaesarException(mcdError);
			}
		}
		else if (channel.Ecu.RollCallManager != null)
		{
			try
			{
				byte[] data2 = channel.Ecu.RollCallManager.DoByteMessage(channel, request.Data.ToArray(), (requiredResponse != null) ? requiredResponse.Data.ToArray() : null);
				response = new Dump(data2);
			}
			catch (CaesarException ex2)
			{
				e = ex2;
			}
		}
		if (!internalRequest)
		{
			channel.RaiseByteMessageComplete(this, e);
		}
	}
}
