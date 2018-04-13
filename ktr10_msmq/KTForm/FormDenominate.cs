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
    public partial class FormDenominate : Form
    {
        public FormDenominate()
        {
            InitializeComponent();
        }

        public string Amount { get; set; }
        public string Currency { get; set; }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Amount = tbAmount.Text;
            this.Currency = tbCurrency.Text; //example
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
