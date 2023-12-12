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
    public class PlotOfLand : Polygon, IData<PlotOfLand>
    {
        private int _registerNumber;
        private ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) _coordinates; //?? prerobit
        private List<int> _properties;
        private string _description;

        private int validCharsInDescription;

        private const int MAX_DESC_LENGTH = 11;
        private const int MAX_PROPERTIES_COUNT = 5;

        /// <summary>
        /// Constructor with input parameters to create object
        /// </summary>
        /// <param name="registerNumber"></param>
        /// <param name="description"></param>
        /// <param name="coordinates"></param>
        /// <param name="properties"></param>
        public PlotOfLand(int registerNumber, string description, ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinates, List<int> properties)
            :base(registerNumber, coordinates)
        {
            RegisterNumber = registerNumber;
            Description = EditDescription(description);
            Properties = properties;
            Coordinates = coordinates;
        }

        /// <summary>
        /// Constructor to initialize Instance
        /// </summary>
        public PlotOfLand() : this(0, "", ((0, 0), (0, 0)), new List<int>())
        {
        }

        

        private string EditDescription(string desc)
        {
            if (desc.Length <= MAX_DESC_LENGTH)
            {
                validCharsInDescription = desc.Length;
                //will add '*' to the length of 15
                return desc.PadRight(MAX_DESC_LENGTH, '*');
            }
            else
            {
                validCharsInDescription = MAX_DESC_LENGTH;
                //will shorten the description
                return desc.Substring(0, MAX_DESC_LENGTH);
            }
        }

        public bool MyEquals(PlotOfLand other)
        {
            return RegisterNumber.Equals(other.RegisterNumber);
        }

        public BitArray getHash(int count)
        {
            byte[] hash = Encoding.UTF8.GetBytes(RegisterNumber.ToString());
            var bitArray = new BitArray(hash);

            // Ensure that the BitArray has at least 'count' bits
            if (bitArray.Length >= count)
            {
                bool[] truncatedBits = new bool[count];
                for (int i = 0; i < count; i++)
                {
                    truncatedBits[i] = bitArray[i];
                }
                return new BitArray(truncatedBits);
            }

            // If the BitArray has fewer bits than 'count', return the entire BitArray
            return bitArray;
        }

        public PlotOfLand createInstanceOfClass()
        {
            return new PlotOfLand();
        }

        public int getSize()
        {
            int size = 0;
            //register number 4
            size += sizeof(int);
            //coordinates => 4*double (4*8)
            size += 4 * sizeof(double);
            //description (max length + 1 as null terminator)
            size += MAX_DESC_LENGTH + 1;
            //validCharsInDesc
            size += sizeof(int);
            //lands (count of records in list + size of each record -> int)
            size += sizeof(int) + MAX_PROPERTIES_COUNT * sizeof(int);
            //80B
            return size;
        }

        public byte[] toByteArray()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(RegisterNumber);

                byte[] descriptionBytes = Encoding.Default.GetBytes(Description);
                writer.Write((byte)descriptionBytes.Length);  // Store the length of the description

                writer.Write(descriptionBytes);
                writer.Write(validCharsInDescription);

                writer.Write(Coordinates.Item1.LongitudeStart);
                writer.Write(Coordinates.Item1.LatitudeStart);
                writer.Write(Coordinates.Item2.LongitudeEnd);
                writer.Write(Coordinates.Item2.LatitudeEnd);

                // Write the number of properties_ids and then each element
                writer.Write(Properties.Count);
                foreach (int landId in Properties)
                {
                    writer.Write(landId);
                }

                return stream.ToArray();
            }
        }

        public void fromByteArray(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                RegisterNumber = reader.ReadInt32();

                byte descriptionLength = reader.ReadByte();
                byte[] descriptionBytes = reader.ReadBytes(descriptionLength);
                Description = Encoding.UTF8.GetString(descriptionBytes);

                int validChars = reader.ReadInt32();

                //Description = Description.Substring(0, validChars);

                double startLongitude = reader.ReadDouble();
                double startLatitude = reader.ReadDouble();
                double endLongitude = reader.ReadDouble();
                double endLatitude = reader.ReadDouble();
                Coordinates = ((startLongitude, startLatitude), (endLongitude, endLatitude));

                int propertiesCount = reader.ReadInt32();
                Properties = new List<int>(propertiesCount);
                Properties.Clear();
                for (int i = 0; i < propertiesCount; i++)
                {
                    int propId = reader.ReadInt32();
                    Properties.Add(propId);
                }
            }
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

        public List<int> Properties
        {
            get => _properties;
            set
            {
                if (value==null)
                {
                    value = new List<int>();
                }
                // Take the first 6 records from the input list or fill with -1 if not full
                _properties = value.Take(MAX_PROPERTIES_COUNT).Concat(Enumerable.Repeat(-1, MAX_PROPERTIES_COUNT- value.Count)).ToList();
            }
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"\n\tRegisterNumber: {RegisterNumber}, ");
            sb.Append($"\n\tDescription: {Description}, ");
            sb.Append($"\n\tCoordinates: ({Coordinates.Item1.LongitudeStart}, {Coordinates.Item1.LatitudeStart}), ");
            sb.Append($"\n\t({Coordinates.Item2.LongitudeEnd}, {Coordinates.Item2.LatitudeEnd}), ");
            sb.Append("\n\tLands: [");

            foreach (int propId in Properties)
            {
                sb.Append($"{propId}, ");
            }

            sb.Append("]");

            return sb.ToString();
        }


    }
}
