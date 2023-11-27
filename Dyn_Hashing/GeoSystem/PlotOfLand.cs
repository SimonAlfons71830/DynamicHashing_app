using QuadTree.Hashing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree.GeoSystem
{
    public class PlotOfLand : IData<Property>
    {
        public string _description;
        public (Coordinates startPos, Coordinates endPos) _coordinates;
        public List<Property> _properties;
        public int _registerNumber;

        public PlotOfLand(int registerNumber, string description, (Coordinates startPos, Coordinates endPos) coordinates, List<Property> properties)
        {
            RegisterNumber = registerNumber;
            Description = description;
            Coordinates = coordinates;
            Properties = properties;
        }

        public int RegisterNumber { get => _registerNumber; set => _registerNumber = value; }
        public string Description { get => _description; set => _description = value; }
        internal (Coordinates startPos, Coordinates endPos) Coordinates { get => _coordinates; set => _coordinates = value; }
        internal List<Property> Properties { get => _properties; set => _properties = value; }

        public bool MyEquals(Property other)
        {
            return this.RegisterNumber.Equals(other.RegisterNumber);
        }

        public void fromByteArray(byte[] byteArray)
        {
            throw new NotImplementedException();
        }

        public BitArray getHash()
        {
            return new BitArray(Encoding.Default.GetBytes(_registerNumber.ToString()));
        }

        public int getSize()
        {
            throw new NotImplementedException();
        }

        public byte[] toByteArray()
        {
            throw new NotImplementedException();
        }

        public Property createInstanceOfClass()
        {
            throw new NotImplementedException();
        }
    }
}
