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
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using RageLib.Models.Model3DViewer;
using UserControl=System.Windows.Forms.UserControl;

namespace RageLib.Models
{
    public partial class ModelView : UserControl
    {
        public ModelView()
        {
            InitializeComponent();
            _model3DView.RenderMode = RenderMode.Solid;
        }

        public RenderMode RenderMode
        {
            get { return _model3DView.RenderMode;  }
            set
            {
                if (_model3DView.RenderMode != value)
                {
                    _model3DView.RenderMode = value;
                    tsbWireframe.Checked = value == RenderMode.Wireframe;
                    tsbSolid.Checked = value == RenderMode.Solid;
                }
            }
        }

        public event TreeViewEventHandler NavigationModelSelected
        {
            add { tvNav.AfterSelect += value; }
            remove { tvNav.AfterSelect -= value; }
        }

        public Model3D SelectedNavigationModel
        {
            get
            {
                return tvNav.SelectedNode.Tag as Model3D;
            }
        }

        public Model3D NavigationModel
        {
            set
            {
                UpdateTreeView(value);
            }
        }

        public Model3D DisplayModel
        {
            set
            {
                _model3DView.Model = value;
            }
        }

        private void UpdateTreeView(Model3D model)
        {
            if (!tvNav.IsDisposed)
            {
                tvNav.Nodes.Clear();
            }

            if (model != null)
            {
                var node = tvNav.Nodes.Add("Model");

                node.Tag = model;

                if (model is Model3DGroup)
                {
                    AddModelGroup(model as Model3DGroup, node);
                }
            }
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

        private void tsbSolid_CheckedChanged(object sender, EventArgs e)
        {
            if (tsbSolid.Checked)
            {
                RenderMode = RenderMode.Solid;
            }
        }

        private void tsbWireframe_Click(object sender, EventArgs e)
        {
            if (tsbWireframe.Checked)
            {
                RenderMode = RenderMode.Wireframe;
            }
        }

    }
}
