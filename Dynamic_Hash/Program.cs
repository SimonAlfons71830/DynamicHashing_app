using Dynamic_Hash.Hashing;
using Dynamic_Hash.Objects;
using Dynamic_Hash.Tests;
using System.Collections.Generic;
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

            /*var block = new Block<Property>(5);
            var list = new List<int>();
            list.Add(1);
            var prop = new Property(0, "Prop", ((12.3, 1.2), (3.5, 4.8)), list);
            var prop2 = new Property(2, "NewHeavenLands", ((5, 9), (10, 30)), list);
            var prop3 = new Property(3, "OldCrook", ((2, 2), (1, 1)), list);
            block.Insert(prop);
            block.Insert(prop2);
            block.OfIndexBefore = 0;
            block.OfindexNext = 418;

            byte[] arr = block.toByteArray();

            var block2 = new Block<Property>(5);
            block2.fromByteArray(arr);

            Trace.WriteLine(block2.ToString());*/

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            /*ApplicationConfiguration.Initialize();
            Application.Run(new Form1());*/
            /*Trace.WriteLine("Starting this program.");
            var list = new List<int>(); 
            list.Add(1);
            var prop = new Property(0,"Prop",((12.3,1.2),(3.5,4.8)),list);
            var prop2 = new Property(2, "NewHeavenLands", ((5, 9), (10, 30)), list);
            var prop3 = new Property(3, "OldCrook", ((2, 2), (1, 1)), list);
            var prop4 = new Property(4, "SeaShean",((6,6),(4,4)),list);
            var prop5 = new Property(5, "MoreOver", ((50, 50), (40, 40)), list);

            DynamicHashing<Property> dynamicHashing = new DynamicHashing<Property>("hashing", 2);
            dynamicHashing.Insert(prop);
            dynamicHashing.Insert(prop2);
            dynamicHashing.Insert(prop3);
            dynamicHashing.Insert(prop4);
            dynamicHashing.Insert(prop5);

            int passed = 0;

            if (prop.MyEquals(dynamicHashing.Find(prop)))
            {
                passed++;
            }

            if (prop2.MyEquals(dynamicHashing.Find(prop2)))
            {
                passed++;
            }
            if (prop3.MyEquals(dynamicHashing.Find(prop3)))
            {
                passed++;
            }
            if (prop4.MyEquals(dynamicHashing.Find(prop4)))
            {
                passed++;
            }
            if (prop5.MyEquals(dynamicHashing.Find(prop5)))
            {
                passed++;
            }*/
            /*var block = new Block<Property>(2);
            block.Insert(prop);
            block.Insert(prop2);

            byte[] arr = block.toByteArray();

            var block2 = new Block<Property>(2);
            block2.fromByteArray(arr);

            Trace.WriteLine(block2.ToString());*/

            /*DynamicHashingTests test = new DynamicHashingTests();
            test.podielInsert = 10;
            test.podielFind = 10;
            test.TestInsertRemoveFind();

            Trace.WriteLine("Test Insert:" + test.passedInsert + "/" + test.pocetInsert);
            Trace.WriteLine("Test Find:" + test.passedFind+ "/" + test.pocetFind);
*/

            DynamicHashingTests dynTest = new DynamicHashingTests();
            Application.Run(new Form1(dynTest));

            
        }
    }
}