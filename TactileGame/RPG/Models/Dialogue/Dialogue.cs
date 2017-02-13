using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Dialogue
    {
        public string[] conditions;

        public Phrase[] phrases;

        internal bool IsAvailable()
        {
            foreach(string condition in conditions)
            {
                if(!Game.HasKnowledge(condition))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
