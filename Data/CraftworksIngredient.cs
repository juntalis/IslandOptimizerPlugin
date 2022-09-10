#nullable enable
using System;
using System.Collections.Generic;
using Islander.Data.Sheets;
using Lumina.Excel;

namespace Islander.Data
{
	public class CraftworksIngredient
	{
		public uint Amount { get; }

		public PouchItem Material { get; }

		public bool IsTimeGated => Material.IsTimeGated;

		private CraftworksIngredient(PouchItem material, ushort amount)
		{
			Material = material;
			Amount = Convert.ToUInt32(amount);
		}

		internal static CraftworksIngredient? ReadFrom(RowParser parser, int column, ExcelSheet<PouchItem> sheet)
		{
			ushort materialId = parser.ReadColumn<ushort>(column);
			ushort amount = parser.ReadColumn<ushort>(column + 1);
			if(amount == 0) { return null; }
			PouchItem? material = sheet.GetRow(materialId);
			if(material == null || !material.IsValid) {
				return null;
			}
			return new CraftworksIngredient(material, amount);
		}

		internal static IEnumerable<CraftworksIngredient> ReadAllFrom(RowParser parser, int column, int maxCount, ExcelSheet<PouchItem> sheet)
		{
			for(int i = 0; i < maxCount; i++) {
				int startColumn = column + i * 2;
				CraftworksIngredient? ingredient = ReadFrom(parser, startColumn, sheet);
				if(ingredient == null) {
					yield break;
				}
				yield return ingredient;
			}
		}
	}
}
