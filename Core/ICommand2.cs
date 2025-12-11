using System.Collections.Generic;

namespace App1.Core
{
    public interface ICommand2
    {
        string Name { get; }
        Dictionary<string, ISousCommand> SousCommands { get; }

        void Execute(Dictionary<string, string> args) ;
    }
}
