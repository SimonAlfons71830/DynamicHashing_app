using QuadTree.Structures;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree.QTree
{
    public class Boundaries : IEquatable<Boundaries>
    {
        private double _x0; //pociatocne suradnice stvoruholniku
        private double _y0;

        private double _xk; //koncove suradnice stvoruholniku
        private double _yk;

        private double width; // Do not rename (binary serialization)
        private double height; // Do not rename (binary serialization)

        /// <summary>
        /// Initializes a new instance of the Boundaries class with the specified starting position
        /// and size.
        /// </summary>
        public Boundaries(double x0, double y0, double xk, double yk)
        {
            this._x0 = x0;
            this._y0 = y0;
            this._xk = xk;
            this._yk = yk;
        }

        /// <summary>
        /// Gets or sets the _x0-coordinate of the upper-left corner of the rectangular region defined by this
        /// </summary>
        public double X0
        {
            get => _x0;
            set => _x0 = value;
        }

        /// <summary>
        /// Gets or sets the _y0-coordinate of the upper-left corner of the rectangular region defined by this
        /// </summary>
        public double Y0
        {
            get => _y0;
            set => _y0 = value;
        }

        /// <summary>
        /// Gets or sets the _x0-coordinate of the upper-left corner of the rectangular region defined by this
        /// </summary>
        public double Xk
        {
            get => _xk;
            set => _xk = value;
        }

        /// <summary>
        /// Gets or sets the _y0-coordinate of the upper-left corner of the rectangular region defined by this
        /// </summary>
        public double Yk
        {
            get => _yk;
            set => _yk = value;
        }

        /// <summary>
        /// Gets or sets the width of the rectangular region defined by this <see cref='System.Drawing.RectangleF'/>.
        /// </summary>
        public double Width
        {
            get => width;
            set => width = value;
        }

        public bool Equals(Boundaries other)
        {
            return this == other;
        }

        public bool IsContainedInQuad(Quad quad) {
            //_x0 _y0 width length
            //4 points  x0y0, xky0, x0yk xkyk
            //4 tops of the rectangular area
            List<MyPoint> points = new List<MyPoint>();
            points.Add(new MyPoint(this._x0, this._y0, 00));
            points.Add(new MyPoint(this._x0, this._yk, 01));
            points.Add(new MyPoint(this._xk ,this._y0, 10));
            points.Add(new MyPoint(this._xk, this._yk, 11));

            foreach (var t in points)
            {
                // Check if the vertex is within the quad's boundaries.
                bool withinXBounds = t._x >= quad._boundaries.X0 && t._x <= quad._boundaries.Xk;
                bool withinYBounds = t._y >= quad._boundaries.Y0 && t._y <= quad._boundaries.Xk;

                if (!withinXBounds || !withinYBounds)
                {
                    // If any vertex is outside the quad, the polygon is not fully contained.
                    return false;
                }
            }
            // If all vertices are within the quad, the polygon is contained.
            return true;
        }

        /*public bool IntersectWith(Quad quad)
        {
            // Check if any of the 4 points of the polygon are within the quad's boundaries.
            List<MyPoint> points = new List<MyPoint>();
            points.Add(new MyPoint(this._x0, this._y0, 00));
            points.Add(new MyPoint(this._x0, this._yk, 01));
            points.Add(new MyPoint(this._xk, this._y0, 10));
            points.Add(new MyPoint(this._xk, this._yk, 11));
            foreach (var t in points)
            {
                // Check if the vertex is within the quad's boundaries.
                bool withinXBounds = t._x >= quad._boundaries.X0 && t._x <= quad._boundaries.Xk;
                bool withinYBounds = t._y >= quad._boundaries.Y0 && t._y <= quad._boundaries.Yk;
                if (withinXBounds && withinYBounds)
                {
                    // If any vertex is within the quad, the polygon intersects with the quad.
                    return true;
                }
            }

            // Check if any of the line segments intersect with the quad's boundaries.
            for (int i = 0; i < points.Count - 1; i++)
            {
                MyPoint p1 = points[i];
                MyPoint p2 = points[i + 1];
                // Check if the line segment intersects with the quad's horizontal boundaries.
                if (p1._y > quad._boundaries.Yk && p2._y < quad._boundaries.Yk)
                {
                    double xIntersection = p1._x + (quad._boundaries.Yk - p1._y) * (p2._x - p1._x) / (p2._y - p1._y);
                    if (xIntersection >= quad._boundaries.X0 && xIntersection <= quad._boundaries.Xk)
                    {
                        // The line segment intersects with the quad's horizontal boundary.
                        return true;
                    }
                }
                else if (p1._y < quad._boundaries.Y0 && p2._y > quad._boundaries.Y0)
                {
                    double xIntersection = p1._x + (quad._boundaries.Y0 - p1._y) * (p2._x - p1._x) / (p2._y - p1._y);
                    if (xIntersection >= quad._boundaries.X0 && xIntersection <= quad._boundaries.Xk)
                    {
                        // The line segment intersects with the quad's horizontal boundary.
                        return true;
                    }
                }
                // Check if the line segment intersects with the quad's vertical boundaries.
                if (p1._x > quad._boundaries.Xk && p2._x < quad._boundaries.Xk)
                {
                    double yIntersection = p1._y + (quad._boundaries.Xk - p1._x) * (p2._y - p1._y) / (p2._y - p1._x);
                    if (yIntersection >= quad._boundaries.Y0 && yIntersection <= quad._boundaries.Yk)
                    {
                        // The line segment intersects with the quad's vertical boundary.
                        return true;
                    }
                }
                else if (p1._x < quad._boundaries.X0 && p2._x > quad._boundaries.X0)
                {
                    double yIntersection = p1._y + (quad._boundaries.X0 - p1._x) * (p2._y - p1._y) / (p2._x - p1._x);
                    if (yIntersection >= quad._boundaries.Y0 && yIntersection <= quad._boundaries.Yk)
                    {
                        // The line segment intersects with the quad's vertical boundary.
                        return true;
                    }
                }
            }

            // If no vertices or line segments intersect with the quad, the polygon does not intersect with the quad.
            return false;
        }*/

        public bool IntersectsWithArea(Boundaries area) 
        {
            return !(area.X0 > this.Xk || area.Xk < this.X0 || area.Y0 > this.Yk || area.Yk < this.Y0);

        }
    }
}
