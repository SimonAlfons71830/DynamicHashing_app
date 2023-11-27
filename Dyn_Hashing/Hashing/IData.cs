using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree.Hashing
{
    public interface IData<T> : IRecord<T> 
    {
        //my equals method
        public bool MyEquals(T other);
        //returns a BitSet
        public BitArray getHash();
        //create an instance of a class
        public T createInstanceOfClass();

    }
}
