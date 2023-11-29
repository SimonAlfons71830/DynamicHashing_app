using Dynamic_Hash.Trie;
using QuadTree.Hashing;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace Dynamic_Hash.Hashing
{
    public class DynamicHashing<T> where T : IData<T>
    {
        private Trie.Trie _trie;
        private int _blockFactor;
        private FileStream _file;
        private int _blockSize;

        public int noOfRecords;

        public DynamicHashing(string fileName, int blockFactor) 
        {
            BlockFactor = blockFactor;
            Trie = new Trie.Trie();

            try
            {
                File = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error in Hashing: IO exception.", e);
            }

            noOfRecords = 0;
        }

        public int BlockFactor { get => _blockFactor; set => _blockFactor = value; }
        public FileStream File { get => _file; set => _file = value; }
        internal Trie.Trie Trie { get => _trie; set => _trie = value; }
        public int BlockSize
        {
            get
            {
                var block = new Block<T>(BlockFactor);
                return block.getSize();
            }
            set => _blockSize = value;
        }


        public bool Insert(T data) 
        {
            //aply hash function
            //find external node by hash function
            // 1. if extenalNode has no address alocate block and save the data there + set the address
            //writr to file

            // 2. if externalNode has address and data can fit there, read block from file add data there and write to same address

            // 3. if external node has address and data do not fit anymore:
            //externalNode -> internalNode
            //create 2 sons
            // alocate 2 block
            //try to move data from old external node and its block to next level of nodes
            //if it fits (<_bf) end 
            //if not repeat until it fits or until the hash id over

            int level = -1;
            BitArray hashData = data.getHash();
            var node = Trie.getExternalNode(hashData, out level);
            if (node != null)
            {
                //1.
                if (node.Index == -1) //no address yet
                {
                    Block<T> block = new Block<T>(BlockFactor);
                    block.Insert(data);

                    //nastavenie adresy na koniec suboru
                    var newIndex = File.Length;
                    File.SetLength(File.Length + block.getSize());
                    node.Index = (int)newIndex;

                    WriteBackToFile(node.Index, block);
                    noOfRecords++;

                    node.CountOfRecords++;
                    Trace.WriteLine("Written to a new address." + block.ToString());
                    return true;
                }

                //2.
                else if (node.Index != -1 && node.CountOfRecords < this.BlockFactor) //adress + space left
                {
                    var block = ReadBlockFromFile(node.Index);
                    if (block.Insert(data))
                    {
                        //write back to file on the same index
                        WriteBackToFile(node.Index, block);
                        noOfRecords++;
                        node.CountOfRecords++;
                        return true;
                    }
                    else
                    {
                        Trace.WriteLine("Error When inserting to existing block.");
                        return false;
                    }
                }

                //3.
                else if (node.CountOfRecords >= BlockFactor)
                {
                    var block = ReadBlockFromFile(node.Index);
                    bool end = false;

                    int oldFreeIndex = node.Index;
                    int newFreeIndex = (int)File.Length;
                    File.SetLength(File.Length + BlockSize);

                    while (node.CountOfRecords+1 > BlockFactor) //added a record to node
                    {
                        if (end)
                        {
                            return true;
                        }

                        //change node to internal node + add sons

                        InternalNode newNode = new InternalNode();
                        newNode.Parent = node.Parent;
                        if (node.Parent.RightNode == node) //is Right son
                        {
                            node.Parent.RightNode = newNode;
                        }
                        else
                        {
                            node.Parent.LeftNode = newNode;
                        }

                        ExternalNode LeftSon = new ExternalNode(-1, 0);
                        ExternalNode RightSon = new ExternalNode(-1, 0);

                        newNode.LeftNode = LeftSon;
                        newNode.RightNode = RightSon;
                        LeftSon.Parent = newNode;
                        RightSon.Parent = newNode;

                        //allocate block for each of the sons

                        Block<T> blockRightSon = new Block<T>(BlockFactor);
                        Block<T> blockLeftSon = new Block<T>(BlockFactor);

                        

                        bool continueRight = false;
                        bool continueLeft = false;   


                        level++; //level of current node + next position of the sons

                        foreach (var rec in block.Records)
                        {
                            var hash = rec.getHash();

                            if (hash.Count > level)
                            {
                                if (hash[level]) //==1 goes to right
                                {
                                    if (blockRightSon.Insert(rec))
                                    {
                                        RightSon.CountOfRecords++;
                                    }
                                    else
                                    {
                                        Trace.WriteLine("Error when inserting to right son after splitting full node.");
                                    }
                                }
                                else
                                {
                                    if (blockLeftSon.Insert(rec))
                                    {
                                        LeftSon.CountOfRecords++;
                                    }
                                    else
                                    {
                                        Trace.WriteLine("Error when inserting to Left son after splitting full node.");
                                    }
                                }
                            }
                            else
                            {
                                Trace.WriteLine("Hash is too short for next level.");
                            }
                        }

                        //also add a new data to left or right son
                        if (level < hashData.Count)
                        {
                            if (hashData[level]) //1
                            {
                                if (blockRightSon.Insert(data))
                                {
                                    RightSon.CountOfRecords++;
                                }
                                else
                                {
                                    //continue with this node
                                    continueRight = true;
                                    Trace.WriteLine("In Left son is no space left. Repeating the insert. Level: [" + level + "]");
                                }

                            }
                            else
                            {
                                if (blockLeftSon.Insert(data))
                                {
                                    LeftSon.CountOfRecords++;
                                }
                                else
                                {
                                    //continue with this node
                                    continueLeft = true;
                                    Trace.WriteLine("In Right son is no space left. Repeating the insert. Level: [" + level + "]");
                                }
                            }
                        }
                        else
                        {
                            Trace.WriteLine("Hash of new data too short for next level");
                        }

                        if (continueLeft)
                        {
                            node = LeftSon;
                        }
                        else if (continueRight)
                        {
                            node = RightSon;
                        }
                        else
                        {
                            end = true;
                            if (LeftSon.CountOfRecords > 0 && RightSon.CountOfRecords > 0)
                            {
                                LeftSon.Index = newFreeIndex;
                                //LeftSon.CountOfRecords = blockLeftSon.ValidRecordsCount;
                                RightSon.Index = oldFreeIndex;
                                //RightSon.CountOfRecords = blockRightSon.ValidRecordsCount;
                                WriteBackToFile(LeftSon.Index, blockLeftSon);
                                WriteBackToFile(RightSon.Index, blockRightSon);
                                noOfRecords++;
                            }
                            else if (LeftSon.CountOfRecords > 0)
                            {
                                LeftSon.Index = oldFreeIndex;
                                WriteBackToFile(LeftSon.Index, blockLeftSon);
                                noOfRecords++;
                            }
                            else if (RightSon.CountOfRecords > 0)
                            {
                                RightSon.Index = oldFreeIndex;
                                WriteBackToFile(RightSon.Index, blockRightSon);
                                noOfRecords++;
                            }

                            Trace.WriteLine("New Nodes have good amount of records, they have been written to file.");

                        }
                    }
                }
            }
            else
            {
                return false;
            }
            return false;

        }

        public Block<T> FindBlockByHash(BitArray hash)
        {
            var levelnot = -1;
            var node = _trie.getExternalNode(hash, out levelnot);
            if (node != null)
            {
                //nacitat block z file a returnut ho
                var blockFromFile = ReadBlockFromFile(node.Index);
                return blockFromFile;
            }
            return null;

        }
        public Block<T> ReadBlockFromFile(int index)
        {
            Block<T> block = new Block<T>(BlockFactor);
            byte[] bytes = new byte[block.getSize()];

            File.Seek(index, SeekOrigin.Begin);
            File.Read(bytes);

            if (File.Length < (index + bytes.Count()))
            {
                Trace.WriteLine("Reading more than File Length.");
            }

            block.fromByteArray(bytes);
            return block;
        }

        public void WriteBackToFile(int index, Block<T> block) 
        {
            File.Seek(index, SeekOrigin.Begin);
            File.Write(block.toByteArray());
        }

        public T Find(T data)
        {
            //find hash func of data
            //find external node
            //read from file from index of the node

            var level = -1;
            var node = Trie.getExternalNode(data.getHash(), out level);
            if (node != null)
            {

                if (node.CountOfRecords == 0)
                {
                    Trace.WriteLine("Node found by hash has no records.");
                    return default(T);
                }
                else
                {
                    var block = ReadBlockFromFile(node.Index);
                    for (int i = 0; i < block.Records.Count; i++)
                    {
                        if (i < block.ValidRecordsCount)
                        {
                            if (data.MyEquals(block.Records.ElementAt(i)))
                            {
                                return block.Records.ElementAt(i);
                            }
                        }
                    }
                    Trace.WriteLine("Item not found.");
                    return default(T);
                }
            }
            else
            {
                return default(T);
            }

        }

        /*public bool Remove(T data) 
        {
        
        }*/


    }
}
