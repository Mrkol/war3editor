using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL4;


namespace Editor.GUI
{
	class TestOpenGLForm : Form
	{
		readonly GLControl testcontrol;

		public TestOpenGLForm()
		{
			testcontrol = new GLControl();

			
			SuspendLayout();

			testcontrol.SuspendLayout();
            testcontrol.Dock = DockStyle.Fill;
            testcontrol.Location = new Point(0, 0);
			testcontrol.Paint += OnGLControlPaint;
			testcontrol.Resize += OnGLControlResize;
			testcontrol.ResumeLayout();

			Controls.Add(testcontrol);
			ResumeLayout(false);
			
			Text = "Environment Editor";
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new Size(1264, 681);
			
			PerformLayout();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			GL.ClearColor(Color.Crimson);
		}

		protected void OnGLControlResize(object s, EventArgs e)
		{
			if (testcontrol.ClientSize.Height == 0)
				testcontrol.ClientSize = new Size(testcontrol.ClientSize.Width, 1);

			GL.Viewport(0, 0, testcontrol.ClientSize.Width, testcontrol.ClientSize.Height);
		}

		private void OnGLControlPaint(object s, PaintEventArgs e)
		{
			if (DesignMode) return;

			testcontrol.MakeCurrent();

			

			testcontrol.SwapBuffers();
		}
	}
}
