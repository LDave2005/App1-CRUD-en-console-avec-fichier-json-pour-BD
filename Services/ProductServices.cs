using App1.DAL;
using App1.Modeles;

namespace App1.Services
{
    public class ProductServices
    {
        public DataStore DataStore { get; set; }
        public ProductServices()
        {
            DataStore = new DataStore();
        }
        public List<Product> LireProduits()
        {
            var donnees = DataStore.Lire();
            if (donnees == null || donnees.products == null) return new List<Product>();
            return donnees.products;
        }

        public void AjouterProduit(Product p)
        {
            if (p == null) return;
            var donnees = DataStore.Lire();
            if (donnees.products == null) donnees.products = new List<Product>();
            /*int max = 0;
            foreach(var pr in donnees.products)
            {
                if (pr.id > max) max = pr.id;
            }
            p.id = max + 1;*/
            p.id = Traitement.CalculIDProduct(donnees);
            donnees.products.Add(p);
            DataStore.Ecrire(donnees);
        }

        public bool ModifierProduit(int id, Product pModifie)
        {
            if (pModifie == null) return false;
            var donnees = DataStore.Lire();
            if (donnees.products == null) return false;
            foreach (var pr in donnees.products)
            {
                if (pr.id == id)
                {
                    pr.nom = pModifie.nom;
                    pr.prix = pModifie.prix;
                    DataStore.Ecrire(donnees);
                    return true;
                }
            }
            return false;
        }

        public bool SupprimerProduit(int id)
        {
            var donnees = DataStore.Lire();
            if (donnees.products == null) return false;
            Product? cible = null;
            foreach (var pr in donnees.products)
            {
                if (pr.id == id)
                {
                    cible = pr;
                    break;
                }
            }
            if (cible == null) return false;
            donnees.products.Remove(cible);
            DataStore.Ecrire(donnees);
            return true;
        }
    }
}
