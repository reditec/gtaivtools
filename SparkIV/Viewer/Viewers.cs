/**********************************************************************\

 Spark IV
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

using System.Collections.Generic;
using System.Windows.Forms;
using RageLib.FileSystem.Common;

namespace SparkIV.Viewer
{
    static class Viewers
    {
        static readonly List<IViewer> _viewers = new List<IViewer>();

        static Viewers()
        {
            _viewers.Add(new TextViewer());
            _viewers.Add(new Xml.XmlViewer());
            _viewers.Add(new Script.ScriptViewer());
            _viewers.Add(new Textures.TextureViewer());
            _viewers.Add(new Models.ModelViewer());
            _viewers.Add(new Models.ModelDictionaryViewer());
        }

        public static Control Get(File file)
        {
            string fileName = file.Name;
            string extension = fileName.Substring(fileName.LastIndexOf('.') + 1);

            foreach (var viewer in _viewers)
            {
                if (viewer.SupportsExtension(extension))
                {
                    var control = viewer.GetView(file);
                    if (control != null)
                    {
                        return control;
                    }
                }
            }
            return null;
        }
    }
}
