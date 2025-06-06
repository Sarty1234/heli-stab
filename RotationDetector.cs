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
        // Number of pixels per full loadbar
        const int TenDegreePixelCount = 200;
        const int HundredThrustPixelCount = 380;

        // Start positions of heli loadbars
        System.Windows.Point RollStartPoint = new System.Windows.Point(960, 805);
        System.Windows.Point PitchStartPoint = new System.Windows.Point(1365, 540);
        System.Windows.Point ThrustStartPoint = new System.Windows.Point(524, 712);  //(524; 332) - endpoint

        // Data required for GetRotation() to work
        int LeftPixels = 0;
        int RightPixels = 0;
        int TopPixels = 0;
        int BottomPixels = 0;
        int ThrustPixels = 0;

        Bitmap myBitmap;
        System.Windows.Point ReturnData;

        public String filename = "ScreenCapture.png";

        /// <summary>
        /// This function gathers data from screenshot {filename} and returns it as (Roll; Pitch)
        /// </summary>
        /// <param name="thrustOutput">Outputs thrust value</param>
        /// <returns></returns>
        public System.Windows.Point GetRotation(out double thrustOutput)
        {
            // Here it resets variables
            LeftPixels = 0;
            RightPixels = 0;
            TopPixels = 0;
            BottomPixels = 0;
            ThrustPixels = 0;


            myBitmap = new Bitmap(filename);
            //s = new Bitmap("E:\\c#_projects\\heli stab\\photos\\Screenshot (435).png");

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


            //Thrust detection
            for (int i = 0; true; i++)
            {
                if (myBitmap.GetPixel((int)ThrustStartPoint.X, (int)ThrustStartPoint.Y - i) == pixelColor)
                {
                    ThrustPixels++;
                }
                else
                {
                    break;
                }
            }


            myBitmap.Dispose();  // It is very important to dispose of bitmap because if you don't there will be file access error

            // Calculating and returning output data
            thrustOutput = (double)ThrustPixels / (double)HundredThrustPixelCount * 100;
            ReturnData = new System.Windows.Point((double)(RightPixels - LeftPixels) / TenDegreePixelCount * 10, (double)(TopPixels - BottomPixels) / TenDegreePixelCount * 10);
            return ReturnData;
        }
    }
}