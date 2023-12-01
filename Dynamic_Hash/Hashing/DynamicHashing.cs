using Dynamic_Hash.Trie;
using QuadTree.Hashing;
using System.Collections;
using System.ComponentModel.DataAnnotations;
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
        private List<int> availableIndexes;

        public int noOfRecords;

        public DynamicHashing(string fileName, int blockFactor) 
        {
            BlockFactor = blockFactor;
            Trie = new Trie.Trie();
            availableIndexes = new List<int>();

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
        public List<int> AvailableIndexes { get => availableIndexes; set => availableIndexes = value; }

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

                    
                    if (availableIndexes.Count > 0)
                    {
                        var newIndex = availableIndexes.ElementAt(0);
                        availableIndexes.RemoveAt(0);
                        node.Index = newIndex;
                    }
                    else
                    {
                        //nastavenie adresy na koniec suboru
                        var newIndex = File.Length;
                        File.SetLength(File.Length + block.getSize());
                        node.Index = (int)newIndex;
                    }


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
                        Trace.WriteLine("Written to a new address." + block.ToString());
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
                            Trace.WriteLine("Trie if full.");
                            return false;
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
                                Trace.WriteLine("Written to a new address." + blockLeftSon.ToString());
                                Trace.WriteLine("Written to a new address." + blockRightSon.ToString());
                                LeftSon.CountOfRecords = blockLeftSon.ValidRecordsCount;
                                RightSon.CountOfRecords = blockRightSon.ValidRecordsCount;

                            }
                            else if (LeftSon.CountOfRecords > 0)
                            {
                                LeftSon.Index = oldFreeIndex;
                                WriteBackToFile(LeftSon.Index, blockLeftSon);
                                noOfRecords++;
                                Trace.WriteLine("Written to a new address." + blockLeftSon.ToString());
                                LeftSon.CountOfRecords = blockLeftSon.ValidRecordsCount;
                            }
                            else if (RightSon.CountOfRecords > 0)
                            {
                                RightSon.Index = oldFreeIndex;
                                WriteBackToFile(RightSon.Index, blockRightSon);
                                noOfRecords++;
                                Trace.WriteLine("Written to a new address." + blockRightSon.ToString());
                                RightSon.CountOfRecords = blockRightSon.ValidRecordsCount;
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

        private void ShortenBranch(ExternalNode node, ExternalNode brotherNode) 
        {
            var end = false;
            while (end)
            {

                if (end)
                {
                    return;
                }
                ExternalNode newNode = new ExternalNode(-1, 0);

                var newBlock = new Block<T>(BlockFactor);

                if (node.Index > -1)
                {
                    var nodeBlock = ReadBlockFromFile(node.Index);
                    for (int i = 0; i < nodeBlock.ValidRecordsCount; i++)
                    {
                        //TODO: if
                        if (newBlock.Insert(nodeBlock.Records[i]))
                        {
                            newNode.CountOfRecords++;
                        }
                        else
                        {
                            Trace.WriteLine("Error when shortening the branch.");
                        }

                    }

                    availableIndexes.Add(node.Index);
                    node.Index = -1;

                }

                if (brotherNode.Index > -1)
                {
                    var brotherBlock = ReadBlockFromFile(brotherNode.Index);
                    for (int i = 0; i < brotherBlock.ValidRecordsCount; i++)
                    {
                        if (newBlock.Insert(brotherBlock.Records[i]))
                        {
                            newNode.CountOfRecords++;
                        }
                        else
                        {
                            Trace.WriteLine("Error when shortening the branch.");
                        }


                    }
                    availableIndexes.Add(brotherNode.Index);
                    brotherNode.Index = -1;
                }

                //TODO: sort
                var newIndex = availableIndexes.ElementAt(0);
                availableIndexes.RemoveAt(0);


                var Parent = node.Parent;

                newNode.Parent = Parent;
                if (Parent.Parent.RightNode == Parent)
                {
                    Parent.Parent.RightNode = newNode;
                }
                else
                {
                    Parent.Parent.LeftNode = newNode;
                }

                newNode.Index = newIndex;

                WriteBackToFile(newNode.Index, newBlock);

                brotherNode = Trie.findBrother(newNode);
                if (brotherNode != null)
                {
                    node = newNode;
                }
                else
                {
                    end = true;
                }
            }
        }

        public bool Remove(T data) 
        {

            var pom = Find(data);
            if (pom == null)
            {
                Trace.WriteLine("Data not in file! - Error Remove");
            }


            int level = -1;
            var dataHash = data.getHash();
            var node = Trie.getExternalNode(dataHash, out level);

            var block = ReadBlockFromFile(node.Index);
            if (block.Remove(data))
            {
                Trace.WriteLine("succesfully removed:" + data.ToString());
                node.CountOfRecords--;
                noOfRecords--;
                WriteBackToFile(node.Index, block);

                if (node.CountOfRecords == 0 )
                {
                    var brotherNode = Trie.findBrother(node);

                    if (brotherNode != null)
                    {
                        if (brotherNode.CountOfRecords + node.CountOfRecords <= BlockFactor && node.Parent != Trie.Root)
                        {
                            ShortenBranch(node, brotherNode);
                        }
                        else
                        {
                            AvailableIndexes.Add(node.Index);
                            node.Index = -1;
                        }
                    }
                    else
                    {
                        AvailableIndexes.Add(node.Index);
                        node.Index = -1;
                    }
                }

                return true;


            }
            else
            {
                Trace.WriteLine("Error when removing data.");
                return false;
            }



           /* if (node != null)
            {
                if (node.Parent == Trie.Root)
                {
                    //???
                    
                }


                var block = this.ReadBlockFromFile(node.Index);

                if (block.Remove(data))
                {
                    node.CountOfRecords--;
                    //WriteBackToFile(node.Index, block);
                    noOfRecords--;

                    var brother = Trie.findBrother(node);

                    //if the brother is null, the node is root

                    if (brother != null)
                    {
                        //node has parrent
                        if (node.CountOfRecords + brother.CountOfRecords <= BlockFactor)
                        {
                            var nodeIndex = node.Index;
                            var brotherIndex = brother.Index;
                            var newIndex = -1;

                            if (brother.Index != -1)
                            {
                                newIndex = nodeIndex > brotherIndex ? nodeIndex : brotherIndex;
                            }
                            else
                            {
                                newIndex = nodeIndex;
                            }

                            //merge sons to parent and create external node from parent
                            ExternalNode newNode = new ExternalNode(newIndex, node.CountOfRecords + brother.CountOfRecords);
                            //brother and node will be deleted and Parent will become newNode

                            if (node.Parent.Parent == null) //node.parent is root
                            {

                            }
                            else
                            {
                                if (node.Parent.Parent.LeftNode == node.Parent)
                                {
                                    node.Parent.Parent.LeftNode = newNode;
                                }
                                else //node.parent.parent.rightnode
                                {
                                    node.Parent.Parent.RightNode = newNode;
                                }
                                //now the node, brother and parent are replaced
                            }

                            //need to find block from the brother and get it from file
                            //need to find block from the node and get it from file
                            //remove things from the blocks and save it into new one
                            //control the adress if its from the end then shorten the file

                            var newBlock = new Block<T>(BlockFactor);

                            var brothersBlock = new Block<T>(BlockFactor);

                            if (brother.Index != -1)
                            {
                                brothersBlock = ReadBlockFromFile(brother.Index);
                                foreach (var rec in brothersBlock.Records)
                                {
                                    if (newBlock.Insert(rec))
                                    {
                                        newNode.CountOfRecords++;
                                    }
                                    else
                                    {
                                        Trace.WriteLine("Error when inserting records in Remove. line:359");
                                    }
                                    brothersBlock.Remove(rec);

                                    if (brotherIndex == (File.Length - BlockSize))
                                    {
                                        //its the last block
                                        // do not need to rewrite, just delete from end
                                        File.SetLength(brother.Index);
                                        //TODO: controll the block before and before
                                    }
                                    else
                                    {
                                        //i do not have to write back i just need to store new index in available indexes

                                        WriteBackToFile(brotherIndex, brothersBlock);
                                    }
                                    
                                }
                            }
                            else
                            {
                                brothersBlock = null;
                            }

                            foreach (var rec in block.Records)
                            {
                                if (newBlock.Insert(rec))
                                {
                                    newNode.CountOfRecords++;
                                }
                                else
                                {
                                    Trace.WriteLine("Error when inserting records in Remove. line:376");
                                }
                                block.Remove(rec);

                                if (nodeIndex == (File.Length - BlockSize))
                                {
                                    //its the last block
                                    // do not need to rewrite, just delete from end
                                    File.SetLength(nodeIndex);
                                    //TODO: controll the block before and before
                                }
                                else
                                {
                                    //writes back the deleted file
                                    WriteBackToFile(nodeIndex, block);
                                }

                            }

                            



                        }
                    }
                }
                else
                {
                    Trace.WriteLine("Did not removed data.");
                }
            }*/
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
                    Trace.WriteLine("Item not found." + data.ToString());
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
