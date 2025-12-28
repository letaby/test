// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.BusMonitorForm
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class BusMonitorForm : Form
{
  private IntPtr handle;
  private Form mainForm;
  private ManualResetEvent formStartedEvent;
  private static BusMonitorForm globalInstance;
  private BusMonitorStatisticsForm statisticsForm;
  private System.Timers.Timer searchTimer;
  private const int TimeForEvent = 50;
  private const int FramesForEvent = 500;
  private IContainer components;
  private ToolStrip toolStrip;
  private ToolStripButton toolStripButtonStart;
  private ToolStripButton toolStripButtonStop;
  private ToolStripComboBox toolStripComboBoxResources;
  private ToolStripSeparator toolStripSeparator1;
  private ToolStripButton toolStripButtonShowEcuNames;
  private ToolStripButton toolStripButtonShowCompletePacket;
  private ToolStripComboBox toolStripComboBoxEcuFilter;
  private ToolStripButton toolStripButtonPause;
  private BusMonitorControl busMonitorControl;
  private ToolStripSeparator toolStripSeparator2;
  private ToolStripButton toolStripButtonLoad;
  private ToolStripButton toolStripButtonSave;
  private ToolStripButton toolStripButtonTimestampDisplay;
  private TableLayoutPanel tableLayoutPanelProperties;
  private Label labelTitleLabel;
  private Label labelStartTimeLabel;
  private Label labelConnectionResourceLabel;
  private Label labelTitle;
  private Label labelStartTime;
  private Label labelConnectionResource;
  private ToolStripButton toolStripButtonSettings;
  private ToolStripSeparator toolStripSeparator3;
  private ToolStripButton toolStripButtonSymbolic;
  private ToolStripButton toolStripButtonClearContents;
  private ToolStripLabel toolStripLabelFilter;
  private ToolStripTextBox toolStripTextBoxFilter;
  private ToolStripButton toolStripButtonStatistics;
  private ToolStripSeparator toolStripSeparator4;

  private BusMonitorForm(ManualResetEvent formStartedEvent)
  {
    this.formStartedEvent = formStartedEvent;
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.toolStrip.Items.Remove((ToolStripItem) this.toolStripButtonSymbolic);
    this.toolStrip.Items.Remove((ToolStripItem) this.toolStripButtonShowEcuNames);
    this.toolStrip.Items.Remove((ToolStripItem) this.toolStripSeparator4);
    this.toolStrip.Items.Remove((ToolStripItem) this.toolStripButtonStatistics);
    using (Graphics graphics = this.CreateGraphics())
    {
      foreach (ToolStrip toolStrip in this.Controls.OfType<ToolStrip>())
      {
        toolStrip.AutoSize = false;
        toolStrip.ImageScalingSize = new Size((int) (20.0 * (double) graphics.DpiX / 96.0), (int) (20.0 * (double) graphics.DpiY / 96.0));
        toolStrip.AutoSize = true;
      }
    }
    this.mainForm = Application.OpenForms[0];
    this.mainForm.FormClosing += new FormClosingEventHandler(this.mainForm_FormClosing);
    SapiManager.GlobalInstance.SapiResetCompleted += new EventHandler(this.GlobalInstance_SapiResetCompleted);
    this.busMonitorControl.PropertyChanged += new PropertyChangedEventHandler(this.busMonitorControl_PropertyChanged);
    this.busMonitorControl.MonitorException += new EventHandler<ResultEventArgs>(this.busMonitorControl_MonitorException);
  }

  private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    this.BeginInvoke((Delegate) new Action(((Form) this).Close));
  }

  public static void Show() => BusMonitorForm.EnsureCreated();

  private static void EnsureCreated()
  {
    if (BusMonitorForm.globalInstance == null)
    {
      ManualResetEvent parameter = new ManualResetEvent(false);
      Thread thread = new Thread(new ParameterizedThreadStart(BusMonitorForm.BusMonitorFormThreadFunc));
      thread.SetApartmentState(ApartmentState.STA);
      thread.Start((object) parameter);
      parameter.WaitOne();
      BusMonitorForm.globalInstance.SetOwner();
    }
    else
      BusMonitorForm.globalInstance.Activate();
  }

  private static void BusMonitorFormThreadFunc(object formStartedEvent)
  {
    CrashHandler.Initialize();
    BusMonitorForm.globalInstance = new BusMonitorForm(formStartedEvent as ManualResetEvent);
    Application.Run((Form) BusMonitorForm.globalInstance);
    BusMonitorForm.globalInstance = (BusMonitorForm) null;
  }

  private void SetOwner()
  {
    BusMonitorForm.NativeMethods.SetWindowLongPtr(new HandleRef((object) this, this.handle), -8, this.mainForm.Handle);
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    this.handle = this.Handle;
    this.formStartedEvent.Set();
    if (this.DesignMode)
      return;
    ((ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels).ConnectCompleteEvent += new ConnectCompleteEventHandler(this.Channels_ConnectCompleteEvent);
    this.ResetIdentifierMap();
    this.KeyPreview = true;
    this.toolStripComboBoxResources.Items.AddRange((object[]) new string[3]
    {
      "CAN1",
      "CAN2",
      "ETHERNET"
    });
    this.LoadSettings();
    this.UpdateUserInterface();
    this.UpdateMonitorProperties();
    this.searchTimer = new System.Timers.Timer(1000.0);
    this.searchTimer.SynchronizingObject = (ISynchronizeInvoke) this;
    this.searchTimer.AutoReset = false;
    this.searchTimer.Elapsed += new ElapsedEventHandler(this.SearchTimer_Elapsed);
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    switch (e.KeyCode)
    {
      case Keys.MediaStop:
        if (this.toolStripButtonStop.Enabled)
        {
          this.toolStripButtonStop_Click((object) this, new EventArgs());
          break;
        }
        break;
      case Keys.MediaPlayPause:
        if (this.toolStripButtonStart.Enabled)
        {
          this.toolStripButtonStart_Click((object) this, new EventArgs());
          break;
        }
        if (this.toolStripButtonPause.Enabled)
        {
          this.toolStripButtonPause_Click((object) this, new EventArgs());
          break;
        }
        break;
    }
    base.OnKeyDown(e);
  }

  private void Channels_ConnectCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.InvokeRequired)
    {
      this.BeginInvoke((Delegate) new EventHandler<ResultEventArgs>(this.Channels_ConnectCompleteEvent), sender, (object) e);
    }
    else
    {
      Channel element = sender as Channel;
      if (!e.Succeeded || element == null || this.busMonitorControl.ContentIsFromFile || !BusMonitorForm.IsSupported(element.Ecu))
        return;
      this.busMonitorControl.AddKnownChannels(Enumerable.Repeat<Channel>(element, 1));
    }
  }

  protected override void OnFormClosing(FormClosingEventArgs e)
  {
    if (this.busMonitorControl.HasMonitor)
      this.busMonitorControl.StopMonitor();
    this.mainForm.FormClosing -= new FormClosingEventHandler(this.mainForm_FormClosing);
    this.SaveSettings();
    base.OnFormClosing(e);
  }

  public new void Activate()
  {
    if (this.InvokeRequired)
      this.BeginInvoke((Delegate) new Action(this.Activate));
    else
      base.Activate();
  }

  private void GlobalInstance_SapiResetCompleted(object sender, EventArgs e)
  {
    if (this.InvokeRequired)
      this.BeginInvoke((Delegate) new EventHandler(this.GlobalInstance_SapiResetCompleted), sender, (object) e);
    else
      this.ResetIdentifierMap();
  }

  private void ResetIdentifierMap()
  {
    this.busMonitorControl.ClearKnownChannels();
    this.busMonitorControl.AddKnownChannels(((IEnumerable<Channel>) SapiManager.GlobalInstance.Sapi.Channels).Where<Channel>((Func<Channel, bool>) (c => c.Online && BusMonitorForm.IsSupported(c.Ecu))));
  }

  private void SelectEcu(Ecu ecu)
  {
    for (int index = 0; index < this.toolStripComboBoxEcuFilter.Items.Count; ++index)
    {
      if (this.toolStripComboBoxEcuFilter.Items[index] is Ecu ecu1 && ecu1.Identifier == ecu.Identifier)
      {
        this.toolStripComboBoxEcuFilter.SelectedItem = (object) ecu1;
        break;
      }
    }
  }

  internal static int GetPriority(string identifier)
  {
    string[] strArray = identifier.Split("-".ToCharArray());
    int result;
    if (strArray.Length <= 1 || !int.TryParse(strArray[1], out result))
      return int.MaxValue;
    return !(strArray[0] == "UDS") ? result + (int) byte.MaxValue : result;
  }

  private void UpdateEcuCombo()
  {
    string text = this.toolStripComboBoxEcuFilter.Text;
    Ecu selectedItem = this.toolStripComboBoxEcuFilter.SelectedItem as Ecu;
    this.toolStripComboBoxEcuFilter.Items.Clear();
    this.toolStripComboBoxEcuFilter.Items.Add((object) Resources.BusMonitorForm_MonitorAll);
    this.toolStripComboBoxEcuFilter.Items.Add((object) Resources.BusMonitorForm_MonitorAllKnown);
    foreach (string str in (IEnumerable<string>) this.busMonitorControl.KnownIdentifiers.OrderBy<string, int>((Func<string, int>) (i => BusMonitorForm.GetPriority(i))))
    {
      string identifier = str;
      Channel channel = this.busMonitorControl.KnownChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu.Identifier == identifier));
      this.toolStripComboBoxEcuFilter.Items.Add(channel != null ? (object) channel.Ecu : (object) identifier);
    }
    if (selectedItem != null)
    {
      this.SelectEcu(selectedItem);
      if (this.toolStripComboBoxEcuFilter.SelectedItem != null)
        return;
      this.toolStripComboBoxEcuFilter.SelectedIndex = 1;
    }
    else
    {
      int stringExact = this.toolStripComboBoxEcuFilter.FindStringExact(text);
      this.toolStripComboBoxEcuFilter.SelectedIndex = stringExact != -1 ? stringExact : 0;
    }
  }

  private void UpdateUserInterface()
  {
    Sapi sapi = Sapi.GetSapi();
    this.toolStripButtonStart.Enabled = !this.busMonitorControl.HasMonitor && !((IEnumerable<BusMonitor>) sapi.BusMonitors).Any<BusMonitor>() || this.busMonitorControl.Paused;
    this.toolStripButtonStop.Enabled = this.busMonitorControl.HasMonitor;
    this.toolStripButtonPause.Enabled = this.busMonitorControl.HasMonitor && !this.busMonitorControl.Paused;
    this.toolStripComboBoxResources.Enabled = !this.busMonitorControl.HasMonitor;
    this.toolStripButtonLoad.Enabled = !this.busMonitorControl.HasMonitor;
    this.toolStripButtonSave.Enabled = this.busMonitorControl.HasContent && !this.busMonitorControl.ContentIsFromFile && !this.busMonitorControl.HasMonitor;
    this.toolStripButtonClearContents.Enabled = this.busMonitorControl.HasContent && !this.busMonitorControl.HasMonitor;
    this.toolStripButtonSettings.Enabled = !this.busMonitorControl.HasMonitor;
    this.UpdateTitle();
  }

  private void UpdateTitle()
  {
    this.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BusMonitorForm_Title, this.busMonitorControl.ContentIsFromFile ? (object) Path.GetFileName(this.busMonitorControl.ContentFromFile) : (this.busMonitorControl.HasMonitor ? (this.busMonitorControl.Paused ? (object) Resources.BusMonitorForm_Paused : (object) Resources.BusMonitorForm_RunningNoFrames) : (object) Resources.BusMonitorForm_Stopped));
  }

  private static bool IsSupported(Ecu ecu)
  {
    return ecu.DiagnosisSource == null || ecu.DiagnosisSource == 2 || ecu.DiagnosisSource == 5;
  }

  private void toolStripButtonStart_Click(object sender, EventArgs e)
  {
    if (this.busMonitorControl.Paused)
    {
      this.busMonitorControl.Paused = false;
    }
    else
    {
      string text = this.toolStripComboBoxResources.Text;
      if (this.busMonitorControl.ContentIsFromFile || this.busMonitorControl.IsEthernet || this.busMonitorControl.ConnectionProperties.ContainsKey("PhysicalInterfaceLink") && this.busMonitorControl.ConnectionProperties["PhysicalInterfaceLink"] != text)
      {
        if (!this.busMonitorControl.ContentIsFromFile && this.PromptContentClear())
          return;
        this.busMonitorControl.Clear();
        this.ResetIdentifierMap();
      }
      try
      {
        if (this.busMonitorControl.StartMonitor(text, 50, 500))
        {
          this.labelStartTime.Text = this.busMonitorControl.StartTime.ToString((IFormatProvider) CultureInfo.CurrentCulture);
        }
        else
        {
          string str = SettingsManager.GlobalInstance.GetValue<StringSetting>(text, "BusMonitorWindow", new StringSetting()).Value;
          if (!string.IsNullOrEmpty(str))
          {
            int num1 = (int) ControlHelpers.ShowMessageBox((Control) this, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BusMonitorForm_CannotLocateInterfaceWithConfiguredSetting, (object) this.toolStripComboBoxResources.Text, (object) str), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
          }
          else
          {
            int num2 = (int) ControlHelpers.ShowMessageBox((Control) this, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BusMonitorForm_CannotLocateInterfaceNoConfiguredSetting, (object) this.toolStripComboBoxResources.Text), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
          }
        }
      }
      catch (Exception ex) when (
      {
        // ISSUE: unable to correctly present filter
        int num3;
        switch (ex)
        {
          case CaesarException _:
          case DllNotFoundException _:
          case BadImageFormatException _:
            num3 = 1;
            break;
          default:
            num3 = ex is InvalidOperationException ? 1 : 0;
            break;
        }
        if ((uint) num3 > 0U)
        {
          SuccessfulFiltering;
        }
        else
          throw;
      }
      )
      {
        int num4 = (int) ControlHelpers.ShowMessageBox((Control) this, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
      }
    }
  }

  private void toolStripButtonPause_Click(object sender, EventArgs e)
  {
    this.busMonitorControl.Paused = true;
  }

  private void toolStripButtonStop_Click(object sender, EventArgs e)
  {
    this.busMonitorControl.StopMonitor();
  }

  private void busMonitorControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    string propertyName = e.PropertyName;
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(propertyName))
    {
      case 113457924:
        if (!(propertyName == "KnownIdentifiers"))
          return;
        goto label_18;
      case 345050244:
        if (!(propertyName == "HasContent"))
          return;
        break;
      case 617902505:
        if (!(propertyName == "Title"))
          return;
        goto label_19;
      case 2427385216:
        if (!(propertyName == "ConnectionProperties"))
          return;
        goto label_19;
      case 3137333059:
        if (!(propertyName == "HasMonitor"))
          return;
        break;
      case 3384282644:
        if (!(propertyName == "KnownChannels"))
          return;
        goto label_18;
      case 3727171563:
        if (!(propertyName == "Paused"))
          return;
        break;
      case 4005559902:
        if (!(propertyName == "IsEthernet"))
          return;
        this.toolStripButtonShowCompletePacket.Enabled = this.busMonitorControl.IsEthernet;
        return;
      default:
        return;
    }
    this.UpdateUserInterface();
    return;
label_18:
    this.UpdateEcuCombo();
    return;
label_19:
    this.UpdateMonitorProperties();
  }

  private void busMonitorControl_MonitorException(object sender, ResultEventArgs e)
  {
    int num = (int) ControlHelpers.ShowMessageBox((Control) this, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BusMonitorForm_MonitorException, (object) e.Exception.Message), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
  }

  private void UpdateMonitorProperties()
  {
    if (this.busMonitorControl.StartTime != DateTime.MinValue)
    {
      this.labelStartTime.Text = this.busMonitorControl.StartTime.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      this.labelStartTime.Visible = this.labelStartTimeLabel.Visible = true;
    }
    else
      this.labelStartTime.Visible = this.labelStartTimeLabel.Visible = false;
    if (this.busMonitorControl.ContentIsFromFile && !string.IsNullOrEmpty(this.busMonitorControl.Title))
    {
      this.labelTitle.Text = this.busMonitorControl.Title;
      this.labelTitle.Visible = this.labelTitleLabel.Visible = true;
    }
    else
      this.labelTitle.Visible = this.labelTitleLabel.Visible = false;
    if (this.busMonitorControl.ConnectionProperties.Count > 0)
    {
      if (this.busMonitorControl.ConnectionProperties.ContainsKey("BaudRate"))
        this.labelConnectionResource.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BusMonitorForm_ConnectionResourceWithBaud, (object) this.busMonitorControl.ConnectionProperties["PhysicalInterfaceLink"], (object) this.busMonitorControl.ConnectionProperties["InterfaceBoard"], (object) this.busMonitorControl.ConnectionProperties["BaudRate"]);
      else
        this.labelConnectionResource.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BusMonitorForm_ConnectionResource, (object) this.busMonitorControl.ConnectionProperties["PhysicalInterfaceLink"], (object) this.busMonitorControl.ConnectionProperties["InterfaceBoard"]);
      this.labelConnectionResource.Visible = this.labelConnectionResourceLabel.Visible = true;
    }
    else
      this.labelConnectionResource.Visible = this.labelConnectionResourceLabel.Visible = false;
  }

  private void toolStripButtonShowEcuNames_CheckStateChanged(object sender, EventArgs e)
  {
    this.busMonitorControl.ShowEcuNames = this.toolStripButtonShowEcuNames.Checked;
  }

  private void toolStripButtonShowCompletePacket_CheckStateChanged(object sender, EventArgs e)
  {
    this.busMonitorControl.ShowCompletePacket = this.toolStripButtonShowCompletePacket.Checked;
  }

  private void toolStripButtonSymbolic_CheckStateChanged(object sender, EventArgs e)
  {
    this.busMonitorControl.ShowSymbolic = this.toolStripButtonSymbolic.Checked;
    this.ClientSize = new Size(this.toolStripButtonSymbolic.Checked ? this.ClientSize.Width * 3 / 2 : this.ClientSize.Width / 3 * 2, this.ClientSize.Height);
  }

  private void toolStripButtonTimestampDisplay_CheckStateChanged(object sender, EventArgs e)
  {
    this.busMonitorControl.DisplayAbsoluteTimestamp = this.toolStripButtonTimestampDisplay.Checked;
  }

  private void toolStripComboBoxEcuFilter_SelectedIndexChanged(object sender, EventArgs e)
  {
    switch (this.toolStripComboBoxEcuFilter.SelectedIndex)
    {
      case 0:
        this.busMonitorControl.Filter = (BusMonitorControlFilterType) 0;
        break;
      case 1:
        this.busMonitorControl.Filter = (BusMonitorControlFilterType) 1;
        break;
      default:
        this.busMonitorControl.FilterEcu = this.toolStripComboBoxEcuFilter.Text;
        break;
    }
  }

  private void LoadSettings()
  {
    Point savedLocation = SettingsManager.GlobalInstance.GetValue<Point>("Location", "BusMonitorWindow", this.Location);
    if (!((IEnumerable<Screen>) Screen.AllScreens).Any<Screen>((Func<Screen, bool>) (s => s.Bounds.Contains(savedLocation))))
      savedLocation = Screen.PrimaryScreen.WorkingArea.Location;
    Rectangle rect = new Rectangle(savedLocation, SettingsManager.GlobalInstance.GetValue<Size>("Size", "BusMonitorWindow", this.Size));
    rect.Intersect(Screen.GetWorkingArea(rect));
    this.Bounds = rect;
    this.toolStripButtonTimestampDisplay.Checked = SettingsManager.GlobalInstance.GetValue<bool>("ShowTimeAsAbsolute", "BusMonitorWindow", false);
    StringSetting filterSetting = SettingsManager.GlobalInstance.GetValue<StringSetting>("Filter", "BusMonitorWindow", new StringSetting(Resources.BusMonitorForm_MonitorAll));
    Ecu ecu = ((IEnumerable<Ecu>) SapiManager.GlobalInstance.Sapi.Ecus).FirstOrDefault<Ecu>((Func<Ecu, bool>) (e => e.Name == filterSetting.Value));
    if (ecu != null)
      this.SelectEcu(ecu);
    else
      this.toolStripComboBoxEcuFilter.SelectedIndex = this.toolStripComboBoxEcuFilter.FindStringExact(filterSetting.Value);
    if (this.toolStripComboBoxEcuFilter.SelectedIndex == -1)
      this.toolStripComboBoxEcuFilter.SelectedIndex = 0;
    StringSetting stringSetting = SettingsManager.GlobalInstance.GetValue<StringSetting>("Resource", "BusMonitorWindow", new StringSetting());
    if (stringSetting.Value != null)
      this.toolStripComboBoxResources.SelectedIndex = this.toolStripComboBoxResources.FindString(stringSetting.Value);
    if (this.toolStripComboBoxResources.SelectedIndex != -1 || this.toolStripComboBoxResources.Items.Count <= 0)
      return;
    this.toolStripComboBoxResources.SelectedIndex = 0;
  }

  private void SaveSettings()
  {
    SettingsManager.GlobalInstance.SetValue<Point>("Location", "BusMonitorWindow", this.Location, false);
    SettingsManager.GlobalInstance.SetValue<Size>("Size", "BusMonitorWindow", this.Size, false);
    SettingsManager.GlobalInstance.SetValue<bool>("ShowTimeAsAbsolute", "BusMonitorWindow", this.toolStripButtonTimestampDisplay.Checked, false);
    SettingsManager.GlobalInstance.SetValue<StringSetting>("Filter", "BusMonitorWindow", new StringSetting(this.toolStripComboBoxEcuFilter.Text), false);
    SettingsManager.GlobalInstance.SetValue<StringSetting>("Resource", "BusMonitorWindow", new StringSetting(this.toolStripComboBoxResources.Text), false);
  }

  private void toolStripComboBoxResources_DropDown(object sender, EventArgs e)
  {
    BusMonitorForm.AdjustComboDropDownWidth(sender as ToolStripComboBox);
  }

  private void toolStripComboBoxEcuFilter_DropDown(object sender, EventArgs e)
  {
    BusMonitorForm.AdjustComboDropDownWidth(sender as ToolStripComboBox);
  }

  private static void AdjustComboDropDownWidth(ToolStripComboBox senderComboBox)
  {
    int num1 = senderComboBox.DropDownWidth;
    int verticalScrollBarWidth = senderComboBox.Items.Count > senderComboBox.MaxDropDownItems ? SystemInformation.VerticalScrollBarWidth : 0;
    foreach (string text in senderComboBox.Items.Cast<object>().Select<object, string>((Func<object, string>) (item => item.ToString())))
    {
      int num2 = TextRenderer.MeasureText(text, senderComboBox.Font).Width + verticalScrollBarWidth;
      if (num1 < num2)
        num1 = num2;
    }
    senderComboBox.DropDownWidth = num1;
  }

  private bool PromptContentClear()
  {
    return this.busMonitorControl.HasContent && !this.busMonitorControl.ContentIsFromFile && ControlHelpers.ShowMessageBox((Control) this, Resources.BusMonitorForm_ContentClearPrompt, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No;
  }

  private void toolStripButtonLoad_Click(object sender, EventArgs e)
  {
    if (this.PromptContentClear())
      return;
    OpenFileDialog openFileDialog = new OpenFileDialog();
    openFileDialog.DefaultExt = "trc";
    openFileDialog.Filter = Resources.BusMonitor_OpenFileDialogFilter;
    openFileDialog.Title = Resources.BusMonitor_OpenFileDialogTitle;
    if (openFileDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
      return;
    Application.UseWaitCursor = true;
    this.busMonitorControl.Open(openFileDialog.FileName);
    Application.UseWaitCursor = false;
  }

  private void toolStripButtonSave_Click(object sender, EventArgs e)
  {
    SaveFileDialog saveFileDialog = new SaveFileDialog();
    if (this.busMonitorControl.IsEthernet)
    {
      saveFileDialog.DefaultExt = "pcapng";
      saveFileDialog.Filter = Resources.BusMonitor_SaveFileDialogFilterEthernet;
    }
    else
    {
      saveFileDialog.DefaultExt = "trc";
      saveFileDialog.Filter = Resources.BusMonitor_SaveFileDialogFilterCAN;
    }
    saveFileDialog.Title = Resources.BusMonitor_SaveFileDialogTitle;
    if (saveFileDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
      return;
    Application.UseWaitCursor = true;
    this.busMonitorControl.Save(saveFileDialog.FileName);
    Application.UseWaitCursor = false;
  }

  private void toolStripButtonSettings_Click(object sender, EventArgs e)
  {
    int num = (int) new BusMonitorSettingsForm().ShowDialog();
  }

  private void toolStripButtonClearContents_Click(object sender, EventArgs e)
  {
    if (this.PromptContentClear())
      return;
    this.busMonitorControl.Clear();
    this.ResetIdentifierMap();
  }

  private void toolStripTextBoxFilter_TextChanged(object sender, EventArgs e)
  {
    if (this.searchTimer.Enabled)
      this.searchTimer.Stop();
    this.searchTimer.Start();
  }

  private void SearchTimer_Elapsed(object sender, ElapsedEventArgs e)
  {
    this.searchTimer.Stop();
    this.busMonitorControl.FilterMessageDescription = this.toolStripTextBoxFilter.Text;
  }

  private void toolStripButtonStatistics_Click(object sender, EventArgs e)
  {
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      Sapi sapi = SapiManager.GlobalInstance.Sapi;
      if (sapi != null)
        ((ChannelBaseCollection) sapi.Channels).ConnectCompleteEvent -= new ConnectCompleteEventHandler(this.Channels_ConnectCompleteEvent);
      SapiManager.GlobalInstance.SapiResetCompleted -= new EventHandler(this.GlobalInstance_SapiResetCompleted);
      if (this.statisticsForm != null)
      {
        this.statisticsForm.Close();
        this.statisticsForm = (BusMonitorStatisticsForm) null;
      }
      if (this.busMonitorControl != null)
      {
        this.busMonitorControl.PropertyChanged -= new PropertyChangedEventHandler(this.busMonitorControl_PropertyChanged);
        this.busMonitorControl.MonitorException -= new EventHandler<ResultEventArgs>(this.busMonitorControl_MonitorException);
        this.busMonitorControl.StopMonitor();
      }
      if (this.searchTimer != null)
      {
        this.searchTimer.Elapsed -= new ElapsedEventHandler(this.SearchTimer_Elapsed);
        this.searchTimer.Dispose();
        this.searchTimer = (System.Timers.Timer) null;
      }
      if (this.components != null)
        this.components.Dispose();
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (BusMonitorForm));
    this.toolStrip = new ToolStrip();
    this.toolStripButtonClearContents = new ToolStripButton();
    this.toolStripButtonStart = new ToolStripButton();
    this.toolStripButtonPause = new ToolStripButton();
    this.toolStripButtonStop = new ToolStripButton();
    this.toolStripComboBoxResources = new ToolStripComboBox();
    this.toolStripSeparator1 = new ToolStripSeparator();
    this.toolStripButtonLoad = new ToolStripButton();
    this.toolStripButtonSave = new ToolStripButton();
    this.toolStripSeparator2 = new ToolStripSeparator();
    this.toolStripButtonTimestampDisplay = new ToolStripButton();
    this.toolStripButtonShowEcuNames = new ToolStripButton();
    this.toolStripButtonShowCompletePacket = new ToolStripButton();
    this.toolStripComboBoxEcuFilter = new ToolStripComboBox();
    this.toolStripLabelFilter = new ToolStripLabel();
    this.toolStripTextBoxFilter = new ToolStripTextBox();
    this.toolStripButtonSymbolic = new ToolStripButton();
    this.toolStripSeparator3 = new ToolStripSeparator();
    this.toolStripSeparator4 = new ToolStripSeparator();
    this.toolStripButtonSettings = new ToolStripButton();
    this.tableLayoutPanelProperties = new TableLayoutPanel();
    this.toolStripButtonStatistics = new ToolStripButton();
    this.labelTitleLabel = new Label();
    this.labelStartTimeLabel = new Label();
    this.labelConnectionResourceLabel = new Label();
    this.labelTitle = new Label();
    this.labelStartTime = new Label();
    this.labelConnectionResource = new Label();
    this.busMonitorControl = new BusMonitorControl();
    this.toolStrip.SuspendLayout();
    this.tableLayoutPanelProperties.SuspendLayout();
    this.SuspendLayout();
    this.toolStrip.Items.AddRange(new ToolStripItem[20]
    {
      (ToolStripItem) this.toolStripButtonClearContents,
      (ToolStripItem) this.toolStripButtonStart,
      (ToolStripItem) this.toolStripButtonPause,
      (ToolStripItem) this.toolStripButtonStop,
      (ToolStripItem) this.toolStripComboBoxResources,
      (ToolStripItem) this.toolStripSeparator1,
      (ToolStripItem) this.toolStripButtonLoad,
      (ToolStripItem) this.toolStripButtonSave,
      (ToolStripItem) this.toolStripSeparator2,
      (ToolStripItem) this.toolStripButtonTimestampDisplay,
      (ToolStripItem) this.toolStripButtonShowEcuNames,
      (ToolStripItem) this.toolStripComboBoxEcuFilter,
      (ToolStripItem) this.toolStripLabelFilter,
      (ToolStripItem) this.toolStripTextBoxFilter,
      (ToolStripItem) this.toolStripButtonShowCompletePacket,
      (ToolStripItem) this.toolStripButtonSymbolic,
      (ToolStripItem) this.toolStripSeparator3,
      (ToolStripItem) this.toolStripButtonSettings,
      (ToolStripItem) this.toolStripSeparator4,
      (ToolStripItem) this.toolStripButtonStatistics
    });
    componentResourceManager.ApplyResources((object) this.toolStrip, "toolStrip");
    this.toolStrip.Name = "toolStrip";
    this.toolStripButtonClearContents.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonClearContents.Image = (Image) Resources.document_plain;
    componentResourceManager.ApplyResources((object) this.toolStripButtonClearContents, "toolStripButtonClearContents");
    this.toolStripButtonClearContents.Name = "toolStripButtonClearContents";
    this.toolStripButtonClearContents.Click += new EventHandler(this.toolStripButtonClearContents_Click);
    this.toolStripButtonStart.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonStart.Image = (Image) Resources.log_play;
    componentResourceManager.ApplyResources((object) this.toolStripButtonStart, "toolStripButtonStart");
    this.toolStripButtonStart.Name = "toolStripButtonStart";
    this.toolStripButtonStart.Click += new EventHandler(this.toolStripButtonStart_Click);
    this.toolStripButtonPause.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonPause.Image = (Image) Resources.log_pause;
    componentResourceManager.ApplyResources((object) this.toolStripButtonPause, "toolStripButtonPause");
    this.toolStripButtonPause.Name = "toolStripButtonPause";
    this.toolStripButtonPause.Click += new EventHandler(this.toolStripButtonPause_Click);
    this.toolStripButtonStop.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonStop.Image = (Image) Resources.log_stop;
    componentResourceManager.ApplyResources((object) this.toolStripButtonStop, "toolStripButtonStop");
    this.toolStripButtonStop.Name = "toolStripButtonStop";
    this.toolStripButtonStop.Click += new EventHandler(this.toolStripButtonStop_Click);
    this.toolStripComboBoxResources.DropDownStyle = ComboBoxStyle.DropDownList;
    this.toolStripComboBoxResources.Name = "toolStripComboBoxResources";
    componentResourceManager.ApplyResources((object) this.toolStripComboBoxResources, "toolStripComboBoxResources");
    this.toolStripComboBoxResources.DropDown += new EventHandler(this.toolStripComboBoxResources_DropDown);
    this.toolStripSeparator1.Name = "toolStripSeparator1";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator1, "toolStripSeparator1");
    this.toolStripButtonLoad.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonLoad.Image = (Image) Resources.folder_out;
    componentResourceManager.ApplyResources((object) this.toolStripButtonLoad, "toolStripButtonLoad");
    this.toolStripButtonLoad.Name = "toolStripButtonLoad";
    this.toolStripButtonLoad.Click += new EventHandler(this.toolStripButtonLoad_Click);
    this.toolStripButtonSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonSave.Image = (Image) Resources.disk_blue;
    componentResourceManager.ApplyResources((object) this.toolStripButtonSave, "toolStripButtonSave");
    this.toolStripButtonSave.Name = "toolStripButtonSave";
    this.toolStripButtonSave.Click += new EventHandler(this.toolStripButtonSave_Click);
    this.toolStripSeparator2.Name = "toolStripSeparator2";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator2, "toolStripSeparator2");
    this.toolStripButtonTimestampDisplay.CheckOnClick = true;
    this.toolStripButtonTimestampDisplay.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonTimestampDisplay.Image = (Image) Resources.clock_refresh;
    componentResourceManager.ApplyResources((object) this.toolStripButtonTimestampDisplay, "toolStripButtonTimestampDisplay");
    this.toolStripButtonTimestampDisplay.Name = "toolStripButtonTimestampDisplay";
    this.toolStripButtonTimestampDisplay.CheckStateChanged += new EventHandler(this.toolStripButtonTimestampDisplay_CheckStateChanged);
    this.toolStripButtonShowEcuNames.Checked = true;
    this.toolStripButtonShowEcuNames.CheckOnClick = true;
    this.toolStripButtonShowEcuNames.CheckState = CheckState.Checked;
    this.toolStripButtonShowEcuNames.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonShowEcuNames.Image = (Image) Resources.ViewAll;
    componentResourceManager.ApplyResources((object) this.toolStripButtonShowEcuNames, "toolStripButtonShowEcuNames");
    this.toolStripButtonShowEcuNames.Name = "toolStripButtonShowEcuNames";
    this.toolStripButtonShowEcuNames.CheckStateChanged += new EventHandler(this.toolStripButtonShowEcuNames_CheckStateChanged);
    this.toolStripButtonShowCompletePacket.Checked = false;
    this.toolStripButtonShowCompletePacket.CheckOnClick = true;
    this.toolStripButtonShowCompletePacket.CheckState = CheckState.Unchecked;
    this.toolStripButtonShowCompletePacket.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonShowCompletePacket.Image = (Image) Resources.row;
    componentResourceManager.ApplyResources((object) this.toolStripButtonShowCompletePacket, "toolStripButtonShowCompletePacket");
    this.toolStripButtonShowCompletePacket.Name = "toolStripButtonShowCompletePacket";
    this.toolStripButtonShowCompletePacket.Enabled = false;
    this.toolStripButtonShowCompletePacket.CheckStateChanged += new EventHandler(this.toolStripButtonShowCompletePacket_CheckStateChanged);
    this.toolStripComboBoxEcuFilter.DropDownStyle = ComboBoxStyle.DropDownList;
    this.toolStripComboBoxEcuFilter.Name = "toolStripComboBoxEcuFilter";
    componentResourceManager.ApplyResources((object) this.toolStripComboBoxEcuFilter, "toolStripComboBoxEcuFilter");
    this.toolStripComboBoxEcuFilter.DropDown += new EventHandler(this.toolStripComboBoxEcuFilter_DropDown);
    this.toolStripComboBoxEcuFilter.SelectedIndexChanged += new EventHandler(this.toolStripComboBoxEcuFilter_SelectedIndexChanged);
    this.toolStripLabelFilter.Name = "toolStripLabelFilter";
    componentResourceManager.ApplyResources((object) this.toolStripLabelFilter, "toolStripLabelFilter");
    this.toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
    componentResourceManager.ApplyResources((object) this.toolStripTextBoxFilter, "toolStripTextBoxFilter");
    this.toolStripTextBoxFilter.TextChanged += new EventHandler(this.toolStripTextBoxFilter_TextChanged);
    this.toolStripButtonSymbolic.CheckOnClick = true;
    this.toolStripButtonSymbolic.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonSymbolic.Image = (Image) Resources.about;
    componentResourceManager.ApplyResources((object) this.toolStripButtonSymbolic, "toolStripButtonSymbolic");
    this.toolStripButtonSymbolic.Name = "toolStripButtonSymbolic";
    this.toolStripButtonSymbolic.CheckStateChanged += new EventHandler(this.toolStripButtonSymbolic_CheckStateChanged);
    this.toolStripSeparator3.Name = "toolStripSeparator3";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator3, "toolStripSeparator3");
    this.toolStripSeparator4.Name = "toolStripSeparator4";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator4, "toolStripSeparator4");
    this.toolStripButtonSettings.DisplayStyle = ToolStripItemDisplayStyle.Image;
    this.toolStripButtonSettings.Image = (Image) Resources.gear;
    componentResourceManager.ApplyResources((object) this.toolStripButtonSettings, "toolStripButtonSettings");
    this.toolStripButtonSettings.Name = "toolStripButtonSettings";
    this.toolStripButtonSettings.Click += new EventHandler(this.toolStripButtonSettings_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelProperties, "tableLayoutPanelProperties");
    this.tableLayoutPanelProperties.Controls.Add((Control) this.labelTitleLabel, 0, 0);
    this.tableLayoutPanelProperties.Controls.Add((Control) this.labelStartTimeLabel, 0, 1);
    this.tableLayoutPanelProperties.Controls.Add((Control) this.labelConnectionResourceLabel, 0, 2);
    this.tableLayoutPanelProperties.Controls.Add((Control) this.labelTitle, 1, 0);
    this.tableLayoutPanelProperties.Controls.Add((Control) this.labelStartTime, 1, 1);
    this.tableLayoutPanelProperties.Controls.Add((Control) this.labelConnectionResource, 1, 2);
    this.tableLayoutPanelProperties.Name = "tableLayoutPanelProperties";
    this.toolStripButtonStatistics.DisplayStyle = ToolStripItemDisplayStyle.Image;
    componentResourceManager.ApplyResources((object) this.toolStripButtonStatistics, "toolStripButtonStatistics");
    this.toolStripButtonStatistics.Name = "toolStripButtonStatistics";
    this.toolStripButtonStatistics.Click += new EventHandler(this.toolStripButtonStatistics_Click);
    componentResourceManager.ApplyResources((object) this.labelTitleLabel, "labelTitleLabel");
    this.labelTitleLabel.Name = "labelTitleLabel";
    componentResourceManager.ApplyResources((object) this.labelStartTimeLabel, "labelStartTimeLabel");
    this.labelStartTimeLabel.Name = "labelStartTimeLabel";
    componentResourceManager.ApplyResources((object) this.labelConnectionResourceLabel, "labelConnectionResourceLabel");
    this.labelConnectionResourceLabel.Name = "labelConnectionResourceLabel";
    componentResourceManager.ApplyResources((object) this.labelTitle, "labelTitle");
    this.labelTitle.Name = "labelTitle";
    componentResourceManager.ApplyResources((object) this.labelStartTime, "labelStartTime");
    this.labelStartTime.Name = "labelStartTime";
    componentResourceManager.ApplyResources((object) this.labelConnectionResource, "labelConnectionResource");
    this.labelConnectionResource.Name = "labelConnectionResource";
    this.busMonitorControl.DisplayAbsoluteTimestamp = false;
    componentResourceManager.ApplyResources((object) this.busMonitorControl, "busMonitorControl");
    this.busMonitorControl.Filter = (BusMonitorControlFilterType) 0;
    this.busMonitorControl.FilterEcu = (string) null;
    this.busMonitorControl.FilterMessageDescription = (string) null;
    ((Control) this.busMonitorControl).Name = "busMonitorControl";
    this.busMonitorControl.ShowEcuNames = true;
    this.busMonitorControl.ShowSymbolic = false;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.busMonitorControl);
    this.Controls.Add((Control) this.tableLayoutPanelProperties);
    this.Controls.Add((Control) this.toolStrip);
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (BusMonitorForm);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.toolStrip.ResumeLayout(false);
    this.toolStrip.PerformLayout();
    this.tableLayoutPanelProperties.ResumeLayout(false);
    this.tableLayoutPanelProperties.PerformLayout();
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  private static class NativeMethods
  {
    public const int GWL_HWNDPARENT = -8;

    public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
    {
      return IntPtr.Size == 8 ? BusMonitorForm.NativeMethods.SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : new IntPtr(BusMonitorForm.NativeMethods.SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
  }
}
