using Dynamic_Hash.Tests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dynamic_Hash.UI
{
    public partial class ToStringMain : Form
    {
        DynamicHashingTests dynTest;
        public ToStringMain(DynamicHashingTests dTest)
        {
            dynTest = dTest;
            InitializeComponent();
        }

        private void ToStringMain_Load(object sender, EventArgs e)
        {
            panelOverflowFile.Hide();
            panel_mainFile.Hide();
        }

        private void endToString_button_Click(object sender, EventArgs e)
        {
            var form = new MainForm(dynTest);
            this.Hide();
            form.ShowDialog();
        }

        private void mainfile_button_Click(object sender, EventArgs e)
        {
            panelOverflowFile.Hide();
            panel_mainFile.Show();
            mainTextBox.Text = dynTest.dynHash.GetString(true);
        }

        private void oveflowfile_button_Click(object sender, EventArgs e)
        {
            panel_mainFile.Hide();
            panelOverflowFile.Show();
            overflowTextBox.Text = dynTest.dynHash.GetString(false);
        }
    }
}
