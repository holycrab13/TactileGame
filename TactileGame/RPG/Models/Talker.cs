using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TactileGame.RPG.Models
{
    class Talker : NPC
    {
        private Dialogue dialogue;

        public Talker(byte[,] texture, Dialogue dialogue)
            : base(texture)
        {
            this.dialogue = dialogue;
        }

        public Dialogue GetDialogue()
        {
            return dialogue;
        }
    }
}
