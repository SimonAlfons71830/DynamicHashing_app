using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using QuadTree.QTree;

namespace QuadTree.Structures
{
    public interface ISpatialObject
    {
        // Properties that are common to both points and polygons.
        public double _x { get; }
        public double _y { get; }
        public int _registerNumber { get; set; }

        Quad FindQuad(Quad quad);
        public bool IsContainedInArea(Boundaries boundaries, bool interfere);
        Quad FindQuadUpdate(Quad quad);
    }
}
