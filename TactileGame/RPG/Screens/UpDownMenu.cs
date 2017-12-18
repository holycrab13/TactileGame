using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Models;
using tud.mci.tangram.audio;

namespace TactileGame.RPG.Menu
{
    public struct UpDownMenuItem
    {
        public Action action;
        public string label;

        public UpDownMenuItem(Action action, string label)
        {
            this.action = action;
            this.label = label;
        }
    }

    public class UpDownMenu
    {
        public int index;

        public UpDownMenuItem[] items;
        private Action abort;

        public UpDownMenu(Action abort, params UpDownMenuItem[] items)
        {
            if (items == null || items.Length == 0)
            {
                throw new ArgumentException("Invalid actions passed");
            }

            this.abort = abort;
            this.items = items;
        }

        public void Next()
        {
            index++;

            index = index % items.Length;
        }

        public void Previous()
        {
            index--;

            while(index < 0)
            {
                index += items.Length;
            }
        }

        public string GetLabel(int index)
        {
            return items[index].label;
        }

        public void Select()
        {
            items[index].action();
        }

        public int Length
        {
            get { return items.Length; }
        }

        public void PlaySelected(AudioRenderer audio)
        {
            audio.AbortCurrentSound();
            audio.PlaySound(GetLabel(index));
        }

        public void Abort()
        {
            if (abort != null)
            {
                abort();
            }
        }

        internal static UpDownMenu FromQuestion(Question question, Action<Question, int> answerSelected)
        {
            UpDownMenuItem[] items = new UpDownMenuItem[question.answers.Count];

            int k = 0;
            foreach(Answer answer in question.answers)
            {
                int i = k++;
                items[i] = new UpDownMenuItem(() => answerSelected(question, i), answer.text);
            }

            return new UpDownMenu(null, items);
        }
    }
}
