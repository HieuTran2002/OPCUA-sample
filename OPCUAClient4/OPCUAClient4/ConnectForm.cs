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
    public partial class ConnectForm : Form
    {

        #region initialize variables
        public static ConnectForm Instance;
        public ComboBox cb1;
        public CheckBox useSec;
        public MaskedTextBox userName;
        public MaskedTextBox passWord;

        #endregion 

        #region return value
        public string _url;
        public string _userName;
        public string _passWord;
        public bool _useSec;
        #endregion

        public ConnectForm()
        {
            InitializeComponent();
            Instance = this;
            ConnDisButton.DialogResult = DialogResult.OK;
            cb1 = comboBox1;
            useSec = useSecurity;
            userName = Username;
            passWord = Password;
        }

        #region ui event handling
        private void useSecurity_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void ConnDisButton_Click(object sender, EventArgs e)
        {
            try
            {
                this._passWord = Password.Text;
                this._userName = userName.Text;
                this._useSec = useSecurity.Checked;
                this._url = comboBox1.Text;
            }
            catch (Exception)
            {
            }
            this.Close();
        }
        #endregion
    }
}
