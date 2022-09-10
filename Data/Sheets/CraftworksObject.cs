using System;
using System.Collections.Generic;
using System.Linq;
using Islander.Extensions;
using Islander.Utilities;
using Lumina;
using Lumina.Data;
using Lumina.Excel;
using Item = Lumina.Excel.GeneratedSheets.Item;

namespace Islander.Data.Sheets
{
	[Sheet("MJICraftworksObject", 0x61B63F0AU)]
	public class CraftworksObject : ExcelRow
	{
		private int timeGatedCount = -1;

		public LazyRow<Item> Item { get; set; }

		public string Name => Item.Value?.Name ?? "null";

		public List<CraftworksObjectTheme> Themes { get; set; }

		public ushort Unknown3 { get; set; }

		public List<CraftworksIngredient> Ingredients { get; set; }

		public ushort ResultAmount { get; set; }

		public ushort Hours { get; set; }

		public ushort Value { get; set; }
		
		public bool IsValid => Item.Row != 0;

		public bool HasMatchingTheme(CraftworksObject other)
		{
			return Themes.Any(other.Themes.Contains);
		}

		public int CountTimeGatedIngredients()
		{
			if(timeGatedCount == -1) {
				timeGatedCount = Ingredients.Where(ingredient => ingredient.IsTimeGated).Sum(
					ingredient => (int)ingredient.Amount
				);
			}
			return timeGatedCount;
		}

		private void ClearData()
		{
			Themes = null;
			Unknown3 = 0;
			Ingredients = null;
			ResultAmount = 0;
			Hours = 0;
			Value = 0;
		}

		private void PopulateThemes(RowParser parser, GameData gameData, Language language)
		{	
			// Initialize our list
			if(Themes == null) {
				Themes = new List<CraftworksObjectTheme>();
			} else {
				Themes.Clear();
			}

			// Early return for 2 blank themes.
			ushort theme1 = parser.ReadColumn<ushort>(1);
			ushort theme2 = parser.ReadColumn<ushort>(2);
			if(theme1 == 0 && theme2 == 0) {
				return;
			}

			// Initialize our sheet and populate the list
			ExcelSheet<CraftworksObjectTheme> themeSheet = gameData.GetExcelSheet<CraftworksObjectTheme>(language);
			if(themeSheet != null) {
				Themes.AddRange(themeSheet.GetNonZeroRows(theme1, theme2));
			} else {
				Logger.LogError("Unable to load ExcelSheet<MJICraftworksObjectTheme>!");
			}
		}

		private void PopulateIngredients(RowParser parser, GameData gameData, Language language)
		{
			// Initialize our list
			if(Ingredients == null) {
				Ingredients = new List<CraftworksIngredient>();
			} else {
				Ingredients.Clear();
			}

			// Initialize our sheet and populate the list
			ExcelSheet<PouchItem> pouchSheet = gameData.GetExcelSheet<PouchItem>(language);
			if(pouchSheet != null) {
				Ingredients.AddRange(CraftworksIngredient.ReadAllFrom(parser, 4, 4, pouchSheet));
			} else {
				Logger.LogError("Unable to load ExcelSheet<MJIItemPouch>!");
			}
		}

		public override void PopulateData(RowParser parser, GameData gameData, Language language)
		{
			base.PopulateData(parser, gameData, language);
			Logger.Log($"{GetDefaultColumnValue()}");
			uint itemId = Convert.ToUInt32(parser.ReadColumn<ushort>(0));
			Item = new LazyRow<Item>(gameData, itemId, language);
			if(itemId == 0) {
				ClearData();
			} else {
				// Columns 1-2
				PopulateThemes(parser, gameData, language);
				Unknown3 = parser.ReadColumn<ushort>(3);
				// Columns 4-11
				PopulateIngredients(parser, gameData, language);
				ResultAmount = parser.ReadColumn<ushort>(12);
				Hours = parser.ReadColumn<ushort>(13);
				Value = parser.ReadColumn<ushort>(14);
			}
		}
		
	}
}
