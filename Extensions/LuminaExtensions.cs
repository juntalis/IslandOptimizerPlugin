#nullable enable
using System;
using System.Collections.Generic;
using Lumina.Excel;

namespace Islander.Extensions
{
	/// <summary>
	/// Class extensions to Lumina
	/// </summary>
	public static class LuminaExtensions
	{
		public static T? GetRow<T>(this ExcelSheet<T> self, byte rowId)
			where T : ExcelRow
		{
			return self.GetRow(Convert.ToUInt32(rowId));
		}

		public static T? GetRow<T>(this ExcelSheet<T> self, ushort rowId)
			where T : ExcelRow
		{
			return self.GetRow(Convert.ToUInt32(rowId));
		}
		
		public static T? GetNonZeroRow<T>(this ExcelSheet<T> self, ushort rowId)
			where T : ExcelRow
		{
			return rowId == 0 ? null : self.GetRow(Convert.ToUInt32(rowId));
		}
		
		public static T? GetNonZeroRow<T>(this ExcelSheet<T> self, uint rowId)
			where T : ExcelRow
		{
			return rowId == 0 ? null : self.GetRow(rowId);
		}

		public static IEnumerable<T?> GetRows<T>(this ExcelSheet<T> self, params ushort[] rowIds)
			where T : ExcelRow
		{
			foreach(ushort rowId in rowIds) {
				yield return self.GetRow(rowId);
			}
		}

		public static IEnumerable<T?> GetRows<T>(this ExcelSheet<T> self, params uint[] rowIds)
			where T : ExcelRow
		{
			foreach(uint rowId in rowIds) {
				yield return self.GetRow(rowId);
			}
		}

		public static IEnumerable<T> GetNonZeroRows<T>(this ExcelSheet<T> self, params ushort[] rowIds)
			where T : ExcelRow
		{
			foreach(ushort rowId in rowIds) {
				T? row = self.GetNonZeroRow(rowId);
				if(row != null) {
					yield return row;
				}
			}
		}

		public static IEnumerable<T> GetNonZeroRows<T>(this ExcelSheet<T> self, params uint[] rowIds)
			where T : ExcelRow
		{
			foreach(uint rowId in rowIds) {
				T? row = self.GetNonZeroRow(rowId);
				if(row != null) {
					yield return row;
				}
			}
		}
		
	}
}

