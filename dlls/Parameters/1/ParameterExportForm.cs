// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.ParameterExportForm
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public class ParameterExportForm : Form
{
  private Channel channel;
  private int exportLevel = 1;
  private bool defaultCollapsed;
  private int checkedCount;
  private string exportPath = Directories.Parameters;
  private bool exportAccumulator;
  private System.ComponentModel.Container components;
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
    get => this.checkedCount;
    set
    {
      this.checkedCount = value;
      if (this.buttonExport.Enabled == (this.checkedCount != 0))
        return;
      this.buttonExport.Enabled = this.checkedCount != 0;
    }
  }

  public string ExportPath
  {
    get => this.exportPath;
    set => this.exportPath = value;
  }

  public ParameterExportForm(Channel channel, ParameterType type)
  {
    this.channel = channel;
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.InitializeLevelSelection();
    ((TreeView) this.parametersTree).TreeViewNodeSorter = (IComparer) null;
    this.exportAccumulator = type == ParameterType.Accumulator;
    if (this.exportAccumulator)
    {
      this.saveFileDialog.Filter = Resources.J2286AccumulatorFilesFilter;
      this.exportLevelComboBox.Enabled = false;
    }
    else
      this.saveFileDialog.Filter = Resources.J2286SaveFilesFilter;
  }

  private void InitializeLevelSelection()
  {
    this.exportLevel = ApplicationInformation.ProductAccessLevel;
    if (ApplicationInformation.CanSelectParameterExportAccessLevel)
    {
      this.selectWritableButton.Visible = true;
      this.exportLevelLabel.Visible = true;
      this.exportLevelComboBox.Visible = true;
      this.exportLevelComboBox.SelectedIndex = 2;
    }
    else
    {
      this.selectWritableButton.Visible = false;
      this.exportLevelLabel.Visible = false;
      this.exportLevelComboBox.Visible = false;
    }
  }

  private void BuildTreeList()
  {
    using (new UpdateManager((Control) this.parametersTree, this.pictureBoxTreeMod))
    {
      ((TreeView) this.parametersTree).BeginUpdate();
      ((TreeView) this.parametersTree).Nodes.Clear();
      this.CheckedCount = 0;
      if (this.channel != null)
      {
        GroupCollection groupCollection = new GroupCollection();
        foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) this.channel.Parameters)
        {
          bool flag = this.exportAccumulator && !parameter.Persistable || !this.exportAccumulator && parameter.Persistable;
          if (((!parameter.Visible ? 0 : (parameter.ReadAccess <= this.exportLevel ? 1 : 0)) & (flag ? 1 : 0)) != 0)
          {
            if (parameter.GroupName.Length == 0)
              ParameterExportForm.AddParameterTreeListItem(parameter, ((TreeView) this.parametersTree).Nodes);
            else
              groupCollection.Add(parameter.GroupName, (object) parameter);
          }
        }
        foreach (Group group in groupCollection)
        {
          TreeNode node = new TreeNode(group.Name);
          ((TreeView) this.parametersTree).Nodes.Add(node);
          foreach (Parameter parameter in group.Items)
            ParameterExportForm.AddParameterTreeListItem(parameter, node.Nodes);
        }
      }
      if (!this.defaultCollapsed)
        ((TreeView) this.parametersTree).ExpandAll();
      if (((TreeView) this.parametersTree).Nodes.Count > 0)
      {
        this.selectAllButton.Enabled = true;
        this.selectModifiedButton.Enabled = true;
        this.selectAllButton.Enabled = true;
        this.selectWritableButton.Enabled = true;
        this.toolStripButtonCollapseAll.Enabled = true;
        this.toolStripButtonExpandAll.Enabled = true;
      }
      else
      {
        this.selectAllButton.Enabled = false;
        this.selectModifiedButton.Enabled = false;
        this.selectAllButton.Enabled = false;
        this.selectWritableButton.Enabled = false;
        this.toolStripButtonCollapseAll.Enabled = false;
        this.toolStripButtonExpandAll.Enabled = false;
      }
      this.CheckLeafNodes(((TreeView) this.parametersTree).Nodes, ParameterExportForm.CheckAction.Modified);
      ((TreeView) this.parametersTree).EndUpdate();
    }
  }

  private static void AddParameterTreeListItem(
    Parameter parameter,
    TreeNodeCollection parentCollection)
  {
    parentCollection.Add(new TreeNode(parameter.Name)
    {
      Tag = (object) parameter
    });
  }

  private void CheckLeafNodes(TreeNodeCollection nodes, ParameterExportForm.CheckAction action)
  {
    foreach (TreeNode node in nodes)
    {
      if (node.Nodes.Count == 0)
      {
        bool flag1 = node.Checked;
        bool flag2;
        switch (action)
        {
          case ParameterExportForm.CheckAction.All:
            flag2 = true;
            break;
          case ParameterExportForm.CheckAction.Modified:
            Parameter tag = (Parameter) node.Tag;
            object objB = tag.OriginalValue ?? tag.LastPersistedValue;
            flag2 = !object.Equals(tag.Value, objB);
            break;
          case ParameterExportForm.CheckAction.Writable:
            flag2 = ((Parameter) node.Tag).WriteAccess <= this.exportLevel;
            break;
          case ParameterExportForm.CheckAction.None:
            flag2 = false;
            break;
          default:
            throw new InvalidEnumArgumentException(nameof (action), (int) action, typeof (ParameterExportForm.CheckAction));
        }
        RecursiveTreeView.GuardedCheck(node, flag2);
      }
      else
        this.CheckLeafNodes(node.Nodes, action);
    }
  }

  private void MarkLeafNodes(TreeNodeCollection nodes)
  {
    foreach (TreeNode node in nodes)
    {
      if (node.Nodes.Count == 0)
        ((Parameter) node.Tag).Marked = node.Checked;
      else
        this.MarkLeafNodes(node.Nodes);
    }
  }

  private void ExportParameters(Channel channel)
  {
    if (channel == null)
      throw new ArgumentNullException(nameof (channel), "Cannot export a null channel");
    this.saveFileDialog.InitialDirectory = this.exportPath;
    if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
      return;
    string fileName = this.saveFileDialog.FileName;
    string b = this.saveFileDialog.Filter.Split('|')[(this.saveFileDialog.FilterIndex - 1) * 2 + 1].Substring(1);
    if (!string.Equals(Path.GetExtension(fileName), b, StringComparison.OrdinalIgnoreCase))
      fileName += b;
    if (this.exportAccumulator)
      channel.Parameters.SaveAccumulator(fileName, ParameterView.GetFileFormat(fileName));
    else
      channel.Parameters.Save(fileName, ParameterView.GetFileFormat(fileName));
    this.exportPath = fileName;
  }

  private void OnClickSelectAll(object sender, EventArgs e)
  {
    this.CheckLeafNodes(((TreeView) this.parametersTree).Nodes, ParameterExportForm.CheckAction.All);
  }

  private void OnClickSelectModified(object sender, EventArgs e)
  {
    this.CheckLeafNodes(((TreeView) this.parametersTree).Nodes, ParameterExportForm.CheckAction.Modified);
  }

  private void OnClickSelectNone(object sender, EventArgs e)
  {
    this.CheckLeafNodes(((TreeView) this.parametersTree).Nodes, ParameterExportForm.CheckAction.None);
  }

  private void OnClickSelectWritable(object sender, EventArgs e)
  {
    this.CheckLeafNodes(((TreeView) this.parametersTree).Nodes, ParameterExportForm.CheckAction.Writable);
  }

  private void OnClickExport(object sender, EventArgs e)
  {
    if (this.channel != null)
    {
      foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) this.channel.Parameters)
        parameter.Marked = false;
      this.MarkLeafNodes(((TreeView) this.parametersTree).Nodes);
      this.ExportParameters(this.channel);
      foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) this.channel.Parameters)
        parameter.Marked = true;
    }
    this.Close();
  }

  protected override void OnLoad(EventArgs e)
  {
    this.BuildTreeList();
    base.OnLoad(e);
  }

  private void OnAfterCheck(object sender, TreeViewEventArgs e)
  {
    if (e.Node.Nodes.Count != 0)
      return;
    if (e.Node.Checked)
      ++this.CheckedCount;
    else
      --this.CheckedCount;
  }

  private void OnLevelSelectionChanged(object sender, EventArgs e)
  {
    int num = this.exportLevelComboBox.SelectedIndex + 1;
    if (this.exportLevel == num)
      return;
    this.exportLevel = num;
    this.BuildTreeList();
  }

  private void OnClickExpandAll(object sender, EventArgs e)
  {
    using (new UpdateManager((Control) this.parametersTree, this.pictureBoxTreeMod))
    {
      this.defaultCollapsed = false;
      ((TreeView) this.parametersTree).ExpandAll();
    }
  }

  private void OnClickCollapseAll(object sender, EventArgs e)
  {
    using (new UpdateManager((Control) this.parametersTree, this.pictureBoxTreeMod))
    {
      this.defaultCollapsed = true;
      ((TreeView) this.parametersTree).CollapseAll();
    }
  }

  private void OnHelpButtonClicked(object sender, CancelEventArgs e)
  {
    e.Cancel = true;
    Link.ShowTarget(Link.AvailableLinks.GetLinkOrEmpty("Form_ParameterExportForm"));
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ParameterExportForm));
    this.panelOptions = new Panel();
    this.selectNoneButton = new Button();
    this.selectWritableButton = new Button();
    this.selectModifiedButton = new Button();
    this.selectAllButton = new Button();
    this.exportLevelComboBox = new ComboBox();
    this.exportLevelLabel = new System.Windows.Forms.Label();
    this.parametersTree = new RecursiveTreeView();
    this.pictureBoxTreeMod = new PictureBox();
    this.toolStripExpandCollapse = new ToolStrip();
    this.toolStripButtonExpandAll = new ToolStripButton();
    this.toolStripButtonCollapseAll = new ToolStripButton();
    this.selectLabel = new System.Windows.Forms.Label();
    this.buttonExport = new Button();
    this.buttonCancel = new Button();
    this.saveFileDialog = new SaveFileDialog();
    this.tableLayoutPanelLowerButtons = new TableLayoutPanel();
    TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
    Panel panel = new Panel();
    tableLayoutPanel.SuspendLayout();
    this.panelOptions.SuspendLayout();
    panel.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxTreeMod).BeginInit();
    this.toolStripExpandCollapse.SuspendLayout();
    this.tableLayoutPanelLowerButtons.SuspendLayout();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) tableLayoutPanel, "tableLayoutPanel");
    tableLayoutPanel.Controls.Add((Control) this.selectLabel, 0, 0);
    tableLayoutPanel.Controls.Add((Control) this.panelOptions, 1, 1);
    tableLayoutPanel.Controls.Add((Control) panel, 0, 1);
    tableLayoutPanel.Controls.Add((Control) this.tableLayoutPanelLowerButtons, 0, 2);
    tableLayoutPanel.Name = "tableLayoutPanel";
    this.panelOptions.Controls.Add((Control) this.selectNoneButton);
    this.panelOptions.Controls.Add((Control) this.selectWritableButton);
    this.panelOptions.Controls.Add((Control) this.selectModifiedButton);
    this.panelOptions.Controls.Add((Control) this.selectAllButton);
    this.panelOptions.Controls.Add((Control) this.exportLevelComboBox);
    this.panelOptions.Controls.Add((Control) this.exportLevelLabel);
    componentResourceManager.ApplyResources((object) this.panelOptions, "panelOptions");
    this.panelOptions.Name = "panelOptions";
    componentResourceManager.ApplyResources((object) this.selectNoneButton, "selectNoneButton");
    this.selectNoneButton.Name = "selectNoneButton";
    this.selectNoneButton.UseVisualStyleBackColor = true;
    this.selectNoneButton.Click += new EventHandler(this.OnClickSelectNone);
    componentResourceManager.ApplyResources((object) this.selectWritableButton, "selectWritableButton");
    this.selectWritableButton.Name = "selectWritableButton";
    this.selectWritableButton.UseVisualStyleBackColor = true;
    this.selectWritableButton.Click += new EventHandler(this.OnClickSelectWritable);
    componentResourceManager.ApplyResources((object) this.selectModifiedButton, "selectModifiedButton");
    this.selectModifiedButton.Name = "selectModifiedButton";
    this.selectModifiedButton.UseVisualStyleBackColor = true;
    this.selectModifiedButton.Click += new EventHandler(this.OnClickSelectModified);
    componentResourceManager.ApplyResources((object) this.selectAllButton, "selectAllButton");
    this.selectAllButton.Name = "selectAllButton";
    this.selectAllButton.UseVisualStyleBackColor = true;
    this.selectAllButton.Click += new EventHandler(this.OnClickSelectAll);
    componentResourceManager.ApplyResources((object) this.exportLevelComboBox, "exportLevelComboBox");
    this.exportLevelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
    this.exportLevelComboBox.FormattingEnabled = true;
    this.exportLevelComboBox.Items.AddRange(new object[3]
    {
      (object) componentResourceManager.GetString("exportLevelComboBox.Items"),
      (object) componentResourceManager.GetString("exportLevelComboBox.Items1"),
      (object) componentResourceManager.GetString("exportLevelComboBox.Items2")
    });
    this.exportLevelComboBox.Name = "exportLevelComboBox";
    this.exportLevelComboBox.SelectedIndexChanged += new EventHandler(this.OnLevelSelectionChanged);
    componentResourceManager.ApplyResources((object) this.exportLevelLabel, "exportLevelLabel");
    this.exportLevelLabel.Name = "exportLevelLabel";
    panel.Controls.Add((Control) this.parametersTree);
    panel.Controls.Add((Control) this.pictureBoxTreeMod);
    panel.Controls.Add((Control) this.toolStripExpandCollapse);
    componentResourceManager.ApplyResources((object) panel, "panelTree");
    panel.Name = "panelTree";
    ((TreeView) this.parametersTree).CheckBoxes = true;
    componentResourceManager.ApplyResources((object) this.parametersTree, "parametersTree");
    ((TreeView) this.parametersTree).FullRowSelect = true;
    ((TreeView) this.parametersTree).HideSelection = false;
    ((Control) this.parametersTree).Name = "parametersTree";
    ((TreeView) this.parametersTree).ShowLines = false;
    ((TreeView) this.parametersTree).AfterCheck += new TreeViewEventHandler(this.OnAfterCheck);
    componentResourceManager.ApplyResources((object) this.pictureBoxTreeMod, "pictureBoxTreeMod");
    this.pictureBoxTreeMod.Name = "pictureBoxTreeMod";
    this.pictureBoxTreeMod.TabStop = false;
    this.toolStripExpandCollapse.AllowMerge = false;
    componentResourceManager.ApplyResources((object) this.toolStripExpandCollapse, "toolStripExpandCollapse");
    this.toolStripExpandCollapse.GripStyle = ToolStripGripStyle.Hidden;
    this.toolStripExpandCollapse.ImageScalingSize = new Size(20, 20);
    this.toolStripExpandCollapse.Items.AddRange(new ToolStripItem[2]
    {
      (ToolStripItem) this.toolStripButtonExpandAll,
      (ToolStripItem) this.toolStripButtonCollapseAll
    });
    this.toolStripExpandCollapse.Name = "toolStripExpandCollapse";
    this.toolStripExpandCollapse.Stretch = true;
    this.toolStripExpandCollapse.TabStop = true;
    this.toolStripButtonExpandAll.Image = (Image) Resources.expandall;
    componentResourceManager.ApplyResources((object) this.toolStripButtonExpandAll, "toolStripButtonExpandAll");
    this.toolStripButtonExpandAll.Name = "toolStripButtonExpandAll";
    this.toolStripButtonExpandAll.Click += new EventHandler(this.OnClickExpandAll);
    this.toolStripButtonCollapseAll.Image = (Image) Resources.collapseall;
    componentResourceManager.ApplyResources((object) this.toolStripButtonCollapseAll, "toolStripButtonCollapseAll");
    this.toolStripButtonCollapseAll.Name = "toolStripButtonCollapseAll";
    this.toolStripButtonCollapseAll.Click += new EventHandler(this.OnClickCollapseAll);
    componentResourceManager.ApplyResources((object) this.selectLabel, "selectLabel");
    tableLayoutPanel.SetColumnSpan((Control) this.selectLabel, 2);
    this.selectLabel.Name = "selectLabel";
    componentResourceManager.ApplyResources((object) this.buttonExport, "buttonExport");
    this.buttonExport.Name = "buttonExport";
    this.buttonExport.Click += new EventHandler(this.OnClickExport);
    this.buttonCancel.DialogResult = DialogResult.Cancel;
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.buttonCancel.Name = "buttonCancel";
    componentResourceManager.ApplyResources((object) this.saveFileDialog, "saveFileDialog");
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelLowerButtons, "tableLayoutPanelLowerButtons");
    tableLayoutPanel.SetColumnSpan((Control) this.tableLayoutPanelLowerButtons, 2);
    this.tableLayoutPanelLowerButtons.Controls.Add((Control) this.buttonExport, 1, 0);
    this.tableLayoutPanelLowerButtons.Controls.Add((Control) this.buttonCancel, 2, 0);
    this.tableLayoutPanelLowerButtons.Name = "tableLayoutPanelLowerButtons";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.CancelButton = (IButtonControl) this.buttonCancel;
    this.Controls.Add((Control) tableLayoutPanel);
    this.HelpButton = true;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (ParameterExportForm);
    this.ShowIcon = false;
    this.ShowInTaskbar = false;
    this.HelpButtonClicked += new CancelEventHandler(this.OnHelpButtonClicked);
    tableLayoutPanel.ResumeLayout(false);
    tableLayoutPanel.PerformLayout();
    this.panelOptions.ResumeLayout(false);
    panel.ResumeLayout(false);
    panel.PerformLayout();
    ((ISupportInitialize) this.pictureBoxTreeMod).EndInit();
    this.toolStripExpandCollapse.ResumeLayout(false);
    this.toolStripExpandCollapse.PerformLayout();
    this.tableLayoutPanelLowerButtons.ResumeLayout(false);
    this.ResumeLayout(false);
  }

  private enum CheckAction
  {
    All,
    Modified,
    Writable,
    None,
  }
}
