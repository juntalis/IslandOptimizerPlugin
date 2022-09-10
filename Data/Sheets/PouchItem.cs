using System;
using Lumina;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Item = Lumina.Excel.GeneratedSheets.Item;

namespace Islander.Data.Sheets
{
	[Sheet("MJIItemPouch", 0x49c5eda3)]
	public class PouchItem : ExcelRow
	{
		public LazyRow<Item> Item { get; set; }
		public LazyRow<MJICropSeed> Seed { get; set; }
		public LazyRow<PouchItemCategory> Category { get; set; }

		public bool IsValid => Category.Value?.IsValid ?? false;

		public bool IsTimeGated => Category.Value?.IsTimeGated ?? false;

		public override void PopulateData(RowParser parser, GameData gameData, Language language)
		{
			base.PopulateData(parser, gameData, language);

			Item = new LazyRow<Item>(gameData, parser.ReadColumn<uint>(0), language);
			Category = new LazyRow<PouchItemCategory>(gameData, parser.ReadColumn<int>(1), language);
			Seed = new LazyRow<MJICropSeed>(gameData, (uint)parser.ReadColumn<byte>(2), language);
		}
	}
}
