using QuadTree.Hashing;
using QuadTree.Structures;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Dynamic_Hash.Objects
{
    public class Property : Polygon, IData<Property>
    {
        private int _registerNumber;
        private ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) _coordinates; //?? prerobit
        private List<int> _lands;
        private string _description;
        private int validCharsInDesc;

        private const int MAX_DESC_LENGTH = 15;
        private const int MAX_LANDS_COUNT = 6;
        

        /// <summary>
        /// Constructor with input parameters to create object
        /// </summary>
        /// <param name="registerNumber"></param>
        /// <param name="description"></param>
        /// <param name="coordinates"></param>
        /// <param name="lands"></param>
        public Property(int registerNumber, string description, ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinates, List<int> lands) 
            :base (registerNumber, coordinates)
        {
            RegisterNumber = registerNumber;
            Description = EditDescription(description);
            Lands = lands;
            Coordinates = coordinates;
        }

        /// <summary>
        /// Constructor to initialize Instance
        /// </summary>
        public Property() : this(0,"",((0,0),(0,0)),new List<int>())
        { 
        }

        private string EditDescription(string desc)
        {
            if (desc.Length < MAX_DESC_LENGTH)
            {
                validCharsInDesc = desc.Length;
                //will add '*' to the length of 15
                return desc.PadRight(MAX_DESC_LENGTH, '*');
            }
            else
            {
                //will shorten the description
                validCharsInDesc = MAX_DESC_LENGTH;
                return desc.Substring(0, MAX_DESC_LENGTH);
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

        public List<int> Lands
        {
            get => _lands;
            set 
            {
                if (value==null)
                {
                    value = new List<int>();
                }
                // Take the first 6 records from the input list or fill with -1 if not full
                _lands = value.Take(MAX_LANDS_COUNT).Concat(Enumerable.Repeat(-1, MAX_LANDS_COUNT - value.Count)).ToList();
            }
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public bool MyEquals(Property other)
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

        /*public BitArray getHash(int depth)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.Default.GetBytes(RegisterNumber.ToString());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Calculate the number of bytes needed to represent the specified bit count
                int byteCount = (depth + 7) / 8;

                // Truncate the hash to the specified number of bytes
                byte[] truncatedHash = new byte[byteCount];
                Array.Copy(hashBytes, truncatedHash, Math.Min(byteCount, hashBytes.Length));

                // Convert the truncated hash to a BitArray
                BitArray truncatedBitArray = new BitArray(truncatedHash);

                // If the BitArray has more bits than required, truncate it
                if (truncatedBitArray.Count > depth)
                {
                    for (int i = depth; i < truncatedBitArray.Count; i++)
                    {
                        truncatedBitArray[i] = false;
                    }
                }

                return truncatedBitArray;
            }
        }*/



        public Property createInstanceOfClass()
        {
            return new Property();
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
            //valid chars in string
            size += sizeof(int);
            //lands (count of records in list + size of each record -> int)
            size += sizeof(int) + MAX_LANDS_COUNT * sizeof(int);
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
                writer.Write(validCharsInDesc);

                writer.Write(Coordinates.Item1.LongitudeStart);
                writer.Write(Coordinates.Item1.LatitudeStart);
                writer.Write(Coordinates.Item2.LongitudeEnd);
                writer.Write(Coordinates.Item2.LatitudeEnd);

                // Write the number of properties_ids and then each element
                writer.Write(Lands.Count);
                foreach (int landId in Lands)
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
                
                int validcharsinDesc = reader.ReadInt32();

                //Description = Description.Substring(0,validcharsinDesc);

                double startLongitude = reader.ReadDouble();
                double startLatitude = reader.ReadDouble();
                double endLongitude = reader.ReadDouble();
                double endLatitude = reader.ReadDouble();
                Coordinates = ((startLongitude,startLatitude), (endLongitude,endLatitude));

                int landsCount = reader.ReadInt32();
                Lands = new List<int>(landsCount);
                Lands.Clear();
                for (int i = 0; i < landsCount; i++)
                {
                    int plotId = reader.ReadInt32();
                    Lands.Add(plotId);
                }
            }
        }

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"\n\tRegisterNumber: {RegisterNumber}, ");
            sb.Append($"\n\tDescription: {Description}, ");
            sb.Append($"\n\tCoordinates: ({Coordinates.Item1.LongitudeStart}, {Coordinates.Item1.LatitudeStart}), ");
            sb.Append($"\n\t({Coordinates.Item2.LongitudeEnd}, {Coordinates.Item2.LatitudeEnd}), ");
            sb.Append("\n\tLands: [");

            foreach (int landId in Lands)
            {
                sb.Append($"{landId}, ");
            }

            sb.Append("]");

            return sb.ToString();
        }

        
    }
}
