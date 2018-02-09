using Editor.ModelRepresentation;
using System.Collections.Generic;
using MDXBone = Editor.ModelRepresentation.Objects.Bone;
using OpenTK;

namespace Editor.Rendering
{
    public class Skeleton
    {
		private List<Bone> roots;
		private List<Bone> allBones;

		public Skeleton()
		{
			roots = new List<Bone>();
			allBones = new List<Bone>();
		}

		//maybe consider memoization? probably
		public static Skeleton BuildSkeleton(ModelX model)
		{
			Skeleton result = new Skeleton();

			if (!model.CBones.HasValue) return result;

			MDXBone[] mdxBones = model.CBones.Value.Bones;

			Bone[] tmpBones = new Bone[mdxBones.Length];
			Dictionary<uint, Bone> mdxIdMap = new Dictionary<uint, Bone>();

			for (int i = 0; i < mdxBones.Length; ++i)
			{
				mdxIdMap.Add(mdxBones[i].Node.ObjectId, tmpBones[i]);
			}

			for (int i = 0; i < mdxBones.Length; ++i)
			{
				if (mdxBones[i].Node.ParentId == uint.MaxValue)
				{
					result.roots.Add(tmpBones[i]);
					break;
				}

				mdxIdMap[mdxBones[i].Node.ParentId].Children.Add(
					mdxIdMap[mdxBones[i].Node.ObjectId]);
			}

			return result;
		}

	    public class Bone
	    {
			public List<Bone> Children;
			public Matrix4d Transformation { get; set; }

			public Bone()
			{
				Children = new List<Bone>();
				Transformation = Matrix4d.Identity;
			}
	    }
    }
}
