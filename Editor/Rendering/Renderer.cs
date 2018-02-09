using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Editor.Rendering
{
    public abstract class Renderer
    {
		public virtual void Render(RenderingArgs args);

        public static void ApplyUniformUVP(RenderingArgs args)
        {
            GL.UniformMatrix4(args.ViewMatrixLocation, false, 
                ref args.ViewMatrix);

            GL.UniformMatrix4(args.ProjectionMatrixLocation, false, 
                ref args.ProjectionMatrix);

            GL.UniformMatrix4(args.ModelMatrixLocation, false, 
                ref args.ModelMatrix);
        }
    }

    public class RenderingArgs
    {
        public ulong Time;
        public Matrix4 ViewMatrix = Matrix4.Identity;
        public Matrix4 ProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelMatrix = Matrix4.Identity;
        public int ViewMatrixLocation;
        public int ProjectionMatrixLocation;
        public int ModelMatrixLocation;
    }
}
