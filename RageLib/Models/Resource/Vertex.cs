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
        public float TextureU { get; set; }
        public float TextureV { get; set; }

        private uint StrideSize { get; set; }

        public Vertex(uint stride)
        {
            StrideSize = stride;
        }

        public Vertex(BinaryReader br, uint stride)
        {
            StrideSize = stride;
            Read(br);
        }

        #region Implementation of IFileAccess

        public void Read(BinaryReader br)
        {
            if (StrideSize == 0x3c)
            {
                // For peds?
                X = br.ReadSingle();
                Y = br.ReadSingle();
                Z = br.ReadSingle();

                br.ReadUInt32(); // Maybe some bone info
                br.ReadUInt32();

                NormalX = br.ReadSingle();
                NormalY = br.ReadSingle();
                NormalZ = br.ReadSingle();

                Diffuse = br.ReadUInt32();

                TextureU = br.ReadSingle();
                TextureV = br.ReadSingle();

                br.ReadSingle(); // Additional UVs?
                br.ReadSingle();

                br.ReadSingle();
                br.ReadSingle();
            }
            else if (StrideSize == 0x34)
            {
                // For some weapons?
                X = br.ReadSingle();
                Y = br.ReadSingle();
                Z = br.ReadSingle();

                NormalX = br.ReadSingle();
                NormalY = br.ReadSingle();
                NormalZ = br.ReadSingle();

                Diffuse = br.ReadUInt32();

                TextureU = br.ReadSingle();
                TextureV = br.ReadSingle(); 
                
                br.ReadSingle(); // Additional UVs?
                br.ReadSingle();

                br.ReadSingle();
                br.ReadSingle();
            }
            else
            {

                const int CurrentStrideSize = 4*9;

                X = br.ReadSingle();
                Y = br.ReadSingle();
                Z = br.ReadSingle();
                NormalX = br.ReadSingle();
                NormalY = br.ReadSingle();
                NormalZ = br.ReadSingle();
                Diffuse = br.ReadUInt32();
                TextureU = br.ReadSingle();
                TextureV = br.ReadSingle();

                if ((StrideSize - CurrentStrideSize) > 0)
                {
                    br.ReadBytes((int) StrideSize - CurrentStrideSize);
                }

            }
        }

        public void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}