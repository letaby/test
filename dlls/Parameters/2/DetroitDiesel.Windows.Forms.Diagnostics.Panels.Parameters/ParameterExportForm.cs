using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public class ParameterExportForm : Form
{
	private enum CheckAction
	{
		All,
		Modified,
		Writable,
		None
	}

	private Channel channel;

	private int exportLevel = 1;

	private bool defaultCollapsed;

	private int checkedCount;

	private string exportPath = Directories.Parameters;

	private bool exportAccumulator;

	private Container components;

	private RecursiveTreeView parametersTree;

	private Button buttonExport;

	private Button buttonCancel;

	private SaveFileDialog saveFileDialog;

	private PictureBox pictureBoxTreeMod;

	private ToolStripButton toolStripButtonExpandAll;

	private ToolStripButton toolStripButtonCollapseAll;

	private ToolStrip toolStripExpandCollapse;

	private Panel panelOptions;

	private Button selectWritableButton;

	private Button selectModifiedButton;

	private Button selectAllButton;

	private ComboBox exportLevelComboBox;

	private System.Windows.Forms.Label exportLevelLabel;

	private Button selectNoneButton;

	private System.Windows.Forms.Label selectLabel;

	private TableLayoutPanel tableLayoutPanelLowerButtons;

	private int CheckedCount
	{
		get
		{
			return checkedCount;
		}
		set
		{
			checkedCount = value;
			if (buttonExport.Enabled != (checkedCount != 0))
			{
				buttonExport.Enabled = checkedCount != 0;
			}
		}
	}

	public string ExportPath
	{
		get
		{
			return exportPath;
		}
		set
		{
			exportPath = value;
		}
	}

	public ParameterExportForm(Channel channel, ParameterType type)
	{
		this.channel = channel;
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		InitializeLevelSelection();
		((TreeView)(object)parametersTree).TreeViewNodeSorter = null;
		exportAccumulator = type == ParameterType.Accumulator;
		if (exportAccumulator)
		{
			saveFileDialog.Filter = Resources.J2286AccumulatorFilesFilter;
			exportLevelComboBox.Enabled = false;
		}
		else
		{
			saveFileDialog.Filter = Resources.J2286SaveFilesFilter;
		}
	}

	private void InitializeLevelSelection()
	{
		exportLevel = ApplicationInformation.ProductAccessLevel;
		if (ApplicationInformation.CanSelectParameterExportAccessLevel)
		{
			selectWritableButton.Visible = true;
			exportLevelLabel.Visible = true;
			exportLevelComboBox.Visible = true;
			exportLevelComboBox.SelectedIndex = 2;
		}
		else
		{
			selectWritableButton.Visible = false;
			exportLevelLabel.Visible = false;
			exportLevelComboBox.Visible = false;
		}
	}

	private void BuildTreeList()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		UpdateManager val = new UpdateManager((Control)(object)parametersTree, pictureBoxTreeMod);
		try
		{
			((TreeView)(object)parametersTree).BeginUpdate();
			((TreeView)(object)parametersTree).Nodes.Clear();
			CheckedCount = 0;
			if (channel != null)
			{
				GroupCollection val2 = new GroupCollection();
				foreach (Parameter parameter2 in channel.Parameters)
				{
					bool flag = (exportAccumulator && !parameter2.Persistable) || (!exportAccumulator && parameter2.Persistable);
					if (parameter2.Visible && parameter2.ReadAccess <= exportLevel && flag)
					{
						if (parameter2.GroupName.Length == 0)
						{
							AddParameterTreeListItem(parameter2, ((TreeView)(object)parametersTree).Nodes);
						}
						else
						{
							val2.Add(parameter2.GroupName, (object)parameter2);
						}
					}
				}
				foreach (Group item in val2)
				{
					TreeNode treeNode = new TreeNode(item.Name);
					((TreeView)(object)parametersTree).Nodes.Add(treeNode);
					foreach (Parameter item2 in item.Items)
					{
						AddParameterTreeListItem(item2, treeNode.Nodes);
					}
				}
			}
			if (!defaultCollapsed)
			{
				((TreeView)(object)parametersTree).ExpandAll();
			}
			if (((TreeView)(object)parametersTree).Nodes.Count > 0)
			{
				selectAllButton.Enabled = true;
				selectModifiedButton.Enabled = true;
				selectAllButton.Enabled = true;
				selectWritableButton.Enabled = true;
				toolStripButtonCollapseAll.Enabled = true;
				toolStripButtonExpandAll.Enabled = true;
			}
			else
			{
				selectAllButton.Enabled = false;
				selectModifiedButton.Enabled = false;
				selectAllButton.Enabled = false;
				selectWritableButton.Enabled = false;
				toolStripButtonCollapseAll.Enabled = false;
				toolStripButtonExpandAll.Enabled = false;
			}
			CheckLeafNodes(((TreeView)(object)parametersTree).Nodes, CheckAction.Modified);
			((TreeView)(object)parametersTree).EndUpdate();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void AddParameterTreeListItem(Parameter parameter, TreeNodeCollection parentCollection)
	{
		TreeNode treeNode = new TreeNode(parameter.Name);
		treeNode.Tag = parameter;
		parentCollection.Add(treeNode);
	}

	private void CheckLeafNodes(TreeNodeCollection nodes, CheckAction action)
	{
		foreach (TreeNode node in nodes)
		{
			if (node.Nodes.Count == 0)
			{
				bool flag = node.Checked;
				switch (action)
				{
				case CheckAction.All:
					flag = true;
					break;
				case CheckAction.Modified:
				{
					Parameter parameter2 = (Parameter)node.Tag;
					object obj = parameter2.OriginalValue;
					if (obj == null)
					{
						obj = parameter2.LastPersistedValue;
					}
					flag = !object.Equals(parameter2.Value, obj);
					break;
				}
				case CheckAction.Writable:
				{
					Parameter parameter = (Parameter)node.Tag;
					flag = parameter.WriteAccess <= exportLevel;
					break;
				}
				case CheckAction.None:
					flag = false;
					break;
				default:
					throw new InvalidEnumArgumentException("action", (int)action, typeof(CheckAction));
				}
				RecursiveTreeView.GuardedCheck(node, flag);
			}
			else
			{
				CheckLeafNodes(node.Nodes, action);
			}
		}
	}

	private void MarkLeafNodes(TreeNodeCollection nodes)
	{
		foreach (TreeNode node in nodes)
		{
			if (node.Nodes.Count == 0)
			{
				Parameter parameter = (Parameter)node.Tag;
				parameter.Marked = node.Checked;
			}
			else
			{
				MarkLeafNodes(node.Nodes);
			}
		}
	}

	private void ExportParameters(Channel channel)
	{
		if (channel == null)
		{
			throw new ArgumentNullException("channel", "Cannot export a null channel");
		}
		saveFileDialog.InitialDirectory = exportPath;
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			string text = saveFileDialog.FileName;
			string[] array = saveFileDialog.Filter.Split('|');
			string text2 = array[(saveFileDialog.FilterIndex - 1) * 2 + 1].Substring(1);
			if (!string.Equals(Path.GetExtension(text), text2, StringComparison.OrdinalIgnoreCase))
			{
				text += text2;
			}
			if (exportAccumulator)
			{
				channel.Parameters.SaveAccumulator(text, ParameterView.GetFileFormat(text));
			}
			else
			{
				channel.Parameters.Save(text, ParameterView.GetFileFormat(text));
			}
			exportPath = text;
		}
	}

	private void OnClickSelectAll(object sender, EventArgs e)
	{
		CheckLeafNodes(((TreeView)(object)parametersTree).Nodes, CheckAction.All);
	}

	private void OnClickSelectModified(object sender, EventArgs e)
	{
		CheckLeafNodes(((TreeView)(object)parametersTree).Nodes, CheckAction.Modified);
	}

	private void OnClickSelectNone(object sender, EventArgs e)
	{
		CheckLeafNodes(((TreeView)(object)parametersTree).Nodes, CheckAction.None);
	}

	private void OnClickSelectWritable(object sender, EventArgs e)
	{
		CheckLeafNodes(((TreeView)(object)parametersTree).Nodes, CheckAction.Writable);
	}

	private void OnClickExport(object sender, EventArgs e)
	{
		if (channel != null)
		{
			foreach (Parameter parameter in channel.Parameters)
			{
				parameter.Marked = false;
			}
			MarkLeafNodes(((TreeView)(object)parametersTree).Nodes);
			ExportParameters(channel);
			foreach (Parameter parameter2 in channel.Parameters)
			{
				parameter2.Marked = true;
			}
		}
		Close();
	}

	protected override void OnLoad(EventArgs e)
	{
		BuildTreeList();
		base.OnLoad(e);
	}

	private void OnAfterCheck(object sender, TreeViewEventArgs e)
	{
		if (e.Node.Nodes.Count == 0)
		{
			if (e.Node.Checked)
			{
				CheckedCount++;
			}
			else
			{
				CheckedCount--;
			}
		}
	}

	private void OnLevelSelectionChanged(object sender, EventArgs e)
	{
		int num = exportLevelComboBox.SelectedIndex + 1;
		if (exportLevel != num)
		{
			exportLevel = num;
			BuildTreeList();
		}
	}

	private void OnClickExpandAll(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		UpdateManager val = new UpdateManager((Control)(object)parametersTree, pictureBoxTreeMod);
		try
		{
			defaultCollapsed = false;
			((TreeView)(object)parametersTree).ExpandAll();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OnClickCollapseAll(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		UpdateManager val = new UpdateManager((Control)(object)parametersTree, pictureBoxTreeMod);
		try
		{
			defaultCollapsed = true;
			((TreeView)(object)parametersTree).CollapseAll();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OnHelpButtonClicked(object sender, CancelEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		e.Cancel = true;
		Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_ParameterExportForm"));
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
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.ParameterExportForm));
		this.panelOptions = new System.Windows.Forms.Panel();
		this.selectNoneButton = new System.Windows.Forms.Button();
		this.selectWritableButton = new System.Windows.Forms.Button();
		this.selectModifiedButton = new System.Windows.Forms.Button();
		this.selectAllButton = new System.Windows.Forms.Button();
		this.exportLevelComboBox = new System.Windows.Forms.ComboBox();
		this.exportLevelLabel = new System.Windows.Forms.Label();
		this.parametersTree = new RecursiveTreeView();
		this.pictureBoxTreeMod = new System.Windows.Forms.PictureBox();
		this.toolStripExpandCollapse = new System.Windows.Forms.ToolStrip();
		this.toolStripButtonExpandAll = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonCollapseAll = new System.Windows.Forms.ToolStripButton();
		this.selectLabel = new System.Windows.Forms.Label();
		this.buttonExport = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.tableLayoutPanelLowerButtons = new System.Windows.Forms.TableLayoutPanel();
		System.Windows.Forms.TableLayoutPanel tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
		tableLayoutPanel.SuspendLayout();
		this.panelOptions.SuspendLayout();
		panel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxTreeMod).BeginInit();
		this.toolStripExpandCollapse.SuspendLayout();
		this.tableLayoutPanelLowerButtons.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		tableLayoutPanel.Controls.Add(this.selectLabel, 0, 0);
		tableLayoutPanel.Controls.Add(this.panelOptions, 1, 1);
		tableLayoutPanel.Controls.Add(panel, 0, 1);
		tableLayoutPanel.Controls.Add(this.tableLayoutPanelLowerButtons, 0, 2);
		tableLayoutPanel.Name = "tableLayoutPanel";
		this.panelOptions.Controls.Add(this.selectNoneButton);
		this.panelOptions.Controls.Add(this.selectWritableButton);
		this.panelOptions.Controls.Add(this.selectModifiedButton);
		this.panelOptions.Controls.Add(this.selectAllButton);
		this.panelOptions.Controls.Add(this.exportLevelComboBox);
		this.panelOptions.Controls.Add(this.exportLevelLabel);
		resources.ApplyResources(this.panelOptions, "panelOptions");
		this.panelOptions.Name = "panelOptions";
		resources.ApplyResources(this.selectNoneButton, "selectNoneButton");
		this.selectNoneButton.Name = "selectNoneButton";
		this.selectNoneButton.UseVisualStyleBackColor = true;
		this.selectNoneButton.Click += new System.EventHandler(OnClickSelectNone);
		resources.ApplyResources(this.selectWritableButton, "selectWritableButton");
		this.selectWritableButton.Name = "selectWritableButton";
		this.selectWritableButton.UseVisualStyleBackColor = true;
		this.selectWritableButton.Click += new System.EventHandler(OnClickSelectWritable);
		resources.ApplyResources(this.selectModifiedButton, "selectModifiedButton");
		this.selectModifiedButton.Name = "selectModifiedButton";
		this.selectModifiedButton.UseVisualStyleBackColor = true;
		this.selectModifiedButton.Click += new System.EventHandler(OnClickSelectModified);
		resources.ApplyResources(this.selectAllButton, "selectAllButton");
		this.selectAllButton.Name = "selectAllButton";
		this.selectAllButton.UseVisualStyleBackColor = true;
		this.selectAllButton.Click += new System.EventHandler(OnClickSelectAll);
		resources.ApplyResources(this.exportLevelComboBox, "exportLevelComboBox");
		this.exportLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.exportLevelComboBox.FormattingEnabled = true;
		this.exportLevelComboBox.Items.AddRange(new object[3]
		{
			resources.GetString("exportLevelComboBox.Items"),
			resources.GetString("exportLevelComboBox.Items1"),
			resources.GetString("exportLevelComboBox.Items2")
		});
		this.exportLevelComboBox.Name = "exportLevelComboBox";
		this.exportLevelComboBox.SelectedIndexChanged += new System.EventHandler(OnLevelSelectionChanged);
		resources.ApplyResources(this.exportLevelLabel, "exportLevelLabel");
		this.exportLevelLabel.Name = "exportLevelLabel";
		panel.Controls.Add((System.Windows.Forms.Control)(object)this.parametersTree);
		panel.Controls.Add(this.pictureBoxTreeMod);
		panel.Controls.Add(this.toolStripExpandCollapse);
		resources.ApplyResources(panel, "panelTree");
		panel.Name = "panelTree";
		((System.Windows.Forms.TreeView)(object)this.parametersTree).CheckBoxes = true;
		resources.ApplyResources(this.parametersTree, "parametersTree");
		((System.Windows.Forms.TreeView)(object)this.parametersTree).FullRowSelect = true;
		((System.Windows.Forms.TreeView)(object)this.parametersTree).HideSelection = false;
		((System.Windows.Forms.Control)(object)this.parametersTree).Name = "parametersTree";
		((System.Windows.Forms.TreeView)(object)this.parametersTree).ShowLines = false;
		((System.Windows.Forms.TreeView)(object)this.parametersTree).AfterCheck += new System.Windows.Forms.TreeViewEventHandler(OnAfterCheck);
		resources.ApplyResources(this.pictureBoxTreeMod, "pictureBoxTreeMod");
		this.pictureBoxTreeMod.Name = "pictureBoxTreeMod";
		this.pictureBoxTreeMod.TabStop = false;
		this.toolStripExpandCollapse.AllowMerge = false;
		resources.ApplyResources(this.toolStripExpandCollapse, "toolStripExpandCollapse");
		this.toolStripExpandCollapse.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		this.toolStripExpandCollapse.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.toolStripExpandCollapse.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripButtonExpandAll, this.toolStripButtonCollapseAll });
		this.toolStripExpandCollapse.Name = "toolStripExpandCollapse";
		this.toolStripExpandCollapse.Stretch = true;
		this.toolStripExpandCollapse.TabStop = true;
		this.toolStripButtonExpandAll.Image = DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties.Resources.expandall;
		resources.ApplyResources(this.toolStripButtonExpandAll, "toolStripButtonExpandAll");
		this.toolStripButtonExpandAll.Name = "toolStripButtonExpandAll";
		this.toolStripButtonExpandAll.Click += new System.EventHandler(OnClickExpandAll);
		this.toolStripButtonCollapseAll.Image = DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties.Resources.collapseall;
		resources.ApplyResources(this.toolStripButtonCollapseAll, "toolStripButtonCollapseAll");
		this.toolStripButtonCollapseAll.Name = "toolStripButtonCollapseAll";
		this.toolStripButtonCollapseAll.Click += new System.EventHandler(OnClickCollapseAll);
		resources.ApplyResources(this.selectLabel, "selectLabel");
		tableLayoutPanel.SetColumnSpan(this.selectLabel, 2);
		this.selectLabel.Name = "selectLabel";
		resources.ApplyResources(this.buttonExport, "buttonExport");
		this.buttonExport.Name = "buttonExport";
		this.buttonExport.Click += new System.EventHandler(OnClickExport);
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.Name = "buttonCancel";
		resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
		resources.ApplyResources(this.tableLayoutPanelLowerButtons, "tableLayoutPanelLowerButtons");
		tableLayoutPanel.SetColumnSpan(this.tableLayoutPanelLowerButtons, 2);
		this.tableLayoutPanelLowerButtons.Controls.Add(this.buttonExport, 1, 0);
		this.tableLayoutPanelLowerButtons.Controls.Add(this.buttonCancel, 2, 0);
		this.tableLayoutPanelLowerButtons.Name = "tableLayoutPanelLowerButtons";
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.Controls.Add(tableLayoutPanel);
		base.HelpButton = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ParameterExportForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(OnHelpButtonClicked);
		tableLayoutPanel.ResumeLayout(false);
		tableLayoutPanel.PerformLayout();
		this.panelOptions.ResumeLayout(false);
		panel.ResumeLayout(false);
		panel.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxTreeMod).EndInit();
		this.toolStripExpandCollapse.ResumeLayout(false);
		this.toolStripExpandCollapse.PerformLayout();
		this.tableLayoutPanelLowerButtons.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
