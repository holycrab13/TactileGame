using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    /// <summary>
    /// The current stuff to be displayed in the dialogue region
    /// </summary>
    class DialogueModel
    {
        /// <summary>
        /// The current dialogue
        /// </summary>
        private DialogueAction dialogue;

        /// <summary>
        /// The current answer (null if there is none)
        /// </summary>
        private Answer currentAnswer;

        /// <summary>
        /// The target of the dialogue
        /// </summary>
        private WorldObject target;


        /// <summary>
        /// Gets the phrase at the current index position
        /// </summary>
        /// <returns></returns>
        internal string GetCurrent()
        {
            return dialogue != null ? dialogue.GetText() : string.Empty;
        }

        /// <summary>
        /// Sets the dialog and resets the index
        /// </summary>
        /// <param name="dialogue"></param>
        internal void SetDialogue(DialogueAction dialogue)
        {
            this.dialogue = dialogue;
            this.currentAnswer = null;



        }

        

      
        /// <summary>
        /// Set the answer of the question
        /// </summary>
        /// <param name="answer"></param>
        internal void SetAnswer(Answer answer)
        {
            currentAnswer = answer;
        }

        /// <summary>
        /// Gets the last set answer of the current question
        /// </summary>
        /// <returns></returns>
        internal Answer GetAnswer()
        {
            return currentAnswer;
        }

        /// <summary>
        /// Clears the dialgue model
        /// </summary>
        internal void Clear()
        {
            this.dialogue = null;
            this.currentAnswer = null;
        }

        internal bool Confirm(LevelModel level)
        {
            return dialogue.OnConfirm(level);
        }

        internal bool HasAction()
        {
            return dialogue != null && !dialogue.IsComplete();
        }
    }
}
