using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;

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

	public DiagnosisVariant DiagnosisVariant => variant;

	public VariantSelect(Ecu targetEcu, DiagnosisVariant targetVariant)
	{
		this.targetEcu = targetEcu;
		bestMatchingVariant = targetVariant;
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.VariantSelect));
		this.listView = new ListViewEx();
		this.columnName = new System.Windows.Forms.ColumnHeader();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonOk = new System.Windows.Forms.Button();
		this.label = new System.Windows.Forms.Label();
		this.listViewExEcus = new ListViewEx();
		this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
		this.tableLayoutPanel = new TableLayoutPanel();
		((System.ComponentModel.ISupportInitialize)this.listView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.listViewExEcus).BeginInit();
		((System.Windows.Forms.Control)(object)this.tableLayoutPanel).SuspendLayout();
		base.SuspendLayout();
		this.listView.CanDelete = false;
		((System.Windows.Forms.ListView)(object)this.listView).Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnName });
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel).SetColumnSpan((System.Windows.Forms.Control)(object)this.listView, 3);
		resources.ApplyResources(this.listView, "listView");
		this.listView.EditableColumn = -1;
		this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
		((System.Windows.Forms.Control)(object)this.listView).Name = "listView";
		this.listView.ShowGlyphs = (GlyphBehavior)1;
		((System.Windows.Forms.ListView)(object)this.listView).UseCompatibleStateImageBehavior = false;
		((System.Windows.Forms.Control)(object)this.listView).DoubleClick += new System.EventHandler(listView_DoubleClick);
		((System.Windows.Forms.ListView)(object)this.listView).SelectedIndexChanged += new System.EventHandler(listView_SelectedIndexChanged);
		resources.ApplyResources(this.columnName, "columnName");
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		resources.ApplyResources(this.buttonOk, "buttonOk");
		this.buttonOk.Name = "buttonOk";
		this.buttonOk.Click += new System.EventHandler(buttonOk_Click);
		this.buttonOk.Enabled = false;
		resources.ApplyResources(this.label, "label");
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel).SetColumnSpan((System.Windows.Forms.Control)this.label, 4);
		this.label.Name = "label";
		this.listViewExEcus.CanDelete = false;
		((System.Windows.Forms.ListView)(object)this.listViewExEcus).Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeader1 });
		resources.ApplyResources(this.listViewExEcus, "listViewExEcus");
		this.listViewExEcus.EditableColumn = -1;
		this.listViewExEcus.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
		((System.Windows.Forms.Control)(object)this.listViewExEcus).Name = "listViewExEcus";
		this.listViewExEcus.ShowGlyphs = (GlyphBehavior)1;
		((System.Windows.Forms.ListView)(object)this.listViewExEcus).UseCompatibleStateImageBehavior = false;
		((System.Windows.Forms.ListView)(object)this.listViewExEcus).SelectedIndexChanged += new System.EventHandler(listViewExEcus_SelectedIndexChanged);
		resources.ApplyResources(this.columnHeader1, "columnHeader1");
		resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel).Controls.Add((System.Windows.Forms.Control)(object)this.listViewExEcus, 0, 1);
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel).Controls.Add(this.label, 0, 0);
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel).Controls.Add((System.Windows.Forms.Control)(object)this.listView, 1, 1);
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel).Controls.Add(this.buttonCancel, 3, 2);
		((System.Windows.Forms.TableLayoutPanel)(object)this.tableLayoutPanel).Controls.Add(this.buttonOk, 2, 2);
		((System.Windows.Forms.Control)(object)this.tableLayoutPanel).Name = "tableLayoutPanel";
		base.AcceptButton = this.buttonOk;
		resources.ApplyResources(this, "$this");
		base.CancelButton = this.buttonCancel;
		base.Controls.Add((System.Windows.Forms.Control)(object)this.tableLayoutPanel);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "VariantSelect";
		base.ShowIcon = false;
		((System.ComponentModel.ISupportInitialize)this.listView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.listViewExEcus).EndInit();
		((System.Windows.Forms.Control)(object)this.tableLayoutPanel).ResumeLayout(false);
		((System.Windows.Forms.Control)(object)this.tableLayoutPanel).PerformLayout();
		base.ResumeLayout(false);
	}

	protected override void OnLoad(EventArgs e)
	{
		if (targetEcu != null)
		{
			label.Text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormatSelectVariantForDevice, targetEcu.Name);
		}
		BuildEcuList();
		BuildList();
		base.OnLoad(e);
	}

	private void BuildEcuList()
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		((ListView)(object)listViewExEcus).Items.Clear();
		foreach (Ecu item in from e in Sapi.GetSapi().Ecus
			where !e.IsRollCall && !e.IsVirtual && !e.OfflineSupportOnly
			orderby e.Priority
			select e)
		{
			if (targetEcu == null || targetEcu == item)
			{
				ListViewItem listViewItem = ((ListView)(object)listViewExEcus).Items.Add((ListViewItem)new ListViewExGroupItem(item.DisplayName + (item.IsMcd ? " [MVCI]" : string.Empty)));
				listViewItem.Tag = item;
				if (item == targetEcu)
				{
					listViewItem.Selected = true;
					listViewItem.Focused = true;
				}
			}
		}
	}

	private void BuildList()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		((ListView)(object)listView).Items.Clear();
		if (ecu == null)
		{
			return;
		}
		foreach (DiagnosisVariant diagnosisVariant in ecu.DiagnosisVariants)
		{
			ListViewItem listViewItem = ((ListView)(object)listView).Items.Add((ListViewItem)new ListViewExGroupItem(diagnosisVariant.Name));
			listViewItem.Tag = diagnosisVariant;
			if (diagnosisVariant == bestMatchingVariant)
			{
				listViewItem.Selected = true;
				listViewItem.Focused = true;
			}
		}
	}

	private void listView_DoubleClick(object sender, EventArgs e)
	{
		if (((ListView)(object)listView).SelectedItems.Count > 0)
		{
			variant = ((ListView)(object)listView).SelectedItems[0].Tag as DiagnosisVariant;
			Close();
		}
	}

	private void buttonOk_Click(object sender, EventArgs e)
	{
		if (((ListView)(object)listView).SelectedItems.Count > 0)
		{
			variant = ((ListView)(object)listView).SelectedItems[0].Tag as DiagnosisVariant;
			Close();
		}
	}

	private void listView_SelectedIndexChanged(object sender, EventArgs e)
	{
		buttonOk.Enabled = ((ListView)(object)listView).SelectedItems.Count != 0;
	}

	private void listViewExEcus_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (((ListView)(object)listViewExEcus).SelectedItems.Count > 0)
		{
			ecu = ((ListView)(object)listViewExEcus).SelectedItems[0].Tag as Ecu;
			buttonOk.Enabled = false;
			BuildList();
		}
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		columnName.Width = ((Control)(object)listView).Width - SystemInformation.VerticalScrollBarWidth * 2;
		columnHeader1.Width = ((Control)(object)listViewExEcus).Width - SystemInformation.VerticalScrollBarWidth * 2;
	}
}
