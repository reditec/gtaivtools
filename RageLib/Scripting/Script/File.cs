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
using System.IO;
using RageLib.Common;

namespace RageLib.Scripting.Script
{
    internal class File
    {
        public File()
        {
            Header = new Header(this);
        }

        public Header Header { get; set; }
        public byte[] Code { get; set; }
        public uint[] LocalVars { get; set; }
        public uint[] GlobalVars { get; set; }

        public bool Open(string filename)
        {
            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            return Open(fs);
        }

        public bool Open(Stream stream)
        {
            var br = new BinaryReader(stream);

            Header.Read(br);

            if (Header.Identifier != Header.Magic && Header.Identifier != Header.MagicEncrypted)
            {
                stream.Close();
                return false;
            }

            bool encrypted = Header.Identifier == Header.MagicEncrypted;

            Code = br.ReadBytes(Header.CodeSize);
            byte[] data1 = br.ReadBytes(Header.LocalVarCount*4);
            byte[] data2 = br.ReadBytes(Header.GlobalVarCount*4);

            if (encrypted)
            {
                Code = DataUtil.Decrypt(Code);
                data1 = DataUtil.Decrypt(data1);
                data2 = DataUtil.Decrypt(data2);
            }

            LocalVars = new uint[Header.LocalVarCount];
            for (int i = 0; i < Header.LocalVarCount; i++)
            {
                LocalVars[i] = BitConverter.ToUInt32(data1, i*4);
            }

            GlobalVars = new uint[Header.GlobalVarCount];
            for (int i = 0; i < Header.GlobalVarCount; i++)
            {
                GlobalVars[i] = BitConverter.ToUInt32(data2, i*4);
            }

            stream.Close();

            return true;
        }
    }
}