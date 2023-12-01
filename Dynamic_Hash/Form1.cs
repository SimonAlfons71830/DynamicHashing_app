using Dynamic_Hash.Tests;
using System.Diagnostics.PerformanceData;
using System.Security.Policy;

namespace Dynamic_Hash
{
    public partial class Form1 : Form
    {
        DynamicHashingTests dynTest;
        public Form1(DynamicHashingTests dynTestH)
        {
            dynTest = dynTestH;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void test_button_Click(object sender, EventArgs e)
        {
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
    }
}