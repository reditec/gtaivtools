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
using System.IO;
using System.Windows.Forms;
using RageLib.Audio.WaveFile;

namespace RageLib.Audio
{
    public class AudioViewController
    {
        private AudioView _view;
        private AudioFile _file;
        private AudioPlayer _player;
        private string _lastSaveDirectory;

        public AudioViewController(AudioView view)
        {
            _view = view;
            _view.PlayClicked += View_PlayClicked;
            _view.StopClicked += View_StopClicked;
            _view.ExportWAVClicked += View_ExportWAVClicked;
            _view.SelectedWaveChanged += View_SelectedBlockChanged;
            _view.Disposed += View_Disposed;

            _player = new AudioPlayer();
        }

        private void View_PlayClicked(object sender, EventArgs e)
        {
            if (_view.SelectedWave != null)
            {
                _player.Play(_view.PlayLooped);
            }
        }

        private void View_StopClicked(object sender, EventArgs e)
        {
            _player.Stop();
        }

        public AudioFile AudioFile
        {
            get { return _file; }
            set
            {
                _file = value;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            _view.ClearWaves();

            if (_file != null)
            {
                foreach (var block in _file)
                {
                    _view.AddWave(block);

                    if (_view.SelectedWave == null)
                    {
                        _view.SelectedWave = block;
                    }
                }
            }
        }

        private void View_Disposed(object sender, EventArgs e)
        {
            _player.Stop();

            if (_file != null)
            {
                _file.Dispose();
                _file = null;
            }
        }

        private void View_ExportWAVClicked(object sender, EventArgs e)
        {
            AudioWave wave = _view.SelectedWave;
            if (wave != null)
            {
                var sfd = new SaveFileDialog
                {
                    AddExtension = true,
                    OverwritePrompt = true,
                    Title = "Export as WAV",
                    Filter = "WAVE Audio File (*.wav)|*.wav",
                    InitialDirectory = _lastSaveDirectory,
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var f = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
                    {
                        WaveExport.Export(_file, wave, f);
                    }

                    _lastSaveDirectory = new FileInfo(sfd.FileName).Directory.FullName;

                    MessageBox.Show("Audio exported.", "Export as WAV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
        }

        private void View_SelectedBlockChanged(object sender, EventArgs e)
        {
            _player.Stop();
            _player.Initialize(_file, _view.SelectedWave);

            if (_view.AutoPlay)
            {
                _player.Play(_view.PlayLooped);
            }
        }
    }
}
