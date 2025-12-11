using App1.DAL;
using App1.Modeles;
using App1.Services;
using Org.BouncyCastle.Crypto;

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
            Traitement op = new Traitement();
            ProductServices product = new ProductServices();
            var produits = product.LireProduits();

            int idW = 2;
            int nomW = 3;
            int descW = 11;
            int prixW = 8;
            int stockW = 5;

            //tailles maximales
            int maxId = 5;
            int maxNom = 20;
            int maxDesc = 30;
            int maxPrix = 10;
            int maxStock = 5;

            foreach (var p in produits)
            {
                idW = Math.Min(Math.Max(p.id.ToString().Length, idW), maxId);
                nomW = Math.Min(Math.Max((p.nom ?? "").ToString().Length, nomW), maxNom);
                descW = Math.Min(Math.Max((p.description ?? "").ToString().Length, descW), maxDesc);
                prixW = Math.Min(Math.Max(p.prix.ToString().Length, prixW), maxPrix);
                stockW = Math.Min(Math.Max(p.stock.ToString().Length, stockW), maxStock);
            }

            string sep = "+" + new string('-', idW + 2) + "+" + new string('-', nomW + 2) + "+" +
                         new string('-', descW + 2) + "+" + new string('-', prixW + 2) + "+" +
                         new string('-', stockW + 2) + "+";

            Console.WriteLine(sep);
            Console.WriteLine($"| {"ID".PadRight(idW)} | {"Nom".PadRight(nomW)} | {"Description".PadRight(descW)} | {"Prix".PadRight(prixW)} | {"Stock".PadRight(stockW)} |");
            Console.WriteLine(sep);

            foreach (var p in produits)
            {
                var idLines = op.Wrap(p.id.ToString(),idW);
                var nomLines = op.Wrap((p.nom ?? "").ToString(), nomW);
                var descLines = op.Wrap((p.description ?? "").ToString(), descW);
                var prixLines = op.Wrap(p.prix.ToString(), prixW);
                var stockLines = op.Wrap((p.stock).ToString(), stockW);

                //
                int max = new List<int>
                {
                    idLines.Count, nomLines.Count,descLines.Count, prixLines.Count, stockLines.Count
                }.Max();

                for(int i = 0; i < max; i++)
                {
                    string id = i < idLines.Count ? idLines[i].PadRight(idW) : new string(' ', idW);
                    string nom = i < nomLines.Count ? nomLines[i].PadRight(nomW) : new string(' ', nomW);
                    string desc = i < descLines.Count ? descLines[i].PadRight(descW) : new string(' ', descW);
                    string prix = i < prixLines.Count ? prixLines[i].PadRight(prixW) : new string(' ', prixW);
                    string stock = i < stockLines.Count ? stockLines[i].PadRight(stockW) : new string(' ', stockW);

                    Console.WriteLine($"| {id} | {nom} | {desc} | {prix} | {stock} |");
                }
                Console.WriteLine(sep);
            }
            
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