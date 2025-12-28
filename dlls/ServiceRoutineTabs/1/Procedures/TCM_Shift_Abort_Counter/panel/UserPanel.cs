// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Shift_Abort_Counter.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Shift_Abort_Counter.panel;

public class UserPanel : CustomPanel, IRefreshable
{
  private readonly string[] shiftAbortCounterRequestMessages = new string[3]
  {
    "222200",
    "222201",
    "222202"
  };
  private Channel tcm = (Channel) null;
  private TableLayoutPanel tableLayoutPanel1;
  private ColumnHeader columnHeader1;
  private ColumnHeader columnHeader2;
  private RunServiceButton runServiceButtonClear;
  private RunServiceButton runServiceButtonSave;
  private ListViewEx listView;

  public UserPanel()
  {
    this.InitializeComponent();
    this.runServiceButtonClear.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.OnServiceComplete);
    this.runServiceButtonSave.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.OnServiceComplete);
  }

  public virtual void OnChannelsChanged() => this.UpdateChannel();

  private void UpdateChannel()
  {
    Channel channel = this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 7);
    if (this.tcm == channel)
      return;
    if (this.tcm != null)
    {
      this.tcm.EcuInfos.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.ecuInfos_EcuInfoUpdateEvent);
      foreach (ListViewExGroupItem listViewExGroupItem in ((ListView) this.listView).Items)
      {
        if (listViewExGroupItem != null)
        {
          listViewExGroupItem.RemoveAll();
          ((ListViewItem) listViewExGroupItem).Remove();
        }
      }
    }
    this.tcm = channel;
    if (this.tcm != null)
    {
      this.tcm.EcuInfos.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.ecuInfos_EcuInfoUpdateEvent);
      this.AddItemsList();
      this.UpdateTcmServiceCalls();
    }
  }

  private void UpdateTcmServiceCalls()
  {
    if (this.tcm == null)
      return;
    if (this.tcm.Ecu.Name == "TCM01T")
    {
      this.runServiceButtonClear.ServiceCall = new ServiceCall("TCM01T", "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start");
      this.runServiceButtonSave.ServiceCall = new ServiceCall("TCM01T", "RT_0440_Schaltabbruchzaehler_sichern_Start");
    }
    else if (this.tcm.Ecu.Name == "TCM05T")
    {
      this.runServiceButtonClear.ServiceCall = new ServiceCall("TCM05T", "RT_0441_Reset_shift_abort_counters_Start");
      this.runServiceButtonSave.ServiceCall = new ServiceCall("TCM05T", "RT_0440_Save_copy_of_shift_abort_counters_Start");
    }
  }

  private void AddItemsList()
  {
    this.listView.BeginUpdate();
    foreach (IGrouping<string, EcuInfo> source in this.tcm.EcuInfos.Where<EcuInfo>((Func<EcuInfo, bool>) (ecuinfo => ecuinfo.Services.Count > 0)).Select(ecuinfo =>
    {
      var data = new
      {
        ecuinfo = ecuinfo,
        requestmessage = ecuinfo.Services[0].RequestMessage.ToString()
      };
      return data;
    }).OrderBy(_param0 => _param0.requestmessage).Where(_param1 => ((IEnumerable<string>) this.shiftAbortCounterRequestMessages).Contains<string>(_param1.requestmessage)).GroupBy(_param0 => _param0.requestmessage, _param0 => _param0.ecuinfo))
    {
      ListViewExGroupItem listViewExGroupItem1 = new ListViewExGroupItem(source.First<EcuInfo>().Name.Split(":".ToCharArray())[0]);
      ((ListViewItem) listViewExGroupItem1).SubItems.Add(string.Empty);
      ((ListView) this.listView).Items.Add((ListViewItem) listViewExGroupItem1);
      foreach (EcuInfo ecuInfo in (IEnumerable<EcuInfo>) source)
      {
        ListViewExGroupItem listViewExGroupItem2 = new ListViewExGroupItem(ecuInfo.Name.Split(":".ToCharArray())[1]);
        ((ListViewItem) listViewExGroupItem2).SubItems.Add(ecuInfo.Value ?? string.Empty);
        ((ListViewItem) listViewExGroupItem2).Tag = (object) ecuInfo;
        listViewExGroupItem1.Add(listViewExGroupItem2);
      }
    }
    this.listView.EndUpdate();
  }

  private void ecuInfos_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
  {
    EcuInfo ecuInfo = sender as EcuInfo;
    ListViewExGroupItem listViewExGroupItem = ((ListView) this.listView).Items.Cast<ListViewExGroupItem>().FirstOrDefault<ListViewExGroupItem>((Func<ListViewExGroupItem, bool>) (li => ((ListViewItem) li).Tag == ecuInfo));
    if (listViewExGroupItem == null)
      return;
    ((ListViewItem) listViewExGroupItem).SubItems[1].Text = ecuInfo.Value ?? string.Empty;
  }

  private void OnServiceComplete(object sender, SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      int num1 = (int) MessageBox.Show(e.Service.Name + Resources.Message_CompletedSuccessfully, ApplicationInformation.ProductName);
    }
    else
    {
      int num2 = (int) MessageBox.Show($"{e.Service.Name}{Resources.Message_Failed0}{((ResultEventArgs) e).Exception.Message}'", ApplicationInformation.ProductName);
    }
  }

  public bool CanRefreshView
  {
    get
    {
      return SapiManager.GlobalInstance.Online && this.tcm != null && this.tcm.CommunicationsState == CommunicationsState.Online;
    }
  }

  public void RefreshView()
  {
    if (!SapiManager.GlobalInstance.Online || this.tcm == null || this.tcm.CommunicationsState != CommunicationsState.Online)
      return;
    this.tcm.EcuInfos.Read(false);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.listView = new ListViewEx();
    this.columnHeader1 = new ColumnHeader();
    this.columnHeader2 = new ColumnHeader();
    this.runServiceButtonClear = new RunServiceButton();
    this.runServiceButtonSave = new RunServiceButton();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((ISupportInitialize) this.listView).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.listView, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonClear, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButtonSave, 2, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.listView.CanDelete = false;
    ((ListView) this.listView).Columns.AddRange(new ColumnHeader[2]
    {
      this.columnHeader1,
      this.columnHeader2
    });
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.listView, 3);
    componentResourceManager.ApplyResources((object) this.listView, "listView");
    this.listView.EditableColumn = -1;
    this.listView.GridLines = true;
    ((Control) this.listView).Name = "listView";
    ((ListView) this.listView).UseCompatibleStateImageBehavior = false;
    componentResourceManager.ApplyResources((object) this.columnHeader1, "columnHeader1");
    componentResourceManager.ApplyResources((object) this.columnHeader2, "columnHeader2");
    componentResourceManager.ApplyResources((object) this.runServiceButtonClear, "runServiceButtonClear");
    ((Control) this.runServiceButtonClear).Name = "runServiceButtonClear";
    this.runServiceButtonClear.ServiceCall = new ServiceCall("TCM01T", "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start");
    componentResourceManager.ApplyResources((object) this.runServiceButtonSave, "runServiceButtonSave");
    ((Control) this.runServiceButtonSave).Name = "runServiceButtonSave";
    this.runServiceButtonSave.ServiceCall = new ServiceCall("TCM01T", "RT_0440_Schaltabbruchzaehler_sichern_Start");
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Transmission_Shift_Abort_Counter");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((ISupportInitialize) this.listView).EndInit();
    ((Control) this).ResumeLayout(false);
  }
}
