using System;
using System.Collections.Generic;
using System.Linq;
using Islander.Data.Sheets;
using Lumina.Excel;

namespace Islander.Extensions
{
	public static class CraftworksSheetExtensions
	{
		public static IEnumerable<CraftworksObject> GetValidRows(this ExcelSheet<CraftworksObject> self)
		{
			return self.Where(row => row.IsValid);
		}
	}
}
