using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using System.Reflection;
using System.Collections.Generic;

namespace Editor.Rendering
{
	public class ShaderManager
	{
		private readonly ShaderType[] shaderTypes =
			{ShaderType.VertexShader, ShaderType.FragmentShader};
		private readonly string[] shaderFileNames = 
			{"Editor.Rendering.main.vsh", "Editor.Rendering.main.fsh"};
		private readonly string[] shaderSources;
		private readonly int[] shaderIds;

		private readonly Tuple<string, int, int>[] shaderPrograms =
			{Tuple.Create("main", 0, 1)};
		public readonly Dictionary<string, int> ShaderPrograms;

		private ShaderManager()
		{
			shaderSources = new string[shaderFileNames.Length];
			shaderIds = new int[shaderFileNames.Length];
			ShaderPrograms = new Dictionary<string, int>();

			for (int i = 0; i < shaderFileNames.Length; ++i)
			{
				using (Stream stream = 
					Assembly.GetExecutingAssembly()
						.GetManifestResourceStream(shaderFileNames[i]))
				using (StreamReader reader = 
					new StreamReader(stream))
				{
					shaderSources[i] = reader.ReadToEnd();
				}

				shaderIds[i] = GL.CreateShader(shaderTypes[i]);
				GL.ShaderSource(shaderIds[i], shaderSources[i]);
				GL.CompileShader(shaderIds[i]);

				int status;
				GL.GetShader(shaderIds[i], ShaderParameter.CompileStatus, out status);
				if (status == 0)
				{
					string log = GL.GetShaderInfoLog(shaderIds[i]);
					FormatException e = new FormatException(
						"Shader compilation failed: \""
						 + shaderFileNames[i] + "\".\n\r"
						 + "Log:\n\r" + log);
					GL.DeleteShader(shaderIds[i]);
					throw e;
				}
					
				Console.WriteLine(shaderFileNames[i] + " compiled successfully.");
			}

			foreach (Tuple<string, int, int> prog in shaderPrograms)
			{
				ShaderPrograms.Add(prog.Item1, GL.CreateProgram());
				GL.AttachShader(ShaderPrograms[prog.Item1], prog.Item2);
				GL.AttachShader(ShaderPrograms[prog.Item1], prog.Item3);
			}
		}

		public static ShaderManager Instance;

		public static void Init()
		{
			Instance = new ShaderManager();
		}
	}
}
