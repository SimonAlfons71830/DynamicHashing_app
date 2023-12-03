using System.Collections;

namespace QuadTree.Hashing
{
    public interface IData<T> : IRecord<T>
    {
        //my equals method
        public bool MyEquals(T other);
        //returns a BitSet
        public BitArray getHash(int count);
        //create an instance of a class
        public T createInstanceOfClass();

    }
}
