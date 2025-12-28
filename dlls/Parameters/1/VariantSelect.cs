// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.VariantSelect
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public class VariantSelect : Form
{
  private ListViewEx listView;
  private ColumnHeader columnName;
  private Button buttonCancel;
  private Button buttonOk;
  private System.Windows.Forms.Label label;
  private DiagnosisVariant variant;
  private DiagnosisVariant bestMatchingVariant;
  private ListViewEx listViewExEcus;
  private ColumnHeader columnHeader1;
  private TableLayoutPanel tableLayoutPanel;
  private Ecu ecu;
  private Ecu targetEcu;

  public DiagnosisVariant DiagnosisVariant => this.variant;

  public VariantSelect(Ecu targetEcu, DiagnosisVariant targetVariant)
  {
    this.targetEcu = targetEcu;
    this.bestMatchingVariant = targetVariant;
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (VariantSelect));
    this.listView = new ListViewEx();
    this.columnName = new ColumnHeader();
    this.buttonCancel = new Button();
    this.buttonOk = new Button();
    this.label = new System.Windows.Forms.Label();
    this.listViewExEcus = new ListViewEx();
    this.columnHeader1 = new ColumnHeader();
    this.tableLayoutPanel = new TableLayoutPanel();
    ((ISupportInitialize) this.listView).BeginInit();
    ((ISupportInitialize) this.listViewExEcus).BeginInit();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    this.SuspendLayout();
    this.listView.CanDelete = false;
    ((ListView) this.listView).Columns.AddRange(new ColumnHeader[1]
    {
      this.columnName
    });
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.listView, 3);
    componentResourceManager.ApplyResources((object) this.listView, "listView");
    this.listView.EditableColumn = -1;
    this.listView.HeaderStyle = ColumnHeaderStyle.None;
    ((Control) this.listView).Name = "listView";
    this.listView.ShowGlyphs = (GlyphBehavior) 1;
    ((ListView) this.listView).UseCompatibleStateImageBehavior = false;
    ((Control) this.listView).DoubleClick += new EventHandler(this.listView_DoubleClick);
    ((ListView) this.listView).SelectedIndexChanged += new EventHandler(this.listView_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.columnName, "columnName");
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.buttonCancel.DialogResult = DialogResult.Cancel;
    this.buttonCancel.Name = "buttonCancel";
    componentResourceManager.ApplyResources((object) this.buttonOk, "buttonOk");
    this.buttonOk.Name = "buttonOk";
    this.buttonOk.Click += new EventHandler(this.buttonOk_Click);
    this.buttonOk.Enabled = false;
    componentResourceManager.ApplyResources((object) this.label, "label");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.label, 4);
    this.label.Name = "label";
    this.listViewExEcus.CanDelete = false;
    ((ListView) this.listViewExEcus).Columns.AddRange(new ColumnHeader[1]
    {
      this.columnHeader1
    });
    componentResourceManager.ApplyResources((object) this.listViewExEcus, "listViewExEcus");
    this.listViewExEcus.EditableColumn = -1;
    this.listViewExEcus.HeaderStyle = ColumnHeaderStyle.None;
    ((Control) this.listViewExEcus).Name = "listViewExEcus";
    this.listViewExEcus.ShowGlyphs = (GlyphBehavior) 1;
    ((ListView) this.listViewExEcus).UseCompatibleStateImageBehavior = false;
    ((ListView) this.listViewExEcus).SelectedIndexChanged += new EventHandler(this.listViewExEcus_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.columnHeader1, "columnHeader1");
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.listViewExEcus, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.label, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.listView, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonCancel, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonOk, 2, 2);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    this.AcceptButton = (IButtonControl) this.buttonOk;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.CancelButton = (IButtonControl) this.buttonCancel;
    this.Controls.Add((Control) this.tableLayoutPanel);
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (VariantSelect);
    this.ShowIcon = false;
    ((ISupportInitialize) this.listView).EndInit();
    ((ISupportInitialize) this.listViewExEcus).EndInit();
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).PerformLayout();
    this.ResumeLayout(false);
  }

  protected override void OnLoad(EventArgs e)
  {
    if (this.targetEcu != null)
      this.label.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormatSelectVariantForDevice, (object) this.targetEcu.Name);
    this.BuildEcuList();
    this.BuildList();
    base.OnLoad(e);
  }

  private void BuildEcuList()
  {
    ((ListView) this.listViewExEcus).Items.Clear();
    foreach (Ecu ecu in (IEnumerable<Ecu>) Sapi.GetSapi().Ecus.Where<Ecu>((Func<Ecu, bool>) (e => !e.IsRollCall && !e.IsVirtual && !e.OfflineSupportOnly)).OrderBy<Ecu, int>((Func<Ecu, int>) (e => e.Priority)))
    {
      if (this.targetEcu == null || this.targetEcu == ecu)
      {
        ListViewItem listViewItem = ((ListView) this.listViewExEcus).Items.Add((ListViewItem) new ListViewExGroupItem(ecu.DisplayName + (ecu.IsMcd ? " [MVCI]" : string.Empty)));
        listViewItem.Tag = (object) ecu;
        if (ecu == this.targetEcu)
        {
          listViewItem.Selected = true;
          listViewItem.Focused = true;
        }
      }
    }
  }

  private void BuildList()
  {
    ((ListView) this.listView).Items.Clear();
    if (this.ecu == null)
      return;
    foreach (DiagnosisVariant diagnosisVariant in (ReadOnlyCollection<DiagnosisVariant>) this.ecu.DiagnosisVariants)
    {
      ListViewItem listViewItem = ((ListView) this.listView).Items.Add((ListViewItem) new ListViewExGroupItem(diagnosisVariant.Name));
      listViewItem.Tag = (object) diagnosisVariant;
      if (diagnosisVariant == this.bestMatchingVariant)
      {
        listViewItem.Selected = true;
        listViewItem.Focused = true;
      }
    }
  }

  private void listView_DoubleClick(object sender, EventArgs e)
  {
    if (((ListView) this.listView).SelectedItems.Count <= 0)
      return;
    this.variant = ((ListView) this.listView).SelectedItems[0].Tag as DiagnosisVariant;
    this.Close();
  }

  private void buttonOk_Click(object sender, EventArgs e)
  {
    if (((ListView) this.listView).SelectedItems.Count <= 0)
      return;
    this.variant = ((ListView) this.listView).SelectedItems[0].Tag as DiagnosisVariant;
    this.Close();
  }

  private void listView_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.buttonOk.Enabled = ((ListView) this.listView).SelectedItems.Count != 0;
  }

  private void listViewExEcus_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (((ListView) this.listViewExEcus).SelectedItems.Count <= 0)
      return;
    this.ecu = ((ListView) this.listViewExEcus).SelectedItems[0].Tag as Ecu;
    this.buttonOk.Enabled = false;
    this.BuildList();
  }

  protected override void OnSizeChanged(EventArgs e)
  {
    base.OnSizeChanged(e);
    this.columnName.Width = ((Control) this.listView).Width - SystemInformation.VerticalScrollBarWidth * 2;
    this.columnHeader1.Width = ((Control) this.listViewExEcus).Width - SystemInformation.VerticalScrollBarWidth * 2;
  }
}
