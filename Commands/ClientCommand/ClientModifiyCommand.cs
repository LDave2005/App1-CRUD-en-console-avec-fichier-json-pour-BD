using App1.Core;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.ClientCommand
{
    public class ClientModifiyCommand : ISousCommand
    {
        public string Name => "modify";
        public List<Parametre> parametres { get; } = new List<Parametre>
        {
            new Parametre("id","Identifiant du client"),
            new Parametre("n", "Nouveau Nom"),
            new Parametre("t", "Nouveau de telephone"),
        };
        public void Execute(Dictionary<string,string> args)
        {
            Traitement op = new Traitement();
            ClientServices services = new ClientServices();

            int id;
            if(args.ContainsKey("id"))
            {
                id = Convert.ToInt32(args["id"]);
            }
            else
            {
                id = Convert.ToInt32(op.LireAvecReset("Identifiant du client a modifier : ", out bool reset)); if (reset) return;
            }
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

            var ok = services.ModifierClient(id, new Modeles.Client { nom = name, numeroTel = phone });
            if (ok)
            {
                op.Afficher("Client modifie avec succes");
            }
            else
            {
                op.Afficher("Echec de la modification du client : Entrez un identifiants et/ou des donnees correctes");
            }
        }
    }
}
