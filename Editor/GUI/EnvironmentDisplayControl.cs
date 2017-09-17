using System;
using System.CodeDom;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using Editor.MapRepresentation;
using Environment = Editor.MapRepresentation.Environment;
using Mode = Editor.GUI.EnvironmentEditor.Mode;
using Tool = Editor.GUI.EnvironmentEditor.Tool;

namespace Editor.GUI
{
    public partial class EnvironmentDisplayControl : Control
    {
        public float CamOffsetX;
        public float CamOffsetY;
        public float CamScale = 20;

        public float CamOriginX;
        public float CamOriginY;
        public int MouseOriginX;
        public int MouseOriginY;

        private Bitmap _finalHeightBuffer;
        private Bitmap _finalWaterBuffer;
        private Bitmap _boundaryBuffer;
        private Bitmap _rampBuffer;
        private Bitmap _blightBuffer;
        private Bitmap _waterBuffer;
        private Bitmap _camBoundaryBuffer;
        private Bitmap _layerHeightBuffer;

        private Rectangle EnvZone => new Rectangle(Point.Empty,
            new Size((int) _environmentReference.Width, (int) _environmentReference.Height));
        private RectangleF EnvDisplayZone => new RectangleF(ViewportTransform(EnvZone.Location), 
            new SizeF(EnvZone.Width*CamScale, EnvZone.Height*CamScale));

        private Environment _environmentReference;
        public Environment EnvironmentReference
        {
            get { return _environmentReference; }
            set
            {
                _environmentReference = value;

                _finalHeightBuffer = new Bitmap((int)value.Width, (int)value.Height);
                _finalWaterBuffer = new Bitmap((int)value.Width, (int)value.Height);
                _boundaryBuffer = new Bitmap((int)value.Width, (int)value.Height);
                _rampBuffer = new Bitmap((int)value.Width, (int)value.Height);
                _blightBuffer = new Bitmap((int)value.Width, (int)value.Height);
                _waterBuffer = new Bitmap((int)value.Width, (int)value.Height);
                _camBoundaryBuffer = new Bitmap((int)value.Width, (int)value.Height);
                _layerHeightBuffer = new Bitmap((int)value.Width, (int)value.Height);

                RedrawRegion(Mode.FinalHeight, EnvZone);
                RedrawRegion(Mode.FinalWater, EnvZone);
                RedrawRegion(Mode.Boundary, EnvZone);
                RedrawRegion(Mode.Ramp, EnvZone);
                RedrawRegion(Mode.Blight, EnvZone);
                RedrawRegion(Mode.Water, EnvZone);
                RedrawRegion(Mode.CamBoundary, EnvZone);
                RedrawRegion(Mode.LayerHeight, EnvZone);
            }
        }
        
        public int ToolSize;
        public int ToolValue;
        public float ToolIntensity;

        private Mode _currentMode = Mode.FinalHeight;
        public Mode CurrentMode
        {
            get { return _currentMode; }

            set
            {
                _currentMode = value;
                Refresh();
            }
        }
        
        private Tool _currentTool;
        public Tool CurrentTool
        {
            get { return _currentTool; }
            set
            {
                _currentTool = value;
            }
        }
        
        private PropertyInfo _target => typeof (MapRepresentation.Tilepoint).GetProperty(CurrentMode.ToString());
        private MouseButtons _currentMouseButton;

        private long _lastRefresh;

        public EnvironmentDisplayControl()
        {
            InitializeComponent();
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            MouseDown += (sender, args) =>
            {
                Focus();
                if (args.Button == MouseButtons.Middle)
                {
                    CamOriginX = CamOffsetX;
                    CamOriginY = CamOffsetY;

                    MouseOriginX = args.X;
                    MouseOriginY = args.Y;

                    _currentMouseButton = MouseButtons.Middle;
                }

                if (args.Button == MouseButtons.Left)
                {
                    _currentMouseButton = MouseButtons.Left;
                    UseTool(InverseTransform(PointToClient(MousePosition)));
                }
                if (args.Button == MouseButtons.Right) _currentMouseButton = MouseButtons.Right;
            };
            MouseMove += (sender, args) =>
            {
                if (_currentMouseButton == MouseButtons.Middle)
                {
                    CamOffsetX = CamOriginX + (args.X - MouseOriginX) / CamScale;
                    CamOffsetY = CamOriginY + (args.Y - MouseOriginY) / CamScale;
                }
                else if (_currentMouseButton == MouseButtons.Left)
                {
                    UseTool(InverseTransform(PointToClient(MousePosition)));
                }
                Refresh();
            };
            MouseUp += (sender, args) =>
            {
                if (args.Button == _currentMouseButton) _currentMouseButton = MouseButtons.None;
            };
            MouseWheel += (sender, args) =>
            {
                if (!ClientRectangle.Contains(PointToClient(MousePosition))) return;

                CamScale *= (float) Math.Pow(1.1f, args.Delta/120f);
                Refresh();
            };
        }

        private void UseTool(PointF where)
        {
            RectangleF brush = new RectangleF(where.X - ToolSize / 2f, where.Y - ToolSize / 2f, ToolSize, ToolSize);

            Func<PointF, PointF, float> metric;

            switch (CurrentTool)
            {
                case Tool.Rectangle:
                    metric = (a, b) => Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
                    break;

                case Tool.Ellipse:
                    metric = (a, b) => (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
                    break;

                case Tool.Rhombus:
                    metric = (a, b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
                    break;

                default:
                    metric = (a, b) => 0;
                    break;
            }

            for (int y = 0; y < ToolSize; y++)
            {
                for (int x = 0; x < ToolSize; x++)
                {
                    Point p = Round(new PointF(where.X - ToolSize / 2f + x, where.Y - ToolSize / 2f + y));

                    if (!EnvZone.Contains(p)) continue;

                    int actualValue = (int)(ToolValue * Math.Pow(metric(p, Floor(where)) + 1, -ToolIntensity));

                    //what
                    object o = EnvironmentReference.Tiles[p.Y, p.X];
                    _target.SetValue(o, Convert.ChangeType(actualValue, _target.PropertyType));
                    EnvironmentReference.Tiles[p.Y, p.X] = (Tilepoint)o;
                }
            }

            RedrawRegion(CurrentMode, new Rectangle((int) (where.X - ToolSize / 2f), (int) (where.Y - ToolSize / 2f), ToolSize, ToolSize));
        }

        PointF ViewportTransform(PointF p)
        {
            return new PointF((p.X + CamOffsetX - EnvironmentReference.Width / 2f) * CamScale + Width / 2f,
                (p.Y + CamOffsetY - EnvironmentReference.Height / 2f) * CamScale + Height / 2f);
        }

        PointF InverseTransform(PointF p)
        {
            return new PointF((p.X - Width / 2f)/CamScale +EnvironmentReference.Width / 2f - CamOffsetX,
                (p.Y - Height / 2f) / CamScale + EnvironmentReference.Height / 2f - CamOffsetY);
        }

        static Point Floor(PointF p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        static Point Round(PointF p)
        {
            return new Point((int)Math.Round(p.X), (int)Math.Round(p.Y));
        }

        public override void Refresh()
        {
            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastRefresh < 10) return;

            base.Refresh();
            _lastRefresh = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Brush bg = new HatchBrush(HatchStyle.BackwardDiagonal, Color.DarkGray, Color.LightGray);
            pe.Graphics.FillRectangle(bg, new Rectangle(0, 0, Width, Height));
            bg.Dispose();

            if (EnvironmentReference != null)
            {
                Bitmap bmp = GetModeBuffer(_currentMode);

                pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half; //this or we lose half a pixel for no apparent reason
                
                if (bmp != null) pe.Graphics.DrawImage(bmp, EnvDisplayZone, EnvZone, GraphicsUnit.Pixel);

                pe.Graphics.DrawRectangle(new Pen(Color.Blue), EnvDisplayZone.X, EnvDisplayZone.Y, EnvDisplayZone.Width, EnvDisplayZone.Height);

                PointF underMouse = InverseTransform(PointToClient(MousePosition));
                Point underMouseFloor = Floor(underMouse);

                if (IsGradientMode(_currentMode)
                    && ((RectangleF) EnvZone).Contains(underMouse))
                {
                    object o = _target.GetValue(EnvironmentReference.Tiles[underMouseFloor.Y, underMouseFloor.X]);
                    string s = o.ToString();

                    if (CamScale/o.ToString().Length > 7)
                    {
                        Font font = new Font("Arial", CamScale/s.Length);
                        SizeF mes = TextRenderer.MeasureText(s, font);
                        Color c = bmp.GetPixel(underMouseFloor.X, underMouseFloor.Y);
                        PointF loc = ViewportTransform(underMouseFloor);
                        loc.X += CamScale / 2f - mes.Width / 2f;
                        loc.Y += CamScale / 2f - mes.Height / 2f;

                        Brush inv = new SolidBrush(Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                        pe.Graphics.DrawString(s, font, inv, loc);
                    }
                }

                /*
                for (int i = 0; i < EnvironmentReference.Height; i++)
                {
                    for (int j = 0; j < EnvironmentReference.Width; j++)
                    {
                        PointF fromF = ViewportTransform(new PointF(j, i));
                        PointF toF = ViewportTransform(new PointF(j + 1, i + 1));

                        if (fromF.X > Width || toF.X < 0 || fromF.Y > Height || toF.Y < 0) continue;

                        Point from = Round(fromF);

                        object o = _target.GetValue(EnvironmentReference.Tiles[i, j]);
                        SolidBrush b = new SolidBrush(Color.Black);
                        switch (_currentMode)
                        {
                            case EnvironmentEditor.Mode.TextureDetails:
                            case EnvironmentEditor.Mode.CliffTexture:
                            case EnvironmentEditor.Mode.GroundTexture:
                                pe.Graphics.FillRectangle(new SolidBrush(Color.Gray),
                                    new Rectangle(from.X + 1, from.Y + 1,
                                    (int)Math.Ceiling(CamScale) - 2, (int)Math.Ceiling(CamScale) - 2));

                                string s = o.ToString();
                                if (CamScale / s.Length > 7)
                                {
                                    Font font = new Font("Arial", CamScale / s.Length);
                                    SizeF mes = TextRenderer.MeasureText(s, font);

                                    pe.Graphics.DrawString(s, font, new SolidBrush(Color.Black),
                                        new PointF(fromF.X + CamScale / 2f - mes.Width / 2f, fromF.Y + CamScale / 2f - mes.Height / 2f));
                                }
                                break;

                            case EnvironmentEditor.Mode.LayerHeight:
                                byte g = (byte) o;
                                b = new SolidBrush(
                                    Color.FromArgb(0, g < 16 ? 16*g : 255, 16 <= g ? (g < 32 ? 16*(g - 16) : 255) : 0));

                                pe.Graphics.FillRectangle(b,
                                    new Rectangle(from.X, from.Y,
                                    (int)Math.Ceiling(CamScale), (int)Math.Ceiling(CamScale)));
                                break;

                            case EnvironmentEditor.Mode.FinalHeight:
                                int g2 = (int)Math.Floor(((float)o + 1024) * 255 / 2048);

                                b = new SolidBrush(
                                    Color.FromArgb(0, g2 <= 255 ? g2 : 255, 0));

                                pe.Graphics.FillRectangle(b,
                                    new Rectangle(from.X, from.Y,
                                    (int)Math.Ceiling(CamScale), (int)Math.Ceiling(CamScale)));
                                break;
                                
                            case EnvironmentEditor.Mode.FinalWater:
                                int g3 = (int)Math.Floor(((float)o + 256) * 255 / 256);

                                b = new SolidBrush(
                                    Color.FromArgb(0, 0, g3 <= 255 ? g3 : 255));

                                pe.Graphics.FillRectangle(b,
                                    new Rectangle(from.X, from.Y,
                                    (int)Math.Ceiling(CamScale), (int)Math.Ceiling(CamScale)));
                                break;

                            default:
                                if ((bool)_target.GetValue(EnvironmentReference.Tiles[i, j]))
                                    b = new SolidBrush(ModeColor(_currentMode));
                                else
                                    b = new SolidBrush(Color.Gray);

                                pe.Graphics.FillRectangle(b,
                                    new Rectangle(from.X, from.Y,
                                    (int)Math.Ceiling(CamScale), (int)Math.Ceiling(CamScale)));
                                break;
                        }
                        
                        if (IsGradientMode(_currentMode)
                            && j <= underMouse.X && underMouse.X < j + 1
                            && i <= underMouse.Y && underMouse.Y < i + 1
                            && CamScale / o.ToString().Length > 7)
                        {
                            string s = o.ToString();
                            Font font = new Font("Arial", CamScale / s.Length);
                            SizeF mes = TextRenderer.MeasureText(s, font);

                            Brush inv = new SolidBrush(Color.FromArgb(255 - b.Color.R, 255 - b.Color.G, 255 - b.Color.B));
                            pe.Graphics.DrawString(s, font, inv,
                                new PointF(fromF.X + CamScale / 2f - mes.Width / 2f, fromF.Y + CamScale / 2f - mes.Height / 2f));
                        }
                    }
                }*/
            }

            Pen cross = new Pen(Color.LawnGreen);
            pe.Graphics.DrawLine(cross, Width / 2f - 20, Height / 2f, Width / 2f + 20, Height / 2f);
            pe.Graphics.DrawLine(cross, Width / 2f, Height / 2f - 20, Width / 2f, Height / 2f + 20);
            cross.Dispose();
        }

        public void RedrawRegion(Mode m, Rectangle r)
        {
            Bitmap bmp = GetModeBuffer(m);

            for (int i = 0; i < EnvironmentReference.Height; i++)
            {
                for (int j = 0; j < EnvironmentReference.Width; j++)
                {
                    object o = typeof(MapRepresentation.Tilepoint).GetProperty(m.ToString()).GetValue(EnvironmentReference.Tiles[i, j]);

                    switch (m)
                    {
                        case Mode.LayerHeight:
                            byte g = (byte)o;

                            bmp.SetPixel(j, i, Color.FromArgb(0, g < 16 ? 16 * g : 255, 16 <= g ? (g < 32 ? 16 * (g - 16) : 255) : 0));
                            break;

                        case Mode.FinalHeight:
                            int g2 = (int)Math.Floor(((float)o + 1024) * 255 / 2048);
                            if (g2 < 0) g2 = 0;

                            bmp.SetPixel(j, i, Color.FromArgb(0, g2 <= 255 ? g2 : 255, 0));
                            break;

                        case Mode.FinalWater:
                            int g3 = (int)Math.Floor(((float)o + 256) * 255 / 256);
                            if (g3 < 0) g3 = 0;

                            bmp.SetPixel(j, i, Color.FromArgb(0, 0, g3 <= 255 ? g3 : 255));
                            break;

                        default:
                            Color c;
                            if ((bool) o)
                                c = ModeColor(m);
                            else
                                c = Color.Gray;

                            bmp.SetPixel(j, i, c);
                            break;
                    }
                }
            }
        }
        
        private Bitmap GetModeBuffer(Mode m)
        {
            switch (m)
            {
                case Mode.FinalHeight: return _finalHeightBuffer;
                case Mode.FinalWater: return _finalWaterBuffer;
                case Mode.Boundary: return _boundaryBuffer;
                case Mode.Ramp: return _rampBuffer;
                case Mode.Blight: return _blightBuffer;
                case Mode.Water: return _waterBuffer;
                case Mode.CamBoundary: return _camBoundaryBuffer;
                case Mode.LayerHeight: return _layerHeightBuffer;
            }
            return null;
        }

        private void PaintEnvironment(Mode m, Tool t, uint size, int value, float intensity)
        {
            //TODO: do
        }

        public bool IsNumericMode(Mode m)
        {
            return m == Mode.GroundTexture || m == Mode.CliffTexture || m == Mode.TextureDetails;
        }

        public bool IsGradientMode(Mode m)
        {
            return m == Mode.FinalHeight || m == Mode.FinalWater || m == Mode.LayerHeight;
        }

        public Color ModeColor(Mode m)
        {
            switch (m)
            {
                case Mode.Boundary:
                    return Color.Black;
                case Mode.CamBoundary:
                    return Color.Black;
                case Mode.Ramp:
                    return Color.Purple;
                case Mode.Blight:
                    return Color.GreenYellow;
                case Mode.Water:
                    return Color.DeepSkyBlue;
            }

            return Color.White;
        }
    }
}
