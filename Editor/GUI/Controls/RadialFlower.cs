using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Editor.GUI.Controls
{
    public class RadialFlower : RadialMenu
    {
        public float EnlargeFactor = 1.1f;
        
        public int SelectionIndex { get; private set; }

        public RectangleF OuterRectEnlarged => new RectangleF(
            Position.X - Radius * EnlargeFactor, Position.Y - Radius * EnlargeFactor, 
            Radius * 2 * EnlargeFactor, Radius * 2 * EnlargeFactor);
        
        public RectangleF InnerRectEnlarged => new RectangleF(
            Position.X - Radius * EnlargeFactor * 0.5f, Position.Y - Radius * EnlargeFactor * 0.5f, 
            Radius * EnlargeFactor, Radius * EnlargeFactor);
        
        public Dictionary<string, Image> Elements = new Dictionary<string, Image>
        {
            {"option 0", null},
            {"option 1", null},
            {"option 2", null},
            {"option 3", null},
            {"option 4", null},
            {"option 5", null},
            {"option 6", null}
        };
        
        public RadialFlower() : base()
        {
            SelectionIndex = -1;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (!_paint) return;

            int i = 0;
            foreach (var el in Elements)
            {
                float begAngle = (float)((2 * Math.PI * i) / Elements.Count);
                float endAngle = (float)((2 * Math.PI * (i + 1)) / Elements.Count);

                bool selected = SelectionIndex == i;

                float angleOffset = (float)((2 * Math.PI * 0.05) / Elements.Count);

                GraphicsPath petal = new GraphicsPath();

                petal.StartFigure();
                petal.AddArc(selected ? OuterRectEnlarged : OuterRect,
                    (float)((begAngle + angleOffset) / Math.PI * 180 - 90f),
                    (float)((endAngle - begAngle - 2*angleOffset) / Math.PI * 180));
                petal.AddArc(selected ? InnerRectEnlarged : InnerRect,
                    (float)((endAngle - angleOffset) / Math.PI * 180 - 90f),
                    (float)((begAngle - endAngle + 2*angleOffset) / Math.PI * 180));
                petal.CloseFigure();

                pe.Graphics.FillPath(new SolidBrush(Color.DimGray), petal);
                petal.Dispose();

                if (selected)
                {
                    Font f = new Font("Arial", 20, FontStyle.Bold);
                    Brush b = new SolidBrush(Color.Black);

                    SizeF s = pe.Graphics.MeasureString(el.Key, f);

                    float midAngle = (begAngle + endAngle)/2f;
                    float horOffset = 2*i >= Elements.Count ? s.Width : 0f;
                    float vertOffset = s.Height / 2;

                    float x = Position.X + (float)Math.Sin(midAngle) * (Radius + 30) - horOffset;
                    float y = Position.Y - (float) Math.Cos(midAngle) * (Radius + 30) - vertOffset;

                    pe.Graphics.FillRectangle(new SolidBrush(Color.DimGray),
                        x, y, s.Width, s.Height); 

                    pe.Graphics.DrawString(el.Key, f, b, new PointF(x, y));

                    f.Dispose();
                    b.Dispose();
                }

                i++;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!Visible) SelectionIndex = -1;
        }

        protected override EventArgs ConstructResultArgs()
        {
            return new ClosedGracefullyEventArgs(SelectionIndex);
        }

        public override void RecalculateSelection()
        {
            float angle = (float)(Math.Atan2(MousePosition.Y - Position.Y, MousePosition.X - Position.X) + Math.PI / 2);
            if (angle < 0) angle += (float)Math.PI * 2;
            SelectionIndex = (int)Math.Floor(angle / (2 * Math.PI) * Elements.Count);

            if (Math.Sqrt((MousePosition.X - Position.X)*(MousePosition.X - Position.X) +
                          (MousePosition.Y - Position.Y)*(MousePosition.Y - Position.Y)) < Radius/2d) SelectionIndex = -1;
        }

        public class ClosedGracefullyEventArgs : EventArgs
        {
            public int SelectionIndex;

            public ClosedGracefullyEventArgs(int ind)
            {
                SelectionIndex = ind;
            }
        }
    }
}
