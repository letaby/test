using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Shift_Abort_Counter.panel;

public class UserPanel : CustomPanel, IRefreshable
{
	private readonly string[] shiftAbortCounterRequestMessages = new string[3] { "222200", "222201", "222202" };

	private Channel tcm = null;

	private TableLayoutPanel tableLayoutPanel1;

	private ColumnHeader columnHeader1;

	private ColumnHeader columnHeader2;

	private RunServiceButton runServiceButtonClear;

	private RunServiceButton runServiceButtonSave;

	private ListViewEx listView;

	public bool CanRefreshView => SapiManager.GlobalInstance.Online && tcm != null && tcm.CommunicationsState == CommunicationsState.Online;

	public UserPanel()
	{
		InitializeComponent();
		runServiceButtonClear.ServiceComplete += OnServiceComplete;
		runServiceButtonSave.ServiceComplete += OnServiceComplete;
	}

	public override void OnChannelsChanged()
	{
		UpdateChannel();
	}

	private void UpdateChannel()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		Channel channel = ((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)7);
		if (tcm == channel)
		{
			return;
		}
		if (tcm != null)
		{
			tcm.EcuInfos.EcuInfoUpdateEvent -= ecuInfos_EcuInfoUpdateEvent;
			foreach (ListViewExGroupItem item in ((ListView)(object)listView).Items)
			{
				ListViewExGroupItem val = item;
				if (val != null)
				{
					val.RemoveAll();
					((ListViewItem)(object)val).Remove();
				}
			}
		}
		tcm = channel;
		if (tcm != null)
		{
			tcm.EcuInfos.EcuInfoUpdateEvent += ecuInfos_EcuInfoUpdateEvent;
			AddItemsList();
			UpdateTcmServiceCalls();
		}
	}

	private void UpdateTcmServiceCalls()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (tcm != null)
		{
			if (tcm.Ecu.Name == "TCM01T")
			{
				runServiceButtonClear.ServiceCall = new ServiceCall("TCM01T", "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start");
				runServiceButtonSave.ServiceCall = new ServiceCall("TCM01T", "RT_0440_Schaltabbruchzaehler_sichern_Start");
			}
			else if (tcm.Ecu.Name == "TCM05T")
			{
				runServiceButtonClear.ServiceCall = new ServiceCall("TCM05T", "RT_0441_Reset_shift_abort_counters_Start");
				runServiceButtonSave.ServiceCall = new ServiceCall("TCM05T", "RT_0440_Save_copy_of_shift_abort_counters_Start");
			}
		}
	}

	private void AddItemsList()
	{
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		listView.BeginUpdate();
		IEnumerable<IGrouping<string, EcuInfo>> enumerable = from ecuinfo in tcm.EcuInfos
			where ecuinfo.Services.Count > 0
			let requestmessage = ecuinfo.Services[0].RequestMessage.ToString()
			orderby requestmessage
			where shiftAbortCounterRequestMessages.Contains(requestmessage)
			group ecuinfo by requestmessage;
		foreach (IGrouping<string, EcuInfo> item in enumerable)
		{
			string text = item.First().Name.Split(":".ToCharArray())[0];
			ListViewExGroupItem val = new ListViewExGroupItem(text);
			((ListViewItem)(object)val).SubItems.Add(string.Empty);
			((ListView)(object)listView).Items.Add((ListViewItem)(object)val);
			foreach (EcuInfo item2 in item)
			{
				string text2 = item2.Name.Split(":".ToCharArray())[1];
				ListViewExGroupItem val2 = new ListViewExGroupItem(text2);
				((ListViewItem)(object)val2).SubItems.Add(item2.Value ?? string.Empty);
				((ListViewItem)(object)val2).Tag = item2;
				val.Add(val2);
			}
		}
		listView.EndUpdate();
	}

	private void ecuInfos_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
	{
		EcuInfo ecuInfo = sender as EcuInfo;
		ListViewExGroupItem val = ((ListView)(object)listView).Items.Cast<ListViewExGroupItem>().FirstOrDefault((ListViewExGroupItem li) => ((ListViewItem)(object)li).Tag == ecuInfo);
		if (val != null)
		{
			((ListViewItem)(object)val).SubItems[1].Text = ecuInfo.Value ?? string.Empty;
		}
	}

	private void OnServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			MessageBox.Show(e.Service.Name + Resources.Message_CompletedSuccessfully, ApplicationInformation.ProductName);
		}
		else
		{
			MessageBox.Show(e.Service.Name + Resources.Message_Failed0 + ((ResultEventArgs)(object)e).Exception.Message + "'", ApplicationInformation.ProductName);
		}
	}

	public void RefreshView()
	{
		if (SapiManager.GlobalInstance.Online && tcm != null && tcm.CommunicationsState == CommunicationsState.Online)
		{
			tcm.EcuInfos.Read(synchronous: false);
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		listView = new ListViewEx();
		columnHeader1 = new ColumnHeader();
		columnHeader2 = new ColumnHeader();
		runServiceButtonClear = new RunServiceButton();
		runServiceButtonSave = new RunServiceButton();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((ISupportInitialize)listView).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)listView, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonClear, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButtonSave, 2, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		listView.CanDelete = false;
		((ListView)(object)listView).Columns.AddRange(new ColumnHeader[2] { columnHeader1, columnHeader2 });
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)listView, 3);
		componentResourceManager.ApplyResources(listView, "listView");
		listView.EditableColumn = -1;
		listView.GridLines = true;
		((Control)(object)listView).Name = "listView";
		((ListView)(object)listView).UseCompatibleStateImageBehavior = false;
		componentResourceManager.ApplyResources(columnHeader1, "columnHeader1");
		componentResourceManager.ApplyResources(columnHeader2, "columnHeader2");
		componentResourceManager.ApplyResources(runServiceButtonClear, "runServiceButtonClear");
		((Control)(object)runServiceButtonClear).Name = "runServiceButtonClear";
		runServiceButtonClear.ServiceCall = new ServiceCall("TCM01T", "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start");
		componentResourceManager.ApplyResources(runServiceButtonSave, "runServiceButtonSave");
		((Control)(object)runServiceButtonSave).Name = "runServiceButtonSave";
		runServiceButtonSave.ServiceCall = new ServiceCall("TCM01T", "RT_0440_Schaltabbruchzaehler_sichern_Start");
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Transmission_Shift_Abort_Counter");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((ISupportInitialize)listView).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
