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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RageLib.Audio.SoundBank;

namespace RageLib.Audio
{
    public class AudioFile : IEnumerable<AudioWave>, IDisposable
    {
        private SoundBankFile _file;
        public List<AudioWave> Blocks { get; private set; }

        internal Stream Stream
        {
            get { return _file.Stream; }
        }

        internal ISoundBank SoundBank
        {
            get { return _file.SoundBank; }
        }

        public int Count
        {
            get { return Blocks.Count; }
        }

        public void Open(string filename)
        {
            _file = new SoundBankFile();
            _file.Open(filename);
            BuildAudioBlocks();
        }

        public void Open(Stream stream)
        {
            _file = new SoundBankFile();
            _file.Open(stream);
            BuildAudioBlocks();
        }


        private void BuildAudioBlocks()
        {
            Blocks = new List<AudioWave>();

            int count = _file.SoundBank.BlockCount;
            for (int i = 0; i < count; i++)
            {
                AudioWave wave = new AudioWave(i);
                wave.SoundWave = _file.SoundBank[i];
                Blocks.Add(wave);
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<AudioWave> GetEnumerator()
        {
            return Blocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (_file != null)
            {
                _file.Dispose();
                _file = null;
            }

            Blocks.Clear();
            Blocks = null;
        }

        #endregion
    }
}