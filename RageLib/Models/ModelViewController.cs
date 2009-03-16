/**********************************************************************\

 RageLib - Models
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
using RageLib.Textures;

namespace RageLib.Models
{
    public class ModelViewController
    {
        private readonly ModelView _view;
        private IModelFile _modelFile;
        private TextureFile _textureFile;
        private string _workingDirectory;

        public ModelViewController(ModelView view)
        {
            _view = view;

            _view.NavigationModelSelected += View_NavigationModelSelected;
            _view.ExportClicked += View_ExportClicked;
            _view.Disposed += View_Disposed;
        }

        private void View_NavigationModelSelected(object sender, TreeViewEventArgs e)
        {
            _view.DisplayModel = _view.SelectedNavigationModel;
            _view.ExportEnabled = _view.SelectedNavigationModel.DataModel is Data.Drawable;
        }

        public TextureFile TextureFile
        {
            get { return _textureFile; }
            set { _textureFile = value; }
        }

        public IModelFile ModelFile
        {
            get { return _modelFile; }
            set
            {
                _modelFile = value;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            if (_modelFile != null)
            {
                _view.NavigationModel = _modelFile.GetModel(_textureFile);
            }
            else
            {
                _view.NavigationModel = null;
                _view.DisplayModel = null;
            }
        }

        private void View_ExportClicked(object sender, EventArgs e)
        {
            var model = _view.SelectedNavigationModel.DataModel as Data.Drawable;
            if (model != null)
            {
                var sfd = new SaveFileDialog
                {
                    AddExtension = true,
                    OverwritePrompt = true,
                    Title = "Export Model",
                    Filter = Export.ExportFactory.GenerateFilterString(),
                    InitialDirectory = _workingDirectory,
                };

                if (sfd.ShowDialog() == DialogResult.OK && sfd.FilterIndex > 0)
                {
                    Export.IExporter exporter = Export.ExportFactory.GetExporter(sfd.FilterIndex - 1);
                    exporter.Export( model, sfd.FileName );

                    _workingDirectory = new FileInfo(sfd.FileName).Directory.FullName;
                }
            }
        }

        private void View_Disposed(object sender, EventArgs e)
        {
            if (TextureFile != null)
            {
                TextureFile.Dispose();
                TextureFile = null;
            }
            
            if (ModelFile != null)
            {
                ModelFile.Dispose();
                ModelFile = null;
            }
        }
    }
}
