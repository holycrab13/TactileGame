using BrailleIO;
using BrailleIO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tud.mci.LanguageLocalization;
using tud.mci.tangram.audio;

namespace TactileGame.RPG.Menu
{
    public class StartScreen : InteractionScreen
    {
        private LL ll;

        private BrailleIOViewRange title;

        public StartScreen(LL ll, int width, int height) : base("StartScreen")
        {
            this.ll = ll;

            title = new BrailleIOViewRange(2, 2, width - 4, height - 4);
            title.SetText(ll.GetTrans("game.title"));

            AddViewRange(title);
        }

        public override void HandleInteraction(BrailleIO_DeviceButtonStates keyEvent)
        {
            if (keyEvent == BrailleIO_DeviceButtonStates.EnterUp)
            {
                Audio.PlayWave(StandardSounds.Start);
                Game.GoToScreen(Game.mainMenuScreen);
            }
        }

        public override void Resize(int width, int height)
        {
            title.SetLeft(2);
            title.SetTop(2);
            title.SetWidth(width - 4);
            title.SetHeight(width - 4);
            title.SetText(ll.GetTrans("game.title"));
        }

        protected override void OnShow()
        {
            Audio.AbortCurrentSound();
            Audio.PlaySound(ll.GetTrans("game.title"));
        }

        protected override void OnHide()
        {
            
        }
    }
}
