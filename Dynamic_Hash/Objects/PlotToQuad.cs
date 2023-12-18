using QuadTree.Hashing;
using QuadTree.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Dynamic_Hash.Objects
{
    public class PlotToQuad : Polygon
    {
        private int _registerNumber;
        private ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) _coordinates;

        /// <summary>
        /// Constructor with input parameters to create object
        /// </summary>
        /// <param name="registerNumber"></param>
        /// <param name="description"></param>
        /// <param name="coordinates"></param>
        /// <param name="properties"></param>
        public PlotToQuad(int registerNumber, ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinates)
            : base(registerNumber, coordinates)
        {
            RegisterNumber = registerNumber;
            Coordinates = coordinates;
        }

        public int RegisterNumber
        {
            get => _registerNumber;
            set => _registerNumber = value;
        }

        public ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) Coordinates
        {
            get => _coordinates;
            set => _coordinates = value;
        }

    }
}
