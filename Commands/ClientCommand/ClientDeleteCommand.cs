using App1.Core;
using App1.Services;

namespace App1.Commands.ClientCommand
{
    public class ClientDeleteCommand : ISousCommand
    {
        public string Name => "delete";
        public List<Parametre> parametres { get; } = new List<Parametre>
        {
            new Parametre("id"),
        };
        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            ClientServices services = new ClientServices();

            int id;
            if(args.ContainsKey("id"))
            {
                id = int.Parse(args["id"]);
            }
            else
            {
                id = Convert.ToInt32(op.LireAvecReset("Identifiant du client a supprimer : ", out bool reset)); if (reset) return;
            }

            var ok = services.Supprimer(id);
            if (ok)
            {
                op.Afficher("Client Supprime");
            }
            else
            {
                op.Afficher("Echec de Suppression : Entrez un identifiant disponible");
            }
        }
    }
}
