using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace Islander
{
	public class Configuration : IPluginConfiguration
	{
		private readonly DalamudPluginInterface pluginInterface;

		public Configuration(DalamudPluginInterface pi)
		{
			pluginInterface = pi;
		}

		#region Saved configuration values

		public string CoolText { get; set; }

		#endregion

		#region IPluginConfiguration Members

		int IPluginConfiguration.Version { get; set; }

		#endregion

		public void Save()
		{
			pluginInterface.SavePluginConfig(this);
		}
	}
}
