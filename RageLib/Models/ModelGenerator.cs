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
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using RageLib.Models.Resource;
using RageLib.Textures;
using Brush=System.Windows.Media.Brush;
using Color=System.Drawing.Color;
using Point=System.Windows.Point;

namespace RageLib.Models
{
    internal static class ModelGenerator
    {
        private static Texture FindTexture(TextureFile textures, string name)
        {
            if (textures == null)
            {
                return null;
            }

            var textureObj = textures.FindTextureByName(name);
            return textureObj;
        }

        internal static Image CreateUVMapImage(DrawableModel drawable)
        {
            var bmp = new Bitmap(512, 512);
            var g = Graphics.FromImage(bmp);
            var pen = new System.Drawing.Pen(Color.Red);

            foreach (var geometryInfo in drawable.GeometryInfos)
            {
                foreach (var dataInfo in geometryInfo.GeometryDataInfos)
                {
                    for (var i = 0; i < dataInfo.FaceCount; i++)
                    {
                        var i1 = (dataInfo.IndexDataInfo.IndexData[i * 3 + 0]);
                        var i2 = (dataInfo.IndexDataInfo.IndexData[i * 3 + 1]);
                        var i3 = (dataInfo.IndexDataInfo.IndexData[i * 3 + 2]);

                        var v1 = dataInfo.VertexDataInfo.VertexData[i1];
                        var v2 = dataInfo.VertexDataInfo.VertexData[i2];
                        var v3 = dataInfo.VertexDataInfo.VertexData[i3];

                        g.DrawLine(pen, v1.TextureU * bmp.Width, v1.TextureV * bmp.Height, v2.TextureU * bmp.Width, v2.TextureV * bmp.Height);
                        g.DrawLine(pen, v1.TextureU * bmp.Width, v1.TextureV * bmp.Height, v3.TextureU * bmp.Width, v3.TextureV * bmp.Height);
                        g.DrawLine(pen, v2.TextureU * bmp.Width, v2.TextureV * bmp.Height, v3.TextureU * bmp.Width, v3.TextureV * bmp.Height);
                    }
                }
            }

            g.Dispose();

            return bmp;
        }

        internal static Model3DGroup GenerateModel(DrawableModel drawable, TextureFile textures)
        {
            var random = new Random();

            var materials = new Material[drawable.MaterialInfos.Entries.Count];
            for(int i=0; i<materials.Length; i++)
            {
                Brush brush =
                    new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte) random.Next(0, 255),
                                                                            (byte) random.Next(0, 255),
                                                                            (byte) random.Next(0, 255)));

                var drawableMat = drawable.MaterialInfos.Entries[i];
                var texture = drawableMat.GetInfoData<MaterialInfoDataTexture>(MaterialInfoDataID.Texture);
                if (texture != null)
                {
                    // 1. Try looking in the embedded texture file (if any)
                    var textureObj = FindTexture(drawable.MaterialInfos.TextureDictionary, texture.TextureName);
                    
                    // 2. Try looking in any attached external texture dictionaries
                    if (textureObj == null)
                    {
                        textureObj = FindTexture(textures, texture.TextureName);
                    }

                    // Generate a brush if we were successful
                    if (textureObj != null)
                    {
                        var bitmap = textureObj.Decode() as Bitmap;
                        
                        var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                            bitmap.GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());

                        // For memory leak work around
                        bitmapSource.Freeze();

                        brush = new ImageBrush(bitmapSource);
                        (brush as ImageBrush).ViewportUnits = BrushMappingMode.Absolute;
                        (brush as ImageBrush).TileMode = TileMode.Tile;

                        bitmap.Dispose();
                    }
                }

                materials[i] = new DiffuseMaterial(brush); ;
            }

            var mainModelGroup = new Model3DGroup();
            foreach (var geometryInfo in drawable.GeometryInfos)
            {
                var index = 0;
                var modelGroup = new Model3DGroup();
                foreach (var dataInfo in geometryInfo.GeometryDataInfos)
                {
                    var mesh = new MeshGeometry3D();

                    foreach (var vertex in dataInfo.VertexDataInfo.VertexData)
                    {
                        mesh.Positions.Add(new Point3D(vertex.X, vertex.Y, vertex.Z));
                        mesh.Normals.Add(new Vector3D(vertex.NormalX, vertex.NormalY, vertex.NormalZ));
                        mesh.TextureCoordinates.Add(new Point(vertex.TextureU, vertex.TextureV));
                    }
                    for (int i = 0; i < dataInfo.FaceCount; i++)
                    {
                        mesh.TriangleIndices.Add(dataInfo.IndexDataInfo.IndexData[i * 3 + 0]);
                        mesh.TriangleIndices.Add(dataInfo.IndexDataInfo.IndexData[i * 3 + 1]);
                        mesh.TriangleIndices.Add(dataInfo.IndexDataInfo.IndexData[i * 3 + 2]);
                    }

                    var material = materials[geometryInfo.MaterialMappings[index]];
                    var model = new GeometryModel3D(mesh, material);

                    modelGroup.Children.Add(model);

                    index++;
                }
                mainModelGroup.Children.Add(modelGroup);
            }
            return mainModelGroup;
        }
    }
}
