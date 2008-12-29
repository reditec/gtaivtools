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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using RageLib.FileSystem;
using RageLib.FileSystem.Common;
using SparkIV.Viewer;
using Directory=RageLib.FileSystem.Common.Directory;
using File=RageLib.FileSystem.Common.File;
using IODirectory = System.IO.Directory;
using IOFile = System.IO.File;

namespace SparkIV
{
    public partial class MainForm : Form
    {
        private const int SizeColumn = 1;

        private FileSystem _fs;
        private int _sortColumn = -1;

        private string _lastOpenPath;
        private string _lastImportExportPath;

        private Directory _selectedDir;

        public MainForm()
        {
            InitializeComponent();

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            tslAbout.Text = "Spark IV " + ver.Major + "." + ver.Minor + "." + ver.Build + "\n" +
                            "(C)2008, Aru";
        }

        #region Helpers

        private string FriendlySize(int size)
        {
            if (size < 1024)
            {
                return size + " B";
            }
            else if (size < 1024 * 1024)
            {
                return size / (1024) + " KB";
            } 
            else
            {
                return size / (1024 * 1024) + " MB";
            }
        }

        private void PopulateListView()
        {
            Directory dir = _selectedDir;

            string filterString = tstFilterBox.Text;

            List<string> selectedFileNames = new List<string>();
            foreach (var o in lvFiles.SelectedItems)
            {
                selectedFileNames.Add((o as ListViewItem).Text);
            }

            var comparer = lvFiles.ListViewItemSorter;
            lvFiles.ListViewItemSorter = null;

            lvFiles.BeginUpdate();
            
            lvFiles.Items.Clear();

            using (new WaitCursor(this))
            {
                foreach (var item in dir)
                {
                    if (!item.IsDirectory)
                    {
                        File file = item as File;

                        if (filterString == "" || file.Name.IndexOf(filterString) > -1)
                        {

                            ListViewItem lvi = lvFiles.Items.Add(file.Name);
                            lvi.Tag = file;

                            lvi.SubItems.Add(FriendlySize(file.Size));

                            /*
                            string compressed = file.IsCompressed ? "Yes (" + FriendlySize(file.CompressedSize) + ")" : "No";
                            lvi.SubItems.Add(compressed);
                             */

                            string resources = file.IsResource ? "Yes" : "No";
                            if (file.IsResource)
                            {
                                string rscType = Enum.IsDefined(file.ResourceType.GetType(), file.ResourceType)
                                                     ?
                                                         file.ResourceType.ToString()
                                                     : string.Format("Unknown 0x{0:x}", (int) file.ResourceType);
                                resources += " (" + rscType + ")";
                            }
                            lvi.SubItems.Add(resources);

                            if (file.IsCustomData)
                            {
                                lvi.ForeColor = SystemColors.HotTrack;
                            }

                            if (selectedFileNames.Contains(file.Name))
                            {
                                lvi.Selected = true;
                            }

                        }
                    }
                }
            }

            lvFiles.EndUpdate();

            lvFiles.ListViewItemSorter = comparer;
            lvFiles.Sort();
        }

        private void CreateDirectoryNode(TreeNode node, Directory dir)
        {
            node.Tag = dir;

            foreach (var item in dir)
            {
                if (item.IsDirectory)
                {
                    Directory subdir = item as Directory;
                    TreeNode subnode = node.Nodes.Add(subdir.Name);
                    CreateDirectoryNode(subnode, subdir);
                }
            }
        }

        private void PopulateUI()
        {
            _sortColumn = -1;
            lvFiles.ListViewItemSorter = null;

            splitContainer.Panel1Collapsed = (_fs is IMGFileSystem);


            tvDir.Nodes.Clear();
            
            TreeNode root = tvDir.Nodes.Add(_fs.RootDirectory.Name);
            CreateDirectoryNode(root, _fs.RootDirectory);
            
            root.ExpandAll();
            root.EnsureVisible();

            tvDir.SelectedNode = root;
        }

        private File FindFileByName(string name)
        {
            foreach (var fsObject in _selectedDir)
            {
                File file = fsObject as File;
                if (file != null)
                {
                    if (file.Name.ToLower() == name.ToLower())
                    {
                        return file;
                    }
                }
            }
            return null;
        }

        private void ExtractToPath(Directory dir, string path)
        {
            foreach (var item in dir)
            {
                if (item.IsDirectory)
                {
                    try
                    {
                        IODirectory.CreateDirectory(path + item.Name);
                        ExtractToPath(item as Directory, path + item.Name + "\\");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    File file = item as File;
                    byte[] data = file.GetData();
                    IOFile.WriteAllBytes( Path.Combine(path, file.Name), data );
                }
            }
        }

        private void PreviewFile(File file)
        {
            string fileName = file.Name;
            string extension = fileName.Substring(fileName.LastIndexOf('.') + 1);

            if (_fs is RealFileSystem && (extension == "img" || extension == "rpf"))
            {
                var form = new MainForm();
                form.Show();

                DirectoryInfo parent = new DirectoryInfo(Program.GTAPath).Parent;
                string archiveFilename = parent == null ? file.FullName : Path.Combine(parent.FullName, file.FullName);
                form.OpenFile(archiveFilename , null);
            }
            else
            {
                Control viewerControl = Viewers.Get(file);
                if (viewerControl != null)
                {
                    var form = new ViewerForm();
                    form.SetFilename(fileName);
                    form.SetControl(viewerControl);
                    form.ShowDialog();
                }
            }

        }

        #endregion

        #region Toolbar Handlers

        private void tsbBrowseGame_Click(object sender, EventArgs e)
        {
            FileSystem fs = new RealFileSystem();

            using (new WaitCursor(this))
            {
                fs.Open(Program.GTAPath);

                if (_fs != null)
                {
                    _fs.Close();
                }
                _fs = fs;

                Text = Application.ProductName + " - Browse Game Directory";
            }

            PopulateUI();
        }

        private void tsbOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open IV Archive";
            ofd.Filter = "All Supported IV Archives|*.rpf;*.img|RPF Files (*.rpf)|*.rpf|IMG Files (*.img)|*.img";
            ofd.FileName = _lastOpenPath;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _lastOpenPath = ofd.FileName;

                FileSystem fs = null;

                if (ofd.FilterIndex == 2)
                {
                    fs = new RPFFileSystem();
                }
                else if (ofd.FilterIndex == 3)
                {
                    fs = new IMGFileSystem();
                }
                else
                {
                    if (ofd.FileName.EndsWith(".rpf"))
                    {
                        fs = new RPFFileSystem();
                    }
                    else if (ofd.FileName.EndsWith(".img"))
                    {
                        fs = new IMGFileSystem();
                    }
                    else
                    {
                        MessageBox.Show("Please select a type for the file you are trying to open.", "Open IV Archive", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

                OpenFile(ofd.FileName, fs);
            }

        }

        private void OpenFile(string filename, FileSystem fs)
        {
            if (fs == null)
            {
                if (filename.EndsWith(".rpf"))
                {
                    fs = new RPFFileSystem();
                }
                else if (filename.EndsWith(".img"))
                {
                    fs = new IMGFileSystem();
                }
            }

            if (fs != null)
            {
                try
                {
                    using (new WaitCursor(this))
                    {
                        fs.Open(filename);

                        if (_fs != null)
                        {
                            _fs.Close();
                        }
                        _fs = fs;

                        Text = Application.ProductName + " - " + new FileInfo(filename).Name;
                    }

                    PopulateUI();
                }
                catch (Exception ex)
                {
                    fs.Close();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            if (_fs == null) return;

            try
            {
                using (new WaitCursor(this))
                {
                    _fs.Save();
                }

                PopulateListView();

                MessageBox.Show("The archive has been saved.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Could not save the archive.", 
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbExportSelected_Click(object sender, EventArgs e)
        {
            if (_fs == null) return;

            if (lvFiles.SelectedItems.Count == 1)
            {
                File file = lvFiles.SelectedItems[0].Tag as File;

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Export...";

                if (_lastImportExportPath != null)
                {
                    sfd.InitialDirectory = _lastImportExportPath;
                    sfd.FileName = Path.Combine(_lastImportExportPath, file.Name);
                }
                else
                {
                    sfd.FileName = file.Name;
                }


                sfd.OverwritePrompt = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _lastImportExportPath = IODirectory.GetParent(sfd.FileName).FullName;

                    using (new WaitCursor(this))
                    {
                        byte[] data = file.GetData();
                        IOFile.WriteAllBytes(sfd.FileName, data);
                    }
                }
            }
            else if (lvFiles.SelectedItems.Count > 1)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Export Selected...";
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = _lastImportExportPath;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _lastImportExportPath = fbd.SelectedPath;

                    string path = fbd.SelectedPath;

                    using (new WaitCursor(this))
                    {
                        foreach (ListViewItem item in lvFiles.SelectedItems)
                        {
                            File file = item.Tag as File;
                            byte[] data = file.GetData();
                            IOFile.WriteAllBytes(Path.Combine(path, file.Name), data);
                        }
                    }

                    MessageBox.Show("All selected files exported.", "Export Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void tsbExportAll_Click(object sender, EventArgs e)
        {
            if (_fs == null) return;
            
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Export All...";
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = _lastImportExportPath;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _lastImportExportPath = fbd.SelectedPath;

                string path = fbd.SelectedPath;
                if (!path.EndsWith("\\")) path += "\\";

                using (new WaitCursor(this))
                {
                    ExtractToPath(_fs.RootDirectory, path);
                }

                MessageBox.Show("All files in archive exported.", "Export All", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tsbImport_Click(object sender, EventArgs e)
        {
            if (_fs == null) return;

            var ofd = new OpenFileDialog();
            ofd.Title = "Import...";

            if (_lastImportExportPath != null)
            {
                ofd.InitialDirectory = _lastImportExportPath;
            }

            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _lastImportExportPath = IODirectory.GetParent(ofd.FileName).FullName;

                List<string> _invalidFiles = new List<string>();
                using (new WaitCursor(this))
                {
                    for (var i = 0; i < ofd.FileNames.Length; i++)
                    {
                        var safename = Path.GetFileName(ofd.FileNames[i]);
                        File file = FindFileByName(safename);
                        if (file == null)
                        {
                            _invalidFiles.Add(safename);
                        }
                        else
                        {
                            byte[] data = IOFile.ReadAllBytes(ofd.FileNames[i]);
                            file.SetData(data);
                        }
                    }
                }

                if (_invalidFiles.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (var s in _invalidFiles)
                    {
                        sb.Append("  " + s + "\n");
                    }
                    MessageBox.Show("The following files were not found in the archive to be replaced:\n\n" + sb +
                                    "\nPlease note that you can not add new files, only replace existing ones. The files must be named exactly " +
                                    "as they are in the archive.", "Import", MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                }

                PopulateListView();
            }
        }

        private void tsbRebuild_Click(object sender, EventArgs e)
        {
            if (_fs == null) return;

            try
            {
                using (new WaitCursor(this))
                {
                    _fs.Rebuild();
                }

                PopulateListView();

                MessageBox.Show("The archive has been rebuilt.", "Rebuild", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Could not rebuild the archive.\n\n" +
                                "Note that only IMG files can be rebuilt at the moment, rebuilding RPF files is not currently supported.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void tstFilterBox_TextChanged(object sender, EventArgs e)
        {
            if (_fs == null) return;

            PopulateListView();
        }

        #endregion

        #region Event Handlers

        private void tvDir_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Directory dir = (e.Node.Tag as Directory);
            _selectedDir = dir;
            PopulateListView();
        }

        private void lvFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitTest = lvFiles.HitTest(e.X, e.Y);
            if (hitTest.Item != null)
            {
                var file = hitTest.Item.Tag as File;

                PreviewFile(file);
            }
        }

        private void lvFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (lvFiles.SelectedItems.Count > 0)
                {
                    var file = lvFiles.SelectedItems[0].Tag as File;
                    PreviewFile(file);
                }
            }
        }

        private void lvFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != _sortColumn)
            {
                _sortColumn = e.Column;
                lvFiles.Sorting = SortOrder.Ascending;
            }
            else
            {
                lvFiles.Sorting = lvFiles.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }

            if (e.Column != SizeColumn)
            {
                lvFiles.ListViewItemSorter = new ListViewItemComparer(e.Column, lvFiles.Sorting == SortOrder.Descending);
            }
            else
            {
                lvFiles.ListViewItemSorter = new ListViewItemComparer(lvFiles.Sorting == SortOrder.Descending);
            }
            
            lvFiles.Sort();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_fs != null)
            {
                _fs.Close();
            }
        }

        #endregion
    }
}
