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
using RageLib.Common.Resources;
using RageLib.Common.ResourceTypes;

namespace RageLib.Models.Resource
{
    // grmModel
    internal class GeometryInfo : IFileAccess
    {
        private uint VTable { get; set; }
        public PtrCollection<GeometryDataInfo> GeometryDataInfos { get; private set; }
        private ushort Unknown1 { get; set; }
        private ushort Unknown2 { get; set; }
        private ushort Unknown3 { get; set; }
        private ushort Unknown4 { get; set; }
        public SimpleArray<Vector4> UnknownVectors { get; private set; }
        public SimpleArray<ushort> MaterialMappings { get; private set; }

        #region Implementation of IFileAccess

        public void Read(BinaryReader br)
        {
            VTable = br.ReadUInt32();

            GeometryDataInfos = new PtrCollection<GeometryDataInfo>(br);

            var unknownVectorOffsets = ResourceUtil.ReadOffset(br);
            var p2Offset = ResourceUtil.ReadOffset(br);

            Unknown1 = br.ReadUInt16();
            Unknown2 = br.ReadUInt16();

            Unknown3 = br.ReadUInt16();
            Unknown4 = br.ReadUInt16();

            //

            br.BaseStream.Seek(unknownVectorOffsets, SeekOrigin.Begin);
            UnknownVectors = new SimpleArray<Vector4>(br, 4, reader => new Vector4(reader));

            br.BaseStream.Seek(p2Offset, SeekOrigin.Begin);
            MaterialMappings = new SimpleArray<ushort>(br, GeometryDataInfos.Count, reader => reader.ReadUInt16());
        }

        public void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}