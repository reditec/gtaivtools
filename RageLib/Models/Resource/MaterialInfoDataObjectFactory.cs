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

namespace RageLib.Models.Resource
{
    internal static class MaterialInfoDataObjectFactory
    {
        public static MaterialInfoDataObject Create(MaterialInfoDataType type)
        {
            switch(type)
            {
                case MaterialInfoDataType.Texture:
                    return new MaterialInfoDataTexture();
                case MaterialInfoDataType.Vector4:
                    return new MaterialInfoDataVector4();
                case MaterialInfoDataType.Matrix:
                    return new MaterialInfoDataMatrix();
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}