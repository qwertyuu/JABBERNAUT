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
        Utilisateur Player;
        public HotAndCold(Utilisateur player) : base()
        {
            rand = new Random();
            toGuess = rand.Next(100);
            Player = player;
        }
        public override string GameLoop(string arg)
        {
            oldGuess = guess;
            guess = -1;
            if (!int.TryParse(arg, out guess) && guess >= 0 && guess <= 100)
            {
                return "Ceci n'était pas une entrée valide\n(Le chiffre doit être entre 0 et 100)";
            }
            else
            {
                turn++;
                int DiffOldGuess = (int)Math.Abs(toGuess - oldGuess);
                int DiffCurrentGuess = (int)Math.Abs(toGuess - guess);
                if (guess == toGuess)
                {
                    Player.WhatAmIDoing = new State(State.Types.Chatting);
                    int score = turn;
                    turn = 0;
                    return string.Format("Bravo, tu l'as trouvé en {0} coups!", score);
                }
                else if (DiffCurrentGuess > DiffOldGuess)
                {
                    return "Tu refroidit";
                }
                else if (DiffCurrentGuess < DiffOldGuess)
                {
                    return "Tu réchauffe!";
                }
                else
                {
                    return "C'est tiède";
                }
            }
        }
    }
}
