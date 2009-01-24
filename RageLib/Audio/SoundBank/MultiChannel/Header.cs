/**********************************************************************\

 RageLib - Audio
 
 Copyright (C) 2009  DerPlaya78
 Portions Copyright (C) 2009  Arushan/Aru <oneforaru at gmail.com>

 Modified and adapted for RageLib from iv_audio_rip
 
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

namespace RageLib.Audio.SoundBank.MultiChannel
{
    internal struct Header : IFileAccess
    {
        public int offsetBlockInfo;
        public int unk1Reserved;
        public int numBlocks;
        public int sizeBlock;
        public int unk2Reserved;
        public int offsetChannelInfo;
        public int unk4Reserved;
        public int unk5offset;
        public int unk6Reserved;
        public int numChannels;
        public int unk7IsCompressed;
        public int sizeHeader;

        public Header(BinaryReader br) : this()
        {
            Read(br);
        }

        #region Implementation of IFileAccess

        public void Read(BinaryReader br)
        {
            offsetBlockInfo = br.ReadInt32();
            unk1Reserved = br.ReadInt32();
            numBlocks = br.ReadInt32();
            sizeBlock = br.ReadInt32();
            unk2Reserved = br.ReadInt32();
            offsetChannelInfo = br.ReadInt32();
            unk4Reserved = br.ReadInt32();
            unk5offset = br.ReadInt32();
            unk6Reserved = br.ReadInt32();
            numChannels = br.ReadInt32();
            unk7IsCompressed = br.ReadInt32();
            sizeHeader = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}