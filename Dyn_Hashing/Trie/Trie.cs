using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadTree.Trie
{
    internal class Trie
    {
        private InternalNode? _root;

        public Trie()
        {
            Root = new InternalNode(); 
        }

        public InternalNode Root
        {
            get { return _root; }
            set { _root = value; }
        }

        /// <summary>
        /// returns a extrnal node based on the bitset given
        /// </summary>
        /// <param name="bitset"></param>
        /// <returns></returns>
        public ExternalNode? getExternalNode(BitArray bitset) 
        {
            var pomNode = Root;
            //go through bitset and according to actual data go right or left
            for (int i = 0; i < bitset.Count; i++)
            {
                if (bitset[i]) // == 1
                {
                    //if type is external node it is at the end
                    if (pomNode.RightNode is ExternalNode)
                    {
                        return (ExternalNode)pomNode.RightNode;
                    }
                    else
                    {
                        pomNode = (InternalNode)pomNode.RightNode;
                    }
                }
                else // == 0 leftNode
                {
                    if (pomNode.LeftNode is ExternalNode)
                    {
                        return (ExternalNode)pomNode.LeftNode;
                    }
                    else
                    {
                        pomNode = (InternalNode)pomNode.LeftNode;
                    }
                }
            }
            return null;
        }

        /*public ExternalNode? GetExternalNode2(BitArray bitset)
        {
            var currentNode = Root;

            foreach (bool bit in bitset)
            {
                if (bit)
                {
                    currentNode = (InternalNode)GetNextNode(currentNode, true);
                }
                else
                {
                    currentNode = (InternalNode)GetNextNode(currentNode, false);
                }

                if (currentNode is ExternalNode externalNode)
                {
                    return externalNode;
                }
            }

            return null;
        }


        private Node GetNextNode(Node currentNode, bool isRight)
        {
            return isRight ? ((InternalNode)currentNode)?.RightNode : ((InternalNode)currentNode)?.LeftNode;
        }*/
    }
}
