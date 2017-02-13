using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Sentence : Phrase
    {
        public static Sentence Create(string phrase)
        {
            return new Sentence()
            {
                text = phrase
            };
        }

        public static Sentence Create(string phrase, params object[] p)
        {
            return new Sentence()
            {
                text = string.Format(phrase, p) 
            };
        }
    }

}
