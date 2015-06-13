using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Web.Mail;

namespace DAC.WindowWatchers
{
	public class Form1 : System.Windows.Forms.Form
	{
		Mons _mons = new Mons();
		private System.Windows.Forms.Panel panel1;
		private System.ComponentModel.Container components = null;
		private Conf _conf;
		private string n = Environment.NewLine;
		public Form1()
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Location = new System.Drawing.Point(24, 16);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1240, 920);
			this.panel1.TabIndex = 0;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1272, 941);
			this.Controls.Add(this.panel1);
			this.Location = new System.Drawing.Point(255, 0);
			this.Name = "Form1";
			this.Text = "Guard";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			try
			{
				Application.Run(new Form1());
			}
			catch (Exception ex)
			{
				Debug.WriteLine ("Execption: " + ex.Message);
				Debug.WriteLine (ex.StackTrace);
			}
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			FileInfo alertLogFileInfo;
			int location = 0;
			_conf = new Conf();

			_conf.ReadXml (@"D:\Arch\MiscData\Source\ActDiag\ActMon\ActMon\XMLFile2.xml");

			KVP.Conf = _conf;
			
			alertLogFileInfo = new FileInfo(Application.StartupPath + @"\" +
												KVP.GetInstance().Value("AlertLogFile"));
			if ( ! (alertLogFileInfo.Exists) ) 
			{
				alertLogFileInfo.Create();
			}
			loadMons();

			this.Mons.start();
			
			foreach (Mon mon in this.Mons._mons)
			{
				foreach (Threshhold threshhold in mon.Threshholds._threshholds)
				{
					threshhold.Alerts.OnAlertAdded += new Alerts.OnAlertAddedHandler(OnAddAlert);
					MonitorControl monitorControl = new MonitorControl(threshhold);
					monitorControl.Location = new Point(1, location);
					location = location + monitorControl.Height;
					this.panel1.Controls.Add(monitorControl );
				}
			}
			
		}
		private Conf Conf
		{
			get
			{
				return _conf;
			}
		}

		private void OnAddAlert(Alert alert)
		{
			lock (this)
			{
				logAlert(alert);	
				sendAlert(alert);
			}
		}	
		private void sendAlert(Alert alert)
		{
			string msg = "";
			KVP kvp = KVP.GetInstance();
			msg = msg + "A threshold has been exceeded.";
			//msg = msg + alertMsg(alert);
			SmtpMail.SmtpServer = kvp.Value("SMTP");
			/*SmtpMail.Send(kvp.Value("SMTP"),
							alert.EmailAddresses,
							kvp.Value("AlertFrom"),
							msg);*/
			Debug.Write ("EMAIL:" + n + msg);
		}
		private string alertMsg(Alert alert)
		{
			string msg = "";
			msg = msg + "Date Time: " + alert.TimeCreated.ToString() + n;
			msg = msg + "Monitor: " + alert.MonName + n;
			msg = msg + "Test Expression: " + alert.Expression + n;
			msg = msg + "Duration: " + alert.Duration + n;
			msg = msg + "Exceeding Data:" + n + n;
			{
				foreach (DataRow dr in alert.ExeededDataRows)
				{
					msg = msg + "\tValue:" + dr[alert.DefiningDataColumn].ToString() + n;
					msg = msg + "\tDateTime: " + dr[alert.TimeStampColumn].ToString() + n;
				}
			}
			return msg;
		}
		private void logAlert(Alert alert)
		{
		    StreamWriter alertLogFile = new StreamWriter(Application.StartupPath + @"\" + 
															KVP.GetInstance().Value("AlertLogFile"), true);
			string logEntry;
			logEntry = n;
			logEntry = logEntry.PadRight(80, Convert.ToChar("-")) + n;
			logEntry = logEntry.PadRight(80, Convert.ToChar("-")) + n;
			logEntry = logEntry.PadRight(80, Convert.ToChar("-")) + n + n ;
			logEntry = logEntry + alertMsg(alert);
			logEntry = logEntry + n;
			logEntry = logEntry.PadRight(80, Convert.ToChar("-")) + n;
			logEntry = logEntry.PadRight(80, Convert.ToChar("-")) + n;
			logEntry = logEntry.PadRight(80, Convert.ToChar("-")) + n;
			alertLogFile.Write(logEntry);
			alertLogFile.Close();
		}
		
		public Mons Mons
		{
			get
			{
				return this._mons;
			}
		}
		
		private void loadMons(){
			Mon mon;	
			foreach (Conf.MonitorRow monDR in Conf.Monitor.Rows){
				switch (monDR.type.Trim().ToUpper())
				{
					case "DIR_FILE_COUNTER":
						mon = (Mon) new DirFileCounter(monDR,  this.Conf);
						break;
					case "PERF_MON":
						mon = (Mon) new PerfMon(monDR, this.Conf);
						break;
					case "DISK_MON":
						mon = (Mon) new DiskMon(monDR, this.Conf);
						break;
					case "PROCESS_MON":
						mon = (Mon) new ProcessMon(monDR, this.Conf);
						break;
					case "EVENT_LOG_MON":
						mon = (Mon) new EventLogMon(monDR, this.Conf);
						break;
					default:
						throw new Exception(monDR.type.Trim().ToUpper() + " is an invalid montior type.");
				}
				if (mon.Enabled)
					Mons.add(mon);
			}
		}
	}
}


