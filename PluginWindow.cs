using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using Islander.Data;
using Islander.Utilities;

namespace Islander
{
	public class PluginWindow : Window
	{
		private Plugin plugin = null;
		private Vector2 GroupSize;

		public PluginWindow()
			: base("TemplateWindow")
		{
			IsOpen = true;
			Size = new Vector2(810, 520);
			GroupSize = new Vector2(500, 400);
			SizeCondition = ImGuiCond.FirstUseEver;
		}

		public void RegisterPlugin(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public override void Draw()
		{
			ImGui.Text("Hello, world!");
			if(plugin.CraftworksManager != null) {
				foreach(CraftworksObjectDetails details in plugin.CraftworksManager.GetValidObjectsDetails()) {
					if(details != null) {
						Util.ShowObject(details);
					} else {
						Logger.Log("details == null");
					}
				}
			}
		}
	}
}
