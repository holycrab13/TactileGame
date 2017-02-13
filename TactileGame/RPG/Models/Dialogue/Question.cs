using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Question : Phrase
    {
        public List<Sentence> answers = new List<Sentence>();

        internal Sentence GetAnswer(int p)
        {
            if(p >= 0 && p < answers.Count)
            {
                return answers[p];
            }

            return null;
        }
    }
}
