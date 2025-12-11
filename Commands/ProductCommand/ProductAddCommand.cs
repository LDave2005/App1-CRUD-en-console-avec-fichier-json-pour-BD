
using App1.Core;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.ProductCommand
{
    public class ProductAddCommand : ISousCommand
    {
        public string Name => "create";
        public List<Parametre> parametres { get; } = new List<Parametre>
        {
            new Parametre("n"),
            new Parametre("d"),
            new Parametre("p"),
            new Parametre("qte"),
        };

        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            ProductServices services = new ProductServices();

            string? name;
            if(args.ContainsKey("n"))
            {
                name = args["n"];
            }
            else
            {
                name = op.LireAvecReset("Nom du produit: ", out bool reset);
                if (reset) return;
            }
            string? description;
            if (args.ContainsKey("d"))
            {
                description = args["d"];
            }
            else
            {
                description = op.LireAvecReset("Description du produit: ", out bool reset);
                if (reset) return;
            }
            decimal price;
            if(args.ContainsKey("p"))
            {
                price = Convert.ToDecimal(args["p"]);
            }
            else
            {
                price = Convert.ToDecimal(op.LireAvecReset("Prix du produit: ", out bool reset)); if(reset) return;
            }
            int quantity;
            if(args.ContainsKey("qte"))
            {
                quantity = Convert.ToInt32(args["qte"]);
            }
            else
            {
                quantity = Convert.ToInt32(op.LireAvecReset("Quantité du produit: ", out bool reset)); if(reset) return;
            }
            try
            {
                services.AjouterProduit(new Modeles.Product { nom = name , description = description , prix = price , stock = quantity});
                op.Afficher("Produit Ajoute");
            }
            catch
            {
                op.Afficher("Erreur lors de l'ajout du produit");
            }
        }

    }
}
