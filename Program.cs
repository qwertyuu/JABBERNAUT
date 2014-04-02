using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Collections;
using agsXMPP.protocol.iq.roster;
using System.Threading;
using ActualMessage = agsXMPP.protocol.client.Message;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;

namespace JABBERNAUT
{
    class Program
    {
        static Dictionary<string, Func<string, string>> Games;
        static List<Jid> online;
        static XmppClientConnection xmpp = new XmppClientConnection("chat.facebook.com");
        static Dictionary<string, Dictionary<string, string>> usernames;
        static string CurrentState;
        static void Main(string[] args)
        {
            Games = new Dictionary<string, Func<string, string>>();
            rand = new Random();
            CurrentState = "none";
            online = new List<Jid>();
            usernames = new Dictionary<string, Dictionary<string, string>>();
            Thread query = new Thread(TellUsTheTruth);
            query.IsBackground = true;
            Console.WriteLine("JID: bot.araph@chat.facebook.com");
            string JID_Sender = "bot.araph@chat.facebook.com";
            string Password = "";
            using (StreamReader sR = new StreamReader("pw.txt"))
            {
                Password = sR.ReadLine();
            }

            Jid jidSender = new Jid(JID_Sender);
            xmpp.Open(jidSender.User, Password);
            xmpp.OnMessage += xmpp_OnMessage;
            xmpp.OnLogin += new ObjectHandler(xmpp_OnLogin);
            xmpp.OnClose += xmpp_OnClose;
            xmpp.OnError += xmpp_OnError;
            xmpp.OnPresence += new PresenceHandler(xmpp_OnPresence);
            Games["hotandcold"] = new Func<string, string>(HotAndCold);
            query.Start(xmpp);
            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    xmpp.Close();
                    break;
                }
                else
                {
                    AttentionWhore();
                }
            }
        }
        static int turn = 0;
        static int oldGuess;
        static int toGuess;
        static int guess;
        static Random rand;
        private static string HotAndCold(string arg)
        {
            oldGuess = guess;
            guess = -1;
            if (!int.TryParse(arg, out guess) && guess >= 0 && guess <= 100)
            {
                return "Ceci n'était pas une entrée valide\n(Le chiffre doit être entre 0 et 100)";
            }
            if (turn == 0)
            {
                oldGuess = toGuess;
                toGuess = rand.Next(100);
                if (guess == toGuess)
                {
                    CurrentState = "none";
                    return "Bravo! C'était ça!";
                }
                else
                {
                    turn++;
                    return "Désolé, essai encore.";
                }
            }
            else
            {
                turn++;
                int DiffOldGuess = (int)Math.Abs(toGuess - oldGuess);
                int DiffCurrentGuess = (int)Math.Abs(toGuess - guess);
                if (guess == toGuess)
                {
                    CurrentState = "none";
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

        private static void AttentionWhore()
        {
            foreach (var item in online)
            {
                xmpp.Send(new ActualMessage(item, "Je suis là!"));
            }
        }

        static void xmpp_OnMessage(object sender, ActualMessage msg)
        {
            string uName = GetInfo(msg.From.User, "name");
            Console.WriteLine("<{0}>{1}", usernames[msg.From.User]["name"], msg.Body);
            if (msg.Body != null)
            {
                ProcessInput(msg);
            }
        }

        private static void ProcessInput(ActualMessage msg)
        {
            var parts = msg.Body.Split(' ');
            switch (CurrentState)
            {
                case "none":
                    switch (parts[0].ToLower())
                    {
                        case "play":
                            Console.WriteLine("{0} a entré la commande Play", GetInfo(msg.From.User, "first_name"));
                            if (Games.ContainsKey(parts[1].ToLower()))
                            {
                                CurrentState = parts[1].ToLower();
                                xmpp.Send(new ActualMessage(msg.From, "Joue à " + parts[1].ToLower()));
                                Console.WriteLine("{0} a entré la commande Play {1}", GetInfo(msg.From.User, "first_name"), parts[1].ToLower());
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    xmpp.Send(new ActualMessage(msg.From, Games[CurrentState](msg.Body)));
                    break;
            }
        }

        private static string GetInfo(string _user, string _key)
        {
            if (!usernames.ContainsKey(_user))
            {
                usernames[_user] = GetName(_user);
            }
            return usernames[_user][_key];
        }

        private static Dictionary<string, string> GetName(string p)
        {
            WebClient c = new WebClient();
            string infos = c.DownloadString("http://graph.facebook.com/" + p.Substring(1));
            string asciiInfos = Regex.Replace(infos,@"\\u(?<Value>[a-zA-Z0-9]{4})",m =>
            {
                return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
            });
            Dictionary<string, string> userInfos = ParseInfos(asciiInfos);
            return userInfos;
        }

        private static Dictionary<string, string> ParseInfos(string asciiInfos)
        {
            asciiInfos = asciiInfos.Replace("\"", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
            var allStrings = asciiInfos.Split(',');
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            foreach (var item in allStrings)
            {
                var items = item.Split(':');
                toReturn[items[0]] = items[1];
            }
            return toReturn;
        }

        static void xmpp_OnClose(object sender)
        {
            Console.WriteLine("Connection closed");
        }

        static void xmpp_OnError(object sender, Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        private static void TellUsTheTruth(object obj)
        {
            while (true)
            {
                Console.Title = string.Format("Connection State:{0} -- Authenticated?:{1} -- CurrentState:{2}", xmpp.XmppConnectionState, xmpp.Authenticated, CurrentState);
                Thread.Sleep(100);
            }
        }

        private static void xmpp_OnPresence(object sender, Presence pres)
        {
            if (pres.Type == PresenceType.unavailable)
            {
                if (online.Contains(pres.From))
                {
                    online.Remove(pres.From);
                }
                string uName = GetInfo(pres.From.User, "name");
                Console.WriteLine("{0} disconnected!", uName);
            }
            else
            {
                if (!online.Contains(pres.From))
                {
                    online.Add(pres.From);
                }
                string uName = GetInfo(pres.From.User, "name");
                Console.WriteLine("{0} connected!", uName);
                //xmpp.Send(new ActualMessage(pres.From, string.Format("Salut {0}!", GetInfo(pres.From.User, "first_name"))));
            }
            //Console.WriteLine("{0}@{1}  {2}",
            //    pres.From.User, pres.From.Server, pres.Type, pres.Nickname);
        }

        private static void xmpp_OnLogin(object sender)
        {
            Console.WriteLine("Logged In");
            Presence p = new Presence(ShowType.chat, "Online");
            p.Type = PresenceType.available;
            xmpp.Send(p);

        }
    }
}
