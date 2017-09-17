using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace Editor.GUI.Controls
{
    public class RadialShell : RadialMenu
    {
        public RectangleF MiddleRect => new RectangleF(Position.X - Radius * 0.4f, Position.Y - Radius * 0.4f, Radius * 0.8f, Radius * 0.8f);

        public int WiggleAngle;

        public int MinWiggleAngle;
        public int MaxWiggleAngle;

        public Func<int, string> DisplayFunc = DefaultDisplayFunc;
        public static readonly Func<int, string> DefaultDisplayFunc = (x) => x.ToString();

        public RadialShell() : base()
        {
            MaxWiggleAngle = 100;
            WiggleAngle = MaxWiggleAngle;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (!_paint) return;

            GraphicsPath center = new GraphicsPath();
            center.StartFigure();
            center.AddArc(MiddleRect, 0, 360);
            center.AddArc(InnerRect, 360, -360);
            center.CloseFigure();
            pe.Graphics.FillPath(new SolidBrush(Color.DimGray), center);
            center.Dispose();

            double angle = ((double)WiggleAngle / (MaxWiggleAngle - MinWiggleAngle)) * Math.PI * 2;

            GraphicsPath wiggle = new GraphicsPath();
            wiggle.StartFigure();
            wiggle.AddArc(OuterRect, -90, (float)(angle/Math.PI*180));
            wiggle.AddArc(MiddleRect, (float)(angle / Math.PI * 180) - 90, -(float)(angle / Math.PI * 180));
            wiggle.CloseFigure();
            pe.Graphics.FillPath(new SolidBrush(Color.DimGray), wiggle);
            wiggle.Dispose();

            string str = DisplayFunc(WiggleAngle + MinWiggleAngle);

            Font f = new Font("Arial", 20, FontStyle.Bold);
            Brush b = new SolidBrush(Color.Black);
            SizeF s = pe.Graphics.MeasureString(str, f);

            float x = Position.X - s.Width / 2f;
            float y = Position.Y - Radius - 20f - s.Height;

            pe.Graphics.FillRectangle(new SolidBrush(Color.DimGray), x, y, s.Width, s.Height);
            pe.Graphics.DrawString(str, f, b, new PointF(x, y));
            f.Dispose();
            b.Dispose();
        }

        protected override EventArgs ConstructResultArgs()
        {
            return new ClosedGracefullyEventArgs(WiggleAngle + MinWiggleAngle);
        }

        public override void RecalculateSelection()
        {
            double angle = Math.Atan2(MousePosition.Y - Position.Y, MousePosition.X - Position.X) + Math.PI / 2;
            if (angle < 0) angle += Math.PI * 2;

            if (Math.Sqrt((MousePosition.X - Position.X) * (MousePosition.X - Position.X) +
                          (MousePosition.Y - Position.Y) * (MousePosition.Y - Position.Y)) >= Radius / 2d)
                WiggleAngle = (int)Math.Round((angle / (Math.PI * 2)) * (MaxWiggleAngle - MinWiggleAngle));
        }

        public class ClosedGracefullyEventArgs : EventArgs
        {
            public double WiggleAngle;

            public ClosedGracefullyEventArgs(double angle)
            {
                WiggleAngle = angle;
            }
        }
    }
}
