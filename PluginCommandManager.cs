using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Game.Command;
using Islander.Attributes;
using static Dalamud.Game.Command.CommandInfo;

namespace Islander
{
	public class PluginCommandManager<THost> : IDisposable
	{
		private readonly CommandManager commandManager;
		private readonly THost host;
		private readonly (string, CommandInfo)[] pluginCommands;

		public PluginCommandManager(THost host, CommandManager commandManager)
		{
			this.host = host;
			this.commandManager = commandManager;

			pluginCommands = host.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
								 .Where(method => method.GetCustomAttribute<CommandAttribute>() != null)
								 .SelectMany(GetCommandInfoTuple)
								 .ToArray();

			AddCommandHandlers();
		}

		#region IDisposable Members

		public void Dispose()
		{
			RemoveCommandHandlers();
			GC.SuppressFinalize(this);
		}

		#endregion

		private void AddCommandHandlers()
		{
			foreach ((string command, CommandInfo commandInfo) in pluginCommands)
			{
				commandManager.AddHandler(command, commandInfo);
			}
		}

		private void RemoveCommandHandlers()
		{
			foreach ((string command, var _) in pluginCommands)
			{
				commandManager.RemoveHandler(command);
			}
		}

		private IEnumerable<(string, CommandInfo)> GetCommandInfoTuple(MethodInfo method)
		{
			HandlerDelegate handlerDelegate = (HandlerDelegate)Delegate.CreateDelegate(typeof(HandlerDelegate), host, method);

			CommandAttribute command = handlerDelegate.Method.GetCustomAttribute<CommandAttribute>();
			AliasesAttribute aliases = handlerDelegate.Method.GetCustomAttribute<AliasesAttribute>();
			HelpMessageAttribute helpMessage = handlerDelegate.Method.GetCustomAttribute<HelpMessageAttribute>();
			DoNotShowInHelpAttribute doNotShowInHelp = handlerDelegate.Method.GetCustomAttribute<DoNotShowInHelpAttribute>();

			CommandInfo commandInfo = new CommandInfo(handlerDelegate) {
				HelpMessage = helpMessage?.HelpMessage ?? string.Empty,
				ShowInHelp = doNotShowInHelp == null
			};

			// Create list of tuples that will be filled with one tuple per alias, in addition to the base command tuple.
			List<(string, CommandInfo)> commandInfoTuples = new List<(string, CommandInfo)> { (command!.Command, commandInfo) };
			if (aliases != null)
			{
				foreach (string alias in aliases.Aliases)
				{
					commandInfoTuples.Add((alias, commandInfo));
				}
			}

			return commandInfoTuples;
		}
	}
}
