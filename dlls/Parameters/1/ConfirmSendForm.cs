// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.ConfirmSendForm
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

internal class ConfirmSendForm : Form, IProvideHtml
{
  private bool listContainsPartNumbers;
  private IContainer components;
  private ListViewEx list;
  private ImageList stateImages;
  private ColumnHeader columnHeaderName;
  private ColumnHeader columnHeaderValue;
  private ColumnHeader columnHeaderOriginalValue;
  private ColumnHeader columnHeaderUnit;
  private Button buttonCancel;
  private Button buttonOk;
  private System.Windows.Forms.Label label1;
  private System.Windows.Forms.Label totalChangesLabel;
  private Button buttonPrint;
  private ColumnHeader columnHeaderPart;
  private ColumnHeader columnHeaderOriginalPart;

  protected override void OnClosing(CancelEventArgs e)
  {
    if (this.DialogResult == DialogResult.OK)
      e.Cancel = !this.NoWarningsOrUserAcknowledgedWarnings();
    base.OnClosing(e);
  }

  private bool NoWarningsOrUserAcknowledgedWarnings()
  {
    bool flag = true;
    foreach (ListViewExGroupItem listViewExGroupItem in ((ListView) this.list).Items)
    {
      if (flag && listViewExGroupItem.Level == 0 && listViewExGroupItem.HasChildren)
      {
        foreach (ListViewExGroupItem child1 in (IEnumerable) listViewExGroupItem.Children)
        {
          if (flag && child1.HasChildren)
          {
            foreach (ListViewExGroupItem child2 in (IEnumerable) child1.Children)
            {
              if (flag && !string.IsNullOrEmpty(((ListViewItem) child2).Tag as string))
              {
                flag = this.UserAcknowlegedWarning(((ListViewItem) child2).Text, ((ListViewItem) child2).SubItems[2].Text, ((ListViewItem) child2).Tag as string);
                if (!flag)
                  break;
              }
            }
          }
        }
      }
    }
    return flag;
  }

  private bool UserAcknowlegedWarning(string parameter, string value, string message)
  {
    DialogResult dialogResult = MessageBox.Show((IWin32Window) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.UserWarningIntro, (object) parameter, (object) value, (object) message), Resources.UserWarningTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, ControlHelpers.IsRightToLeft((Control) this) ? MessageBoxOptions.RtlReading : (MessageBoxOptions) 0);
    if (dialogResult == DialogResult.Yes)
    {
      string text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.UserWarningLogLabel, (object) parameter, (object) value);
      StatusLog.Add(new StatusMessage(text, (StatusMessageType) 2, (object) this));
      SapiManager.GlobalInstance.Sapi.LogFiles.LabelLog(text);
    }
    return dialogResult == DialogResult.Yes;
  }

  internal ConfirmSendForm()
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.stateImages.Images.Add(ConfirmSendForm.ImageIndex.Group.ToString(), (Image) Resources.group_open);
  }

  private void buttonPrint_Click(object sender, EventArgs e)
  {
    if (!this.CanPrint)
      return;
    PrintHelper.ShowPrintDialog(Resources.ConfirmParameterSendPrint, (IProvideHtml) this, (IncludeInfo) 3);
  }

  private bool CanPrint => !PrintHelper.Busy && this.CanProvideHtml;

  public bool CanProvideHtml => true;

  public string ToHtml()
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.CanProvideHtml)
    {
      XmlWriter writer = PrintHelper.CreateWriter(stringBuilder);
      foreach (ListViewExGroupItem listViewExGroupItem in ((ListView) this.list).Items)
      {
        if (listViewExGroupItem.Level == 0)
        {
          writer.WriteStartElement("div");
          writer.WriteStartAttribute("id");
          writer.WriteString(((ListViewItem) listViewExGroupItem).Text);
          writer.WriteEndAttribute();
          writer.WriteStartAttribute("name");
          writer.WriteString(((ListViewItem) listViewExGroupItem).Text);
          writer.WriteEndAttribute();
          writer.WriteStartElement("table");
          writer.WriteStartElement("tr");
          writer.WriteStartElement("td");
          writer.WriteStartAttribute("class");
          writer.WriteString("ecu");
          writer.WriteEndAttribute();
          writer.WriteString(((ListViewItem) listViewExGroupItem).Text);
          writer.WriteFullEndElement();
          writer.WriteStartElement("td");
          writer.WriteStartAttribute("valign");
          writer.WriteString("bottom");
          writer.WriteEndAttribute();
          writer.WriteStartAttribute("class");
          writer.WriteString("standard");
          writer.WriteEndAttribute();
          writer.WriteFullEndElement();
          writer.WriteFullEndElement();
          writer.WriteFullEndElement();
          writer.WriteStartElement("div");
          writer.WriteStartAttribute("class");
          writer.WriteString(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "gradientline{0}", (object) (((ListViewItem) listViewExGroupItem).Index % 3)));
          writer.WriteEndAttribute();
          writer.WriteFullEndElement();
          writer.WriteStartElement("table");
          bool ecuContainsPartNumbers = false;
          if (((ListViewItem) listViewExGroupItem).Tag != null)
            ecuContainsPartNumbers = (bool) ((ListViewItem) listViewExGroupItem).Tag;
          this.AddColumnHeaderToHtml(writer, ecuContainsPartNumbers);
          this.AddChildrenToHtml(listViewExGroupItem, writer, ecuContainsPartNumbers);
          writer.WriteFullEndElement();
          writer.WriteFullEndElement();
        }
      }
      writer.Close();
    }
    return stringBuilder.ToString();
  }

  private void AddColumnHeaderToHtml(XmlWriter xmlWriter, bool ecuContainsPartNumbers)
  {
    xmlWriter.WriteStartElement("tr");
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteRaw("&nbsp;");
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteRaw("&nbsp;");
    xmlWriter.WriteFullEndElement();
    if (ecuContainsPartNumbers)
    {
      xmlWriter.WriteStartElement("td");
      xmlWriter.WriteString(this.columnHeaderPart.Text);
      xmlWriter.WriteEndElement();
    }
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteString(this.columnHeaderValue.Text);
    xmlWriter.WriteEndElement();
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteRaw("&nbsp;");
    xmlWriter.WriteFullEndElement();
    if (ecuContainsPartNumbers)
    {
      xmlWriter.WriteStartElement("td");
      xmlWriter.WriteString(this.columnHeaderOriginalPart.Text);
      xmlWriter.WriteEndElement();
    }
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteString(this.columnHeaderOriginalValue.Text);
    xmlWriter.WriteEndElement();
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteRaw("&nbsp;");
    xmlWriter.WriteFullEndElement();
    xmlWriter.WriteStartElement("td");
    xmlWriter.WriteString(this.columnHeaderUnit.Text);
    xmlWriter.WriteEndElement();
    xmlWriter.WriteEndElement();
  }

  private void AddChildrenToHtml(
    ListViewExGroupItem item,
    XmlWriter xmlWriter,
    bool ecuContainsPartNumbers)
  {
    foreach (ListViewExGroupItem child in (IEnumerable) item.Children)
    {
      if (child.HasChildren)
      {
        xmlWriter.WriteStartElement("tr");
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteStartAttribute("class");
        xmlWriter.WriteString("group");
        xmlWriter.WriteEndAttribute();
        xmlWriter.WriteString(((ListViewItem) child).Text);
        xmlWriter.WriteFullEndElement();
        if (ecuContainsPartNumbers)
        {
          xmlWriter.WriteStartElement("td");
          xmlWriter.WriteRaw("&nbsp;");
          xmlWriter.WriteFullEndElement();
          if (((ListViewItem) child).SubItems.Count > this.columnHeaderPart.Index)
          {
            xmlWriter.WriteStartElement("td");
            xmlWriter.WriteString(((ListViewItem) child).SubItems[this.columnHeaderPart.Index].Text);
            xmlWriter.WriteFullEndElement();
          }
          xmlWriter.WriteStartElement("td");
          xmlWriter.WriteRaw("&nbsp;");
          xmlWriter.WriteFullEndElement();
          xmlWriter.WriteStartElement("td");
          xmlWriter.WriteRaw("&nbsp;");
          xmlWriter.WriteFullEndElement();
          if (((ListViewItem) child).SubItems.Count > this.columnHeaderOriginalPart.Index)
          {
            xmlWriter.WriteStartElement("td");
            xmlWriter.WriteString(((ListViewItem) child).SubItems[this.columnHeaderOriginalPart.Index].Text);
            xmlWriter.WriteFullEndElement();
          }
        }
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteStartAttribute("class");
        xmlWriter.WriteString("group");
        xmlWriter.WriteEndAttribute();
        xmlWriter.WriteFullEndElement();
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteStartAttribute("class");
        xmlWriter.WriteString("group");
        xmlWriter.WriteEndAttribute();
        xmlWriter.WriteFullEndElement();
        this.AddChildrenToHtml(child, xmlWriter, ecuContainsPartNumbers);
      }
      else
      {
        xmlWriter.WriteStartElement("tr");
        xmlWriter.WriteAttributeString("class", "standard");
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteString(((ListViewItem) child).Text);
        xmlWriter.WriteFullEndElement();
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteRaw("&nbsp;");
        xmlWriter.WriteFullEndElement();
        if (ecuContainsPartNumbers)
        {
          xmlWriter.WriteStartElement("td");
          xmlWriter.WriteString(((ListViewItem) child).SubItems[this.columnHeaderPart.Index].Text);
          xmlWriter.WriteFullEndElement();
        }
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteString(((ListViewItem) child).SubItems[this.columnHeaderValue.Index].Text);
        xmlWriter.WriteFullEndElement();
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteRaw("&nbsp;");
        xmlWriter.WriteFullEndElement();
        if (ecuContainsPartNumbers)
        {
          xmlWriter.WriteStartElement("td");
          xmlWriter.WriteString(((ListViewItem) child).SubItems[this.columnHeaderOriginalPart.Index].Text);
          xmlWriter.WriteFullEndElement();
        }
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteString(((ListViewItem) child).SubItems[this.columnHeaderOriginalValue.Index].Text);
        xmlWriter.WriteFullEndElement();
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteRaw("&nbsp;");
        xmlWriter.WriteFullEndElement();
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteString(((ListViewItem) child).SubItems[this.columnHeaderUnit.Index].Text);
        xmlWriter.WriteFullEndElement();
        xmlWriter.WriteFullEndElement();
      }
    }
  }

  public string StyleSheet => string.Empty;

  protected override void OnLoad(EventArgs e)
  {
    int num = 0;
    this.list.LockSorting();
    this.list.BeginUpdate();
    foreach (Channel activeChannel in SapiManager.GlobalInstance.ActiveChannels)
    {
      if (activeChannel.Online)
      {
        ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(activeChannel.Ecu.Name);
        ((ListViewItem) listViewExGroupItem).StateImageIndex = 0;
        ((ListViewItem) listViewExGroupItem).Font = new Font(((ListViewItem) listViewExGroupItem).Font.FontFamily, ((ListViewItem) listViewExGroupItem).Font.Size, FontStyle.Bold);
        ((ListViewItem) listViewExGroupItem).BackColor = SystemColors.Control;
        ((ListView) this.list).Items.Add((ListViewItem) listViewExGroupItem);
        bool flag1 = false;
        GroupCollection groupCollection = new GroupCollection();
        foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) activeChannel.Parameters)
        {
          if (!object.Equals(parameter.Value, parameter.OriginalValue))
            groupCollection.Add(parameter.GroupName, (object) parameter);
        }
        bool flag2 = false;
        foreach (Group group in groupCollection)
        {
          ListViewExGroupItem owner1 = new ListViewExGroupItem(group.Name);
          ((ListViewItem) owner1).StateImageIndex = 0;
          listViewExGroupItem.Add(owner1);
          string empty1 = string.Empty;
          string empty2 = string.Empty;
          string text1 = string.Empty;
          string text2 = string.Empty;
          Parameter parameter1 = group.Items.OfType<Parameter>().First<Parameter>();
          ParameterGroupDataItem parameterGroupDataItem = new ParameterGroupDataItem(parameter1, new Qualifier((QualifierTypes) 128 /*0x80*/, activeChannel.Ecu.Name, parameter1.GroupQualifier), ServerDataManager.GlobalInstance.GetFactoryAggregatePart(activeChannel, parameter1.GroupQualifier), ServerDataManager.GlobalInstance.GetEngineeringCorrectionFactorParameters(activeChannel, parameter1.GroupQualifier));
          if (parameterGroupDataItem.HasPartNumbers)
          {
            flag2 = this.listContainsPartNumbers = true;
            CodingChoice valueAsCodingChoice1 = parameterGroupDataItem.OriginalValueAsCodingChoice;
            if (valueAsCodingChoice1 != null)
            {
              empty1 = valueAsCodingChoice1.Part.ToString();
              text1 = valueAsCodingChoice1.Meaning;
            }
            CodingChoice valueAsCodingChoice2 = parameterGroupDataItem.ValueAsCodingChoice;
            if (valueAsCodingChoice2 != null)
            {
              empty2 = valueAsCodingChoice2.Part.ToString();
              text2 = valueAsCodingChoice2.Meaning;
            }
          }
          foreach (Parameter parameter2 in group.Items)
          {
            ParameterDataItem parameterDataItem = new ParameterDataItem(parameter2, new Qualifier((QualifierTypes) 4, activeChannel.Ecu.Name, parameter2.Qualifier), parameterGroupDataItem);
            string text3 = parameter2.OriginalValue != null ? parameter2.OriginalValue.ToString() : string.Empty;
            string str = string.Empty;
            string text4 = string.Empty;
            Conversion conversion = Converter.GlobalInstance.GetConversion(parameter2.Units);
            if (parameter2.Value != null)
            {
              Choice choice = parameter2.Value as Choice;
              if (choice != (object) null)
              {
                if (choice.RawValue != null)
                {
                  str = choice.RawValue.ToString();
                  if (choice.Name != null)
                    text4 = choice.Name.ToString();
                }
              }
              else if (parameter2.Value is Dump)
              {
                str = parameter2.Value.ToString();
                text4 = str;
              }
              else
              {
                int int32 = Convert.ToInt32(parameter2.Precision, (IFormatProvider) CultureInfo.InvariantCulture);
                if (conversion != null)
                  int32 += conversion.PrecisionAdjustment;
                string formatString = Converter.GetFormatString(int32);
                if (parameter2.OriginalValue != null)
                  text3 = conversion == null ? string.Format((IFormatProvider) CultureInfo.GetCultureInfo("en-US"), formatString, parameter2.OriginalValue) : string.Format((IFormatProvider) CultureInfo.GetCultureInfo("en-US"), formatString, (object) conversion.Convert(parameter2.OriginalValue));
                str = conversion == null ? string.Format((IFormatProvider) CultureInfo.GetCultureInfo("en-US"), formatString, parameter2.Value) : string.Format((IFormatProvider) CultureInfo.GetCultureInfo("en-US"), formatString, (object) conversion.Convert(parameter2.Value));
                text4 = str;
              }
            }
            ListViewExGroupItem owner2 = new ListViewExGroupItem(parameter2.Name);
            ((ListViewItem) owner2).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem) owner2, parameterDataItem.ValuePartDisplayText));
            ((ListViewItem) owner2).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem) owner2, text4));
            ((ListViewItem) owner2).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem) owner2, parameterDataItem.OriginalValuePartDisplayText));
            ((ListViewItem) owner2).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem) owner2, text3));
            ((ListViewItem) owner2).StateImageIndex = -1;
            if (ParameterWarning.GlobalInstance.HasWarning(parameter2.Channel.Ecu.Name, parameter2.Qualifier, str))
              ((ListViewItem) owner2).Tag = (object) ParameterWarning.GlobalInstance.Warning(parameter2.Channel.Ecu.Name, parameter2.Qualifier, str);
            ((ListViewItem) owner2).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem) owner2, Converter.Translate(conversion != null ? conversion.OutputUnit : parameter2.Units)));
            owner1.Add(owner2);
            flag1 = true;
            ++num;
          }
          if (!string.IsNullOrEmpty(empty1) || !string.IsNullOrEmpty(empty2))
          {
            ((ListViewItem) owner1).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem) owner1, empty2));
            ((ListViewItem) owner1).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem) owner1, text2));
            ((ListViewItem) owner1).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem) owner1, empty1));
            ((ListViewItem) owner1).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem) owner1, text1));
          }
        }
        ((ListViewItem) listViewExGroupItem).Tag = (object) flag2;
        if (!flag1)
          ((ListView) this.list).Items.Remove((ListViewItem) listViewExGroupItem);
      }
    }
    this.UpdateColumns();
    this.totalChangesLabel.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CompareSendForm_Format_TotalChanges, (object) num);
    this.list.EndUpdate();
    this.list.UnlockSorting();
    base.OnLoad(e);
  }

  private void UpdateColumns()
  {
    Size size = ((Control) this.list).ClientSize;
    int num1 = size.Width - SystemInformation.VerticalScrollBarWidth;
    size = SystemInformation.BorderSize;
    int num2 = size.Width * 2;
    int num3 = num1 - num2;
    if (this.listContainsPartNumbers)
    {
      this.columnHeaderName.Width = num3 / 7 * 2;
      this.columnHeaderPart.Width = this.columnHeaderOriginalPart.Width = num3 / 7;
      this.columnHeaderValue.Width = this.columnHeaderOriginalValue.Width = num3 / 7;
      this.columnHeaderUnit.Width = num3 / 7;
    }
    else
    {
      this.columnHeaderName.Width = num3 / 5 * 2;
      this.columnHeaderPart.Width = this.columnHeaderOriginalPart.Width = 0;
      this.columnHeaderValue.Width = this.columnHeaderOriginalValue.Width = num3 / 5;
      this.columnHeaderUnit.Width = num3 / 5;
    }
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ConfirmSendForm));
    this.list = new ListViewEx();
    this.columnHeaderName = new ColumnHeader();
    this.columnHeaderPart = new ColumnHeader();
    this.columnHeaderValue = new ColumnHeader();
    this.columnHeaderOriginalPart = new ColumnHeader();
    this.columnHeaderOriginalValue = new ColumnHeader();
    this.columnHeaderUnit = new ColumnHeader();
    this.stateImages = new ImageList(this.components);
    this.buttonCancel = new Button();
    this.buttonOk = new Button();
    this.label1 = new System.Windows.Forms.Label();
    this.totalChangesLabel = new System.Windows.Forms.Label();
    this.buttonPrint = new Button();
    ((ISupportInitialize) this.list).BeginInit();
    this.SuspendLayout();
    componentResourceManager.ApplyResources((object) this.list, "list");
    this.list.CanDelete = false;
    ((ListView) this.list).Columns.AddRange(new ColumnHeader[6]
    {
      this.columnHeaderName,
      this.columnHeaderPart,
      this.columnHeaderValue,
      this.columnHeaderOriginalPart,
      this.columnHeaderOriginalValue,
      this.columnHeaderUnit
    });
    this.list.EditableColumn = -1;
    this.list.GridLines = true;
    ((Control) this.list).Name = "list";
    this.list.ShowGlyphs = (GlyphBehavior) 2;
    this.list.ShowStateImages = (ImageBehavior) 2;
    ((ListView) this.list).StateImageList = this.stateImages;
    ((ListView) this.list).UseCompatibleStateImageBehavior = false;
    componentResourceManager.ApplyResources((object) this.columnHeaderName, "columnHeaderName");
    componentResourceManager.ApplyResources((object) this.columnHeaderPart, "columnHeaderPart");
    componentResourceManager.ApplyResources((object) this.columnHeaderValue, "columnHeaderValue");
    componentResourceManager.ApplyResources((object) this.columnHeaderOriginalPart, "columnHeaderOriginalPart");
    componentResourceManager.ApplyResources((object) this.columnHeaderOriginalValue, "columnHeaderOriginalValue");
    componentResourceManager.ApplyResources((object) this.columnHeaderUnit, "columnHeaderUnit");
    this.stateImages.ColorDepth = ColorDepth.Depth32Bit;
    componentResourceManager.ApplyResources((object) this.stateImages, "stateImages");
    this.stateImages.TransparentColor = Color.Transparent;
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.buttonCancel.DialogResult = DialogResult.Cancel;
    this.buttonCancel.Name = "buttonCancel";
    componentResourceManager.ApplyResources((object) this.buttonOk, "buttonOk");
    this.buttonOk.DialogResult = DialogResult.OK;
    this.buttonOk.Name = "buttonOk";
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    componentResourceManager.ApplyResources((object) this.totalChangesLabel, "totalChangesLabel");
    this.totalChangesLabel.Name = "totalChangesLabel";
    componentResourceManager.ApplyResources((object) this.buttonPrint, "buttonPrint");
    this.buttonPrint.Name = "buttonPrint";
    this.buttonPrint.UseVisualStyleBackColor = true;
    this.buttonPrint.Click += new EventHandler(this.buttonPrint_Click);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.buttonPrint);
    this.Controls.Add((Control) this.totalChangesLabel);
    this.Controls.Add((Control) this.label1);
    this.Controls.Add((Control) this.buttonOk);
    this.Controls.Add((Control) this.buttonCancel);
    this.Controls.Add((Control) this.list);
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (ConfirmSendForm);
    this.ShowInTaskbar = false;
    ((ISupportInitialize) this.list).EndInit();
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  internal enum ImageIndex
  {
    NoImage = -1, // 0xFFFFFFFF
    Group = 0,
  }
}
