using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;

namespace TactileGame.RPG.Models
{
    class Phrase : DialogueAction
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


        public override string GetText()
        {
            return text;
        }


        public override bool Complete(LevelModel level)
        {
            ApplySetsAndClears();
            return true;
        }
    }
}
