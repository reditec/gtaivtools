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
    // gtaDrawable 
    class DrawableModel : IFileAccess, IDataReader, IEmbeddedResourceReader, IDisposable
    {
        public uint VTable { get; private set; }

        private uint BlockMapOffset { get; set; }

        public MaterialInfo MaterialInfos { get; set; }
        public UnDocData ModelInfo { get; private set; }        // Probably has data on the mount points!

        public Vector4 Center { get; private set; }
        public Vector4 BoundsMin { get; private set; }
        public Vector4 BoundsMax { get; private set; }

        public PtrCollection<GeometryInfo>[] GeometryInfos { get; private set; }

        public Vector4 AbsoluteMax { get; private set; }

        private uint Unk1 { get; set; }     // either 1 or 9
        
        private uint Neg1 { get; set; }
        private uint Neg2 { get; set; }
        private uint Neg3 { get; set; }

        private float Unk2 { get; set; }

        private uint Unk3 { get; set; }
        private uint Unk4 { get; set; }
        private uint Unk5 { get; set; }

        private uint Unk6 { get; set; }  // This should be a CSimpleCollection
        private uint Unk7 { get; set; }

        public void ReadData(BinaryReader br)
        {
            foreach (var geometryInfo in GeometryInfos)
            {
                foreach (var info in geometryInfo)
                {
                    foreach (var dataInfo in info.GeometryDataInfos)
                    {
                        dataInfo.VertexDataInfo.ReadData(br);
                        dataInfo.IndexDataInfo.ReadData(br);
                    }                    
                }
            }
        }

        #region IFileAccess Members

        public void Read(BinaryReader br)
        {
            // rage::datBase
            VTable = br.ReadUInt32();

            // rage::pgBase
            BlockMapOffset = ResourceUtil.ReadOffset(br);

            // rage::rmcDrawableBase
            //    rage::rmcDrawable
            //        gtaDrawable

            var materialInfoOffset = ResourceUtil.ReadOffset(br);
            var modelInfoOffset = ResourceUtil.ReadOffset(br);

            Center = new Vector4(br);
            BoundsMin = new Vector4(br);
            BoundsMax = new Vector4(br);

            int geometryInfoCount = 0;
            var geometryInfoOffsets = new uint[4];
            for (int i = 0; i < 4; i++)
            {
                geometryInfoOffsets[i] = ResourceUtil.ReadOffset(br);
                if (geometryInfoOffsets[i] != 0)
                {
                    geometryInfoCount++;
                }
            }

            AbsoluteMax = new Vector4(br);

            Unk1 = br.ReadUInt32();

            Neg1 = br.ReadUInt32();
            Neg2 = br.ReadUInt32();
            Neg3 = br.ReadUInt32();

            Unk2 = br.ReadSingle();

            Unk3 = br.ReadUInt32();
            Unk4 = br.ReadUInt32();
            Unk5 = br.ReadUInt32();
            Unk6 = br.ReadUInt32();
            Unk7 = br.ReadUInt32();

            //

            if (materialInfoOffset != 0)
            {
                br.BaseStream.Seek(materialInfoOffset, SeekOrigin.Begin);
                MaterialInfos = new MaterialInfo(br);
            }

            br.BaseStream.Seek(modelInfoOffset, SeekOrigin.Begin);
            ModelInfo = new UnDocData(br);

            GeometryInfos = new PtrCollection<GeometryInfo>[geometryInfoCount];
            for (int i = 0; i < geometryInfoCount; i++)
            {
                br.BaseStream.Seek(geometryInfoOffsets[i], SeekOrigin.Begin);
                GeometryInfos[i] = new PtrCollection<GeometryInfo>(br);
            }
        }

        public void Write(BinaryWriter bw)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IEmbeddedResourceReader

        public void ReadEmbeddedResources(Stream systemMemory, Stream graphicsMemory)
        {
            if (MaterialInfos.TextureDictionaryOffset != 0)
            {
                systemMemory.Seek(MaterialInfos.TextureDictionaryOffset, SeekOrigin.Begin);

                MaterialInfos.TextureDictionary = new TextureFile();
                MaterialInfos.TextureDictionary.Open(systemMemory, graphicsMemory);
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (MaterialInfos != null)
            {
                if (MaterialInfos.TextureDictionary != null)
                {
                    MaterialInfos.TextureDictionary.Dispose();
                    MaterialInfos.TextureDictionary = null;
                }
            }
        }

        #endregion
    }
}
