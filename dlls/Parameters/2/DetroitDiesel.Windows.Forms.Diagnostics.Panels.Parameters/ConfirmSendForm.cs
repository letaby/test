using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

internal class ConfirmSendForm : Form, IProvideHtml
{
	internal enum ImageIndex
	{
		NoImage = -1,
		Group
	}

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

	private bool CanPrint
	{
		get
		{
			if (!PrintHelper.Busy)
			{
				return CanProvideHtml;
			}
			return false;
		}
	}

	public bool CanProvideHtml => true;

	public string StyleSheet => string.Empty;

	protected override void OnClosing(CancelEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			e.Cancel = !NoWarningsOrUserAcknowledgedWarnings();
		}
		base.OnClosing(e);
	}

	private bool NoWarningsOrUserAcknowledgedWarnings()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		bool flag = true;
		foreach (ListViewExGroupItem item in ((ListView)(object)list).Items)
		{
			ListViewExGroupItem val = item;
			if (!flag || val.Level != 0 || !val.HasChildren)
			{
				continue;
			}
			foreach (ListViewExGroupItem child in val.Children)
			{
				ListViewExGroupItem val2 = child;
				if (!flag || !val2.HasChildren)
				{
					continue;
				}
				foreach (ListViewExGroupItem child2 in val2.Children)
				{
					ListViewExGroupItem val3 = child2;
					if (flag && !string.IsNullOrEmpty(((ListViewItem)(object)val3).Tag as string))
					{
						flag = UserAcknowlegedWarning(((ListViewItem)(object)val3).Text, ((ListViewItem)(object)val3).SubItems[2].Text, ((ListViewItem)(object)val3).Tag as string);
						if (!flag)
						{
							break;
						}
					}
				}
			}
		}
		return flag;
	}

	private bool UserAcknowlegedWarning(string parameter, string value, string message)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		string text = string.Format(CultureInfo.InvariantCulture, Resources.UserWarningIntro, parameter, value, message);
		DialogResult dialogResult = MessageBox.Show(this, text, Resources.UserWarningTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, ControlHelpers.IsRightToLeft((Control)this) ? MessageBoxOptions.RtlReading : ((MessageBoxOptions)0));
		if (dialogResult == DialogResult.Yes)
		{
			string text2 = string.Format(CultureInfo.InvariantCulture, Resources.UserWarningLogLabel, parameter, value);
			StatusLog.Add(new StatusMessage(text2, (StatusMessageType)2, (object)this));
			SapiManager.GlobalInstance.Sapi.LogFiles.LabelLog(text2);
		}
		return dialogResult == DialogResult.Yes;
	}

	internal ConfirmSendForm()
	{
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		stateImages.Images.Add(ImageIndex.Group.ToString(), Resources.group_open);
	}

	private void buttonPrint_Click(object sender, EventArgs e)
	{
		if (CanPrint)
		{
			PrintHelper.ShowPrintDialog(Resources.ConfirmParameterSendPrint, (IProvideHtml)(object)this, (IncludeInfo)3);
		}
	}

	public string ToHtml()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		StringBuilder stringBuilder = new StringBuilder();
		if (CanProvideHtml)
		{
			XmlWriter xmlWriter = PrintHelper.CreateWriter(stringBuilder);
			foreach (ListViewExGroupItem item in ((ListView)(object)list).Items)
			{
				ListViewExGroupItem val = item;
				if (val.Level == 0)
				{
					xmlWriter.WriteStartElement("div");
					xmlWriter.WriteStartAttribute("id");
					xmlWriter.WriteString(((ListViewItem)(object)val).Text);
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteStartAttribute("name");
					xmlWriter.WriteString(((ListViewItem)(object)val).Text);
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteStartElement("table");
					xmlWriter.WriteStartElement("tr");
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteStartAttribute("class");
					xmlWriter.WriteString("ecu");
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteString(((ListViewItem)(object)val).Text);
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteStartAttribute("valign");
					xmlWriter.WriteString("bottom");
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteStartAttribute("class");
					xmlWriter.WriteString("standard");
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteStartElement("div");
					xmlWriter.WriteStartAttribute("class");
					xmlWriter.WriteString(string.Format(CultureInfo.InvariantCulture, "gradientline{0}", ((ListViewItem)(object)val).Index % 3));
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteStartElement("table");
					bool ecuContainsPartNumbers = false;
					if (((ListViewItem)(object)val).Tag != null)
					{
						ecuContainsPartNumbers = (bool)((ListViewItem)(object)val).Tag;
					}
					AddColumnHeaderToHtml(xmlWriter, ecuContainsPartNumbers);
					AddChildrenToHtml(val, xmlWriter, ecuContainsPartNumbers);
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteFullEndElement();
				}
			}
			xmlWriter.Close();
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
			xmlWriter.WriteString(columnHeaderPart.Text);
			xmlWriter.WriteEndElement();
		}
		xmlWriter.WriteStartElement("td");
		xmlWriter.WriteString(columnHeaderValue.Text);
		xmlWriter.WriteEndElement();
		xmlWriter.WriteStartElement("td");
		xmlWriter.WriteRaw("&nbsp;");
		xmlWriter.WriteFullEndElement();
		if (ecuContainsPartNumbers)
		{
			xmlWriter.WriteStartElement("td");
			xmlWriter.WriteString(columnHeaderOriginalPart.Text);
			xmlWriter.WriteEndElement();
		}
		xmlWriter.WriteStartElement("td");
		xmlWriter.WriteString(columnHeaderOriginalValue.Text);
		xmlWriter.WriteEndElement();
		xmlWriter.WriteStartElement("td");
		xmlWriter.WriteRaw("&nbsp;");
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteStartElement("td");
		xmlWriter.WriteString(columnHeaderUnit.Text);
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndElement();
	}

	private void AddChildrenToHtml(ListViewExGroupItem item, XmlWriter xmlWriter, bool ecuContainsPartNumbers)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		foreach (ListViewExGroupItem child in item.Children)
		{
			ListViewExGroupItem val = child;
			if (val.HasChildren)
			{
				xmlWriter.WriteStartElement("tr");
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteStartAttribute("class");
				xmlWriter.WriteString("group");
				xmlWriter.WriteEndAttribute();
				xmlWriter.WriteString(((ListViewItem)(object)val).Text);
				xmlWriter.WriteFullEndElement();
				if (ecuContainsPartNumbers)
				{
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteRaw("&nbsp;");
					xmlWriter.WriteFullEndElement();
					if (((ListViewItem)(object)val).SubItems.Count > columnHeaderPart.Index)
					{
						xmlWriter.WriteStartElement("td");
						xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[columnHeaderPart.Index].Text);
						xmlWriter.WriteFullEndElement();
					}
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteRaw("&nbsp;");
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteRaw("&nbsp;");
					xmlWriter.WriteFullEndElement();
					if (((ListViewItem)(object)val).SubItems.Count > columnHeaderOriginalPart.Index)
					{
						xmlWriter.WriteStartElement("td");
						xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[columnHeaderOriginalPart.Index].Text);
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
				AddChildrenToHtml(val, xmlWriter, ecuContainsPartNumbers);
			}
			else
			{
				xmlWriter.WriteStartElement("tr");
				xmlWriter.WriteAttributeString("class", "standard");
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteString(((ListViewItem)(object)val).Text);
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteRaw("&nbsp;");
				xmlWriter.WriteFullEndElement();
				if (ecuContainsPartNumbers)
				{
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[columnHeaderPart.Index].Text);
					xmlWriter.WriteFullEndElement();
				}
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[columnHeaderValue.Index].Text);
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteRaw("&nbsp;");
				xmlWriter.WriteFullEndElement();
				if (ecuContainsPartNumbers)
				{
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[columnHeaderOriginalPart.Index].Text);
					xmlWriter.WriteFullEndElement();
				}
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[columnHeaderOriginalValue.Index].Text);
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteRaw("&nbsp;");
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[columnHeaderUnit.Index].Text);
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
			}
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Expected O, but got Unknown
		int num = 0;
		list.LockSorting();
		list.BeginUpdate();
		foreach (Channel activeChannel in SapiManager.GlobalInstance.ActiveChannels)
		{
			if (!activeChannel.Online)
			{
				continue;
			}
			ListViewExGroupItem val = new ListViewExGroupItem(activeChannel.Ecu.Name);
			((ListViewItem)(object)val).StateImageIndex = 0;
			((ListViewItem)(object)val).Font = new Font(((ListViewItem)(object)val).Font.FontFamily, ((ListViewItem)(object)val).Font.Size, FontStyle.Bold);
			((ListViewItem)(object)val).BackColor = SystemColors.Control;
			((ListView)(object)list).Items.Add((ListViewItem)(object)val);
			bool flag = false;
			GroupCollection val2 = new GroupCollection();
			foreach (Parameter parameter3 in activeChannel.Parameters)
			{
				if (!object.Equals(parameter3.Value, parameter3.OriginalValue))
				{
					val2.Add(parameter3.GroupName, (object)parameter3);
				}
			}
			bool flag2 = false;
			foreach (Group item in val2)
			{
				ListViewExGroupItem val3 = new ListViewExGroupItem(item.Name);
				((ListViewItem)(object)val3).StateImageIndex = 0;
				val.Add(val3);
				string value = string.Empty;
				string value2 = string.Empty;
				string text = string.Empty;
				string text2 = string.Empty;
				Parameter parameter = item.Items.OfType<Parameter>().First();
				ParameterGroupDataItem val4 = new ParameterGroupDataItem(parameter, new Qualifier((QualifierTypes)128, activeChannel.Ecu.Name, parameter.GroupQualifier), ServerDataManager.GlobalInstance.GetFactoryAggregatePart(activeChannel, parameter.GroupQualifier), ServerDataManager.GlobalInstance.GetEngineeringCorrectionFactorParameters(activeChannel, parameter.GroupQualifier));
				if (val4.HasPartNumbers)
				{
					flag2 = (listContainsPartNumbers = true);
					CodingChoice originalValueAsCodingChoice = val4.OriginalValueAsCodingChoice;
					if (originalValueAsCodingChoice != null)
					{
						value = originalValueAsCodingChoice.Part.ToString();
						text = originalValueAsCodingChoice.Meaning;
					}
					CodingChoice valueAsCodingChoice = val4.ValueAsCodingChoice;
					if (valueAsCodingChoice != null)
					{
						value2 = valueAsCodingChoice.Part.ToString();
						text2 = valueAsCodingChoice.Meaning;
					}
				}
				foreach (Parameter item2 in item.Items)
				{
					ParameterDataItem val5 = new ParameterDataItem(item2, new Qualifier((QualifierTypes)4, activeChannel.Ecu.Name, item2.Qualifier), val4);
					string text3 = ((item2.OriginalValue != null) ? item2.OriginalValue.ToString() : string.Empty);
					string text4 = string.Empty;
					string text5 = string.Empty;
					Conversion conversion = Converter.GlobalInstance.GetConversion(item2.Units);
					if (item2.Value != null)
					{
						Choice choice = item2.Value as Choice;
						if (choice != null)
						{
							if (choice.RawValue != null)
							{
								text4 = choice.RawValue.ToString();
								if (choice.Name != null)
								{
									text5 = choice.Name.ToString();
								}
							}
						}
						else if (item2.Value is Dump)
						{
							text4 = item2.Value.ToString();
							text5 = text4;
						}
						else
						{
							int num2 = Convert.ToInt32(item2.Precision, CultureInfo.InvariantCulture);
							if (conversion != null)
							{
								num2 += conversion.PrecisionAdjustment;
							}
							string formatString = Converter.GetFormatString(num2);
							if (item2.OriginalValue != null)
							{
								text3 = ((conversion == null) ? string.Format(CultureInfo.GetCultureInfo("en-US"), formatString, item2.OriginalValue) : string.Format(CultureInfo.GetCultureInfo("en-US"), formatString, conversion.Convert(item2.OriginalValue)));
							}
							text4 = ((conversion == null) ? string.Format(CultureInfo.GetCultureInfo("en-US"), formatString, item2.Value) : string.Format(CultureInfo.GetCultureInfo("en-US"), formatString, conversion.Convert(item2.Value)));
							text5 = text4;
						}
					}
					ListViewExGroupItem val6 = new ListViewExGroupItem(item2.Name);
					((ListViewItem)(object)val6).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem)(object)val6, val5.ValuePartDisplayText));
					((ListViewItem)(object)val6).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem)(object)val6, text5));
					((ListViewItem)(object)val6).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem)(object)val6, val5.OriginalValuePartDisplayText));
					((ListViewItem)(object)val6).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem)(object)val6, text3));
					((ListViewItem)(object)val6).StateImageIndex = -1;
					if (ParameterWarning.GlobalInstance.HasWarning(item2.Channel.Ecu.Name, item2.Qualifier, text4))
					{
						((ListViewItem)(object)val6).Tag = ParameterWarning.GlobalInstance.Warning(item2.Channel.Ecu.Name, item2.Qualifier, text4);
					}
					((ListViewItem)(object)val6).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem)(object)val6, Converter.Translate((conversion != null) ? conversion.OutputUnit : item2.Units)));
					val3.Add(val6);
					flag = true;
					num++;
				}
				if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2))
				{
					((ListViewItem)(object)val3).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem)(object)val3, value2));
					((ListViewItem)(object)val3).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem)(object)val3, text2));
					((ListViewItem)(object)val3).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem)(object)val3, value));
					((ListViewItem)(object)val3).SubItems.Add(new ListViewItem.ListViewSubItem((ListViewItem)(object)val3, text));
				}
			}
			((ListViewItem)(object)val).Tag = flag2;
			if (!flag)
			{
				((ListView)(object)list).Items.Remove((ListViewItem)(object)val);
			}
		}
		UpdateColumns();
		totalChangesLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.CompareSendForm_Format_TotalChanges, num);
		list.EndUpdate();
		list.UnlockSorting();
		base.OnLoad(e);
	}

	private void UpdateColumns()
	{
		int num = ((Control)(object)list).ClientSize.Width - SystemInformation.VerticalScrollBarWidth - SystemInformation.BorderSize.Width * 2;
		if (listContainsPartNumbers)
		{
			columnHeaderName.Width = num / 7 * 2;
			ColumnHeader columnHeader = columnHeaderPart;
			int num2 = (columnHeaderOriginalPart.Width = num / 7);
			columnHeader.Width = num2;
			ColumnHeader columnHeader2 = columnHeaderValue;
			num2 = (columnHeaderOriginalValue.Width = num / 7);
			columnHeader2.Width = num2;
			columnHeaderUnit.Width = num / 7;
		}
		else
		{
			columnHeaderName.Width = num / 5 * 2;
			ColumnHeader columnHeader3 = columnHeaderPart;
			int num2 = (columnHeaderOriginalPart.Width = 0);
			columnHeader3.Width = num2;
			ColumnHeader columnHeader4 = columnHeaderValue;
			num2 = (columnHeaderOriginalValue.Width = num / 5);
			columnHeader4.Width = num2;
			columnHeaderUnit.Width = num / 5;
		}
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.ConfirmSendForm));
		this.list = new ListViewEx();
		this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderPart = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderValue = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderOriginalPart = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderOriginalValue = new System.Windows.Forms.ColumnHeader();
		this.columnHeaderUnit = new System.Windows.Forms.ColumnHeader();
		this.stateImages = new System.Windows.Forms.ImageList(this.components);
		this.buttonCancel = new System.Windows.Forms.Button();
		this.buttonOk = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.totalChangesLabel = new System.Windows.Forms.Label();
		this.buttonPrint = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.list).BeginInit();
		base.SuspendLayout();
		resources.ApplyResources(this.list, "list");
		this.list.CanDelete = false;
		((System.Windows.Forms.ListView)(object)this.list).Columns.AddRange(new System.Windows.Forms.ColumnHeader[6] { this.columnHeaderName, this.columnHeaderPart, this.columnHeaderValue, this.columnHeaderOriginalPart, this.columnHeaderOriginalValue, this.columnHeaderUnit });
		this.list.EditableColumn = -1;
		this.list.GridLines = true;
		((System.Windows.Forms.Control)(object)this.list).Name = "list";
		this.list.ShowGlyphs = (GlyphBehavior)2;
		this.list.ShowStateImages = (ImageBehavior)2;
		((System.Windows.Forms.ListView)(object)this.list).StateImageList = this.stateImages;
		((System.Windows.Forms.ListView)(object)this.list).UseCompatibleStateImageBehavior = false;
		resources.ApplyResources(this.columnHeaderName, "columnHeaderName");
		resources.ApplyResources(this.columnHeaderPart, "columnHeaderPart");
		resources.ApplyResources(this.columnHeaderValue, "columnHeaderValue");
		resources.ApplyResources(this.columnHeaderOriginalPart, "columnHeaderOriginalPart");
		resources.ApplyResources(this.columnHeaderOriginalValue, "columnHeaderOriginalValue");
		resources.ApplyResources(this.columnHeaderUnit, "columnHeaderUnit");
		this.stateImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
		resources.ApplyResources(this.stateImages, "stateImages");
		this.stateImages.TransparentColor = System.Drawing.Color.Transparent;
		resources.ApplyResources(this.buttonCancel, "buttonCancel");
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Name = "buttonCancel";
		resources.ApplyResources(this.buttonOk, "buttonOk");
		this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOk.Name = "buttonOk";
		resources.ApplyResources(this.label1, "label1");
		this.label1.Name = "label1";
		resources.ApplyResources(this.totalChangesLabel, "totalChangesLabel");
		this.totalChangesLabel.Name = "totalChangesLabel";
		resources.ApplyResources(this.buttonPrint, "buttonPrint");
		this.buttonPrint.Name = "buttonPrint";
		this.buttonPrint.UseVisualStyleBackColor = true;
		this.buttonPrint.Click += new System.EventHandler(buttonPrint_Click);
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.buttonPrint);
		base.Controls.Add(this.totalChangesLabel);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.buttonOk);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add((System.Windows.Forms.Control)(object)this.list);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ConfirmSendForm";
		base.ShowInTaskbar = false;
		((System.ComponentModel.ISupportInitialize)this.list).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
