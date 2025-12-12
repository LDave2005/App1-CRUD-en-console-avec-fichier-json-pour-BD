using App1.Core;
using App1.Services;

namespace App1.Commands.UserCommand
{
    public class UserDesactivationCommand : ISousCommand
    {
        public string Name => "desactivate";

        public List<Parametre> parametres { get; } = new List<Parametre>
        {
            new Parametre("email","Email de l'utilisateur initiateur de la desactivation"),
            new Parametre("pwd","Mot de passe de l'utilisateur initiateur de la desactivation"),
            new Parametre("target","Email de l'utilisateur cible"),
        };

        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            UserServices services = new UserServices();

            string? dEmail;
            if(args.ContainsKey("email"))
            {
                dEmail = args["email"];
            }
            else
            {
                dEmail = op.LireAvecReset("Votre Email : ", out bool reset); if (reset) return;
            }
            string? dMdp;
            if (args.ContainsKey("pwd"))
            {
                dMdp = args["pwd"];
            }
            else
            {
                dMdp = op.LireMotDePasseAvecReset(); if(dMdp == null) return;
            }
            string? cEmail;
            if (args.ContainsKey("target"))
            {
                cEmail = args["target"];
            }
            else
            {
                cEmail = op.LireAvecReset("Email de l'User a desactiver : ", out bool reset); if(reset) return;
            }

            var ok = services.DesactiverUtilisateur(dEmail, dMdp, cEmail);
            if (ok)
            {
                op.Afficher("Le compte user a ete desactive avec succee");
            }
            else
            {
                op.Afficher("Le compte n'a pas ete descativee : Les informations que vous avez entres sont erronees");
            }
        }
    }
}
