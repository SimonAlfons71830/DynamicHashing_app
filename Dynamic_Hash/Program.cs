using Dynamic_Hash.Hashing;
using Dynamic_Hash.Objects;
using System.Diagnostics;
namespace Dynamic_Hash
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            /*ApplicationConfiguration.Initialize();
            Application.Run(new Form1());*/
            Trace.WriteLine("Starting this program.");
            var list = new List<int>(); 
            list.Add(1);
            var prop = new Property(0,"Prop",((12.3,1.2),(3.5,4.8)),list);
            var prop2 = new Property(2, "NewHeavenLands", ((5, 9), (10, 30)), list);

            var block = new Block<Property>(2);
            block.Insert(prop);
            block.Insert(prop2);

            byte[] arr = block.toByteArray();

            var block2 = new Block<Property>(2);
            block2.fromByteArray(arr);
        }
    }
}