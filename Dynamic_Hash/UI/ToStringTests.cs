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
    public partial class ToStringTests : Form
    {
        DynamicHashingTests dynTest;
        public ToStringTests(DynamicHashingTests dyn_test)
        {
            dynTest = dyn_test;
            InitializeComponent();
        }

        private void mainfile_button_Click(object sender, EventArgs e)
        {
            dynTest.dynHash.GetString(true);
        }

        private void oveflowfile_button_Click(object sender, EventArgs e)
        {
            dynTest.dynHash.GetString(false);
        }

        private void endToString_button_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
