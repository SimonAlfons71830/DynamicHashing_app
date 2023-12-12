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
        public Node? _root;
        //public InternalNode? _root;

        public Trie()
        {
            _root = new ExternalNode(-1, 0); // firstly the _root will be node holding records and address

            //old
            /*Root = new InternalNode(); //cant switch from internal to external so 2 ext nodes will be created
            Root.RightNode = new ExternalNode(-1, 0);
            Root.RightNode.Parent = Root;

            Root.LeftNode = new ExternalNode(-1, 0);
            Root.LeftNode.Parent = Root;*/
        }
        /*public InternalNode Root
        {
            get { return _root; }
            set { _root = value; }
        }*/

        public Node Root
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

            for (int i = 0; i <= bitset.Count; i++)
            {
                if (currentNode is ExternalNode externalNode)
                {
                    level = i - 1;
                    return externalNode;
                }

                currentNode = bitset[i] ? ((InternalNode)currentNode).RightNode : ((InternalNode)currentNode).LeftNode;
            }

            level = -1;
            Trace.WriteLine("Did not find an external node by bitset. ERR");
            return null;

        }

        public ExternalNode findBrother(ExternalNode node)
        {
            if (node.Parent != null) //is root
            {
                if (node.Parent.RightNode == node) //Right son i need to return left son
                {
                    if (node.Parent.LeftNode is ExternalNode)
                    {
                        return (ExternalNode)node.Parent.LeftNode;
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    if (node.Parent.RightNode is ExternalNode)
                    {
                        return (ExternalNode)node.Parent.RightNode;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            else
            {
                //cant find brother from root
                return null;
            }


        }

        public List<(ExternalNode node, BitArray bitset)> getLeaves()
        {
            var returnList = new List<(ExternalNode node, BitArray bitset)>();

            var helper = new List<(Node node, BitArray bitset)>();
            helper.Add((Root, new BitArray(0)));

            while (helper.Count > 0)
            {
                var pom = helper[0];
                helper.RemoveAt(0);

                if (pom.node is ExternalNode)
                {
                    returnList.Add(((ExternalNode node, BitArray bitset))(pom.node, pom.bitset));
                    continue;
                }
                if (((InternalNode)pom.node).LeftNode != null)
                {
                    // For left node, set the last bit to '1'
                    var bitsetForLeft = new BitArray(pom.bitset);
                    bitsetForLeft.Set(++bitsetForLeft.Length - 1, false);
                    helper.Add((((InternalNode)pom.node).LeftNode, bitsetForLeft));
                }
                if (((InternalNode)pom.node).RightNode != null) //rightnode
                {

                    // For right node, set the last bit to '0'
                    var bitsetForRight = new BitArray(pom.bitset);
                    bitsetForRight.Set(++bitsetForRight.Length - 1, true);
                    helper.Add((((InternalNode)pom.node).RightNode, bitsetForRight));
                }

            }
            return returnList;


        }

        public void ReadLeaves(string filepath)
        {
            foreach (string line in File.ReadLines(filepath))
            {
                string[] parts = line.Split(';');

                if (parts.Length == 3)
                {
                    string bitsetString = parts[0];
                    int nodeIndex = int.Parse(parts[1]);
                    int countOfRecords = int.Parse(parts[2]);

                    //lambda
                    BitArray bitset = new BitArray(bitsetString.Select(c => c == '1').ToArray());

                    ReconstructTrie(bitset, nodeIndex, countOfRecords);
                }
                else
                {
                    //invalid line format??
                }
            }
        }

        private void ReconstructTrie(BitArray bitset, int nodeIndex, int countOfRecords)
        {
            if (_root == null)
            {
                // Initialize the root if it's null (or handle differently based on your logic)
                _root = new ExternalNode(-1, 0);
            }

            Node currentNode = _root;

            for (int i = 0; i < bitset.Length; i++)
            {
                if (bitset[i])
                {
                    var internalNodeCurrent = new InternalNode();
                    if (currentNode is ExternalNode)//((InternalNode)currentNode).RightNode == null
                    {
                        // Create a new right node if it doesn't exist


                        
                        internalNodeCurrent.Parent = currentNode.Parent;

                        if (currentNode == _root)
                        {
                            //nothing with parent
                            _root = internalNodeCurrent;
                        }
                        else
                        {
                            if (currentNode.Parent.RightNode == currentNode)
                            {
                                internalNodeCurrent.Parent.RightNode = internalNodeCurrent;
                            }
                            else
                            {
                                internalNodeCurrent.Parent.LeftNode = internalNodeCurrent;
                            }
                        }

                        //current node needs to be new internal node and to set it parent right

                        internalNodeCurrent.RightNode = new ExternalNode(nodeIndex, countOfRecords);
                        internalNodeCurrent.RightNode.Parent = internalNodeCurrent;
                    }
                    else
                    {

                        if (((InternalNode)currentNode).RightNode == null)
                        {
                            ((InternalNode)currentNode).RightNode = new ExternalNode(nodeIndex, countOfRecords);
                            ((InternalNode)currentNode).RightNode.Parent = ((InternalNode)currentNode);
                            currentNode = ((InternalNode)currentNode).RightNode;
                            continue;
                        }
                        else
                        {
                            currentNode = ((InternalNode)currentNode).RightNode;
                            continue;
                        }
                    }
                    currentNode = internalNodeCurrent.RightNode;
                }
                else
                {
                    var internalNodeCurrent = new InternalNode();
                    if (currentNode is ExternalNode)//((InternalNode)currentNode).RightNode == null
                    {
                        // Create a new right node if it doesn't exist


                        internalNodeCurrent = new InternalNode();
                        internalNodeCurrent.Parent = currentNode.Parent;

                        if (currentNode == _root)
                        {
                            //nothing with parent
                            _root = internalNodeCurrent;
                        }
                        else
                        {
                            if (currentNode.Parent.RightNode == currentNode)
                            {
                                internalNodeCurrent.Parent.RightNode = internalNodeCurrent;
                            }
                            else
                            {
                                internalNodeCurrent.Parent.LeftNode = internalNodeCurrent;
                            }
                        }

                        //current node needs to be new internal node and to set it parent right

                        internalNodeCurrent.LeftNode = new ExternalNode(nodeIndex, countOfRecords);
                        internalNodeCurrent.LeftNode.Parent = internalNodeCurrent;
                    }
                    else
                    {
                        if (((InternalNode)currentNode).LeftNode == null)
                        {
                            ((InternalNode)currentNode).LeftNode = new ExternalNode(nodeIndex, countOfRecords);
                            ((InternalNode)currentNode).LeftNode.Parent = ((InternalNode)currentNode);
                            currentNode = ((InternalNode)currentNode).LeftNode;
                            continue;
                        }
                        else
                        {
                            currentNode = ((InternalNode)currentNode).LeftNode;
                            continue;
                        }
                    }
                    

                    currentNode = internalNodeCurrent.LeftNode;
                }
            }
        }


        public void SaveState(string filePath)
        {
            StringBuilder sb = new StringBuilder();
            var list = this.getLeaves();

            foreach (var leaf in list)
            {
                //leaf.bitset.ToString() 
                string bitset = "";

                for (int i = 0; i < leaf.bitset.Count; i++)
                {
                    if (leaf.bitset[i])
                    {
                        bitset += "1";
                    }
                    else
                    {
                        bitset += "0";
                    }
                }

                sb.Append(bitset + ";" + leaf.node.Index + ";" + leaf.node.CountOfRecords + "\n");
            }
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}

