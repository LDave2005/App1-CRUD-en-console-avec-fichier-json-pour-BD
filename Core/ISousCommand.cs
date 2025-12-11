using System.Collections.Generic;


namespace App1.Core
{
    public interface ISousCommand
    {
        string Name { get; }
        List<Parametre> parametres { get; }

        void Execute(Dictionary<string, string> args);
    }
}
