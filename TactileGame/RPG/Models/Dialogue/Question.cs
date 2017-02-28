using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Question : Phrase
    {
        public List<Answer> answers = new List<Answer>();

        internal Answer GetAnswer(int p)
        {
            if(p >= 0 && p < answers.Count)
            {
                return answers[p];
            }

            return null;
        }
    }
}
