using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.Properties;

namespace TactileGame.RPG
{
    class BooleanTexture
    {
        public byte[,] Data 
        {
            get { return _data; }
            set { _data = value; }
        }

        private byte[,] _data;

        public static BooleanTexture FromBitmap(Bitmap bitmap)
        {
            BooleanTexture result = new BooleanTexture();


            result.Data = new byte[bitmap.Width, bitmap.Height];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var color = bitmap.GetPixel(x, y);

                    if (color.B > 0)
                    {
                        result.Data[x, y] = 0;
                    }
                    else if (color.G > 0)
                    {
                        result.Data[x, y] = 1;
                    }
                    else
                    {
                        result.Data[x, y] = 2;
                    }
                }
            }

            return result;
        }

    }
}
