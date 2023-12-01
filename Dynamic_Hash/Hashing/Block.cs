using QuadTree.Hashing;
using System;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dynamic_Hash.Hashing
{
    public class Block<T> : IRecord<T> where T : IData<T>
    {
        //list of records stored
        private List<T> _records;
        //valid records count (non valid are created from empty constructor)
        private int _validRecordsCount;
        //information about class type stored in Block
        private Type _type;
        //number of blocks for the List
        private int _bf;

        public Block(int blockFactor)
        {
            BlockFactor = blockFactor;
            TypeOfData = typeof(T);
            Records = new List<T>(BlockFactor);
            ValidRecordsCount = 0;

            for (int i = 0; i < BlockFactor; i++)
            {
                Records.Add((T)Activator.CreateInstance(TypeOfData));
            }
        }

        public List<T> Records
        {
            get => _records;
            set => _records = value;
        }

        public int ValidRecordsCount
        {
            get => _validRecordsCount;
            set => _validRecordsCount = value;
        }

        public Type TypeOfData
        {
            get => _type;
            set => _type = value;
        }

        public int BlockFactor
        {
            get => _bf;
            set => _bf = value;
        }

        public bool Insert(T record)
        {
            //TODO : check for duplicates
            if (ValidRecordsCount < BlockFactor)
            {
                Records[ValidRecordsCount++] = record;
                return true;
            }
            return false;
        }

        public bool Remove(T data) 
        {
            for (int i = 0; i < ValidRecordsCount; i++)
            {
                if (Records.ElementAt(i).MyEquals(data))
                {
                    // Swap the found record with the last valid record in the list
                    T temp = Records[i];
                    Records[i] = Records[ValidRecordsCount - 1];
                    Records[ValidRecordsCount - 1] = temp;
                    //TODO: maybe rewrite the data for empty block (optional)

                    // Decrease the count of valid records
                    ValidRecordsCount--;

                    return true; // Indicate that a record was moved to the end of the list
                }
            }
            return false; // Indicate that the record was not found

        }

        public void fromByteArray(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.Default, true))
            {
                ValidRecordsCount = reader.ReadInt32();
                Records.Clear();

                for (int i = 0; i < BlockFactor; i++)
                {
                    T record = Activator.CreateInstance<T>();
                    record.fromByteArray(reader.ReadBytes(record.getSize()));
                    Records.Add(record);
                }
            }
        }

        public int getSize()
        {
            var size = 0;
            //valid records count - int 4B
            size += sizeof(int);
            //size of the record * bf
            size += Activator.CreateInstance<T>().getSize() * BlockFactor;

            return size;

        }

        public byte[] toByteArray()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(ValidRecordsCount);

                foreach (T record in Records)
                {
                    //records has own method implemented
                    writer.Write(record.toByteArray());
                }

                // Get the byte array from the stream
                return stream.ToArray();
            }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{ValidRecordsCount}/{BlockFactor} records,");

            for (int i = 0; i < ValidRecordsCount; i++)
            {

                T record = Records[i];
                sb.Append($" record{i + 1}: {record.ToString()}, ");
            }

            sb.Append("]");

            return sb.ToString();
        }

    }
}
