using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Question : DialogueAction
    {
        public string text;

        public List<Answer> answers = new List<Answer>();

        public Answer currentAnswer;

        internal bool SetAnswer(int p)
        {
            if (p >= 0 && p < answers.Count)
            {
                currentAnswer = answers[p];
                return true;
            }

            return false;
        }

        internal string GetAnswerText()
        {
           if(currentAnswer != null)
           {
               return currentAnswer.GetText();
           }

            return "Bitte wähle eine Antwort mit den Pfeiltasten aus";
        }

        public override string GetText()
        {
            return text;
        }

        public override bool Complete(LevelModel level)
        {
            if(currentAnswer != null)
            {
                currentAnswer.Complete(level);
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            currentAnswer = null;
            base.Reset();
        }
    }
}
