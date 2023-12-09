using Dynamic_Hash.Tests;
using QuadTree.GeoSystem;
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
        GeoApp geoApp;
        bool listProperties;
        public ToStringMain( GeoApp geoapp, bool properties)
        {
            //dynTest = dTest;
            this.geoApp = geoapp;
            listProperties = properties;
            InitializeComponent();
        }

        private void ToStringMain_Load(object sender, EventArgs e)
        {
            panelOverflowFile.Hide();
            panel_mainFile.Hide();
        }

        private void endToString_button_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void mainfile_button_Click(object sender, EventArgs e)
        {
            panelOverflowFile.Hide();
            panel_mainFile.Show();
            if (listProperties)
            {
                mainTextBox.Text = geoApp.getContentProperties(true);
            }
            else
            {
                mainTextBox.Text = geoApp.getContentLands(true);
            }
            
        }

        private void oveflowfile_button_Click(object sender, EventArgs e)
        {
            panel_mainFile.Hide();
            panelOverflowFile.Show();

            if (listProperties)
            {
                overflowTextBox.Text = geoApp.getContentProperties(false);
            }
            else
            {
                overflowTextBox.Text = geoApp.getContentLands(false);
            }
            
        }
    }
}
