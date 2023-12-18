using QuadTree.Hashing;
using QuadTree.Structures;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Dynamic_Hash.Objects
{
    public class PropToQuad : Polygon
    {
        private int _registerNumber;
        private ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) _coordinates;

        /// <summary>
        /// Constructor with input parameters to create object
        /// </summary>
        /// <param name="registerNumber"></param>
        /// <param name="description"></param>
        /// <param name="coordinates"></param>
        /// <param name="lands"></param>
        public PropToQuad(int registerNumber, ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinates)
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

        public bool MyEquals(PropToQuad other)
        {
            return RegisterNumber.Equals(other.RegisterNumber);
        }

    }
}
