using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class PrintFaultsDialog : Form, IProvideHtml
{
	private List<string> catagoires = new List<string> { "Engine", "Transmission", "Vehicle", "Other" };

	private WebBrowserList webBrowserList = new WebBrowserList();

	private IContainer components;

	private TableLayoutPanel tableLayoutPanelMain;

	private CheckBox checkBoxJ1939;

	private CheckBox checkBoxVehicle;

	private CheckBox checkBoxPowertrain;

	private Button buttonPrint;

	private Button buttonCancel;

	public bool CanProvideHtml => webBrowserList.CanProvideHtml;

	public string StyleSheet => webBrowserList.StyleSheet;

	public PrintFaultsDialog()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		webBrowserList.SetWriterFunction((Action<XmlWriter>)WriteFaultInformation);
	}

	private void WriteFaultInformation(XmlWriter writer)
	{
		bool flag = false;
		foreach (Channel item in ((IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels).OrderBy((Channel ac) => ac.Ecu.Priority))
		{
			IEnumerable<FaultCodeIncident> orderedFaultCodeIncidents = FaultList.GetOrderedFaultCodeIncidents(item);
			if (!catagoires.Contains(SapiExtensions.EquipmentCategory(item.Ecu)) || orderedFaultCodeIncidents.Count() <= 0)
			{
				continue;
			}
			writer.WriteStartElement("P");
			writer.WriteStartElement("div");
			writer.WriteAttributeString("class", "heading2");
			writer.WriteString(item.Ecu.Name);
			writer.WriteEndElement();
			writer.WriteStartElement("table");
			writer.WriteStartElement("tr");
			writer.WriteStartElement("th");
			writer.WriteString(Resources.PrintFaultsDialog_Fault);
			writer.WriteEndElement();
			writer.WriteStartElement("th");
			writer.WriteString(Resources.PrintFaultsDialog_Status);
			writer.WriteEndElement();
			writer.WriteEndElement();
			foreach (FaultCodeIncident item2 in orderedFaultCodeIncidents)
			{
				flag = true;
				WriteRow(writer, item2.FaultCode.Name, SapiExtensions.ToStatusString(item2));
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
		if (!flag)
		{
			writer.WriteStartElement("div");
			writer.WriteAttributeString("class", "heading2");
			writer.WriteString(Resources.PrintFaultsDialog_None);
			writer.WriteEndElement();
		}
	}

	private static void WriteRow(XmlWriter writer, params string[] columns)
	{
		writer.WriteStartElement("tr");
		foreach (string text in columns)
		{
			writer.WriteStartElement("td");
			writer.WriteString(text);
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
	}

	protected override void OnLoad(EventArgs e)
	{
		CheckBox checkBox = checkBoxJ1939;
		CheckBox checkBox2 = checkBoxVehicle;
		CheckBox checkBox3 = checkBoxPowertrain;
		bool flag = (buttonPrint.Enabled = true);
		bool flag3 = (checkBox3.Checked = flag);
		bool flag5 = (checkBox2.Checked = flag3);
		checkBox.Checked = flag5;
		base.OnLoad(e);
	}

	private void buttonPrint_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void checkBox_CheckedChanged(object sender, EventArgs e)
	{
		catagoires = new List<string>();
		if (checkBoxJ1939.Checked)
		{
			catagoires.Add("Other");
		}
		if (checkBoxVehicle.Checked)
		{
			catagoires.Add("Vehicle");
		}
		if (checkBoxPowertrain.Checked)
		{
			catagoires.Add("Engine");
			catagoires.Add("Transmission");
		}
		buttonPrint.Enabled = checkBoxJ1939.Checked || checkBoxVehicle.Checked || checkBoxPowertrain.Checked;
	}

	public string ToHtml()
	{
		return webBrowserList.ToHtml();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.PrintFaultsDialog));
		this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
		this.checkBoxJ1939 = new System.Windows.Forms.CheckBox();
		this.checkBoxVehicle = new System.Windows.Forms.CheckBox();
		this.checkBoxPowertrain = new System.Windows.Forms.CheckBox();
		this.buttonPrint = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.tableLayoutPanelMain.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.tableLayoutPanelMain, "tableLayoutPanelMain");
		this.tableLayoutPanelMain.Controls.Add(this.checkBoxJ1939, 0, 2);
		this.tableLayoutPanelMain.Controls.Add(this.checkBoxVehicle, 0, 1);
		this.tableLayoutPanelMain.Controls.Add(this.checkBoxPowertrain, 0, 0);
		this.tableLayoutPanelMain.Controls.Add(this.buttonPrint, 1, 4);
		this.tableLayoutPanelMain.Controls.Add(this.buttonCancel, 2, 4);
		this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
		resources.ApplyResources(this.checkBoxJ1939, "checkBoxJ1939");
		this.checkBoxJ1939.Checked = true;
		this.checkBoxJ1939.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tableLayoutPanelMain.SetColumnSpan(this.checkBoxJ1939, 3);
		this.checkBoxJ1939.Name = "checkBoxJ1939";
		this.checkBoxJ1939.UseVisualStyleBackColor = true;
		this.checkBoxJ1939.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
		resources.ApplyResources(this.checkBoxVehicle, "checkBoxVehicle");
		this.checkBoxVehicle.Checked = true;
		this.checkBoxVehicle.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tableLayoutPanelMain.SetColumnSpan(this.checkBoxVehicle, 3);
		this.checkBoxVehicle.Name = "checkBoxVehicle";
		this.checkBoxVehicle.UseVisualStyleBackColor = true;
		this.checkBoxVehicle.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
		resources.ApplyResources(this.checkBoxPowertrain, "checkBoxPowertrain");
		this.checkBoxPowertrain.Checked = true;
		this.checkBoxPowertrain.CheckState = System.Windows.Forms.CheckState.Checked;
		this.tableLayoutPanelMain.SetColumnSpan(this.checkBoxPowertrain, 3);
		this.checkBoxPowertrain.Name = "checkBoxPowertrain";
		this.checkBoxPowertrain.UseVisualStyleBackColor = true;
		this.checkBoxPowertrain.CheckedChanged += new System.EventHandler(checkBox_CheckedChanged);
		resources.ApplyResources(this.buttonPrint, "buttonPrint");
		this.buttonPrint.Name = "buttonPrint";
		this.buttonPrint.UseVisualStyleBackColor = true;
		this.buttonPrint.Click += new System.EventHandler(buttonPrint_Click);
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanelMain);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "PrintFaultsDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.tableLayoutPanelMain.ResumeLayout(false);
		this.tableLayoutPanelMain.PerformLayout();
		base.ResumeLayout(false);
	}
}
