using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Editor.GUI.Controls;
using Editor.MapRepresentation;
using Environment = Editor.MapRepresentation.Environment;

namespace Editor.GUI
{
    public partial class EnvironmentEditor : Form
    {
        public string CurrentFilepath { get; private set; } = "lel";

        private RadialFlower _modeRadialFlower;
        private RadialFlower _toolRadialFlower;
        private RadialShell _sizeRadialShell;
        private RadialShell _valueRadialShell;
        private RadialShell _intensityRadialShell;

        public int ToolSize => _sizeRadialShell.WiggleAngle + _sizeRadialShell.MinWiggleAngle;
        public int ToolValue => _valueRadialShell.WiggleAngle + _valueRadialShell.MinWiggleAngle;
        public float ToolIntensity => (_intensityRadialShell.WiggleAngle + _intensityRadialShell.MinWiggleAngle) / 100f;

        private Mode _currentMode;
        public Mode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                _currentMode = value;
                envDisplay.CurrentMode = value;

                switch (value)
                {
                    case Mode.Blight:
                    case Mode.Ramp:
                    case Mode.Water:
                    case Mode.CamBoundary:
                    case Mode.Boundary:
                        _valueRadialShell.MinWiggleAngle = 0;
                        _valueRadialShell.WiggleAngle = 0;
                        _valueRadialShell.MaxWiggleAngle = 1;
                        _valueRadialShell.DisplayFunc = RadialShell.DefaultDisplayFunc;
                        break;

                    case Mode.LayerHeight:
                        _valueRadialShell.MinWiggleAngle = Tilepoint.LayerHeightMin;
                        _valueRadialShell.WiggleAngle = 2;
                        _valueRadialShell.MaxWiggleAngle = Tilepoint.LayerHeightMax;
                        _valueRadialShell.DisplayFunc = RadialShell.DefaultDisplayFunc;
                        break;

                    case Mode.FinalHeight:
                        //special case, changes are relative
                        _valueRadialShell.MinWiggleAngle = -128;
                        _valueRadialShell.WiggleAngle = 0;
                        _valueRadialShell.MaxWiggleAngle = 128;

                        _valueRadialShell.DisplayFunc = (x) => $"{Tilepoint.ToFinalHeight(2, (short) x):0000.00}";
                        break;

                    case Mode.FinalWater:
                        _valueRadialShell.MinWiggleAngle = Tilepoint.WaterLevelMin;
                        _valueRadialShell.WiggleAngle = 0;
                        _valueRadialShell.MaxWiggleAngle = Tilepoint.WaterLevelMax;
                        _valueRadialShell.DisplayFunc = RadialShell.DefaultDisplayFunc;
                        break;

                    case Mode.TextureDetails:
                        _valueRadialShell.MinWiggleAngle = Tilepoint.TextureDetailsMin;
                        _valueRadialShell.WiggleAngle = 0;
                        _valueRadialShell.MaxWiggleAngle = Tilepoint.TextureDetailsMax;
                        _valueRadialShell.DisplayFunc = RadialShell.DefaultDisplayFunc;
                        break;

                    case Mode.CliffTexture:
                        _valueRadialShell.MinWiggleAngle = Tilepoint.CliffTextureMin;
                        _valueRadialShell.WiggleAngle = 0;
                        _valueRadialShell.MaxWiggleAngle = Tilepoint.CliffTextureMax;
                        _valueRadialShell.DisplayFunc = RadialShell.DefaultDisplayFunc;
                        break;

                    case Mode.GroundTexture:
                        _valueRadialShell.MinWiggleAngle = Tilepoint.GroundHeightMin;
                        _valueRadialShell.WiggleAngle = 0;
                        _valueRadialShell.MaxWiggleAngle = Tilepoint.GroundHeightMax;
                        _valueRadialShell.DisplayFunc = RadialShell.DefaultDisplayFunc;
                        break;

                }
            }
        }

        private Environment _environmentReference;
        private Environment EnvironmentReference
        {
            get { return _environmentReference; }
            set
            {
                _environmentReference = value;
            }
        }

        private Tool _currentTool;
        public Tool CurrentTool
        {
            get { return _currentTool; }
            set
            {
                _currentTool = value;
                envDisplay.CurrentTool = value;
            }
        }


        public EnvironmentEditor(Environment env)
        {
            InitializeComponent();
            _environmentReference = env;

            Text = "Environment Editor";

            _modeRadialFlower = new RadialFlower();
            _modeRadialFlower.Elements = new Dictionary<string, Image>();
            foreach (var m in Enum.GetValues(typeof (Mode)))
            {
                _modeRadialFlower.Elements.Add(m.ToString(), null);
            }

            _toolRadialFlower = new RadialFlower();
            _toolRadialFlower.Elements = new Dictionary<string, Image>();
            foreach (var m in Enum.GetValues(typeof(Tool)))
            {
                _toolRadialFlower.Elements.Add(m.ToString(), null);
            }

            _sizeRadialShell = new RadialShell();
            _sizeRadialShell.MinWiggleAngle = 1;
            _sizeRadialShell.WiggleAngle = 7;
            _sizeRadialShell.MaxWiggleAngle = 128;

            _valueRadialShell = new RadialShell();

            _intensityRadialShell = new RadialShell();
            _intensityRadialShell.MinWiggleAngle = 0;
            _intensityRadialShell.WiggleAngle = 100;
            _intensityRadialShell.MaxWiggleAngle = 400;
            _intensityRadialShell.DisplayFunc = (x) => (x/100f).ToString();

            envDisplay.EnvironmentReference = env;
            KeyPreview = true;

            WinapiAccess.GlobalKeyDown += (s, e) =>
            {
                if (ActiveForm != this) return;
                switch (e.KeyCode)
                {
                    case Keys.E:
                        if (e.Shift) RadialMenu.ShowNew(_modeRadialFlower, MousePosition);
                        break;

                    case Keys.T:
                        if (e.Shift) RadialMenu.ShowNew(_toolRadialFlower, MousePosition);
                        break;

                    case Keys.S:
                        if (e.Shift) RadialMenu.ShowNew(_sizeRadialShell, MousePosition);
                        else if (e.Control) saveToolStripMenuItem_Click(null, null);
                        break;

                    case Keys.D:
                        if (e.Shift) RadialMenu.ShowNew(_valueRadialShell, MousePosition);
                        break;

                    case Keys.Q:
                        if (e.Shift) RadialMenu.ShowNew(_intensityRadialShell, MousePosition);
                        break;
                }
            };

            WinapiAccess.GlobalKeyUp += (s, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.LShiftKey:
                        RadialMenu.CloseGracefullyCurrent();
                        break;
                }
            };

            _modeRadialFlower.ClosedGracefully += (s, e) =>
            {
                int ind = _modeRadialFlower.SelectionIndex;
                if (ind != -1) CurrentMode = (Mode)ind;
            };

            _toolRadialFlower.ClosedGracefully += (s, e) =>
            {
                int ind = _toolRadialFlower.SelectionIndex;
                if (ind != -1) CurrentTool = (Tool)ind;
            };

            _sizeRadialShell.ClosedGracefully += (s, e) =>
            {
                envDisplay.ToolSize = ToolSize;
            };

            _intensityRadialShell.ClosedGracefully += (s, e) =>
            {
                envDisplay.ToolIntensity = ToolIntensity;
            };

            _valueRadialShell.ClosedGracefully += (s, e) =>
            {
                envDisplay.ToolValue = ToolValue;
            };

            CurrentMode = Mode.FinalHeight;
            CurrentTool = Tool.Rectangle;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //env creation dialogue
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    CurrentFilepath = openFileDialog1.FileName;

                    FileStream envfile = new FileStream(CurrentFilepath, FileMode.Open);
                    byte[] envraw = new byte[envfile.Length];
                    envfile.Read(envraw, 0, (int) envfile.Length);
                    _environmentReference = Environment.Read(envraw);
                    envDisplay.EnvironmentReference = _environmentReference;
                    Text = "Environment Editor — " + CurrentFilepath;
                    envfile.Close();
                }
                catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException)
                {
                    MessageBox.Show("The selected file has invalid format.", "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException)
                {
                    MessageBox.Show("Unable to get access to the selected file.", "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream envfile = new FileStream(CurrentFilepath, FileMode.Create);
            _environmentReference.Write(envfile);
            envfile.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CurrentFilepath = saveFileDialog1.FileName;
                
                FileStream envfile = new FileStream(CurrentFilepath, FileMode.Create);
                _environmentReference.Write(envfile);
                envfile.Close();
            }
        }

        public enum Mode
        {
            FinalHeight,
            FinalWater,
            Boundary,
            Ramp,
            Blight,
            Water,
            CamBoundary,
            GroundTexture,
            TextureDetails,
            CliffTexture,
            LayerHeight
        }

        public enum Tool
        {
            Rectangle,
            Ellipse,
            Rhombus,
            Spray,
            Line
        }
    }
}
