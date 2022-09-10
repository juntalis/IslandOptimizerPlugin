using System;
using System.Linq;
using System.Text;
using Dalamud.Plugin;
using ImGuiNET;
using Islander.Attributes;
using Islander.Data;
using Islander.Extensions;
using Islander.Structs;
using Islander.Utilities;

namespace Islander
{
	public class Plugin : IDalamudPlugin
	{
		private readonly PluginCommandManager<Plugin> commandManager;
		private readonly Configuration config;
		private readonly DalamudPluginInterface pluginInterface;
		// private readonly WindowSystem windowSystem;
		public CraftworksObjectManager CraftworksManager = null;

		private unsafe CraftworksObjectManager GetManager()
		{
			MJIManager.Initialize();
			if(CraftworksManager == null && Service.Data.IsDataReady && MJIManager.TryGetInstance(out MJIManager* mjiManager)) {
				CraftworksManager = new CraftworksObjectManager(mjiManager, Service.Data);
			}
			return CraftworksManager;
		}

		public Plugin(DalamudPluginInterface pi)
		{
			pluginInterface = pi;
			pluginInterface.Create<Service>();

			// Get or create a configuration object
			config = (Configuration)pluginInterface.GetPluginConfig() ?? pluginInterface.Create<Configuration>();

			// Initialize the UI
			// windowSystem = new WindowSystem(typeof(Plugin).AssemblyQualifiedName);
			// PluginWindow window = pluginInterface.Create<PluginWindow>();
			// if(window is not null) {
				// window.RegisterPlugin(this);
				// windowSystem.AddWindow(window);
			// }

			//pluginInterface.UiBuilder.Draw += windowSystem.Draw;

			// Load all of our commands
			commandManager = new PluginCommandManager<Plugin>(this, Service.Commands);
		}

		#region IDalamudPlugin Members

		public string Name => "Island Optimizer";

		#endregion
		
		[Flags]
		private enum CraftworkFields
		{
			None       = 0,
			Demand     = 1 << 0,
			Supply     = 1 << 1,
			Popularity = 1 << 2,
		}

		private CraftworkFields ParseCommandType(string args)
		{
			if(String.IsNullOrEmpty(args)) {
				return CraftworkFields.None;
			}

			Logger.Log($"args=\"{args}\"");
			CraftworkFields commandType = CraftworkFields.None;
			string cleanArgs = args.Trim().ToLower();
			foreach(string arg in cleanArgs.Split(' ')) {
				if(String.Equals(arg, "popularity", StringComparison.Ordinal)) {
					commandType |= CraftworkFields.Popularity;
				}
				if(String.Equals(arg, "supply", StringComparison.Ordinal)) {
					commandType |= CraftworkFields.Supply;
				}
				if(String.Equals(arg, "demand", StringComparison.Ordinal)) {
					commandType |= CraftworkFields.Demand;
				}
			}
			return commandType;
		}

		private void AppendRow(StringBuilder sb, CraftworkFields fields, string popularity, string supply, string demandShift)
		{
			string[] fieldValues = new string[] {
				String.Empty, String.Empty, String.Empty
			};
			if(fields.HasFlag(CraftworkFields.Popularity)) {
				fieldValues[0] = popularity;
			}
			if(fields.HasFlag(CraftworkFields.Supply)) {
				fieldValues[1] = supply;
			}
			if(fields.HasFlag(CraftworkFields.Demand)) {
				fieldValues[2] = demandShift;
			}
			sb.AppendLine(String.Join('\t', fieldValues.Where(s => !String.IsNullOrEmpty(s)).ToArray()));
		}

		private void BuildRow(StringBuilder sb, CraftworksObjectDetails craftworkObject, CraftworkFields fields)
		{
			if(craftworkObject == null) {
				AppendRow(sb, fields, String.Empty, String.Empty, String.Empty);
			} else {
				string popularity = craftworkObject.Popularity.Value.GetTextValue();
				string supply = craftworkObject.Supply.Value.GetTextValue();
				string demandShift = craftworkObject.DemandShift.GetTextValue();
				AppendRow(sb, fields, popularity, supply, demandShift);
				Logger.Log($"Appended craftwork #{craftworkObject.RowId}");
			}
		}

		[Command("/islanderptr")]
		[HelpMessage("/islanderptr")]
		public void IslanderCommandPtr(string command, string args)
		{
			var manager = GetManager();
			if(manager != null) {
				Logger.Log($"Manager.Ptr = 0x{manager.ManagerPtr:X}");
				Logger.Log($"Manager.SupplyDemandPtr = 0x{manager.SupplyDemandPtr:X}");
				Logger.Log($"Manager.CurrentPopularity =>");
				Logger.Log(manager.CurrentPopularity.PopularityTypeIds);
				Logger.Log($"Manager.SupplyAndDemandShiftBytes =>");
				Logger.Log(manager.SupplyAndDemandShiftBytes);
			}
		}

		[Command("/islander")]
		[HelpMessage("/islander <demand|supply|popularity>")]
		public void IslanderCommand(string command, string args)
		{
			CraftworksObjectManager manager = GetManager();
			if(manager == null) {
				Service.Chat.PrintError("Failed to access MJIManager.");
				return;
			}
			
			CraftworkFields fields = ParseCommandType(args);
			if(fields == CraftworkFields.None) {
				Service.Chat.PrintError($"Failed to parse command type from arguments: '{args}'!");
				return;
			}

			try {
				StringBuilder sb = new StringBuilder();
				foreach(CraftworksObjectDetails craftworkObject in manager.GetValidObjectsDetails()) {
					BuildRow(sb, craftworkObject, fields);
				}

				string textValue = sb.ToString();
				if(!String.IsNullOrEmpty(textValue)) {
					ImGui.SetClipboardText(textValue);
					Service.Chat.Print("Copied rows to clipboard.");
				}
			} catch(Exception exception) {
				Logger.LogError(exception, "GetValidObjectsDetails");
				Service.Chat.PrintError($"Failed during data lookup of '{args}'!");
			}
		}

		#region IDisposable Support

		protected virtual void Dispose(bool disposing)
		{
			if(!disposing) {
				return;
			}

			CraftworksManager?.Dispose();
			CraftworksManager = null;
			commandManager.Dispose();
			pluginInterface.SavePluginConfig(config);
			// pluginInterface.UiBuilder.Draw -= windowSystem.Draw;

			// windowSystem.RemoveAllWindows();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
