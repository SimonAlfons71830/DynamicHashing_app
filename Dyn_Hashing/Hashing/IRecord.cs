using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree.Hashing
{
    public interface IRecord<T>
    {
        //get a size of bytes that are going to be saved in file
        public int getSize();
        public byte[] toByteArray();
        public void fromByteArray(byte[] byteArray);
        
    }
}
