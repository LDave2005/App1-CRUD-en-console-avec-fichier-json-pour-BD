using App1.Core;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.ProductCommand
{
    public class ProductDeleteCommand : ISousCommand
    {
        public string Name => "delete";
        public List<Parametre> parametres { get; } = new List<Parametre>
        {
            new Parametre("id","Identifiant du produit a supprimer"),
        };
        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            ProductServices services = new ProductServices();

            int id;
            if (args.ContainsKey("id"))
            {
                id = int.Parse(args["id"]);
            }
            else
            {
                id = Convert.ToInt32(op.LireAvecReset("Identifiant du produit a supprimer : ", out bool reset)); if (reset) return;
            }

            var ok = services.SupprimerProduit(id);
            if (ok)
            {
                op.Afficher("produit Supprime");
            }
            else
            {
                op.Afficher("Echec de Suppression : Entrez un identifiant disponible");
            }
        }
    }
}
