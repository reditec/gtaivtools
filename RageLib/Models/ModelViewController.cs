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
using RageLib.Textures;

namespace RageLib.Models
{
    public class ModelViewController
    {
        private readonly ModelView _view;
        private IModelFile _modelFile;
        private TextureFile _textureFile;

        public ModelViewController(ModelView view)
        {
            _view = view;

            _view.NavigationModelSelected += delegate { _view.DisplayModel = _view.SelectedNavigationModel; };
            _view.Disposed += View_Disposed;
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
