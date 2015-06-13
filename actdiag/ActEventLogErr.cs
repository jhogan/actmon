using System;
using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;

namespace ActDiag
{
	
	public class ActEventLogCache : IEnumerable
	{
		private ArrayList _arr_lstActEventLogErr = new ArrayList();
		private EventLog _EventLog;
		public delegate void ActEntryWrittenEventHandler(object source, ActEventLogErr actEventLogErr);
		public ActEntryWrittenEventHandler actEntryWrittenEventHandler;
		private bool _blnRefreshCurSet;
		private string _strTimeZone; private int _intUTCOffset;
		// Filter on these members
		private string _strLogName = ""; private string _strMachineName = "";
		private string _strEventSrc = ""; private string _strEventMsg = "";
		private string _str_vbErrNum = ""; private string _str_vbErrSrc = ""; 
		private string _str_vbErrDesc= ""; private string _strEventType = "";
		private DateTime _dtmFrom; private DateTime _dtmTo;
		

		private ActEventLogErr this[int Index]
		{
			get
			{
				return (ActEventLogErr) _arr_lstActEventLogErr[Index];
			}
		}
		public ActEventLogCache()
		{
			_EventLog = new EventLog("Application");
			_EventLog.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);
		}
		public EventLog EventLog {get{return this._EventLog;}}
		public bool EnableRaisingEvent
		{
			get{return this._EventLog.EnableRaisingEvents;}
			set{this._EventLog.EnableRaisingEvents = value;}
		}
		public string strLogName 
		{
			get{return _strLogName;}
			set
			{
				if (_strLogName != value && value != "") 
				{
					_blnRefreshCurSet = true;
					this._EventLog.Log = value;
				}
			}
		}
		public string strMachineName 
		{
			get{return _strMachineName;}
			set
			{
				if (_strMachineName != value && value != "") 
				{
					_blnRefreshCurSet = true;
					this._EventLog.MachineName = value;
				}
			}
		}
		public string strTimeZone 
		{
			get{return _strTimeZone;}
			set
			{
				_strTimeZone = value;
				switch (value.ToUpper())
				{
					case "JAPAN":
						this._intUTCOffset =  9; break;
					case "ARIZONA":
						this._intUTCOffset = -7; break;
				}
			}
		}
		public string strEventSrc 
		{
			get{return _strEventSrc;}
			set
			{
				if (_strEventSrc != "" && value == "") {_blnRefreshCurSet = true;}
				_strEventSrc = value;
			}
		}

		public string strEventMsg		
		{
				get{return _strEventMsg;}	
			set
			{
				if (_strEventMsg != "" && value == "") {_blnRefreshCurSet = true;}
				_strEventMsg = value;
			}
		}

		public string str_vbErrNum		
		{
				get{return _str_vbErrNum;}
			set
			{
				if (_str_vbErrNum != "" && value != "") {_blnRefreshCurSet = true;}
				_str_vbErrNum = value;
			}
		}
		public string str_vbErrSrc		
		{
				get{return _str_vbErrSrc;}
			set
			{
				if (_str_vbErrSrc != "" && value == "") {_blnRefreshCurSet = true;}
				_str_vbErrSrc = value;
			}
		}
		public string str_vbErrDesc		
		{
				get{return _str_vbErrDesc;}
			set
			{
				if (_str_vbErrDesc != "" && value == "") {_blnRefreshCurSet = true;}
				_str_vbErrDesc = value;
			}
		}
		public string strEventType	
		{
			get{return _strEventType;}	
			set
			{
				if (_strEventType != "" && value == "") {_blnRefreshCurSet = true;}
				_strEventType = value;
			}
		}
		public DateTime dtmFrom
		{
			get{return _dtmFrom;}	
			set
			{
				if (_dtmFrom != new DateTime())
				{
					if (_dtmFrom > value){_blnRefreshCurSet = true;}
				}

			
				_dtmFrom = value;
			}
		}
		
		public DateTime dtmTo
		{
			get{return _dtmTo;}	
			set
			{
				if (_dtmTo != new DateTime())
				{
					if (_dtmTo < value){_blnRefreshCurSet = true;}
				}

			
				_dtmTo = value;
			}
		}
		
		public void load()
		{
			if (_blnRefreshCurSet)
			{
				_arr_lstActEventLogErr.Clear();
				int i = _EventLog.Entries.Count;
				ActEventLogErr actEventLogErr;
				do
				{
					i--; actEventLogErr = new ActEventLogErr(_EventLog.Entries[i]); 
					if (!filtered(actEventLogErr))
					{
						_arr_lstActEventLogErr.Add(actEventLogErr);
					}
				}while (actEventLogErr.TimeGenerated > this.dtmFrom.AddHours(_intUTCOffset));
			}
			else
			{
				foreach (ActEventLogErr actEventLogErr in _arr_lstActEventLogErr)
				{
					if (!filtered(actEventLogErr))
					{
						_arr_lstActEventLogErr.Add(actEventLogErr);
					}
				}
			}
			_blnRefreshCurSet = false;

		}
		private void OnEntryWritten(Object source, EntryWrittenEventArgs e)
		{
			ActEventLogErr actEventLogErr = new ActEventLogErr(e.Entry);
			if (!filtered(actEventLogErr))
			{
				actEntryWrittenEventHandler(source, actEventLogErr);
			}
		}
		private bool filtered (ActEventLogErr actEventLogErr)
		{
			if (( _EventLog.EnableRaisingEvents  ? true : (actEventLogErr.TimeGenerated < this.dtmTo.AddHours(_intUTCOffset))) &&
				((_strEventSrc	== "")	||	(actEventLogErr.Source.IndexOf(_strEventSrc)		!= -1))		&&
				((_strEventMsg	== "")	||	(actEventLogErr.Message.IndexOf(_strEventMsg)		!= -1))		&&
				((_str_vbErrNum == "")	||	(_str_vbErrNum == actEventLogErr.str_vbErrNum))					&&
				((_str_vbErrSrc == "")	||	(actEventLogErr.str_vbErrSrc.IndexOf(_str_vbErrSrc)	!= -1))		&&
				((_str_vbErrDesc== "")	||	(actEventLogErr.str_vbErrDesc.IndexOf(_str_vbErrDesc)	!= 1))	&&
				((_strEventType == "")	||	(_strEventType == actEventLogErr.EntryType))
				)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public Object[] arrEventSources{
			get
			{
				ArrayList arr_lstEventSources = new ArrayList();
				foreach (ActEventLogErr actEventLogErr in _arr_lstActEventLogErr)
				{
					if (arr_lstEventSources.IndexOf(actEventLogErr.Source ) == -1)
					{
						arr_lstEventSources.Add(actEventLogErr.Source);
					}
				}
				return arr_lstEventSources.ToArray() ;
			}
		}
		public Object[] arrEventMsg{
			get
			{
				ArrayList arr_lstEventMsg = new ArrayList();
				foreach (ActEventLogErr actEventLogErr in _arr_lstActEventLogErr)
				{
					if (arr_lstEventMsg.IndexOf(actEventLogErr.Message ) == -1)
					{
						arr_lstEventMsg.Add(actEventLogErr.Message);
					}
				}
				return arr_lstEventMsg.ToArray();
			}
		}
		public Object[] arrVBErrNum
		{
			get
			{
				ArrayList arr_lstVBErrNum= new ArrayList();
				foreach (ActEventLogErr actEventLogErr in _arr_lstActEventLogErr)
				{
					if (arr_lstVBErrNum.IndexOf(actEventLogErr.str_vbErrNum) == -1 && actEventLogErr.str_vbErrNum != "")
					{
						arr_lstVBErrNum.Add(actEventLogErr.str_vbErrNum);
					}
				}
				return arr_lstVBErrNum.ToArray();
			}
		}
		public Object[] arrVBErrSrc
		{
			get
			{
				ArrayList arr_lstVBErrSrc= new ArrayList();
				foreach (ActEventLogErr actEventLogErr in _arr_lstActEventLogErr)
				{
					if (arr_lstVBErrSrc.IndexOf(actEventLogErr.str_vbErrSrc) == -1 && actEventLogErr.str_vbErrSrc != "")
					{
						arr_lstVBErrSrc.Add(actEventLogErr.str_vbErrSrc);
					}
				}
				return arr_lstVBErrSrc.ToArray();
			}
		}

		public Object[] arrVBErrDesc
		{
			get
			{
				ArrayList arr_lstVBErrDesc= new ArrayList();
				foreach (ActEventLogErr actEventLogErr in _arr_lstActEventLogErr)
				{
					if (arr_lstVBErrDesc.IndexOf(actEventLogErr.str_vbErrDesc) == -1 && actEventLogErr.str_vbErrDesc != "")
					{
						arr_lstVBErrDesc.Add(actEventLogErr.str_vbErrDesc);
					}
				}
				return arr_lstVBErrDesc.ToArray();
			}
		}
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return (IEnumerator) new classEnum(this);
		}

		#endregion
		public class classEnum : IEnumerator
		{
			ActEventLogCache _actEventLogCache;
			private int index;
			public classEnum(ActEventLogCache actEventLogCache)
			{
				this._actEventLogCache = actEventLogCache;
				index = -1;
			}
			#region IEnumerator Members

			public void Reset()
			{
				index = -1;
			}

			public object Current
			{
				get
				{
					return _actEventLogCache[index];
				}
			}

			public bool MoveNext()
			{
				index++;
				if (index >= _actEventLogCache._arr_lstActEventLogErr.Count)
					return false;
				else
					return true;
			}

			#endregion
		}
	}

	public class ActEventLogErr
	{
		EventLogEntry _eventLogEntry;
		

		public ActEventLogErr(EventLogEntry eventLogEntry)
		{
			_eventLogEntry = eventLogEntry;
		}
		public EventLogEntry EventLogEntry{get{return _eventLogEntry;}}
		public string str_vbErrNum
		{
			get
			{
				Regex regex = new Regex(@"^\s*Number\s*[:|]\s*(\d*)\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				Match match = regex.Match(_eventLogEntry.Message);
				return match.Groups[1].Value;
			}
		}
		public string str_vbErrSrc
		{
			get
			{
				Regex regex = new Regex(@"^\s*SOURCE\s*[:|]\s*(.*)\r$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				Match match = regex.Match(_eventLogEntry.Message);
				return match.Groups[1].Value;
			}
		}
		public string str_vbErrDesc
		{
			get
			{
				Regex regex = new Regex(@"Desc.*\s*:\s*(.*)$", RegexOptions.IgnoreCase|RegexOptions.Singleline);
				Match match = regex.Match(_eventLogEntry.Message);
				return match.Groups[1].Value;
			}
		}
		
		public string strPreferedMsg
		{
			get
			{
				return (str_vbErrDesc != "" ) ? this.str_vbErrDesc : this.Message;
				
			}
		}

		public int EventID{get{return this._eventLogEntry.EventID;}}
		
		public int Index{get{return this._eventLogEntry.Index;}}
	
		public string Message{get{return this._eventLogEntry.Message;}}

		public string Source{get{return this._eventLogEntry.Source;}}

		public string EntryType{get{return this._eventLogEntry.EntryType.ToString() ;}}

		public DateTime TimeGenerated{get{return this._eventLogEntry.TimeGenerated.ToUniversalTime();}}

		public DateTime TimeWritten{get{return this._eventLogEntry.TimeWritten.ToUniversalTime();}}

	}
}
