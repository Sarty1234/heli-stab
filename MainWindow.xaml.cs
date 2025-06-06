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
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        const UInt32 WM_KEYDOWN = 0x0100;
        const UInt32 WM_KEYUP = 0x0202;
        const int VK_RollLeft = 0x51; //Q
        const int VK_RollRight = 0x45; //E
        const int VK_PitchForward = 0x4F; //O
        const int VK_PitchBackward = 0x50; //P

        System.Windows.Point rotation;
        public MainWindow()
        {
            InitializeComponent();

            Console.Beep(2000, 300);

            DateTime time = DateTime.Now;
            const int tpr = 250;

            RotationDetector rotationDetector = new RotationDetector();
            Process[] processes = Process.GetProcessesByName("SquadGame");


            int targetPitch = 0;
            int tartgetRoll = 0;
            double threshold = 1;


            int pitchTime = 0;
            int rollTime = 0;
            while (true)
            {
                if (DateTime.Now > time.AddMilliseconds(tpr))
                {
                    time = DateTime.Now;

                    MakeScreenshot();
                    rotation = rotationDetector.GetRotation();
                    output.Content = rotation.ToString();


                    if (Keyboard.IsKeyDown(Key.N))
                    {
                        targetPitch = 5;
                    } else
                    {
                        targetPitch = 0;
                    }


                    if (tartgetRoll + rotation.X > 1 || tartgetRoll + rotation.X < -1) rollTime = 2;
                    else if (tartgetRoll + rotation.X > 5 || tartgetRoll + rotation.X < -5) rollTime = 2;
                    else if (tartgetRoll + rotation.X > 8 || tartgetRoll + rotation.X < -8) rollTime = 10;

                    if (targetPitch + rotation.Y > 2 || targetPitch + rotation.Y < -2) pitchTime = 2;
                    else if (targetPitch + rotation.Y > 5 || targetPitch + rotation.Y < -5) pitchTime = 10;




                    if (tartgetRoll + rotation.X > threshold)
                    {
                        keybd_event(VK_RollLeft, 0, 0, IntPtr.Zero);

                        Thread.Sleep(rollTime);

                        keybd_event(VK_RollLeft, 0, 2, IntPtr.Zero);

                        Console.Beep(2000, 50);
                    }
                    else if (tartgetRoll + rotation.X < -threshold)
                    {
                        keybd_event(VK_RollRight, 0, 0, IntPtr.Zero);

                        Thread.Sleep(rollTime);

                        keybd_event(VK_RollRight, 0, 2, IntPtr.Zero);

                        Console.Beep(2000, 50);
                    }


                    if (targetPitch + rotation.Y > threshold)
                    {
                        keybd_event(VK_PitchForward, 0, 0, IntPtr.Zero);

                        Thread.Sleep(pitchTime);

                        keybd_event(VK_PitchForward, 0, 2, IntPtr.Zero);

                        Console.Beep(2000, 50);
                    }
                    else if (targetPitch + rotation.Y < -threshold)
                    {
                        keybd_event(VK_PitchBackward, 0, 0, IntPtr.Zero);

                        Thread.Sleep(pitchTime);

                        keybd_event(VK_PitchBackward, 0, 2, IntPtr.Zero);

                        Console.Beep(2000, 50);
                    }
                }
            }
        }




        double screenLeft = SystemParameters.VirtualScreenLeft;
        double screenTop = SystemParameters.VirtualScreenTop;
        double screenWidth = SystemParameters.VirtualScreenWidth;
        double screenHeight = SystemParameters.VirtualScreenHeight;
        Bitmap bmp;
        Graphics g;
        String filename;
        private void MakeScreenshot()
        {
            using (bmp = new Bitmap((int)screenWidth,
                (int)screenHeight))
            {
                using (g = Graphics.FromImage(bmp))
                {
                    filename = "ScreenCapture.png";
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