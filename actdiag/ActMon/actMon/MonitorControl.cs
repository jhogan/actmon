using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Timers;
using System.Threading;

namespace DAC.WindowWatchers
{

	/// <summary>
	/// Summary description for MonitorControl.
	/// </summary>
	public class MonitorControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Panel panel1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.Label lblType;
		private System.Windows.Forms.Label lblServer;
		private System.Windows.Forms.Label lblLocation;
		private System.Windows.Forms.Label lblCount;
		private System.Windows.Forms.Label lblOperator;
		private System.Windows.Forms.Label lblDesc;
		private System.Windows.Forms.Label lblDuration;
		private System.Windows.Forms.Label lblExceededPause;
		private System.Windows.Forms.Label lblExpression;
		private System.Windows.Forms.ListBox lstSamples;
		private System.Windows.Forms.Label lblSamplesCnt;
		private System.Windows.Forms.TextBox txtSample;
		private System.Windows.Forms.ListBox lstAlerts;
		private System.Windows.Forms.Label lblAlertsCount;
		private System.Windows.Forms.Label lblAlert;
		private Threshhold _threshhold;
		private System.Threading.Timer timer;
		public MonitorControl(Threshhold threshhold)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
			this.Threshhold = threshhold;
			this.lblName.Text =			"Name: " + Mon.Name;
			this.lblType.Text =			"Type: " + Mon.Type;
			this.lblServer.Text =		"Svr: " + Mon.Server;
			this.lblLocation.Text =		"Loc: " + Mon.Location;
			this.lblCount.Text =		"Cnt: " + this.Threshhold.Count.ToString();
			this.lblOperator.Text =		"Op: " + this.Threshhold.ComparativeOperator;
			this.lblDesc.Text =			"Desc: " + this.Threshhold.Description;
			this.lblDuration.Text =		"Dur: " + this.Threshhold.Duration.ToString();
			this.lblExceededPause.Text ="xPaus: " + this.Threshhold.ExceedPause.ToString();
			this.lblExpression.Text =	"Exp: " + this.Threshhold.Expression;

			threshhold.Mon.Samples.OnDataChange += new Samples.OnDataChangeHandler(OnDataChangeHandler);
			threshhold.Alerts.OnAlertAdded		+= new Alerts.OnAlertAddedHandler(OnRaiseAlert);
			
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			TimerCallback timerCallback = new TimerCallback(this.OnDataRefresh);
			timer = new System.Threading.Timer(timerCallback, autoResetEvent,0, 1000);
		
			/*System.Timers.Timer dataRefresh = new System.Timers.Timer(10000);
			dataRefresh.Elapsed += new ElapsedEventHandler(OnDataRefresh);
			dataRefresh.Enabled = false;*/
		}
		private void OnRaiseAlert(Alert alert)
		{
			this.lstAlerts.BeginUpdate();
			this.lstAlerts.Items.Clear();
			foreach (Alert a in this.Threshhold.Alerts._alerts)
			{
				this.lstAlerts.Items.Insert(0, a.TimeCreated);
			}
			/*
			foreach (DataRow dr in alert.ExeededDataRows)
			{
				this.lstAlerts.Items.Insert(0, dr[alert.DefiningDataColumn].ToString() + "-" 
													+ dr[alert.TimeStampColumn].ToString());
			}
			*/
			this.lstAlerts.EndUpdate();
			this.lblAlertsCount.Text = this.Threshhold.Alerts.Count.ToString(); ;
		}
		private void OnDataRefresh(Object stateInfo)
		{
			this.lblDuration.Text = Convert.ToInt64(this.Threshhold.SecondsUntilActive).ToString() ;
		}

		public void OnDataChangeHandler(Samples samples, EventArgs e)
		{
			this.lstSamples.Items.Clear();
			
			this.lstSamples.BeginUpdate();
			lock(samples)
			{
				foreach (DataRow dr in samples.DataTable.Rows)
				{
					this.lstSamples.Items.Insert(0, dr[samples.DefiningDataColumn].ToString() + "-" +   dr[samples.TimeStampColumn].ToString());
				}
			}

			this.lstSamples.EndUpdate();

			this.lblSamplesCnt.Text = samples.DataTable.Rows.Count + " samples";
			if (this.Threshhold.Exceeded)
			{
				this.lblAlert.BackColor = Color.Yellow;
				this.lblAlert.Text = "ALERT";
			}
			else
			{
				this.lblAlert.BackColor = this.lblDuration.BackColor;
				this.lblAlert.Text = "";
			}
		}
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblAlert = new System.Windows.Forms.Label();
			this.lstAlerts = new System.Windows.Forms.ListBox();
			this.txtSample = new System.Windows.Forms.TextBox();
			this.lblSamplesCnt = new System.Windows.Forms.Label();
			this.lblAlertsCount = new System.Windows.Forms.Label();
			this.lstSamples = new System.Windows.Forms.ListBox();
			this.lblExpression = new System.Windows.Forms.Label();
			this.lblExceededPause = new System.Windows.Forms.Label();
			this.lblDuration = new System.Windows.Forms.Label();
			this.lblLocation = new System.Windows.Forms.Label();
			this.lblDesc = new System.Windows.Forms.Label();
			this.lblServer = new System.Windows.Forms.Label();
			this.lblOperator = new System.Windows.Forms.Label();
			this.lblType = new System.Windows.Forms.Label();
			this.lblCount = new System.Windows.Forms.Label();
			this.lblName = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.lblAlert);
			this.panel1.Controls.Add(this.lstAlerts);
			this.panel1.Controls.Add(this.txtSample);
			this.panel1.Controls.Add(this.lblSamplesCnt);
			this.panel1.Controls.Add(this.lblAlertsCount);
			this.panel1.Controls.Add(this.lstSamples);
			this.panel1.Controls.Add(this.lblExpression);
			this.panel1.Controls.Add(this.lblExceededPause);
			this.panel1.Controls.Add(this.lblDuration);
			this.panel1.Controls.Add(this.lblLocation);
			this.panel1.Controls.Add(this.lblDesc);
			this.panel1.Controls.Add(this.lblServer);
			this.panel1.Controls.Add(this.lblOperator);
			this.panel1.Controls.Add(this.lblType);
			this.panel1.Controls.Add(this.lblCount);
			this.panel1.Controls.Add(this.lblName);
			this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(680, 208);
			this.panel1.TabIndex = 20;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// lblAlert
			// 
			this.lblAlert.Location = new System.Drawing.Point(568, 48);
			this.lblAlert.Name = "lblAlert";
			this.lblAlert.TabIndex = 39;
			// 
			// lstAlerts
			// 
			this.lstAlerts.ItemHeight = 16;
			this.lstAlerts.Location = new System.Drawing.Point(8, 80);
			this.lstAlerts.Name = "lstAlerts";
			this.lstAlerts.ScrollAlwaysVisible = true;
			this.lstAlerts.Size = new System.Drawing.Size(192, 84);
			this.lstAlerts.TabIndex = 38;
			// 
			// txtSample
			// 
			this.txtSample.Location = new System.Drawing.Point(408, 80);
			this.txtSample.Multiline = true;
			this.txtSample.Name = "txtSample";
			this.txtSample.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtSample.Size = new System.Drawing.Size(264, 96);
			this.txtSample.TabIndex = 37;
			this.txtSample.Text = "";
			// 
			// lblSamplesCnt
			// 
			this.lblSamplesCnt.Location = new System.Drawing.Point(208, 176);
			this.lblSamplesCnt.Name = "lblSamplesCnt";
			this.lblSamplesCnt.Size = new System.Drawing.Size(80, 24);
			this.lblSamplesCnt.TabIndex = 36;
			// 
			// lblAlertsCount
			// 
			this.lblAlertsCount.Location = new System.Drawing.Point(8, 176);
			this.lblAlertsCount.Name = "lblAlertsCount";
			this.lblAlertsCount.Size = new System.Drawing.Size(104, 16);
			this.lblAlertsCount.TabIndex = 35;
			// 
			// lstSamples
			// 
			this.lstSamples.ItemHeight = 16;
			this.lstSamples.Location = new System.Drawing.Point(208, 80);
			this.lstSamples.Name = "lstSamples";
			this.lstSamples.ScrollAlwaysVisible = true;
			this.lstSamples.Size = new System.Drawing.Size(192, 84);
			this.lstSamples.TabIndex = 34;
			this.lstSamples.DoubleClick += new System.EventHandler(this.lstSamples_DoubleClick);
			this.lstSamples.SelectedIndexChanged += new System.EventHandler(this.lstSamples_SelectedIndexChanged);
			// 
			// lblExpression
			// 
			this.lblExpression.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblExpression.Location = new System.Drawing.Point(0, 56);
			this.lblExpression.Name = "lblExpression";
			this.lblExpression.Size = new System.Drawing.Size(560, 16);
			this.lblExpression.TabIndex = 33;
			this.lblExpression.Text = "Exp";
			this.lblExpression.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblExceededPause
			// 
			this.lblExceededPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblExceededPause.Location = new System.Drawing.Point(336, 32);
			this.lblExceededPause.Name = "lblExceededPause";
			this.lblExceededPause.Size = new System.Drawing.Size(64, 16);
			this.lblExceededPause.TabIndex = 32;
			this.lblExceededPause.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblDuration
			// 
			this.lblDuration.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblDuration.Location = new System.Drawing.Point(272, 32);
			this.lblDuration.Name = "lblDuration";
			this.lblDuration.Size = new System.Drawing.Size(64, 16);
			this.lblDuration.TabIndex = 31;
			this.lblDuration.Text = "Dur";
			this.lblDuration.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblLocation
			// 
			this.lblLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblLocation.Location = new System.Drawing.Point(400, 32);
			this.lblLocation.Name = "lblLocation";
			this.lblLocation.Size = new System.Drawing.Size(136, 16);
			this.lblLocation.TabIndex = 30;
			this.lblLocation.Text = "Loc";
			this.lblLocation.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblDesc
			// 
			this.lblDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblDesc.Location = new System.Drawing.Point(112, 32);
			this.lblDesc.Name = "lblDesc";
			this.lblDesc.Size = new System.Drawing.Size(160, 16);
			this.lblDesc.TabIndex = 29;
			this.lblDesc.Text = "Desc";
			this.lblDesc.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblServer
			// 
			this.lblServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblServer.Location = new System.Drawing.Point(344, 8);
			this.lblServer.Name = "lblServer";
			this.lblServer.Size = new System.Drawing.Size(96, 16);
			this.lblServer.TabIndex = 28;
			this.lblServer.Text = "Svr";
			this.lblServer.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblOperator
			// 
			this.lblOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblOperator.Location = new System.Drawing.Point(48, 32);
			this.lblOperator.Name = "lblOperator";
			this.lblOperator.Size = new System.Drawing.Size(56, 16);
			this.lblOperator.TabIndex = 27;
			this.lblOperator.Text = "Op";
			this.lblOperator.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblType
			// 
			this.lblType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblType.Location = new System.Drawing.Point(136, 8);
			this.lblType.Name = "lblType";
			this.lblType.Size = new System.Drawing.Size(176, 16);
			this.lblType.TabIndex = 26;
			this.lblType.Text = "Type";
			this.lblType.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblCount
			// 
			this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblCount.Location = new System.Drawing.Point(0, 32);
			this.lblCount.Name = "lblCount";
			this.lblCount.Size = new System.Drawing.Size(48, 16);
			this.lblCount.TabIndex = 25;
			this.lblCount.Text = "Cnt";
			this.lblCount.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblName
			// 
			this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblName.Location = new System.Drawing.Point(0, 8);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(136, 16);
			this.lblName.TabIndex = 24;
			this.lblName.Text = "Name:";
			this.lblName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// MonitorControl
			// 
			this.Controls.Add(this.panel1);
			this.Name = "MonitorControl";
			this.Size = new System.Drawing.Size(696, 216);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		
		}

		private void lstSamples_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			object selectedItem = lstSamples.SelectedItem;
			if (selectedItem != null)
				this.txtSample.Text = selectedItem.ToString();	
		}

		private void lstSamples_DoubleClick(object sender, System.EventArgs e)
		{
			DataTableForm dataTableForm = new DataTableForm(this.Threshhold.Samples.DataTable);
			dataTableForm.Activate();
			dataTableForm.Visible = true;
			dataTableForm.Focus();
		}
	
		private Threshhold Threshhold
		{
			get
			{
				return _threshhold;
			}
			set
			{
				_threshhold = value;
			}
		}
		private Mon Mon
		{
			get
			{
				return this.Threshhold.Mon;
			}
		}
	}
}
