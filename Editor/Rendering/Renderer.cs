using System;

namespace Editor.Rendering
{
    abstract class Renderer
    {
    	public bool Initialized { get; private set; }

    	public virtual void Initialize()
    	{
    		Initialized = true;
    	}

		public virtual void Render()
		{
			if (!Initialized) throw new UninitializedRendererException(this);
		}

    	public virtual void Finalize()
    	{
    		Initialized = false;
    	}
    }

    abstract class RendererBuilder<T>
    {
		public abstract virtual Renderer Build(T param);
    }

    public class UninitializedRendererException : Exception
    {
    	private UninitializedRendererException()
    	{

    	}

    	public UninitializedRendererException(Renderer renderer)
    	{
    		Message = 
    			"A "
    			+ renderer
    			+ " renderer was attempted to be invoked"
    			+ " although it was not initialized.";
    	}

	    public UninitializedRendererException(string message) : base(message)
	    {
	    }

	    public UninitializedRendererException(string message, Exception inner) : base(message, inner)
	    {
	    }
    }
}