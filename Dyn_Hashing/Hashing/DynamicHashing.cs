using QuadTree.Trie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree.Hashing
{
    internal class DynamicHashing
    {
        private Node _root;
        private int _blockFactor;
        private FileStream _file;

        public DynamicHashing(String fileName,int blockFactor)
        {
            _blockFactor = blockFactor;

            try
            {
                // Open or create the file with Read-Write access
                _file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (FileNotFoundException e)
            {
                throw new InvalidOperationException("Error in Hashing: File not found.", e);
            }
            catch (IOException e)
            {
                throw new InvalidOperationException("Error in Hashing: IO exception.", e);
            }
        }
    }
}
