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
    public class MenuScreen : InteractionScreen
    {
        private UpDownMenu menu;
        private int left;
        private int top;
        private int width;
        private int itemHeight;

        private BrailleIOViewRange[] ranges;

        public MenuScreen(string name, UpDownMenu menu, int left, int top, int width, int itemHeight) : base(name)
        {
            this.menu = menu;
            this.left = left;
            this.top = top;
            this.width = width;
            this.itemHeight = itemHeight;

            createOrUpdate(width, itemHeight);
        }

        public override void Resize(int width, int height)
        {
            this.width = width;
            createOrUpdate(width, itemHeight);
        }

        public void SetMenu(UpDownMenu menu)
        {
            ranges = null;

            {
                RemoveViewRange(range.Name);
            }

            this.menu = menu;

            createOrUpdate(width, itemHeight);
        }

        private void createOrUpdate(int width, int itemHeight)
        {
            if (ranges == null && menu != null)
            {
                ranges = new BrailleIOViewRange[menu.Length];

                for (int i = 0; i < ranges.Length; i++)
                {
                    BrailleIOViewRange range = new BrailleIOViewRange(left, top + i * itemHeight, width - left, itemHeight);
                    range.SetText(menu.GetLabel(i));

                    ranges[i] = range;

                    AddViewRange(range);
                }
            }

            if (ranges != null)
            {
                for (int i = 0; i < ranges.Length; i++)
                {
                    ranges[i].SetTop(top + i * itemHeight);
                    ranges[i].SetWidth(width - left);
                    ranges[i].SetHeight(itemHeight);

                    if (menu.index == i)
                    {
                        ranges[i].SetBorder(1);
                        ranges[i].SetPadding(1);
                    }
                    else
                    {
                        ranges[i].SetBorder(0);
                        ranges[i].SetPadding(2);
                    }
                }
            }
        }

        internal void UpdateSelection()
        {
            for (int i = 0; i < ranges.Length; i++)
            {
                if (menu.index == i)
                {
                    ranges[i].SetBorder(1);
                    ranges[i].SetPadding(1);
                }
                else
                {
                    ranges[i].SetBorder(0);
                    ranges[i].SetPadding(2);
                }
            }
        }

        internal void Previous()
        {
            menu.Previous();
            UpdateSelection();
            Render();

            PlaySelected();
        }

        internal void Next()
        {
            menu.Next();
            UpdateSelection();
            Render();

            PlaySelected();
        }

        internal void Select()
        {
            menu.Select();
        }

        internal void Abort()
        {
            menu.Abort();
        }

        internal void PlaySelected()
        {
            Audio.AbortCurrentSound();
            Audio.PlaySound(menu.GetLabel(menu.index));
        }

        public override void HandleInteraction(BrailleIO.Interface.BrailleIO_DeviceButtonStates keyEvent)
        {
            if (keyEvent == BrailleIO_DeviceButtonStates.UpUp)
            {
                Previous();
            }

            if (keyEvent == BrailleIO_DeviceButtonStates.DownUp)
            {
                Next();
            }

            if (keyEvent == BrailleIO_DeviceButtonStates.EnterUp)
            {
                Select();
            }

            if (keyEvent == BrailleIO_DeviceButtonStates.AbortUp)
            {
                Abort();
            }
        }

        protected override void OnShow()
        {
            menu.index = 0;
            UpdateSelection();
            Render();

            PlaySelected();
        }

        protected override void OnHide()
        {
            
        }
    }
}
