using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCUAClient
{
    public partial class WriteValueDialog : Form
    {

        public WriteValueDialog()
        {
            InitializeComponent();
        }

        #region private
        private OPCUAClient mServer;

        #endregion

        #region method

        public void Show(OPCUAClient Server, ListViewItem[] listViewItem)
        {
            if (Server == null)
            {
                throw new ArgumentNullException("Server");
            }

            mServer = Server;
            foreach (var item in listViewItem)
            {
                this.WriteDialog_ListView.Items.Add(item);
            }
            WriteDialog_ListView.Columns[0].Width = -2;
            WriteDialog_ListView.Columns[1].Width = -2;
            WriteDialog_ListView.Columns[2].Width = -2;
            WriteDialog_ListView.Columns[3].Width = -2;


            updateValues();
            Show();
            BringToFront();

        }
        public void updateValues()
        {
            try
            {
                List<string> nodeToReads = new List<string>(this.WriteDialog_ListView.Items.Count);
                List<string> readResult = new List<string>();

                foreach (ListViewItem item in this.WriteDialog_ListView.Items)
                {
                    nodeToReads.Add(item.SubItems[1].Text);
                }

                readResult = mServer.readValues(nodeToReads);
                int i = 0;
                foreach (ListViewItem item in this.WriteDialog_ListView.Items)
                {

                    item.SubItems[3].Text = readResult[i];
                    i++;

                }
            }
            catch (Exception e)
            {
                toolStripStatusLabel1.Text = "Update failed :" + e.Message;
            }
        }
        public void writeValues()
        {
            try
            {
                List<string> nodeToWrite = new List<string>();
                List<string> writeValues = new List<string>();
                List<string> dataTypes = new List<string>();


                foreach (ListViewItem item in this.WriteDialog_ListView.Items)
                {
                    string Values = item.SubItems[0].Text;
                    string datatype = item.SubItems[2].Text;

                    if (Values.Length == 0)
                    { 
                        continue;
                    }
                    writeValues.Add(Values);
                    nodeToWrite.Add(item.SubItems[1].Text);
                    dataTypes.Add(datatype);
                }
                //mServer.writeValues(writeValues, nodeToWrite);
                mServer.writeValues(writeValues, nodeToWrite, dataTypes);

                toolStripStatusLabel1.Text = "Write value succeeded";
            }
            catch (Exception e)
            {
                toolStripStatusLabel1.Text = "Error: " + e.Message;
            }

        }

        #endregion

        #region UI event handling
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            writeValues();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            writeValues();
            updateValues();
        }

        private void WriteDialog_ListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar  == (char)13)
            {
                writeValues();
                updateValues();
            }
        }

        #endregion
    }
}
