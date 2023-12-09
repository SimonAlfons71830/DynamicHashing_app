using QuadTree.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree.QTree
{
    public interface IQuadTree
    {
        public int _maxDepth { get; set; }
        public int _objectsCount { get; set; }
        public int _objectsSearched { get; set; }
        public int MAX_QUAD_CAPACITY { get; set; }
        public Boundaries _dimension { get; set; }
        public Quad _root  { get; set; }


        public void Insert(ISpatialObject _object);
        public ISpatialObject PointSearch(ISpatialObject _object);
        public List<ISpatialObject> IntervalSearch(Boundaries boundaries, bool interfere);
        public bool RemoveObject(ISpatialObject _object);
        public void ResetTree(Quad quad);
        public List<ISpatialObject> IntervalSearchN(Boundaries boundaries, bool interfere);
        public void SetNewDepth(int newDepth);

    }
}
