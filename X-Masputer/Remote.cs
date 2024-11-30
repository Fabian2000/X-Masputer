using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X_Masputer
{
    public partial class Remote : Form
    {
        private List<Preview> _preview = new List<Preview>();
        private readonly int _maxScreens = Screen.AllScreens.Length;
        private readonly LightSpeed[] _lightSpeeds;
        private int _currentSpeedIndex = 0;

        public Remote()
        {
            InitializeComponent();
            _lightSpeeds = (LightSpeed[])Enum.GetValues(typeof(LightSpeed));
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private void Remote_Load(object sender, EventArgs e)
        {
            int radius = 70;
            IntPtr hRgn = CreateRoundRectRgn(2, 0, (int)this.Width, (int)this.Height - 2, radius, radius);
            SetWindowRgn(this.Handle, hRgn, true);

            this.MouseDown += (s, e2) =>
            {
                if (e2.Button == MouseButtons.Left)
                {
                    var handle = this.Handle;
                    ReleaseCapture();
                    SendMessage(handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
                }
            };

            RoundButtonEdges(OnBtn);
            RoundButtonEdges(TimerBtn);
            RoundButtonEdges(OffBtn);
            RoundButtonEdges(Num1Btn);
            RoundButtonEdges(Num2Btn);
            RoundButtonEdges(Num3Btn);
            RoundButtonEdges(Num4Btn);
            RoundButtonEdges(Num5Btn);
            RoundButtonEdges(Num6Btn);
            RoundButtonEdges(Num7Btn);
            RoundButtonEdges(Num8Btn);
            RoundButtonEdges(DimDownBtn);
            RoundButtonEdges(DimUpBtn);
            LittleRoundButtonEdges(MinimizeBtn);
            LittleRoundButtonEdges(CloseBtn);

            MinimizeBtn.Visible = false;
            CloseBtn.Visible = false;

            // Notification Icon
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Icon = this.Icon;
            notifyIcon.Visible = true;
            notifyIcon.Text = "X-Masputer";
            notifyIcon.DoubleClick += (s, e2) =>
            {
                this.Show();
            };
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Open", (s, e2) =>
            {
                this.Show();
            });
            contextMenu.MenuItems.Add("About", (s, e2) =>
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                string description = "X-Masputer is a little christmal project to give your computer a little christmas feeling. It allows you to control little christmas lights on top of the taskbar.";
                MessageBox.Show($"X-Masputer{Environment.NewLine}" +
                $"Version: {version}{Environment.NewLine}{Environment.NewLine}" +
                $"Developed by: Fabian Schlüter{Environment.NewLine}" +
                $"© {DateTime.Now.Year} Fabian Schlüter. All rights reserved.{Environment.NewLine}{Environment.NewLine}" +
                $"{description}",
                "About X-Masputer");
            });
            contextMenu.MenuItems.Add("Exit", (s, e2) =>
            {
                Application.Exit();
            });
            notifyIcon.ContextMenu = contextMenu;
        }

        private static void RoundButtonEdges(Button button)
        {
            int diameter = Math.Min(button.Width, button.Height); // Perfekte Rundung basierend auf der kleineren Dimension
            IntPtr rgn = CreateRoundRectRgn(0, 0, button.Width, button.Height, diameter, diameter);
            button.Region = Region.FromHrgn(rgn);
        }

        private static void LittleRoundButtonEdges(Button button)
        {
            IntPtr rgn = CreateRoundRectRgn(0, 0, button.Width, button.Height, 3, 3);
            button.Region = Region.FromHrgn(rgn);
        }

        private void OnBtn_Click(object sender, EventArgs e)
        {
            if (_preview.Any())
            {
                return;
            }

            for (int i = 0; i < _maxScreens; i++)
            {
                _preview.Add(new Preview(i));
                _preview[i].Show();
            }
            this.Focus();
        }

        private void TimerBtn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p =>
            {
                if (_currentSpeedIndex >= _lightSpeeds.Length - 1)
                {
                    _currentSpeedIndex = 0;
                }

                p.ChangeTime(_lightSpeeds[_currentSpeedIndex++]);
            });
            this.Focus();
        }

        private void OffBtn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any())
            {
                return;
            }

            _preview.ForEach(preview => preview.Close());
            _preview.Clear();
            this.Focus();
        }

        private void Num1Btn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p => p.Mode1());
            this.Focus();
        }

        private void Num2Btn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p => p.Mode2());
            this.Focus();
        }

        private void Num3Btn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p => p.Mode3());
            this.Focus();
        }

        private void Num4Btn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p => p.Mode4());
            this.Focus();
        }

        private void Num5Btn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p => p.Mode5());
            this.Focus();
        }

        private void Num6Btn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p => p.Mode6());
            this.Focus();
        }

        private void Num7Btn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p => p.Mode7());
            this.Focus();
        }

        private void Num8Btn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p => p.Mode8());
            this.Focus();
        }

        private void DimDownBtn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p=>
            {
                if (p.Opacity >= 0.5)
                {
                    p.Opacity -= 0.1;
                }
            });

            this.Focus();
        }

        private void DimUpBtn_Click(object sender, EventArgs e)
        {
            _preview.ForEach(p =>
            {
                if (p.Opacity <= 1)
                {
                    p.Opacity += 0.1;
                }
            });

            this.Focus();
        }

        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Focus();
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Remote_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Y < 24)
            {
                MinimizeBtn.Visible = true;
                CloseBtn.Visible = true;
            }
            else
            {
                MinimizeBtn.Visible = false;
                CloseBtn.Visible = false;
            }
        }
    }
}
