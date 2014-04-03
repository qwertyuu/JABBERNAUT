using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JABBERNAUT
{
    public abstract class Game
    {
        public Utilisateur Player;
        public abstract string GameName { get; set; }
        public Game(Utilisateur player)
        {
            Player = player;
        }
        public virtual bool Input(string arg)
        {
            if (arg.Trim().ToLower() == "quit")
            {
                Player.QuitGame();
                return true;
            }
            return false;
        }
    }
}
