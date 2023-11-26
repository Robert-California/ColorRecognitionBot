using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ColorRecognitionBot
{
    public partial class MainWindow : Window
    {
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;


        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Click()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        private void OnButtonSearchPixelClick(object sender, RoutedEventArgs e)
        {
            string inputHexColorCode = TextBoxHexColor.Text;
            SearchPixel(inputHexColorCode);
        }
        private void DoubleClickAtPosition(int posX, int posY)
        {

            SetCursorPos(posX, posY);
            Click();
            Click();
        }
        private bool SearchPixel(string hexcode)
        {
            // Get the primary screen dimensions using SystemParameters
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            // Convert screen dimensions to integer
            int screenIntWidth = Convert.ToInt32(screenWidth);
            int screenIntHeight = Convert.ToInt32(screenHeight);

            // Create a bitmap with screen dimensions
            Bitmap bmp = new Bitmap(screenIntWidth, screenIntHeight);

            Graphics graphics = Graphics.FromImage(bmp as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, bmp.Size);

            System.Drawing.Color desiredPixelColor = ColorTranslator.FromHtml(hexcode);
            for (int i = 0; i < screenIntWidth; i++)
            {
                for (int j = 0; j < screenIntHeight; j++)
                {
                    System.Drawing.Color currentPixelColor = bmp.GetPixel(i, j);
                    if (desiredPixelColor == currentPixelColor)
                    {
                        MessageBox.Show(String.Format("Found pixel at {0}, {1}", i, j));
                        DoubleClickAtPosition(i, j);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
