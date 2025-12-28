// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.FaultCodeTabs.General.All_Faults.panel.UserPanel
// Assembly: FaultCodeTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 35DAF471-66CA-4F8E-B39E-2FF7E69A8BE3
// Assembly location: C:\Users\petra\Downloads\Архив (2)\FaultCodeTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Reflection;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.FaultCodeTabs.General.All_Faults.panel;

public class UserPanel : CustomPanel
{
  private EditSupportHelper editSupport = new EditSupportHelper();
  private ToolTip toolTip;
  private ReadinessControl readinessControl1;
  private Button buttonSymptom;
  private Button buttonFault;
  private Button clearSingleFault;
  private Button clearFaults;
  private ContextMenuStrip contextMenuStrip;
  private ToolStripMenuItem menuItemFaultEngineering;
  private ToolStripMenuItem menuItemFaultAdvanced;
  private ToolStripMenuItem menuItemFaultTraditional;
  private ToolStripMenuItem menuItemTroubleshootSymptoms;
  private ToolStripSeparator toolStripSeparator1;
  private ToolStripMenuItem menuItemClearAllFaults;
  private ToolStripMenuItem menuItemClearFault;
  private TableLayoutPanel tableLayoutPanel1;
  private ToolStripMenuItem manipulationToolStripMenuItem;
  private FaultList faultDisplay;

  public UserPanel()
  {
    this.InitializeComponent();
    if (ApplicationInformation.ProductAccessLevel < 3)
      this.manipulationToolStripMenuItem.Visible = false;
    this.toolTip = new ToolTip();
    this.menuItemFaultAdvanced.Image = FaultListIcons.Dynamic;
    this.menuItemFaultTraditional.Image = FaultListIcons.Static;
    this.menuItemFaultEngineering.Image = FaultListIcons.Engineering;
    this.menuItemFaultAdvanced.Click += (EventHandler) ((sender, e) => this.TroubleshootFault((TroubleshootingType) 0));
    this.menuItemFaultTraditional.Click += (EventHandler) ((sender, e) => this.TroubleshootFault((TroubleshootingType) 1));
    this.menuItemFaultEngineering.Click += (EventHandler) ((sender, e) => this.TroubleshootFault((TroubleshootingType) 2));
    this.buttonFault.Click += (EventHandler) ((sender, e) => this.TroubleshootFault((TroubleshootingType) 4));
    this.menuItemTroubleshootSymptoms.Click += new EventHandler(this.OnTroubleshootSymptom);
    this.menuItemClearFault.Click += new EventHandler(this.OnClearFault);
    this.menuItemClearAllFaults.Click += new EventHandler(this.OnClearAllFaults);
    this.clearFaults.Click += new EventHandler(this.OnClearAllFaults);
    this.clearSingleFault.Click += new EventHandler(this.OnClearFault);
    this.buttonSymptom.Click += new EventHandler(this.OnTroubleshootSymptom);
    this.faultDisplay.SelectionChanged += new EventHandler(this.OnSelectedItemChanged);
    this.faultDisplay.CanClearFaultCodesChanged += new EventHandler(this.OnCanClearFaultCodesChanged);
    ((Control) this.faultDisplay).ContextMenuStrip = this.contextMenuStrip;
    ((ContextHelpControl) this).SetLink(LinkSupport.GetViewLink((PanelIdentifier) 1));
    SapiManager.GlobalInstance.EquipmentTypeChanged += new EventHandler<EquipmentTypeChangedEventArgs>(this.GlobalInstance_EquipmentTypeChanged);
  }

  private void OnTroubleshootSymptom(object sender, EventArgs e)
  {
    if (!(((Control) this).TopLevelControl is IContainerApplication topLevelControl) || topLevelControl.Troubleshooting == null)
      return;
    topLevelControl.Troubleshooting.StartTroubleshootingSymptom();
  }

  private void TroubleshootFault(TroubleshootingType type)
  {
    FaultCode selectedFaultCode = this.faultDisplay.SelectedFaultCode;
    if (selectedFaultCode == null || !(((Control) this).TopLevelControl is IContainerApplication topLevelControl) || topLevelControl.Troubleshooting == null)
      return;
    if (type != 4)
      topLevelControl.Troubleshooting.StartTroubleshootingFault(selectedFaultCode, type);
    else
      topLevelControl.Troubleshooting.StartTroubleshootingFault(selectedFaultCode);
  }

  private void UpdateSymptomTroubleshooting()
  {
    if (!(((Control) this).TopLevelControl is IContainerApplication topLevelControl) || topLevelControl.Troubleshooting == null)
      return;
    switch ((int) topLevelControl.Troubleshooting.CanTroubleshootSymptoms())
    {
      case 0:
        this.menuItemTroubleshootSymptoms.Image = this.buttonSymptom.Image = FaultListIcons.Dynamic;
        this.buttonSymptom.Enabled = this.menuItemTroubleshootSymptoms.Enabled = true;
        break;
      case 1:
        this.menuItemTroubleshootSymptoms.Image = this.buttonSymptom.Image = FaultListIcons.Static;
        this.buttonSymptom.Enabled = this.menuItemTroubleshootSymptoms.Enabled = true;
        break;
      case 2:
        this.menuItemTroubleshootSymptoms.Image = this.buttonSymptom.Image = FaultListIcons.Engineering;
        this.buttonSymptom.Enabled = this.menuItemTroubleshootSymptoms.Enabled = false;
        break;
      case 3:
        this.menuItemTroubleshootSymptoms.Image = this.buttonSymptom.Image = FaultListIcons.None;
        this.buttonSymptom.Enabled = this.menuItemTroubleshootSymptoms.Enabled = false;
        break;
      case 4:
        this.menuItemTroubleshootSymptoms.Image = this.buttonSymptom.Image = FaultListIcons.Unknown;
        this.buttonSymptom.Enabled = this.menuItemTroubleshootSymptoms.Enabled = true;
        break;
      default:
        throw new InvalidOperationException("Unknown enumeration");
    }
  }

  private void UpdateFaultTroubleshooting()
  {
    List<TroubleshootingType> list = this.faultDisplay.SelectedFaultTroubleshootingTypes.ToList<TroubleshootingType>();
    this.menuItemFaultAdvanced.Enabled = list.Contains((TroubleshootingType) 0);
    this.menuItemFaultTraditional.Enabled = list.Contains((TroubleshootingType) 1);
    this.menuItemFaultEngineering.Visible = list.Contains((TroubleshootingType) 2) && ApplicationInformation.ProductAccessLevel > 2;
    switch ((int) this.faultDisplay.SelectedTroubleshootingType)
    {
      case 0:
        this.buttonFault.Image = FaultListIcons.Dynamic;
        this.buttonFault.Enabled = true;
        this.toolTip.SetToolTip((Control) this.buttonFault, Resources.Message_AdvancedDiagnostics);
        break;
      case 1:
        this.buttonFault.Image = FaultListIcons.Static;
        this.buttonFault.Enabled = true;
        this.toolTip.SetToolTip((Control) this.buttonFault, Resources.Message_TraditionalTroubleshooting);
        break;
      case 2:
        this.buttonFault.Image = FaultListIcons.Engineering;
        this.buttonFault.Enabled = true;
        this.toolTip.SetToolTip((Control) this.buttonFault, Resources.Message_EngineeringTroubleshooting);
        break;
      case 3:
        this.buttonFault.Image = FaultListIcons.None;
        this.buttonFault.Enabled = false;
        this.toolTip.SetToolTip((Control) this.buttonFault, Resources.Message_ReferToTechLit);
        break;
      case 4:
        this.buttonFault.Image = FaultListIcons.Unknown;
        this.buttonFault.Enabled = true;
        this.toolTip.SetToolTip((Control) this.buttonFault, Resources.Message_ReferToTroubleshootingView);
        break;
      default:
        throw new InvalidOperationException("Unknown enumeration.");
    }
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.UpdateFaultClearing();
    this.UpdateFaultTroubleshooting();
    this.UpdateSymptomTroubleshooting();
    this.UpdateReadinessControl();
  }

  private void UpdateReadinessControl()
  {
    if (ApplicationInformation.ProductAccessLevel < 3)
      return;
    this.readinessControl1 = new ReadinessControl();
    ((Control) this.readinessControl1).Margin = new Padding(4, 4, 4, 4);
    ((Control) this.readinessControl1).Location = new Point(0, 0);
    ((Control) this.readinessControl1).Dock = DockStyle.Top;
    ((Control) this.readinessControl1).AutoSize = true;
    ((UserControl) this.readinessControl1).AutoSizeMode = AutoSizeMode.GrowAndShrink;
    ((Control) this.readinessControl1).TabIndex = 0;
    ((Control) this.readinessControl1).Size = new Size(799, 0);
    ((Control) this).Controls.Add((Control) this.readinessControl1);
    ((Control) this.readinessControl1).Visible = true;
  }

  private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
  {
    this.UpdateSymptomTroubleshooting();
  }

  private void UpdateFaultClearing()
  {
    this.clearSingleFault.Enabled = this.menuItemClearFault.Enabled = this.faultDisplay.CanClearSelectedFaultCode;
    this.clearFaults.Enabled = this.menuItemClearAllFaults.Enabled = this.faultDisplay.CanClearFaultCodes;
  }

  private void OnClearAllFaults(object sender, EventArgs e) => FaultList.ClearAllFaults();

  private void OnClearFault(object sender, EventArgs e) => this.faultDisplay.ClearSingleFault();

  private void OnSelectedItemChanged(object sender, EventArgs e)
  {
    this.UpdateFaultTroubleshooting();
    this.UpdateFaultClearing();
    this.UpdateManipulationMenuItem();
  }

  private void OnCanClearFaultCodesChanged(object sender, EventArgs e)
  {
    this.UpdateFaultClearing();
  }

  public virtual bool CanUndo
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanUndo;
    }
  }

  public virtual void Undo()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Undo();
  }

  public virtual bool CanCopy
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanCopy;
    }
  }

  public virtual void Copy()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Copy();
  }

  public virtual bool CanDelete
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanDelete;
    }
  }

  public virtual bool CanPaste
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanPaste;
    }
  }

  public virtual void Cut()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Cut();
  }

  public virtual bool CanSelectAll
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanSelectAll;
    }
  }

  public virtual void Delete()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Delete();
  }

  public virtual bool CanCut
  {
    get
    {
      this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
      return this.editSupport.CanCut;
    }
  }

  public virtual void Paste()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.Paste();
  }

  public virtual void SelectAll()
  {
    // ISSUE: explicit non-virtual call
    this.editSupport.SetTarget((object) __nonvirtual (((ContainerControl) this).ActiveControl));
    this.editSupport.SelectAll();
  }

  public bool Search(string searchText, bool caseSensitive, FindMode direction)
  {
    return this.faultDisplay.Search(searchText, caseSensitive, direction);
  }

  [Browsable(false)]
  public bool CanSearch => this.faultDisplay.CanSearch;

  [Browsable(false)]
  public virtual bool CanProvideHtml => this.faultDisplay.CanProvideHtml;

  public virtual string ToHtml() => this.faultDisplay.ToHtml();

  [Browsable(false)]
  public virtual string StyleSheet => this.faultDisplay.StyleSheet;

  private DataItem SelectedDataItem
  {
    get
    {
      DataItem selectedDataItem = (DataItem) null;
      FaultCode selectedFaultCode = this.faultDisplay.SelectedFaultCode;
      if (selectedFaultCode != null)
        selectedDataItem = DataItem.Create(new Qualifier((QualifierTypes) 32 /*0x20*/, selectedFaultCode.Channel.Ecu.Name, selectedFaultCode.Qualifier), (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
      return selectedDataItem;
    }
  }

  private void UpdateManipulationMenuItem()
  {
    this.manipulationToolStripMenuItem.Enabled = ManipulationForm.CanManipulate(this.SelectedDataItem);
  }

  private void manipulationToolStripMenuItem_Click(object sender, EventArgs e)
  {
    ManipulationForm.Show(this.SelectedDataItem);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.clearFaults = new Button();
    this.clearSingleFault = new Button();
    this.buttonSymptom = new Button();
    this.buttonFault = new Button();
    this.faultDisplay = new FaultList();
    this.contextMenuStrip = new ContextMenuStrip(this.components);
    this.menuItemFaultAdvanced = new ToolStripMenuItem();
    this.menuItemFaultTraditional = new ToolStripMenuItem();
    this.menuItemFaultEngineering = new ToolStripMenuItem();
    this.menuItemTroubleshootSymptoms = new ToolStripMenuItem();
    this.manipulationToolStripMenuItem = new ToolStripMenuItem();
    this.toolStripSeparator1 = new ToolStripSeparator();
    this.menuItemClearAllFaults = new ToolStripMenuItem();
    this.menuItemClearFault = new ToolStripMenuItem();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    this.contextMenuStrip.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.clearFaults, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.clearSingleFault, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSymptom, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonFault, 4, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.clearFaults, "clearFaults");
    this.clearFaults.Name = "clearFaults";
    this.clearFaults.UseCompatibleTextRendering = true;
    this.clearFaults.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.clearSingleFault, "clearSingleFault");
    this.clearSingleFault.Name = "clearSingleFault";
    this.clearSingleFault.UseCompatibleTextRendering = true;
    this.clearSingleFault.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonSymptom, "buttonSymptom");
    this.buttonSymptom.Name = "buttonSymptom";
    this.buttonSymptom.UseCompatibleTextRendering = true;
    this.buttonSymptom.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonFault, "buttonFault");
    this.buttonFault.Name = "buttonFault";
    this.buttonFault.UseCompatibleTextRendering = true;
    this.buttonFault.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.faultDisplay, "faultDisplay");
    this.faultDisplay.FaultPrioritization = false;
    ((Control) this.faultDisplay).Name = "faultDisplay";
    this.contextMenuStrip.Items.AddRange(new ToolStripItem[8]
    {
      (ToolStripItem) this.menuItemFaultAdvanced,
      (ToolStripItem) this.menuItemFaultTraditional,
      (ToolStripItem) this.menuItemFaultEngineering,
      (ToolStripItem) this.menuItemTroubleshootSymptoms,
      (ToolStripItem) this.manipulationToolStripMenuItem,
      (ToolStripItem) this.toolStripSeparator1,
      (ToolStripItem) this.menuItemClearAllFaults,
      (ToolStripItem) this.menuItemClearFault
    });
    this.contextMenuStrip.Name = "contextMenuStrip";
    componentResourceManager.ApplyResources((object) this.contextMenuStrip, "contextMenuStrip");
    componentResourceManager.ApplyResources((object) this.menuItemFaultAdvanced, "menuItemFaultAdvanced");
    this.menuItemFaultAdvanced.Name = "menuItemFaultAdvanced";
    componentResourceManager.ApplyResources((object) this.menuItemFaultTraditional, "menuItemFaultTraditional");
    this.menuItemFaultTraditional.Name = "menuItemFaultTraditional";
    this.menuItemFaultEngineering.Name = "menuItemFaultEngineering";
    componentResourceManager.ApplyResources((object) this.menuItemFaultEngineering, "menuItemFaultEngineering");
    this.menuItemTroubleshootSymptoms.Name = "menuItemTroubleshootSymptoms";
    componentResourceManager.ApplyResources((object) this.menuItemTroubleshootSymptoms, "menuItemTroubleshootSymptoms");
    this.manipulationToolStripMenuItem.Name = "manipulationToolStripMenuItem";
    componentResourceManager.ApplyResources((object) this.manipulationToolStripMenuItem, "manipulationToolStripMenuItem");
    this.manipulationToolStripMenuItem.Click += new EventHandler(this.manipulationToolStripMenuItem_Click);
    this.toolStripSeparator1.Name = "toolStripSeparator1";
    componentResourceManager.ApplyResources((object) this.toolStripSeparator1, "toolStripSeparator1");
    this.menuItemClearAllFaults.Name = "menuItemClearAllFaults";
    componentResourceManager.ApplyResources((object) this.menuItemClearAllFaults, "menuItemClearAllFaults");
    this.menuItemClearFault.Name = "menuItemClearFault";
    componentResourceManager.ApplyResources((object) this.menuItemClearFault, "menuItemClearFault");
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.faultDisplay);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    this.contextMenuStrip.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }
}
