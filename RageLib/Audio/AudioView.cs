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
using System.Drawing;
using System.Drawing.Drawing2D;
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
            add { tsbExportWave.Click += value; }
            remove { tsbExportWave.Click -= value; }
        }

        public event EventHandler ExportMultichannelWAVClicked
        {
            add { tsbExportMultiChannel.Click += value; }
            remove { tsbExportMultiChannel.Click -= value; }
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

        public bool SupportsMultichannelExport
        {
            set
            {
                tsbExportMultiChannel.Enabled = value;
            }
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

        private void listAudioBlocks_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 32;
            e.ItemWidth = listAudioBlocks.Width;
        }

        private void listAudioBlocks_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            bool selected = ((e.State & DrawItemState.Selected) != 0);

            var wave = listAudioBlocks.Items[e.Index] as AudioWave;
            if (wave == null)
                return;

            string textMain = wave.ToString();
            string textSub = wave.Length.ToString();
            Font fontNormal = listAudioBlocks.Font;
            Font fontBold = new Font(fontNormal, FontStyle.Bold);
            Brush brushFG = selected ? SystemBrushes.HighlightText : SystemBrushes.ControlText;

            Graphics g = e.Graphics;

            // Clear the background
            if (selected)
            {
                Brush brushBG =
                    new LinearGradientBrush(e.Bounds, SystemColors.Highlight, SystemColors.HotTrack,
                                            LinearGradientMode.Horizontal);
                g.FillRectangle(brushBG, e.Bounds);
            }
            else
            {
                Brush brushBG = SystemBrushes.Window;
                g.FillRectangle(brushBG, e.Bounds);
            }

            const int ListPadding = 2;

            // Draw the text
            int textLeft = ListPadding * 2;

            SizeF sizeMain = g.MeasureString(textMain, fontBold);
            SizeF sizeSub = g.MeasureString(textSub, fontNormal);

            int textSpacer = (int)(e.Bounds.Height - (sizeMain.Height + sizeSub.Height + ListPadding * 2)) / 2;

            g.DrawString(textMain, fontBold, brushFG, textLeft, textSpacer + e.Bounds.Top + ListPadding);
            g.DrawString(textSub, fontNormal, brushFG, textLeft,
                         textSpacer + sizeMain.Height + e.Bounds.Top + ListPadding);

        }

    }
}
