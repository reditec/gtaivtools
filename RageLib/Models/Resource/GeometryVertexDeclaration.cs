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
    internal class GeometryVertexDeclaration : IFileAccess
    {
        public uint Flags { get; set; }
        public ushort Stride { get; set; }
        public byte Unknown1 { get; set; }
        public byte Type { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }

        public GeometryVertexDeclaration(BinaryReader br)
        {
            Read(br);
        }

        #region Implementation of IFileAccess

        public void Read(BinaryReader br)
        {
            Flags = br.ReadUInt32();
            Stride = br.ReadUInt16();
            Unknown1 = br.ReadByte();       // Stream index probably?
            Type = br.ReadByte();

            Unknown2 = br.ReadUInt32();
            Unknown3 = br.ReadUInt32();
        }

        public void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
