using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Timers;
using System.Threading;
namespace DAC.WindowWatchers
{
	/// <summary>
	/// Summary description for Sample.
	/// </summary>


	public abstract class Samples
	{
		public delegate void OnDataChangeHandler(Samples Samples, EventArgs e);
		public event OnDataChangeHandler OnDataChange;
		private DataColumn _definingDataColumn;
		private DataColumn _timeStampColumn;
		private DataTable _dt;
		private long _timeToLive;
		protected DataRow _lastSample;
		private bool _isAccumulative;
		System.Threading.Timer _expireTimer;
		public Samples(long timeToLive)
		{
			_timeToLive = timeToLive;
			_isAccumulative = true;
			_dt = new DataTable();
			_dt.Columns.Add("DateStamp", System.Type.GetType("System.DateTime"));

			
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			TimerCallback timerCallback = new TimerCallback(this.expire);
			_expireTimer = new System.Threading.Timer(timerCallback, autoResetEvent,0, 1000);
			
			/*
			Timer expireTimer		=  new Timer();
			expireTimer.Elapsed		+= new ElapsedEventHandler(expire);
			expireTimer.Interval	=  1000;
			expireTimer.Enabled		=  true;
			*/

		}
		public Samples()
		{
			_isAccumulative = false;
		}
		public abstract void Publish(Object obj);

		public void Clear()
		{
			OnDataChangeWrap(this, new EventArgs());
			_dt.Rows.Clear();
		}
		public DataTable DataTable
		{
			get
			{
				return _dt;
			}
			set
			{
				_dt = value;
			}
		}
		public DataView DataView
		{
			get
			{
				return _dt.DefaultView;
			}
		}
		public int Count
		{
			get
			{
				return this._dt.Rows.Count;
			}
		}

		public bool IsAccumulative
		{
			get
			{
				return _isAccumulative;
			}
		}

		public DataRow LastSample
		{
			get
			{
				return _lastSample;
			}
			set
			{
				// TODO: What the hell is this?
				if  (_lastSample == null)
				{
					_lastSample = value;
					return;
				}
				if (_lastSample.RowState == System.Data.DataRowState.Detached)
				{
					_lastSample = value;
					return;
				}
				if (_lastSample[this.DefiningDataColumn].ToString() != value[this.DefiningDataColumn].ToString())
				{
					_lastSample = value;
					return;
				}
			}
		}
		public string LastValue
		{
			get
			{
				return this.LastSample[this.DefiningDataColumn].ToString();
			}
		}

		public void expire(Object stateInfo)
		{
			expire();
		}

		public void expire()
		{
			bool rowsWereExpired = false;

			if (_isAccumulative)
			{
				Debug.WriteLine ("EXPIRING: Row Count=" + _dt.Rows.Count.ToString() + " Now=" + DateTime.Now.ToLongTimeString() + " TTL=" + this.TimeToLive.ToString());
				try
				{
					lock (this)
					{
						DataRow dr; 
						for (int i = this.DataTable.Rows.Count - 1 ; i>=0; i--)
						{
							dr = this.DataTable.Rows[i];
							//Debug.WriteLine ("\t DataRow Timestamp Considered: " + dr[this.TimeStampColumn].ToString());
							DateTime timeGenerated = DateTime.Parse(dr[this.TimeStampColumn].ToString());
							if (DateTime.Now.AddSeconds(-this.TimeToLive) > timeGenerated)
							{
								this.DataTable.Rows.RemoveAt(i);
								rowsWereExpired = true;
								//Debug.WriteLine ("\t \t" + "-Removed");
							}
						}
						if (rowsWereExpired)
						{
							OnDataChangeWrap(this, new EventArgs());
						}
					}
				}
				catch (System.Exception e)
				{
					Debug.WriteLine("Exception: " + e.Message);
				}


				//Debug.WriteLine("\n");
			}
		}

		public long TimeToLive
		{
			get
			{
				return _timeToLive;
			}
			set
			{
				_timeToLive = value;
			}
		}

		public DataColumn DefiningDataColumn
		{
			get
			{
				return _definingDataColumn;
			}
			set
			{
				_definingDataColumn = value;
			}
		}
		
		public DataColumn TimeStampColumn
		{
			get
			{
				return _timeStampColumn;
			}
			set
			{
				_timeStampColumn = value;
			}
		}

		public void OnDataChangeWrap(Samples Samples, EventArgs e)
		{
			OnDataChangeHandler eh = OnDataChange;
			if (eh != null)
			{
				eh(Samples, e);
			}
		}
	}
	public class EventLogSamples : Samples
	{
		public EventLogSamples(long timeToLive) : base(timeToLive)
		{
			createTable();
			base.DefiningDataColumn = base.DataTable.Columns["elMessage"];
			base.TimeStampColumn =  base.DataTable.Columns["elTimeGenerated"];
		} 
		public EventLogSamples() : base()
		{
			createTable();
			base.DefiningDataColumn = base.DataTable.Columns["elMessage"];
			base.TimeStampColumn =  base.DataTable.Columns["elTimeGenerated"];
		} 
		public override void Publish(object obj)
		{
			DAC.WindowWatchers.EventLogEntry eventLogEntry = (DAC.WindowWatchers.EventLogEntry) obj;
			DataRow dr = eventLogEntry.getDR(this.DataTable);
			if (!base.IsAccumulative)
			{
				base.Clear();
			}
			this.DataTable.Rows.Add(dr);    
			base.LastSample = dr;
			base.OnDataChangeWrap(this, new EventArgs());
		}
		public void createTable()
		{
			System.Type stringType = System.Type.GetType("System.String");
			base.DataTable= new DataTable("tblSamples");
			base.DataTable.Columns.Add("Value",		stringType);
			base.DataTable.Columns.Add("TypeParsedAs",		stringType);
			base.DataTable.Columns.Add("elCategory",			stringType);
			base.DataTable.Columns.Add("elCategoryNumber",	stringType);
			base.DataTable.Columns.Add("elEntryType",			stringType);
			base.DataTable.Columns.Add("elSource",			stringType);
			base.DataTable.Columns.Add("elTimeGenerated",		stringType);
			base.DataTable.Columns.Add("elUserName",			stringType);
			base.DataTable.Columns.Add("elMessage",			stringType);
			base.DataTable.Columns.Add("vbCallStack",			stringType);
			base.DataTable.Columns.Add("vbNumber",			stringType);
			base.DataTable.Columns.Add("vbSource",			stringType);
			base.DataTable.Columns.Add("vbDescription",		stringType);
			base.DataTable.Columns.Add("acaLogCategory",		stringType);
			base.DataTable.Columns.Add("acaHeader",			stringType);
			base.DataTable.Columns.Add("acaEventID",			stringType);
			base.DataTable.Columns.Add("acaBody",				stringType);
			base.DataTable.Columns.Add("acaSeverity",			stringType);
			base.DataTable.Columns.Add("acaDateTime",			stringType);
		}

		 
	}

	public class ScalerSamples : Samples
	{
		public ScalerSamples(long timeToLive) : base(timeToLive)
		{
			createTable();
			base.DefiningDataColumn = base.DataTable.Columns["Value"];
			base.TimeStampColumn =  base.DataTable.Columns["TimeStamp"];
		}
		public ScalerSamples() : base()
		{
			createTable();
			base.DefiningDataColumn = base.DataTable.Columns["Value"];
			base.TimeStampColumn =  base.DataTable.Columns["TimeStamp"];
		} 


		public override void Publish(object obj)
		{

			double sampleData = (double) obj;
			Debug.WriteLine("sampleData = " + sampleData.ToString());
			if (!base.IsAccumulative)
			{
				base.Clear();
			}
			DataRow dr = this.DataTable.NewRow();
			dr["Value"] = sampleData;
			dr["TimeStamp"] = DateTime.Now;
			lock(this)
			{
				this.DataTable.Rows.Add(dr);
			}
			base.LastSample = dr;
			base.OnDataChangeWrap(this, new EventArgs());
		}

		public void createTable()
		{
			base.DataTable= new DataTable("tblSamples");
			base.DataTable.Columns.Add("Value",System.Type.GetType("System.Double"));
			base.DataTable.Columns.Add("TimeStamp",System.Type.GetType("System.DateTime"));
		}
	}

	public interface IPublishable
	{
		void Save(DataTable dt);
		DataRow DR
		{
			get;
		}
	}


}

		