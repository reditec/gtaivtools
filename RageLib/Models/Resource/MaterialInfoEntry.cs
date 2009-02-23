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

using System;
using System.Collections.Generic;
using System.IO;
using RageLib.Common;
using RageLib.Common.Resources;
using RageLib.Common.ResourceTypes;

namespace RageLib.Models.Resource
{
    internal class MaterialInfoEntry : IFileAccess
    {
        private uint VTable { get; set; }
        private uint BlockMapOffset { get; set; }
        private ushort Unknown1 { get; set; }
        private byte Unknown2 { get; set; }
        private byte Unknown3 { get; set; }
        private ushort Unknown4 { get; set; }
        private ushort Unknown4_1 { get; set; }
        private uint Unknown5 { get; set; }
        private uint Unknown6 { get; set; }
        public int InfoDataCount { get; set; }
        private uint Unknown8 { get; set; }
        public uint Hash { get; private set; }
        private uint Unknown9 { get; set; }
        private uint Unknown10 { get; set; }
        private uint Unknown11 { get; set; }
        private uint Unknown12 { get; set; }
        private uint Unknown13 { get; set; }
        private uint Unknown14 { get; set; }
        private uint Unknown15 { get; set; }
        private uint Unknown16 { get; set; }
        private uint Unknown17 { get; set; }

        private SimpleArray<uint> InfoDataOffsets { get; set; }
        private SimpleArray<byte> InfoDataTypes { get; set; }
        private SimpleArray<uint> InfoDataIDs { get; set; }

        public Dictionary<MaterialInfoDataID, MaterialInfoDataObject> InfoDatas { get; private set; }

        public string ShaderName { get; private set; }
        public string ShaderSPS { get; private set; }

        public T GetInfoData<T>(MaterialInfoDataID id) where T: MaterialInfoDataObject
        {
            MaterialInfoDataObject value;
            InfoDatas.TryGetValue(id, out value);
            return value as T;
        }

        #region Implementation of IFileAccess

        public void Read(BinaryReader br)
        {
            VTable = br.ReadUInt32();

            BlockMapOffset = ResourceUtil.ReadOffset(br);

            Unknown1 = br.ReadUInt16();
            Unknown2 = br.ReadByte();
            Unknown3 = br.ReadByte();

            Unknown4 = br.ReadUInt16();
            Unknown4_1 = br.ReadUInt16();

            Unknown5 = br.ReadUInt32();

            var infoOffsetTablePtr = ResourceUtil.ReadOffset(br);

            Unknown6 = br.ReadUInt32();
            InfoDataCount = br.ReadInt32();
            Unknown8 = br.ReadUInt32();

            var infoTypePtr = ResourceUtil.ReadOffset(br);

            Hash = br.ReadUInt32();
            Unknown9 = br.ReadUInt32();
            Unknown10 = br.ReadUInt32();

            var infoIDsPtr = ResourceUtil.ReadOffset(br);

            Unknown11 = br.ReadUInt32();
            Unknown12 = br.ReadUInt32();
            Unknown13 = br.ReadUInt32();

            // shaderFx starts here...
            var shaderNamePtr = ResourceUtil.ReadOffset(br);
            var shaderSpsPtr = ResourceUtil.ReadOffset(br);

            Unknown14 = br.ReadUInt32();
            Unknown15 = br.ReadUInt32();
            Unknown16 = br.ReadUInt32();
            Unknown17 = br.ReadUInt32();

            //

            br.BaseStream.Seek(infoOffsetTablePtr, SeekOrigin.Begin);
            InfoDataOffsets = new SimpleArray<uint>(br, InfoDataCount, ResourceUtil.ReadOffset);

            br.BaseStream.Seek(infoTypePtr, SeekOrigin.Begin);
            InfoDataTypes = new SimpleArray<byte>(br, InfoDataCount, r => r.ReadByte());

            br.BaseStream.Seek(infoIDsPtr, SeekOrigin.Begin);
            InfoDataIDs = new SimpleArray<uint>(br, InfoDataCount, r => r.ReadUInt32());

            br.BaseStream.Seek(shaderNamePtr, SeekOrigin.Begin);
            ShaderName = ResourceUtil.ReadNullTerminatedString(br);

            br.BaseStream.Seek(shaderSpsPtr, SeekOrigin.Begin);
            ShaderSPS = ResourceUtil.ReadNullTerminatedString(br);

            InfoDatas = new Dictionary<MaterialInfoDataID, MaterialInfoDataObject>(InfoDataCount);
            for(int i=0; i<InfoDataCount; i++)
            {
                try
                {
                    var obj = MaterialInfoDataObjectFactory.Create((MaterialInfoDataType)InfoDataTypes[i]);

                    br.BaseStream.Seek(InfoDataOffsets[i], SeekOrigin.Begin);
                    obj.Read(br);

                    InfoDatas.Add((MaterialInfoDataID)InfoDataIDs[i], obj);
                }
                catch
                {
                    InfoDatas.Add((MaterialInfoDataID)InfoDataIDs[i], null);
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