using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamic_Hash.Trie
{
    internal class Trie
    {
        public InternalNode? _root;

        public Trie()
        {
            //_root = new ExternalNode(0,-1); // firstly the _root will be node holding records and address

            //old
            Root = new InternalNode(); //cant switch from internal to external so 2 ext nodes will be created
            Root.RightNode = new ExternalNode(-1, 0);
            Root.RightNode.Parent = Root;

            Root.LeftNode = new ExternalNode(-1, 0);
            Root.LeftNode.Parent = Root;
        }
        public InternalNode Root
        {
            get { return _root; }
            set { _root = value; }
        }

        public ExternalNode? getExternalNode(BitArray bitset, out int level) 
        {
           /* Node pomnode = Root;
            for (int i = 0; i < bitset.Count; i++)
            {
                if (bitset[i]) // 1
                {
                    //if type is external the search is at the end
                    if (pomnode is ExternalNode)
                    {
                        level = i;
                        return (ExternalNode?)pomnode;
                    }
                    else
                    {
                        pomnode = ((InternalNode)pomnode).RightNode;
                    }
                }
                else //0
                {
                    //if type is external the search is at the end
                    if (pomnode is ExternalNode)
                    {
                        level = i;
                        return (ExternalNode?)pomnode;
                    }
                    else
                    {
                        pomnode = ((InternalNode)pomnode).LeftNode;
                    }
                }
            }

            //if not found
            level = -1;
            Trace.WriteLine("Did not find external node by bitset. ERR");
            return null;*/

            Node currentNode = Root;

            for (int i = 0; i < bitset.Count; i++)
            {
                if (currentNode is ExternalNode externalNode)
                {
                    level = i-1;
                    return externalNode;
                }

                currentNode = bitset[i] ? ((InternalNode)currentNode).RightNode : ((InternalNode)currentNode).LeftNode;
            }

            level = -1;
            Trace.WriteLine("Did not find an external node by bitset. ERR");
            return null;

        }

    }
}
