// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.PrintFaultsDialog
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class PrintFaultsDialog : Form, IProvideHtml
{
  private List<string> catagoires = new List<string>()
  {
    "Engine",
    "Transmission",
    "Vehicle",
    "Other"
  };
  private WebBrowserList webBrowserList = new WebBrowserList();
  private IContainer components;
  private TableLayoutPanel tableLayoutPanelMain;
  private CheckBox checkBoxJ1939;
  private CheckBox checkBoxVehicle;
  private CheckBox checkBoxPowertrain;
  private Button buttonPrint;
  private Button buttonCancel;

  public PrintFaultsDialog()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.webBrowserList.SetWriterFunction(new Action<XmlWriter>(this.WriteFaultInformation));
  }

  private void WriteFaultInformation(XmlWriter writer)
  {
    bool flag = false;
    foreach (Channel channel in (IEnumerable<Channel>) ((IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels).OrderBy<Channel, int>((Func<Channel, int>) (ac => ac.Ecu.Priority)))
    {
      IEnumerable<FaultCodeIncident> faultCodeIncidents = FaultList.GetOrderedFaultCodeIncidents(channel);
      if (this.catagoires.Contains(SapiExtensions.EquipmentCategory(channel.Ecu)) && faultCodeIncidents.Count<FaultCodeIncident>() > 0)
      {
        writer.WriteStartElement("P");
        writer.WriteStartElement("div");
        writer.WriteAttributeString("class", "heading2");
        writer.WriteString(channel.Ecu.Name);
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
        foreach (FaultCodeIncident faultCodeIncident in faultCodeIncidents)
        {
          flag = true;
          PrintFaultsDialog.WriteRow(writer, faultCodeIncident.FaultCode.Name, SapiExtensions.ToStatusString(faultCodeIncident));
        }
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }
    if (flag)
      return;
    writer.WriteStartElement("div");
    writer.WriteAttributeString("class", "heading2");
    writer.WriteString(Resources.PrintFaultsDialog_None);
    writer.WriteEndElement();
  }

  private static void WriteRow(XmlWriter writer, params string[] columns)
  {
    writer.WriteStartElement("tr");
    foreach (string column in columns)
    {
      writer.WriteStartElement("td");
      writer.WriteString(column);
      writer.WriteEndElement();
    }
    writer.WriteEndElement();
  }

  protected override void OnLoad(EventArgs e)
  {
    this.checkBoxJ1939.Checked = this.checkBoxVehicle.Checked = this.checkBoxPowertrain.Checked = this.buttonPrint.Enabled = true;
    base.OnLoad(e);
  }

  private void buttonPrint_Click(object sender, EventArgs e)
  {
    this.DialogResult = DialogResult.OK;
    this.Close();
  }

  private void buttonCancel_Click(object sender, EventArgs e)
  {
    this.DialogResult = DialogResult.Cancel;
    this.Close();
  }

  private void checkBox_CheckedChanged(object sender, EventArgs e)
  {
    this.catagoires = new List<string>();
    if (this.checkBoxJ1939.Checked)
      this.catagoires.Add("Other");
    if (this.checkBoxVehicle.Checked)
      this.catagoires.Add("Vehicle");
    if (this.checkBoxPowertrain.Checked)
    {
      this.catagoires.Add("Engine");
      this.catagoires.Add("Transmission");
    }
    this.buttonPrint.Enabled = this.checkBoxJ1939.Checked || this.checkBoxVehicle.Checked || this.checkBoxPowertrain.Checked;
  }

  public bool CanProvideHtml => this.webBrowserList.CanProvideHtml;

  public string ToHtml() => this.webBrowserList.ToHtml();

  public string StyleSheet => this.webBrowserList.StyleSheet;

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (PrintFaultsDialog));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.checkBoxJ1939 = new CheckBox();
    this.checkBoxVehicle = new CheckBox();
    this.checkBoxPowertrain = new CheckBox();
    this.buttonPrint = new Button();
    this.buttonCancel = new Button();
    this.tableLayoutPanelMain.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    this.tableLayoutPanelMain.Controls.Add((Control) this.checkBoxJ1939, 0, 2);
    this.tableLayoutPanelMain.Controls.Add((Control) this.checkBoxVehicle, 0, 1);
    this.tableLayoutPanelMain.Controls.Add((Control) this.checkBoxPowertrain, 0, 0);
    this.tableLayoutPanelMain.Controls.Add((Control) this.buttonPrint, 1, 4);
    this.tableLayoutPanelMain.Controls.Add((Control) this.buttonCancel, 2, 4);
    this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.checkBoxJ1939, "checkBoxJ1939");
    this.checkBoxJ1939.Checked = true;
    this.checkBoxJ1939.CheckState = CheckState.Checked;
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.checkBoxJ1939, 3);
    this.checkBoxJ1939.Name = "checkBoxJ1939";
    this.checkBoxJ1939.UseVisualStyleBackColor = true;
    this.checkBoxJ1939.CheckedChanged += new EventHandler(this.checkBox_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxVehicle, "checkBoxVehicle");
    this.checkBoxVehicle.Checked = true;
    this.checkBoxVehicle.CheckState = CheckState.Checked;
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.checkBoxVehicle, 3);
    this.checkBoxVehicle.Name = "checkBoxVehicle";
    this.checkBoxVehicle.UseVisualStyleBackColor = true;
    this.checkBoxVehicle.CheckedChanged += new EventHandler(this.checkBox_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.checkBoxPowertrain, "checkBoxPowertrain");
    this.checkBoxPowertrain.Checked = true;
    this.checkBoxPowertrain.CheckState = CheckState.Checked;
    this.tableLayoutPanelMain.SetColumnSpan((Control) this.checkBoxPowertrain, 3);
    this.checkBoxPowertrain.Name = "checkBoxPowertrain";
    this.checkBoxPowertrain.UseVisualStyleBackColor = true;
    this.checkBoxPowertrain.CheckedChanged += new EventHandler(this.checkBox_CheckedChanged);
    componentResourceManager.ApplyResources((object) this.buttonPrint, "buttonPrint");
    this.buttonPrint.Name = "buttonPrint";
    this.buttonPrint.UseVisualStyleBackColor = true;
    this.buttonPrint.Click += new EventHandler(this.buttonPrint_Click);
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.buttonCancel.Name = "buttonCancel";
    this.buttonCancel.UseVisualStyleBackColor = true;
    this.buttonCancel.Click += new EventHandler(this.buttonCancel_Click);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.tableLayoutPanelMain);
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (PrintFaultsDialog);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.tableLayoutPanelMain.ResumeLayout(false);
    this.tableLayoutPanelMain.PerformLayout();
    this.ResumeLayout(false);
  }
}
