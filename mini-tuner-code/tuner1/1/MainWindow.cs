// Decompiled with JetBrains decompiler
// Type: TunerSolution.MainWindow
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using Microsoft.VisualBasic;
using Microsoft.Win32;
using rp1210;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using TunerSolution.Properties;
using Vehicle_Applications;

#nullable disable
namespace TunerSolution;

public class MainWindow : Window, IComponentConnector
{
  private bool bConnected;
  private RP1210BDriverInfo driverInfo;
  private bool SniffStarted;
  private Flasher Vehicle;
  private BackgroundWorker SniffWorker;
  internal Button btnQuery;
  internal ComboBox cmbDriverList;
  internal ComboBox cmbDeviceList;
  internal TextBox txtStatus;
  internal ProgressBar prgBar1;
  internal ComboBox cmbSpecificVehicle;
  internal ComboBox cmbTruckList;
  internal TextBox txtBoxFileName;
  internal Button btnFlashFile;
  internal Button btnLoadFile;
  internal Button btSniff;
  internal TextBox txtBoxRestore;
  internal Button btnWriteRestore;
  internal Button btnReadRestore;
  internal Button btnLoadParamFile;
  internal RadioButton RadioFullFlash;
  internal RadioButton RadioCalibration;
  internal CheckBox CheckBoxJ1708;
  internal CheckBox CheckBoxJ1939;
  internal Button btSave;
  internal Button btnReadFile;
  internal TextBox txtBoxStartAddr;
  internal TextBox txtBoxLength;
  internal RadioButton radio250k;
  internal RadioButton radio500k;
  private bool _contentLoaded;

  public MainWindow() => this.InitializeComponent();

  private void ProgressChanged(object sender, ProgressChangedEventArgs e)
  {
    this.prgBar1.Value = (double) e.ProgressPercentage;
    this.txtStatus.Text = e.UserState.ToString();
  }

  private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
  {
    if (e.Error == null)
    {
      this.prgBar1.Value = 100.0;
      this.txtStatus.Text = e.Result.ToString();
      this.btnQuery.Content = (object) "Disconnect";
      this.bConnected = true;
    }
    else
    {
      this.prgBar1.Value = 0.0;
      this.txtStatus.Text = "Error: " + e.Error.Message;
      this.bConnected = false;
      this.btnQuery.Content = (object) "Query";
    }
  }

  private void QueryCompleted(object sender, RunWorkerCompletedEventArgs e)
  {
    if (e.Error == null)
    {
      this.prgBar1.Value = 100.0;
      this.txtStatus.Text = e.Result.ToString();
      this.btnQuery.Content = (object) "Disconnect";
      if (this.cmbTruckList.SelectedItem.ToString() == "Paccar")
      {
        this.btnReadFile.IsEnabled = true;
        if (this.Vehicle.ecmModel.StartsWith("PC4__1408"))
        {
          this.txtBoxStartAddr.Text = "240000";
          this.txtBoxLength.Text = "2FF000";
        }
        else
        {
          this.txtBoxStartAddr.Text = "200000";
          this.txtBoxLength.Text = "2FF000";
        }
        if (this.Vehicle.ecmModel.StartsWith("PC4_A14"))
        {
          int num = (int) MessageBox.Show("This ECM Uses the New Style Bootloader, Contact Support");
        }
      }
      this.bConnected = true;
    }
    else
    {
      this.prgBar1.Value = 0.0;
      this.txtStatus.Text = "Error: " + e.Error.Message;
      this.bConnected = false;
      this.btnQuery.Content = (object) "Query";
    }
  }

  private void SniffProgressChanged(object sender, ProgressChangedEventArgs e)
  {
    this.prgBar1.Value = (double) e.ProgressPercentage;
    TextBox txtStatus = this.txtStatus;
    txtStatus.Text = txtStatus.Text + e.UserState.ToString() + Environment.NewLine;
  }

  private void SniffWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
  {
    if (e.Error == null)
    {
      this.prgBar1.Value = 100.0;
      this.btSniff.Content = (object) "Sniffer";
    }
    else
    {
      this.prgBar1.Value = 0.0;
      this.txtStatus.Text = "Error: " + e.Error.Message;
    }
  }

  private void Grid_Loaded(object sender, RoutedEventArgs e)
  {
    this.cmbDriverList.ItemsSource = (IEnumerable) RP121032.ScanForDrivers();
    this.cmbDriverList.SelectedIndex = Settings.Default.selectedDriver;
    this.cmbDeviceList.SelectedIndex = Settings.Default.selectedDevice;
    this.cmbTruckList.SelectedIndex = Settings.Default.selectedTruck;
    this.txtBoxFileName.Text = Settings.Default.fileName;
    this.txtBoxRestore.Text = Settings.Default.parameterFileName;
  }

  private void btnLoadFile_Click_1(object sender, RoutedEventArgs e)
  {
    OpenFileDialog openFileDialog = new OpenFileDialog();
    openFileDialog.Filter = "Tuning Files | *.bin";
    bool? nullable = openFileDialog.ShowDialog();
    bool flag = true;
    if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
      return;
    this.txtBoxFileName.Text = openFileDialog.FileName;
    Settings.Default.fileName = openFileDialog.FileName;
    Settings.Default.Save();
    if (!this.bConnected)
      return;
    this.btnFlashFile.IsEnabled = true;
  }

  private void cmbTruckList_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    switch (this.cmbTruckList.SelectedItem.ToString())
    {
      case "Maxxforce":
        this.cmbSpecificVehicle.Items.Clear();
        foreach (MaxxModuleType newItem in Enum.GetValues(typeof (MaxxModuleType)))
          this.cmbSpecificVehicle.Items.Add((object) newItem);
        this.cmbSpecificVehicle.IsEnabled = true;
        this.cmbSpecificVehicle.SelectedIndex = Settings.Default.selectedSpecific;
        break;
      case "DetroitMCM":
        this.cmbSpecificVehicle.Items.Clear();
        foreach (DetroitMCMModuleType newItem in Enum.GetValues(typeof (DetroitMCMModuleType)))
          this.cmbSpecificVehicle.Items.Add((object) newItem);
        this.cmbSpecificVehicle.IsEnabled = true;
        this.cmbSpecificVehicle.SelectedIndex = Settings.Default.selectedSpecific;
        break;
      case "DetroitACM":
        this.cmbSpecificVehicle.Items.Clear();
        foreach (DetroitMCMModuleType newItem in Enum.GetValues(typeof (DetroitMCMModuleType)))
          this.cmbSpecificVehicle.Items.Add((object) newItem);
        this.cmbSpecificVehicle.IsEnabled = true;
        this.cmbSpecificVehicle.SelectedIndex = Settings.Default.selectedSpecific;
        break;
      default:
        try
        {
          if (!this.cmbSpecificVehicle.IsEnabled)
            break;
          this.cmbSpecificVehicle.Items.Clear();
          this.cmbSpecificVehicle.IsEnabled = false;
          break;
        }
        catch (Exception ex)
        {
          break;
        }
    }
  }

  private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    if (this.bConnected)
      return;
    this.driverInfo = RP121032.LoadDeviceParameters($"{Environment.GetEnvironmentVariable("SystemRoot")}\\{this.cmbDriverList.SelectedItem.ToString()}.ini");
    this.cmbDeviceList.ItemsSource = (IEnumerable) this.driverInfo.RP1210Devices;
  }

  private void btSniff_Click(object sender, RoutedEventArgs e)
  {
    Settings.Default.selectedDriver = this.cmbDriverList.SelectedIndex;
    Settings.Default.selectedDevice = this.cmbDeviceList.SelectedIndex;
    Settings.Default.selectedTruck = this.cmbTruckList.SelectedIndex;
    Settings.Default.selectedSpecific = this.cmbSpecificVehicle.SelectedIndex;
    Settings.Default.Save();
    try
    {
      this.btSniff.Content = (object) "Stop";
      if (!this.SniffStarted)
      {
        if (this.Vehicle == null)
          this.Vehicle = new Flasher(new RP121032(this.cmbDriverList.SelectedItem.ToString()), new RP121032(this.cmbDriverList.SelectedItem.ToString()), new RP121032(this.cmbDriverList.SelectedItem.ToString()));
        DeviceInfo selectedValue = (DeviceInfo) this.cmbDeviceList.SelectedValue;
        try
        {
          this.txtStatus.Clear();
          this.SniffStarted = true;
          this.SniffWorker = new BackgroundWorker();
          this.SniffWorker.WorkerReportsProgress = true;
          this.SniffWorker.WorkerSupportsCancellation = true;
          this.SniffWorker.DoWork += new DoWorkEventHandler(this.Vehicle.Sniffer);
          this.SniffWorker.ProgressChanged += new ProgressChangedEventHandler(this.SniffProgressChanged);
          this.SniffWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.SniffWorkerCompleted);
          DeviceInfo selectedDevice = selectedValue;
          bool? isChecked = this.CheckBoxJ1708.IsChecked;
          int num1 = isChecked.Value ? 1 : 0;
          isChecked = this.CheckBoxJ1939.IsChecked;
          int num2 = isChecked.Value ? 1 : 0;
          this.SniffWorker.RunWorkerAsync((object) new SnifferArgs(selectedDevice, num1 != 0, num2 != 0));
        }
        catch (Exception ex)
        {
          this.txtStatus.Text = ex.Message;
        }
      }
      else
      {
        this.SniffStarted = false;
        this.SniffWorker.CancelAsync();
        this.SniffWorker.Dispose();
      }
    }
    catch (Exception ex)
    {
      this.txtStatus.Text = ex.Message;
    }
  }

  private void btnQuery_Click(object sender, RoutedEventArgs e)
  {
    Settings.Default.selectedDriver = this.cmbDriverList.SelectedIndex;
    Settings.Default.selectedDevice = this.cmbDeviceList.SelectedIndex;
    Settings.Default.selectedTruck = this.cmbTruckList.SelectedIndex;
    Settings.Default.selectedSpecific = this.cmbSpecificVehicle.SelectedIndex;
    Settings.Default.Save();
    this.txtStatus.Text = "Connecting....";
    if (this.bConnected)
    {
      if (this.Vehicle.J1708inst != null)
        this.Vehicle.J1708inst = (RP121032) null;
      if (this.Vehicle.J1939inst != null)
        this.Vehicle.J1939inst = (RP121032) null;
      if (this.Vehicle.ISO15765inst != null)
        this.Vehicle.ISO15765inst = (RP121032) null;
      this.btnQuery.Content = (object) "Connect";
      this.bConnected = false;
    }
    else
    {
      this.Vehicle = new Flasher(new RP121032(this.cmbDriverList.SelectedItem.ToString()), new RP121032(this.cmbDriverList.SelectedItem.ToString()), new RP121032(this.cmbDriverList.SelectedItem.ToString()));
      DeviceInfo selectedValue = (DeviceInfo) this.cmbDeviceList.SelectedValue;
      bool? isChecked = this.radio250k.IsChecked;
      bool flag = true;
      selectedValue.SelectedRate = !(isChecked.GetValueOrDefault() == flag & isChecked.HasValue) ? BaudRate.Can500 : BaudRate.Can250;
      try
      {
        switch (this.cmbTruckList.SelectedItem.ToString())
        {
          case "Paccar":
            BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(this.Vehicle.PaccarQuery);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.QueryCompleted);
            backgroundWorker1.RunWorkerAsync((object) selectedValue);
            break;
          case "DetroitACM":
            BackgroundWorker backgroundWorker2 = new BackgroundWorker();
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.DoWork += new DoWorkEventHandler(this.Vehicle.DetroitACMQuery);
            backgroundWorker2.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
            backgroundWorker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.QueryCompleted);
            backgroundWorker2.RunWorkerAsync((object) selectedValue);
            break;
          case "DetroitMCM":
            BackgroundWorker backgroundWorker3 = new BackgroundWorker();
            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.DoWork += new DoWorkEventHandler(this.Vehicle.DetroitMCMQuery);
            backgroundWorker3.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
            backgroundWorker3.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.QueryCompleted);
            backgroundWorker3.RunWorkerAsync((object) new DetroitMCMAsyncArgs((DetroitMCMModuleType) this.cmbSpecificVehicle.SelectedItem, (string) null, selectedValue, this.RadioFullFlash.IsChecked.Value));
            break;
          case "Maxxforce":
            BackgroundWorker backgroundWorker4 = new BackgroundWorker();
            backgroundWorker4.WorkerReportsProgress = true;
            backgroundWorker4.DoWork += new DoWorkEventHandler(this.Vehicle.MaxxforceQuery);
            backgroundWorker4.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
            backgroundWorker4.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.QueryCompleted);
            backgroundWorker4.RunWorkerAsync((object) new MaxxAsyncArgs((MaxxModuleType) this.cmbSpecificVehicle.SelectedItem, (string) null, selectedValue, false));
            break;
          case "CaterpillarAG":
            BackgroundWorker backgroundWorker5 = new BackgroundWorker();
            backgroundWorker5.WorkerReportsProgress = true;
            backgroundWorker5.DoWork += new DoWorkEventHandler(this.Vehicle.CatAGQuery);
            backgroundWorker5.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
            backgroundWorker5.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.QueryCompleted);
            backgroundWorker5.RunWorkerAsync((object) selectedValue);
            break;
          default:
            throw new Exception("Incompatible Truck Selected");
        }
      }
      catch (Exception ex)
      {
        this.txtStatus.Text = ex.Message;
      }
      if (this.txtBoxFileName.Text != "")
        this.btnFlashFile.IsEnabled = true;
      if (this.txtBoxRestore.Text != "")
        this.btnWriteRestore.IsEnabled = true;
      this.btnReadRestore.IsEnabled = true;
      this.btSniff.IsEnabled = false;
    }
  }

  private void btnFlashFile_Click(object sender, RoutedEventArgs e)
  {
    try
    {
      switch (this.cmbTruckList.SelectedItem.ToString())
      {
        case "Paccar":
          BackgroundWorker backgroundWorker1 = new BackgroundWorker();
          backgroundWorker1.WorkerReportsProgress = true;
          backgroundWorker1.DoWork += new DoWorkEventHandler(this.Vehicle.PaccarFlash);
          backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
          backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
          backgroundWorker1.RunWorkerAsync((object) new Flasher.AddressRange(Convert.ToUInt32(this.txtBoxStartAddr.Text, 16 /*0x10*/), Convert.ToUInt32(this.txtBoxLength.Text, 16 /*0x10*/), this.txtBoxFileName.Text));
          break;
        case "DetroitACM":
          BackgroundWorker backgroundWorker2 = new BackgroundWorker();
          backgroundWorker2.WorkerReportsProgress = true;
          backgroundWorker2.DoWork += new DoWorkEventHandler(this.Vehicle.DetroitACMFlash);
          backgroundWorker2.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
          backgroundWorker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
          backgroundWorker2.RunWorkerAsync((object) new DetroitMCMAsyncArgs((DetroitMCMModuleType) this.cmbSpecificVehicle.SelectedItem, this.txtBoxFileName.Text, (DeviceInfo) null, this.RadioFullFlash.IsChecked.Value));
          break;
        case "DetroitMCM":
          BackgroundWorker backgroundWorker3 = new BackgroundWorker();
          backgroundWorker3.WorkerReportsProgress = true;
          backgroundWorker3.DoWork += new DoWorkEventHandler(this.Vehicle.DetroitMCMFlash);
          backgroundWorker3.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
          backgroundWorker3.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
          backgroundWorker3.RunWorkerAsync((object) new DetroitMCMAsyncArgs((DetroitMCMModuleType) this.cmbSpecificVehicle.SelectedItem, this.txtBoxFileName.Text, (DeviceInfo) null, this.RadioFullFlash.IsChecked.Value));
          break;
        case "Maxxforce":
          BackgroundWorker backgroundWorker4 = new BackgroundWorker();
          backgroundWorker4.WorkerReportsProgress = true;
          backgroundWorker4.DoWork += new DoWorkEventHandler(this.Vehicle.MaxxforceFlash);
          backgroundWorker4.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
          backgroundWorker4.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
          backgroundWorker4.RunWorkerAsync((object) new MaxxAsyncArgs((MaxxModuleType) this.cmbSpecificVehicle.SelectedItem, this.txtBoxFileName.Text, (DeviceInfo) null, this.RadioFullFlash.IsChecked.Value));
          break;
        default:
          throw new Exception("Incompatible Truck Selected");
      }
    }
    catch (Exception ex)
    {
      this.txtStatus.Text = ex.Message;
    }
  }

  private void btnLoadParamFile_Click(object sender, RoutedEventArgs e)
  {
    OpenFileDialog openFileDialog = new OpenFileDialog();
    openFileDialog.Filter = "Restore Files|*.txt";
    bool? nullable = openFileDialog.ShowDialog();
    bool flag = true;
    if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
      return;
    this.txtBoxRestore.Text = openFileDialog.FileName;
    Settings.Default.parameterFileName = openFileDialog.FileName;
    Settings.Default.Save();
    if (!this.bConnected)
      return;
    this.btnWriteRestore.IsEnabled = true;
  }

  private void btnReadRestore_Click(object sender, RoutedEventArgs e)
  {
    try
    {
      if (!(this.cmbTruckList.SelectedItem.ToString() == "Maxxforce"))
        throw new Exception("Vehicle Not Implemented");
      BackgroundWorker backgroundWorker = new BackgroundWorker();
      backgroundWorker.WorkerReportsProgress = true;
      backgroundWorker.DoWork += new DoWorkEventHandler(this.Vehicle.MaxxforceReadRestore);
      backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
      backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
      backgroundWorker.RunWorkerAsync((object) new MaxxAsyncArgs((MaxxModuleType) this.cmbSpecificVehicle.SelectedItem, this.txtBoxRestore.Text, (DeviceInfo) null, false));
    }
    catch (Exception ex)
    {
      this.txtStatus.Text = ex.Message;
    }
  }

  private void btnWriteRestore_Click(object sender, RoutedEventArgs e)
  {
    try
    {
      if (!(this.cmbTruckList.SelectedItem.ToString() == "Maxxforce"))
        throw new Exception("Vehicle Not Implemented");
      BackgroundWorker backgroundWorker = new BackgroundWorker();
      backgroundWorker.WorkerReportsProgress = true;
      backgroundWorker.DoWork += new DoWorkEventHandler(this.Vehicle.MaxxforceWriteRestore);
      backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
      backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
      backgroundWorker.RunWorkerAsync((object) new MaxxAsyncArgs((MaxxModuleType) this.cmbSpecificVehicle.SelectedItem, this.txtBoxRestore.Text, (DeviceInfo) null, false));
    }
    catch (Exception ex)
    {
      this.txtStatus.Text = ex.Message;
    }
  }

  private void btSave_Click(object sender, RoutedEventArgs e)
  {
    SaveFileDialog saveFileDialog = new SaveFileDialog();
    saveFileDialog.Filter = "Sniff Files|*.txt";
    bool? nullable = saveFileDialog.ShowDialog();
    bool flag = true;
    if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
      return;
    File.WriteAllText(saveFileDialog.FileName, this.txtStatus.Text);
  }

  private void BtnReadFile_Click(object sender, RoutedEventArgs e)
  {
    this.btnReadFile.IsEnabled = false;
    try
    {
      if (!(this.cmbTruckList.SelectedItem.ToString() == "Paccar"))
        throw new Exception("No Read Available");
      BackgroundWorker backgroundWorker = new BackgroundWorker();
      backgroundWorker.WorkerReportsProgress = true;
      backgroundWorker.DoWork += new DoWorkEventHandler(this.Vehicle.PaccarRead);
      backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
      backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
      backgroundWorker.RunWorkerAsync((object) new Flasher.AddressRange(Convert.ToUInt32(this.txtBoxStartAddr.Text, 16 /*0x10*/), Convert.ToUInt32(this.txtBoxLength.Text, 16 /*0x10*/), this.txtBoxFileName.Text));
    }
    catch (Exception ex)
    {
      this.txtStatus.Text = ex.Message;
    }
  }

  private void BtDetDel_Click(object sender, RoutedEventArgs e)
  {
    this.btnReadFile.IsEnabled = false;
    try
    {
      BackgroundWorker backgroundWorker = new BackgroundWorker();
      backgroundWorker.WorkerReportsProgress = true;
      backgroundWorker.DoWork += new DoWorkEventHandler(this.Vehicle.DetroitDelete);
      backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
      backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
      backgroundWorker.RunWorkerAsync();
    }
    catch (Exception ex)
    {
      this.txtStatus.Text = ex.Message;
    }
  }

  private void CmbDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
  }

  private void CmbSpecificVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
  }

  private void Window_Loaded(object sender, RoutedEventArgs e)
  {
  }

  public string RandomString(int size, bool lowerCase)
  {
    StringBuilder stringBuilder = new StringBuilder();
    Random random = new Random();
    for (int index = 0; index < size; ++index)
    {
      char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
      stringBuilder.Append(ch);
    }
    return lowerCase ? stringBuilder.ToString().ToLower() : stringBuilder.ToString();
  }

  private void Window_Initialized(object sender, EventArgs e)
  {
    string str = this.RandomString(10, false);
    if (!(Interaction.InputBox(str, "Password", str) != BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(str))).Replace("-", string.Empty)))
      return;
    this.Close();
  }

  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public void InitializeComponent()
  {
    if (this._contentLoaded)
      return;
    this._contentLoaded = true;
    Application.LoadComponent((object) this, new Uri("/TunerSolution;component/mainwindow.xaml", UriKind.Relative));
  }

  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  void IComponentConnector.Connect(int connectionId, object target)
  {
    switch (connectionId)
    {
      case 1:
        ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Window_Loaded);
        ((FrameworkElement) target).Initialized += new EventHandler(this.Window_Initialized);
        break;
      case 2:
        ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Grid_Loaded);
        break;
      case 3:
        this.btnQuery = (Button) target;
        this.btnQuery.Click += new RoutedEventHandler(this.btnQuery_Click);
        break;
      case 4:
        this.cmbDriverList = (ComboBox) target;
        this.cmbDriverList.SelectionChanged += new SelectionChangedEventHandler(this.comboBox1_SelectionChanged);
        break;
      case 5:
        this.cmbDeviceList = (ComboBox) target;
        this.cmbDeviceList.SelectionChanged += new SelectionChangedEventHandler(this.CmbDeviceList_SelectionChanged);
        break;
      case 6:
        this.txtStatus = (TextBox) target;
        break;
      case 7:
        this.prgBar1 = (ProgressBar) target;
        break;
      case 8:
        this.cmbSpecificVehicle = (ComboBox) target;
        this.cmbSpecificVehicle.SelectionChanged += new SelectionChangedEventHandler(this.CmbSpecificVehicle_SelectionChanged);
        break;
      case 9:
        this.cmbTruckList = (ComboBox) target;
        this.cmbTruckList.SelectionChanged += new SelectionChangedEventHandler(this.cmbTruckList_SelectionChanged);
        break;
      case 10:
        this.txtBoxFileName = (TextBox) target;
        break;
      case 11:
        this.btnFlashFile = (Button) target;
        this.btnFlashFile.Click += new RoutedEventHandler(this.btnFlashFile_Click);
        break;
      case 12:
        this.btnLoadFile = (Button) target;
        this.btnLoadFile.Click += new RoutedEventHandler(this.btnLoadFile_Click_1);
        break;
      case 13:
        this.btSniff = (Button) target;
        this.btSniff.Click += new RoutedEventHandler(this.btSniff_Click);
        break;
      case 14:
        this.txtBoxRestore = (TextBox) target;
        break;
      case 15:
        this.btnWriteRestore = (Button) target;
        this.btnWriteRestore.Click += new RoutedEventHandler(this.btnWriteRestore_Click);
        break;
      case 16 /*0x10*/:
        this.btnReadRestore = (Button) target;
        this.btnReadRestore.Click += new RoutedEventHandler(this.btnReadRestore_Click);
        break;
      case 17:
        this.btnLoadParamFile = (Button) target;
        this.btnLoadParamFile.Click += new RoutedEventHandler(this.btnLoadParamFile_Click);
        break;
      case 18:
        this.RadioFullFlash = (RadioButton) target;
        break;
      case 19:
        this.RadioCalibration = (RadioButton) target;
        break;
      case 20:
        this.CheckBoxJ1708 = (CheckBox) target;
        break;
      case 21:
        this.CheckBoxJ1939 = (CheckBox) target;
        break;
      case 22:
        this.btSave = (Button) target;
        this.btSave.Click += new RoutedEventHandler(this.btSave_Click);
        break;
      case 23:
        this.btnReadFile = (Button) target;
        this.btnReadFile.Click += new RoutedEventHandler(this.BtnReadFile_Click);
        break;
      case 24:
        this.txtBoxStartAddr = (TextBox) target;
        break;
      case 25:
        this.txtBoxLength = (TextBox) target;
        break;
      case 26:
        this.radio250k = (RadioButton) target;
        break;
      case 27:
        this.radio500k = (RadioButton) target;
        break;
      default:
        this._contentLoaded = true;
        break;
    }
  }
}
