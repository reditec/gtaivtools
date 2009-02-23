/**********************************************************************\

 RageLib - Models
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

using RageLib.Models.Resource;

namespace RageLib.Models.Data
{
    public class MaterialParam
    {
        public uint NameHash { get; private set; }

        public MaterialParam(uint nameHash)
        {
            NameHash = nameHash;
        }

        internal static MaterialParam Create(uint nameHash, MaterialInfoDataObject obj)
        {
            if (obj is MaterialInfoDataMatrix)
            {
                return new MaterialParamMatrix44(nameHash, obj as MaterialInfoDataMatrix);
            }
            if (obj is MaterialInfoDataVector4)
            {
                return new MaterialParamVector4(nameHash, obj as MaterialInfoDataVector4);
            }
            if (obj is MaterialInfoDataTexture)
            {
                return new MaterialParamTexture(nameHash, obj as MaterialInfoDataTexture);
            }
            return null;
        }
    }
}