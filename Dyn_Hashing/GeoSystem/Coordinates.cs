using Microsoft.VisualBasic;
using QuadTree.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace QuadTree.GeoSystem
{
    public class Coordinates : MyPoint
    {
        //_longitude >=0  -> E  
        //_longitude < 0  -> W
        //_latitude   >=0  -> N
        //_latitude   < 0  -> S

        private double _longitude;
        private double _latitude;
        private char _longHem;
        private char _latHem;

        public Coordinates(double x, double y, int id) : base(x, y, id)
        {
            _longitude = x;
            _latitude = y;
            LongHem = (Longitude >= 0) ? 'E' : 'W';
            LatHem = (Latitude >= 0) ? 'N' : 'S';
        }

        public double Longitude { get => _longitude; set => _longitude = value; }
        public double Latitude { get => _latitude; set => _latitude = value; }
        public char LongHem { get => _longHem; set => _longHem = value; }
        public char LatHem { get => _latHem; set => _latHem = value; }
    }
}
