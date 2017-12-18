using System;
using System.Drawing;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
    /// <summary>
    /// Super hacky event für das Spielende! Wirklich nur am Ende verwenden. Speichert nicht, killt einfach nur das Spiel und
    /// geht zum Hauptmenü
    /// </summary>
    class GameOver : DialogueAction 
	{
        public GameOver()
        {

        }

        public override bool Complete(LevelModel level)
        {
            Game.GoToScreen(Game.mainMenuScreen);
            return true;
        }

        public override string GetText()
        {
            return Game.ll.GetTrans("dialogue.gameover");
        }
    }
}
