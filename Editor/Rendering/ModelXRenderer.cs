using Editor.ModelRepresentation;
using OpenTK.Graphics.OpenGL4;

namespace Editor.Rendering
{
    public class ModelXRendererBuilder : RendererBuilder<ModelX>
    {
    	public override Renderer Build(ModelX param)
        {
        	return new ModelXRenderer(param);
        }

        public class ModelXRenderer : Renderer
        {
        	readonly ModelX Target;
        	private int BufferObjectHandle;

        	public ModelXRenderer(ModelX targ)
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