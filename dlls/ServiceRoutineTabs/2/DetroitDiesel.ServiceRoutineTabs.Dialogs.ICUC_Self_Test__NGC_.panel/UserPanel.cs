using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Self_Test__NGC_.panel;

public class UserPanel : CustomPanel
{
	private RunServiceButton runServiceButtonGaugeTestStart;

	private TableLayoutPanel tableLayoutPanelMain;

	private RunServiceButton runServiceButtonDisplayTestStop;

	private System.Windows.Forms.Label labelDisplay;

	private RunServiceButton runServiceButtonLampTestStop;

	private System.Windows.Forms.Label labelIndicatorLamp;

	private RunServiceButton runServiceButtonLampTestStart;

	private RunServiceButton runServiceButtonGaugeTestStop;

	private System.Windows.Forms.Label labelGaugeSweep;

	private RunServiceButton runServiceButtonIllumTestStart;

	private System.Windows.Forms.Label labelIllumination;

	private RunServiceButton runServiceButtonIllumTestStop;

	private RunServiceButton runServiceButtonDisplayTestStart;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		Channel channel = ((CustomPanel)this).GetChannel("ICUC01T");
		if (channel != null && channel.Online)
		{
			Service service = channel.Services["FN_HardReset"];
			if (service != null)
			{
				service.Execute(synchronous: false);
			}
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		runServiceButtonGaugeTestStart = new RunServiceButton();
		tableLayoutPanelMain = new TableLayoutPanel();
		runServiceButtonDisplayTestStop = new RunServiceButton();
		labelDisplay = new System.Windows.Forms.Label();
		runServiceButtonLampTestStop = new RunServiceButton();
		labelIndicatorLamp = new System.Windows.Forms.Label();
		runServiceButtonLampTestStart = new RunServiceButton();
		runServiceButtonGaugeTestStop = new RunServiceButton();
		labelGaugeSweep = new System.Windows.Forms.Label();
		runServiceButtonIllumTestStart = new RunServiceButton();
		labelIllumination = new System.Windows.Forms.Label();
		runServiceButtonIllumTestStop = new RunServiceButton();
		runServiceButtonDisplayTestStart = new RunServiceButton();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(runServiceButtonGaugeTestStart, "runServiceButtonGaugeTestStart");
		((Control)(object)runServiceButtonGaugeTestStart).Name = "runServiceButtonGaugeTestStart";
		runServiceButtonGaugeTestStart.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=1", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" });
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)runServiceButtonDisplayTestStop, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelDisplay, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)runServiceButtonLampTestStop, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelIndicatorLamp, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)runServiceButtonLampTestStart, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)runServiceButtonGaugeTestStop, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)runServiceButtonGaugeTestStart, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelGaugeSweep, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)runServiceButtonIllumTestStart, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelIllumination, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)runServiceButtonIllumTestStop, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)runServiceButtonDisplayTestStart, 1, 3);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(runServiceButtonDisplayTestStop, "runServiceButtonDisplayTestStop");
		((Control)(object)runServiceButtonDisplayTestStop).Name = "runServiceButtonDisplayTestStop";
		runServiceButtonDisplayTestStop.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=4", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" });
		componentResourceManager.ApplyResources(labelDisplay, "labelDisplay");
		labelDisplay.Name = "labelDisplay";
		labelDisplay.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(runServiceButtonLampTestStop, "runServiceButtonLampTestStop");
		((Control)(object)runServiceButtonLampTestStop).Name = "runServiceButtonLampTestStop";
		runServiceButtonLampTestStop.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=2", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" });
		componentResourceManager.ApplyResources(labelIndicatorLamp, "labelIndicatorLamp");
		labelIndicatorLamp.Name = "labelIndicatorLamp";
		labelIndicatorLamp.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(runServiceButtonLampTestStart, "runServiceButtonLampTestStart");
		((Control)(object)runServiceButtonLampTestStart).Name = "runServiceButtonLampTestStart";
		runServiceButtonLampTestStart.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=2", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" });
		componentResourceManager.ApplyResources(runServiceButtonGaugeTestStop, "runServiceButtonGaugeTestStop");
		((Control)(object)runServiceButtonGaugeTestStop).Name = "runServiceButtonGaugeTestStop";
		runServiceButtonGaugeTestStop.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=1", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" });
		componentResourceManager.ApplyResources(labelGaugeSweep, "labelGaugeSweep");
		labelGaugeSweep.Name = "labelGaugeSweep";
		labelGaugeSweep.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(runServiceButtonIllumTestStart, "runServiceButtonIllumTestStart");
		((Control)(object)runServiceButtonIllumTestStart).Name = "runServiceButtonIllumTestStart";
		runServiceButtonIllumTestStart.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=3", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" });
		componentResourceManager.ApplyResources(labelIllumination, "labelIllumination");
		labelIllumination.Name = "labelIllumination";
		labelIllumination.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(runServiceButtonIllumTestStop, "runServiceButtonIllumTestStop");
		((Control)(object)runServiceButtonIllumTestStop).Name = "runServiceButtonIllumTestStop";
		runServiceButtonIllumTestStop.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=3", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" });
		componentResourceManager.ApplyResources(runServiceButtonDisplayTestStart, "runServiceButtonDisplayTestStart");
		((Control)(object)runServiceButtonDisplayTestStart).Name = "runServiceButtonDisplayTestStart";
		runServiceButtonDisplayTestStart.ServiceCall = new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>)new string[3] { "OptionRecord_Byte5=4", "OptionRecord_Byte6=00", "OptionRecord_Byte7=00" });
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
