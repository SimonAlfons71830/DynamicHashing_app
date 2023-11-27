
namespace QuadTree.Hashing
{
    public interface IRecord<T>
    {
        //get a size of bytes that are going to be saved in file
        public int getSize();
        //writes object to array of bytes
        public byte[] toByteArray();
        //reads object from array of bytes
        public void fromByteArray(byte[] byteArray);

    }
}
