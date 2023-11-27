using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamic_Hash.Trie
{
    internal class InternalNode : Node
    {
        private Node _leftNode;
        private Node _rightNode;

        public InternalNode()
        {
            LeftNode = null;
            RightNode = null;
        }

        public Node LeftNode
        {
            get { return _leftNode; }
            set { _leftNode = value; }
        }

        public Node RightNode
        {
            get { return _rightNode; }
            set { _rightNode = value; }
        }
    }
}
