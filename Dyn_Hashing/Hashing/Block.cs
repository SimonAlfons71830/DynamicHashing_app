using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuadTree.Hashing
{
    public class Block<T> : IRecord<T> where T : IData<T>
    {
        //records 
        private List<T> _records;
        //valid records count (non valid are fillind the address)
        private int _validRecordsCount;
        //information of class type ?? Plot/Property
        private Type _type;
        //number of blocks for the AL
        private int _bf;


        public Block(int blockFactor) 
        {
            _bf = blockFactor;
            this._type = typeof(T);
            this._records = new List<T>(_bf);

            for (int i = 0; i < _bf; i++)
            {
                var record = (T)Activator.CreateInstance(_type);
                this._records.Add(record) ;
            }

            _validRecordsCount = 0;
        }


        public void fromByteArray(byte[] byteArray)
        {
            throw new NotImplementedException();
        }

        public int getSize()
        {
            throw new NotImplementedException();
        }

        public byte[] toByteArray()
        {
            throw new NotImplementedException();
        }
    }
}
