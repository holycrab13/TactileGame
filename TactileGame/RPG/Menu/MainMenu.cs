using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Menu
{
    class MainMenu
    {
        public int index;

        public Action[] menuAction;

        public MainMenu(params Action[] actions)
        {
            if(actions == null || actions.Length == 0)
            {
                throw new ArgumentException("Invalid actions passed");
            }

            menuAction = actions;
        }

        public void Next()
        {
            index++;

            index = index % menuAction.Length;
        }

        public void Previous()
        {
            index--;

            while(index < 0)
            {
                index += menuAction.Length;
            }
        }

        public void Select()
        {
            menuAction[index]();
        }
    }
}
