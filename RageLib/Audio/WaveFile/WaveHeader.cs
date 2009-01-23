/**********************************************************************\

 RageLib - Audio
 Copyright (C) 2009  Arushan/Aru <oneforaru at gmail.com>

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

namespace RageLib.Audio.WaveFile
{
    class WaveHeader : IFileAccess
    {
        // RIFF chunk
        int RiffChunkID;
        int RiffChunkSize;
        int Format;

        // fmt sub-chunk
        private int FmtChunkID;
        private int FmtChunkSize;
        private short AudioFormat;
        private short NumChannels;
        private int SampleRate;
        private int ByteRate;
        private short BlockAlign;
        private short BitsPerSample;

        // data sub-chunk
        private int DataChunkID;
        private int DataChunkSize;

        public WaveHeader()
        {
            RiffChunkID = 0x46464952;   // "RIFF"
            Format = 0x45564157;        // "WAVE"

            FmtChunkID = 0x20746D66;    // "fmt "
            FmtChunkSize = 0x10;
            AudioFormat = 1;
            NumChannels = 1;    // Mono only for now
            
            BlockAlign = 2;
            BitsPerSample = 16; // 16bit audio only for now

            DataChunkID = 0x61746164;   // "data"
        }

        public int FileSize
        {
            set
            {
                RiffChunkSize = value - 8;
                DataChunkSize = value - 44;
            }
        }

        public int SamplesPerSecond
        {
            set
            {
                SampleRate = value;
                ByteRate = value*2;     // 16 bit
            }
        }

        #region Implementation of IFileAccess

        public void Read(BinaryReader br)
        {
            RiffChunkID = br.ReadInt32();
            RiffChunkSize = br.ReadInt32();
            Format = br.ReadInt32();

            FmtChunkID = br.ReadInt32();
            FmtChunkSize = br.ReadInt32();
            AudioFormat = br.ReadInt16();
            NumChannels = br.ReadInt16();
            SampleRate = br.ReadInt32();
            ByteRate = br.ReadInt32();
            BlockAlign = br.ReadInt16();
            BitsPerSample = br.ReadInt16();

            DataChunkID = br.ReadInt32();
            DataChunkSize = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(RiffChunkID);
            bw.Write(RiffChunkSize);
            bw.Write(Format);

            bw.Write(FmtChunkID);
            bw.Write(FmtChunkSize);
            bw.Write(AudioFormat);
            bw.Write(NumChannels);
            bw.Write(SampleRate);
            bw.Write(ByteRate);
            bw.Write(BlockAlign);
            bw.Write(BitsPerSample);

            bw.Write(DataChunkID);
            bw.Write(DataChunkSize);
        }

        #endregion
    }
}