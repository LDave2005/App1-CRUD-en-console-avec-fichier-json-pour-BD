using App1.Core;
using App1.Services;
using System.Runtime.Remoting;

namespace App1.Commands.UserCommand
{
    public class UserValidationGuestCommand : ISousCommand
    {
        public string Name => "validate";
        public List<Parametre> parametres { get; } = new List<Parametre>
        {
            new Parametre("email"),
            new Parametre("pwd"),
            new Parametre("target"),
            new Parametre("nr"),
        };

        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            UserServices services = new UserServices();

            string? dEmail;
            if (args.ContainsKey("email"))
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
                dMdp = op.LireMotDePasseAvecReset(); if (dMdp == null) return;
            }
            string? cEmail;
            if (args.ContainsKey("target"))
            {
                cEmail = args["target"];
            }
            else
            {
                cEmail = op.LireAvecReset("Email de l'invite a valider : ", out bool reset); if (reset) return;
            }
            string? role;
            if (args.ContainsKey("nr"))
            {
                role = args["nr"];
            }
            else
            {
                role = op.LireAvecReset("Le role a attribuer : ", out bool reset); if(reset) return;
            }

            var ok = services.AssignerRoleAdmin(dEmail,dMdp,cEmail,role);
            if (ok)
            {
                op.Afficher($"L'invite est desormais un {role}");
            }
            else
            {
                op.Afficher("Echec : invite non trouve ou informations erronees");
            }
        }
    }
}
