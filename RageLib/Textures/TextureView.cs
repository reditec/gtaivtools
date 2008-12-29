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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Brush=System.Drawing.Brush;
using FontStyle=System.Drawing.FontStyle;
using Graphics=System.Drawing.Graphics;
using SystemBrushes=System.Drawing.SystemBrushes;

namespace RageLib.Textures
{
    public partial class TextureView : UserControl
    {
        private string _lastSaveDirectory;

        private TextureFile _textureFile;
        private const int TextureListIconPadding = 2;
        private const int TextureListIconSize = Texture.ThumbnailSize;

        public TextureView()
        {
            InitializeComponent();
        }

        public TextureFile TextureFile
        {
            get
            {
                return _textureFile;
            }
            set
            {
                _textureFile = value;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            listTextures.Items.Clear();
            picPreview.Image = null;
            picPreview.Size = new Size(1, 1);
            if (_textureFile != null)
            {
                foreach (var texture in _textureFile)
                {
                    listTextures.Items.Add(texture);
                }
                if (listTextures.Items.Count > 0)
                {
                    listTextures.SelectedIndex = 0;
                }

                tslTexturesInfo.Text = _textureFile.Count + " Textures";
            }
        }

        private void listTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            var texture = listTextures.SelectedItem as Texture;
            if (texture != null)
            {
                picPreview.Image = texture.Decode();
            }
        }

        private void listTextures_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = TextureListIconPadding * 2 + TextureListIconSize;
            e.ItemWidth = listTextures.Width;
        }

        private void listTextures_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            bool selected = ((e.State & DrawItemState.Selected) != 0);

            var texture = listTextures.Items[e.Index] as Texture;
            if (texture == null)
                return;

            string format;
            switch(texture.TextureType)
            {
                case TextureType.DXT1:
                    format = "DXT1";
                    break;
                case TextureType.DXT3:
                    format = "DXT3";
                    break;
                case TextureType.DXT5:
                    format = "DXT5";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string textMain = texture.Name.Substring(6);        // skip the pack:/
            string textSub = texture.Width + "x" + texture.Height + " (" + format + ")";
            Font fontNormal = listTextures.Font;
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

            Image thumbnail = texture.DecodeAsThumbnail();

            // Draw the icon
            int iconLeft = TextureListIconPadding + (TextureListIconSize - thumbnail.Width) / 2;
            int iconTop = TextureListIconPadding + (TextureListIconSize - thumbnail.Height) / 2;
            g.DrawImage(thumbnail, iconLeft, iconTop + e.Bounds.Top, thumbnail.Width, thumbnail.Height);

            // Draw the text
            int textLeft = TextureListIconSize + TextureListIconPadding * 2;

            SizeF sizeMain = g.MeasureString(textMain, fontBold);
            SizeF sizeSub = g.MeasureString(textSub, fontNormal);

            int textSpacer = (int)(e.Bounds.Height - (sizeMain.Height + sizeSub.Height + TextureListIconPadding * 2)) / 2;

            g.DrawString(textMain, fontBold, brushFG, textLeft, textSpacer + e.Bounds.Top + TextureListIconPadding);
            g.DrawString(textSub, fontNormal, brushFG, textLeft,
                         textSpacer + sizeMain.Height + e.Bounds.Top + TextureListIconPadding);

        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            var texture = listTextures.SelectedItem as Texture;
            if (texture != null)
            {
                var sfd = new SaveFileDialog
                              {
                                  AddExtension = true,
                                  OverwritePrompt = true,
                                  Title = "Save Texture",
                                  Filter = "Portable Network Graphics (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg",
                                  InitialDirectory = _lastSaveDirectory,
                              };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var image = texture.Decode();

                    var format = ImageFormat.Png;
                    if (sfd.FileName.EndsWith(".jpg") || sfd.FileName.EndsWith(".jpeg"))
                    {
                        format = ImageFormat.Jpeg;
                    }

                    image.Save(sfd.FileName, format);

                    _lastSaveDirectory = new FileInfo(sfd.FileName).Directory.FullName;

                    MessageBox.Show("Texture saved.", "Save Texture", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void tsbSaveAll_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog
                          {
                              Description = "Select path to save textures to...",
                              SelectedPath = _lastSaveDirectory,
                              ShowNewFolderButton = true
                          };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                foreach (var texture in _textureFile)
                {
                    var image = texture.Decode();
                    image.Save(Path.Combine(fbd.SelectedPath, texture.TitleName + ".png"), ImageFormat.Png);
                }

                MessageBox.Show("Textures saved.", "Save All Textures", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}