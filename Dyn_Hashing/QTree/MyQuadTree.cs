using QuadTree.Optimalization;
using QuadTree.Structures;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree.QTree
{
    public class MyQuadTree : IQuadTree
    {
        public Health TreeHealth { get; set; } = new Health();
        public bool wasOptimalized = false;
        public bool noMoreOpt = false;

        public double improvement = 0;
        public Queue<Quad> needsToBeImproved = new Queue<Quad>();
        public bool wantOptimizing = false;

        public int quadsImproved = 0;



        public int _maxDepth { get; set; }
        public int _objectsCount { get; set; }
        public int _objectsSearched { get; set; }
        public int MAX_QUAD_CAPACITY { get; set; }
        public Boundaries _dimension { get; set; }
        public Quad _root { get; set; }
        public int maxDepth;

        public MyQuadTree(Boundaries dimension, int maxDepth, int max_cap)
        {
            _maxDepth = maxDepth;
            MAX_QUAD_CAPACITY = max_cap;
            _dimension = dimension;
            _root = new Quad(_dimension, 0);
        }

        public void Insert(ISpatialObject _object)
        {
            Quad current = _root;
            while (true)
            {
                //if there is any child of the current currentQuad, determine in which childQuad point belongs to
                //if it is on the _borders the subQuad will be null and object will be added to current
                if (current.getNW() != null)
                {
                    //find position of the point to the right currentQuad
                    Quad subQuad = _object.FindQuadUpdate(current);
                    if (subQuad != null)
                    {
                        current._objects.Remove(_object);
                        current = subQuad;
                        //continue the iterations until the leaf is found
                        continue;
                    }
                }
                current._objects.Add(_object);
                //pom
                _objectsCount++;

                if (!NeedToSplit(current) && current._objects.Count > MAX_QUAD_CAPACITY)
                {
                        this.TreeHealth.CalculateNewHealthObjCountInQuad(current._objects.Count, MAX_QUAD_CAPACITY);

                        /*//ak healt klesene pod 62
                        if (this.TreeHealth.Value <= 62 && this.wantOptimizing)
                        {
                            this.Optimize();
                        }*/
                }

                //check whether the currentQuad needs to be split - (maxQuadCapacity, level, already is split)
                while (NeedToSplit(current))
                {
                    
                    current.splitQuadUpdate(this.findCentroid(current._objects));
                    //every point from the currentQuad must bee removed and reinserted to the correct child of currentQuad
                    Queue<ISpatialObject> reinsertObjects = new Queue<ISpatialObject>(current._objects);
                    current._objects.Clear();
                    //pom to remember the last point's quad
                    var pomC = current;

                    //pom
                    if (current._northEast.level > maxDepth)
                    {
                        maxDepth = current._northEast.level;
                    }

                    while (reinsertObjects.Count > 0)
                    {
                        //find the right quad for objects
                        var rObject = reinsertObjects.Dequeue();
                        Quad? rQuad = rObject.FindQuadUpdate(current);

                        (rQuad ?? current)._objects.Add(rObject);

                        pomC = rQuad;

                        //splitol sa ale objekty sa nedali nizsie
                        if (current._objects.Count > MAX_QUAD_CAPACITY)
                        {
                            this.TreeHealth.CalculateNewHealthObjCountInQuad(current._objects.Count, MAX_QUAD_CAPACITY);
                        }
                    }
                    
                    //check if quad where was the last point inserted needs to be split again 
                    //last point's is checked bc of it the quad needed to be split in first place
                    current = pomC;
                    //if the current is null -> object is on the boundary
                    if (current == null) { break; }
                }

                break;
            }
        }

        public bool NeedToSplit(Quad quad)
        {
            /*if (wantOptimizing)
            {*//*
                if (quad._objects.Count > MAX_QUAD_CAPACITY && quad.level >= _maxDepth)
                {
                   // this.TreeHealth.CalculateNewHealthObjCountInQuad(quad._objects.Count, MAX_QUAD_CAPACITY);

                if (this.TreeHealth.Value < 50)
                {
                    this.Optimize();
                }
            }*/
            //}
            /*if (quad._objects.Count > MAX_QUAD_CAPACITY && quad.level > 0 && quad.level < _maxDepth)
            {
                // not in the lowest level of the struct but will be optimized in the second part of optimalization
                if (!needsToBeImproved.Contains(quad))
                {
                    needsToBeImproved.Enqueue(quad);
                }
                
            }*/

            return quad._objects.Count > MAX_QUAD_CAPACITY && quad.level < _maxDepth && quad.getNW() == null;


        }

        public MyPoint findCentroid(List<ISpatialObject> _objects)
        {
            double sumX = 0.0;
            double sumY = 0.0;

            foreach (var point in _objects)
            {
                sumX += point._x;
                sumY += point._y;
            }

            // Calculate the centroid (midpoint)
            double midX = sumX / _objects.Count;
            double midY = sumY / _objects.Count;

            return new MyPoint(midX, midY, -1);
        }

        public List<ISpatialObject> IntervalSearch(Boundaries boundaries, bool interfere)
        {
            List<ISpatialObject> foundObjects = new List<ISpatialObject>();
            Queue<Quad> quadsQ = new Queue<Quad>();
            quadsQ.Enqueue(_root);

            while (quadsQ.Count > 0)
            {
                Quad current = quadsQ.Dequeue();

                if (boundaries.IntersectsWithArea(current._boundaries))
                {
                    foreach (var _object in current._objects)
                    {
                        this._objectsSearched++;
                        if (_object.IsContainedInArea(boundaries, interfere))
                        {
                            foundObjects.Add(_object);
                        }
                    }

                    if (current.getNE() != null)
                    {
                        quadsQ.Enqueue(current.getNE());
                        quadsQ.Enqueue(current.getNW());
                        quadsQ.Enqueue(current.getSW());
                        quadsQ.Enqueue(current.getSE());
                    }
                }
            }
            return foundObjects;
        }

        public List<ISpatialObject> IntervalSearchN(Boundaries boundaries, bool intersects)
        {
            List<ISpatialObject> foundObjects = new List<ISpatialObject>();
            Queue<Quad> quads = new Queue<Quad>();
            //quads.Enqueue(_root);

            if (boundaries.IntersectsWithArea(_root._boundaries))
            {
                quads.Enqueue(_root);
                while (quads.Count > 0)
                {

                    Quad current = quads.Dequeue();
                    foreach (var _object in current._objects)
                    {
                        if (_object.IsContainedInArea(boundaries, intersects))
                        {
                            foundObjects.Add(_object);
                        }
                    }

                    if (current.getNW() != null)
                    {
                        quads.Enqueue(current.getNW());
                        quads.Enqueue(current.getNE());
                        quads.Enqueue(current.getSW());
                        quads.Enqueue(current.getSE());
                    }
                }
                return foundObjects;
            }
            return null;
        }

        public ISpatialObject PointSearch(ISpatialObject _object)
        {
            Queue<Quad> quads = new Queue<Quad>();
            quads.Enqueue(_root);

            while (quads.Count > 0)
            {
                Quad quad = quads.Dequeue();
                if (_object.IsContainedInArea(quad._boundaries, false))
                {
                    //when the object is located in one of the 4 added currentQuad i can erease all of them from there
                    //ill be adding just childQuads of the currentQuad that is object within
                    quads.Clear();

                    foreach (var point in quad._objects)
                    {
                        //if (point is MyPoint)
                        //{
                        if (_object == point)
                        {
                            //return (MyPoint?)point;
                            return point;
                        }
                        //}
                    }
                    if (quad.getNE() != null)
                    {
                        quads.Enqueue(quad.getNE());
                        quads.Enqueue(quad.getNW());
                        quads.Enqueue(quad.getSE());
                        quads.Enqueue(quad.getSW());
                    }
                }
            }
            return null;
        }

        public bool RemoveObject(ISpatialObject _object)
        {
            //find object
            //if quad that would containd P is empty return null
            //else remove P change to empty
            //ak max jeden z childnodov ma nejaky bod tak zucim tieto childnody
            //ak dvaja surodenci obsahuju bod tak ich necham tak

            var listOfObjectsOnThePositions = this.IntervalSearch(new Boundaries(_object._x, _object._y, _object._x, _object._y), true);
            ISpatialObject objectDelete = null;

            foreach (ISpatialObject _item in listOfObjectsOnThePositions)
            {
                if (_object is Polygon && _item is Polygon)
                {
                    if (((Polygon)_object).Equals((Polygon)_item))
                    {
                        objectDelete = _item;
                        break;
                    }
                }
                else if (_object is MyPoint && _item is MyPoint)
                {
                    if (((MyPoint)_object).Equals((MyPoint)_item))
                    {
                        objectDelete = _item;
                        break;
                    }
                }
            }

            List<Quad> pathToObject = new List<Quad>();
            pathToObject.Add(_root);
            if (objectDelete != null)
            {
                Quad current = _root;
                while (current != null)
                {
                    foreach (var _obj in current._objects)
                    {
                        if (_obj == objectDelete)
                        {
                            int oldCount = current._objects.Count;

                            current._objects.Remove(_obj);
                            this._objectsCount--;

                            //HEALTH CHECK
                            if (oldCount > MAX_QUAD_CAPACITY && current._objects.Count <= MAX_QUAD_CAPACITY)
                            {
                                if (TreeHealth.Value < 100)
                                {
                                    TreeHealth.ReverseHealth(current._objects.Count, MAX_QUAD_CAPACITY);
                                }
                            }
                            //this.Rejoin(current, _parent);
                            this.RejoinUpdate(pathToObject);

                            return true;
                        }
                    }
                    //not in the current -> search childQuad
                    current = objectDelete.FindQuadUpdate(current);
                    pathToObject.Add(current);
                }
                //notfound

                return false;
            }
            else
            {

                return false;
            }
        }

        public void RejoinUpdate(List<Quad> pathToObject)
        {
            //objekt mazany z roota
            if (pathToObject.Count == 1)
            {
                if (_objectsCount < MAX_QUAD_CAPACITY)
                {
                    //root je posledny quad
                    Queue<Quad> q = new Queue<Quad>();
                    q.Enqueue(pathToObject.ElementAt(0));
                    List<ISpatialObject> objectsToReinsert = new List<ISpatialObject>();

                    while (q.Count > 0)
                    {
                        var quad = q.Dequeue();
                        objectsToReinsert.AddRange(quad._objects);
                        quad._objects.Clear();
                        if (quad._southEast != null)
                        {
                            q.Enqueue(quad._southEast);
                            q.Enqueue(quad._northEast);
                            q.Enqueue(quad._southWest);
                            q.Enqueue(quad._northWest);
                        }
                    }

                    if (objectsToReinsert.Count < MAX_QUAD_CAPACITY)
                    {
                        _root._objects.AddRange(objectsToReinsert);

                        _root._northEast = null;
                        _root._northWest = null;
                        _root._southEast = null;
                        _root._southWest = null;

                    }
                }
            }
            while (pathToObject.Count > 1)
            {
                var current = pathToObject.ElementAt(pathToObject.Count - 1);
                var parent = pathToObject.ElementAt(pathToObject.Count - 2);

                //spocitam objekty parentovych potomkov
                Queue<Quad> parentQuads = new Queue<Quad>();
                parentQuads.Enqueue(parent);
                var count = 0;
                List<ISpatialObject> objectsToReinsert = new List<ISpatialObject>();

                while (parentQuads.Count > 0)
                {

                    Quad quad = parentQuads.Dequeue();
                    count += quad._objects.Count;

                    objectsToReinsert.AddRange(quad._objects);


                    if (quad._southEast != null)
                    {
                        parentQuads.Enqueue(quad.getNW());
                        parentQuads.Enqueue(quad.getSW());
                        parentQuads.Enqueue(quad.getNE());
                        parentQuads.Enqueue(quad.getSE());
                    }

                }
                if (count <= MAX_QUAD_CAPACITY)
                {
                    parent._southWest = null;
                    parent._northEast = null;
                    parent._northWest = null;
                    parent._southEast = null;

                    parent._objects = objectsToReinsert;
                }
                else
                {
                    return;
                }

                pathToObject.RemoveAt(pathToObject.Count - 1);
            }
        }

        public void ResetTree(Quad quad)
        {
            var pom = _root;
            _root = new Quad(pom._boundaries, 0);
            _objectsCount = 0;
            _objectsSearched = 0;
            maxDepth = 0;
            TreeHealth.Reset();
            wasOptimalized = false;
            improvement = 0;
            needsToBeImproved.Clear();
            quadsImproved = 0;
            noMoreOpt = false;

    }

        public void SetNewDepth(int newDepth)
        {
            //int maxDepth;
            //Queue<Quad> quadsAtMaxDepth = GetQuadsAtMaxDepth(out maxDepth);

            if (newDepth > maxDepth)
            {
                var list = GetQuadsAtDepth(maxDepth);
                //tree has to grow
                this.Grow(list, newDepth);
                this.maxDepth = newDepth;
                this._maxDepth = newDepth;
            }
            else
            {
                noMoreOpt = false;
                // var quadsToShrink = this.GetQuadsAtDepth(newDepth);
                //tree has to shrink
                this.Shrink(newDepth);
                this.maxDepth = newDepth;
                this._maxDepth = newDepth;
            }
            //else the depth stays the same
        }

        public Queue<Quad> GetQuadsAtDepth(int targetDepth)
        {
            Queue<Quad> allQuads = new Queue<Quad>();
            Queue<Quad> quadQueue = new Queue<Quad>();

            // Start from the root Quad
            quadQueue.Enqueue(_root);

            while (quadQueue.Count > 0)
            {
                // Dequeue the current Quad
                Quad currentQuad = quadQueue.Dequeue();

                // Check if the current Quad has the desired level
                if (currentQuad.level == targetDepth)
                {
                    allQuads.Enqueue(currentQuad);
                }

                // Enqueue child quads if they exist
                if (currentQuad.getNE() != null)
                {
                    quadQueue.Enqueue(currentQuad.getNE());
                    quadQueue.Enqueue(currentQuad.getNW());
                    quadQueue.Enqueue(currentQuad.getSW());
                    quadQueue.Enqueue(currentQuad.getSE());
                }
            }

            return allQuads;
        }

        public void Grow(Queue<Quad> growingQuads, int desiredDepth)
        {
            while (growingQuads.Count > 0)
            {
                Quad current = growingQuads.Dequeue();
                if (current._objects.Count > 0)
                {
                    int oldCurrentCount = current._objects.Count;
                    //if it has to grow i just need to take the quads from list that has the same level as the newDepth and split them until the level is reached
                    current.splitQuadUpdate(this.findCentroid(current._objects));
                    //every point from the currentQuad must bee removed and reinserted to the correct child of currentQuad
                    Queue<ISpatialObject> reinsertObjects = new Queue<ISpatialObject>(current._objects);
                    current._objects.Clear();
                    //pom to remember the last point's quad
                    var pomC = current;

                    while (reinsertObjects.Count > 0)
                    {
                        //find the right quad for objects
                        var rObject = reinsertObjects.Dequeue();
                        Quad? rQuad = rObject.FindQuadUpdate(current);

                        (rQuad ?? current)._objects.Add(rObject);

                        pomC = rQuad;
                        if (rQuad != null)
                        {
                            //quad sa bude delit az kym nedosiahne desiredDepth ak su v nom nejake objekty
                            if (rQuad._objects.Count != 0 && rQuad.level < desiredDepth)
                            {
                                growingQuads.Enqueue(rQuad);
                            }
                            //otherwise we do not have to split more
                        }
                    }
                    if (oldCurrentCount > MAX_QUAD_CAPACITY && oldCurrentCount > current._objects.Count && current._objects.Count <= MAX_QUAD_CAPACITY)
                    {
                        TreeHealth.ReverseHealth(current._objects.Count, MAX_QUAD_CAPACITY);
                    }
                }
            }
        }


        public void Shrink(int desiredDepth)
        {
            // maxDepth - je aktualna level stromu
            //zoberiem vsetky objekty z quadov od <maxDept, newDepth-1> //nemusim ist uplne po spodok ak je maxDepth mensia
            //jednotlivym objektom najdem kvad na newdepth a vlozim ich do neho
            improvement = 0;
            quadsImproved = 0;
            TreeHealth.Value = 0;

            List<ISpatialObject> reinsertObj = new List<ISpatialObject>();
            Queue<Quad> quadsAtDesiredDepth = GetQuadsAtDepth(desiredDepth);
            bool isRoot = desiredDepth == 0 ? true : false;
            Queue<Quad> parentsOfDesiredDepthQuads;
            if (isRoot)
            {
                parentsOfDesiredDepthQuads = null;
            }
            else
            {
                parentsOfDesiredDepthQuads = GetQuadsAtDepth(desiredDepth - 1);
            }



            //prehladat vsetky quads z nizsich levelov ako je desired depth 
            //ulozit si ich objekty do listu
            for (int i = maxDepth; i > desiredDepth; i--)
            {
                Queue<Quad> quadsFromMaxD = this.GetQuadsAtDepth(i);

                foreach (Quad quad in quadsFromMaxD)
                {
                    reinsertObj.AddRange(quad._objects);
                }
            }


            //ak je root tak zoberiem vsetky objekty a vlozim ich do roota inak ->
            if (isRoot)
            {
                _root._objects.AddRange(reinsertObj);
            }
            else
            {
                //pre ulozzene objekty najst prislusny quad z desired depth
                //mam funkciu find quad - kde z parenta najde najvhodnejsieho childQuada - tymi budu desiredDepth quady
                foreach (var _object in reinsertObj)
                {
                    foreach (var quad in parentsOfDesiredDepthQuads)
                    {
                        Quad pomQ = _object.FindQuadUpdate(quad);

                        if (pomQ != null && quadsAtDesiredDepth.Contains(pomQ))
                        {
                            pomQ._objects.Add(_object);

                            if (pomQ._objects.Count > MAX_QUAD_CAPACITY)
                            {
                                if (TreeHealth.Value > 0)
                                {
                                    TreeHealth.CalculateNewHealthObjCountInQuad(pomQ._objects.Count, MAX_QUAD_CAPACITY);
                                }
                            }
                        }
                    }
                }
            }

            //pre vsetkych desired depth vymazat potomkov - GC
            foreach (var quad in quadsAtDesiredDepth) {
                if (quad.getNE() != null)
                {
                    quad._northEast = null;
                    quad._northWest = null;
                    quad._southEast = null;
                    quad._southWest = null;

                }
            }
        }

        public ISpatialObject ShowObject(ISpatialObject _obj)
        {
            var foundObj = this.IntervalSearch(new Boundaries(_obj._x, _obj._y, _obj._x, _obj._y), true);
            ISpatialObject objectEdit = null;

            foreach (ISpatialObject _item in foundObj)
            {
                if (_obj is Polygon && _item is Polygon)
                {
                    if (((Polygon)_obj).Equals((Polygon)_item))
                    {
                        return _item;
                    }
                }
                else if (_obj is MyPoint && _item is MyPoint)
                {
                    if (((MyPoint)_obj).Equals((MyPoint)_item))
                    {
                        return _item;
                    }
                }
            }
            return null;

        }


        public void Optimize()
        {
            //first part is to collect all the quads on the lowest level,
            //check if they need to split and if so, do it
            //adjust healt value
            wasOptimalized = true;
            var oldHealth = TreeHealth.Value;
            
            Queue<Quad> quadsToBeOptimized = this.GetQuadsAtDepth(maxDepth);
            var QuadsToImprove = quadsToBeOptimized.Where(quad => quad._objects.Count > MAX_QUAD_CAPACITY).ToList();
            if (QuadsToImprove.Count == 0)
            {
                noMoreOpt = true;
            }

            while (quadsToBeOptimized.Count > 0)
            {
                var quad = quadsToBeOptimized.Dequeue();

                if (quad._objects.Count > MAX_QUAD_CAPACITY)
                {
                    quad.splitQuadUpdate(this.findCentroid(quad._objects));

                    Queue<ISpatialObject> reinsertObjects = new Queue<ISpatialObject>(quad._objects);
                    quad._objects.Clear();

                    if (quad._northEast.level > maxDepth)
                    {
                        maxDepth = quad._northEast.level;
                    }

                    while (reinsertObjects.Count > 0)
                    {
                        //find the right quad for objects
                        var rObject = reinsertObjects.Dequeue();
                        Quad? rQuad = rObject.FindQuadUpdate(quad);

                        (rQuad ?? quad)._objects.Add(rObject);
                    }

                    //podarilo sa opravit tento quad - > zvysi sa health
                    if (quad._objects.Count <= MAX_QUAD_CAPACITY)
                    {
                        TreeHealth.ReverseHealth(quad._objects.Count, MAX_QUAD_CAPACITY);
                        quadsImproved++;
                    }
                }
            }

            //improvement += TreeHealth.Value - oldHealth;

            //second part is to collect other quads that are not at the end and that needs to split
            //starting from the lowest level in these quads - collect all their objects and objects of the child quads
            //try to split them again
            //maybe try sorting the object according to their size

            /*//var QuadsToImprove = needsToBeImproved.ToList();
            var QuadsToImprove = needsToBeImproved.Where(quad => quad._objects.Count > MAX_QUAD_CAPACITY).ToList();
            needsToBeImproved.Clear();
            QuadsToImprove.Sort((quad1, quad2) => quad2.level.CompareTo(quad1.level));
            var quadsToBeImproved = new Queue<Quad>(QuadsToImprove);


            while (quadsToBeImproved.Count > 0)
            {
                var current = quadsToBeImproved.Dequeue();
                //collect all its child quads objects and resplit again
                List<ISpatialObject> objectsFromCurrent = new List<ISpatialObject>();

                Queue<Quad> quadsToCollectObj = new Queue<Quad>();
                quadsToCollectObj.Enqueue(current);

                while (quadsToCollectObj.Count > 0)
                {

                    var quad = quadsToCollectObj.Dequeue();
                    objectsFromCurrent.AddRange(quad._objects);

                    if (quad._northWest != null)
                    {
                        quadsToCollectObj.Enqueue(quad._northWest);
                        quadsToCollectObj.Enqueue(quad._southEast);
                        quadsToCollectObj.Enqueue(quad._southWest);
                        quadsToCollectObj.Enqueue(quad._northEast);
                    }
                }

                current._objects.Clear();
                current._northEast = null;
                current._southWest = null;
                current._northWest = null;
                current._southEast = null;


                //now insert the objects from the list to current and continue until no object is left in list
                //forget the depth 

                var polygons = objectsFromCurrent.OfType<Polygon>().ToList();
                polygons.Sort();
                var oldCurrent = current;

                while (polygons.Count > 0)
                {
                    //get back to first quad
                    current = oldCurrent;

                    var obj = polygons.ElementAt(0);
                    //i have obj and a ned Quad current

                    //ak je uz podeleny najde potomka kde objekt patri
                    while (current.getNE() != null)
                    {
                        var subquad = obj.FindQuadUpdate(current);

                        if (subquad != null)
                        {
                            current = subquad;
                        }
                        else
                        {
                            break;
                        }
                    }

                    current._objects.Add(obj);
                    polygons.RemoveAt(0);



                    if (current._objects.Count > MAX_QUAD_CAPACITY)
                    {
                        current.splitQuadUpdate(this.findCentroid(current._objects));
                        //vytiahnut vsetky z currenta a nanovo ich vlozit
                        List<ISpatialObject> reinsertList = new List<ISpatialObject>(current._objects);
                        current._objects.Clear();

                        while (reinsertList.Count > 0)
                        {
                            var subquad = reinsertList.ElementAt(0).FindQuadUpdate(current);
                            if (subquad != null)
                            {
                                subquad._objects.Add(reinsertList.ElementAt(0));
                            }
                            else
                            {
                                current._objects.Add(reinsertList.ElementAt(0));
                            }
                            reinsertList.RemoveAt(0);
                        }
                    }

                    
                }
                if (oldCurrent._objects.Count < MAX_QUAD_CAPACITY)
                {
                    quadsImproved++;
                    this.TreeHealth.ReverseHealth(current._objects.Count, MAX_QUAD_CAPACITY);
                }
            }*/

            improvement += TreeHealth.Value - oldHealth;
        }



    }
}
