using System;
using System.Collections;

namespace DateTimeTZ
{
	public class DateTimeTZ 
	{
		DateTime _DateTime;
		enum enuTimeZones
		{
			tzGreenwichMeanTime = 0,
			tzEniwetok			= 12,
			tzSamoa				= 11,
			tzHawaii			= 10,
			tzAlaska			= 9,
			tzPacificTime		= 8,
			tzMountainTime		= 7,
			tzCentralTime		= 6,
			tzEasternTime		= 5,
			tzAtlanticTime		= 4,
			tzBrazilia			= 3,
			tzMidAtlantic		= 2,
			tzAzores			= 1,
			tzRome				= 1,
			tzIsrael			= 2,
			tzMoscow			= 3,
			tzBaghdadIraq		= 4,
			tzNewDelhi			= 5,
			tzDhakar			= 6,
			tzBangkok			= 7,
			tzHongKong			= 8,
			tzTokyo				= 9,
			tzSydney			= 10,
			tzMagadan			= 11,
			tzWellington		= 12
		}
		public DateTimeTZ(DateTime dateTime)
		{
			_DateTime = dateTime;
		}
		public DateTime DateTime
		{
			get{return _DateTime;}
			set{_DateTime = value;}
		}
		/*
		public void convertTo(enuTimeZones TimeZone)
		{
		}
		*/

	}
}
