using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActualMessage = agsXMPP.protocol.client.Message;

namespace JABBERNAUT
{
    class InputHandler
    {


        private static void ProcessInput(ActualMessage msg)
        {
            var parts = msg.Body.Split(' ');
            switch (CurrentState.state)
            {
                case State.Types.Chatting:
                    switch (parts[0].ToLower())
                    {
                        case "play":
                            Console.WriteLine("{0} a entré la commande Play", online.Find(a => a.ID == msg.From).infos.FirstName);
                            Games outGame = Games.None;
                            if (Enum.TryParse(parts[1], true, out outGame))
                            {

                                CurrentState = new State(State.Types.Playing, outGame);
                                xmpp.Send(new ActualMessage(msg.From, "Joue à " + parts[1].ToLower()));
                                Console.WriteLine("{0} a entré la commande Play {1}", online.Find(a => a.ID == msg.From).infos.FirstName, parts[1].ToLower());

                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case State.Types.Playing:
                    switch (CurrentState.currentlyPlaying)
                    {
                        case Games.None:
                            CurrentState = new State(State.Types.Chatting);
                            break;
                        case Games.HotAndCold:

                            break;
                        default:
                            break;
                    }
                    break;
            }
        }
    }
}
