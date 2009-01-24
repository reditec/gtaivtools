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

using System;
using System.Collections.Generic;
using System.IO;
using RageLib.Audio.SoundBank.MultiChannel;

namespace RageLib.Audio.SoundBank
{
    class SoundBankMultiChannel : ISoundBank
    {
        public const int BlockSize = 2048;

        private Header _fileHeader;
        private ChannelInfoHeader[] _channelInfoHeader;
        private ChannelInfo[] _channelInfo;
        private bool _isCompressed = true;
        private BlockInfoHeader[] _blockInfoHeader;
        private BlockInfo[] _blockInfo;
        private int _sizeBlockHeader = BlockSize;

        private List<ISoundWave> _waveInfos;

        #region Implementation of ISoundBank

        public int WaveCount
        {
            get { return _waveInfos.Count; }
        }

        public ISoundWave this[int index]
        {
            get { return _waveInfos[index]; }
        }

        public int ExportWaveBlockAsPCM(int waveIndex, int blockIndex, ref DviAdpcmDecoder.AdpcmState state, Stream soundBankStream, Stream outStream)
        {
            // waveIndex would be the channel here...
            // blockIndex is the real block index

            BinaryWriter writer = new BinaryWriter(outStream);

            int offset = _blockInfo[blockIndex].computed_offset + _sizeBlockHeader;
            soundBankStream.Seek(offset, SeekOrigin.Begin);

            if (_isCompressed)
            {
                for (int i = 0; i < _blockInfo[blockIndex].codeIndices.Length; i++)
                {
                    int currentChannel = _blockInfo[blockIndex].codeIndices[i].computed_channel;
                    if (currentChannel == waveIndex)
                    {
                        int adpcmIndex = _blockInfo[blockIndex].codeIndices[i].computed_adpcmIndex;
                        if (adpcmIndex < _channelInfo[currentChannel].adpcmInfo.states.Length)
                        {
                            state = _channelInfo[currentChannel].adpcmInfo.states[adpcmIndex];
                        }

                        byte[] buffer = new byte[BlockSize];
                        soundBankStream.Read(buffer, 0, BlockSize);

                        for (int j = 0; j < BlockSize; j++)
                        {
                            byte code = buffer[j];
                            writer.Write(DviAdpcmDecoder.DecodeAdpcm((byte) (code & 0xf), ref state));
                            writer.Write(DviAdpcmDecoder.DecodeAdpcm((byte) ((code >> 4) & 0xf), ref state));
                        }
                    }
                    else
                    {
                        soundBankStream.Seek(2048, SeekOrigin.Current);
                    }
                }

            }
            else
            {
                for (int i = 0; i < _blockInfo[blockIndex].codeIndices.Length; i++)
                {
                    int currentChannel = _blockInfo[blockIndex].codeIndices[i].computed_channel;

                    if (currentChannel == waveIndex)
                    {
                        short[] block = new short[BlockSize / 2];

                        // all the seeking is done here...
                        soundBankStream.Seek(_blockInfo[blockIndex].computed_offset + i * BlockSize + _sizeBlockHeader, SeekOrigin.Begin);
                        for (int j = 0; j < BlockSize / 2; j++)
                        {
                            byte[] b = new byte[2];
                            soundBankStream.Read(b, 0, 2);
                            block[j] = BitConverter.ToInt16(b, 0);
                        }

                        // this adjusts for weird values in codeIndices.
                        if (_blockInfo[blockIndex].channelInfo[currentChannel].offset < 0)
                        {
                            _blockInfo[blockIndex].codeIndices[i].startIndex -=
                                _blockInfo[blockIndex].channelInfo[currentChannel].offset;
                            _blockInfo[blockIndex].channelInfo[currentChannel].offset = 0;
                        }
                        else if (_blockInfo[blockIndex].channelInfo[currentChannel].offset > 0)
                        {
                            int len = _blockInfo[blockIndex].channelInfo[currentChannel].offset;
                            short[] newblock = new short[BlockSize / 2 - len];
                            Array.Copy(block, len, newblock, 0, BlockSize / 2 - len);
                            block = newblock;
                            _blockInfo[blockIndex].channelInfo[currentChannel].offset = 0;
                        }

                        int count = _blockInfo[blockIndex].codeIndices[i].endIndex -
                                    _blockInfo[blockIndex].codeIndices[i].startIndex;
                        for (int j = 0; j <= count; j++)
                        {
                            writer.Write(block[j]);
                        }
                    }
                }
            }
            return 0;

        }

        public void ExportAsPCM(int index, Stream soundBankStream, Stream outStream)
        {
            int count = _fileHeader.numBlocks;

            DviAdpcmDecoder.AdpcmState state = new DviAdpcmDecoder.AdpcmState();

            for (int k = 0; k < count; k++)
            {
                ExportWaveBlockAsPCM(index, k, ref state, soundBankStream, outStream);
            }
        }

        #endregion

        #region Implementation of IFileAccess

        public void Read(BinaryReader br)
        {
            // Read header

            _fileHeader = new Header(br);

            bool errorCondition = false;

            if (_fileHeader.unk1Reserved != 0) errorCondition = true;
            if (!(_fileHeader.numBlocks > 0)) errorCondition = true;
            if (!(_fileHeader.sizeBlock > 0)) errorCondition = true;
            if (_fileHeader.numBlocks * _fileHeader.sizeBlock > br.BaseStream.Length) errorCondition = true;
            if (_fileHeader.unk2Reserved != 0) errorCondition = true;
            if (_fileHeader.unk4Reserved != 0) errorCondition = true;
            if (_fileHeader.unk6Reserved != 0) errorCondition = true;
            if (_fileHeader.offsetBlockInfo > _fileHeader.sizeHeader) errorCondition = true;
            if (_fileHeader.unk5offset > _fileHeader.sizeHeader) errorCondition = true;

            if (errorCondition)
            {
                throw new SoundBankException("Unexpected values in Header");
            }

            // read adpcm state info

            br.BaseStream.Seek(_fileHeader.offsetChannelInfo, SeekOrigin.Begin);
            _channelInfoHeader = new ChannelInfoHeader[_fileHeader.numChannels];
            for (int i = 0; i < _fileHeader.numChannels; i++)
            {
                _channelInfoHeader[i] = new ChannelInfoHeader(br);
            }
            
            _channelInfo = new ChannelInfo[_fileHeader.numChannels];
            var currentOffset = br.BaseStream.Position;

            for (int i = 0; i < _fileHeader.numChannels; i++)
            {
                br.BaseStream.Seek(currentOffset + _channelInfoHeader[i].offset, SeekOrigin.Begin);
                _channelInfo[i] = new ChannelInfo(br);

                if (_channelInfoHeader[i].size <= 32)
                {
                    _isCompressed = false;
                }
                else
                {
                    _channelInfo[i].adpcmInfo = new AdpcmInfo(br);
                }
            }

            // Read comp block info (in header)
            br.BaseStream.Seek(_fileHeader.offsetBlockInfo, SeekOrigin.Begin);
            _blockInfoHeader = new BlockInfoHeader[_fileHeader.numBlocks];
            for (int i = 0; i < _fileHeader.numBlocks; i++)
            {
                _blockInfoHeader[i] = new BlockInfoHeader(br);
            }

            // Read comp block info / channel info
            _blockInfo = new BlockInfo[_fileHeader.numBlocks];
            for (int i = 0; i < _fileHeader.numBlocks; i++)
            {
                var computedOffset = _fileHeader.sizeHeader + _fileHeader.sizeBlock*i;
                br.BaseStream.Seek(computedOffset, SeekOrigin.Begin);

                _blockInfo[i] = new BlockInfo(br);
                _blockInfo[i].computed_offset = computedOffset;

                _blockInfo[i].channelInfo = new BlockChannelInfo[_fileHeader.numChannels];
                int numCodeIndices = 0;
                for(int j=0; j<_fileHeader.numChannels; j++)
                {
                    _blockInfo[i].channelInfo[j] = new BlockChannelInfo(br);
                    
                    int end = _blockInfo[i].channelInfo[j].startIndex + _blockInfo[i].channelInfo[j].count;
                    if (numCodeIndices < end)
                    {
                        numCodeIndices = end;
                    }
                }

                _blockInfo[i].codeIndices = new CodeIndices[numCodeIndices];
                for (int j = 0; j < numCodeIndices; j++)
                {
                    _blockInfo[i].codeIndices[j] = new CodeIndices(br);
                    _blockInfo[i].codeIndices[j].computed_channel = -1;

                    if (_isCompressed)
                    {
                        _blockInfo[i].codeIndices[j].computed_adpcmIndex = _blockInfo[i].codeIndices[j].startIndex/
                                                                            4096;
                    }
                }

                for (int j = 0; j < _fileHeader.numChannels; j++)
                {
                    int channelIdxStart = _blockInfo[i].channelInfo[j].startIndex;
                    int channelIdxCount = _blockInfo[i].channelInfo[j].count;
                    for (int k = 0; k < channelIdxCount; k++)
                    {
                        _blockInfo[i].codeIndices[k + channelIdxStart].computed_channel = j;
                    }
                }

                int new_start =
                    (int) (Math.Ceiling((float) (_blockInfo[i].offset2 + 8*numCodeIndices)/BlockSize)*BlockSize);
                _sizeBlockHeader = Math.Max(_sizeBlockHeader, new_start);
            }

            _waveInfos = new List<ISoundWave>(_fileHeader.numChannels);
            for (int i = 0; i < _fileHeader.numChannels; i++)
            {
                _waveInfos.Add( new SoundWave(_fileHeader, _channelInfo[i]) );
            }
        }

        public void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
