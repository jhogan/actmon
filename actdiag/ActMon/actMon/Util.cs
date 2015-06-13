using System;
using System.Data;

namespace DAC.WindowWatchers
{
	/// <summary>
	/// Summary description for Util.
	/// </summary>
	public class Util
	{
		private Util()
		{

		}
		public static DataRow[] Clone(DataRow[] drs)
		{
			int i = 0;
			DataRow[] clonedDrs = new DataRow[drs.Length];
			DataRow  clonedDr;
			foreach (DataRow dr in drs)
			{
				clonedDr = dr.Table.NewRow();
				foreach (DataColumn dc in dr.Table.Columns)
				{
					clonedDr[dc.ColumnName] = dr[dc];
				}
				clonedDrs[i++] = clonedDr;
			}
			return clonedDrs;
		}




	}
}
