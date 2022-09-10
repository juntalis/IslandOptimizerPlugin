using System;
using System.Text;
using Dalamud.Logging;

namespace Islander.Utilities
{
	internal static class Logger
	{
		private static string Format(string msg)
		{
			return msg;
		}

		internal static void Log(byte[] msg)
		{
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < msg.Length; i++) {
				if(i % 16 == 0) {
					sb.AppendLine();
				} else {
					sb.Append(" ");
				}
				sb.Append($"{msg[i]:X2}");
			}
			Log(sb.ToString());
		}

		internal static void Log(string msg)
		{
			PluginLog.Log(Format(msg), Array.Empty<object>());
		}

		internal static void LogWarning(string msg)
		{
			PluginLog.LogWarning(Format(msg), Array.Empty<object>());
		}

		internal static void LogError(string msg)
		{
			PluginLog.LogError(Format(msg), Array.Empty<object>());
		}

		internal static void LogError(Exception ex, string msg)
		{
			PluginLog.LogError(ex, Format(msg), Array.Empty<object>());
		}
	}
}
