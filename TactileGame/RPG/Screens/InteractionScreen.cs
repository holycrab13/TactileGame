using BrailleIO;
using BrailleIO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tud.mci.tangram.audio;

namespace TactileGame.RPG.Menu
{
    public abstract class InteractionScreen : BrailleIOScreen
    {
        protected BrailleIOMediator IO
        {
            get { return BrailleIOMediator.Instance; }
        }

        protected AudioRenderer Audio
        {
            get { return AudioRenderer.Instance; }
        }

        public InteractionScreen(string name)
        {
            this.Name = name;
        }

        public abstract void HandleInteraction(BrailleIO_DeviceButtonStates keyEvent);

        public abstract void Resize(int width, int height);

        protected abstract void OnShow();

        protected abstract void OnHide();

        public void Show()
        {
            IO.ShowView(Name);
            IO.RenderDisplay();

            OnShow();
        }

        public void Hide()
        {
            IO.HideView(Name);

            OnHide();
        }

        public void Render()
        {
            IO.RenderDisplay();
        }

        public void Register()
        {
            IO.AddView(Name, this);
        }

        protected void createOrUpdateViewRange(ref BrailleIOViewRange viewRange, int top, int left, int width, int height, uint border = 0,
            uint padding = 0, string text = null)
        {
            if (viewRange == null)
            {
                viewRange = new BrailleIOViewRange(top, left, width, height);
            }

            viewRange.SetTop(top);
            viewRange.SetWidth(width);
            viewRange.SetHeight(height);
            viewRange.SetLeft(left);
            viewRange.SetBorder(border, border, border);
            viewRange.SetPadding(padding);

            if (text != null)
            {
                viewRange.SetText(text);
            }
        }

        protected void AddViewRange(BrailleIOViewRange range)
        {
            AddViewRange(Guid.NewGuid().ToString(), range);
        }
    }
}
