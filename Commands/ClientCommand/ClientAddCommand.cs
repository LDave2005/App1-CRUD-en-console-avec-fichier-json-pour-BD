using App1.Core;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.ClientCommand
{
    public class ClientAddCommand : ISousCommand
    {
        public string Name => "create";
        public List<Parametre> parametres { get; } = new List<Parametre>
        {
            new Parametre("n","Nom du Client"),
            new Parametre("t", "Numero de telephone"),
        };

        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            ClientServices services = new ClientServices();

            string? name;
            if (args.ContainsKey("n"))
            {
                name = args["n"];
            }
            else
            {
                name = op.LireAvecReset("Nom du Client : ", out bool reset); if (reset) return;
            }
            int phone;
            if (args.ContainsKey("t"))
            {
                phone = Convert.ToInt32(args["t"]);
            }
            else
            {
                phone = Convert.ToInt32(op.LireAvecReset("Numero du Client : ", out bool reset)); if (reset) return;
            }

            try
            {
                services.AjouterClient(new Modeles.Client { nom = name, numeroTel = phone });
                op.Afficher("Client ajoute avec succes");
            }
            catch
            {
                op.Afficher("Erreur : veuillez rentrer des information correstion");
            }
        }

    }
}
