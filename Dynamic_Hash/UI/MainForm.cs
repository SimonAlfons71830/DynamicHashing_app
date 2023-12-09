using Dynamic_Hash.Tests;
using Dynamic_Hash.UI;
using QuadTree.GeoSystem;
using QuadTree.UI;
using System.Diagnostics.PerformanceData;
using System.Security.Policy;

namespace Dynamic_Hash
{
    public partial class MainForm : Form
    {
        DynamicHashingTests dynTest;
        GeoApp geoApp;
        public MainForm( GeoApp geoApp)
        {
            //dynTest = dynTestH;
            this.geoApp = geoApp;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void test_button_Click(object sender, EventArgs e)
        {
            dynTest.setNewBFOverflow((int)bfOverflow_no.Value);
            dynTest.setNewHashCount((int)hashCount_no.Value);
            dynTest.setNewBF((int)blockFactor_no.Value);
            dynTest.ResetTest();
            dynTest.podielInsert = (int)insert_no.Value;
            dynTest.podielFind = (int)find_no.Value;
            dynTest.podielRemove = (int)noRemove.Value;

            dynTest.TestInsertRemoveFind();

            richTextBox1.Text = "POCET VYKONANYCH OPERACII : " + dynTest.pocetVykonanychOperacii + "\n" +
            "   pocet operacii insert : \n\t\tpassed: " + dynTest.passedInsert + "\n\t\tfailed: " + dynTest.failedInsert + "\n" +
            "   pocet operacii find : \n\t\tpassed: " + dynTest.passedFind + "\n\t\tfailed: " + dynTest.failedFind + "\n" +
            "   pocet operacii remove : \n\t\tpassed: " + dynTest.passedRemove + "\n\t\tfailed: " + dynTest.failedRemove + "\n";

        }

        private void toStrintg_button_Click(object sender, EventArgs e)
        {
            var form = new ToStringMain(geoApp, true);
            this.Hide();
            form.ShowDialog();

        }

        private void appbutton_Click(object sender, EventArgs e)
        {
            var test = new App( geoApp);
            this.Hide();
            test.ShowDialog();
        }
    }
}