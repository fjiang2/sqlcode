using System;
using System.Collections.Generic;

namespace Sys.Data
{
	class DbCmdManager
	{
		private const string UNKNOWN = "Unknown";

		private Dictionary<string, Func<string, IDbCmd>> commands = new Dictionary<string, Func<string, IDbCmd>>();
		private string activeName = UNKNOWN;

		public DbCmdManager()
		{
		}

		public Func<string, IDbCmd> ActiveCommand
		{
			get
			{
				if (commands.Count == 0)
					throw new InvalidOperationException($"DbCmd not registered");

				if (!commands.ContainsKey(activeName))
					throw new InvalidOperationException($"invalid DbCmd name:{activeName}");

				return commands[activeName];

			}
		}

		public void Activate(string name)
		{
			if (name == null)
				name = UNKNOWN;

			if (!commands.ContainsKey(name))
				throw new InvalidOperationException($"invalid DbCmd name:{name}");

			this.activeName = name;
		}

		public void Register(string name, Func<string, IDbCmd> cmd)
		{
			if (name == null)
				name = UNKNOWN;

			if (commands.ContainsKey(name))
				commands[name] = cmd;
			else
				commands.Add(name, cmd);
		}

		public void Unregister(string name)
		{
			if (commands.ContainsKey(name))
				commands.Remove(name);
		}
	}
}
