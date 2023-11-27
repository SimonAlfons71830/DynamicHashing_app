using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamic_Hash.Trie
{
    internal class ExternalNode : Node
    {
        //address of the record in file
        private int _index;
        //count of records 
        private int _countOfRecords;

        public ExternalNode(int index, int countOfRecords)
        {
            Index = index;
            CountOfRecords = countOfRecords;
        }

        public int CountOfRecords
        {
            get { return _countOfRecords; }
            set { _countOfRecords = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
    }
}
