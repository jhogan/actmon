using System;
using System.Collections;
using System.Data;

namespace DAC.WindowWatchers
{
	/// <summary>
	/// Summary description for TheshHolds.
	/// </summary>
	
	public class Threshholds
	{
		//UNDONE Make class enumable
		public ArrayList _threshholds = new ArrayList();
		private Samples _samples;
		public Threshholds(Mon mon, Conf.ThreshholdRow[] threshholdRows, Conf conf)
		{
			for (int i=0; i<threshholdRows.Length; i++)
			{
				if (threshholdRows[i].Enabled)
					_threshholds.Add(new Threshhold(mon, threshholdRows[i], conf));
			}
		}
		
		public long LongestDuration
		{
			get
			{
				long MaxDuration = 0;
				foreach(Threshhold threshhold in _threshholds)
				{
					if (threshhold.Duration > MaxDuration)
					{
						MaxDuration = threshhold.Duration;
					}
				}
				return MaxDuration;
			}
		}
		public bool IsDurational
		{
			get
			{
				foreach(Threshhold threshhold in _threshholds)
				{
					if (threshhold.IsDurational)
					{
						return true;
					}
				}
				return false;
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
				foreach (Threshhold threshhold in _threshholds)
				{
					threshhold.Samples = value;
				}
				_samples = value;
			}
		}
	}

	public class Threshhold
	{
		private Conf _conf;
		private Mon _mon;
		private Conf.ThreshholdRow _threshholdRow;
		private TestResults _lastestTestResults;
		private DistList _distList;
		private Alerts _alerts;
		private Samples _samples;
		public Threshhold(Mon mon, Conf.ThreshholdRow threshholdRow, Conf conf)
		{
			this.Mon =  mon;
			this.LastestTestResults = new TestResults(this);
			_threshholdRow = threshholdRow; 
			_conf = conf;
			_distList = new DistList(_threshholdRow.distList, _conf);
			_alerts = new Alerts();
		}
		public Samples Samples
		{
			get 
			{
				return _samples;
			}
			set
			{
				_samples = value;
			}
		}
		public System.DateTime StartAt
		{
			get 
			{
				return _threshholdRow.startAt;
			}
		}
		public bool IsStartAtNull
		{
			get
			{
				return this._threshholdRow.IsstartAtNull();
			}
		}

		public DateTime EndAt
		{
			
			get {return _threshholdRow.endAt;}
		}

		public bool IsEndAtNull
		{
			get
			{
				return this._threshholdRow.IsendAtNull();
			}
		}
		
		public double Value		
		{
			get {return _threshholdRow.Value;}
		}

		public long Duration
		{
			get 
			{
				return _threshholdRow.IsDuarationNull() ? 0 : _threshholdRow.Duaration;
			}
		}

		public bool IsDurational
		{
			get 
			{
				return !(this._threshholdRow.IsDuarationNull() || this._threshholdRow.Duaration == 0);
			}
		}
		public string Type1
		{
			get
			{
				return this._threshholdRow.Type.ToUpper();
			}
		}
		public bool IsTypeMax
		{
			get
			{
				return (this.Type1 == "MAX");
			}
		}
		public bool IsTypeMin
		{
			get
			{
				return (this.Type1 == "MIN");
			}
		}
		public bool IsTypeEq
		{
			get
			{
				return (this.Type1 == "EQ");
			}
		}
					
		public double ExceedPause
		{
			get
			{
				return this._threshholdRow.ExceededPause;
			}
		}
		public string Description
		{
			get
			{
				return !this.ThreshholdRow.IsDescriptionNull() ? this.ThreshholdRow.Description : "";
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
				if (this._distList == null)
				{
					return this.Mon.DistList;
				}
				else
				{
					return this._distList;
				}
			}
		}
		private Conf.ThreshholdRow ThreshholdRow
		{
			get
			{
				return _threshholdRow;
			}
			set
			{
				_threshholdRow = value;
			}
		}
		public Mon Mon
		{
			get
			{
				return _mon;
			}
			set
			{
				_mon = value;
			}
		}

		public DateTime BeginTimeForDuration
		{
			get
			{
				return DateTime.Now.AddSeconds(-this.Duration);
			}
			
		}
		public bool IsWithinDuration(DateTime dt)
		{
			if (this.BeginTimeForDuration.CompareTo(dt) == -1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsConsecutive		
		{
			get
			{
				return this._threshholdRow.Consecutive;
			}
		}

		public double SecondsUntilActive
		{
			get
			{
				//System.Diagnostics.Trace.WriteLine("SecondsUntilActive");
				if (this.LastestTestResults.HasBeenExceeded)
				{
					
					TimeSpan ts = DateTime.Now.Subtract(this.LastestTestResults.TimeLastExceeded);
					System.Diagnostics.Trace.WriteLine("Count Down " + ts.TotalSeconds );
					if (ts.TotalSeconds < this.ThreshholdRow.ExceededPause)
					{
						System.Diagnostics.Trace.WriteLine("ts.TotalSeconds < this.ThreshholdRow.ExceededPause");
						return (this.ThreshholdRow.ExceededPause - ts.TotalSeconds);
					}
					else
					{
						System.Diagnostics.Trace.WriteLine("ts.TotalSeconds > this.ThreshholdRow.ExceededPause");
						return 0;
					}
				}
				else
				{
					return 0;
				}
			}
		}

		public bool Active
		{
			get
			{
				if (this.SecondsUntilActive > 0D)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
		public bool Enabled
		{
			get
			{
				return this.ThreshholdRow.Enabled;
			}
		}

		public int Count
		{
			get
			{
				if (this.ThreshholdRow.IsCountNull())
				{
					return 1;
				}
				else
				{
					return this.ThreshholdRow.Count;
				}
			}
		}
		public string Expression
		{
			get
			{
				return this._threshholdRow.Expression;
			}
		}
		public bool IsExpressive
		{
			get
			{
				return !this._threshholdRow.IsExpressionNull();
			}
		}
		public string ComparativeOperator
		{
			get
			{
				if (this.ThreshholdRow.IsComparativeOperatorNull())
				{
					return "EQ";
				}
				else
				{
					return this.ThreshholdRow.ComparativeOperator.ToUpper();
				}
			}
		}

		public TestResults LastestTestResults
		{
			get
			{
				return _lastestTestResults;
			}
			set
			{
				_lastestTestResults = value;
			}
		}


		public DataRow[] Matches()
		{
			string expression = this.Expression;
			if (this.IsDurational)
			{
				expression += "\n" + 
						" AND " + this.Samples.TimeStampColumn.ColumnName + " >= " + 
								"'" + this.EarliesSampleTime.ToString() + "'" +
						" AND " + this.Samples.TimeStampColumn.ColumnName + " <= " + 
								"'" + DateTime.Now.ToString() + "'";
			}

			return this.Samples.DataTable.Select(expression);
		}
		public DateTime EarliesSampleTime
		{
			get
			{
				if (this.IsDurational)
				{
					return DateTime.Now.AddSeconds(-this.Duration);
				}
				else
				{
					throw new Exception();
				}
			}
		}

		public void test()
		{
			bool threshholdExceededed = false;
			DataRow[] drMatches = this.Matches();
			
			int matchCount = drMatches.Length;
			
			switch (this.ComparativeOperator)
			{
				case "EQ":
					if (this.Count == matchCount)
					{
						threshholdExceededed = true;
					}
					break;
				case "NE":
					if (this.Count != matchCount)
					{
						threshholdExceededed = true;
					}
					break;
				case "LT":
					if (this.Count > matchCount)
					{
						threshholdExceededed = true;
					}
					break;
				case "GT":
					if (this.Count < matchCount)
					{
						threshholdExceededed = true;
					}
					break;
			}
			if (threshholdExceededed)
			{
				this.LastestTestResults.DRExceeded= drMatches;
				this.LastestTestResults.TimeLastExceeded = DateTime.Now;
				this.LastestTestResults.Exceeded = true;
				this.LastestTestResults.ExceededCount = matchCount;
				Alert alert = new Alert(this.EmailAddresses,
										this.LastestTestResults.TimeLastExceeded,
										this.Mon.Name,
										this.Expression,
										this.Duration,
										Util.Clone(this.LastestTestResults.DRExceeded),
										this.Mon.Samples.TimeStampColumn,
										this.Mon.Samples.DefiningDataColumn);
				this.Alerts.add(alert);
			}
			else
			{
				this.LastestTestResults.Exceeded = false;
			}
		}
		public bool Exceeded
		{
			get
			{
				return this.LastestTestResults.Exceeded;
			}
		}
		public Alerts Alerts
		{
			get
			{
				return this._alerts;
			}
		}

	}
	
		public class TestResults
		{
			private DataRow[] _DRExceeded; 
			private DateTime _timeLastExceeded; 
			private bool _Exceeded; 
			private Threshhold _threshhold;
			private bool _hasBeenExceeded;
			private int _exceededCount;

			public TestResults(Threshhold threshhold)
			{
				_threshhold = threshhold;
			}
			
			public int ExceededCount
			{
				get
				{
					return _exceededCount;
				}
				set 
				{
					
					_exceededCount = value;
				}
			}
			public DataRow[] DRExceeded
			{
				get
				{
					return _DRExceeded;
				}
				set 
				{
					_DRExceeded = value;
				}
			}
			public DateTime TimeLastExceeded
			{
				get
				{
					return _timeLastExceeded;
				}
				set 
				{
					
					_timeLastExceeded = value;
				}
			}
			public bool Exceeded
			{
				get
				{
					return _Exceeded;
				}
				set 
				{
					if (value)
					{
						this.HasBeenExceeded = true;
						_Exceeded = true;
					}
					else
					{
						_Exceeded = false;
					}
				}
			}
			public bool HasBeenExceeded
			{
				get
				{
					return _hasBeenExceeded;
				}
				set 
				{
					_hasBeenExceeded = value;
				}
			}


			public DAC.WindowWatchers.Threshhold ThreshHold
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
			public object Clone()
			{
				return this.MemberwiseClone();
			}
		}
	}

