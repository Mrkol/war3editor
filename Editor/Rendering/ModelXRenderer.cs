using System;
using OpenTK.Graphics.OpenGL4;
using Editor.ModelRepresentation;
using Editor.ModelRepresentation.Objects;

namespace Editor.Rendering
{
    public class ModelXRenderer : Renderer
    {
        private int[] geosetVBOs;
        private int[][] geosetVertexGroupEBOs;
        private int[][] geosetVertexGroupSizes;
        private uint[][] geosetVertexGroupTypes;

        private ModelXRenderer()
        {

        }

        public override void Render(RenderingArgs args)
        {
            //for teh bones
            ModelXRenderingArgs mdxArgs = args as ModelXRenderingArgs;
            
            ApplyUniformUVP(args);

            for (int i = 0; i < geosetVBOs.Length; ++i)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, geosetVBOs[i]);
                for (int j = 0; j < geosetVertexGroupEBOs[i].Length; ++j)
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 
                        geosetVertexGroupEBOs[i][j]);

                    GL.DrawElements((PrimitiveType) geosetVertexGroupTypes[i][j], 
                        geosetVertexGroupSizes[i][j], DrawElementsType.UnsignedShort, 
                        (IntPtr) 0);
                }
            }
        }

        ~ModelXRenderer()
        {
            //Those always have to be initialized
            GL.DeleteBuffers(geosetVBOs.Length, geosetVBOs);
            foreach (int[] groupEBOs in geosetVertexGroupEBOs)
            {
                GL.DeleteBuffers(groupEBOs.Length, groupEBOs);
            }
        }

        static ModelXRenderer BuildRenderer(ModelX model)
        {
            ModelXRenderer result = new ModelXRenderer();

            if (model.CGeosets.HasValue)
            {
                result.geosetVBOs = new int[model.CGeosets.Value.Geosets.Length];
                result.geosetVertexGroupEBOs = new int[geosetVBOs.Length][];
                result.geosetVertexGroupTypes = new uint[geosetVBOs.Length][];
                result.geosetVertexGroupSizes = new int[geosetVBOs.Length][];

                GL.GenBuffers(result.geosetVBOs.Length, result.geosetVBOs);

                Geoset[] geosets = model.CGeosets.Value.Geosets; 

                for (int i = 0; i < geosets.Length; ++i)
                {
                    float[] buffer = new float[geosets[i].Vrtx.VerticesCount * 8];
                    for (int j = 0; j < geosets[i].Vrtx.VerticesCount; ++j)
                    {
                        buffer[8*j + 0] = geosets[i].Vrtx.Vertices[j].X;
                        buffer[8*j + 1] = geosets[i].Vrtx.Vertices[j].Y;
                        buffer[8*j + 2] = geosets[i].Vrtx.Vertices[j].Z;

                        buffer[8*j + 3] = geosets[i].Nrms.Normals[j].X;
                        buffer[8*j + 4] = geosets[i].Nrms.Normals[j].Y;
                        buffer[8*j + 5] = geosets[i].Nrms.Normals[j].Z;
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, result.geosetVBOs[i]);
                    
                    GL.BufferData<float>(BufferTarget.ArrayBuffer, 
                        (IntPtr) buffer.Length, buffer, BufferUsageHint.StaticDraw);

                    result.geosetVertexGroupEBOs[i] = 
                        new int[geosets[i].Pcnt.FaceGroupsCount];
                    GL.GenBuffers(result.geosetVertexGroupEBOs[i].Length,
                        result.geosetVertexGroupEBOs[i]);

                    int groupNo = 0;
                    for (int j = 0; j < geosets[i].Pvtx.FaceGroupsCount;)
                    {
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 
                            result.geosetVertexGroupEBOs[i][j]);
                        ushort[] groupIndices = 
                            new ushort[
                                geosets[i].Pcnt.FaceGroupPrimitiveCounts[groupNo]];

                        Array.ConstrainedCopy(geosets[i].Pvtx.FaceGroups, j, 
                            groupIndices, 0, groupIndices.Length);
                        

                        GL.BufferData(BufferTarget.ElementArrayBuffer, 
                            (IntPtr) groupIndices.Length, groupIndices, 
                            BufferUsageHint.StaticDraw);

                        result.geosetVertexGroupSizes[i][groupNo] = 
                            groupIndices.Length;

                        result.geosetVertexGroupTypes[i][groupNo] = 
                            geosets[i].Ptyp.FaceGroupPrimitiveTypes[groupNo];

                        j += groupIndices.Length;
                        ++groupNo;
                    }
                }
            }

            return result;
        }

        public class ModelXRenderingArgs : RenderingArgs
        {
            public Skeleton Skeleton;
        }
    }
}
