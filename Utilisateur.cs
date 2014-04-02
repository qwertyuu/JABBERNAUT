using agsXMPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JABBERNAUT
{
    class Utilisateur
    {
        public Jid ID;
        public State WhatAmIDoing;
        public Game WhatAmIPlaying;
        public Infos infos;
        public Utilisateur(Jid _id)
        {
            ID = _id;
            WhatAmIDoing = new State(State.Types.Chatting);
        }
        public void Update(string message)
        {

        }
    }
}
