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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using RageLib.Textures.Resource;
using File=RageLib.Textures.Resource.File;

namespace RageLib.Textures
{
    public class TextureFile : IEnumerable<Texture>
    {
        private File _file;
        public List<Texture> Textures { get; private set; }

        public int Count
        {
            get { return Textures.Count; }
        }

        public void Open(string filename)
        {
            _file = new File();
            _file.Open(filename);
            BuildTextures();
        }

        public void Open(Stream stream)
        {
            _file = new File();
            _file.Open(stream);
            BuildTextures();
        }

        public void Open(Stream systemMemory, Stream graphicsMemory)
        {
            _file = new File();
            _file.Open(systemMemory, graphicsMemory);
            BuildTextures();
        }

        public void Save(Stream stream)
        {
            _file.Save(stream);
        }

        public void Save(Stream systemMemory, Stream graphicsMemory)
        {
            _file.Save(systemMemory, graphicsMemory);
        }

        private void BuildTextures()
        {
            Textures = new List<Texture>(_file.Header.TextureCount);

            foreach (TextureInfo info in _file.Textures)
            {
                Textures.Add(new Texture(info));
            }
        }

        public Texture FindTextureByName(string name)
        {
            name = name.ToLower();
            foreach (var texture in this)
            {
                if (texture.Name.ToLower() == name)
                {
                    return texture;
                }
                if (texture.TitleName.ToLower() == name)
                {
                    return texture;
                }
            }
            return null;
        }

        public IEnumerator<Texture> GetEnumerator()
        {
            return Textures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Textures.GetEnumerator();
        }
    }
}