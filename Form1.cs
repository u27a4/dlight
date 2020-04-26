using dlight.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace dlight
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Controls.Add(new TransParentLabel()
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(Font.FontFamily, 50f),
                Dock = DockStyle.Fill,
                Text = "light",
            });
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            var s = Settings.Default;

            if (Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(s.Location)))
            {
                Location = s.Location;
            }

            foreach (var screen in Screen.AllScreens)
            {
                var wa = screen.WorkingArea;
                var center = new Point(wa.X + wa.Width / 2, wa.Y + wa.Height / 2);

                var form = new Form();

                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
                form.StartPosition = FormStartPosition.Manual;
                form.Location = center;
                form.BackColor = Color.Black;
                form.Move += Form_Move;
                form.Shown += Form_Move;
                form.DoubleClick += Form_DoubleClick;

                form.Show(this);
            }
        }

        private void Form_DoubleClick(object sender, EventArgs e)
        {
            Close();
        }

        private void Form_Move(object sender, EventArgs e)
        {
            foreach (var form in OwnedForms)
            {
                form.FormBorderStyle = FormBorderStyle.None;
                form.Visible = Screen.FromControl(form).DeviceName != Screen.FromControl(this).DeviceName;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var s = Settings.Default;
            s.Location = WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location;
            s.Save();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;

            if (m.Msg == WM_NCHITTEST)
            {
                if (m.Result.ToInt32() == HTCLIENT)
                {
                    m.Result = (IntPtr)HTCAPTION;
                }
            }
        }
    }

    class TransParentLabel : Label
    {
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            const int WM_NCHITTEST = 0x84;
            const int HTTRANSPARENT = -1;

            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
            }
        }
    }
}
