// Decompiled with JetBrains decompiler
// Type: rp1210.rp1210driver
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;

#nullable disable
namespace rp1210;

public class rp1210driver
{
  private RP121032 J1939inst;
  private RP121032 J1587inst;
  private RP1210BDriverInfo driverInfo;
  private DeviceInfo deviceInfo;
  private Thread DataPoller;
  public List<J1939Message> J1939MessageFilter;
  public List<J1587Message> J1587MessageFilter;
  private List<string> _DriverList;
  private string _SelectedDriver;
  private List<string> _DeviceList;
  private string _SelectedDevice;

  public bool J1939Connected { get; set; }

  public bool J1587Connected { get; set; }

  public List<rp1210driver.PeriodicMessage> PeriodicMessages { get; set; }

  public List<string> DriverList => this._DriverList;

  public string SelectedDriver
  {
    get => this._SelectedDriver;
    set
    {
      this._SelectedDriver = value;
      this._DeviceList.Clear();
      this.driverInfo = RP121032.LoadDeviceParameters($"{Environment.GetEnvironmentVariable("SystemRoot")}\\{this._SelectedDriver}.ini");
      foreach (DeviceInfo rp1210Device in this.driverInfo.RP1210Devices)
        this._DeviceList.Add(rp1210Device.DeviceName);
    }
  }

  public List<string> DeviceList => this._DeviceList;

  public string SelectedDevice
  {
    get => this._SelectedDevice;
    set
    {
      this._SelectedDevice = value;
      this.deviceInfo = this.driverInfo.RP1210Devices.Find((Predicate<DeviceInfo>) (x => x.DeviceName == this._SelectedDevice));
    }
  }

  public event rp1210driver.DataRecievedHandler J1939DataRecieved;

  public rp1210driver()
  {
    this._DeviceList = new List<string>();
    this._DriverList = RP121032.ScanForDrivers();
    this.SelectedDriver = this._DriverList[0];
    this.PeriodicMessages = new List<rp1210driver.PeriodicMessage>();
    this.J1939MessageFilter = new List<J1939Message>();
  }

  private void J1939AddressClaim()
  {
    byte[] numArray = new byte[8]
    {
      (byte) 0,
      (byte) 0,
      (byte) 32 /*0x20*/,
      (byte) 37,
      (byte) 0,
      (byte) 129,
      (byte) 0,
      (byte) 0
    };
    byte[] fpchClientCommand = new byte[numArray.Length + 2];
    fpchClientCommand[0] = (byte) 0;
    fpchClientCommand[1] = numArray[0];
    fpchClientCommand[2] = numArray[1];
    fpchClientCommand[3] = numArray[2];
    fpchClientCommand[4] = numArray[3];
    fpchClientCommand[5] = numArray[4];
    fpchClientCommand[6] = numArray[5];
    fpchClientCommand[7] = numArray[6];
    fpchClientCommand[8] = numArray[7];
    fpchClientCommand[9] = (byte) 0;
    this.J1939inst.RP1210_SendCommand(RP1210_Commands.RP1210_Protect_J1939_Address, fpchClientCommand, (short) fpchClientCommand.Length);
  }

  public void J1939Connect()
  {
    bool flag1 = false;
    if (this.J1939Connected)
    {
      if (this.J1939inst != null)
        this.J1939inst = (RP121032) null;
      this.J1939Connected = false;
    }
    else
    {
      this.J1939inst = new RP121032(this._SelectedDriver);
      try
      {
        this.J1939inst.RP1210_ClientConnect(this.deviceInfo.DeviceId, new StringBuilder("J1939"), 0, 0, (short) 0);
        this.DataPoller = new Thread(new ThreadStart(this.PollingDriver));
        this.DataPoller.IsBackground = true;
        this.DataPoller.Start();
        bool flag2;
        try
        {
          this.J1939inst.RP1210_SendCommand(RP1210_Commands.RP1210_Set_All_Filters_States_to_Pass, (byte[]) null, (short) 0);
          try
          {
            this.J1939AddressClaim();
          }
          catch (Exception ex)
          {
            flag2 = true;
            throw new Exception(ex.Message);
          }
        }
        catch (Exception ex)
        {
          flag2 = true;
          throw new Exception(ex.Message);
        }
      }
      catch (Exception ex)
      {
        flag1 = true;
      }
      if (flag1)
        return;
      this.J1939Connected = true;
    }
  }

  public void J1587Connect()
  {
    bool flag = false;
    if (this.J1939Connected)
    {
      if (this.J1587inst != null)
        this.J1587inst = (RP121032) null;
      this.J1939Connected = false;
    }
    else
    {
      this.J1587inst = new RP121032(this._SelectedDevice);
      try
      {
        this.J1587inst.RP1210_ClientConnect(this.deviceInfo.DeviceId, new StringBuilder("J1708"), 0, 0, (short) 0);
      }
      catch (Exception ex)
      {
        flag = true;
      }
      try
      {
        this.J1587inst.RP1210_SendCommand(RP1210_Commands.RP1210_Set_All_Filters_States_to_Pass, (byte[]) null, (short) 0);
      }
      catch (Exception ex)
      {
        flag = true;
      }
      if (flag)
        return;
      this.J1587Connected = true;
    }
  }

  public void J1939Disconnect()
  {
    if (this.J1939inst == null)
      return;
    this.J1939Connected = false;
    int num = (int) this.J1939inst.RP1210_ClientDisconnect();
    this.J1939inst.Dispose();
    this.J1939inst = (RP121032) null;
  }

  public void J1587Disconnect()
  {
    if (this.J1587inst == null)
      return;
    this.J1587Connected = false;
    int num = (int) this.J1587inst.RP1210_ClientDisconnect();
    this.J1587inst.Dispose();
    this.J1587inst = (RP121032) null;
  }

  public void Close()
  {
    try
    {
      foreach (rp1210driver.PeriodicMessage periodicMessage in this.PeriodicMessages)
        periodicMessage.Dispose();
      this.PeriodicMessages.Clear();
      this.DataPoller.Abort();
      this.J1939Disconnect();
      this.J1587Disconnect();
    }
    catch
    {
    }
  }

  public void SendPeriodicMessage(J1939Message msgToSend, double interval)
  {
    this.PeriodicMessages.Add(new rp1210driver.PeriodicMessage(msgToSend, interval, new rp1210driver.PeriodicMessage.txJ1939Data(this.SendData)));
  }

  public void SendPeriodicMessage(J1587Message msgToSend, double interval)
  {
    this.PeriodicMessages.Add(new rp1210driver.PeriodicMessage(msgToSend, interval, new rp1210driver.PeriodicMessage.txJ1587Data(this.SendData)));
  }

  public void SendData(J1939Message msgToSend)
  {
    if (this.J1939inst == null)
      return;
    try
    {
      byte[] fpchClientMessage = RP121032.EncodeJ1939Message(msgToSend);
      int priority = (int) msgToSend.Priority;
      int pgn = (int) msgToSend.PGN;
      int sourceAddress = (int) msgToSend.SourceAddress;
      int num = (int) this.J1939inst.RP1210_SendMessage(fpchClientMessage, (short) fpchClientMessage.Length, (short) 0, (short) 0);
    }
    catch (Exception ex)
    {
    }
  }

  public void SendData(J1587Message msgToSend)
  {
    if (this.J1587inst == null)
      return;
    try
    {
      byte[] array = msgToSend.ToArray();
      int num = (int) this.J1939inst.RP1210_SendMessage(array, (short) array.Length, (short) 0, (short) 0);
    }
    catch (Exception ex)
    {
    }
  }

  private void PollingDriver()
  {
    while (true)
    {
      rp1210driver.DataRecievedArgs e;
      do
      {
        byte[] message1;
        do
        {
          while (this.J1939inst == null)
          {
            if (this.J1587inst == null)
              return;
          }
          message1 = this.J1939inst.RP1210_ReadMessage((short) 0);
        }
        while (message1.Length <= 1);
        e = new rp1210driver.DataRecievedArgs();
        e.J1939 = true;
        J1939Message message = RP121032.DecodeJ1939Message(message1);
        if (this.J1939MessageFilter.Count != 0)
        {
          if (this.J1939MessageFilter.Find((Predicate<J1939Message>) (x => (int) x.PGN == (int) message.PGN)) != null)
            e.RecievedJ1939Message = message;
        }
        else
          e.RecievedJ1939Message = message;
      }
      while (this.J1939DataRecieved == null || e.RecievedJ1939Message == null);
      this.J1939DataRecieved((object) this, e);
    }
  }

  public class PeriodicMessage : IDisposable
  {
    private bool disposed;
    private J1939Message _j1939Message;
    private J1587Message _j1587Message;
    private System.Timers.Timer _timeKeeper;
    private rp1210driver.PeriodicMessage.PeriodicMessageType _MessageType;
    private ElapsedEventHandler _EventHandler;

    public J1939Message j1939Message => this._j1939Message;

    public J1587Message j1587Message => this._j1587Message;

    public rp1210driver.PeriodicMessage.PeriodicMessageType MessageType => this._MessageType;

    public rp1210driver.PeriodicMessage.txJ1939Data SendJ1939Data { get; set; }

    public rp1210driver.PeriodicMessage.txJ1587Data SendJ1587Data { get; set; }

    public PeriodicMessage(
      J1939Message msg,
      double interval,
      rp1210driver.PeriodicMessage.txJ1939Data txDelegate)
    {
      this._MessageType = rp1210driver.PeriodicMessage.PeriodicMessageType.J1939;
      this._timeKeeper = new System.Timers.Timer(interval);
      this.SendJ1939Data = txDelegate;
      this._j1939Message = msg;
      this._EventHandler = new ElapsedEventHandler(this._timeKeeper_Elapsed);
      this._timeKeeper.Elapsed += this._EventHandler;
      this._timeKeeper.Enabled = true;
    }

    public PeriodicMessage(
      J1587Message msg,
      double interval,
      rp1210driver.PeriodicMessage.txJ1587Data txDelegate)
    {
      this._MessageType = rp1210driver.PeriodicMessage.PeriodicMessageType.J1587;
      this._timeKeeper = new System.Timers.Timer(interval);
      this.SendJ1587Data = txDelegate;
      this._j1587Message = msg;
      this._EventHandler = new ElapsedEventHandler(this._timeKeeper_Elapsed);
      this._timeKeeper.Elapsed += this._EventHandler;
      this._timeKeeper.Enabled = true;
    }

    private void _timeKeeper_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (this._MessageType == rp1210driver.PeriodicMessage.PeriodicMessageType.J1939)
      {
        if (this._j1939Message == null || this.SendJ1939Data == null)
          return;
        this.SendJ1939Data(this._j1939Message);
      }
      else
      {
        if (this._MessageType != rp1210driver.PeriodicMessage.PeriodicMessageType.J1587 || this._j1587Message == null || this.SendJ1587Data == null)
          return;
        this.SendJ1587Data(this._j1587Message);
      }
    }

    public void Dispose()
    {
      if (!this.disposed)
      {
        this._timeKeeper.Enabled = false;
        this._timeKeeper.Elapsed -= this._EventHandler;
      }
      this.disposed = true;
    }

    ~PeriodicMessage() => this.Dispose();

    public enum PeriodicMessageType
    {
      J1939,
      J1587,
    }

    public delegate void txJ1939Data(J1939Message msgToSend);

    public delegate void txJ1587Data(J1587Message msgToSend);
  }

  public class DataRecievedArgs : EventArgs
  {
    public bool J1939 { get; set; }

    public bool J1587 { get; set; }

    public J1939Message RecievedJ1939Message { get; set; }

    public J1587Message RecievedJ1587Message { get; set; }
  }

  public delegate void DataRecievedHandler(object sender, rp1210driver.DataRecievedArgs e);
}
