using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ColorRecognitionBot
{
    public partial class MainWindow : Window
    {
        // Each action has a virtual key key, source:
        //https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
        private const UInt32 MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const UInt32 MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int VK_1 = 0x31; 

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Click()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            // mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            // mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private void OnButtonSearchPixelClick(object sender, RoutedEventArgs e)
        {
            string inputHexColorCode = TextBoxHexColor.Text;
            SearchPixel(inputHexColorCode);
        }

        private void DoubleClickAtPosition(int posX, int posY)
        {
            SetCursorPos(posX, posY);
            //Click();
            Click();
        }

        private bool SearchPixel(string hexcode)
        {
            IntPtr targetWindowHandle = FindWindow(null, "World of Warcraft"); // Replace with the title of your target application

            if (targetWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Target window not found.");
                return false;
            }
            else
            {
                // Get the window text (title)
                StringBuilder windowText = new StringBuilder(256);
                GetWindowText(targetWindowHandle, windowText, windowText.Capacity);
                string windowTitle = windowText.ToString();

                MessageBox.Show($"Target window found: {windowTitle}");
            }

            RECT targetWindowRect;
            GetWindowRect(targetWindowHandle, out targetWindowRect);

            int screenWidth = targetWindowRect.Right - targetWindowRect.Left;
            int screenHeight = targetWindowRect.Bottom - targetWindowRect.Top;

            Bitmap bmp = new Bitmap(screenWidth, screenHeight);

            using (Graphics graphics = Graphics.FromImage(bmp as Image))
            {
                graphics.CopyFromScreen(targetWindowRect.Left, targetWindowRect.Top, 0, 0, bmp.Size);
            }

            Color desiredPixelColor = ColorTranslator.FromHtml(hexcode);

            for (int i = 0; i < screenWidth; i++)
            {
                for (int j = 0; j < screenHeight; j++)
                {
                    Color currentPixelColor = bmp.GetPixel(i, j);

                    if (desiredPixelColor == currentPixelColor)
                    {
                        MessageBox.Show($"Found pixel at {i + targetWindowRect.Left}, {j + targetWindowRect.Top}");
                        DoubleClickAtPosition(i + targetWindowRect.Left, j + targetWindowRect.Top);

                        // Press the '1' key
                        keybd_event(VK_1, 0, 0, IntPtr.Zero);
                        keybd_event(VK_1, 0, 2, IntPtr.Zero);

                        return true;
                    }
                }
            }

            MessageBox.Show("Pixel not found.");
            return false;
        }
    }
}
