using QuadTree.Hashing;
using System.Collections;
using System.Text;

namespace QuadTree.GeoSystem
{
    public class Property : IData<Property>
    {
        //public int _registerNumber { get; set; }
        public (Coordinates x, Coordinates y) suradnice { get; set; }
        public string _description;
        public List<int> _lands_ids;
        public int _registerNumber;

        private const int MAX_DESC_LENGTH = 15;
        private const int MAX_RECORDS_OF_PLOTS = 6;

        /// <summary>
        /// new constructor for SEM2
        /// </summary>
        /// <param name="registerNumber"></param>
        /// <param name="description"></param>
        /// <param name="coordinates"></param>
        /// <param name="lands"></param>
        public Property(int registerNumber, string description, (Coordinates x, Coordinates y) coordinates, List<int> lands_ids)
           : base(registerNumber, coordinates)
        {
            RegisterNumber = registerNumber;
            Description = ShortenDesc(description);
            Coordinates = coordinates;
            Lands_ids = lands_ids.Take(MAX_RECORDS_OF_PLOTS).ToList();
        }

        public Property()
            : this(0, "", (new Coordinates(0, 0, -1), new Coordinates(0, 0, -1)), new List<int>())
        {
            //empty inicialization constructor
        }

        private string ShortenDesc(string desc) 
        {
            if (desc.Length <= MAX_DESC_LENGTH)
            {
                return desc;
            }
            else
            {
                return desc.Substring(0, MAX_DESC_LENGTH);
            }
        }

        public int RegisterNumber { get => _registerNumber; set => _registerNumber = value; }
        public string Description { get => _description; set => _description = ShortenDesc(value); }
        
        internal (Coordinates x, Coordinates y) Coordinates { get => suradnice; set => suradnice = value; }
        internal List<int> Lands_ids { get => _lands_ids; set => _lands_ids = value.Take(MAX_RECORDS_OF_PLOTS).ToList(); }
        
        public bool MyEquals(Property other)
        {
            return this.RegisterNumber.Equals(other.RegisterNumber);
        }

        public BitArray getHash()
        {
            //default, UTF8, ...
            byte[] regnumberChars = Encoding.Default.GetBytes(this._registerNumber.ToString());
            var bitarr = new BitArray(regnumberChars);
            return bitarr;

            //return new BitArray(Encoding.Default.GetBytes(_registerNumber.ToString()));
        }

        public int getSize()
        {
            throw new NotImplementedException();
        }

        public byte[] toByteArray()
        {
            throw new NotImplementedException();
        }

        public void fromByteArray(byte[] byteArray)
        {
            throw new NotImplementedException();
        }

        public Property createInstanceOfClass()
        {
            return new Property();
        }
    }
}
