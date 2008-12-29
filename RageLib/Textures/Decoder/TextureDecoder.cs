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
using System.Drawing.Imaging;

namespace RageLib.Textures.Decoder
{
    internal class TextureDecoder
    {
        internal static Image Decode(Texture texture)
        {
            uint width = texture.Info.Width;
            uint height = texture.Info.Height;

            byte[] data = texture.TextureData;
            
            switch(texture.TextureType)
            {
                case TextureType.DXT1:
                    data = DXTDecoder.DecodeDXT1(data, (int)width, (int)height);
                    break;
                case TextureType.DXT3:
                    data = DXTDecoder.DecodeDXT3(data, (int)width, (int)height);
                    break;
                case TextureType.DXT5:
                    data = DXTDecoder.DecodeDXT5(data, (int)width, (int)height);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Bitmap bmp = new Bitmap((int) width, (int) height, PixelFormat.Format32bppArgb);

            Rectangle rect = new Rectangle(0, 0, (int) width, (int) height);
            BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            System.Runtime.InteropServices.Marshal.Copy(data, 0, bmpdata.Scan0, (int) width*(int) height*4);
            bmp.UnlockBits(bmpdata);

            return bmp;

            /*
            
            // For the XBOX360, we had to use a larger surface to do the decoding... so crop it to the real size.
             
            Bitmap croppedBitmap = new Bitmap((int)texture.Width, (int)texture.Height);
            Graphics g = Graphics.FromImage(croppedBitmap);
            Rectangle croppedRect = new Rectangle(0, 0, (int) texture.Width, (int) texture.Height);
            g.DrawImage(bmp, croppedRect, croppedRect, GraphicsUnit.Pixel);

            return croppedBitmap;
             */
        }
    }
}
