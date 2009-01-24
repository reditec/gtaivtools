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
using System.Windows.Forms;

namespace RageLib.Audio
{
    public partial class AudioView : UserControl
    {
        private static bool _AutoPlay;
        private static bool _PlayLooped;

        public AudioView()
        {
            InitializeComponent();

            chkAutoPlay.Checked = _AutoPlay;
            chkPlayLooped.Checked = _PlayLooped;

            chkAutoPlay.CheckedChanged += delegate { _AutoPlay = chkAutoPlay.Checked; };
            chkPlayLooped.CheckedChanged += delegate { _PlayLooped = chkPlayLooped.Checked; };
        }

        public event EventHandler PlayClicked
        {
            add { btnPlay.Click += value; }
            remove { btnPlay.Click -= value; }
        }

        public event EventHandler StopClicked
        {
            add { btnStop.Click += value; }
            remove { btnStop.Click -= value; }
        }

        public event EventHandler ExportWAVClicked
        {
            add { btnExport.Click += value; }
            remove { btnExport.Click -= value; }
        }

        public event EventHandler SelectedWaveChanged
        {
            add { listAudioBlocks.SelectedIndexChanged += value; }
            remove { listAudioBlocks.SelectedIndexChanged -= value; }
        }

        public bool AutoPlay
        {
            get { return chkAutoPlay.Checked; }
            set { chkAutoPlay.Checked = value; }
        }

        public bool PlayLooped
        {
            get { return chkPlayLooped.Checked; }
            set { chkPlayLooped.Checked = value; }
        }

        public AudioWave SelectWave
        {
            get { return listAudioBlocks.SelectedItem as AudioWave; }
            set { listAudioBlocks.SelectedItem = value; }
        }

        public void ClearWaves()
        {
            listAudioBlocks.SelectedItem = null;
            listAudioBlocks.Items.Clear();
        }

        public void AddWave(AudioWave audioWave)
        {
            listAudioBlocks.Items.Add(audioWave);
        }

        public AudioWave SelectedWave
        {
            get { return listAudioBlocks.SelectedItem as AudioWave; }
            set { listAudioBlocks.SelectedItem = value; }
        }

    }
}
