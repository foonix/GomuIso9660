using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WinformReaderTest.UC
{
    public partial class TreeViewTable : TreeView
    {
        public TreeViewTable()
        {
            InitializeComponent();
        }

        public void FillTreeviewPaths(string[] paths,string separator)
        {
            string[] tmpNodes = null;
            TreeNode tn = null;
            TreeNodeCollection nodeCol = null;

            for (int i = 0; i < paths.Length; i++)
            {
                tmpNodes = paths[i].Split(separator.ToCharArray());

                nodeCol = this.Nodes;

                foreach (string n in tmpNodes)
                {
                    if (n.Length > 0)
                    {
                        tn = FindNode(n, nodeCol, true);
                        nodeCol = tn.Nodes;
                    }
                }
            }
        }

        private TreeNode FindNode(string node, TreeNodeCollection nodes, bool createNode)
        {
            bool bFound = false;
            TreeNode tnRet = new TreeNode();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Text == node)
                {
                    bFound = true;
                    tnRet = nodes[i];

                    break;
                }
            }

            if (!bFound)
            {
                if (createNode)
                {
                    tnRet = nodes.Add(node);
                }
                else
                {
                    tnRet = null;
                }
            }

            return tnRet;
        }
    }
}
