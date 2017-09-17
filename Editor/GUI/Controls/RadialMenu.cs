using System;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;

namespace Editor.GUI.Controls
{
    public abstract class RadialMenu : Form
    {
        public int Radius = 100;
        
        public Point Position;
        public RectangleF OuterRect => new RectangleF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
        public RectangleF InnerRect => new RectangleF(Position.X - Radius * 0.5f, Position.Y - Radius * 0.5f, Radius, Radius);

        protected override bool ShowWithoutActivation => true;
        protected bool _paint = true;

        public delegate void ClosedGracefullyEvent(object sender, EventArgs e);
        public event ClosedGracefullyEvent ClosedGracefully;

        public delegate void CanceledEvent(object sender, EventArgs e);
        public event CanceledEvent Canceled;

        private long _lastRefresh;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOPMOST = 0x00000008;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= WS_EX_TOPMOST | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
                return createParams;
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            const int WM_MOUSEACTIVATE = 0x21;
            const int MA_NOACTIVATE = 0x0003;

            switch (m.Msg)
            {
                case WM_MOUSEACTIVATE:
                    m.Result = (IntPtr)MA_NOACTIVATE;
                    return;
            }
            base.DefWndProc(ref m);
        }

        protected RadialMenu()
        {
            SuspendLayout();

            AutoScaleMode = AutoScaleMode.Font;
            Name = "RadialMenu";
            Text = "RadialMenu";

            FormBorderStyle = FormBorderStyle.None;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);
            TransparencyKey = Color.Lime;
            ShowInTaskbar = false;

            DesktopBounds = Screen.GetBounds(MousePosition);

            ResumeLayout(false);
        }

        public override void Refresh()
        {
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastRefresh < 10) return;

            base.Refresh();
            _lastRefresh = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Lime);
        }

        protected virtual EventArgs ConstructResultArgs()
        {
            return EventArgs.Empty;
        }

        public virtual void RecalculateSelection()
        {
            
        }

        public static RadialMenu Current;

        static RadialMenu()
        {
            WinapiAccess.GlobalMouseMove += (s, e) =>
            {
                if (Current == null) return;
                Current.RecalculateSelection();
                Current.Refresh();
            };
        }

        public static void ShowNew(RadialMenu rm, Point pos)
        {
            if (Current != null) return;
            if (rm == null) return;

            Current = rm;
            Current.Position = pos;
            Current._paint = true;
            Current.Show();
        }

        public static void CanceledCurrent()
        {
            if (Current == null) return;

            Current.Canceled?.Invoke(null, EventArgs.Empty);

            Current._paint = false;
            Current.Refresh();
            Current.Hide();
            Current = null;
        }

        public static void CloseGracefullyCurrent()
        {
            if (Current == null) return;
            
            Current.ClosedGracefully?.Invoke(null, Current.ConstructResultArgs());

            Current._paint = false;
            Current.Refresh();
            Current.Hide();
            Current = null;
        }
    }
}
