﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace X_Masputer
{
    public partial class Preview : Form
    {
        private readonly int _screen = 0;
        private List<PictureBox> _lights = new List<PictureBox>();
        private List<Bitmap> _arrayTemplate = new List<Bitmap>();
        private int _arrayTemplateIterStartIndex = 0;

        public Preview()
        {
            InitializeComponent();
        }

        public Preview(int screenIndex) {
            _screen = screenIndex;
            InitializeComponent();
            this.Opacity = 0;

            int maxLights = Screen.AllScreens[_screen].Bounds.Width / LightPB.Width;
            _lights.Add(LightPB);
            for (int i = 0; i < maxLights; i++)
            {
                PictureBox light = new PictureBox();
                light.Image = LightPB.Image;
                light.Size = LightPB.Size;
                light.Location = new Point(LightPB.Location.X + i * LightPB.Width, LightPB.Location.Y);
                light.Visible = true;
                light.SizeMode = PictureBoxSizeMode.Zoom;
                light.BackColor = Color.Transparent;
                this.Controls.Add(light);
                _lights.Add(light);
            }

            Mode1();
        }

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;

        private void MakeWindowUnclickable()
        {
            IntPtr hWnd = this.Handle;

            // Call current window styles and add Layered/Transparent
            int extendedStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        private void SetAlwaysOnTop(IntPtr hWnd)
        {
            // Set window above all other windows
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        private void AlwaysOnTop_Tick(object sender, EventArgs e)
        {
            SetAlwaysOnTop(FindWindow(null, "Form1"));
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            int x = Screen.AllScreens[_screen].WorkingArea.Left;
            int y = Screen.AllScreens[_screen].WorkingArea.Bottom;
            this.Location = new Point(x, y);
            this.Width = Screen.AllScreens[_screen].WorkingArea.Width;
            this.Height = Screen.AllScreens[_screen].Bounds.Height - Screen.AllScreens[_screen].WorkingArea.Height;
            this.Opacity = 1;
            MakeWindowUnclickable();
        }

        private void ColorChanger_Tick(object sender, EventArgs e)
        {
            if (_arrayTemplate == null || _arrayTemplate.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _lights.Count; i++)
            {
                int nextImageIndex = (_arrayTemplateIterStartIndex + i) % _arrayTemplate.Count;

                _lights[i].Image = _arrayTemplate[nextImageIndex];
            }

            _arrayTemplateIterStartIndex--;

            if (_arrayTemplateIterStartIndex >= _arrayTemplate.Count)
            {
                _arrayTemplateIterStartIndex = 0;
            }

            if (_arrayTemplateIterStartIndex < 0)
            {
                _arrayTemplateIterStartIndex = _arrayTemplate.Count - 1;
            }
        }

        public void ChangeTime(LightSpeed speed)
        {
            if (speed == LightSpeed.None)
            {
                ColorChanger.Enabled = false;
                return;
            }
            else
            {
                ColorChanger.Enabled = true;
            }

            ColorChanger.Interval = (int)speed;
        }

        public void Mode1()
        {
            _arrayTemplate.Clear();
            _arrayTemplate.Add(Lights.Light_Effect_Blue);
            _arrayTemplate.Add(Lights.Light_Effect_Red);
            _arrayTemplate.Add(Lights.Light_Effect_Green);
            _arrayTemplate.Add(Lights.Light_Effect_Yellow);
        }

        public void Mode2()
        {
            _arrayTemplate.Clear();
            _arrayTemplate.Add(Lights.Light_Effect_Blue);
        }

        public void Mode3()
        {
            _arrayTemplate.Clear();
            _arrayTemplate.Add(Lights.Light_Effect_Red);
        }

        public void Mode4()
        {
            _arrayTemplate.Clear();
            _arrayTemplate.Add(Lights.Light_Effect_Green);
        }

        public void Mode5()
        {
            _arrayTemplate.Clear();
            _arrayTemplate.Add(Lights.Light_Effect_Yellow);
        }

        public void Mode6()
        {
            _arrayTemplate.Clear();
            _arrayTemplate.Add(Lights.Light_Effect_Blue);
            _arrayTemplate.Add(Lights.Light_Effect_Red);
        }

        public void Mode7()
        {
            _arrayTemplate.Clear();
            _arrayTemplate.Add(Lights.Light_Effect_Green);
            _arrayTemplate.Add(Lights.Light_Effect_Yellow);
        }

        public void Mode8()
        {
            _arrayTemplate.Clear();

            var colors = new List<Bitmap>
            {
                Lights.Light_Effect_Blue,
                Lights.Light_Effect_Red,
                Lights.Light_Effect_Green,
                Lights.Light_Effect_Yellow
            };

            Random random = new Random((int)DateTime.UtcNow.Ticks);

            for (int i = 0; i < colors.Count; i++)
            {
                int groupSize = random.Next(3, 6);

                for (int j = 0; j < groupSize; j++)
                {
                    _arrayTemplate.Add(colors[i]);
                }
            }
        }
    }

    public enum LightSpeed
    {
        None = 0,
        VerySlow = 2000,
        Slow = 1000,
        Medium = 500,
        Fast = 250,
        VeryFast = 100
    }
}
