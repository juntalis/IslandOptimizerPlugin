#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Excel;

namespace Islander.Data
{
	public class ExcelRowStore<TRow>
		where TRow : ExcelRow
	{
		private readonly Dictionary<UInt32, TRow> rowsDict;

		public ExcelRowStore(ExcelSheet<TRow> sheet)
			: this(sheet, null)
		{}

		public ExcelRowStore(ExcelSheet<TRow> sheet, Func<TRow, bool>? filter)
		{
			filter ??= new Func<TRow, bool>(_ => true);
			rowsDict = sheet.Where(filter).ToDictionary(row => row.RowId, row => row);
		}
		
		public TRow this[byte rowId] => rowsDict[rowId];

		public TRow this[ushort rowId] => rowsDict[rowId];

		public TRow this[uint rowId] => rowsDict[rowId];

		public bool TryGetValue(byte rowId, out TRow? row) => rowsDict.TryGetValue(rowId, out row);

		public bool TryGetValue(ushort rowId, out TRow? row) => rowsDict.TryGetValue(rowId, out row);

		public bool TryGetValue(uint rowId, out TRow? row) => rowsDict.TryGetValue(rowId, out row);
	}
}
