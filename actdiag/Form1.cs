using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Text;
using DateTimeTZ;
using ActDiag;



namespace WindowsApplication2
{
	/// <summary>
	/// Summary description for frmActEventViewer.
	/// </summary>
	public class frmActEventViewer : System.Windows.Forms.Form
	{
		private ActDiag.ActEventLogCache _actEventLogCache = new ActDiag.ActEventLogCache();
		private System.Diagnostics.EventLog eventLog1;
		private int _intUTCOffset;
		private System.Windows.Forms.ComboBox cboFilterByMsg;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtFilterByMsg;
		private System.Windows.Forms.ComboBox cboFilterByType;
		private System.Windows.Forms.ComboBox cboTimeZones;
		private System.Windows.Forms.TextBox txtEventMSG;
		private System.Windows.Forms.DateTimePicker dtpFrom;
		private System.Windows.Forms.DateTimePicker dtpTo;
		private System.Windows.Forms.CheckBox chkTail;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button cmdFilter;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ListView lvEvents;
		private System.Windows.Forms.ColumnHeader colType;
		private System.Windows.Forms.ColumnHeader colDate;
		private System.Windows.Forms.ColumnHeader colTime;
		private System.Windows.Forms.ComboBox cboMachine;
		private System.Windows.Forms.ComboBox cboFilterByVBErrSrc;
		private System.Windows.Forms.ComboBox cboFilterByVBErrDesc;
		private System.Windows.Forms.ComboBox cboFilterByVBErrNum;
		private System.Windows.Forms.ComboBox cboFilterEventSrc;
		private System.Windows.Forms.ColumnHeader col_vbErrNum;
		private System.Windows.Forms.ColumnHeader col_vbErrSrc;
		private System.Windows.Forms.ColumnHeader col_vbErrDesc;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtVbErrDesc;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ColumnHeader colSource;
		

		public frmActEventViewer()
		{
			InitializeComponent();
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
			this.eventLog1 = new System.Diagnostics.EventLog();
			this.cboFilterByMsg = new System.Windows.Forms.ComboBox();
			this.cboFilterByVBErrSrc = new System.Windows.Forms.ComboBox();
			this.cboFilterByVBErrNum = new System.Windows.Forms.ComboBox();
			this.cboFilterByVBErrDesc = new System.Windows.Forms.ComboBox();
			this.cboFilterEventSrc = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtFilterByMsg = new System.Windows.Forms.TextBox();
			this.cboFilterByType = new System.Windows.Forms.ComboBox();
			this.txtEventMSG = new System.Windows.Forms.TextBox();
			this.cboTimeZones = new System.Windows.Forms.ComboBox();
			this.dtpFrom = new System.Windows.Forms.DateTimePicker();
			this.dtpTo = new System.Windows.Forms.DateTimePicker();
			this.chkTail = new System.Windows.Forms.CheckBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.cmdFilter = new System.Windows.Forms.Button();
			this.lvEvents = new System.Windows.Forms.ListView();
			this.colType = new System.Windows.Forms.ColumnHeader();
			this.colDate = new System.Windows.Forms.ColumnHeader();
			this.colTime = new System.Windows.Forms.ColumnHeader();
			this.colSource = new System.Windows.Forms.ColumnHeader();
			this.col_vbErrNum = new System.Windows.Forms.ColumnHeader();
			this.col_vbErrSrc = new System.Windows.Forms.ColumnHeader();
			this.col_vbErrDesc = new System.Windows.Forms.ColumnHeader();
			this.cboMachine = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.txtVbErrDesc = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
			this.SuspendLayout();
			// 
			// eventLog1
			// 
			this.eventLog1.SynchronizingObject = this;
			// 
			// cboFilterByMsg
			// 
			this.cboFilterByMsg.Location = new System.Drawing.Point(168, 40);
			this.cboFilterByMsg.Name = "cboFilterByMsg";
			this.cboFilterByMsg.Size = new System.Drawing.Size(184, 21);
			this.cboFilterByMsg.TabIndex = 1;
			this.cboFilterByMsg.DropDown += new System.EventHandler(this.cboFilterByMsg_DropDown);
			this.cboFilterByMsg.SelectedIndexChanged += new System.EventHandler(this.cboFilterByMsg_SelectedIndexChanged);
			// 
			// cboFilterByVBErrSrc
			// 
			this.cboFilterByVBErrSrc.Location = new System.Drawing.Point(168, 152);
			this.cboFilterByVBErrSrc.Name = "cboFilterByVBErrSrc";
			this.cboFilterByVBErrSrc.Size = new System.Drawing.Size(184, 21);
			this.cboFilterByVBErrSrc.TabIndex = 2;
			this.cboFilterByVBErrSrc.DropDown += new System.EventHandler(this.cboFilterByVBErrSrc_DropDown);
			// 
			// cboFilterByVBErrNum
			// 
			this.cboFilterByVBErrNum.Location = new System.Drawing.Point(168, 120);
			this.cboFilterByVBErrNum.Name = "cboFilterByVBErrNum";
			this.cboFilterByVBErrNum.Size = new System.Drawing.Size(184, 21);
			this.cboFilterByVBErrNum.TabIndex = 3;
			this.cboFilterByVBErrNum.DropDown += new System.EventHandler(this.cboFilterByVBErrNum_DropDown);
			// 
			// cboFilterByVBErrDesc
			// 
			this.cboFilterByVBErrDesc.Location = new System.Drawing.Point(168, 176);
			this.cboFilterByVBErrDesc.Name = "cboFilterByVBErrDesc";
			this.cboFilterByVBErrDesc.Size = new System.Drawing.Size(184, 21);
			this.cboFilterByVBErrDesc.TabIndex = 4;
			this.cboFilterByVBErrDesc.DropDown += new System.EventHandler(this.cboFilterByVBErrDesc_DropDown);
			this.cboFilterByVBErrDesc.SelectedIndexChanged += new System.EventHandler(this.cboFilterByVBErrDesc_SelectedIndexChanged);
			// 
			// cboFilterEventSrc
			// 
			this.cboFilterEventSrc.Cursor = System.Windows.Forms.Cursors.Cross;
			this.cboFilterEventSrc.Location = new System.Drawing.Point(168, 8);
			this.cboFilterEventSrc.Name = "cboFilterEventSrc";
			this.cboFilterEventSrc.Size = new System.Drawing.Size(184, 21);
			this.cboFilterEventSrc.TabIndex = 5;
			this.cboFilterEventSrc.DropDown += new System.EventHandler(this.cboFilterEventSrc_DropDown);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 288);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 24);
			this.label1.TabIndex = 8;
			this.label1.Text = "From";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(168, 288);
			this.label2.Name = "label2";
			this.label2.TabIndex = 9;
			this.label2.Text = "To";
			// 
			// txtFilterByMsg
			// 
			this.txtFilterByMsg.Location = new System.Drawing.Point(168, 64);
			this.txtFilterByMsg.Multiline = true;
			this.txtFilterByMsg.Name = "txtFilterByMsg";
			this.txtFilterByMsg.Size = new System.Drawing.Size(184, 48);
			this.txtFilterByMsg.TabIndex = 10;
			this.txtFilterByMsg.Text = "";
			// 
			// cboFilterByType
			// 
			this.cboFilterByType.Items.AddRange(new object[] {
																 "",
																 "Error",
																 "Information",
																 "Warning",
																 "SuccessAudit",
																 "FailureAudit"});
			this.cboFilterByType.Location = new System.Drawing.Point(168, 256);
			this.cboFilterByType.Name = "cboFilterByType";
			this.cboFilterByType.Size = new System.Drawing.Size(184, 21);
			this.cboFilterByType.TabIndex = 11;
			this.cboFilterByType.Text = "Error";
			// 
			// txtEventMSG
			// 
			this.txtEventMSG.AcceptsReturn = true;
			this.txtEventMSG.AcceptsTab = true;
			this.txtEventMSG.Location = new System.Drawing.Point(384, 248);
			this.txtEventMSG.Multiline = true;
			this.txtEventMSG.Name = "txtEventMSG";
			this.txtEventMSG.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtEventMSG.Size = new System.Drawing.Size(600, 256);
			this.txtEventMSG.TabIndex = 12;
			this.txtEventMSG.Text = "";
			// 
			// cboTimeZones
			// 
			this.cboTimeZones.Items.AddRange(new object[] {
															  "Arizona",
															  "Japan"});
			this.cboTimeZones.Location = new System.Drawing.Point(24, 368);
			this.cboTimeZones.Name = "cboTimeZones";
			this.cboTimeZones.Size = new System.Drawing.Size(88, 21);
			this.cboTimeZones.TabIndex = 13;
			this.cboTimeZones.Text = "Arizona";
			this.cboTimeZones.SelectedValueChanged += new System.EventHandler(this.cboTimeZones_SelectedValueChanged);
			// 
			// dtpFrom
			// 
			this.dtpFrom.CustomFormat = ((string)(configurationAppSettings.GetValue("dtpFrom.CustomFormat", typeof(string))));
			this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpFrom.Location = new System.Drawing.Point(24, 320);
			this.dtpFrom.Name = "dtpFrom";
			this.dtpFrom.Size = new System.Drawing.Size(128, 20);
			this.dtpFrom.TabIndex = 14;
			this.dtpFrom.Value = new System.DateTime(2003, 11, 21, 15, 18, 22, 618);
			// 
			// dtpTo
			// 
			this.dtpTo.CustomFormat = "dd\'/\'MM\'/\'yy hh\':\'mm tt";
			this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpTo.Location = new System.Drawing.Point(168, 320);
			this.dtpTo.Name = "dtpTo";
			this.dtpTo.Size = new System.Drawing.Size(128, 20);
			this.dtpTo.TabIndex = 15;
			this.dtpTo.Value = new System.DateTime(2003, 11, 22, 15, 18, 22, 628);
			// 
			// chkTail
			// 
			this.chkTail.Location = new System.Drawing.Point(312, 320);
			this.chkTail.Name = "chkTail";
			this.chkTail.Size = new System.Drawing.Size(48, 24);
			this.chkTail.TabIndex = 16;
			this.chkTail.Text = "Tail";
			this.chkTail.CheckedChanged += new System.EventHandler(this.chkTail_CheckedChanged);
			// 
			// cmdFilter
			// 
			this.cmdFilter.Location = new System.Drawing.Point(160, 408);
			this.cmdFilter.Name = "cmdFilter";
			this.cmdFilter.Size = new System.Drawing.Size(64, 24);
			this.cmdFilter.TabIndex = 17;
			this.cmdFilter.Text = "&Filter";
			this.cmdFilter.Click += new System.EventHandler(this.cmdFilter_Click);
			// 
			// lvEvents
			// 
			this.lvEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.colType,
																					   this.colDate,
																					   this.colTime,
																					   this.colSource,
																					   this.col_vbErrNum,
																					   this.col_vbErrSrc,
																					   this.col_vbErrDesc});
			this.lvEvents.Location = new System.Drawing.Point(384, 8);
			this.lvEvents.MultiSelect = false;
			this.lvEvents.Name = "lvEvents";
			this.lvEvents.Size = new System.Drawing.Size(600, 224);
			this.lvEvents.TabIndex = 18;
			this.lvEvents.View = System.Windows.Forms.View.Details;
			this.lvEvents.SelectedIndexChanged += new System.EventHandler(this.lvEvents_SelectedIndexChanged);
			// 
			// colType
			// 
			this.colType.Text = "Type";
			this.colType.Width = 103;
			// 
			// colDate
			// 
			this.colDate.Text = "Date";
			// 
			// colTime
			// 
			this.colTime.Text = "Time";
			// 
			// colSource
			// 
			this.colSource.Text = "Source";
			// 
			// col_vbErrNum
			// 
			this.col_vbErrNum.Text = "Err.Number";
			// 
			// col_vbErrSrc
			// 
			this.col_vbErrSrc.Text = "Err.Souce";
			// 
			// col_vbErrDesc
			// 
			this.col_vbErrDesc.Text = "Err.Desc";
			// 
			// cboMachine
			// 
			this.cboMachine.Items.AddRange(new object[] {
															"TEW",
															"IBM_MAWEBD1",
															"IBM_MAWEBQ1",
															"IBM_MAWEBT1",
															"IBM_MAWEBP1"});
			this.cboMachine.Location = new System.Drawing.Point(16, 408);
			this.cboMachine.Name = "cboMachine";
			this.cboMachine.Size = new System.Drawing.Size(104, 21);
			this.cboMachine.TabIndex = 19;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(136, 16);
			this.label3.TabIndex = 20;
			this.label3.Text = "Filter by Event Source";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(136, 16);
			this.label4.TabIndex = 21;
			this.label4.Text = "Filter by this err.number";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(24, 152);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(136, 16);
			this.label5.TabIndex = 22;
			this.label5.Text = "Filter by this err.source";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(24, 176);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(136, 16);
			this.label6.TabIndex = 23;
			this.label6.Text = "Filter by this err.desc";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(24, 256);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(136, 16);
			this.label7.TabIndex = 24;
			this.label7.Text = "Filter by Type";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(24, 40);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(136, 16);
			this.label8.TabIndex = 25;
			this.label8.Text = "Filter by this MSG";
			// 
			// txtVbErrDesc
			// 
			this.txtVbErrDesc.Location = new System.Drawing.Point(168, 200);
			this.txtVbErrDesc.Multiline = true;
			this.txtVbErrDesc.Name = "txtVbErrDesc";
			this.txtVbErrDesc.Size = new System.Drawing.Size(184, 48);
			this.txtVbErrDesc.TabIndex = 26;
			this.txtVbErrDesc.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(160, 464);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 24);
			this.button1.TabIndex = 27;
			this.button1.Text = "&WriteLog";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// frmActEventViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1008, 613);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.txtVbErrDesc);
			this.Controls.Add(this.txtEventMSG);
			this.Controls.Add(this.txtFilterByMsg);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.cboMachine);
			this.Controls.Add(this.lvEvents);
			this.Controls.Add(this.cmdFilter);
			this.Controls.Add(this.chkTail);
			this.Controls.Add(this.dtpTo);
			this.Controls.Add(this.dtpFrom);
			this.Controls.Add(this.cboTimeZones);
			this.Controls.Add(this.cboFilterByType);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboFilterEventSrc);
			this.Controls.Add(this.cboFilterByVBErrDesc);
			this.Controls.Add(this.cboFilterByVBErrNum);
			this.Controls.Add(this.cboFilterByVBErrSrc);
			this.Controls.Add(this.cboFilterByMsg);
			this.Name = "frmActEventViewer";
			this.Load += new System.EventHandler(this.frmActEventViewer_Load);
			((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmActEventViewer());
		}
		private void frmActEventViewer_Load(object sender, System.EventArgs eArgs)
		{
				_actEventLogCache.actEntryWrittenEventHandler += new ActEventLogCache.ActEntryWrittenEventHandler(OnEntryWritten);
		}
		private void OnEntryWritten(object source, ActEventLogErr actEventLogErr)
		{
			addEventToLV(actEventLogErr);
		}
		private void cmdFilter_Click(object sender, System.EventArgs e)
		{
			lvEvents.Items.Clear();
			this.txtEventMSG.Text = "";
			this._actEventLogCache.dtmFrom = this.dtpFrom.Value;
			this._actEventLogCache.dtmTo = this.dtpTo.Value;
			this._actEventLogCache.str_vbErrNum = cboFilterByVBErrNum.Text;
			this._actEventLogCache.str_vbErrSrc = cboFilterByVBErrSrc.Text;
			this._actEventLogCache.str_vbErrDesc = cboFilterByVBErrDesc.Text;
			this._actEventLogCache.strEventSrc = cboFilterEventSrc.Text;
			this._actEventLogCache.strEventMsg = cboFilterByMsg.Text;
			this._actEventLogCache.strEventType = this.cboFilterByType.Text;
			this._actEventLogCache.strLogName = "Application";
			this._actEventLogCache.strMachineName = cboMachine.Text;
			this._actEventLogCache.load();
			foreach (ActEventLogErr actEventLogErr in _actEventLogCache)
			{
				addEventToLV(actEventLogErr);
			}
		}
		private void addEventToLV(ActEventLogErr actEventLogErr)
		{
			ListViewItem listViewItem = new ListViewItem(new string[]{
																		 actEventLogErr.EntryType,
																		 actEventLogErr.TimeGenerated.AddHours(_intUTCOffset).ToShortDateString() ,
																		 actEventLogErr.TimeGenerated.AddHours(_intUTCOffset).ToShortTimeString(),
																		 actEventLogErr.Source.ToString(),		
																		 actEventLogErr.str_vbErrNum,
																		 actEventLogErr.str_vbErrSrc,
																		 actEventLogErr.str_vbErrDesc
																	 });
			listViewItem.Tag = actEventLogErr;
			lvEvents.Items.Insert(0,listViewItem);
		}
		private void cboFilterByMsg_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.txtFilterByMsg.Text =  cboFilterByMsg.Items[cboFilterByMsg.SelectedIndex].ToString();
		}

		private void lvEvents_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ActDiag.ActEventLogErr actEventLogErr;
			foreach (ListViewItem listViewItem in lvEvents.SelectedItems)
			{
				actEventLogErr = (ActDiag.ActEventLogErr) listViewItem.Tag;
				this.txtEventMSG.Text = actEventLogErr.Message;
			}
		}
		private void cboFilterEventSrc_DropDown(object sender, System.EventArgs e)
		{
			cboFilterEventSrc.Items.Clear(); cboFilterEventSrc.Items.Add("");
			cboFilterEventSrc.Items.AddRange(_actEventLogCache.arrEventSources);
		}

		private void cboFilterByMsg_DropDown(object sender, System.EventArgs e)
		{
			cboFilterByMsg.Items.Clear(); cboFilterByMsg.Items.Add("");
			cboFilterByMsg.Items.AddRange(_actEventLogCache.arrEventMsg);
		}

		private void cboFilterByVBErrNum_DropDown(object sender, System.EventArgs e)
		{
			cboFilterByVBErrNum.Items.Clear(); cboFilterByVBErrNum.Items.Add("");
			cboFilterByVBErrNum.Items.AddRange(_actEventLogCache.arrVBErrNum);
		}

		private void cboFilterByVBErrSrc_DropDown(object sender, System.EventArgs e)
		{
			cboFilterByVBErrSrc.Items.Clear(); cboFilterByVBErrSrc.Items.Add("");
			cboFilterByVBErrSrc.Items.AddRange(_actEventLogCache.arrVBErrSrc);
		}

		private void cboFilterByVBErrDesc_DropDown(object sender, System.EventArgs e)
		{
			cboFilterByVBErrDesc.Items.Clear(); cboFilterByVBErrDesc.Items.Add("");
			cboFilterByVBErrDesc.Items.AddRange(_actEventLogCache.arrVBErrDesc);
		}

		private void cboFilterByVBErrDesc_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.txtVbErrDesc.Text = cboFilterByVBErrDesc.Text;
		}

		private void chkTail_CheckedChanged(object sender, System.EventArgs e)
		{
			this._actEventLogCache.EnableRaisingEvent = chkTail.Checked;
		}

		private void button1_Click(object sender, System.EventArgs e)
		{		
			this._actEventLogCache.EventLog.Source = "VBRuntime1";
			string s = @"The VB Application identified by the event source logged this Application OrderManagement: Thread ID: 1892 ,Logged: An Error Occurred.
CALL STACK :   EMAX\I18N\Email\intSendMail
NUMBER :  -2147220977
SOURCE :  I18N
DESCRIPTION :  The server rejected one or more recipient addresses. The server response was: 452 4.2.2 Mailbox full";

			this._actEventLogCache.EventLog.WriteEntry(this.txtEventMSG.Text, EventLogEntryType.Error);
			//DateTimeTZ.DateTimeTZ dt = new DateTimeTZ.DateTimeTZ(2003,12,24);
			
		}

		private void cboTimeZones_SelectedValueChanged(object sender, System.EventArgs e)
		{
			this._actEventLogCache.strTimeZone = cboTimeZones.Text;
			switch (cboTimeZones.Text.ToUpper())
			{
				case "JAPAN":
					this._intUTCOffset =  9; break;
				case "ARIZONA":
					this._intUTCOffset = -7; break;
			}
		}
	}
}
