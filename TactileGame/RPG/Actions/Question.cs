using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Events;
using TactileGame.RPG.Menu;

namespace TactileGame.RPG.Models
{
    class Question : ActionBase
    {
        public string question;

        public List<Answer> answers = new List<Answer>();
        
        private bool pushed;
        private Phrase questionAction;
        private int selectedAnswer = -1;
        private bool isComplete;

        public override void Update(Controller.LevelController levelController)
        {
            if(!pushed)
            {
                pushed = true;
                questionAction = new Phrase() { text = question };
                levelController.GetDialogue().SetDialogue(questionAction);
            }

            if(selectedAnswer > -1)
            {
                answers[selectedAnswer].Complete(levelController.GetModel());
                isComplete = true;
                return;
            }

            if(questionAction.IsComplete())
            {
                UpDownMenu menu = UpDownMenu.FromQuestion(this, answerSelected);

                Game.questionScreen.SetMenu(menu);
                Game.GoToScreen(Game.questionScreen);
            }
        }

        private void answerSelected(Question question, int index)
        {
            Game.GoToScreen(Game.gameScreen);
            selectedAnswer = index;
        }

        public override bool IsComplete()
        {
            return isComplete;
        }

        public override void Reset()
        {
            selectedAnswer = -1;
            pushed = false;
            isComplete = false;

            base.Reset();
        }



      
    }
}
