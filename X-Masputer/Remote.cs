using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        
        // For --autostart only
        private Mode _mode = Mode.Mode1;
        private double _opacity = 1;
        private bool _isOn = false;
        private LightSpeed _currentSpeed = LightSpeed.Slow;
        private bool _efficientAlwaysOnTop = true;

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
            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Visible = true,
                Text = "X-Masputer"
            };

            notifyIcon.DoubleClick += (s, e2) => this.Show();

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Open", (s, e2) => this.Show());

            contextMenu.MenuItems.Add("About", (s, e2) =>
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                string description = "X-Masputer is a little christmas project to give your computer a little christmas feeling. It allows you to control little christmas lights on top of the taskbar.";
                MessageBox.Show($"X-Masputer{Environment.NewLine}" +
                $"Version: {version}{Environment.NewLine}{Environment.NewLine}" +
                $"Developed by: Fabian Schlüter{Environment.NewLine}" +
                $"© {DateTime.Now.Year} Fabian Schlüter. All rights reserved.{Environment.NewLine}{Environment.NewLine}" +
                $"{description}",
                "About X-Masputer");
            });

            // Toggle Menu
            MenuItem efficienceMenu = new MenuItem("Efficience");
            MenuItem onMenuItem = new MenuItem("On");
            MenuItem offMenuItem = new MenuItem("Off");

            // Initial state
            if (Properties.Settings.Default.EfficientAlwaysOnTop)
            {
                onMenuItem.Text = "• On";
                offMenuItem.Text = "Off";
            }
            else
            {
                onMenuItem.Text = "On";
                offMenuItem.Text = "• Off";
            }

            // Add click events
            onMenuItem.Click += (s, e2) =>
            {
                if (!_efficientAlwaysOnTop)
                {
                    _efficientAlwaysOnTop = true;
                    _preview.ForEach(p => p.EfficientAlwaysOnTop());

                    // Update toggle indicators
                    onMenuItem.Text = "• On";
                    offMenuItem.Text = "Off";

                    SaveSettings();
                }
            };

            offMenuItem.Click += (s, e2) =>
            {
                if (_efficientAlwaysOnTop)
                {
                    _efficientAlwaysOnTop = false;
                    _preview.ForEach(p => p.InefficientAlwaysOnTop());

                    // Update toggle indicators
                    onMenuItem.Text = "On";
                    offMenuItem.Text = "• Off";

                    SaveSettings();
                }
            };

            // Add items to the efficience menu
            efficienceMenu.MenuItems.Add(onMenuItem);
            efficienceMenu.MenuItems.Add(offMenuItem);

            contextMenu.MenuItems.Add(efficienceMenu);

            // Exit Menu
            contextMenu.MenuItems.Add("Exit", (s, e2) =>
            {
                Process.GetCurrentProcess().Kill(); // Because Application.Exit() doesn't exit threads (Whyever...)
            });

            // Assign context menu
            notifyIcon.ContextMenu = contextMenu;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Mode = (int)_mode;
            Properties.Settings.Default.Opacity = _opacity;
            Properties.Settings.Default.IsOn = _isOn;
            Properties.Settings.Default.Speed = (int)_currentSpeed;
            Properties.Settings.Default.CurrentSpeedIndex = _currentSpeedIndex;
            Properties.Settings.Default.EfficientAlwaysOnTop = _efficientAlwaysOnTop;
            Properties.Settings.Default.Save();
        }

        private async Task LoadSettings()
        {
            // IsOn
            if (Properties.Settings.Default.IsOn)
            {
                OnBtn_Click(null, EventArgs.Empty); // On automatically saves in this method, so no extra save needed
            }
            else
            {
                MinimizeBtn_Click(null, EventArgs.Empty);
                return;
            }

            // Mode
            int mode = Properties.Settings.Default.Mode;
            switch ((Mode)Properties.Settings.Default.Mode)
            {
                case Mode.Mode1:
                    Num1Btn_Click(null, EventArgs.Empty); // Mode automatically saves in this method(s), so no extra save needed
                    break;
                case Mode.Mode2:
                    Num2Btn_Click(null, EventArgs.Empty);
                    break;
                case Mode.Mode3:
                    Num3Btn_Click(null, EventArgs.Empty);
                    break;
                case Mode.Mode4:
                    Num4Btn_Click(null, EventArgs.Empty);
                    break;
                case Mode.Mode5:
                    Num5Btn_Click(null, EventArgs.Empty);
                    break;
                case Mode.Mode6:
                    Num6Btn_Click(null, EventArgs.Empty);
                    break;
                case Mode.Mode7:
                    Num7Btn_Click(null, EventArgs.Empty);
                    break;
                case Mode.Mode8:
                    Num8Btn_Click(null, EventArgs.Empty);
                    break;
                default:
                    Num1Btn_Click(null, EventArgs.Empty);
                    break;
            }

            // Opacity
            await Task.Delay(1100);
            _preview.ForEach(p => p.Opacity = Properties.Settings.Default.Opacity);
            _opacity = Properties.Settings.Default.Opacity;

            // Speed
            _currentSpeedIndex = Properties.Settings.Default.CurrentSpeedIndex;

            _currentSpeed = _lightSpeeds[_currentSpeedIndex];

            _preview.ForEach(p => p.ChangeTime(_currentSpeed));

            await Task.Delay(1500);
            // EfficientAlwaysOnTop
            if (Properties.Settings.Default.EfficientAlwaysOnTop)
            {
                _preview.ForEach(p => p.EfficientAlwaysOnTop());
            }
            else
            {
                _preview.ForEach(p => p.InefficientAlwaysOnTop());
            }
            _efficientAlwaysOnTop = Properties.Settings.Default.EfficientAlwaysOnTop;


            MinimizeBtn_Click(null, EventArgs.Empty);
        }

        private static void RoundButtonEdges(Button button)
        {
            int diameter = Math.Min(button.Width, button.Height);
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

            if (sender is not null)
            {
                _currentSpeed = LightSpeed.Slow;
                _currentSpeedIndex = _lightSpeeds.ToList().IndexOf(LightSpeed.Slow);
            }

            _isOn = true;
            if (sender is not null)
            {
                SaveSettings();
            }
            this.Focus();
        }

        private void TimerBtn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any())
            {
                return;
            }

            _preview.ForEach(p => p.ChangeTime(_lightSpeeds[_currentSpeedIndex]));

            _currentSpeed = _lightSpeeds[_currentSpeedIndex];

            _currentSpeedIndex = (_currentSpeedIndex + 1) % _lightSpeeds.Length;

            if (sender is not null)
            {
                SaveSettings();
            }

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
            _isOn = false;

            if (sender is not null)
            {
                SaveSettings();
            }
            this.Focus();
        }

        private void Num1Btn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any()) {
                return;
            }

            _preview.ForEach(p => p.Mode1());
            _mode = Mode.Mode1;
            
            if (sender is not null)
            {
                SaveSettings();
            }
            this.Focus();
        }

        private void Num2Btn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any())
            {
                return;
            }

            _preview.ForEach(p => p.Mode2());
            _mode = Mode.Mode2;

            if (sender is not null)
            {
                SaveSettings();
            }
            this.Focus();
        }

        private void Num3Btn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any())
            {
                return;
            }

            _preview.ForEach(p => p.Mode3());
            _mode = Mode.Mode3;

            if (sender is not null)
            {
                SaveSettings();
            }
            this.Focus();
        }

        private void Num4Btn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any())
            {
                return;
            }

            _preview.ForEach(p => p.Mode4());
            _mode = Mode.Mode4;

            if (sender is not null)
            {
                SaveSettings();
            }
            this.Focus();
        }

        private void Num5Btn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any())
            {
                return;
            }

            _preview.ForEach(p => p.Mode5());
            _mode = Mode.Mode5;

            if (sender is not null)
            {
                SaveSettings();
            }
            this.Focus();
        }

        private void Num6Btn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any())
            {
                return;
            }

            _preview.ForEach(p => p.Mode6());
            _mode = Mode.Mode6;

            if (sender is not null)
            {
                SaveSettings();
            }
            this.Focus();
        }

        private void Num7Btn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any())
            {
                return;
            }

            _preview.ForEach(p => p.Mode7());
            _mode = Mode.Mode7;

            if (sender is not null)
            {
                SaveSettings();
            }
            this.Focus();
        }

        private void Num8Btn_Click(object sender, EventArgs e)
        {
            if (!_preview.Any())
            {
                return;
            }

            _preview.ForEach(p => p.Mode8());
            _mode = Mode.Mode8;

            if (sender is not null)
            {
                SaveSettings();
            }
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

                _opacity = p.Opacity;
            });
            
            if (sender is not null)
            {
                SaveSettings();
            }

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

                _opacity = p.Opacity;
            });

            if (sender is not null)
            {
                SaveSettings();
            }

            this.Focus();
        }

        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Focus();
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill(); // Because Application.Exit() doesn't exit threads (Whyever...)
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

        private async void Remote_Shown(object sender, EventArgs e)
        {
            if (Environment.GetCommandLineArgs().Contains("--autostart"))
            {
                await LoadSettings();
            }
        }
    }

    enum Mode
    {
        Mode1,
        Mode2,
        Mode3,
        Mode4,
        Mode5,
        Mode6,
        Mode7,
        Mode8
    }
}
