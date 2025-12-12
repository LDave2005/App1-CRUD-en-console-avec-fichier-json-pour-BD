using App1.Core;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.ProductCommand
{
    public class ProductModifyCommand : ISousCommand
    {
        public string Name => "modify";
        public List<Parametre> parametres { get; } = new List<Parametre>
        {
            new Parametre("id","Identitfiant du produit a modifier"),
            new Parametre("n","Nouveau nom du produit"),
            new Parametre("d","Nouvelle description"),
            new Parametre("p","Nouveau Prix"),
            new Parametre("qte","Nouvelle quantite"),
        };
        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            ProductServices services = new ProductServices();

            int id;
            if (args.ContainsKey("id"))
            {
                id = Convert.ToInt32(args["id"]);
            }
            else
            {
                id = Convert.ToInt32(op.LireAvecReset("Identifiant du produit a modifier : ", out bool reset)); if (reset) return;
            }
            string? name;
            if (args.ContainsKey("n"))
            {
                name = args["n"];
            }
            else
            {
                name = op.LireAvecReset("Nom du produit : ", out bool reset); if (reset) return;
            }
            string? description;
            if (args.ContainsKey("d"))
            {
                description = args["d"];
            }
            else
            {
                description = op.LireAvecReset("Description du produit : ", out bool reset); if (reset) return;
            }
                decimal price;
            if (args.ContainsKey("p"))
            {
                price = Convert.ToInt32(args["p"]);
            }
            else
            {
                price = Convert.ToInt32(op.LireAvecReset("Prix du produit : ", out bool reset)); if (reset) return;
            }
            int quantity;
            if(args.ContainsKey("qte"))
            {
                quantity = Convert.ToInt32(args["qte"]);
            }
            else
            {
                quantity = Convert.ToInt32(op.LireAvecReset("Quantite du produit : ", out bool reset)); if (reset) return;
            }

            var ok = services.ModifierProduit(id, new Modeles.Product { nom = name , description = description , prix = price , stock = quantity});
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
