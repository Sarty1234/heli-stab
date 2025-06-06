using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace heli_stab
{
    public class RotationDetector
    {
        const int TenDegreePixelCount = 200;

        System.Windows.Point RollStartPoint = new System.Windows.Point(960, 805);
        System.Windows.Point PitchStartPoint = new System.Windows.Point(1365, 540);

        int LeftPixels = 0;
        int RightPixels = 0;
        int TopPixels = 0;
        int BottomPixels = 0;

        Bitmap myBitmap;
        System.Windows.Point ReturnData;

        public System.Windows.Point GetRotation()
        {
            LeftPixels = 0;
            RightPixels = 0;
            TopPixels = 0;
            BottomPixels = 0;


            //myBitmap = new Bitmap("ScreenCapture.png");
            myBitmap = new Bitmap("ScreenCapture.png");

            Color pixelColor = Color.FromArgb(255, 255, 254, 128);


            //Horizontal pixels detection (Roll)
            for (int i = 0; true; i++)
            {
                if (myBitmap.GetPixel((int)RollStartPoint.X - i - 1, (int)RollStartPoint.Y) == pixelColor)
                {
                    LeftPixels++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; true; i++)
            {
                if (myBitmap.GetPixel((int)RollStartPoint.X + i, (int)RollStartPoint.Y) == pixelColor)
                {
                    RightPixels++;
                }
                else
                {
                    break;
                }
            }


            //Vertical pixels detection (Pitch)
            for (int i = 0; true; i++)
            {
                if (myBitmap.GetPixel((int)PitchStartPoint.X, (int)PitchStartPoint.Y - i - 1) == pixelColor)
                {
                    TopPixels++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; true; i++)
            {
                if (myBitmap.GetPixel((int)PitchStartPoint.X, (int)PitchStartPoint.Y + i) == pixelColor)
                {
                    BottomPixels++;
                }
                else
                {
                    break;
                }
            }



            myBitmap.Dispose();

            ReturnData = new System.Windows.Point((double)(RightPixels - LeftPixels) / TenDegreePixelCount * 10, (double)(TopPixels - BottomPixels) / TenDegreePixelCount * 10);
            return ReturnData;
        }
    }
}