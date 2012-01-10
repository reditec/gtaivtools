using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace SparkIV
{
    public class Updater
    {
        private const string VersionUrl = "http://gtaivtools.googlecode.com/svn/updates/version.txt";
        private const string UpdateUrl = "http://gtaivtools.googlecode.com/svn/updates/url.txt";
        private const string DownloadListUrl = "http://code.google.com/p/gtaivtools/downloads/list";

        public static void CheckForUpdate()
        {
            string version = GetWebString(VersionUrl);

            if ( string.IsNullOrEmpty(version))
            {
                DialogResult result =
                    MessageBox.Show(
                        "An error has occurred. Please manually check the Google Code project page for updates.",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    Process.Start(DownloadListUrl);
                }
            }
            else
            {
                var versionSplit = version.Split(new[] {'.'}, 3);
                int versionCode = 0;
                foreach (var s in versionSplit)
                {
                    versionCode *= 0x100;
                    versionCode += int.Parse(s);
                }

                Version vrs = Assembly.GetExecutingAssembly().GetName().Version;
                int assemblyVersionCode = (vrs.Major * 0x100 + vrs.Minor) * 0x100 + vrs.Build;
                
                if (versionCode > assemblyVersionCode)
                {
                    string message =
                        "There is a new version of SparkIV available! Would you like to download the newest version?" +
                        "\n" + "\n" + "This version is:  " + vrs.Major + "." + vrs.Minor + "." + vrs.Build + "\n"
                        + "New Version is: " + version;

                    DialogResult result = MessageBox.Show(message, "New Update!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        var url = GetWebString(UpdateUrl);

                        if ( string.IsNullOrEmpty(url) )
                        {
                            result =
                                MessageBox.Show(
                                    "An error has occurred. Would you like to check the Google Code project page for updates?",
                                    "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                            if (result == DialogResult.Yes)
                            {
                                Process.Start(DownloadListUrl);
                            }
                        }
                        else
                        {
                            Process.Start( url );
                            Application.Exit();                            
                        }
                    }
                }
                else
                {
                    MessageBox.Show(String.Format("There is no update available at this time."),
                                    "No update available", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }

        private static string GetWebString(string url)
        {
            string result;
            try
            {
                var client = new System.Net.WebClient();
                result = client.DownloadString(url);
            }
            catch (Exception ex)
            {
                string errorDetails = String.Empty;
                MessageBoxIcon iconsToShow = MessageBoxIcon.Information;

                if (ex.Message.Contains("could not be resolved"))
                {
                    errorDetails =
                        String.Format(
                            "The update check server could not be resolved.\nPlease check your internet connection and try again.");
                    iconsToShow = MessageBoxIcon.Error;
                }
                else if (ex.Message.Contains("404"))
                {
                    errorDetails = "The update check server is currently down. Please try again later.";
                    iconsToShow = MessageBoxIcon.Information;
                }
                MessageBox.Show(errorDetails, "Update check server down", MessageBoxButtons.OK, iconsToShow);
                return null;
            }

            return result;
        }
    }
}