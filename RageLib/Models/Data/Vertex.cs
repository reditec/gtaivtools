using System.IO;
using RageLib.Common.ResourceTypes;

namespace RageLib.Models.Data
{
    public struct Vertex
    {
        public Vector3 Position { get; private set; }
        public Vector3 Normal { get; private set; }
        public uint DiffuseColor { get; private set; }
        public uint SpecularColor { get; private set; }
        public Vector2[] TextureCoordinates { get; private set; }

        public const int MaxTextureCoordinates = 8;

        // To add: BlendWeight, BlendIndices, Tangent, Binormal (sizes/etc??)

        internal Vertex(BinaryReader br, Mesh mesh) : this()
        {
            TextureCoordinates = new Vector2[MaxTextureCoordinates];

            VertexElement[] elements = mesh.VertexDeclaration.Elements;
            foreach (var element in elements)
            {
                if (element.Stream == -1)
                {
                    break;
                }

                switch(element.Usage)
                {
                    case VertexElementUsage.Position:
                        Position = new Vector3(br);
                        break;
                    case VertexElementUsage.Normal:
                        Normal = new Vector3(br);
                        break;
                    case VertexElementUsage.TextureCoordinate:
                        TextureCoordinates[element.UsageIndex] = new Vector2(br);
                        break;
                    case VertexElementUsage.Color:
                        if (element.UsageIndex == 0)
                        {
                            DiffuseColor = br.ReadUInt32();
                        }
                        else if (element.UsageIndex == 1)
                        {
                            SpecularColor = br.ReadUInt32();
                        }
                        else
                        {
                            br.ReadUInt32();
                        }
                        break;
                    default:
                        br.BaseStream.Seek(element.Size, SeekOrigin.Current);
                        break;
                }
            }
        }
    }
}