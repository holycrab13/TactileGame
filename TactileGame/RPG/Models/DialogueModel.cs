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
        /// the current phrase index
        /// </summary>
        private int index;

        /// <summary>
        /// The current answer (null if there is none)
        /// </summary>
        private Answer currentAnswer;

        /// <summary>
        /// The target of the dialogue
        /// </summary>
        private WorldObject target;


        /// <summary>
        /// Increases the phrase index
        /// </summary>
        internal void SetNext()
        {
            index++;
        }

        /// <summary>
        /// Gets the phrase at the current index position
        /// </summary>
        /// <returns></returns>
        internal string GetCurrent()
        {
            return dialogue.GetText();
        }

        /// <summary>
        /// Sets the dialog and resets the index
        /// </summary>
        /// <param name="dialogue"></param>
        internal void SetDialogue(DialogueAction dialogue)
        {
            this.dialogue = dialogue;
            this.index = 0;
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
            this.index = 0;
            this.currentAnswer = null;
        }


        /// <summary>
        /// Sets the target of the current dialogue
        /// </summary>
        /// <param name="target"></param>
        internal void SetTarget(WorldObject target)
        {
            this.target = target;
        }

        /// <summary>
        /// Gets the target of the current dialogue
        /// </summary>
        /// <returns></returns>
        internal WorldObject GetTarget()
        {
            return target;
        }

        internal bool Boop(LevelModel level)
        {
            return dialogue.Boop(level);
        }

        internal bool HasAction()
        {
            return dialogue != null && !dialogue.IsComplete();
        }

        internal bool HasQuestion()
        {
            return dialogue is Question;
        }

        internal Question GetQuestion()
        {
            return dialogue as Question;
        }
    }
}
