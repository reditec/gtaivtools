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
using System.Security.Cryptography;
using Microsoft.Win32;

namespace RageLib.Common
{
    public static class KeyUtil
    {
        public static string FindGTADirectory()
        {
            string dir = null;

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "gtaiv.exe")))
            {
                dir = Directory.GetCurrentDirectory();
            }
            else
            {
                const string Key32 = @"SOFTWARE\Rockstar Games\Grand Theft Auto IV";
                const string Key64 = @"SOFTWARE\Wow6432Node\Rockstar Games\Grand Theft Auto IV";
                const string ValueName = "InstallFolder";

                RegistryKey key;
                if ((key = Registry.LocalMachine.OpenSubKey(Key32)) != null ||
                    (key = Registry.LocalMachine.OpenSubKey(Key64)) != null)
                {
                    dir = key.GetValue(ValueName).ToString();
                }
            }

            return dir;
        }

        public static byte[] FindKey(string gtaPath)
        {
            var gtaExe = Path.Combine(gtaPath, "gtaiv.exe");

            uint[] searchOffsets = {
                                       //Old EFIGS EXEs
                                       0xA94204 /* 1.0 */, 
                                       0xB607C4 /* 1.0.1 */, 
                                       0xB56BC4 /* 1.0.2 */,
                                       0xB75C9C /* 1.0.3 */,
                                       0xB7AEF4 /* 1.0.4 */,
                                       //Old Russian EXEs
                                       0xB5B65C /* 1.0.0.1 */,
                                       0xB569F4 /* 1.0.1.1 */,
                                       0xB76CB4 /* 1.0.2.1 */,
                                       0xB7AEFC /* 1.0.3.1 */,
                                       //Old Japan EXEs
				                       0xB8813C /* 1.0.1.2 */,
				                       0xB8C38C /* 1.0.2.2 */,
                                       //Worldwide
                                       0xBE1370 /* 1.0.4r2 */,
									   0xBE6540 /* 1.0.6 */,
                                   };
            const string validHash = "DEA375EF1E6EF2223A1221C2C575C47BF17EFA5E";
            byte[] key = null;

            var fs = new FileStream(gtaExe, FileMode.Open, FileAccess.Read);

            foreach (var u in searchOffsets)
            {
                if (u <= fs.Length - 32)
                {
                    var tempKey = new byte[32];
                    fs.Seek(u, SeekOrigin.Begin);
                    fs.Read(tempKey, 0, 32);

                    var hash = BitConverter.ToString(SHA1.Create().ComputeHash(tempKey)).Replace("-", "");
                    if (hash == validHash)
                    {
                        key = tempKey;
                        break;
                    }
                }
            }

            fs.Close();

            return key;
        }
    }
}