using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace Editor.Rendering
{
	public class GlobalRenderer
	{
		private Dictionary<string, IRenderer> registry;
		public Vector3d CameraPosition;
		public Quaternion CameraRotation;

		public void Render()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

		}

		public void RegisterRenderer(string id, IRenderer renderer)
		{

		}
	}
}
