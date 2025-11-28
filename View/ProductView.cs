using App1.DAL;
using App1.Services;
using App1.Modeles;

namespace App1.View
{
    public class ProductView
    {
        public DataStore DataStore { get; set; }

        public ProductView()
        {
            DataStore = new DataStore();
        }

        public void AfficherProduitsTable()
        {
            ProductServices product = new ProductServices();
            var produits = product.LireProduits();

            int idW = 2;
            int nomW = 3;
            int descW = 11;
            int prixW = 8;
            int stockW = 5;

            foreach (var p in produits)
            {
                if (p.id.ToString().Length > idW) idW = p.id.ToString().Length;
                if ((p.nom ?? "").Length > nomW) nomW = (p.nom ?? "").Length;
                if ((p.description ?? "").Length > descW) descW = (p.description ?? "").Length;
                if (p.prix.ToString("F2").Length > prixW) prixW = p.prix.ToString("F2").Length;
                if (p.stock.ToString().Length > stockW) stockW = p.stock.ToString().Length;
            }

            string sep = "+" + new string('-', idW + 2) + "+" + new string('-', nomW + 2) + "+" +
                         new string('-', descW + 2) + "+" + new string('-', prixW + 2) + "+" +
                         new string('-', stockW + 2) + "+";

            Console.WriteLine(sep);
            Console.WriteLine($"| {"ID".PadRight(idW)} | {"Nom".PadRight(nomW)} | {"Description".PadRight(descW)} | {"Prix".PadRight(prixW)} | {"Stock".PadRight(stockW)} |");
            Console.WriteLine(sep);

            foreach (var p in produits)
            {
                string idS = p.id.ToString().PadRight(idW);
                string nom = (p.nom ?? "").PadRight(nomW);
                string desc = (p.description ?? "").PadRight(descW);
                string prix = p.prix.ToString("F2").PadRight(prixW);
                string stock = p.stock.ToString().PadRight(stockW);

                Console.WriteLine($"| {idS} | {nom} | {desc} | {prix} | {stock} |");
            }
            Console.WriteLine(sep);
        }

        public void MenuProduct()
        {
            bool reset;
            bool inProducts = true;
            while (inProducts)
            {
                Console.Clear();
                Console.WriteLine("=== Gestion des produits (admin) ===");
                Console.WriteLine("1) Afficher tous les produits");
                Console.WriteLine("2) Ajouter un produit");
                Console.WriteLine("3) Modifier un produit");
                Console.WriteLine("4) Supprimer un produit");
                Console.WriteLine("5) Retour");
                Console.Write("Choix: ");
                var pc = Console.ReadLine();
                ProductServices pr = new ProductServices();

                switch (pc)
                {
                    case "1":
                        AfficherProduitsTable();
                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                        Console.ReadLine();
                        break;
                    case "2":
                        Traitement op = new Traitement();
                        var pnom = op.LireAvecReset("Nom du Produit: ", out reset);
                        if (reset) break; // retour au menu principal
                        var pdesc = op.LireAvecReset("Description du produit: ", out reset);
                        if (reset) break; // retour au menu principal
                                          //Console.Write("Prix: ");
                        if (!decimal.TryParse(op.LireAvecReset("Prix: ", out reset), out decimal pprix)) pprix = 0m;
                        if (reset) break;
                        //Console.Write("Stock: ");
                        if (!int.TryParse(op.LireAvecReset("Stock: ", out reset), out int pstock)) pstock = 0;
                        if (reset) break;
                        // ici on suppose l'admin déjà authentifié : passer user.email/user.mot de passe si vous appelez wrapper
                        pr.AjouterProduit(new Product { nom = pnom, description = pdesc, prix = pprix, stock = pstock });
                        Console.WriteLine("Produit ajouté.");
                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                        Console.ReadLine();
                        break;
                    case "3":
                        Console.Write("ID produit: ");
                        if (!int.TryParse(Console.ReadLine(), out int mid)) { Console.WriteLine("ID invalide"); Console.ReadLine(); break; }
                        Console.Write("Nouveau nom: "); var mnom = Console.ReadLine() ?? "";
                        Console.Write("Nouvelle description: "); var mdesc = Console.ReadLine() ?? "";
                        Console.Write("Nouveau prix: ");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal mprix)) mprix = 0m;
                        Console.Write("Nouveau stock: ");
                        if (!int.TryParse(Console.ReadLine(), out int mstock)) mstock = 0;
                        if (pr.ModifierProduit(mid, new Product { nom = mnom, description = mdesc, prix = mprix, stock = mstock }))
                            Console.WriteLine("Produit modifié.");
                        else
                            Console.WriteLine("Produit non trouvé.");
                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                        Console.ReadLine();
                        break;
                    case "4":
                        Console.Write("ID produit à supprimer: ");
                        if (!int.TryParse(Console.ReadLine(), out int did)) { Console.WriteLine("ID invalide"); Console.ReadLine(); break; }
                        if (pr.SupprimerProduit(did))
                            Console.WriteLine("Produit supprimé.");
                        else
                            Console.WriteLine("Produit non trouvé.");
                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                        Console.ReadLine();
                        break;
                    case "5":
                        inProducts = false;
                        break;
                    default:
                        Console.WriteLine("Option non valide");
                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}