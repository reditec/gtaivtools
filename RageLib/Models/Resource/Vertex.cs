/**********************************************************************\

 RageLib
 Copyright (C) 2008  Arushan/Aru <oneforaru at gmail.com>

 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.

\**********************************************************************/

using System.Diagnostics;
using System.IO;
using RageLib.Common;

namespace RageLib.Models.Resource
{
    internal class Vertex : IFileAccess
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float NormalX { get; set; }
        public float NormalY { get; set; }
        public float NormalZ { get; set; }
        public uint Diffuse { get; set; }

        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }

        private float[] TextureUCoordinates;
        private float[] TextureVCoordinates;
        private const int DefaultCoordinateIndex = 0;
        
        public float TextureU { get { return TextureUCoordinates[DefaultCoordinateIndex]; } set { TextureUCoordinates[DefaultCoordinateIndex] = value; } }
        public float TextureV { get { return TextureVCoordinates[DefaultCoordinateIndex]; } set { TextureVCoordinates[DefaultCoordinateIndex] = value; } }

        private GeometryVertexDeclaration Declaration { get; set; }

        public Vertex(GeometryVertexDeclaration declaration)
        {
            Declaration = declaration;
        }

        public Vertex(BinaryReader br, GeometryVertexDeclaration declaration) : this(declaration)
        {
            Read(br);
        }

        private void ReadUV(BinaryReader br, int max)
        {
            TextureUCoordinates = new float[max];
            TextureVCoordinates = new float[max];
            for (var i = 0; i < max; i++)
            {
                TextureUCoordinates[i] = br.ReadSingle();
                TextureVCoordinates[i] = br.ReadSingle();
            }
        }

        #region Implementation of IFileAccess

        public void Read(BinaryReader br)
        {
            switch(Declaration.Type)
            {
                case 4:
                    Debug.Assert(Declaration.Stride == 0x24);

                    X = br.ReadSingle();
                    Y = br.ReadSingle();
                    Z = br.ReadSingle();
                    
                    NormalX = br.ReadSingle();
                    NormalY = br.ReadSingle();
                    NormalZ = br.ReadSingle();

                    Diffuse = br.ReadUInt32();

                    ReadUV(br, 1);

                    break;
                case 5:
                    Debug.Assert(Declaration.Stride == 0x34);

                    X = br.ReadSingle();
                    Y = br.ReadSingle();
                    Z = br.ReadSingle();

                    NormalX = br.ReadSingle();
                    NormalY = br.ReadSingle();
                    NormalZ = br.ReadSingle();

                    Diffuse = br.ReadUInt32();

                    ReadUV(br, 3);

                    break;
                case 6:
                    Debug.Assert(Declaration.Stride == 0x2c);

                    X = br.ReadSingle();
                    Y = br.ReadSingle();
                    Z = br.ReadSingle();

                    br.ReadUInt32();
                    br.ReadUInt32();

                    NormalX = br.ReadSingle();
                    NormalY = br.ReadSingle();
                    NormalZ = br.ReadSingle();

                    Diffuse = br.ReadUInt32();

                    ReadUV(br, 1);

                    break;
                case 7:
                    Debug.Assert(Declaration.Stride == 0x3c);

                    X = br.ReadSingle();
                    Y = br.ReadSingle();
                    Z = br.ReadSingle();

                    Unknown1 = br.ReadUInt32();
                    Unknown2 = br.ReadUInt32();

                    NormalX = br.ReadSingle();
                    NormalY = br.ReadSingle();
                    NormalZ = br.ReadSingle();

                    Diffuse = br.ReadUInt32();

                    ReadUV(br, 3);

                    break;
                default:
                    Debug.Assert(false);

                    break;
            }
        }

        public void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}