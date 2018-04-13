using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KTForm
{
    public partial class Form1 : Form
    {
        private KTForm.CashDispenseModule.MainUIForm _CDM;
        private KTForm.CashInModule.MainUIForm _CIM;

        public Form1()
        {
            InitializeComponent();
        }

        public bool CDMIsOpen = false;
        public bool CIMIsOpen = false;

        private void bOpenCDM_Click(object sender, EventArgs e)
        {
            if (!CDMIsOpen)
            {
                _CDM = new CashDispenseModule.MainUIForm(this);
                _CDM.Show();
                CDMIsOpen = true;
            }
        }

        private void bOpenCIM_Click(object sender, EventArgs e)
        {
            if (!CIMIsOpen)
            {
                _CIM = new CashInModule.MainUIForm(this);
                _CIM.Show();
                CIMIsOpen = true;
            }
        }

        private void bExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
