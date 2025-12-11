using App1.Services;

namespace App1
{
    public class Commande
    {
        public string? nomC { get; set; }
        public List<Commande>? sousCommande = new List<Commande>();
        public UserServices service = new UserServices();
        public Traitement op = new Traitement();

        public Commande(string? nom)
        {
            nomC = nom;
            sousCommande = null;
        }

        public void AjouterSousCommande(Commande cmd)
        {
            if (sousCommande == null)
            {
                sousCommande = new List<Commande>();
            }
            sousCommande.Add(cmd);
        }
        
        public void ConnectCommand(string[] args)
        {
            string? username = null;
            string? password = null;

            for(int i  = 0; i < args.Length;i++)
            {
                switch(args[i])
                {
                    case "-u":
                        if(i + 1 < args.Length)
                        {
                            username = args[i + 1];
                            i++;
                        }
                        break;
                    case "-p":
                        if(i + 1 < args.Length)
                        {
                            password = args[i + 1];
                            i++;
                        }
                        break;
                }
            }
            if(string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Email : ");
                username = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Mot de passe : ");
                password = Console.ReadLine();
            }

            var user = service.Authentifier(username ?? "", password ?? "");
            if (user == null)
            {
                Console.WriteLine("Échec d'authentification (ou compte inactif ou en attente de validation).");
                var action = op.AttendreEntreeOuReset("Appuyez sur Entrée pour réessayer ou Ctrl+R pour revenir au menu principal...");
                if (action == Traitement.PauseAction.Reset)
                    return; // revient au menu principal
                 
            }
        }
    }
}
