using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using QuadTree.Hashing;
using QuadTree.QTree;

namespace QuadTree.Structures
{
    public class Polygon: IEquatable<Polygon>, ISpatialObject, IComparable<Polygon>
    {
        public ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) _borders;
        //public (MyPoint startP, MyPoint endP) _borders;
        public int _registerNumber { get; set; }
        // Calculate the center (centroid) of the polygon based on its vertices.
        public double _x
        {
            get
            {
                double sumX = 0;
                sumX += _borders.Item1.LongitudeStart;
                sumX += _borders.Item2.LongitudeEnd;
                /*sumX += _borders.startP._x;
                sumX += _borders.endP._x;*/
                return sumX / 2;
            }
        }

        public double _y
        {
            get
            {
                double sumY = 0;
                sumY += _borders.Item1.LatitudeStart;
                sumY += _borders.Item2.LatitudeEnd;
                /*sumY += _borders.startP._y;
                sumY += _borders.endP._y;*/
                return sumY / 2;
            }
        }


        /*public Polygon(int registerNumber, (MyPoint startP, MyPoint endP) borders)
        {
            _registerNumber = registerNumber;
            _borders = borders;
        }*/

        public Polygon(int registerNumber, ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) borders)
        {
            _registerNumber = registerNumber;
            _borders = borders;
        }

        public bool Equals(Polygon? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            //same reference means same objects
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            /*//comparing pairs of tops
            if (!(_borders.startP._x == other._borders.startP._x && _borders.startP._y == other._borders.startP._y && 
                _borders.endP._x == other._borders.endP._x && _borders.endP._y == other._borders.endP._y))
            {
                return false;
            }*/

            if (!(_borders.Item1.LongitudeStart == other._borders.Item1.LongitudeStart && _borders.Item1.LatitudeStart == other._borders.Item1.LatitudeStart &&
                _borders.Item2.LongitudeEnd == other._borders.Item2.LongitudeEnd && _borders.Item2.LatitudeEnd == other._borders.Item2.LatitudeEnd))
            {
                return false;
            }

            if (other._registerNumber != this._registerNumber)
            {
                return false;
            }

            return true; 
        }


        /// <summary>
        /// returns a reference to a quad that can fit whole polygon
        /// if the return value is null, the polygon cant fit into any quad
        /// </summary>
        /// <param name="quad"></param>
        /// <returns></returns>
        public Quad? FindQuad(Quad quad)
        {
            //find the center of quad -> from there the boundaries will be determined
            double centerX = (quad._boundaries.X0 + quad._boundaries.Xk) / 2;
            double centerY = (quad._boundaries.Y0 + quad._boundaries.Yk) / 2;


            double PcenterX = (_borders.Item1.LongitudeStart + _borders.Item2.LongitudeEnd) / 2; 
            double PcenterY = (_borders.Item1.LatitudeStart + _borders.Item2.LatitudeEnd) / 2;

            var pointStartBorders = new MyPoint(_borders.Item1.LongitudeStart, _borders.Item1.LatitudeStart, 0);
            var pointEndBorders = new MyPoint(_borders.Item2.LongitudeEnd, _borders.Item2.LatitudeEnd, 0);

            if (PcenterX < centerX)
            {
                if (PcenterY < centerY)
                {
                    
                    if (pointStartBorders.IsContainedInArea(quad.getSW()._boundaries, false) &&
                        pointEndBorders.IsContainedInArea(quad.getSW()._boundaries, false))
                    {
                        return quad.getSW();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (pointStartBorders.IsContainedInArea(quad.getNW()._boundaries, false) &&
                        pointEndBorders.IsContainedInArea(quad.getNW()._boundaries, false))
                    {
                        return quad.getNW();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (PcenterY < centerY)
                {
                    if (pointStartBorders.IsContainedInArea(quad.getSE()._boundaries, false) 
                        && pointEndBorders.IsContainedInArea(quad.getSE()._boundaries, false))
                    {
                        return quad.getSE();
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    if (pointStartBorders.IsContainedInArea(quad.getNE()._boundaries, false) 
                        && pointEndBorders.IsContainedInArea(quad.getNE()._boundaries, false))
                    {
                        return quad.getNE();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public bool IsContainedInArea(Boundaries boundaries, bool interfere) {

            if (interfere)
            {
                var pointIntersection = false;
                bool sideIntersection = false;

                bool startPointWithinBounds = _borders.Item1.LongitudeStart >= boundaries.X0 && _borders.Item1.LongitudeStart <= boundaries.Xk &&
                                                _borders.Item1.LatitudeStart >= boundaries.Y0 && _borders.Item1.LatitudeStart <= boundaries.Yk;

                bool endPointWithinBounds = _borders.Item2.LongitudeEnd >= boundaries.X0 && _borders.Item2.LongitudeEnd <= boundaries.Xk &&
                                           _borders.Item2.LatitudeEnd >= boundaries.Y0 && _borders.Item2.LatitudeEnd <= boundaries.Yk;

                pointIntersection = startPointWithinBounds || endPointWithinBounds;




                //checking the side intersection only applies to rectangles 
                MyPoint top0 = new MyPoint(_borders.Item1.LongitudeStart, _borders.Item1.LatitudeStart,0);
                //var top0 = _tops.ElementAt(0);
                var topK = new MyPoint(_borders.Item2.LongitudeEnd, _borders.Item2.LatitudeEnd, 0);
                //var topK = _tops.ElementAt(1);
                sideIntersection = (top0._x < boundaries.Xk && topK._x > boundaries.X0 && top0._y < boundaries.Yk && topK._y > boundaries.Y0) ||
                    (boundaries.X0 < topK._x && boundaries.Xk > top0._x && boundaries.Y0 < topK._y && boundaries.Yk > top0._y);


                return pointIntersection || sideIntersection;

            }
            else
            {

                bool startPointWithinBounds = _borders.Item1.LongitudeStart >= boundaries.X0 && _borders.Item1.LongitudeStart <= boundaries.Xk &&
                                                _borders.Item1.LatitudeStart >= boundaries.Y0 && _borders.Item1.LatitudeStart <= boundaries.Yk;

                bool endPointWithinBounds = _borders.Item2.LongitudeEnd >= boundaries.X0 && _borders.Item2.LongitudeEnd <= boundaries.Xk &&
                                           _borders.Item2.LatitudeEnd >= boundaries.Y0 && _borders.Item2.LatitudeEnd <= boundaries.Yk;

                return startPointWithinBounds && endPointWithinBounds;

            }
        }

        public Quad FindQuadUpdate(Quad quad)
        {
            //find the center of quad -> from there the boundaries will be determined
            if (quad._northEast == null)
            {
                return null;
            }
            double centerX = quad._northEast._boundaries.X0;
            double centerY = quad._northEast._boundaries.Y0;


            double PcenterX = (_borders.Item1.LongitudeStart + _borders.Item2.LongitudeEnd) / 2; 
            double PcenterY = (_borders.Item1.LatitudeStart + _borders.Item2.LatitudeEnd) / 2;

            var pointStartBorders = new MyPoint(_borders.Item1.LongitudeStart, _borders.Item1.LatitudeStart, 0);
            var pointEndBorders = new MyPoint(_borders.Item2.LongitudeEnd, _borders.Item2.LatitudeEnd, 0);

            if (PcenterX < centerX)
            {
                if (PcenterY < centerY)
                {
                    if (pointStartBorders.IsContainedInArea(quad.getSW()._boundaries, false) &&
                        pointEndBorders.IsContainedInArea(quad.getSW()._boundaries, false))
                    {
                        return quad.getSW();
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    if (pointStartBorders.IsContainedInArea(quad.getNW()._boundaries, false) &&
                        pointEndBorders.IsContainedInArea(quad.getNW()._boundaries, false))
                    {
                        return quad.getNW();
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            else
            {
                if (PcenterY < centerY)
                {
                    if (pointStartBorders.IsContainedInArea(quad.getSE()._boundaries, false) && pointEndBorders.IsContainedInArea(quad.getSE()._boundaries, false))
                    {
                        return quad.getSE();
                    }
                    else
                    {
                        return null;
                    }


                }
                else
                {
                    if (pointStartBorders.IsContainedInArea(quad.getNE()._boundaries, false) && pointEndBorders.IsContainedInArea(quad.getNE()._boundaries, false))
                    {
                        return quad.getNE();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int CompareTo(Polygon? other)
        {
            var sizeThis = (this._borders.Item2.LongitudeEnd - this._borders.Item1.LongitudeStart ) * (this._borders.Item2.LatitudeEnd - this._borders.Item1.LatitudeStart);
            var sizeOther = (other._borders.Item2.LongitudeEnd - other._borders.Item1.LongitudeStart ) * (other._borders.Item2.LatitudeEnd - other._borders.Item1.LatitudeStart);

            return sizeOther.CompareTo(sizeThis);
        }
    }
}
