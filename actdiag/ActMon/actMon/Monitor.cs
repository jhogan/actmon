using System;
using System.Collections;
using System.Diagnostics;
using System.Timers;
using System.IO;
using System.Data;



namespace DAC.WindowWatchers
{
	/// <summary>
	/// Summary description for Mons.
	/// </summary>
	
	public enum CollectionEventType
	{
		Default,
		Custom
	}
	public enum SampleType
	{
		Scaler,
		EventLog,
		Custom
	}
	public class Mons
	{
		public ArrayList _mons = new ArrayList();
		public Mons()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public void add(Mon mon){
			_mons.Add(mon);
		}
		public void start()
		{
			foreach (Mon mon in _mons)
			{
				mon.start();
			}

		}
	}
	public abstract class Mon
	{
		private Threshholds _threshholds;
		private System.Threading.Timer _expireTimer;
		protected Samples _samples;
		protected Conf _conf;
		protected Conf.MonitorRow _monDR;
		protected DistList _distList;
		public Mon(Conf.MonitorRow monDR, Conf conf)
		{
			_monDR = monDR; _conf = conf;
			_threshholds = new Threshholds(this, this._monDR.GetThreshholdRows(), _conf);
			_distList = new DistList(_monDR.distList, _conf);
			switch (this.InstantiateSample())
			{
				case SampleType.Scaler:
					if (this.Threshholds.IsDurational)
					{
						this.Samples = new ScalerSamples(this.Threshholds.LongestDuration);
					}
					else
					{
						this.Samples = new ScalerSamples();
					}
					break;
				case SampleType.EventLog:
				{
					if (this.Threshholds.IsDurational)
					{
						this.Samples = new EventLogSamples(this.Threshholds.LongestDuration);
					}
					else
					{
						this.Samples = new EventLogSamples();
					}
					break;
				}
				case SampleType.Custom:
				{
					// Implementation will be found in subclass
					break;
				}
			}
			if (this.InitializeCollectionEvent() == CollectionEventType.Default)
			{

				System.Threading.AutoResetEvent autoResetEvent = new System.Threading.AutoResetEvent(false);
				System.Threading.TimerCallback timerCallback = new System.Threading.TimerCallback(SampleCollection);
				this.ExpireTimer = new System.Threading.Timer(timerCallback, autoResetEvent,0, 1000);
			}
		}		
		protected System.Threading.Timer ExpireTimer
		{
			get
			{
				return _expireTimer;
			}
			set
			{
				_expireTimer = value;
			}
		}
		public abstract void start();
		public string Value	
		{
			get
			{
				if ( !(this.Samples.LastSample == null) )
				{
					return this.Samples.LastSample[this.Samples.DefiningDataColumn.ColumnName].ToString();
				}
				else
				{
					return "";
				}
				
			}
		}
	
		public int id
		{
			get{return _monDR.id;}
		}
		public int PollInterval
		{
			get
			{
				if (this.MonDR.IspollIntervalNull())
				{
					return 0;
				}
				else
				{
					return _monDR.pollInterval * 1000;
				}
			}
		}
		public string Name		
		{
			get{return _monDR.name;}
		}
		public string Server
		{
			get
			{
				if ( !(_monDR.IsserverNull()) )
				{
					return _monDR.server;
				}
				else
				{
					return "";
				}
			}
		}
		public Conf.MonitorRow MonDR
		{
			get
			{
				return this._monDR;
			}
		}
		public bool IsLocationNull
		{
			get
			{
				return this.MonDR.IslocationNull();
			}
		}

		public string Location
		{
			get
			{
				if (this.IsLocationNull)
				{
					return "";
				}
				else
				{
					return _monDR.location;
				}
			}
		}
		public string Type
		{
			get{return _monDR.type.Trim().ToUpper();}
		}
		public Threshholds Threshholds
		{
			get
			{
				return _threshholds;
			}
		}

		public Samples Samples
		{
			get 
			{
				return _samples;
			}
			set
			{
				this.Threshholds.Samples = value;
				_samples = value;
			}
		}
		protected void TestThreshhold()
		{

			//_samples.expire();
			foreach(Threshhold threshhold in this.Threshholds._threshholds)
			{
				if (!threshhold.Active)
				{
					continue;
				}
				else
				{
					threshhold.test();
				}
			}
		}
		public string UNCPath
		{
			get
			{
				if (this.Server != "")
				{
					return @"\\" + this.Server + @"\" + this.Location;
				}
				else
				{
					return this.Location;
				}
			}
		}
		public string EmailAddresses
		{
			get
			{
				return this.DistList.EmailAddresses;
			}
		}
		public DistList DistList
		{
			get
			{
				return this._distList;
			}
		}
		public bool Enabled
		{
			get
			{
				foreach (Threshhold threshhold in this.Threshholds._threshholds)
				{
					if (threshhold.Enabled)
					{
						return true;
					}
				}
				return false;
			}
		}
		protected abstract SampleType InstantiateSample();
		protected abstract CollectionEventType InitializeCollectionEvent();
		protected abstract void SampleCollection(object state);
	}
	public class DirFileCounter : Mon	
	{
		private DirectoryInfo _dir;
		public DirFileCounter(Conf.MonitorRow monDR, Conf conf): base(monDR, conf)
		{
		}
		protected override SampleType InstantiateSample()
		{
			return SampleType.Scaler;
		}
		protected override CollectionEventType InitializeCollectionEvent()
		{
			return CollectionEventType.Default;
		}
		protected override void SampleCollection(object state)
		{
			lock (this)
			{
				try			
				{
					Debug.WriteLine("Polling for: " + this.GetType());
					
					base.Samples.Publish((object) (double) this.Dir.GetFiles().Length);
					base.TestThreshhold();
				}
				catch(Exception ex)
				{
					Debug.WriteLine ("Execption: " + ex.Message);
					Debug.WriteLine (ex.StackTrace);
				}
			}
		}

		public override void start()
		{}
		public new string Value
		{
			get{return base.Value;}
		}

		private DirectoryInfo Dir
		{
			get
			{
				if (_dir == null)
				{
					_dir = new DirectoryInfo(base.UNCPath);
				}
				return _dir;
			}
			set
			{
				_dir = value;
			}
		}
	}
	public class DiskMon : Mon
	{
		DiskDrive _disk;
		private System.Threading.Timer _expireTimer;
		public DiskMon(Conf.MonitorRow monDR, Conf conf): base(monDR, conf)
		{}

		
		public override void start()
		{
			InitializeCollectionEvent();
		}
		protected override SampleType InstantiateSample()
		{
			return SampleType.Scaler;
		}
		protected override CollectionEventType InitializeCollectionEvent()
		{
			return CollectionEventType.Default;
		}
		protected override void SampleCollection(object state)
		{
			lock (this)
			{
				Disk.Refresh();
				try			
				{
					Debug.WriteLine("Polling for: " + this.GetType());
					base.Samples.Publish((object) (double) Disk.FreeBytesAvailable);
					base.TestThreshhold();
				}
				catch(Exception ex)
				{
					Debug.WriteLine ("Execption: " + ex.Message);
					Debug.WriteLine (ex.StackTrace);
				}
			}
		}
		private DiskDrive Disk
		{
			get
			{
				if (_disk == null)
					_disk = new DiskDrive(base.UNCPath);
				return _disk;
			}
			set
			{
				_disk = value;
			}
		}
	}
	public class EventLogMon :Mon
	{
		EventLog _eventLog;
		public EventLogMon(Conf.MonitorRow monDR, Conf conf): base(monDR, conf)
		{
		}
		protected override SampleType InstantiateSample()
		{
			return SampleType.Scaler;
		}
		protected override CollectionEventType InitializeCollectionEvent()
		{
			_eventLog = new EventLog();
			_eventLog.Log = base.Location;
			if (base.Server != "") 
				_eventLog.MachineName = base.Server;

			_eventLog.EntryWritten += new EntryWrittenEventHandler(CollectEntries);
			_eventLog.EnableRaisingEvents = true;
			return CollectionEventType.Custom;
		}
		public override void start()
		{
			InitializeCollectionEvent();
		}

		protected override void SampleCollection(object state)
		{
			// Not implemented here. CollectEntries() is used instead.
		}
		private void CollectEntries(Object source, EntryWrittenEventArgs e)
		{
			try
			{
				lock(this)
				{
					DAC.WindowWatchers.EventLogEntry eventLogEntry = DAC.WindowWatchers.EventLogEntry.GetEventLogEntry(e.Entry);

					//Debug.WriteLine("Collecting MSG " + eventLogEntry.Entry.Message);
					base.Samples.Publish(eventLogEntry);
					base.TestThreshhold();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine ("Execption: " + ex.Message);
				Debug.WriteLine (ex.StackTrace);
			}

		}
	}
	public class PerfMon : Mon
	{
		private System.Threading.Timer _expireTimer;
		System.Diagnostics.PerformanceCounter _performanceCounter;
		public PerfMon(Conf.MonitorRow monDR, Conf conf): base(monDR, conf)
		{
		}

		protected override SampleType InstantiateSample()
		{
			return SampleType.Scaler;
		}
		protected override CollectionEventType InitializeCollectionEvent()
		{
			return CollectionEventType.Default;
		}

		public override void start()
		{
			InitializeCollectionEvent();
		}

		protected override void SampleCollection(object state)
		{
			lock (this)
			{
				try
				{
					Debug.WriteLine("Polling for: " + this.GetType());
					//Debug.WriteLine("mPerformanceCounter: " + mPerformanceCounter.NextValue().ToString());
					object obj = (double) PerformanceCounter.NextValue();
					base.Samples.Publish(obj);
					base.TestThreshhold();
				}
				catch(Exception ex)
				{
					Debug.WriteLine ("Execption: " + ex.Message);
					Debug.WriteLine (ex.StackTrace);
				}
			}
		}
		private PerformanceCounter PerformanceCounter
		{
			get
			{
				if (_performanceCounter == null)
				{
					_performanceCounter = new PerformanceCounter(this.PerfCatagory,this.perfCounterName, this.perfInstanceName, base.Server);
				}
				return _performanceCounter;
			}
			set
			{
				_performanceCounter = value;
			}
		}
		public string PerfCatagory
		{
			get {return base._monDR.perfCatagory;}
		}

		public string perfCounterName
		{
			get {return base._monDR.perfCounterName;}
		}
		
		public string perfInstanceName
		{
			get {return base._monDR.perfInstanceName;}
		}

	}
	public class PingMon: Mon
	{
		private System.Threading.Timer _expireTimer;
		private Ping ping = new Ping();
		public PingMon(Conf.MonitorRow monDR, Conf conf): base(monDR, conf)
		{
		}
		public override void start()
		{
			InitializeCollectionEvent();
		}
		protected override SampleType InstantiateSample()
		{
			return SampleType.Scaler;
		}
		protected override CollectionEventType InitializeCollectionEvent()
		{
			return CollectionEventType.Default;
		}
		protected override void SampleCollection(object state)
		{
			ping.ping("www.yahoo.com");
		}
	}
	public class ProcessMon : Mon
	{
		public ProcessMon(Conf.MonitorRow monDR, Conf conf): base(monDR, conf)
		{
		}
		public override void start()
		{
			InitializeCollectionEvent();
		}
		protected override SampleType InstantiateSample()
		{
			return SampleType.Scaler;
		}
		protected override CollectionEventType InitializeCollectionEvent()
		{
			return CollectionEventType.Default;
		}

		protected override void SampleCollection(object state)
		{
			Debug.WriteLine("Polling for: " + this.GetType());
			Debug.WriteLine("Process count for: " + base.Location+ ": " + this.ProcessesCount.ToString());
			this.Samples.Publish((object) (double)  this.ProcessesCount);
			base.TestThreshhold();
		}
		public Process[] Processes
		{
			get
			{	
				return Process.GetProcessesByName(base.Location, base.Server);
			}
		}
		private int ProcessesCount
		{
			get { return Processes.Length; }
		}
	}
}
