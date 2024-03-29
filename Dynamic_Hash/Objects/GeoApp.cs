﻿using Dynamic_Hash.Hashing;
using Dynamic_Hash.Objects;
using QuadTree.QTree;
using QuadTree.Structures;
using QuadTree.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace QuadTree.GeoSystem
{
    public class GeoApp
    {
        public MyQuadTree _area;
        private Random _random = new Random();
        private DynamicHashing<Property> hashProperties;
        private DynamicHashing<PlotOfLand> hashLands;
        public bool import = false;
        public int newId;

        public GeoApp(MyQuadTree area) 
        {
            hashProperties = new DynamicHashing<Property>("Properties","OFProperties", 2,2,2);
            hashLands = new DynamicHashing<PlotOfLand>("Lands","OFLands", 2,2,2);
            _area = area;
            //seed
        }

        public void AddProperty(int registerNumber, string description, ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinates, bool import)
        {
            var list = this.FindInterval(coordinates);
            var listForProp = list.OfType<PlotToQuad>().ToList();

            var listOfRegNumbersLands = new List<int>();

            //find each land in file and edit its record list to add register number of property if possible
            foreach (var item in listForProp)
            {
                //najst item vo file  - pridat mu do listu tuto property, zapisat ho naspat
                var land = new PlotOfLand();
                land.RegisterNumber = item.RegisterNumber;
                (Block<PlotOfLand> block, int i, int index, int file) edit =  hashLands.AddDataToRecords(land);

                if (edit.block.ValidRecordsCount < 5)
                {
                    var plot = edit.block.Records[edit.i];
                    for (int i = 0; i < plot.Properties.Count; i++)
                    {
                        if (plot.Properties[i] == -1)
                        {
                            plot.Properties[i] = registerNumber;
                            //if it is here something changed in the block -> need to write it to file
                            hashLands.WriteBackToFile(edit.index, edit.block, edit.file == 1 ? true : false);
                            break;
                        }
                    }
                }
            }


            foreach (var item in listForProp) 
            {
                if (listOfRegNumbersLands.Count == 6)
                {
                    break;
                }
                //pre nehnutelnost evidujem max 6 parciel (registracnych cisel)
                listOfRegNumbersLands.Add(item.RegisterNumber);
                
            }


            var prop = new Property(registerNumber, description, coordinates, listOfRegNumbersLands);

            this._area.Insert(new PropToQuad(registerNumber,coordinates));

            if (!import)
            {
                this.hashProperties.Insert(prop);
            }
        }


        public void AddPlot(int registerNumber, string description, ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinates, bool import) 
        {
            var list = this.FindInterval(coordinates);
            var listForProp = list.OfType<PropToQuad>().ToList();

            var listOfRegNumbersProperties = new List<int>();

            if (!import)
            {
                //find each land in file and edit its record list to add register number of property if possible
                foreach (var item in listForProp)
                {
                    //najst item vo file  - pridat mu do listu tento land, zapisat ho naspat
                    var property = new Property();
                    property.RegisterNumber = item.RegisterNumber;
                    (Block<Property> block, int i, int index, int file) edit = hashProperties.AddDataToRecords(property);

                    if (edit.block.ValidRecordsCount < 5)
                    {
                        var prop = edit.block.Records[edit.i];
                        for (int i = 0; i < prop.Lands.Count; i++)
                        {
                            if (prop.Lands[i] == -1)
                            {
                                prop.Lands[i] = registerNumber;
                                //if it is here something changed in the block -> need to write it to file
                                hashProperties.WriteBackToFile(edit.index, edit.block, edit.file == 1 ? true : false);
                                break;
                            }
                        }

                    }

                }


                foreach (var item in listForProp)
                {

                    if (listOfRegNumbersProperties.Count == 5)
                    {
                        break;
                    }
                    //pre nehnutelnost evidujem max 6 parciel (registracnych cisel)
                    listOfRegNumbersProperties.Add(item.RegisterNumber);
                }
            }
            var plot = new PlotOfLand(registerNumber, description, coordinates, listOfRegNumbersProperties);

            this._area.Insert(new PlotToQuad(registerNumber,coordinates));
            if (!import)
            {
                this.hashLands.Insert(plot);
            }
        }

        public bool RemoveObj(Polygon obj) 
        {

            //need to find object to remove it from list
            //same as editing and deleting 
            //find objects on same gps
            //if equals -> delete it from references in lists

            //ak sa podari vymazat obj ak je to prop tak najdem vsetky ploty ktore sa nachadzaju v ramci 

            if (obj is PropToQuad)
            {
                //sear for plots
                //var list = FindInterval((new Coordinates(((Property)obj)._borders.startP._x, ((Property)obj)._borders.startP._y, 0), new Coordinates(((Property)obj)._borders.endP._x, ((Property)obj)._borders.endP._y, 0)));
                var list = FindInterval(((obj._borders.Item1.LongitudeStart, obj._borders.Item1.LatitudeStart), (obj._borders.Item2.LongitudeEnd, obj._borders.Item2.LatitudeEnd)));

                var potentialObjList = list.OfType<PropToQuad>().ToList();
                PropToQuad obj_to_rem = null;

                foreach (var potObj in potentialObjList)
                {
                    if (((PropToQuad)potObj).Equals(((PropToQuad)obj)))
                    {
                        return _area.RemoveObject(obj);
                        
                    }
                }
                return false;




            }
            else
            {
                //search for properties v ramci plotu
                //var list = FindInterval((new Coordinates(((PlotOfLand)obj)._coordinates.startPos._x, ((PlotOfLand)obj)._coordinates.startPos._y, 0), new Coordinates(((PlotOfLand)obj)._coordinates.endPos._x, ((PlotOfLand)obj)._coordinates.endPos._y, 0)));
                var list = FindInterval(((obj._borders.Item1.LongitudeStart, obj._borders.Item1.LatitudeStart), (obj._borders.Item2.LongitudeEnd, obj._borders.Item2.LatitudeEnd)));


                var potentialPlotList = list.OfType<PlotToQuad>().ToList();
                PlotToQuad plot_to_rem = null;

                foreach (var potPlot in potentialPlotList)
                {
                    if (((PlotToQuad)potPlot).Equals(((PlotToQuad)obj)))
                    {
                        return _area.RemoveObject(obj);
                        
                    }
                }
                return false;

            }
            
        }

        public Property findProperty(int registerNumber) 
        {
            return hashProperties.Find(new Property(registerNumber, "", ((0, 0), (0, 0)),null));
        }
        public PlotOfLand findLand(int registerNumber)
        {
            return hashLands.Find(new PlotOfLand(registerNumber, "", ((0, 0), (0, 0)), null));
        }
        public bool RemoveProp(int registerNumber) 
        {
            var listOfLands = findProperty((int)registerNumber).Lands;

            for (int i = 0; i < listOfLands.Count; i++)
            {
                //find land according to id 
                if (listOfLands[i] != -1)
                {
                    
                    var land = findLand(listOfLands[i]); 
                    (Block<PlotOfLand> block, int i,int index,int file) edit =  hashLands.AddDataToRecords(land);
                    for (int j = 0; j < edit.block.Records.Count; j++)
                    {
                        for (int k = 0; k < edit.block.Records[j].Properties.Count; k++)
                        {
                            if (edit.block.Records[j].Properties[k] == registerNumber)
                            {
                                edit.block.Records[j].Properties[k] = -1;
                                hashLands.WriteBackToFile(edit.index, edit.block, edit.file == 1 ? true : false);
                                break;
                            }
                        }
                        
                    }
                }
            }

            return hashProperties.RemoveNew(new Property(registerNumber, "", ((0, 0), (0, 0)), null));

        }
        public bool RemoveLand(int registerNumber)
        {
            var listOfProperties = findLand((int)registerNumber).Properties;

            for (int i = 0; i < listOfProperties.Count; i++)
            {
                //find land according to id 
                if (listOfProperties[i] != -1)
                {

                    var property = findProperty(listOfProperties[i]);
                    (Block<Property> block, int i, int index, int file) edit = hashProperties.AddDataToRecords(property);
                    for (int j = 0; j < edit.block.Records.Count; j++)
                    {
                        for (int k = 0; k < edit.block.Records[j].Lands.Count; k++)
                        {
                            if (edit.block.Records[j].Lands[k] == registerNumber)
                            {
                                edit.block.Records[j].Lands[k] = -1;
                                hashProperties.WriteBackToFile(edit.index, edit.block, edit.file == 1 ? true : false);
                                break;
                            }
                        }
                       
                    }
                }
            }

            return hashLands.RemoveNew(new PlotOfLand(registerNumber, "", ((0, 0), (0, 0)), null));
        }


        public List<Polygon> FindInterval(((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinates) 
        {
            var listOfObj = _area.IntervalSearch(new Boundaries(coordinates.Item1.LongitudeStart,coordinates.Item1.LatitudeStart,
                coordinates.Item2.LongitudeEnd, coordinates.Item2.LatitudeEnd) , true);

            var returnList = listOfObj.OfType<Polygon>().ToList();

            return returnList;
        }


        /*public List<Polygon> FindOBJInterval(((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinates, bool interfere, bool properties)
        {
            var foundObjects = new List<ISpatialObject>();

            foundObjects = this._area.IntervalSearch(new Boundaries(coordinates.Item1.LongitudeStart , coordinates.Item1.LatitudeStart, coordinates.Item2.LongitudeEnd, coordinates.Item2.LatitudeEnd), interfere);

            if (properties)
            {
                var listOfProperties = foundObjects.OfType<Property>().Cast<Polygon>().ToList();
                return listOfProperties;
            }
            else
            {
                var listOfPlots = foundObjects.OfType<PlotOfLand>().Cast<Polygon>().ToList();
                return listOfPlots;
            }
        }*/

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


        public void seedApp(double startX, double startY, double endX, double endY, int numberOfProp, int numberOfPlot, int max_quad_cap, int max_depth) 
        {
            //when seeding the app i want to remove everything, 
            //from quad tree, from files, from hashing files,...
            _area.ResetTree(_area._root);
            //reset hashing ???

            this.Reset();

            //10-1000 N -> (>0)
            //30 - 15000 E -> (>0)

            _area._dimension.X0 = startX;
            _area._dimension.Y0 = startY;
            _area._dimension.Xk = endX;
            _area._dimension.Yk = endY;

            _area.MAX_QUAD_CAPACITY = max_quad_cap;
            _area._maxDepth = max_depth;

            var listofPropertyNames = this.LoadLandNames("PropNames.txt");
            var listofPlotNames = this.LoadLandNames("PlotNames.txt");

            for (int i = 0; i < numberOfProp; i++)
            {
                double X0 = (int)_area._dimension.X0;
                double Xk = (int)_area._dimension.Xk;

                double Y0 = (int)_area._dimension.Y0;
                double Yk = (int)_area._dimension.Yk;

               //pri generovani property su to tie iste suradnice 
                double startPosX = _random.NextDouble() * (Xk - X0) + X0;
                double startPosY = _random.NextDouble() * (Yk - Y0) + Y0;

                //new gen
                double endPosX = startPosX + 1;
                double endPosY = startPosY + 1;


                string desc = listofPropertyNames.ElementAt(_random.Next(listofPropertyNames.Count - 1));
                //this.AddProperty(i, desc, (new Coordinates(startPosX, startPosY, 0), new Coordinates(startPosX, startPosY, 0)));
                this.AddProperty(i, desc,((startPosX,startPosY),(endPosX,endPosY)),false);
                this.newId++;
            }

            for (int i = 0; i < numberOfPlot; i++)
            {
                //generovanie rozmeru
                var rozmer = 0.0;

                if ((this._area._dimension.Xk - this._area._dimension.X0) > (this._area._dimension.Yk - this._area._dimension.Y0))
                {
                    rozmer = _random.NextDouble() * ((this._area._dimension.Yk - this._area._dimension.Y0) / 3.0);
                }
                else
                {
                    rozmer = _random.NextDouble() * ((this._area._dimension.Xk - this._area._dimension.X0) / 3.0);
                }

                //pri generovani plotOfLand -> zadavam rozmer a suradnice sa prepocitaju
                rozmer = 25;

                var startPosGen = new MyPoint(
                    _random.NextDouble() * (this._area._dimension.Xk - rozmer) + this._area._dimension.X0,
                    _random.NextDouble() * (this._area._dimension.Yk - rozmer) + this._area._dimension.Y0,
                    _random.Next(1000000));

                var endPosGen = new MyPoint(
                    startPosGen._x + rozmer,
                    startPosGen._y + rozmer,
                    _random.Next(100000));

                string desc = listofPlotNames.ElementAt(_random.Next(listofPlotNames.Count - 1));

                this.AddPlot(numberOfProp + i, desc, ((startPosGen._x, startPosGen._y), (endPosGen._x, endPosGen._y)),false);
                this.newId++;
            }
        }

        public void ChangeKeyAttr(Polygon refObj, Polygon newObj) 
        {
            if (refObj is Property && newObj is Property)
            {
                var pomInQuad = new PropToQuad(((Property)refObj).RegisterNumber, ((Property)refObj).Coordinates);

                var regN = ((Property)newObj).RegisterNumber;
                var desc = ((Property)newObj).Description;
                if (this.RemoveObj(pomInQuad)) 
                {
                    var list = this.FindInterval(((Property)refObj).Coordinates);
                    
                    var listToChange = list.OfType<PlotToQuad>().ToList();

                    var listToChangeInts = hashProperties.Find((Property)refObj).Lands;
                    hashProperties.RemoveNew((Property)refObj);
                    //from each list record in list remove register number
                    foreach (var land in listToChange) 
                    {
                        var pomLand = new PlotOfLand();
                        pomLand.RegisterNumber = land.RegisterNumber;
                        //var hashLand = hashLands.Find((PlotOfLand)land);
                       (Block<PlotOfLand> block, int i, int address, int file) edit = hashLands.AddDataToRecords(pomLand);
                        var landFromBlock = edit.block.Records[edit.i];
                        for (int i = 0; i < landFromBlock.Properties.Count; i++)
                        {
                            if (landFromBlock.Properties[i] == ((Property)refObj).RegisterNumber)
                            {
                                landFromBlock.Properties[i] = -1;
                                hashLands.WriteBackToFile(edit.address, edit.block, edit.file == 1 ? true : false);
                                break;
                            }
                        } 

                    }

                    this.AddProperty(regN,desc,((Property)newObj).Coordinates,false);
                }
                //((Property)refObj).Coordinates = (new Coordinates(x0, y0, 0), new Coordinates(xk, yk, 0));
            }
            else //is PLOT
            {
                var pomInQuad = new PlotToQuad(((PlotOfLand)refObj).RegisterNumber, ((PlotOfLand)refObj).Coordinates);

                var regN = ((PlotOfLand)newObj).RegisterNumber;
                var desc = ((PlotOfLand)newObj).Description;
                if (this.RemoveObj(pomInQuad))//_area.RemoveObject(refObj)
                {
                    var list = this.FindInterval(((PlotOfLand)refObj)._borders);
                    var listToChange = list.OfType<PropToQuad>().ToList();

                    var listToChangeInts = hashLands.Find((PlotOfLand)refObj).Properties;
                    hashLands.RemoveNew((PlotOfLand)refObj);
                    //from each list record in list remove register number
                    foreach (var property in listToChange)
                    {
                        var pomProp = new Property();
                        pomProp.RegisterNumber = property.RegisterNumber;
                        //var hashLand = hashLands.Find((PlotOfLand)land);
                        (Block<Property> block, int i, int address, int file) edit = hashProperties.AddDataToRecords(pomProp);
                        var propFromBlock = edit.block.Records[edit.i];
                        for (int i = 0; i < propFromBlock.Lands.Count; i++)
                        {
                            if (propFromBlock.Lands[i] == ((PlotOfLand)refObj).RegisterNumber)
                            {
                                propFromBlock.Lands[i] = -1;
                                hashProperties.WriteBackToFile(edit.address, edit.block, edit.file == 1 ? true : false);
                                break;
                            }
                        }

                    }

                    this.AddPlot(regN, desc, ((PlotOfLand)newObj).Coordinates, false);
                }
                //((PlotOfLand)refObj).Coordinates = (new Coordinates(x0, y0, 0), new Coordinates(xk, yk, 0));
            }

        }

        public void ChangeNonKeyAttr(Polygon refObj, int regNumber, string description) 
        {
            if (refObj is Property)
            {
                ((Property)refObj).RegisterNumber = regNumber;
                ((Property)refObj).Description = description;
                (Block<Property> Block, int i, int address, int file) edit = hashProperties.AddDataToRecords((Property)refObj);

                edit.Block.Records[edit.i].Description = description;

                hashProperties.WriteBackToFile(edit.address, edit.Block, edit.file == 1 ? true : false);

            }
            else
            {
                ((PlotOfLand)refObj).RegisterNumber = regNumber;
                ((PlotOfLand)refObj).Description = description;

                (Block<PlotOfLand> Block, int i, int address, int file) edit = hashLands.AddDataToRecords((PlotOfLand)refObj);

                edit.Block.Records[edit.i].Description = description;

                hashLands.WriteBackToFile(edit.address, edit.Block, edit.file == 1 ? true : false);
            }
        }

        public bool EditObject(Polygon oldObj, Polygon newObj, bool keyAttr) 
        {
            //returns the reference to a object in structure
            //var _refObj = this._area.ShowObject(oldObj);
            if (oldObj is Property)
            {
                if (keyAttr)
                {
                    this.ChangeKeyAttr(((Property)oldObj), ((Property)newObj));
                }
                else
                {
                    this.ChangeNonKeyAttr(((Property)oldObj),
                           ((Property)newObj).RegisterNumber, ((Property)newObj).Description);
                }
                return true;
            }
            else
            {
                if (keyAttr)
                {
                    this.ChangeKeyAttr(((PlotOfLand)oldObj), (PlotOfLand)newObj);
                }
                else 
                { 
                    this.ChangeNonKeyAttr(((PlotOfLand)oldObj),
                            ((PlotOfLand)newObj).RegisterNumber, ((PlotOfLand)newObj).Description);
                }
                return true;
            }
            
/*

            if (_refObj != null)
            {
                //nasiel sa
                if (keyAttr)
                {
                    if (newObj is PlotOfLand)
                    {
                        this.ChangeKeyAttr(((PlotOfLand)_refObj), (PlotOfLand)newObj);



                    }
                    else
                    {
                        this.ChangeKeyAttr(((Property)_refObj), ((Property)newObj));
                    }
                    
                    return true;
                }
                else
                {
                    if (newObj is PlotOfLand)
                    {
                        this.ChangeNonKeyAttr(((PlotOfLand)_refObj),
                            ((PlotOfLand)newObj).RegisterNumber, ((PlotOfLand)newObj).Description);
                    }
                    else
                    {
                        this.ChangeNonKeyAttr(((Property)_refObj),
                            ((Property)newObj).RegisterNumber, ((Property)newObj).Description);
                    }
                    return true;
                }
            }
            else
            {
                return false;
            }*/




        }
        

        public Polygon PickToEdit(Polygon obj)
        {
            //return (Polygon)this._area.ShowObject(obj);
            return this.hashLands.Find((PlotOfLand)obj);
        }

        public Property PickAttrProp(Property obj)
        {
            /*Property _refProp = (Property)this._area.ShowObject(obj);

            //just attributes for form
            Property pomProp = new Property(_refProp.RegisterNumber, _refProp.Description, _refProp.Coordinates, lands : null);

            return pomProp;*/
            return this.hashProperties.Find(obj);
        }

        public PlotOfLand PickAttrPlot(PlotOfLand obj)
        {
            PlotOfLand _refPlot = (PlotOfLand)this._area.ShowObject(obj);

            //just attributes for form
            PlotOfLand pomPlot = new PlotOfLand(_refPlot.RegisterNumber, _refPlot.Description, _refPlot.Coordinates, properties : null);

            return pomPlot;
        }

        public void SaveData(string path) 
        {
            hashProperties.SaveData("TrieProp.txt","DataProp.txt",newId, path);
            hashLands.SaveData("TrieLands.txt", "DataLands.txt", newId, path);

        }

        public void LoadData(string landsTrieFile, string dataLandsFile, string propTrieFile, string dataPropFile) 
        {
            this.Reset();

            this.newId = hashLands.LoadData(landsTrieFile, dataLandsFile);
            hashProperties.LoadData(propTrieFile, dataPropFile);

        }

        public void WriteToFiles(string pathDirectory)
        {
            StreamWriter writerProp = null;
            StreamWriter writerPlots = null;
            try
            {
                writerProp = new StreamWriter(pathDirectory + "\\Properties.txt");
                writerPlots = new StreamWriter(pathDirectory + "\\Plots.txt");


                //var AllObj = this.FindInterval((new Coordinates(this._area._dimension.X0, this._area._dimension.Y0, 0), new Coordinates(this._area._dimension.Xk, this._area._dimension.Yk, 0)));
                var AllObj = this.FindInterval(((this._area._dimension.X0, this._area._dimension.Y0), (this._area._dimension.Xk, this._area._dimension.Yk)));


                foreach (var obj in AllObj)
                {
                    if (obj is PropToQuad)
                    {
                        //LongHem = (Longitude >= 0) ? 'E' : 'W';
                        //LatHem = (Latitude >= 0) ? 'N' : 'S';

                        writerProp.WriteLine(obj.GetType().Name + ";" + ((PropToQuad)obj).RegisterNumber + ";" +
                            ((PropToQuad)obj).Coordinates.Item1.LongitudeStart + ";" +
                            ((PropToQuad)obj).Coordinates.Item1.LatitudeStart + ";" +
                            ((PropToQuad)obj).Coordinates.Item2.LongitudeEnd + ";" +
                            ((PropToQuad)obj).Coordinates.Item2.LatitudeEnd );
                    }
                    else
                    {
                        writerPlots.WriteLine(obj.GetType().Name + ";" + ((PlotToQuad)obj).RegisterNumber + ";" +
                            ((PlotToQuad)obj).Coordinates.Item1.LongitudeStart + ";" +
                            ((PlotToQuad)obj).Coordinates.Item1.LatitudeStart + ";" +
                            ((PlotToQuad)obj).Coordinates.Item2.LongitudeEnd + ";" +
                            ((PlotToQuad)obj).Coordinates.Item2.LatitudeEnd );
                    }
                }

                writerProp.Close();
                writerPlots.Close();
                
                //ask for all quads
                //Queue<Quad> AllQuads = this._area.GetQuadsAtDepth(0);
                
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                writerProp.Close();
            }
        }

        public void ReadProperties(String pathProp)
        {

            StreamReader reader = new StreamReader(pathProp);
            StringBuilder builder = new StringBuilder();
            try
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("PropToQuad"))
                    {
                        // Split the line by semicolon to extract data
                        string[] parts = line.Split(';');

                        if (parts.Length >= 6)
                        {
                            //Property;3;79E;491N;description
                            // Parse the relevant data from the line
                            int id = int.Parse(parts[1]);
                            //double coordX = double.Parse(parts[2]);

                            string rawCoordX = parts[2];
                            string numericPartX = new string(rawCoordX.Reverse().SkipWhile(char.IsLetter).Reverse().ToArray());
                            double coordX = double.Parse(numericPartX);

                            //double coordY = double.Parse(parts[3]);
                            string rawCoordY = parts[3];
                            string numericPartY = new string(rawCoordY.Reverse().SkipWhile(char.IsLetter).Reverse().ToArray());
                            double coordY = double.Parse(numericPartY);

                            string rawCoordXE = parts[4];
                            string numericPartXE = new string(rawCoordXE.Reverse().SkipWhile(char.IsLetter).Reverse().ToArray());
                            double coordXE = double.Parse(numericPartXE);

                            //double coordY = double.Parse(parts[3]);
                            string rawCoordYE = parts[5];
                            string numericPartYE = new string(rawCoordYE.Reverse().SkipWhile(char.IsLetter).Reverse().ToArray());
                            double coordYE = double.Parse(numericPartYE);

                            

                            AddProperty(id,"",((coordX,coordY),(coordXE,coordYE)),true);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        public void ReadPlots(String pathPlot)
        {

            StreamReader reader = new StreamReader(pathPlot);
            StringBuilder builder = new StringBuilder();
            try
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("PlotToQuad"))
                    {
                        // Split the line by semicolon to extract data
                        string[] parts = line.Split(';');

                        if (parts.Length >= 6)
                        {
                            //PlotOfLand;25;113E;261N;202E;350N;description
                            int id = int.Parse(parts[1]);

                            string rawCoordStartX = parts[2];
                            string numericPartStartX = new string(rawCoordStartX.Reverse().SkipWhile(char.IsLetter).Reverse().ToArray());
                            double coordStartX = double.Parse(numericPartStartX);

                            string rawCoordStartY = parts[3];
                            string numericPartStartY = new string(rawCoordStartY.Reverse().SkipWhile(char.IsLetter).Reverse().ToArray());
                            double coordStartY = double.Parse(numericPartStartY);

                            string rawCoordEndX = parts[4];
                            string numericPartEndX = new string(rawCoordEndX.Reverse().SkipWhile(char.IsLetter).Reverse().ToArray());
                            double coordEndX = double.Parse(numericPartEndX);

                            string rawCoordEndY = parts[5];
                            string numericPartEndY = new string(rawCoordEndY.Reverse().SkipWhile(char.IsLetter).Reverse().ToArray());
                            double coordEndY = double.Parse(numericPartEndY);


                            AddPlot(id, " ", ((coordStartX,coordStartY),(coordEndX,coordEndY)), true);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        public void ChangeDepth(int newDepth) 
        {
            this._area.SetNewDepth(newDepth);
        }

        public int getDepthOfStruct()
        {
            return this._area.maxDepth;
        }


        public void Reset() 
        {
            this._area.ResetTree(this._area._root);
            //this.hashLands = new DynamicHashing<PlotOfLand>();
            
        }

        public void DataInsert(string fileNameLands, string OFfileNameLands, string fileNameProp, string OffileNameProp, int BF, int bfOf, int countHashFun) 
        {
            this.hashLands = new DynamicHashing<PlotOfLand>(fileNameLands,OFfileNameLands,BF,bfOf,countHashFun);
            this.hashProperties = new DynamicHashing<Property>(fileNameProp, OffileNameProp, BF, bfOf, countHashFun);
        
        }
        public void setBFinMainFile(int blockFactor) 
        {
            hashLands.BlockFactor = blockFactor;
            hashProperties.BlockFactor = blockFactor;
        }

        public void setBFinOFFile(int blockFactor)
        {
            hashLands.BlockSizeOF = blockFactor;
            hashProperties.BlockSizeOF = blockFactor;
        }

        public void setLengthOfHashFunc(int length)
        {

            hashLands.CountHashFun = length;
            hashProperties.CountHashFun = length;
        }

        public String getContentProperties(bool main)
        { 
            return this.hashProperties.GetString(main);
        }

        public String getContentLands(bool main)
        {
            return this.hashLands.GetString(main);
        }
    }
}
