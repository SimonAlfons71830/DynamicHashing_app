using QuadTree.QTree;
using QuadTree.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree.GeoSystem
{
    public class GeoApp
    {
        public MyQuadTree _area;
        private Random _random = new Random();
        public bool improvedWithReinsert = false;

        public GeoApp(MyQuadTree area) 
        {
            _area = area;
            //seed
        }

        public void AddProperty(int registerNumber, string description, (Coordinates startPos, Coordinates endPos) coordinates)
        {
            var list = this.FindInterval((coordinates.startPos, coordinates.endPos));
            var listForProp = list.OfType<PlotOfLand>().ToList();

            var prop = new Property(registerNumber, description, coordinates, listForProp);

            foreach(var item in listForProp) 
            {
                ((PlotOfLand)item)._properties.Add(prop);
            }

            this._area.Insert(prop);
        }

        public void AddPlot(int registerNumber, string description, (Coordinates startPos, Coordinates endPos) coordinates) 
        {
            var list = this.FindInterval((coordinates.startPos, coordinates.endPos));
            var listForProp = list.OfType<Property>().ToList();

            var plot = new PlotOfLand(registerNumber, description, coordinates, listForProp);

            foreach (var item in listForProp)
            {
                ((Property)item)._lands.Add(plot);
            }

            this._area.Insert(plot);
        }

        public bool RemoveObj(Polygon obj) 
        {

            //need to find object to remove it from list
            //same as editing and deleting 
            //find objects on same gps
            //if equals -> delete it from references in lists

            //ak sa podari vymazat obj ak je to prop tak najdem vsetky ploty ktore sa nachadzaju v ramci 

            if (obj is Property)
            {
                //sear for plots
                var list = FindInterval((new Coordinates(((Property)obj)._borders.startP._x, ((Property)obj)._borders.startP._y, 0), new Coordinates(((Property)obj)._borders.endP._x, ((Property)obj)._borders.endP._y, 0)));

                var potentialObjList = list.OfType<Property>().ToList();
                Property obj_to_rem = null;

                foreach (var potObj in potentialObjList)
                {
                    if (((Property)potObj).Equals(((Property)obj)))
                    {
                        obj_to_rem = potObj;
                        break;
                    }
                }

                if (obj_to_rem != null)
                {
                    //list of plots interfering with obj coordinates
                    var plotList = list.OfType<PlotOfLand>().ToList();
                    foreach (var item in plotList)
                    {
                        ((PlotOfLand)item)._properties.Remove((Property)obj_to_rem);
                    }
                    //now its possible to remove
                    return _area.RemoveObject(obj);


                }
            }
            else
            {
                //search for properties v ramci plotu
                var list = FindInterval((new Coordinates(((PlotOfLand)obj)._coordinates.startPos._x, ((PlotOfLand)obj)._coordinates.startPos._y, 0), new Coordinates(((PlotOfLand)obj)._coordinates.endPos._x, ((PlotOfLand)obj)._coordinates.endPos._y, 0)));

                var potentialPlotList = list.OfType<PlotOfLand>().ToList();
                PlotOfLand plot_to_rem = null;

                foreach (var potPlot in potentialPlotList)
                {
                    if (((PlotOfLand)potPlot).Equals(((PlotOfLand)obj)))
                    {
                        plot_to_rem = potPlot;
                        break;
                    }
                }


                if (plot_to_rem != null)
                {

                    //list of properties that plot needs to be removed
                    var propList = list.OfType<Property>().ToList();

                    foreach (var item in propList)
                    {
                        ((Property)item)._lands.Remove((PlotOfLand)plot_to_rem);
                    }

                    return _area.RemoveObject(obj);

                }
                else
                {
                    var debug = 0;
                }

            }
            return false;
            
        }

        public List<Polygon> FindInterval((Coordinates startPos, Coordinates endPos) coordinates) 
        {
            var listOfObj = _area.IntervalSearch(new Boundaries(coordinates.startPos.Longitude,coordinates.startPos.Latitude,
                coordinates.endPos.Longitude, coordinates.endPos.Latitude) , true);

            var returnList = listOfObj.OfType<Polygon>().ToList();

            return returnList;
        }

        public List<Polygon> FindOBJInterval((Coordinates startPos, Coordinates endPos) coordinates, bool interfere, bool properties)
        {
            var foundObjects = new List<ISpatialObject>();

            foundObjects = this._area.IntervalSearch(new Boundaries(coordinates.startPos.Longitude, coordinates.startPos.Latitude, coordinates.endPos.Longitude, coordinates.endPos.Latitude), interfere);

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


        public void seedApp(double startX, double startY, double endX, double endY, int numberOfProp, int numberOfPlot, int max_quad_cap, int max_depth) 
        {
            _area.ResetTree(_area._root);
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
                this.AddProperty(i, desc, (new Coordinates(startPosX, startPosY, 0), new Coordinates(endPosX, endPosY, 0)));

            }

            for (int i = 0; i < numberOfPlot; i++)
            {
                //generovanie rozmeru
                var rozmer = 0.0;

                if ((this._area._dimension.Xk - this._area._dimension.X0) > (this._area._dimension.Yk - this._area._dimension.Y0))
                {
                    rozmer = _random.NextDouble() * ((this._area._dimension.Yk - this._area._dimension.Y0) / 5.0);
                }
                else
                {
                    rozmer = _random.NextDouble() * ((this._area._dimension.Xk - this._area._dimension.X0) / 5.0);
                }

                //pri generovani plotOfLand -> zadavam rozmer a suradnice sa prepocitaju
                rozmer = 10;

                var startPosGen = new MyPoint(
                    _random.NextDouble() * (this._area._dimension.Xk - rozmer) + this._area._dimension.X0,
                    _random.NextDouble() * (this._area._dimension.Yk - rozmer) + this._area._dimension.Y0,
                    _random.Next(1000000));

                var endPosGen = new MyPoint(
                    startPosGen._x + rozmer,
                    startPosGen._y + rozmer,
                    _random.Next(100000));

                string desc = listofPlotNames.ElementAt(_random.Next(listofPlotNames.Count - 1));

                this.AddPlot(numberOfProp + i, desc, (new Coordinates(startPosGen._x, startPosGen._y, 0), new Coordinates(endPosGen._x, endPosGen._y, 0)));
            }


            if (this.getHealtOfStruct() < 62 && this._area.wantOptimizing)
            {
                this.OptimizeStruct();
            }
        }

        public void ChangeKeyAttr(Polygon refObj, Polygon newObj) 
        {
            if (refObj is Property && newObj is Property)
            {
                var regN = ((Property)newObj)._registerNumber;
                var desc = ((Property)newObj)._description;
                if (this.RemoveObj(refObj)) //_area.RemoveObject(refObj)
                {
                    this.AddProperty(regN,desc,((Property)newObj).Coordinates);
                }
                //((Property)refObj).Coordinates = (new Coordinates(x0, y0, 0), new Coordinates(xk, yk, 0));
            }
            else //is PLOT
            {
                var regN = ((PlotOfLand)newObj)._registerNumber;
                var desc = ((PlotOfLand)newObj)._description;
                if (this.RemoveObj(refObj))//_area.RemoveObject(refObj)
                {
                    this.AddPlot(regN, desc, ((PlotOfLand)newObj).Coordinates);
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
            }
            else
            {
                ((PlotOfLand)refObj).RegisterNumber = regNumber;
                ((PlotOfLand)refObj).Description = description;
            }
        }

        public bool EditObject(Polygon oldObj, Polygon newObj, bool keyAttr) 
        {
            //returns the reference to a object in structure
            var _refObj = this._area.ShowObject(oldObj);

            if (_refObj != null)
            {
                //nasiel sa
                if (keyAttr)
                {
                    if (newObj is PlotOfLand)
                    {
                        this.ChangeKeyAttr(((PlotOfLand)_refObj), (PlotOfLand)newObj);
                            /*((PlotOfLand)newObj).Coordinates.startPos.Longitude, 
                            ((PlotOfLand)newObj).Coordinates.startPos.Latitude, 
                            ((PlotOfLand)newObj).Coordinates.endPos.Longitude, 
                            ((PlotOfLand)newObj).Coordinates.endPos.Latitude);*/
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
            }




        }

        public Polygon PickToEdit(Polygon obj)
        {
            return (Polygon)this._area.ShowObject(obj);
        }

        public Property PickAttrProp(Property obj)
        {
            Property _refProp = (Property)this._area.ShowObject(obj);

            //just attributes for form
            Property pomProp = new Property(_refProp.RegisterNumber, _refProp.Description, _refProp.Coordinates, null);

            return pomProp;
        }

        public PlotOfLand PickAttrPlot(PlotOfLand obj)
        {
            PlotOfLand _refPlot = (PlotOfLand)this._area.ShowObject(obj);

            //just attributes for form
            PlotOfLand pomPlot = new PlotOfLand(_refPlot.RegisterNumber, _refPlot.Description, _refPlot.Coordinates, null);

            return pomPlot;
        }

        public void WriteToFiles()
        {
            StreamWriter writerProp = null;
            StreamWriter writerPlots = null;
            try
            {
                writerProp = new StreamWriter("Properties.txt");
                writerPlots = new StreamWriter("Plots.txt");


                var AllObj = this.FindInterval((new Coordinates(this._area._dimension.X0, this._area._dimension.Y0, 0), new Coordinates(this._area._dimension.Xk, this._area._dimension.Yk, 0)));
                
                foreach (var obj in AllObj)
                {
                    if (obj is Property)
                    {
                        writerProp.WriteLine( obj.GetType().Name + ";" +   ((Property)obj)._registerNumber + ";" +
                            ((Property)obj).Coordinates.x.Longitude + ((Property)obj).Coordinates.x.LongHem + ";" + 
                            ((Property)obj).Coordinates.x.Latitude + ((Property)obj).Coordinates.x.LatHem + ";" +
                            ((Property)obj).Coordinates.y.Longitude + ((Property)obj).Coordinates.y.LongHem + ";"+
                            ((Property)obj).Coordinates.y.Latitude + ((Property)obj).Coordinates.y.LatHem + ";"+

                            ((Property)obj)._description);
                    }
                    else
                    {
                        writerPlots.WriteLine(obj.GetType().Name + ";" + ((PlotOfLand)obj)._registerNumber + ";" + ((PlotOfLand)obj).Coordinates.startPos.Longitude + ((PlotOfLand)obj).Coordinates.startPos.LongHem + ";" + ((PlotOfLand)obj).Coordinates.startPos.Latitude + ((PlotOfLand)obj).Coordinates.startPos.LatHem + ";" +
                            ((PlotOfLand)obj).Coordinates.endPos.Longitude + ((PlotOfLand)obj).Coordinates.endPos.LongHem + ";" + ((PlotOfLand)obj).Coordinates.endPos.Latitude + ((PlotOfLand)obj).Coordinates.endPos.LatHem + ";" + ((PlotOfLand)obj)._description);
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
                    if (line.Contains("Property"))
                    {
                        // Split the line by semicolon to extract data
                        string[] parts = line.Split(';');

                        if (parts.Length >= 7)
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

                            string description = parts[6];

                            AddProperty(id, description, ((new Coordinates(coordX,coordY,0)),new Coordinates(coordXE, coordYE, 0)));
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
                    if (line.Contains("PlotOfLand"))
                    {
                        // Split the line by semicolon to extract data
                        string[] parts = line.Split(';');

                        if (parts.Length >= 7)
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

                            string description = parts[6];

                            AddPlot(id, description, ((new Coordinates(coordStartX, coordStartY, 0)), new Coordinates(coordEndX, coordEndY, 0)));
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


        //TODO: PRESYPANIE VSETKYCH DAT DO NOVEJ STRUKTURY
        //NASTAVENIE ROZSAHU + VLOZENIE PRVKOV
        //ZORADENIE

        public void WithdrawAndOrder(bool increaseSize) 
        {
            var oldHealth = this._area.TreeHealth.Value;
            var objects = this._area.IntervalSearch(this._area._dimension, true);
            var polygons = objects.OfType<Polygon>().ToList();

            //sort according to size
            polygons.Sort();

            MyQuadTree newArea = null;
            if (increaseSize)
            {
                //widen and lenghten to 10 percent of original size
                double tenPercentX = (this._area._dimension.Xk - this._area._dimension.X0) * 0.1;
                double tenPercentY = (this._area._dimension.Yk - this._area._dimension.Y0) * 0.1;

                Boundaries newBoundaries = new Boundaries(this._area._dimension.X0 - (tenPercentX / 2), this._area._dimension.Y0 - (tenPercentY / 2),
                    this._area._dimension.Xk + (tenPercentX / 2), this._area._dimension.Yk + (tenPercentY / 2));
                newArea = new MyQuadTree(newBoundaries, this._area.maxDepth, this._area.MAX_QUAD_CAPACITY);
            }
            else
            {
                newArea = new MyQuadTree(this._area._dimension, this._area.maxDepth, this._area.MAX_QUAD_CAPACITY);
            }
            

            foreach (var obj in polygons)
            {
                newArea.Insert(obj);
            }

            if (newArea.TreeHealth.Value > this._area.TreeHealth.Value)
            {
                this._area = newArea;
                this._area.improvement += this._area.TreeHealth.Value - oldHealth;
                improvedWithReinsert = true;
            }
        }

        public double getHealtOfStruct()
        {
            return this._area.TreeHealth.Value;
        }

        public double getImprovement()
        {
            return this._area.improvement;
        }

        public int getDepthOfStruct()
        {
            return this._area.maxDepth;
        }

        public void setOptimalization(bool optimalize) 
        {
            this._area.wantOptimizing = optimalize;
        }

        public void Reset() 
        {
            this._area.ResetTree(this._area._root);
            improvedWithReinsert = false;
        }

        public void OptimizeStruct() {
            while (!this._area.noMoreOpt)
            {
                this._area.Optimize();
            }
            
        }
    }
}
