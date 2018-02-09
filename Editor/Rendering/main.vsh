in vec3 position;
in vec3 normal;
in vec2 texcoords;

in vec3i attachedBones;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

buffer boneBlock
{
  mat4 bones[];
};

out vec2 frag_texcoords;

int main()
{
	gl_Position = projection * view * model * position;
	frag_texcoords = texcoords;
}
