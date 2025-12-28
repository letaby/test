using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using rp1210;
using TunerSolution.Properties;
using Vehicle_Applications;

namespace TunerSolution;

public partial class MainWindow : Window, IComponentConnector
{
	private bool bConnected;

	private RP1210BDriverInfo driverInfo;

	private bool SniffStarted;

	private Flasher Vehicle;

	private BackgroundWorker SniffWorker;

	public MainWindow()
	{
		InitializeComponent();
	}

	private void ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		prgBar1.Value = e.ProgressPercentage;
		txtStatus.Text = e.UserState.ToString();
	}

	private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error == null)
		{
			prgBar1.Value = 100.0;
			txtStatus.Text = e.Result.ToString();
			btnQuery.Content = "Disconnect";
			bConnected = true;
		}
		else
		{
			prgBar1.Value = 0.0;
			txtStatus.Text = "Error: " + e.Error.Message;
			bConnected = false;
			btnQuery.Content = "Query";
		}
	}

	private void QueryCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error == null)
		{
			prgBar1.Value = 100.0;
			txtStatus.Text = e.Result.ToString();
			btnQuery.Content = "Disconnect";
			if (cmbTruckList.SelectedItem.ToString() == "Paccar")
			{
				btnReadFile.IsEnabled = true;
				if (Vehicle.ecmModel.StartsWith("PC4__1408"))
				{
					txtBoxStartAddr.Text = "240000";
					txtBoxLength.Text = "2FF000";
				}
				else
				{
					txtBoxStartAddr.Text = "200000";
					txtBoxLength.Text = "2FF000";
				}
				if (Vehicle.ecmModel.StartsWith("PC4_A14"))
				{
					MessageBox.Show("This ECM Uses the New Style Bootloader, Contact Support");
				}
			}
			bConnected = true;
		}
		else
		{
			prgBar1.Value = 0.0;
			txtStatus.Text = "Error: " + e.Error.Message;
			bConnected = false;
			btnQuery.Content = "Query";
		}
	}

	private void SniffProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		prgBar1.Value = e.ProgressPercentage;
		TextBox textBox = txtStatus;
		textBox.Text = textBox.Text + e.UserState.ToString() + Environment.NewLine;
	}

	private void SniffWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error == null)
		{
			prgBar1.Value = 100.0;
			btSniff.Content = "Sniffer";
		}
		else
		{
			prgBar1.Value = 0.0;
			txtStatus.Text = "Error: " + e.Error.Message;
		}
	}

	private void Grid_Loaded(object sender, RoutedEventArgs e)
	{
		List<string> itemsSource = RP121032.ScanForDrivers();
		cmbDriverList.ItemsSource = itemsSource;
		cmbDriverList.SelectedIndex = Settings.Default.selectedDriver;
		cmbDeviceList.SelectedIndex = Settings.Default.selectedDevice;
		cmbTruckList.SelectedIndex = Settings.Default.selectedTruck;
		txtBoxFileName.Text = Settings.Default.fileName;
		txtBoxRestore.Text = Settings.Default.parameterFileName;
	}

	private void btnLoadFile_Click_1(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Tuning Files | *.bin";
		if (openFileDialog.ShowDialog() == true)
		{
			txtBoxFileName.Text = openFileDialog.FileName;
			Settings.Default.fileName = openFileDialog.FileName;
			Settings.Default.Save();
			if (bConnected)
			{
				btnFlashFile.IsEnabled = true;
			}
		}
	}

	private void cmbTruckList_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		switch (cmbTruckList.SelectedItem.ToString())
		{
		case "Maxxforce":
			cmbSpecificVehicle.Items.Clear();
			foreach (MaxxModuleType value in Enum.GetValues(typeof(MaxxModuleType)))
			{
				cmbSpecificVehicle.Items.Add(value);
			}
			cmbSpecificVehicle.IsEnabled = true;
			cmbSpecificVehicle.SelectedIndex = Settings.Default.selectedSpecific;
			return;
		case "DetroitMCM":
			cmbSpecificVehicle.Items.Clear();
			foreach (DetroitMCMModuleType value2 in Enum.GetValues(typeof(DetroitMCMModuleType)))
			{
				cmbSpecificVehicle.Items.Add(value2);
			}
			cmbSpecificVehicle.IsEnabled = true;
			cmbSpecificVehicle.SelectedIndex = Settings.Default.selectedSpecific;
			return;
		case "DetroitACM":
			cmbSpecificVehicle.Items.Clear();
			foreach (DetroitMCMModuleType value3 in Enum.GetValues(typeof(DetroitMCMModuleType)))
			{
				cmbSpecificVehicle.Items.Add(value3);
			}
			cmbSpecificVehicle.IsEnabled = true;
			cmbSpecificVehicle.SelectedIndex = Settings.Default.selectedSpecific;
			return;
		}
		try
		{
			if (cmbSpecificVehicle.IsEnabled)
			{
				cmbSpecificVehicle.Items.Clear();
				cmbSpecificVehicle.IsEnabled = false;
			}
		}
		catch (Exception)
		{
		}
	}

	private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (!bConnected)
		{
			driverInfo = RP121032.LoadDeviceParameters(Environment.GetEnvironmentVariable("SystemRoot") + "\\" + cmbDriverList.SelectedItem.ToString() + ".ini");
			cmbDeviceList.ItemsSource = driverInfo.RP1210Devices;
		}
	}

	private void btSniff_Click(object sender, RoutedEventArgs e)
	{
		Settings.Default.selectedDriver = cmbDriverList.SelectedIndex;
		Settings.Default.selectedDevice = cmbDeviceList.SelectedIndex;
		Settings.Default.selectedTruck = cmbTruckList.SelectedIndex;
		Settings.Default.selectedSpecific = cmbSpecificVehicle.SelectedIndex;
		Settings.Default.Save();
		try
		{
			btSniff.Content = "Stop";
			if (!SniffStarted)
			{
				if (Vehicle == null)
				{
					Vehicle = new Flasher(new RP121032(cmbDriverList.SelectedItem.ToString()), new RP121032(cmbDriverList.SelectedItem.ToString()), new RP121032(cmbDriverList.SelectedItem.ToString()));
				}
				DeviceInfo selectedDevice = (DeviceInfo)cmbDeviceList.SelectedValue;
				try
				{
					txtStatus.Clear();
					SniffStarted = true;
					SniffWorker = new BackgroundWorker();
					SniffWorker.WorkerReportsProgress = true;
					SniffWorker.WorkerSupportsCancellation = true;
					SniffWorker.DoWork += Vehicle.Sniffer;
					SniffWorker.ProgressChanged += SniffProgressChanged;
					SniffWorker.RunWorkerCompleted += SniffWorkerCompleted;
					SnifferArgs argument = new SnifferArgs(selectedDevice, CheckBoxJ1708.IsChecked.Value, CheckBoxJ1939.IsChecked.Value);
					SniffWorker.RunWorkerAsync(argument);
					return;
				}
				catch (Exception ex)
				{
					txtStatus.Text = ex.Message;
					return;
				}
			}
			SniffStarted = false;
			SniffWorker.CancelAsync();
			SniffWorker.Dispose();
		}
		catch (Exception ex2)
		{
			txtStatus.Text = ex2.Message;
		}
	}

	private void btnQuery_Click(object sender, RoutedEventArgs e)
	{
		Settings.Default.selectedDriver = cmbDriverList.SelectedIndex;
		Settings.Default.selectedDevice = cmbDeviceList.SelectedIndex;
		Settings.Default.selectedTruck = cmbTruckList.SelectedIndex;
		Settings.Default.selectedSpecific = cmbSpecificVehicle.SelectedIndex;
		Settings.Default.Save();
		txtStatus.Text = "Connecting....";
		if (bConnected)
		{
			if (Vehicle.J1708inst != null)
			{
				Vehicle.J1708inst = null;
			}
			if (Vehicle.J1939inst != null)
			{
				Vehicle.J1939inst = null;
			}
			if (Vehicle.ISO15765inst != null)
			{
				Vehicle.ISO15765inst = null;
			}
			btnQuery.Content = "Connect";
			bConnected = false;
			return;
		}
		Vehicle = new Flasher(new RP121032(cmbDriverList.SelectedItem.ToString()), new RP121032(cmbDriverList.SelectedItem.ToString()), new RP121032(cmbDriverList.SelectedItem.ToString()));
		DeviceInfo deviceInfo = (DeviceInfo)cmbDeviceList.SelectedValue;
		if (radio250k.IsChecked == true)
		{
			deviceInfo.SelectedRate = BaudRate.Can250;
		}
		else
		{
			deviceInfo.SelectedRate = BaudRate.Can500;
		}
		try
		{
			switch (cmbTruckList.SelectedItem.ToString())
			{
			case "Paccar":
			{
				BackgroundWorker backgroundWorker5 = new BackgroundWorker();
				backgroundWorker5.WorkerReportsProgress = true;
				backgroundWorker5.DoWork += Vehicle.PaccarQuery;
				backgroundWorker5.ProgressChanged += ProgressChanged;
				backgroundWorker5.RunWorkerCompleted += QueryCompleted;
				backgroundWorker5.RunWorkerAsync(deviceInfo);
				break;
			}
			case "DetroitACM":
			{
				BackgroundWorker backgroundWorker4 = new BackgroundWorker();
				backgroundWorker4.WorkerReportsProgress = true;
				backgroundWorker4.DoWork += Vehicle.DetroitACMQuery;
				backgroundWorker4.ProgressChanged += ProgressChanged;
				backgroundWorker4.RunWorkerCompleted += QueryCompleted;
				backgroundWorker4.RunWorkerAsync(deviceInfo);
				break;
			}
			case "DetroitMCM":
			{
				BackgroundWorker backgroundWorker3 = new BackgroundWorker();
				backgroundWorker3.WorkerReportsProgress = true;
				backgroundWorker3.DoWork += Vehicle.DetroitMCMQuery;
				backgroundWorker3.ProgressChanged += ProgressChanged;
				backgroundWorker3.RunWorkerCompleted += QueryCompleted;
				backgroundWorker3.RunWorkerAsync(new DetroitMCMAsyncArgs((DetroitMCMModuleType)cmbSpecificVehicle.SelectedItem, null, deviceInfo, RadioFullFlash.IsChecked.Value));
				break;
			}
			case "Maxxforce":
			{
				BackgroundWorker backgroundWorker2 = new BackgroundWorker();
				backgroundWorker2.WorkerReportsProgress = true;
				backgroundWorker2.DoWork += Vehicle.MaxxforceQuery;
				backgroundWorker2.ProgressChanged += ProgressChanged;
				backgroundWorker2.RunWorkerCompleted += QueryCompleted;
				backgroundWorker2.RunWorkerAsync(new MaxxAsyncArgs((MaxxModuleType)cmbSpecificVehicle.SelectedItem, null, deviceInfo, fullFlash: false));
				break;
			}
			case "CaterpillarAG":
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.WorkerReportsProgress = true;
				backgroundWorker.DoWork += Vehicle.CatAGQuery;
				backgroundWorker.ProgressChanged += ProgressChanged;
				backgroundWorker.RunWorkerCompleted += QueryCompleted;
				backgroundWorker.RunWorkerAsync(deviceInfo);
				break;
			}
			default:
				throw new Exception("Incompatible Truck Selected");
			}
		}
		catch (Exception ex)
		{
			txtStatus.Text = ex.Message;
		}
		if (txtBoxFileName.Text != "")
		{
			btnFlashFile.IsEnabled = true;
		}
		if (txtBoxRestore.Text != "")
		{
			btnWriteRestore.IsEnabled = true;
		}
		btnReadRestore.IsEnabled = true;
		btSniff.IsEnabled = false;
	}

	private void btnFlashFile_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			switch (cmbTruckList.SelectedItem.ToString())
			{
			case "Paccar":
			{
				BackgroundWorker backgroundWorker4 = new BackgroundWorker();
				backgroundWorker4.WorkerReportsProgress = true;
				backgroundWorker4.DoWork += Vehicle.PaccarFlash;
				backgroundWorker4.ProgressChanged += ProgressChanged;
				backgroundWorker4.RunWorkerCompleted += WorkerCompleted;
				backgroundWorker4.RunWorkerAsync(new Flasher.AddressRange(Convert.ToUInt32(txtBoxStartAddr.Text, 16), Convert.ToUInt32(txtBoxLength.Text, 16), txtBoxFileName.Text));
				break;
			}
			case "DetroitACM":
			{
				BackgroundWorker backgroundWorker3 = new BackgroundWorker();
				backgroundWorker3.WorkerReportsProgress = true;
				backgroundWorker3.DoWork += Vehicle.DetroitACMFlash;
				backgroundWorker3.ProgressChanged += ProgressChanged;
				backgroundWorker3.RunWorkerCompleted += WorkerCompleted;
				backgroundWorker3.RunWorkerAsync(new DetroitMCMAsyncArgs((DetroitMCMModuleType)cmbSpecificVehicle.SelectedItem, txtBoxFileName.Text, null, RadioFullFlash.IsChecked.Value));
				break;
			}
			case "DetroitMCM":
			{
				BackgroundWorker backgroundWorker2 = new BackgroundWorker();
				backgroundWorker2.WorkerReportsProgress = true;
				backgroundWorker2.DoWork += Vehicle.DetroitMCMFlash;
				backgroundWorker2.ProgressChanged += ProgressChanged;
				backgroundWorker2.RunWorkerCompleted += WorkerCompleted;
				backgroundWorker2.RunWorkerAsync(new DetroitMCMAsyncArgs((DetroitMCMModuleType)cmbSpecificVehicle.SelectedItem, txtBoxFileName.Text, null, RadioFullFlash.IsChecked.Value));
				break;
			}
			case "Maxxforce":
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.WorkerReportsProgress = true;
				backgroundWorker.DoWork += Vehicle.MaxxforceFlash;
				backgroundWorker.ProgressChanged += ProgressChanged;
				backgroundWorker.RunWorkerCompleted += WorkerCompleted;
				backgroundWorker.RunWorkerAsync(new MaxxAsyncArgs((MaxxModuleType)cmbSpecificVehicle.SelectedItem, txtBoxFileName.Text, null, RadioFullFlash.IsChecked.Value));
				break;
			}
			default:
				throw new Exception("Incompatible Truck Selected");
			}
		}
		catch (Exception ex)
		{
			txtStatus.Text = ex.Message;
		}
	}

	private void btnLoadParamFile_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Restore Files|*.txt";
		if (openFileDialog.ShowDialog() == true)
		{
			txtBoxRestore.Text = openFileDialog.FileName;
			Settings.Default.parameterFileName = openFileDialog.FileName;
			Settings.Default.Save();
			if (bConnected)
			{
				btnWriteRestore.IsEnabled = true;
			}
		}
	}

	private void btnReadRestore_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			string text = cmbTruckList.SelectedItem.ToString();
			if (text == "Maxxforce")
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.WorkerReportsProgress = true;
				backgroundWorker.DoWork += Vehicle.MaxxforceReadRestore;
				backgroundWorker.ProgressChanged += ProgressChanged;
				backgroundWorker.RunWorkerCompleted += WorkerCompleted;
				backgroundWorker.RunWorkerAsync(new MaxxAsyncArgs((MaxxModuleType)cmbSpecificVehicle.SelectedItem, txtBoxRestore.Text, null, fullFlash: false));
				return;
			}
			throw new Exception("Vehicle Not Implemented");
		}
		catch (Exception ex)
		{
			txtStatus.Text = ex.Message;
		}
	}

	private void btnWriteRestore_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			string text = cmbTruckList.SelectedItem.ToString();
			if (text == "Maxxforce")
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.WorkerReportsProgress = true;
				backgroundWorker.DoWork += Vehicle.MaxxforceWriteRestore;
				backgroundWorker.ProgressChanged += ProgressChanged;
				backgroundWorker.RunWorkerCompleted += WorkerCompleted;
				backgroundWorker.RunWorkerAsync(new MaxxAsyncArgs((MaxxModuleType)cmbSpecificVehicle.SelectedItem, txtBoxRestore.Text, null, fullFlash: false));
				return;
			}
			throw new Exception("Vehicle Not Implemented");
		}
		catch (Exception ex)
		{
			txtStatus.Text = ex.Message;
		}
	}

	private void btSave_Click(object sender, RoutedEventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "Sniff Files|*.txt";
		if (saveFileDialog.ShowDialog() == true)
		{
			File.WriteAllText(saveFileDialog.FileName, txtStatus.Text);
		}
	}

	private void BtnReadFile_Click(object sender, RoutedEventArgs e)
	{
		btnReadFile.IsEnabled = false;
		try
		{
			string text = cmbTruckList.SelectedItem.ToString();
			if (text == "Paccar")
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.WorkerReportsProgress = true;
				backgroundWorker.DoWork += Vehicle.PaccarRead;
				backgroundWorker.ProgressChanged += ProgressChanged;
				backgroundWorker.RunWorkerCompleted += WorkerCompleted;
				backgroundWorker.RunWorkerAsync(new Flasher.AddressRange(Convert.ToUInt32(txtBoxStartAddr.Text, 16), Convert.ToUInt32(txtBoxLength.Text, 16), txtBoxFileName.Text));
				return;
			}
			throw new Exception("No Read Available");
		}
		catch (Exception ex)
		{
			txtStatus.Text = ex.Message;
		}
	}

	private void BtDetDel_Click(object sender, RoutedEventArgs e)
	{
		btnReadFile.IsEnabled = false;
		try
		{
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.WorkerReportsProgress = true;
			backgroundWorker.DoWork += Vehicle.DetroitDelete;
			backgroundWorker.ProgressChanged += ProgressChanged;
			backgroundWorker.RunWorkerCompleted += WorkerCompleted;
			backgroundWorker.RunWorkerAsync();
		}
		catch (Exception ex)
		{
			txtStatus.Text = ex.Message;
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
		for (int i = 0; i < size; i++)
		{
			char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
			stringBuilder.Append(value);
		}
		if (lowerCase)
		{
			return stringBuilder.ToString().ToLower();
		}
		return stringBuilder.ToString();
	}

	private void Window_Initialized(object sender, EventArgs e)
	{
		string text = RandomString(10, lowerCase: false);
		string text2 = Interaction.InputBox(text, "Password", text);
		MD5 mD = MD5.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(text);
		string text3 = BitConverter.ToString(mD.ComputeHash(bytes)).Replace("-", string.Empty);
		if (text2 != text3)
		{
			Close();
		}
	}
}
