using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;

namespace TactileGame.RPG.Models
{
    /// <summary>
    /// A dialogue
    /// </summary>
    abstract class DialogueAction : ActionBase
    {
        private bool pushed;

        private bool isComplete;

        public abstract bool Complete(LevelModel level);

        public bool OnConfirm(LevelModel level)
        {
            isComplete = Complete(level);
            return isComplete;
        }

        public abstract string GetText();

        public override void Update(LevelController levelController)
        {
            if (!pushed)
            {
                levelController.GetDialogue().SetDialogue(this);
            }
        }

        public override bool IsComplete()
        {
            return isComplete;
        }

        public override void Reset()
        {
            pushed = false;
            isComplete = false;
        }
    }
}
