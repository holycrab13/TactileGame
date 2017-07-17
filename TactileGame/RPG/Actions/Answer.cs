using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Answer : DialogueAction
    {
        public string text;

        public string trigger;

        public override bool Complete(LevelModel level)
        {
            ApplySetsAndClears();
            level.TriggerEvent(trigger);
            return true;
        }

        public override string GetText()
        {
            return text;
        }
    }
}
