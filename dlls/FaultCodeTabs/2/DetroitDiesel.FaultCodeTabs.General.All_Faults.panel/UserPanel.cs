using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Reflection;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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

	public override bool CanUndo
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanUndo;
		}
	}

	public override bool CanCopy
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanCopy;
		}
	}

	public override bool CanDelete
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanDelete;
		}
	}

	public override bool CanPaste
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanPaste;
		}
	}

	public override bool CanSelectAll
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanSelectAll;
		}
	}

	public override bool CanCut
	{
		get
		{
			editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
			return editSupport.CanCut;
		}
	}

	[Browsable(false)]
	public bool CanSearch => faultDisplay.CanSearch;

	[Browsable(false)]
	public override bool CanProvideHtml => faultDisplay.CanProvideHtml;

	[Browsable(false)]
	public override string StyleSheet => faultDisplay.StyleSheet;

	private DataItem SelectedDataItem
	{
		get
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			DataItem result = null;
			FaultCode selectedFaultCode = faultDisplay.SelectedFaultCode;
			if (selectedFaultCode != null)
			{
				result = DataItem.Create(new Qualifier((QualifierTypes)32, selectedFaultCode.Channel.Ecu.Name, selectedFaultCode.Qualifier), (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
			}
			return result;
		}
	}

	public UserPanel()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
		if (ApplicationInformation.ProductAccessLevel < 3)
		{
			manipulationToolStripMenuItem.Visible = false;
		}
		toolTip = new ToolTip();
		menuItemFaultAdvanced.Image = FaultListIcons.Dynamic;
		menuItemFaultTraditional.Image = FaultListIcons.Static;
		menuItemFaultEngineering.Image = FaultListIcons.Engineering;
		menuItemFaultAdvanced.Click += delegate
		{
			TroubleshootFault((TroubleshootingType)0);
		};
		menuItemFaultTraditional.Click += delegate
		{
			TroubleshootFault((TroubleshootingType)1);
		};
		menuItemFaultEngineering.Click += delegate
		{
			TroubleshootFault((TroubleshootingType)2);
		};
		buttonFault.Click += delegate
		{
			TroubleshootFault((TroubleshootingType)4);
		};
		menuItemTroubleshootSymptoms.Click += OnTroubleshootSymptom;
		menuItemClearFault.Click += OnClearFault;
		menuItemClearAllFaults.Click += OnClearAllFaults;
		clearFaults.Click += OnClearAllFaults;
		clearSingleFault.Click += OnClearFault;
		buttonSymptom.Click += OnTroubleshootSymptom;
		faultDisplay.SelectionChanged += OnSelectedItemChanged;
		faultDisplay.CanClearFaultCodesChanged += OnCanClearFaultCodesChanged;
		((Control)(object)faultDisplay).ContextMenuStrip = contextMenuStrip;
		((ContextHelpControl)this).SetLink(LinkSupport.GetViewLink((PanelIdentifier)1));
		SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
	}

	private void OnTroubleshootSymptom(object sender, EventArgs e)
	{
		Control topLevelControl = ((Control)this).TopLevelControl;
		IContainerApplication val = (IContainerApplication)(object)((topLevelControl is IContainerApplication) ? topLevelControl : null);
		if (val != null && val.Troubleshooting != null)
		{
			val.Troubleshooting.StartTroubleshootingSymptom();
		}
	}

	private void TroubleshootFault(TroubleshootingType type)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		FaultCode selectedFaultCode = faultDisplay.SelectedFaultCode;
		if (selectedFaultCode == null)
		{
			return;
		}
		Control topLevelControl = ((Control)this).TopLevelControl;
		IContainerApplication val = (IContainerApplication)(object)((topLevelControl is IContainerApplication) ? topLevelControl : null);
		if (val != null && val.Troubleshooting != null)
		{
			if ((int)type != 4)
			{
				val.Troubleshooting.StartTroubleshootingFault(selectedFaultCode, type);
			}
			else
			{
				val.Troubleshooting.StartTroubleshootingFault(selectedFaultCode);
			}
		}
	}

	private void UpdateSymptomTroubleshooting()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected I4, but got Unknown
		Control topLevelControl = ((Control)this).TopLevelControl;
		IContainerApplication val = (IContainerApplication)(object)((topLevelControl is IContainerApplication) ? topLevelControl : null);
		if (val != null && val.Troubleshooting != null)
		{
			TroubleshootingType val2 = val.Troubleshooting.CanTroubleshootSymptoms();
			switch ((int)val2)
			{
			case 0:
			{
				ToolStripMenuItem toolStripMenuItem5 = menuItemTroubleshootSymptoms;
				Image image = (buttonSymptom.Image = FaultListIcons.Dynamic);
				toolStripMenuItem5.Image = image;
				Button button5 = buttonSymptom;
				bool enabled = (menuItemTroubleshootSymptoms.Enabled = true);
				button5.Enabled = enabled;
				break;
			}
			case 1:
			{
				ToolStripMenuItem toolStripMenuItem4 = menuItemTroubleshootSymptoms;
				Image image = (buttonSymptom.Image = FaultListIcons.Static);
				toolStripMenuItem4.Image = image;
				Button button4 = buttonSymptom;
				bool enabled = (menuItemTroubleshootSymptoms.Enabled = true);
				button4.Enabled = enabled;
				break;
			}
			case 2:
			{
				ToolStripMenuItem toolStripMenuItem3 = menuItemTroubleshootSymptoms;
				Image image = (buttonSymptom.Image = FaultListIcons.Engineering);
				toolStripMenuItem3.Image = image;
				Button button3 = buttonSymptom;
				bool enabled = (menuItemTroubleshootSymptoms.Enabled = false);
				button3.Enabled = enabled;
				break;
			}
			case 4:
			{
				ToolStripMenuItem toolStripMenuItem2 = menuItemTroubleshootSymptoms;
				Image image = (buttonSymptom.Image = FaultListIcons.Unknown);
				toolStripMenuItem2.Image = image;
				Button button2 = buttonSymptom;
				bool enabled = (menuItemTroubleshootSymptoms.Enabled = true);
				button2.Enabled = enabled;
				break;
			}
			case 3:
			{
				ToolStripMenuItem toolStripMenuItem = menuItemTroubleshootSymptoms;
				Image image = (buttonSymptom.Image = FaultListIcons.None);
				toolStripMenuItem.Image = image;
				Button button = buttonSymptom;
				bool enabled = (menuItemTroubleshootSymptoms.Enabled = false);
				button.Enabled = enabled;
				break;
			}
			default:
				throw new InvalidOperationException("Unknown enumeration");
			}
		}
	}

	private void UpdateFaultTroubleshooting()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected I4, but got Unknown
		List<TroubleshootingType> list = faultDisplay.SelectedFaultTroubleshootingTypes.ToList();
		menuItemFaultAdvanced.Enabled = list.Contains((TroubleshootingType)0);
		menuItemFaultTraditional.Enabled = list.Contains((TroubleshootingType)1);
		menuItemFaultEngineering.Visible = list.Contains((TroubleshootingType)2) && ApplicationInformation.ProductAccessLevel > 2;
		TroubleshootingType selectedTroubleshootingType = faultDisplay.SelectedTroubleshootingType;
		switch ((int)selectedTroubleshootingType)
		{
		case 0:
			buttonFault.Image = FaultListIcons.Dynamic;
			buttonFault.Enabled = true;
			toolTip.SetToolTip(buttonFault, Resources.Message_AdvancedDiagnostics);
			break;
		case 1:
			buttonFault.Image = FaultListIcons.Static;
			buttonFault.Enabled = true;
			toolTip.SetToolTip(buttonFault, Resources.Message_TraditionalTroubleshooting);
			break;
		case 2:
			buttonFault.Image = FaultListIcons.Engineering;
			buttonFault.Enabled = true;
			toolTip.SetToolTip(buttonFault, Resources.Message_EngineeringTroubleshooting);
			break;
		case 4:
			buttonFault.Image = FaultListIcons.Unknown;
			buttonFault.Enabled = true;
			toolTip.SetToolTip(buttonFault, Resources.Message_ReferToTroubleshootingView);
			break;
		case 3:
			buttonFault.Image = FaultListIcons.None;
			buttonFault.Enabled = false;
			toolTip.SetToolTip(buttonFault, Resources.Message_ReferToTechLit);
			break;
		default:
			throw new InvalidOperationException("Unknown enumeration.");
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		UpdateFaultClearing();
		UpdateFaultTroubleshooting();
		UpdateSymptomTroubleshooting();
		UpdateReadinessControl();
	}

	private void UpdateReadinessControl()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		if (ApplicationInformation.ProductAccessLevel >= 3)
		{
			readinessControl1 = new ReadinessControl();
			((Control)(object)readinessControl1).Margin = new Padding(4, 4, 4, 4);
			((Control)(object)readinessControl1).Location = new Point(0, 0);
			((Control)(object)readinessControl1).Dock = DockStyle.Top;
			((Control)(object)readinessControl1).AutoSize = true;
			((UserControl)(object)readinessControl1).AutoSizeMode = AutoSizeMode.GrowAndShrink;
			((Control)(object)readinessControl1).TabIndex = 0;
			((Control)(object)readinessControl1).Size = new Size(799, 0);
			((Control)this).Controls.Add((Control)(object)readinessControl1);
			((Control)(object)readinessControl1).Visible = true;
		}
	}

	private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		UpdateSymptomTroubleshooting();
	}

	private void UpdateFaultClearing()
	{
		Button button = clearSingleFault;
		bool enabled = (menuItemClearFault.Enabled = faultDisplay.CanClearSelectedFaultCode);
		button.Enabled = enabled;
		Button button2 = clearFaults;
		enabled = (menuItemClearAllFaults.Enabled = faultDisplay.CanClearFaultCodes);
		button2.Enabled = enabled;
	}

	private void OnClearAllFaults(object sender, EventArgs e)
	{
		FaultList.ClearAllFaults();
	}

	private void OnClearFault(object sender, EventArgs e)
	{
		faultDisplay.ClearSingleFault();
	}

	private void OnSelectedItemChanged(object sender, EventArgs e)
	{
		UpdateFaultTroubleshooting();
		UpdateFaultClearing();
		UpdateManipulationMenuItem();
	}

	private void OnCanClearFaultCodesChanged(object sender, EventArgs e)
	{
		UpdateFaultClearing();
	}

	public override void Undo()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Undo();
	}

	public override void Copy()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Copy();
	}

	public override void Cut()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Cut();
	}

	public override void Delete()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Delete();
	}

	public override void Paste()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.Paste();
	}

	public override void SelectAll()
	{
		editSupport.SetTarget((object)((ContainerControl)this).ActiveControl);
		editSupport.SelectAll();
	}

	public bool Search(string searchText, bool caseSensitive, FindMode direction)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return faultDisplay.Search(searchText, caseSensitive, direction);
	}

	public override string ToHtml()
	{
		return faultDisplay.ToHtml();
	}

	private void UpdateManipulationMenuItem()
	{
		manipulationToolStripMenuItem.Enabled = ManipulationForm.CanManipulate(SelectedDataItem);
	}

	private void manipulationToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ManipulationForm.Show(SelectedDataItem);
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		clearFaults = new Button();
		clearSingleFault = new Button();
		buttonSymptom = new Button();
		buttonFault = new Button();
		faultDisplay = new FaultList();
		contextMenuStrip = new ContextMenuStrip(base.components);
		menuItemFaultAdvanced = new ToolStripMenuItem();
		menuItemFaultTraditional = new ToolStripMenuItem();
		menuItemFaultEngineering = new ToolStripMenuItem();
		menuItemTroubleshootSymptoms = new ToolStripMenuItem();
		manipulationToolStripMenuItem = new ToolStripMenuItem();
		toolStripSeparator1 = new ToolStripSeparator();
		menuItemClearAllFaults = new ToolStripMenuItem();
		menuItemClearFault = new ToolStripMenuItem();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		contextMenuStrip.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(clearFaults, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(clearSingleFault, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSymptom, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonFault, 4, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(clearFaults, "clearFaults");
		clearFaults.Name = "clearFaults";
		clearFaults.UseCompatibleTextRendering = true;
		clearFaults.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(clearSingleFault, "clearSingleFault");
		clearSingleFault.Name = "clearSingleFault";
		clearSingleFault.UseCompatibleTextRendering = true;
		clearSingleFault.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonSymptom, "buttonSymptom");
		buttonSymptom.Name = "buttonSymptom";
		buttonSymptom.UseCompatibleTextRendering = true;
		buttonSymptom.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonFault, "buttonFault");
		buttonFault.Name = "buttonFault";
		buttonFault.UseCompatibleTextRendering = true;
		buttonFault.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(faultDisplay, "faultDisplay");
		faultDisplay.FaultPrioritization = false;
		((Control)(object)faultDisplay).Name = "faultDisplay";
		contextMenuStrip.Items.AddRange(new ToolStripItem[8] { menuItemFaultAdvanced, menuItemFaultTraditional, menuItemFaultEngineering, menuItemTroubleshootSymptoms, manipulationToolStripMenuItem, toolStripSeparator1, menuItemClearAllFaults, menuItemClearFault });
		contextMenuStrip.Name = "contextMenuStrip";
		componentResourceManager.ApplyResources(contextMenuStrip, "contextMenuStrip");
		componentResourceManager.ApplyResources(menuItemFaultAdvanced, "menuItemFaultAdvanced");
		menuItemFaultAdvanced.Name = "menuItemFaultAdvanced";
		componentResourceManager.ApplyResources(menuItemFaultTraditional, "menuItemFaultTraditional");
		menuItemFaultTraditional.Name = "menuItemFaultTraditional";
		menuItemFaultEngineering.Name = "menuItemFaultEngineering";
		componentResourceManager.ApplyResources(menuItemFaultEngineering, "menuItemFaultEngineering");
		menuItemTroubleshootSymptoms.Name = "menuItemTroubleshootSymptoms";
		componentResourceManager.ApplyResources(menuItemTroubleshootSymptoms, "menuItemTroubleshootSymptoms");
		manipulationToolStripMenuItem.Name = "manipulationToolStripMenuItem";
		componentResourceManager.ApplyResources(manipulationToolStripMenuItem, "manipulationToolStripMenuItem");
		manipulationToolStripMenuItem.Click += manipulationToolStripMenuItem_Click;
		toolStripSeparator1.Name = "toolStripSeparator1";
		componentResourceManager.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
		menuItemClearAllFaults.Name = "menuItemClearAllFaults";
		componentResourceManager.ApplyResources(menuItemClearAllFaults, "menuItemClearAllFaults");
		menuItemClearFault.Name = "menuItemClearFault";
		componentResourceManager.ApplyResources(menuItemClearFault, "menuItemClearFault");
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)faultDisplay);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		contextMenuStrip.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
