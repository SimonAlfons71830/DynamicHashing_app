using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamic_Hash.Trie
{
    internal class Node
    {
        private InternalNode? _parent;

        public Node()
        { 
            _parent = null;
        }

        public InternalNode Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

    }
}
