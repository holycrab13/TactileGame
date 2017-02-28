using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Phrase
    {
        public string text;

        public static Phrase Create(string phrase)
        {
            return new Phrase()
            {
                text = phrase
            };
        }

        public static Phrase Create(string phrase, params object[] p)
        {
            return new Phrase()
            {
                text = string.Format(phrase, p)
            };
        }
    }
}
