using agsXMPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActualMessage = agsXMPP.protocol.client.Message;

namespace JABBERNAUT
{
    public class Utilisateur
    {
        public Jid ID;
        public State WhatAmIDoing;
        public Game WhatAmIPlaying;
        public Infos infos;
        public Utilisateur(Jid _id)
        {
            infos = new Infos(_id);
            
            ID = _id;
            WhatAmIDoing = new State(State.Types.Chatting);
        }
        public void Input(string message)
        {
            switch (WhatAmIDoing.state)
            {
                case State.Types.Chatting:
                    var parts = message.Split(' ');
                    switch (parts[0].ToLower())
                    {
                        case "play":
                            Games outGame = Games.None;
                            if (Enum.TryParse(parts[1], true, out outGame))
                            {
                                WhatAmIDoing = new State(State.Types.Playing, outGame);
                                WhatAmIPlaying = GetGame(outGame);
                                Program.Loggit(this, string.Format("joue à {0}", WhatAmIPlaying.GameName), ConsoleColor.DarkYellow);
                            }
                            break;
                        case "aide":
                            Tell("Le seul jeu pour le moment c'est HotAndCold :P Pour jouer entre\nPlay HotAndCold");
                            break;
                        default:
                            Tell(new string(message.Reverse().ToArray()));
                            break;
                    }
                    break;
                case State.Types.Playing:
                    switch (WhatAmIDoing.currentlyPlaying)
                    {
                        case Games.None:
                            QuitGame();
                            break;
                        default:
                            WhatAmIPlaying.Input(message);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private Game GetGame(Games game)
        {
            switch (game)
            {
                case Games.HotAndCold:
                    return new HotAndCold(this);
                default:
                    return null;
            }
        }
        public void Tell(string message)
        {
            Program.Loggit(this, message, ConsoleColor.DarkRed);
            Program.xmpp.Send(new ActualMessage(ID, message));
            System.Threading.Thread.Sleep(100);
        }

        internal void QuitGame()
        {
            Tell("De retour au chat!");
            Program.Loggit(this, "a quitté le jeu", ConsoleColor.DarkYellow);
            WhatAmIDoing = new State(State.Types.Chatting);
            WhatAmIPlaying = null;
        }
    }
}
