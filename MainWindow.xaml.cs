using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace heli_stab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Here it includeskeybindevent for helicopter control
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);


        // Declaration of keys
        const int VK_RollLeft = 0x51; //Q
        const int VK_RollRight = 0x45; //E
        const int VK_PitchForward = 0x4F; //O
        const int VK_PitchBackward = 0x50; //P
        const int VK_IncreaseThrust = 0x57; //W
        const int VK_DecreaseThrust = 0x53; //S

        public MainWindow()
        {
            InitializeComponent();

            Console.Beep(2000, 300);

            // --------------------Declaration of variables used in app to avoid huge memory leak--------------------
            const int tpr = 250;        // Time beetwen each cycle

            RotationDetector rotationDetector = new RotationDetector();
            Process[] processes = Process.GetProcessesByName("SquadGame");


            DateTime time = DateTime.Now;
            System.Windows.Point rotation;

            int pitchTime = 0;
            int rollTime = 0;
            int thrustTime = 0;

            double thrust = 0;

            bool IsWorking = false;

            // Declaration of targets to be acomplished
            double targetPitch = 0;
            double tartgetRoll = 0;
            int targetThrust = 45;
            double threshold = 0.2;

            // App infinite cycle
            while (true)
            {
                if (DateTime.Now > time.AddMilliseconds(tpr)) // Check if enouth time passed since last cycle
                {
                    time = DateTime.Now;

                    IsWorking = false;
                    targetPitch = 0;
                    targetThrust = 45;

                    if (Keyboard.IsKeyDown(Key.NumPad1))
                    {
                        IsWorking = true;
                    }

                    if (Keyboard.IsKeyDown(Key.NumPad2))
                    {
                        IsWorking = true;
                        targetPitch = 5;
                        targetThrust = 30;
                    }

                    if (Keyboard.IsKeyDown(Key.NumPad3))
                    {
                        IsWorking = true;
                        targetPitch = -8.9;
                        targetThrust = 60;
                    }

                    if (!IsWorking) continue;  // Check if key is pressed for app to work


                    // Make screenshot and extract all data from it
                    MakeScreenshot();
                    rotation = rotationDetector.GetRotation(out thrust);



                    // Here it sets rotation speed for Roll based on how much is left to target rotation
                    if (tartgetRoll + rotation.X > 1 || tartgetRoll + rotation.X < -1) rollTime = 2;
                    else if (tartgetRoll + rotation.X > 5 || tartgetRoll + rotation.X < -5) rollTime = 2;
                    else if (tartgetRoll + rotation.X > 8 || tartgetRoll + rotation.X < -8) rollTime = 10;

                    // Here it sets rotation speed for Pitch based on how much is left to target rotation
                    if (targetPitch + rotation.Y > 2 || targetPitch + rotation.Y < -2) pitchTime = 10;
                    else if (targetPitch + rotation.Y > 5 || targetPitch + rotation.Y < -5) pitchTime = 20;
                    else if (targetPitch + rotation.Y > 8 || targetPitch + rotation.Y < -8) pitchTime = 40;

                    // Here it sets change speed for Thrust based on how much is left to target thrust
                    if (targetThrust - thrust > 2 || targetThrust - thrust < -2) thrustTime = 50;
                    if (targetThrust - thrust > 5 || targetThrust - thrust < -5) thrustTime = 100;
                    if (targetThrust - thrust > 8 || targetThrust - thrust < -8) thrustTime = 160;



                    // Here it rotates Roll
                    if (tartgetRoll + rotation.X > threshold)
                    {
                        PressKey(rollTime, VK_RollLeft);
                    }
                    else if (tartgetRoll + rotation.X < -threshold)
                    {
                        PressKey(rollTime, VK_RollRight);
                    }


                    // Here it rotates Pitch
                    if (targetPitch + rotation.Y > threshold)
                    {
                        PressKey(pitchTime, VK_PitchForward);
                    }
                    else if (targetPitch + rotation.Y < -threshold)
                    {
                        PressKey(pitchTime, VK_PitchBackward);
                    }


                    // Here it changes Thrust
                    if (targetThrust - thrust > threshold)
                    {
                        PressKey(thrustTime, VK_IncreaseThrust);
                    }
                    else if (targetThrust - thrust < -threshold)
                    {
                        PressKey(thrustTime, VK_DecreaseThrust);
                    }
                }
            }
        }


        private void PressKey(int time, byte Key)
        {
            keybd_event(Key, 0, 0, IntPtr.Zero);

            Thread.Sleep(time);

            keybd_event(Key, 0, 2, IntPtr.Zero);

            Console.Beep(2000, 50);
        }



        // Declaration of data required for MakeScreenshot() to work to avoid memory leaks
        double screenLeft = SystemParameters.VirtualScreenLeft;
        double screenTop = SystemParameters.VirtualScreenTop;
        double screenWidth = SystemParameters.VirtualScreenWidth;
        double screenHeight = SystemParameters.VirtualScreenHeight;
        Bitmap bmp;
        Graphics g;
        String filename = "ScreenCapture.png";

        /// <summary>
        /// This function makes screenshot and saves it with name filename localy
        /// </summary>
        private void MakeScreenshot()
        {
            using (bmp = new Bitmap((int)screenWidth,
                (int)screenHeight))
            {
                using (g = Graphics.FromImage(bmp))
                {
                    Opacity = .0;
                    g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
                    bmp.Save(filename, ImageFormat.Png);
                    bmp.Dispose();
                    Opacity = 1;
                }

            }
        }
    }
}