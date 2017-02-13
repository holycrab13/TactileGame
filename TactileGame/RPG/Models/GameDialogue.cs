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
    class GameDialogue
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
        private Sentence currentAnswer;


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

        internal bool HasSentence()
        {
            return dialogue.phrases[index] is Sentence;
        }

        internal bool HasQuestion()
        {
            return dialogue.phrases[index] is Question;
        }

        internal Question GetQuestion()
        {
            return dialogue.phrases[index] as Question;
        }

        internal void SetAnswer(Sentence answer)
        {
            currentAnswer = answer;
        }

        internal Sentence GetAnswer()
        {
            return currentAnswer;
        }

        internal void Clear()
        {
            this.dialogue = null;
            this.index = 0;
            this.currentAnswer = null;
        }

        internal void FireEvent()
        {
            dialogue.phrases[index].Fire();
        }

        internal void SetTarget(WorldObject target)
        {
            this.target = target;
        }

        internal WorldObject GetTarget()
        {
            return target;
        }
    }
}
