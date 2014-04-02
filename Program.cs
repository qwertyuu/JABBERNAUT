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
    public static enum Games { None, HotAndCold }
    class Program
    {
        static List<Utilisateur> online;
        static XmppClientConnection xmpp = new XmppClientConnection("chat.facebook.com");
        static void Main(string[] args)
        {
            CurrentState = new State(State.Types.Chatting);
            online = new List<Utilisateur>();
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
            query.Start(xmpp);
            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    xmpp.Close();
                    break;
                }
            }
        }

        static void xmpp_OnMessage(object sender, ActualMessage msg)
        {
            var user = online.Find(a => a.ID == msg.From);
            if (user == null)
            {
                user = new Utilisateur(msg.From);
                online.Add(user);
            }

            string uName = online.Find(a => a.ID == msg.From).infos.FirstName;
            Console.WriteLine("<{0}>{1}", user.infos.Name, msg.Body);
            if (msg.Body != null)
            {
                //FAIRE UN INPUTHANDLER POUR LES INPUT!!!!!
                user.Update(msg.Body);
            }
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
            var user = online.Find(a => a.ID == pres.From);
            if (pres.Type == PresenceType.unavailable)
            {
                if (online.Contains(user))
                {
                    online.Remove(user);
                }
                string uName = user.infos.FirstName;
                Console.WriteLine("{0} disconnected!", uName);
            }
            else
            {
                if (!online.Contains(user))
                {
                    online.Add(user);
                }
                string uName = user.infos.FirstName;
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
