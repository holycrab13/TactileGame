using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Dialogue
    {
        public string[] phrases;

        public Dialogue(string[] phrases)
        {
            this.phrases = phrases;
        }

        public Dialogue(string Description)
        {
            this.phrases = new string[] { Description };
        }
    }
}
