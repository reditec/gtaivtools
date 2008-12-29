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

using System.Windows.Forms;
using System.Windows.Media.Media3D;
using RageLib.Models.Model3DViewer;
using RageLib.Textures;
using UserControl=System.Windows.Forms.UserControl;

namespace RageLib.Models
{
    public partial class ModelView : UserControl
    {
        private IModelFile _modelFile;
        private TextureFile _textureFile;

        public ModelView()
        {
            InitializeComponent();

            _textureFile = null;
            _model3DView.RenderMode = RenderMode.Solid;
        }

        public IModelFile ModelFile
        {
            get { return _modelFile;  }
            set
            {
                _modelFile = value;
                UpdateView();
            }
        }

        public TextureFile TextureFile
        {
            get { return _textureFile; }
            set { _textureFile = value; }
        }

        private void UpdateView()
        {
            var model = _modelFile.GetModel(_textureFile);
            
            var group = model as Model3DGroup;
            
            var node = tvNav.Nodes.Add("Model");

            node.Tag = group;
            AddModelGroup(group, node);
        }

        private void AddModelGroup(Model3DGroup group, TreeNode node)
        {
            int index = 1;
            foreach (var child in group.Children)
            {
                TreeNode newNode = node.Nodes.Add("Geometry " + index);
                newNode.Tag = child;

                if (child is Model3DGroup)
                {
                    AddModelGroup(child as Model3DGroup, newNode);
                }

                index++;
            }
        }

        private void tvNav_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var model = e.Node.Tag as Model3D;
            _model3DView.Model = model;
        }

        private void tsbSolid_Click(object sender, System.EventArgs e)
        {
            if (tsbSolid.Checked)
            {
                _model3DView.RenderMode = RenderMode.Solid;
            }
            tsbWireframe.Checked = !tsbSolid.Checked;
        }

        private void tsbWireframe_Click(object sender, System.EventArgs e)
        {
            if (tsbWireframe.Checked)
            {
                _model3DView.RenderMode = RenderMode.Wireframe;
            }
            tsbSolid.Checked = !tsbWireframe.Checked;
        }

    }
}
