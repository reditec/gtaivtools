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
using System.Reflection;
using IOFile = System.IO.File;
using System.Net;
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
                string message = "Your GTAIV.exe seems to be modified or is a newer version than this tool supports. " +
                                "SparkIV can not run without a supported GTAIV.exe file." + "\n" + "Would you like to check for updates?";
                string caption = "Newer or Modified GTAIV.exe";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    {
                        try
                        {
                            //Updater adapted heavily from http://digitalformula.net/technical/c-self-updating-application-without-clickonce/
                            System.Net.WebClient client = new System.Net.WebClient();
                            client.DownloadFile("http://www.gzwn.net/sparkiv/version.txt", "version.txt");
                        }
                        catch (Exception ex)
                        {
                            string errorDetails10 = String.Empty;
                            MessageBoxIcon iconsToShow10 = MessageBoxIcon.Information;
                            if (ex.Message.Contains("could not be resolved"))
                            {
                                errorDetails10 = String.Format("The update check server could not be resolved.\nPlease check your internet connection and try again.");
                                iconsToShow10 = MessageBoxIcon.Error;
                            }
                            else if (ex.Message.Contains("404"))
                            {
                                errorDetails10 = "The update check server is currently down. Please try again later.";
                                iconsToShow10 = MessageBoxIcon.Information;
                            }
                            DialogResult result10 = MessageBox.Show(String.Format("{0}", errorDetails10), "Update check error", MessageBoxButtons.OK, iconsToShow10);
                            return;
                        }
                        TextReader tr = new StreamReader("version.txt");
                        string tempStr = tr.ReadLine();
                        tr.Close();
                        if (tempStr == null)
                        {
                            string message3 = "An error has occurred. Please manually check the Google Code project page for updates.";
                            string caption3 = "Error";
                            MessageBoxButtons buttons3 = MessageBoxButtons.YesNo;
                            DialogResult result3;

                            result3 = MessageBox.Show(message3, caption3, buttons3, MessageBoxIcon.Error);

                            if (result3 == System.Windows.Forms.DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start("http://code.google.com/p/gtaivtools/downloads/list");
                                return;
                            }

                            if (result3 == System.Windows.Forms.DialogResult.No)
                            {
                                return;
                            }
                        }
                        string longVersionFromFile = tempStr;
                        string shortVersionFromFile = tempStr.Replace(".", String.Empty);
                        Version vrs = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                        string longVersionFromVrs = String.Format("{0}.{1}.{2}", vrs.Major, vrs.Minor, vrs.Build);
                        string shortVersionFromVrs = String.Format("{0}{1}{2}", vrs.Major, vrs.Minor, vrs.Build);
                        if (Convert.ToInt32(shortVersionFromVrs) < Convert.ToInt32(shortVersionFromFile))
                        {
                            string message4 = "There is a new version of SparkIV available! Would you like to go to the Google Project page to download the newest version?" + "\n" + "\n" + "This version is:  " + vrs.Major + "." + vrs.Minor + "." + vrs.Build + "\n"
                                + "New Version is: " + longVersionFromFile;
                            string caption4 = "New Update!";
                            MessageBoxButtons buttons4 = MessageBoxButtons.YesNo;
                            DialogResult result4;

                            result4 = MessageBox.Show(message4, caption4, buttons4, MessageBoxIcon.Information);

                            if (result4 == System.Windows.Forms.DialogResult.Yes)
                            {
                                try
                                {
                                    System.Net.WebClient client = new System.Net.WebClient();
                                    client.DownloadFile("http://www.gzwn.net/sparkiv/updateurl.txt", "updateurl.txt");
                                }
                                catch (Exception ex)
                                {
                                    string errorDetails8 = String.Empty;
                                    MessageBoxIcon iconsToShow8 = MessageBoxIcon.Information;
                                    if (ex.Message.Contains("could not be resolved"))
                                    {
                                        errorDetails8 = String.Format("The update check server could not be resolved.\nPlease check your internet connection and try again.");
                                        iconsToShow8 = MessageBoxIcon.Error;
                                    }
                                    else if (ex.Message.Contains("404"))
                                    {
                                        errorDetails8 = "The update check server is currently down. Please try again later.";
                                        iconsToShow8 = MessageBoxIcon.Information;
                                    }
                                    DialogResult result8 = MessageBox.Show(String.Format("{0}", errorDetails8), "Update check server down", MessageBoxButtons.OK, iconsToShow8);
                                    return;
                                }
                                TextReader tr1 = new StreamReader("updateurl.txt");
                                string updateURL = tr1.ReadLine();
                                tr.Close();
                                if (updateURL == null)
                                {
                                    string message7 = "An error has occurred. Would you like to check the Google Code project page for updates.";
                                    string caption7 = "Error";
                                    MessageBoxButtons buttons7 = MessageBoxButtons.YesNo;
                                    DialogResult result7;

                                    result7 = MessageBox.Show(message7, caption7, buttons7, MessageBoxIcon.Error);

                                    if (result7 == System.Windows.Forms.DialogResult.Yes)
                                    {
                                        System.Diagnostics.Process.Start("http://code.google.com/p/gtaivtools/downloads/list");
                                        Application.Exit();
                                    }

                                    if (result7 == System.Windows.Forms.DialogResult.No)
                                    {
                                        Application.Exit();
                                    }
                                }
                                System.Diagnostics.Process.Start(updateURL);
                                Application.Exit();
                            }

                        if (result4 == System.Windows.Forms.DialogResult.No)
                        {
                             Application.Exit();
                        }
                      }
                        else
                        {
                            DialogResult result5 = MessageBox.Show(String.Format("There is no update available at this time."), "No update available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    return;
                }

                if (result == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

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

