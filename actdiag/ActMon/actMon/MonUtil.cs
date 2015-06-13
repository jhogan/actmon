using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;



namespace DAC.WindowWatchers
{
	/// <summary>
	/// Summary description for MonUtil.
	/// </summary>
	public class Ping
	{
		#region Enums
		enum WSABaseErrors : int
		{
			INADDR_NONE = 0xFFFF,
			SOCKET_ERROR = 2,
			INVALID_SOCKET = 3,
		}
		
		#endregion

		#region C Declarations
		[DllImport("ICMP.dll", SetLastError=true)]
		private static extern Int32 IcmpCreateFile();
		[DllImport("ICMP.dll", SetLastError=true)]
		private static extern bool IcmpCloseHandle(Int32 HANDLE); 
		[DllImport("ICMP.dll", SetLastError=true)]
		private static extern bool IcmpSendEcho(Int32 HANDLE,
			Int32 DestinationAddress,
			string RequestData,
			UInt16 RequestSize,
			PIP_OPTION_INFORMATION pip_option_information,
			byte[] ReplyBuffer, //ICMP_ECHO_REPLY ReplyBuffer,
			Int32 ReplySize,
			Int32 Timeout);
		[DllImport("ws2_32.dll", SetLastError=true)]
		private static extern Int32 inet_addr(string cp);

		#endregion

		#region Members
		private int _count;
		private UInt16 _RequestSize = 32;
		private int _timeOut = 5000;
		private Byte _TTL = 255;
		private Byte _TOS;
		private string _strHost;
		
		public int		Count				{get{return _count;}			set{_count = value;}}
		public UInt16	RequestSize		{get{return _RequestSize;}		set{_RequestSize = value;}}
		public int		TimeOut				{get{return _timeOut;}			set{_timeOut = value;}}
		public string	RequestData			{get{return "".PadRight(this.RequestSize, 'A');}}
		public Byte		TTL					{get{return _TTL;}				set{_TTL = value;}}
		public Byte		TOS					{get{return _TOS;}				set{_TOS = value;}}
		private string	Host				{get{return _strHost;}			set{_strHost= value;}}
		private Int32	DestinationAddress	
		{
			get
			{
				this.Host = Dns.GetHostByName(this.Host).AddressList[0].ToString();
				return inet_addr(this.Host);
			}
		}

		#endregion
		public Ping()
		{
		}

		public void ping(IPAddress ipAddr)
		{
		}
		public void ping(string strHost)
		{
			this.Host = strHost;
			ping();
		}
		private void ping()	
		{
			//Int32 ReplySize= 28;
			Int32 IcmpHandle = IcmpCreateFile();
			PIP_OPTION_INFORMATION pip_option_information = new PIP_OPTION_INFORMATION();
			ICMP_ECHO_REPLY icmp_echo_reply = new ICMP_ECHO_REPLY();
			
			icmp_echo_reply.Address = new Byte[4];
			pip_option_information.OptionsData = "".PadRight(128,'\0');
			pip_option_information.Ttl = this.TTL;
			pip_option_information.Tos = this.TOS;
			//icmp_echo_reply.Options = new PIP_OPTION_INFORMATION();
			icmp_echo_reply.Options.OptionsData = "".PadRight(128,'\0');
			byte[] replyBuffer = new byte[1];
			if (IcmpHandle != (int) WSABaseErrors.INADDR_NONE)
			{
				if (!IcmpSendEcho(IcmpHandle,	
					this.DestinationAddress,
					this.RequestData,
					this.RequestSize,
					pip_option_information,
					replyBuffer, //out icmp_echo_reply,	
					replyBuffer.Length, //ReplySize,
					this.TimeOut))
				{
					Win32Exception win32Exception = new Win32Exception(Marshal.GetLastWin32Error());
					Trace.WriteLine (win32Exception.ToString() );
				}
			}
			else
			{
				//throw error
			}
		}
	}

	#region API Structs
	internal struct PIP_OPTION_INFORMATION
	{
		public Byte Ttl;
		public Byte Tos;
		public Byte Flags;
		public Int32 OptionsSize;
		public string OptionsData;
	}
	internal struct ICMP_ECHO_REPLY 
	{
		public Byte[] Address;
		public UInt32 Status;
		public UInt32 RoundTripTime;
		public UInt16 DataSize;
		public UInt16 Reserved;
		public Int32 Data;
		public PIP_OPTION_INFORMATION Options;
	} 
	#endregion
	public class DiskDrive
	{
		/*TODO: It would be nice if we could use a drive letter in the contructor for this class.
		  We would need to iterate through all of the shared drives given for the computer name and
		 found out if any of these shares match the drive letter given to the class. Don't know what
		 this would take though.*/
		 
		public enum _quantifier: int
		{
			kilo = 1,
			mega = 2,
			giga = 3,
			tera = 4,
			peta = 5,
			exa =  6,
			zetta = 7,
			yotta = 8
		}

		private Int64 _FreeBytesAvailable; 
		private Int64 _TotalNumberOfBytes; 
		private Int64 _TotalNumberOfFreeBytes;
		private string _Path;
		[DllImport("kernel32.dll", SetLastError=true)]
		static extern Boolean GetDiskFreeSpaceEx(String lpDirectoryName,
			out Int64 lpFreeBytesAvailable,
			out Int64 lpTotalNumberOfBytes,
			out Int64 lpTotalNumberOfFreeBytes);

		public DiskDrive(string Path)
		{
			if (!Path.EndsWith(@"\"))
			{
				Path = Path + @"\";
			}
			_Path = Path;

			GetDiskFreeSpaceEx(Path);
		}
		public void Refresh()
		{
			GetDiskFreeSpaceEx(_Path);
		}


		private void GetDiskFreeSpaceEx(string DirectoryName)
		{
			if (!GetDiskFreeSpaceEx((String) DirectoryName,
				out _FreeBytesAvailable,
				out _TotalNumberOfBytes,
				out _TotalNumberOfFreeBytes))
			{
				Trace.WriteLine("GetDiskFreeSpaceEx Failed");
			}

		}



		

		//FreeBytesAvailable
		public Int64 FreeBytesAvailable
		{get{return _FreeBytesAvailable;}}

		public float FreeKBytesAvailable
		{
			get{return ConvertBytes(_FreeBytesAvailable, _quantifier.kilo);	}
		}

		public float FreeMBytesAvailable
		{
			get	{return ConvertBytes(_FreeBytesAvailable, _quantifier.mega);}
		}
		public float FreeGBytesAvailable
		{
			get	{return ConvertBytes(_FreeBytesAvailable, _quantifier.giga);}
		}


		// TotalNumberOfFreeBytes
		
		public Int64 TotalNumberOfFreeBytes
		{get{return _TotalNumberOfFreeBytes;}}
		
		public float TotalNumberOfFreeKBytes
		{
			get	{return ConvertBytes(_TotalNumberOfFreeBytes, _quantifier.kilo);}
		}	
		public float TotalNumberOfFreeMBytes
		{
			get {return ConvertBytes(_TotalNumberOfFreeBytes, _quantifier.mega);}
		}
		public float TotalNumberOfFreeGBytes
		{ 
			get{return ConvertBytes(_TotalNumberOfFreeBytes, _quantifier.giga);			}
		}


		// TotalNumberOfBytes
		
		public Int64 TotalNumberOfBytes
		{get{return _TotalNumberOfBytes;}}

		public float TotalNumberOfKBytes
		{
			get	{return ConvertBytes(_TotalNumberOfBytes, _quantifier.kilo);}
		}
		public float TotalNumberOfMBytes
		{
			get{return ConvertBytes(_TotalNumberOfBytes, _quantifier.mega);	}
		}
		public float TotalNumberOfGBytes
		{
			get	{return ConvertBytes(_TotalNumberOfBytes, _quantifier.giga);}
		}
		
		
		private float ConvertBytes(Int64 Bytes, _quantifier Quantifier)
		{
			return (float)((double) Bytes / Math.Pow(1024, (double) Quantifier));
		}
	}
	public class EventLogEntry
	{
		private System.Diagnostics.EventLogEntry  _eventLogEntry;
		protected DataRow _dr;
		public EventLogEntry(System.Diagnostics.EventLogEntry eventLogEntry)
		{
			_eventLogEntry = eventLogEntry;
		}
		public System.Diagnostics.EventLogEntry Entry
		{get{return _eventLogEntry;}}

		public virtual void Save(DataTable dt)
		{
			DataRow dr = dt.NewRow();
			this.Fill(dr);
			dr["TypeParsedAs"] = this.GetType().Name;
			dt.Rows.Add(dr);
		}
		public virtual DataRow getDR(DataTable dt)
		{
			if (_dr == null)
			{
				_dr = dt.NewRow();
				this.Fill(_dr);
				_dr["TypeParsedAs"] = this.GetType().Name;
			}
			return _dr;
		}
		protected void Fill(DataRow dr)
		{
			dr["elCategory"]			= _eventLogEntry.Category;
			dr["elCategoryNumber"]		= _eventLogEntry.CategoryNumber;
			dr["elEntryType"]			= _eventLogEntry.EntryType;
			dr["elSource"]				= _eventLogEntry.Source;
			dr["elTimeGenerated"]		= _eventLogEntry.TimeGenerated;
			dr["elUserName"]			= _eventLogEntry.UserName;
			dr["elMessage"]				= _eventLogEntry.Message;
		}

		public static EventLogEntry GetEventLogEntry(System.Diagnostics.EventLogEntry eventLogEntry)
		{
			//Regex vbRegex = new Regex(@".*CALL STACK.*NUMBER.*SOURCE.*DESCRIPTION.*", RegexOptions.IgnoreCase);
			Regex vbRegex = new Regex(@".*CALL STACK.*NUMBER.*", RegexOptions.IgnoreCase | RegexOptions.Singleline );
			
			if (vbRegex.IsMatch(eventLogEntry.Message))
			{
				return (DAC.WindowWatchers.EventLogEntry) new vbEventLogEntry(eventLogEntry);
			}

			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(eventLogEntry.Message);
			if (xmlDocument.FirstChild.FirstChild.OuterXml.ToUpper() == "ACALOG")
			{
				return (DAC.WindowWatchers.EventLogEntry) new acaEventLogEntry(eventLogEntry);
			}

			return new DAC.WindowWatchers.EventLogEntry(eventLogEntry);
		}
	}

	public class vbEventLogEntry : EventLogEntry
	{
		public vbEventLogEntry(System.Diagnostics.EventLogEntry eventLogEntry) : base(eventLogEntry) {}

		public string CallStack
		{
			get
			{
				Regex regex = new Regex(@"^\s*CALL STACK\s*[:|]\s*(.*)\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				string s = base.Entry.Message;
				Match match = regex.Match(s);
				return match.Groups[1].Value;
			}
		}

		public string Number
		{
			get
			{
				Regex regex = new Regex(@"^\s*Number\s*[:|]\s*([-|]\s*\d*)\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				string s = base.Entry.Message;
				Match match = regex.Match(s);
				return match.Groups[1].Value;
			}
		}
		public string Source
		{
			get
			{
				Regex regex = new Regex(@"^\s*SOURCE\s*[:|]\s*(.*)\r$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				Match match = regex.Match(base.Entry.Message);
				return match.Groups[1].Value;
			}
		}
		public string Description
		{
			get
			{
				Regex regex = new Regex(@"DESC(RIPTION|)\s*[:|]\s*(.*)$", RegexOptions.IgnoreCase |RegexOptions.Singleline);
				Match match = regex.Match(base.Entry.Message);
				return match.Groups[2].Value;
			}
		}

		public override DataRow getDR(DataTable dt)
		{
			if (_dr == null)
			{
				_dr = dt.NewRow();
				this.Fill(_dr);
				_dr["TypeParsedAs"] = this.GetType().Name;
			}
			return _dr;
		}
		protected new void Fill(DataRow dr)
		{
			dr["vbCallStack"] = this.CallStack;
			dr["vbNumber"] = this.Number;
			dr["vbSource"] = this.Source;
			dr["vbDescription"] = this.Description;
			base.Fill(dr);
		}

	}
	public class acaEventLogEntry : EventLogEntry
	{
		public acaEventLogEntry(System.Diagnostics.EventLogEntry eventLogEntry) : base(eventLogEntry) {}
		public string LogCategory
		{
			get
			{return "";}
		}	
		public string Header
		{
			get
			{return "";}
		}
		public string EventID
		{
			get
			{return "";}
		}
		public string Body
		{
			get
			{return "";}
		}
		public string Severity
		{
			get
			{return "";}
		}
		public string DateTime
		{
			get
			{return "";}
		}
		public new DataRow getDR(DataTable dt)
		{
			if (_dr == null)
			{
				DataRow dr = dt.NewRow();
				this.Fill(dr);
				dr["TypeParsedAs"] = this.GetType().Name;
			}
			return _dr;
		}
		protected new void Fill(DataRow dr)
		{
			dr["acaLogCategory"] = this.LogCategory;
			dr["acaHeader"] = this.Header;
			dr["acaEventID"] = this.EventID;
			dr["acaBody"] = this.Body;
			dr["acaSeverity"] = this.Severity;
			dr["acaDateTime"] = this.DateTime;
			base.Fill(dr);
		}
	}
	public class DistList
	{
		int _id;
		string _emailAddresses;
		string _desc;
		public DistList(int id, Conf conf)
		{
			DataRow[] drs = conf.DistList.Select("DistListID = " + id);
		    this.EmailAddresses = drs[0]["EmailAddresses"].ToString();
			this.Desc = drs[0]["Desc"].ToString();
			this._id = id;
		}
		
		public int ID 
		{
			get { return _id;}
		}
		public string EmailAddresses 
		{
			get { return _emailAddresses;}
			set {_emailAddresses = value;}
		}
		public string Desc 
		{
			get { return _desc;}
			set {_desc = value;}
		}
	}
	public class Alerts
	{
		public ArrayList _alerts = new ArrayList();
		int _max;
		public delegate void OnAlertAddedHandler(Alert alert);
		public event OnAlertAddedHandler OnAlertAdded;
		public Alerts()
		{
			_max = Convert.ToInt32(KVP.GetInstance().Value("MaxAlerts"));
		}
		public int Count
		{
			get
			{
				return this._alerts.Count;
			}
				
		}

		public int Max
		{
			get { return _max;}
			set {_max = value;}
		}

		public void add(Alert alert)
		{
			if (this.Max > 0)
			{
				if (this._alerts.Count == this.Max)
				{
					Trace.WriteLine ("_alerts.Count = " + _alerts.Count);
					_alerts.RemoveAt(_alerts.Count - 1);
				}
			}
			_alerts.Add(alert);
			OnAlertAdded(alert);
		}
	}
	public class Alert
	{
		DateTime _timeCreated;
		string _emailAddresses, _monName, _expression;
		long _duration; DataRow[] _exeededDataRows;
		DataColumn _timeStampColumn, _definingDataColumn;

		public Alert(string emailAddresses, DateTime timeCreated, 
							string monName, string expression,
							long duration, DataRow[] exeededDataRows, 
							DataColumn timeStampColumn, DataColumn definingDataColumn)
		{
			_timeCreated = DateTime.Now;
			_emailAddresses = emailAddresses;	_timeCreated = timeCreated;
			_monName = monName;					
			_expression = expression;			_duration = duration;
			_exeededDataRows = exeededDataRows;	_timeStampColumn = timeStampColumn;
			_definingDataColumn = definingDataColumn;
		}

		public string EmailAddresses
			{get{return _emailAddresses;}}

		public DateTime TimeCreated
			{get{return _timeCreated;}}

		public string MonName
			{get{return _monName;}}

		public string Expression
			{get{return _expression;}}

		public long Duration
			{get{return _duration;}}

		public DataRow[] ExeededDataRows
			{get{return _exeededDataRows;}}

		public DataColumn TimeStampColumn
			{get{return _timeStampColumn;}}

		public DataColumn DefiningDataColumn
			{get{return _definingDataColumn;}}

	}
	public class KVP
	{
		private static Conf _conf;
		private static KVP _instance;
		private static object syncRoot = new object();
		private KVP()
		{}
		public static Conf Conf
		{
			set
			{
				_conf = value;
			}
		}
		public static KVP GetInstance()
		{
			if (_instance == null)
			{
				lock (syncRoot)
				{
					if (_instance == null)
					{
						_instance = new KVP();
					}
				}
			}
			return _instance;
		}
		public string Value(string key)
		{
			lock(syncRoot)
			{
				Conf.KVPRow[] dr = (Conf.KVPRow[]) _conf.KVP.Select("key = '" + key + "'");
				
				if (dr.Length > 0)
				{
					return dr[0].Value;
				}
				else
				{
					throw new Exception("A value was sought for a non-existent key in the KVP list.");
				}

			}
		}
	}
}
