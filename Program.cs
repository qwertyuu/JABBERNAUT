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
        static List<Jid> online;
        static XmppClientConnection xmpp = new XmppClientConnection("chat.facebook.com");
        static Dictionary<string, Dictionary<string, string>> usernames;
        static void Main(string[] args)
        {
            online = new List<Jid>();
            usernames = new Dictionary<string, Dictionary<string, string>>();
            Thread query = new Thread(TellUsTheTruth);
            query.IsBackground = true;
            Console.WriteLine("Login");
            Console.WriteLine();
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
            xmpp.OnRosterEnd += xmpp_OnRosterEnd;
            xmpp.OnPresence += new PresenceHandler(xmpp_OnPresence);
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
                xmpp.Send(new ActualMessage(msg.From, string.Format("{0}\n\n{1}", msg.Body, new string(msg.Body.Reverse().ToArray()))));
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

        static void xmpp_OnRosterEnd(object sender)
        {
            //Console.WriteLine("Roster end");
        }

        static void xmpp_OnClose(object sender)
        {
            Console.WriteLine("Closed");
        }

        static void xmpp_OnError(object sender, Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        private static void TellUsTheTruth(object obj)
        {
            while (true)
            {
                Console.Title = string.Format("Connection State:{0} -- Authenticated?:{1}", xmpp.XmppConnectionState, xmpp.Authenticated);
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
                xmpp.Send(new ActualMessage(pres.From, string.Format("Salut {0}!", GetInfo(pres.From.User, "first_name"))));
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
