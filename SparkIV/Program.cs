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

using System;
using System.IO;
using System.Windows.Forms;
using RageLib.Common;

namespace SparkIV
{
    static class Program
    {
        public static string GTAPath { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*
            Version envVer = Environment.Version;
            if (envVer.Major <= 2 && envVer.Revision < 1435) // 2.0 SP1 (http://en.wikipedia.org/wiki/.NET_Framework_version_list)
            {
                MessageBox.Show("It appears that you are not running the latest version of the .NET Framework.\n\n" +
                    "SparkIV requires that you atleast have both .NET Framework 2.0 SP1 and 3.0 installed. " + 
                    "Alternatively an install of .NET Framework 3.5 will install both these components.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
             */

            string gtaPath = KeyUtil.FindGTADirectory();
            while (gtaPath == null)
            {
                var fbd = new FolderBrowserDialog
                {
                    Description =
                        "Could not find the GTAIV game directory. Please select the directory containing GTAIV.exe",
                    ShowNewFolderButton = false
                };

                if (fbd.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show(
                        "GTAIV.exe is required to extract cryptographic keys for this program to function. " +
                        "SparkIV can not run without this file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (File.Exists(Path.Combine(fbd.SelectedPath, "gtaiv.exe")))
                {
                    gtaPath = fbd.SelectedPath;
                }
            }

            byte[] key = KeyUtil.FindKey(gtaPath);
            if (key == null)
            {
                MessageBox.Show("Your GTAIV.exe seems to be modified or is a newer version than this tool supports. " +
                                "If it is a newer version, please check for an update of SparkIV. SparkIV can not run " +
                                "without a supported GTAIV.exe file.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            KeyStore.SetKeyLoader(() => key);

            GTAPath = gtaPath;

            if (args.Length > 0)
            {
                MainForm form = new MainForm();
                form.OpenFile(args[0], null);
                Application.Run(form);
            }
            else
            {
                Application.Run(new MainForm());
            }
        }
    }
}

