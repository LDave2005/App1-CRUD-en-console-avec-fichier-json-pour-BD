using App1.Core;
using App1.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.ClientCommand
{
    public class ClientCommand : ICommand2
    {
        public string Name => "client";

        public Dictionary<string, ISousCommand> SousCommands { get; } = new Dictionary<string, ISousCommand>();

        public ClientCommand()
        {
            SousCommands["list"] = new ClientListCommand();
            SousCommands["create"] = new ClientAddCommand();
            SousCommands["modify"] = new ClientModifiyCommand();
            SousCommands["delete"] = new ClientDeleteCommand();
        }

        public void Execute(Dictionary<string, string> args)
        {

        }
    }
}
