using System.ComponentModel;
using System.Drawing.Drawing2D;
using QuadTree.Structures;

namespace QuadTree.QTree
{
    /// <summary>
    /// also called Node (Vrchol)
    /// </summary>
    public class Quad
    {
        /*
        struct that represents a rectangle
        double _x0, _y0; //start of the rectangle (left bottom as xy axis)
        double xk, yk; //end of the rectangle (right upper corner)
        */

        // in every node/quad there is
        // List of pointers to each child (NW, NE, SW, SE)
        // coordinates of area that node represents
        // list to save data 

        //primary _registerNumber
        private MyPoint _key; //the starting points of the quad
        //secondary _registerNumber
        public Boundaries _boundaries; //X0, Y0, Heigth, Width
        public int level;

        //list of the points that belongs to the Quad
        public List<ISpatialObject> _objects = new List<ISpatialObject>();

        //public List<Quad> children = new List<Quad>();

        public Quad? _northWest { get; set; }
        public Quad? _northEast { get; set; }
        public Quad? _southEast { get; set; }
        public Quad? _southWest { get; set; }

        public Quad(Boundaries boundaries, int level)
        {
            _key = new MyPoint(boundaries.X0, boundaries.Y0, 90000);
            _boundaries = boundaries;
            this.level = level;
        }

        public Quad? getNW() { return _northWest; }
        public Quad? getNE() { return _northEast; }
        public Quad? getSE() { return _southEast; }
        public Quad? getSW() { return _southWest; }

        /// <summary>
        /// Splits the current Quad into four child Quads, 
        /// dividing its boundaries into equal-sized Rectangles.
        /// </summary>
        /// <remarks>
        /// This method divides the current Quad into four child Quads, each covering a distinct quadrant
        /// within the boundaries of the _parent Quad. The Quad is split into the following child Quads:
        /// - North-West (Top-left) Quadrant
        /// - North-East (Top-right) Quadrant
        /// - South-East (Bottom-right) Quadrant
        /// - South-West (Bottom-left) Quadrant
        /// </remarks>
        public void splitQuad() {
            int pom = level + 1;
            _northWest = new Quad(new Boundaries(_boundaries.X0, (_boundaries.Y0 + _boundaries.Yk) /2, (_boundaries.Xk + _boundaries.X0) /2 , _boundaries.Yk),pom);
            _northEast = new Quad(new Boundaries((_boundaries.Xk + _boundaries.X0) / 2, (_boundaries.Yk + _boundaries.Y0) / 2, _boundaries.Xk,_boundaries.Yk), pom);
            _southEast = new Quad(new Boundaries((_boundaries.Xk + _boundaries.X0) / 2,_boundaries.Y0,_boundaries.Xk, (_boundaries.Yk + _boundaries.Y0) / 2), pom);
            _southWest = new Quad(new Boundaries(_boundaries.X0,_boundaries.Y0, (_boundaries.Xk + _boundaries.X0) / 2, (_boundaries.Yk + _boundaries.Y0) / 2), pom);

        }

        //proportions ?
        public void splitQuadUpdate(MyPoint centroid) 
        {
            int pom = level + 1;
            _northWest = new Quad(new Boundaries(_boundaries.X0, centroid._y, centroid._x, _boundaries.Yk), pom);
            _northEast = new Quad(new Boundaries(centroid._x, centroid._y, _boundaries.Xk, _boundaries.Yk), pom);
            _southEast = new Quad(new Boundaries(centroid._x, _boundaries.Y0, _boundaries.Xk, centroid._y), pom);
            _southWest = new Quad(new Boundaries(_boundaries.X0, _boundaries.Y0, centroid._x, centroid._y), pom);

        }


        public bool Equals(Quad? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            double epsylon = 0.0000001;

            return (_boundaries.X0 + epsylon > other._boundaries.X0 && _boundaries.X0 - epsylon < other._boundaries.X0 ) &&
          (_boundaries.Y0 + epsylon > other._boundaries.Y0 && _boundaries.Y0 - epsylon < other._boundaries.Y0) &&
          (_boundaries.Xk + epsylon > other._boundaries.Xk && _boundaries.Xk - epsylon < other._boundaries.Xk) &&
          (_boundaries.Yk + epsylon > other._boundaries.Yk && _boundaries.Yk - epsylon < other._boundaries.Yk);
        }
    }
}
