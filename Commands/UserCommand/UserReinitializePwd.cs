using App1.Core;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.UserCommand
{
    public class UserReinitializePwd : ISousCommand
    {
        public string Name => "reinitialize";

        public List<Parametre> parametres { get; } = new List<Parametre>
        { 
            new Parametre("email"),
            new Parametre("pwd"),
            new Parametre("targetE"),
            new Parametre("npwd"),
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
                dMdp = op.LireMotDePasseAvecReset(); if (dMdp == null) return;
            }
            string? cEmail;
            if (args.ContainsKey("targetE"))
            {
                cEmail = args["targetE"];
            }
            else
            {
                cEmail = op.LireAvecReset("Email du user a modifier : ", out bool reset); if (reset) return;
            }
            string? nMdp;
            if (args.ContainsKey("npwd"))
            {
                nMdp = args["npwd"];
            }
            else
            {
                nMdp = op.LireMotDePasseAvecReset(); if (nMdp == null) return;
            }

            var ok = services.ReinitialiserMdp(dEmail, dMdp, cEmail, nMdp);
            if (ok)
            {
                op.Afficher("Mot de Passe modifier avec succes");
            }
            else
            {
                op.Afficher("Erreur modification du mot de passe echouee : Verfier les informations que vous avez entrez");
            }
        }
    }
}
