using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Models;
namespace TactileGame.RPG
{
    class BooleanCanvas 
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

		public int X { get; set; }

		public int Y { get; set; }

        private bool[,] _values;

        public bool[,] Data
        {
            get { return _values; }
        }

        public BooleanCanvas(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this._values = new bool[height, width];
        }

        public void Clear()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _values[j, i] = false;
                }
            }
        }

        public void Set(int x, int y, bool value)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                _values[y, x] = value;
            }
        }



        public bool Get(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                return _values[y, x];
            }

            return false;
        }

        public void Draw(WorldObject worldObject)
        {
            if (Overlaps(worldObject))
            {
                for (int i = 0; i < worldObject.Width; i++)
                {
                    for (int j = 0; j < worldObject.Height; j++)
                    {
                        if (worldObject.Pattern[i, j] == 2)
                        {
                            Set(i + worldObject.X - X, j + worldObject.Y - Y, true);
                        }

                        if (worldObject.Pattern[i, j] == 1)
                        {
                            Set(i + worldObject.X - X, j + worldObject.Y - Y, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the canvas overlaps the passed rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool Overlaps(int x, int y, int width, int height)
        {
            return (x <= X + Width - 1 && X <= x + width - 1) && (y <= Y + Height - 1 && Y <= y + height - 1);
        }

        /// <summary>
        /// Checks whether the canvas overlaps the passed object
        /// </summary>
        /// <param name="worldObject"></param>
        /// <returns></returns>
        public bool Overlaps(WorldObject worldObject)
        {
            return Overlaps(worldObject.X, worldObject.Y, worldObject.Width, worldObject.Height);
        }

        public override bool Equals(object obj)
        {
            if (obj is BooleanCanvas)
            {
                BooleanCanvas other = obj as BooleanCanvas;

                if (other.Width == Width && other.Height == Height)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Height; j++)
                        {
                            if (_values[j, i] != other._values[j, i])
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

    }
}
