using Editor.ModelRepresentation;
using OpenTK.Graphics.OpenGL4;

namespace Editor.Rendering
{
    class ModelXRendererBuilder : RendererBuilder<ModelX>
    {
    	public override Renderer Build(ModelX param)
        {

        }

        public class ModelXRenderer : Renderer
        {
        	readonly ModelX Target;
        	private int BufferObjectHandle;

        	ModelXRenderer(ModelX targ)
        	{
        		Target = targ;
        	}

            public override void Initialize()
            {
            	BufferObjectHandle = GL.GenBuffer();
            }

        	public override void Render()
            {

            }

            public override void Finalize()
            {
            	GL.DeleteBuffer(BufferObjectHandle);
            }
        }
    }
}