using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JABBERNAUT
{
    class State
    {
        public static enum Types { Chatting, Playing }
        public Types state;
        public Games currentlyPlaying;
        public State(Types wut, Games game = Games.None)
        {
            state = wut;
            currentlyPlaying = game;
        }
    }
}
