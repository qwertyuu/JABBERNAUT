using agsXMPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JABBERNAUT
{
    class HotAndCold : Game
    {
        int turn = 0;
        int oldGuess;
        int toGuess;
        int guess;
        Random rand;
        public override string GameName { get; set; }
        public HotAndCold(Utilisateur player) : base(player)
        {
            GameName = "Hot and Cold";
            rand = new Random();
            toGuess = rand.Next(100);
            Player.Tell("Tu joue à Hot and Cold là!");
        }
        public override bool Input(string arg)
        {
            if (base.Input(arg))
            {
                return true;
            }
            oldGuess = guess;
            guess = -1;
            if (!int.TryParse(arg, out guess) && guess >= 0 && guess <= 100)
            {
                Player.Tell("Ceci n'était pas une entrée valide\n(Le chiffre doit être entre 0 et 100)");
            }
            else
            {
                turn++;
                int DiffOldGuess = (int)Math.Abs(toGuess - oldGuess);
                int DiffCurrentGuess = (int)Math.Abs(toGuess - guess);
                if (guess == toGuess)
                {
                    int score = turn;
                    turn = 0;
                    Player.Tell(string.Format("Bravo, tu l'as trouvé en {0} coups!", score));
                    Player.QuitGame();
                }
                else if (turn == 1)
                {
                    Player.Tell("Pas ça, essaye encore!");
                }
                else if (DiffCurrentGuess > DiffOldGuess)
                {
                    Player.Tell("Tu refroidit");
                }
                else if (DiffCurrentGuess < DiffOldGuess)
                {
                    Player.Tell("Tu réchauffe!");
                }
                else
                {
                    Player.Tell("C'est tiède");
                }
            }
            return false;
        }
    }
}
