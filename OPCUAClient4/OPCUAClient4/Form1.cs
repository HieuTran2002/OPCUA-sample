using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace OPCUAClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Create new client
            mServer = new OPCUAClient();

        }

        #region public 
        public ApplicationInstance application;
        public ApplicationConfiguration config;
        public TreeNode root;

        public delegate void SelectionChangedEventHandler(TreeNode selectedNode);
        public event SelectionChangedEventHandler selectionChanged = null;
        #endregion

        #region private
        private OPCUAClient mServer = null;
        private Subscription mSubscription;
        private TreeNode m_CurrentNode;

        //private bool Connected = false;
        //private string url;
        //private string username;
        //private string password;
        #endregion

        #region connect/disconnect
        /// <summary>
        /// Connect to opc ua server
        /// </summary>
        public async Task<bool> Connect(string url, bool useSec, string username, string password)
        {
            try
            {
                await mServer.Connect(url, "none", MessageSecurityMode.None, useSec, username, password);
                if (mServer.Session != null && mServer.Session.Connected)
                    {
                    //    toolStripStatusLabel1.Text = "Connected";
                    //    LoadTreeView.Visible = true;
                    return true;
                }
                return false;

            }
            catch (Exception e)
            {
                toolStripStatusLabel1.Text = e.Message;
                return false;
            }
        } 
        
        private void Disconnect()
        {
            try
            {
                mServer.Disconnect();
                toolStripStatusLabel1.Text = "Disconnected";
                treeView1.Nodes.Clear();
                listView1.Items.Clear();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion

        #region Browse
        private void AddNamespaceToTreeview()
        {
            //mServer.AddNodes(root);
            //clear treeview first
            treeView1.Nodes.Clear();

            //add root folder into treeview
            root = new TreeNode() { Text = "ROOT", ImageIndex = 0 , SelectedImageIndex = 0};
            treeView1.Nodes.Add(root);

            //Then load it
            ReferenceDescriptionCollection rootCollection =  mServer.BrowseObjectFolders();
            foreach (var rd1 in rootCollection)
            {

                TreeNode childNode1 = new TreeNode() { Text = rd1.DisplayName.ToString() };
                root.Nodes.Add(childNode1);
                mServer.BrowseNodes(rd1, childNode1);

            }
            treeView1.Sort();
        }

        /// <summary>
        /// call browse service of opc ua client
        /// </summary>
        /// <param name="parentNode">Node want to expand</param>
        private void Browse(TreeNode parentNode)
        {
            treeView1.BeginUpdate();
            NodeId nodeToBrowse;
            TreeNodeCollection nodeCollection;

            if (parentNode == null)
            {

                nodeToBrowse = new NodeId(ExpandedNodeId.ToNodeId(Objects.RootFolder, null));
                nodeCollection = treeView1.Nodes;
            }
            else
            {
                parentNode.Nodes.Clear();

                ReferenceDescription parentReference = (ReferenceDescription)parentNode.Tag;
                nodeToBrowse = (NodeId)parentReference.NodeId;
                nodeCollection = parentNode.Nodes;
            }
            ReferenceDescriptionCollection browseResult;
            ReferenceDescription reference = new ReferenceDescription();

            reference.NodeId = nodeToBrowse;
            

            browseResult = mServer.browseNodes(reference);

            foreach (ReferenceDescription rd in browseResult)
            {
                if (rd.ReferenceTypeId != ReferenceTypeIds.HasNotifier)
                {
                    
                    int idx = GetNodeClaseIdx(rd);
                    TreeNode node = new TreeNode();
                    node.Text = rd.DisplayName.Text.ToString();
                    node.Tag = rd;
                    node.ImageIndex = idx;
                    node.SelectedImageIndex = idx;


                    
                    TreeNode fake = new TreeNode("nothing");
                    node.Nodes.Add(fake);
                    nodeCollection.Add(node);

                }
            }
            treeView1.EndUpdate();
            toolStripStatusLabel1.Text = $"Browse on node {reference.NodeId} successed";
        }

        /// <summary>
        /// Return correct image index for each node type.
        /// </summary>
        /// <param name="rd">Reference decription of node</param>
        /// <returns></returns>
        public int GetNodeClaseIdx(ReferenceDescription rd)
        {
            int idx = 0;
            switch (rd.NodeClass.ToString())
            {
                case "Object":
                    idx = 0;
                    break;
                case "Variable":
                    idx = 1;
                    break;
                case "Method":
                    idx = 2;
                    break;
                case "View":
                    idx = 3;
                    break;
                case "ObjectType":
                    idx = 4;
                    break;
                case "VariableType":
                    idx = 5;
                    break;
                case "ReferenceType":
                    idx = 6;
                    break;
                case "DataType":
                    idx = 7;
                    break;

            }
            return idx;
        }
        
        #endregion

        #region method

        #endregion

        #region UI event handle

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (((TreeNode)e.Item).Tag.GetType() == typeof(ReferenceDescription))
            {
                ReferenceDescription reference = (ReferenceDescription)((TreeNode)e.Item).Tag;

                if (reference.NodeId.IsAbsolute || reference.NodeClass != NodeClass.Variable)
                {
                    return;
                }
                //string NodeID = reference.NodeId.ToString();
                
                DragDropEffects dde = treeView1.DoDragDrop(reference, DragDropEffects.Copy);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            onSelectionChanged(node);
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Get item at events position.
                TreeNode node = treeView1.GetNodeAt(e.X, e.Y);

                // Select this node in the tree view.
                treeView1.SelectedNode = node;

                // Check if node is valid.
                if (node != null)
                {
                    // Check if node is a method.
                    ReferenceDescription refDescr = node.Tag as ReferenceDescription;

                    if (refDescr != null)
                    {
                        if (refDescr.NodeClass == NodeClass.Variable)
                        {
                            m_CurrentNode = node;
                        }
                        else if (refDescr.NodeClass == NodeClass.Object)
                        {
                            m_CurrentNode = node;
                        }
                        else
                        {
                            m_CurrentNode = node;
                        }
                    }
                }
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)

        {
            try
            {
                //handle data from darg/drop 
                ReferenceDescription reference = (ReferenceDescription)e.Data.GetData(typeof(ReferenceDescription));

                string nodeID = reference.NodeId.ToString();
                if (listView1.Items.Count == 0 || !listView1.Items.ContainsKey(nodeID))
                {

                    //get datatype
                    string typeid = mServer.readDataType(nodeID);

                    //subcribe to server if it have done yet
                    if (mSubscription == null)
                    {

                        mSubscription = mServer.subscribe(200);
                        mServer.itemChangedNotification += new MonitoredItemNotificationEventHandler(Client_ValueChanged);
                    }

                    //create new listviewitem
                    ListViewItem item = new ListViewItem(nodeID);
                    item.Name = reference.NodeId.ToString();

                    object serverHandle = null;

                    //add subitem
                    item.SubItems.Add("100");
                    item.SubItems.Add(String.Empty);
                    item.SubItems.Add(String.Empty);
                    item.SubItems.Add(String.Empty);
                    item.SubItems.Add(String.Empty);
                    item.SubItems.Add(String.Empty);

                    try
                    {
                        MonitoredItem tempMonitoredItem = mServer.addMonitoredItem(mSubscription, nodeID, "item1", 100);
                        tempMonitoredItem.Handle = item;
                        serverHandle = tempMonitoredItem;

                    }
                    catch (ServiceResultException monitoredItemResult)
                    {
                        //get Erorr status code
                        item.SubItems[5].Text = monitoredItemResult.StatusCode.ToString();
                    }

                    //add datatype
                    item.SubItems[6].Text = mServer.readNode(typeid).BrowseName.ToString();

                    item.Tag = serverHandle;

                    //MessageBox.Show(item.Name);
                    listView1.Items.Add(item);

                    // Fit column width to the longest item and add a few pixel:
                    listView1.Columns[0].Width = -1;
                    listView1.Columns[0].Width += 15;
                    // Fit column width to the column content:
                    listView1.Columns[1].Width = -2;
                    listView1.Columns[5].Width = -2;
                    // Fix settings:
                    listView1.Columns[2].Width = 95;
                    listView1.Columns[3].Width = 75;
                    listView1.Columns[4].Width = 75;

                } 
            }
            catch (Exception ex)
            {

                throw ex;
            }
            }

        private void listView1_DragOver(object sender, DragEventArgs e)
        {

            e.Effect = DragDropEffects.Copy;
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void writeValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem[] listViewItems;
            listViewItems = null;
            Array.Resize(ref listViewItems, this.listView1.SelectedItems.Count);
            int i = 0;

            foreach (ListViewItem selectedItem in this.listView1.SelectedItems)
            {

                string nodeID = selectedItem.SubItems[0].Text;
                string DataType = selectedItem.SubItems[6].Text;
                string currValue = selectedItem.SubItems[2].Text;
                ListViewItem item = new ListViewItem("");

                item.SubItems.Add(nodeID);
                item.SubItems.Add(DataType);
                item.SubItems.Add(currValue);

                listViewItems[i] = item;

                i++;
            }
            try
            {
                WriteValueDialog writeValueDialog = new WriteValueDialog();
                writeValueDialog.Show(mServer, listViewItems);
                toolStripStatusLabel1.Text = "Ready to write";

            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Error" + ex.Message; 
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectForm ConnectDialog = new ConnectForm();
            ConnectDialog.Show();
            bool connected = false;
            ConnectDialog.FormClosed += new FormClosedEventHandler(async (object Sender, FormClosedEventArgs E) =>
            {
                ConnectForm connFr = Sender as ConnectForm;
                toolStripStatusLabel1.Text = "Connnecting, pls wait !";
                
                if(connFr.DialogResult == DialogResult.OK)
                {
                    connected =  await Task.Run(() => Connect(connFr._url, connFr._useSec, connFr._userName, connFr._passWord));
                    if (connected)
                    {
                        Browse(null);
                    }
                }
            });     
        }
        
        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Disconnect();
            
            toolStripStatusLabel1.Text = "Disconnected from server";
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {

                if (this.listView1.SelectedItems.Count >= 0)
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        listView1.Items.Remove(item);
                        mServer.removeMonitoredItem(mSubscription, item.Tag as MonitoredItem);
                    }
                }

            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

            TreeNode node = e.Node;
            if (node != null)
            {
                Browse(node);
            }
        }

        #endregion

        #region EventHandling

        /// <summary>
        /// Update value when it changed
        /// </summary>
        /// <param name="monitoredItem"></param>
        /// <param name="e"></param>
        private void Client_ValueChanged(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MonitoredItemNotificationEventHandler(Client_ValueChanged), monitoredItem, e);
                return;
            }
            MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
            if (notification == null )
            {
                return;
            }
            object value = notification.Value.WrappedValue.Value;
            ListViewItem item = (ListViewItem)monitoredItem.Handle;

            item.SubItems[2].Text = Utils.Format("{0}", notification.Value.Value);
            item.SubItems[3].Text = Utils.Format("{0}", notification.Value.StatusCode);
            item.SubItems[4].Text = Utils.Format("{0:HH:mm:ss.fff}", notification.Value.SourceTimestamp.ToLocalTime());

        }

        public void onSelectionChanged(TreeNode node)
        {
            if (selectionChanged != null)
            {
                selectionChanged(node);
            }
        }

        #endregion

    }
}
   
