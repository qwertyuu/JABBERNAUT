using agsXMPP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace JABBERNAUT
{
    class Infos
    {
        public string ID;
        public string Name;
        public string FirstName;
        public string LastName;
        public string Gender;
        public string Locale;
        public string Username;
        public Infos(Jid userID)
        {
            SetInfos(userID.User);
        }

        private void SetInfos(string ID)
        {
            WebClient c = new WebClient();
            string infos = c.DownloadString("http://graph.facebook.com/" + ID.Substring(1));
            string asciiInfos = Regex.Replace(infos, @"\\u(?<Value>[a-zA-Z0-9]{4})", m =>
            {
                return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
            });
            asciiInfos = asciiInfos.Replace("\"", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
            var allStrings = asciiInfos.Split(',');
            for (int i = 0; i < allStrings.Length; i++)
            {
                allStrings[i] = allStrings[i].Split(':')[1];
            }
            int j = 0;
            ID = allStrings[j++];
            Name = allStrings[j++];
            FirstName = allStrings[j++];
            LastName = allStrings[j++];
            Gender = allStrings[j++];
            Locale = allStrings[j++];
            Username = allStrings[j++];
        }
    }
}
