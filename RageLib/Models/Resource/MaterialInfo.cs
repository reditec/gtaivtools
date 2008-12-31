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
using System.IO;
using RageLib.Common;
using RageLib.Common.Resources;
using RageLib.Common.ResourceTypes;
using RageLib.Textures;

namespace RageLib.Models.Resource
{
    /// grmShaderGroup 
    class MaterialInfo : IFileAccess
    {
        private uint VTable { get; set; }

        public uint TextureDictionaryOffset { get; private set; }
        public TextureFile TextureDictionary { get; set; }

        public PtrCollection<MaterialInfoEntry> Entries { get; private set; }

        private SimpleArray<uint> Zeros { get; set; }

        private SimpleCollection<uint> Data2 { get; set; }

        private SimpleCollection<uint> Data3 { get; set; }

        public MaterialInfo()
        {
        }

        public MaterialInfo(BinaryReader br)
        {
            Read(br);
        }

        #region Implementation of IFileAccess

        public void Read(BinaryReader br)
        {
            VTable = br.ReadUInt32();

            TextureDictionaryOffset = ResourceUtil.ReadOffset(br);

            // CPtrCollection<T>
            Entries = new PtrCollection<MaterialInfoEntry>(br);

            Zeros = new SimpleArray<uint>(br, 12, r => r.ReadUInt32());

            Data2 = new SimpleCollection<uint>(br, reader => reader.ReadUInt32());

            Data3 = new SimpleCollection<uint>(br, reader => reader.ReadUInt32());
        }

        public void Write(BinaryWriter bw)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
