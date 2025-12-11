using App1.Core;

namespace App1.Commands.UserCommand
{
    public class UserCommand2 : ICommand2
    {
        string Name => "user";

        string ICommand2.Name => Name;

        //public Dictionary<string, ISousCommand> SousCommands => throw new NotImplementedException();

        Dictionary<string, ISousCommand> SousCommands { get; } = new Dictionary<string, ISousCommand>();

        Dictionary<string, ISousCommand> ICommand2.SousCommands => SousCommands;

        public UserCommand2()
        {
            SousCommands["create"] = new UserCreateSubCommand();
            SousCommands["list"] = new UserListSubCommand();
            SousCommands["connect"] = new UserConnectCommand();
            SousCommands["reinitialize"] = new UserReinitializePwd();
            SousCommands["desactivate"] = new UserDesactivationCommand();
            SousCommands["validate"] = new UserValidationGuestCommand();
        }

        public void Execute(Dictionary<string, string> args)
        {
            //Code qui realise ces taches (creation et listing des user)
            
        } 
    }
}
