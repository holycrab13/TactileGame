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
        private Dialogue dialogue;

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
        /// Indicates whether there is more after the current phrase
        /// </summary>
        /// <returns></returns>
        internal bool HasNext()
        {
            return dialogue != null && index < dialogue.phrases.Length - 1;        
        }

        /// <summary>
        /// Indicates whether there is more after the current phrase
        /// </summary>
        /// <returns></returns>
        internal bool HasPhrase()
        {
            return dialogue != null && index < dialogue.phrases.Length;        
        }

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
            return dialogue.phrases[index].text;
        }

        /// <summary>
        /// Sets the dialog and resets the index
        /// </summary>
        /// <param name="dialogue"></param>
        internal void SetDialogue(Dialogue dialogue)
        {
            this.dialogue = dialogue;
            this.index = 0;
            this.currentAnswer = null;
        }

        /// <summary>
        /// Sets the dialog and resets the index
        /// </summary>
        /// <param name="dialogue"></param>
        internal void SetDialogue(Phrase phrase)
        {
            this.dialogue = new Dialogue() { phrases = new Phrase[] { phrase } };
            this.index = 0;
            this.currentAnswer = null;
        }

        /// <summary>
        /// Indicates whether the current phrase is a sentence
        /// </summary>
        /// <returns></returns>
        internal bool HasSentence()
        {
            return !(dialogue.phrases[index] is Question);
        }


        /// <summary>
        /// Get the current phrase as a question
        /// </summary>
        /// <returns></returns>
        internal Question GetQuestion()
        {
            return dialogue.phrases[index] as Question;
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
        /// Fires the event method of the current dialogue event
        /// </summary>
        internal void FireEvent()
        {
            dialogue.Fire();
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
    }
}
