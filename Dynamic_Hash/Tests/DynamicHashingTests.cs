using Dynamic_Hash.Hashing;
using Dynamic_Hash.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamic_Hash.Tests
{
    public class DynamicHashingTests
    {
        public int podielInsert;
        //public int podielRemove;
        public int podielFind;


        public int passedInsert;
        //public int passedRemove;
        public int passedFind;

        public int failedInsert;
        //public int failedRemove;
        public int failedFind;

        public int pocetVykonanychOperacii;
        public int pocetInsert;
        //public int pocetRemove;
        public int pocetFind;

        public DynamicHashing<Property> dynHash = new DynamicHashing<Property>("hashFile", 2);
        List<Property> availableObjects = new List<Property>();
        List<Property> usedKeys = new List<Property>();
        HashSet<int> uniqueNumbers = new HashSet<int>();
        public List<Property> failedObj = new List<Property>();

        public int passed = 0;
        public int failed = 0;

        private static Random random = new Random(0);

        public void TestInsertRemoveFind()
        {
            this.GenerateObjects(podielInsert + podielFind);
            for (int i = 0; i < podielFind; i++)
            {
                int insertIndex = random.Next(availableObjects.Count);
                Property insertData = availableObjects[insertIndex];

                dynHash.Insert(insertData);
                var data = dynHash.Find(insertData);
                if (data == null)
                {
                    var pom = "error";
                }

                usedKeys.Add(insertData);
                availableObjects.RemoveAt(insertIndex);
            }

            var pocetop = podielFind + podielInsert;
            for (int i = 0; i < pocetop; i++)
            {
                int cislo = random.Next(pocetop - i);

                if (cislo < podielInsert) //INSERT
                {
                    this.TestInsert();
                }
                else
                {
                    this.TestFind();
                }
            }
        }

        private void GenerateObjects(int pocetObjektov)
        {
            var listNames = this.LoadLandNames("PropNames.txt");
            for (int i = 0; i < pocetObjektov; i++)
            {
                var plots = new List<int>();
                for (int j = 0; j < random.Next(1, 6); j++)
                {
                    plots.Add(random.Next(10, 100)); //random int
                }

                double startPosX = random.NextDouble() * 100;
                double startPosY = random.NextDouble() * 100;
                string desc = listNames.ElementAt(random.Next(listNames.Count - 1));

                availableObjects.Add(new Property(i, desc, ((startPosX, startPosY), (startPosX + 1, startPosY + 1)), plots));
            }
        }

        public List<string> LoadLandNames(string filePath)
        {
            List<string> landNames = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        landNames.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return landNames;
        }

        public void TestInsert()
        {
            int oldSize = this.dynHash.noOfRecords;

            int insertIndex = random.Next(availableObjects.Count);
            Property insertData = availableObjects[insertIndex];

            dynHash.Insert(insertData);
            if (oldSize + 1 == this.dynHash.noOfRecords)
            {
                passedInsert++;
                passed++;
            }
            else
            {
                failedInsert++;
                failed++;
            }
            usedKeys.Add(insertData);
            availableObjects.RemoveAt(insertIndex);
            podielInsert--;
            pocetInsert++;
            pocetVykonanychOperacii++;
        }

        public void TestFind()
        {
            int tryFindIndex = random.Next(usedKeys.Count);
            Property tryFindKey = usedKeys[tryFindIndex];
            var pomNodeKey = tryFindKey;

            var data = dynHash.Find(tryFindKey);
            if (data == null)
            {
                failed++;
                //return; 
            }
            else
            {
                if (pomNodeKey.MyEquals(data))
                {
                    passedFind++;
                    passed++;
                }
                else
                {

                    failedFind++;
                    failed++;
                    failedObj.Add(tryFindKey);
                }
            }

            podielFind--;
            pocetVykonanychOperacii++;
            pocetFind++;
        }


        public void TestRemove()
        {

        }


        public void ResetTest()
        {
            //pocetOperacii = 0;
            podielInsert = 0;
            //podielRemove = 0;
            podielFind = 0;
            //seed = 0;

            passedInsert = 0;
            //passedRemove = 0;
            passedFind = 0;

            failedInsert = 0;
            //failedRemove = 0;
            failedFind = 0;

            pocetVykonanychOperacii = 0;
            pocetInsert = 0;
            //pocetRemove = 0;
            pocetFind = 0;

            this.availableObjects.Clear();
            this.usedKeys.Clear();
            this.uniqueNumbers.Clear();

            this.passed = 0;
            this.failed = 0;

            failedObj = new List<Property>();

            // Specify the path of the file you want to delete
            string filePath = "hashFile";

            // Check if the file exists before attempting to delete
            if (File.Exists(filePath))
            {
                // Delete the file
                File.Delete(filePath);
            }
        }

    }
}
