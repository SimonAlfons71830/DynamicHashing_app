﻿using Dynamic_Hash.Objects;
using Dynamic_Hash.Trie;
using QuadTree.Hashing;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dynamic_Hash.Hashing
{
    public class DynamicHashing<T> where T : IData<T>
    {
        private Trie.Trie _trie;
        private int _blockFactor;
        private FileStream _file;
        private FileStream _fileOverflow;
        private int _blockSize;
        private int _blockSizeOf;
        //private List<int> availableIndexes;
        //private List<int> availableIndexesOverflow;
        private int _countHashFun;
        private int _blockFactorOverflow;

        private int _emptyBlocksIndex;
        private int _emptyBlocksIndexOverflow;

        public int noOfRecords;
        private string _fileName;
        private string _ofFileName;


        public DynamicHashing(string fileName, string OFfilename, int blockFactor, int blockFactorOverflow, int countOfHashFunc)
        {
            _fileName = fileName;
            _ofFileName = OFfilename;

            BlockFactor = blockFactor;
            Trie = new Trie.Trie();
            //availableIndexes = new List<int>();
            //availableIndexesOverflow = new List<int>();
            CountHashFun = countOfHashFunc;
            BlockFactorOverflow = blockFactorOverflow;
            EmptyBlocksIndex = -1;
            EmptyBlocksIndexOverflow = -1;

            //create main file
            try
            {
                File = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error in Hashing: IO exception.", e);
            }
            //create overflow File
            try
            {
                FileOverflow = new FileStream(OFfilename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
        public int CountHashFun { get => _countHashFun; set => _countHashFun = value; }
        //public List<int> AvailableIndexes { get => availableIndexes; set => availableIndexes = value; }
        public FileStream FileOverflow { get => _fileOverflow; set => _fileOverflow = value; }
        //public List<int> AvailableIndexesOverflow { get => availableIndexesOverflow; set => availableIndexesOverflow = value; }
        public int BlockFactorOverflow { get => _blockFactorOverflow; set => _blockFactorOverflow = value; }

        public int BlockSize
        {
            get
            {
                var block = new Block<T>(BlockFactor);
                return block.getSize();
            }
            set => _blockSize = value;
        }

        public int BlockSizeOF
        {
            get
            {
                var block = new Block<T>(BlockFactorOverflow);
                return block.getSize();
            }
            set => _blockSizeOf = value;
        }
        public int EmptyBlocksIndex { get => _emptyBlocksIndex; set => _emptyBlocksIndex = value; }
        public int EmptyBlocksIndexOverflow { get => _emptyBlocksIndexOverflow; set => _emptyBlocksIndexOverflow = value; }


        public int ReturnFreeAdress(bool main)
        {

            if (main)
            {

                if (EmptyBlocksIndex != -1)
                {
                    var index = EmptyBlocksIndex;
                    var block = ReadBlockFromFile(EmptyBlocksIndex, true);
                    if (block.ChainIndexAfter != -1)
                    {
                        var next = ReadBlockFromFile(block.ChainIndexAfter, true);
                        next.ChainIndexBefore = -1;
                        EmptyBlocksIndex = block.ChainIndexAfter;
                        WriteBackToFile(block.ChainIndexAfter, next, true);
                        return index;
                    }
                    else
                    {
                        //its first one in chaining

                        EmptyBlocksIndex = -1;
                        return index;
                    }
                }
                else
                {
                    //file length increase
                    //nastavenie adresy na koniec suboru
                    var newIndex = File.Length;
                    File.SetLength(File.Length + BlockSize);
                    return (int)newIndex;
                }

            }
            else
            {

                if (EmptyBlocksIndexOverflow != -1)
                {
                    var index = EmptyBlocksIndexOverflow;
                    var block = ReadBlockFromFile(EmptyBlocksIndexOverflow, false);
                    if (block.ChainIndexAfter != -1)
                    {
                        var next = ReadBlockFromFile(block.ChainIndexAfter, false);
                        next.ChainIndexBefore = -1;
                        EmptyBlocksIndexOverflow = block.ChainIndexAfter;
                        WriteBackToFile(block.ChainIndexAfter, next, false);
                        return index;
                    }
                    else
                    {
                        //its first one in chaining

                        EmptyBlocksIndexOverflow = -1;
                        return index;
                    }
                }
                else
                {
                    //file length increase
                    //nastavenie adresy na koniec suboru
                    var newIndex = FileOverflow.Length;
                    FileOverflow.SetLength(FileOverflow.Length + BlockSizeOF);
                    return (int)newIndex;
                }

            }
        }

        //TODO: dokumentacia - diagram tried + pri kazdej metode kolko krat pristupujem do suboru + popis algoritmu

        //TODO: pri inserte checknut ci sa tam uz nenachadza duplicitny kluc
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
            BitArray hashData = data.getHash(CountHashFun);
            var node = Trie.getExternalNode(hashData, out level);
            if (node != null)
            {
                //1.
                if (node.Index == -1) //no address yet
                {
                    Block<T> block = new Block<T>(BlockFactor);
                    if (block.Insert(data))
                    {

                        node.Index = ReturnFreeAdress(true);

                        WriteBackToFile(node.Index, block, true);
                        noOfRecords++;

                        node.CountOfRecords++;
                        Trace.WriteLine("Written to a new address." + block.ToString());
                        return true;
                    }
                    else
                    {
                        //ERR 
                        Trace.WriteLine("Error");
                    }
                }

                //2.
                else if (node.Index != -1 && node.CountOfRecords < this.BlockFactor) //adress + space left
                {
                    var block = ReadBlockFromFile(node.Index, true);
                    if (block.Insert(data))
                    {
                        //write back to file on the same index
                        WriteBackToFile(node.Index, block, true);
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
                    var block = ReadBlockFromFile(node.Index, true);

                    if (level >= CountHashFun - 1)
                    {
                        //look for block OfIndex
                        //if it has search for it until next bloeck does not have ofindex
                        //try to insert
                        //if yes -> end
                        //if not -> create new block and insert data + set index
                        AddToOverflowBlock(block, data);
                        node.CountOfRecords++;
                        WriteBackToFile(node.Index, block, true);
                        return true;

                    }
                    bool end = false;

                    //availableIndexes.Add(node.Index);
                    /*int oldFreeIndex = node.Index;
                    int newFreeIndex = (int)File.Length;
                    File.SetLength(File.Length + BlockSize);*/

                    while (node.CountOfRecords + 1 > BlockFactor) //added a record to node
                    {
                        if (end)
                        {
                            return true;
                        }

                        //change node to internal node + add sons

                        InternalNode newNode = new InternalNode();
                        
                        newNode.Parent = node.Parent;

                        //index from external node add to empty blocks management

                        if (node.Index != -1)
                        {
                            block.ValidRecordsCount = 0;
                            this.AddToEmptyBlock(node.Index, block, true);
                        }
                        

                        if (node != Trie.Root)
                        {
                            //i do make parents parent reference
                            if (node.Parent.RightNode == node) //is Right son
                            {
                                node.Parent.RightNode = newNode;
                            }
                            else
                            {
                                node.Parent.LeftNode = newNode;
                            }
                        }
                        else
                        {
                            Trie.Root = newNode;
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
                            var hash = rec.getHash(CountHashFun);

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
                                    //controll the level, if its bigger than desired hash func than this cycle cant continue
                                    if (level == CountHashFun - 1)
                                    {
                                        AddToOverflowBlock(blockRightSon, data);
                                        RightSon.Index = ReturnFreeAdress(true);

                                        //ChooseIndex(RightSon, blockRightSon);
                                        //ChooseIndex(LeftSon, blockLeftSon);
                                        //RightSon.Index = oldFreeIndex;
                                        LeftSon.Index = -1;
                                        WriteBackToFile(RightSon.Index, blockRightSon, true);
                                        //WriteBackToFile(LeftSon.Index, blockLeftSon, true);
                                        node.CountOfRecords++;
                                        return true;
                                    }
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
                                    //controll the level, if its bigger than desired hash func than this cycle cant continue
                                    if (level == CountHashFun - 1)
                                    {
                                        AddToOverflowBlock(blockLeftSon, data);
                                        LeftSon.Index = ReturnFreeAdress(true);

                                        //ChooseIndex(LeftSon, blockLeftSon);
                                        //ChooseIndex(RightSon, blockRightSon);
                                        RightSon.Index = -1; //has 0 records
                                        WriteBackToFile(LeftSon.Index, blockLeftSon, true);
                                        node.CountOfRecords++;
                                        //WriteBackToFile(RightSon.Index, blockRightSon, true);
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //where does data belong
                            //create new OverFlowBlock and insert data there
                            if (hashData[level])
                            {
                                //add overflow block to the right son
                                AddToOverflowBlock(blockRightSon, data);

                            }
                            else
                            {
                                AddToOverflowBlock(blockLeftSon, data);
                            }
                            Trace.WriteLine("Added to overflow block");

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
                                RightSon.Index = ReturnFreeAdress(true);
                                LeftSon.Index = ReturnFreeAdress(true);

                                //ChooseIndex(RightSon, blockRightSon);
                                //ChooseIndex(LeftSon, blockLeftSon);
                                //LeftSon.Index = newFreeIndex;
                                //LeftSon.CountOfRecords = blockLeftSon.ValidRecordsCount;
                                //RightSon.Index = oldFreeIndex;
                                //RightSon.CountOfRecords = blockRightSon.ValidRecordsCount;
                                WriteBackToFile(LeftSon.Index, blockLeftSon, true);
                                WriteBackToFile(RightSon.Index, blockRightSon, true);
                                noOfRecords++;
                                Trace.WriteLine("Written to a new address." + blockLeftSon.ToString());
                                Trace.WriteLine("Written to a new address." + blockRightSon.ToString());
                                LeftSon.CountOfRecords = blockLeftSon.ValidRecordsCount;
                                RightSon.CountOfRecords = blockRightSon.ValidRecordsCount;

                            }
                            else if (LeftSon.CountOfRecords > 0)
                            {
                                LeftSon.Index = ReturnFreeAdress(true);
                                //ChooseIndex(LeftSon, blockLeftSon);
                                WriteBackToFile(LeftSon.Index, blockLeftSon, true);
                                noOfRecords++;
                                Trace.WriteLine("Written to a new address." + blockLeftSon.ToString());
                                LeftSon.CountOfRecords = blockLeftSon.ValidRecordsCount;
                            }
                            else if (RightSon.CountOfRecords > 0)
                            {
                                RightSon.Index = ReturnFreeAdress(true);
                                //ChooseIndex(RightSon, blockRightSon);
                                WriteBackToFile(RightSon.Index, blockRightSon, true);
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
            return true;
        }

        private void AddToOverflowBlock(Block<T> block, T data)
        {
            var indexes = new List<int>();
            indexes.Add(block.OfindexNext);
            //index from the main file to the overflowing one

            int index = block.OfindexNext; //index of the start of this overflow block sequence
            if (block.OfindexNext == -1)
            {

                block.OfindexNext = ReturnFreeAdress(false);

                /*   //nastavenie adresy na koniec suboru
                   var newIndex = FileOverflow.Length;
                   FileOverflow.SetLength(FileOverflow.Length + block.getSize());
                   block.OfindexNext = (int)newIndex;*/


                var blockOverflow = new Block<T>(BlockFactorOverflow);
                if (!blockOverflow.Insert(data))
                {
                    Trace.WriteLine("Error");
                }
                WriteBackToFile(block.OfindexNext, blockOverflow, false);
            }
            else
            {
                int i = 0;
                bool first = true;
                while (true)
                {
                    block = ReadBlockFromFile(block.OfindexNext, false);
                    i++;
                    if (i > 1)
                    {
                        first = false;
                    }
                    if (block.OfindexNext == -1)
                    {
                        //last block was found
                        break;
                    }
                    indexes.Add(block.OfindexNext);

                }
                if (block.ValidRecordsCount < BlockFactorOverflow)
                {
                    //fill record in
                    if (!block.Insert(data))
                    {
                        Trace.WriteLine("Error");
                    }

                    WriteBackToFile(indexes[indexes.Count() - 1], block, false);

                }
                else //create new block
                {

                    int newIndex = ReturnFreeAdress(false);
                    /*   //nastavenie adresy na koniec suboru
                       var newIndex = FileOverflow.Length;
                       FileOverflow.SetLength(FileOverflow.Length + block.getSize());
                       //block.OfindexNext = (int)newIndex;*/
                    indexes.Add((int)newIndex);


                    var blockOverflow = new Block<T>(BlockFactorOverflow);
                    if (!blockOverflow.Insert(data))
                    {
                        Trace.WriteLine("Error");
                    }

                    if (first)
                    {
                        block.OfIndexBefore = -1; //stays that way
                        block.OfindexNext = indexes[indexes.Count() - 1];
                        //block.OfindexNext = indexes[indexes.Count() -2];
                        //it will write to indexes[0]
                        blockOverflow.OfIndexBefore = indexes[indexes.Count() - 2];
                        WriteBackToFile(block.OfindexNext, blockOverflow, false);
                        WriteBackToFile(blockOverflow.OfIndexBefore, block, false);
                    }
                    else
                    {
                        //block.OfIndexBefore = indexes[indexes.Count() - 3];
                        block.OfindexNext = indexes[indexes.Count() - 1];
                        blockOverflow.OfIndexBefore = indexes[indexes.Count() - 2];
                        WriteBackToFile(block.OfindexNext, blockOverflow, false);
                        WriteBackToFile(blockOverflow.OfIndexBefore, block, false);
                    }


                }

            }
        }

        private ExternalNode ShortenBranch(ExternalNode node, Block<T> block, ExternalNode brotherNode, Block<T> brotherBlock)
        {
            var end = false;
            while (!end)
            {
                var pom = brotherBlock.ValidRecordsCount;
                for (int i = 0; i < pom; i++)
                {
                    var record = brotherBlock.Records[0];
                    brotherBlock.Remove(record);
                    if (!block.Insert(record))
                    {
                        Trace.WriteLine("Error when shortening branches.");
                    }
                    
                    //add empty brotherBlock to empty blocks manager
                    node.CountOfRecords++;
                    brotherNode.CountOfRecords--;
                }

                if (brotherNode.Index != -1 && brotherNode.CountOfRecords == 0)
                {
                    //skratenie file
                    if (!ShortenFileMain(brotherNode))
                    {
                        //zretazenie
                        AddToEmptyBlock(brotherNode.Index, brotherBlock, true);
                    }
                }
                //cut parent and brotherNode

                if (node.Parent == Trie.Root)
                {
                    Trie.Root = node;
                    node.Parent = null;
                    //nesetujem parentovho parenta
                }
                else
                {
                    var newParent = node.Parent.Parent;

                    if (node.Parent == newParent.RightNode)
                    {
                        newParent.RightNode = node;
                        node.Parent = newParent;
                    }
                    else
                    {
                        newParent.LeftNode = node;
                        node.Parent = newParent;
                    }
                }
                
                //nodes index stays the same

                WriteBackToFile(node.Index, block, true);

                //check for repeating

                brotherNode = Trie.findBrother(node);
                if (brotherNode == null)
                {
                    return node;
                }

                if (brotherNode.Index == -1)
                {
                    brotherBlock = new Block<T>(BlockFactor); //fake
                }
                else
                {
                    brotherBlock = ReadBlockFromFile(brotherNode.Index, true);
                }
                
                if (brotherBlock.OfindexNext != -1)
                {
                    return node;
                }

                if (block.ValidRecordsCount + brotherBlock.ValidRecordsCount > BlockFactor)
                {
                    end = true;
                }
            }
            return null;
        }

        //manazment volnych blokov
        //- volne bloky mam zretazene 
        //ak mam nejaky uvolneny od konca tak ho vymazem
        //musim prejst vsetky volne bloky a pozriet ci sa nachadza nejaky na konci suboru
        //vymazat ho a prenastavit nasledovnika a predchodcu
        //cyklus

        public void AddToEmptyBlock(int indexFromNode, Block<T> block, bool main)
        {
            if (main)
            {
                var indexes = new List<int>();
                indexes.Add(EmptyBlocksIndex);
                //ak nie je ziadny block este prazdny pridam ho na zaciatok
                if (EmptyBlocksIndex == -1)
                {
                    //start of the chaining
                    block.ChainIndexBefore = -1;

                    if (block.OfindexNext == -1) //no next blocks in OF
                    {
                        //block is empty add to empty blocks in main file

                        EmptyBlocksIndex = indexFromNode;
                        WriteBackToFile(indexFromNode, block, true);
                        return;
                        //shorten the tree upwards

                    }
                    else
                    {
                        return;
                    }

                }
                //we continue in chaining - linking with the last one
                //first one linked

                var pomBlock = ReadBlockFromFile(EmptyBlocksIndex, true);
                indexes.Add(pomBlock.ChainIndexAfter); //2nd block in chaining
                if (pomBlock.ChainIndexAfter == -1)
                {
                    //there is a chaining
                    pomBlock.ChainIndexAfter = indexFromNode;
                    block.ChainIndexBefore = EmptyBlocksIndex;
                    WriteBackToFile(indexFromNode, block, true);
                    WriteBackToFile(EmptyBlocksIndex, pomBlock, true);
                    return;
                }
                while (true)
                {
                    pomBlock = ReadBlockFromFile(pomBlock.ChainIndexAfter, true);
                    indexes.Add(pomBlock.ChainIndexAfter);
                    if (pomBlock.ChainIndexAfter == -1)
                    {
                        //end , chain here
                        break;
                    }
                    //continue
                }
                pomBlock.ChainIndexAfter = indexFromNode;
                block.ChainIndexBefore = indexes[indexes.Count - 2]; //need to remember from array
                WriteBackToFile(indexFromNode, block, true);
                WriteBackToFile(block.ChainIndexBefore, pomBlock, true);
                return;
            }
            else
            {
                var indexes = new List<int>();
                indexes.Add(EmptyBlocksIndexOverflow);
                //ak nie je ziadny block este prazdny pridam ho na zaciatok
                if (EmptyBlocksIndexOverflow == -1)
                {
                    //start of the chaining
                    block.ChainIndexBefore = -1;
                    block.ChainIndexAfter = -1;
                    if (block.OfindexNext == -1) //no next blocks in OF
                    {
                        if (block.OfIndexBefore != -1)
                        {
                            var before = ReadBlockFromFile(block.OfIndexBefore, false);
                            before.OfindexNext = -1;
                            WriteBackToFile(block.OfIndexBefore, before, false);
                            block.OfIndexBefore = -1;
                        }
                        //block is empty add to empty blocks in OF

                        EmptyBlocksIndexOverflow = indexFromNode;
                        WriteBackToFile(indexFromNode, block, false);
                        return;

                    }
                    else
                    {
                        //?? should not happen
                        return;
                    }

                }
                //we continue in chaining - linking with the last one
                //first one linked

                var pomBlock = ReadBlockFromFile(EmptyBlocksIndexOverflow, false);
                indexes.Add(pomBlock.ChainIndexAfter); //2nd block in chaining
                if (pomBlock.ChainIndexAfter == -1)
                {
                    //there is a chaining
                    pomBlock.ChainIndexAfter = indexFromNode;
                    block.ChainIndexBefore = EmptyBlocksIndexOverflow;
                    block.ChainIndexAfter = -1;

                    if (block.OfindexNext == -1) //no next blocks in OF
                    {
                        if (block.OfIndexBefore != -1)
                        {
                            var before = ReadBlockFromFile(block.OfIndexBefore, false);
                            before.OfindexNext = -1;
                            WriteBackToFile(block.OfIndexBefore, before, false);
                            block.OfIndexBefore = -1;
                        }

                    }

                    WriteBackToFile(indexFromNode, block, false);
                    WriteBackToFile(EmptyBlocksIndexOverflow, pomBlock, false);
                    return;
                }
                while (true)
                {
                    pomBlock = ReadBlockFromFile(pomBlock.ChainIndexAfter, false);
                    indexes.Add(pomBlock.ChainIndexAfter);
                    if (pomBlock.ChainIndexAfter == -1)
                    {
                        //end , chain here
                        break;
                    }
                    //continue
                }
                pomBlock.ChainIndexAfter = indexFromNode;
                block.ChainIndexBefore = indexes[indexes.Count - 2]; //need to remember from array
                block.ChainIndexAfter = -1;
                if (block.OfindexNext == -1) //no next blocks in OF
                {
                    if (block.OfIndexBefore != -1)
                    {
                        var before = ReadBlockFromFile(block.OfIndexBefore, false);
                        before.OfindexNext = -1;
                        WriteBackToFile(block.OfIndexBefore, before, false);
                        block.OfIndexBefore = -1;
                    }

                }
                WriteBackToFile(indexFromNode, block, false);
                WriteBackToFile(block.ChainIndexBefore, pomBlock, false);
                return;
            }

        }
        public bool RemoveNew(T data) 
        {
            int level = -1;
            var dataHash = data.getHash(CountHashFun);
            var node = Trie.getExternalNode(dataHash, out level);
            var block = ReadBlockFromFile(node.Index, true);

            //record is in MF
            for (int i = 0; i < block.ValidRecordsCount; i++)
            {
                if (block.Records[i].MyEquals(data))
                {
                    block.Remove(data);
                    node.CountOfRecords--;

                    WriteBackToFile(node.Index, block, true);

                    var brotherNode = Trie.findBrother(node);
                    
                    if (brotherNode == null)
                    {
                        if (node.CountOfRecords == 0)
                        {
                            if (!ShortenFileMain(node))
                            {
                                //uvolnenie adresy
                                this.AddToEmptyBlock(node.Index, block, true);
                                node.Index = -1;
                            }
                        }
                        else
                        {
                            Shake(node);
                        }       
                        //strasenie
                        return true;
                    }

                    Block<T> brotherBlock = null;
                    if (brotherNode.Index == -1)
                    {
                        brotherBlock = new Block<T>(BlockFactor);
                    }
                    else
                    {
                        brotherBlock = ReadBlockFromFile(brotherNode.Index, true);
                    }

                    if (brotherBlock.ValidRecordsCount + block.ValidRecordsCount <= BlockFactor && brotherBlock.OfindexNext == -1 && block.OfindexNext == -1)
                    {
                        //switching nodes according to index so index at the EOF is free
                        /*if (node.Index < brotherNode.Index)
                        {
                            ShortenBranch(node, block, brotherNode, brotherBlock);
                        }
                        else
                        {
                            ShortenBranch(brotherNode, brotherBlock, node, block);
                        }*/
                        ShortenBranch(node, block, brotherNode, brotherBlock);

                        return true;
                    }

                    if (node.CountOfRecords == 0)
                    {
                        //skratenie file
                        if (!ShortenFileMain(node))
                        {
                            //uvolnenie adresy
                            this.AddToEmptyBlock(node.Index, block, true);
                        }
                    }
                    else
                    {
                        Shake(node);
                    }
                    
                    //strasenie
                    return true;

                }
            }
            //record is in OF
            if (block.OfindexNext == -1)
            {
                return false;
            }

            var index = block.OfindexNext;
            var blockOF = ReadBlockFromFile(block.OfindexNext,false);
            
            while (true)
            {
                for (int i = 0; i < blockOF.ValidRecordsCount; i++)
                {
                    if (blockOF.Records[i].MyEquals(data))
                    {
                        blockOF.Remove(data);
                        node.CountOfRecords--;

                        WriteBackToFile(index, blockOF, false);
                        
                        Shake(node);
                        //uvolnenie file OF
                        
                        //ShortenFileOF(index);
                        
                        return true;
                    }
                }

                if (blockOF.OfindexNext == -1)
                {
                    return false;
                }
                index = blockOF.OfindexNext;
                blockOF = ReadBlockFromFile(blockOF.OfindexNext, false);
            }


        }

        private void Shake(ExternalNode node)
        {
            //from node to the end of the OF file count valid records and potential valid records
            int validsum = 0;
            int fullsize = 0;
            int countOFblocks = 0;

            List<Block<T>> blocks = new List<Block<T>>();
            List<int> indexes = new List<int>();

            var block = ReadBlockFromFile(node.Index, true);
            blocks.Add(block);
            indexes.Add(node.Index);
            validsum += block.ValidRecordsCount;
            fullsize += block.BlockFactor;

            if (block.OfindexNext == -1)
            {
                return;
            }

            while (block.OfindexNext != -1)
            {
                indexes.Add(block.OfindexNext);
                block = ReadBlockFromFile(block.OfindexNext, false);
                blocks.Add(block);
                validsum += block.ValidRecordsCount;
                fullsize += block.BlockFactor;
            }

            var result = fullsize - validsum; //if its more or equals as BFOVERFLOW - one block can be emptied from OF


            var toShuffle = block.ValidRecordsCount; //one block needs to be freed so all data from last block needs to e reshufled
            var shuffleCounter = 0;

            if (result < BlockFactorOverflow)
            {
                return;
            }

            List<T> listToReinsert = new List<T>();

            for (int i = 0; i < toShuffle; i++)
            {
                listToReinsert.Add(block.Records[0]);
                block.Remove(block.Records[0]);
            }

            WriteBackToFile(indexes[indexes.Count - 1], block, false); //last one is empty

            //pridanie do prazdneho zretazenia blokov
            if (block.ValidRecordsCount == 0 && block.OfindexNext == -1)
            {
                if (block.OfIndexBefore == -1)
                {
                    //viem ze jeho predchodca je z main file
                    var predchodca = blocks[0];
                    predchodca.OfindexNext = -1;
                    WriteBackToFile(indexes[0], predchodca, true);
                }
                if (!ShortenFileOF(indexes[indexes.Count - 1],block))
                {
                    AddToEmptyBlock(indexes[indexes.Count - 1], block, false);
                }
            }

            blocks.RemoveAt(blocks.Count - 1);
            indexes.RemoveAt(indexes.Count - 1);

            blocks[blocks.Count - 1].OfindexNext = -1; // z predposledneho vymazem referenciu lokalne z Listu

            if (listToReinsert.Count > 0)
            {
                while (blocks[0].ValidRecordsCount < BlockFactor) //najprv do node v mainfile
                {
                    blocks[0].Insert(listToReinsert[0]);
                    listToReinsert.RemoveAt(0);
                    if (listToReinsert.Count == 0)
                    {
                       break;
                    }
                }

                WriteBackToFile(indexes[0], blocks[0], true);
                blocks.RemoveAt(0);
                indexes.RemoveAt(0);

                if (listToReinsert.Count == 0)
                {
                    return;
                }

                var count = blocks.Count;
                for (int i = 0; i < count; i++)
                {
                    while (blocks[0].ValidRecordsCount < BlockFactorOverflow)
                    {
                        blocks[0].Insert(listToReinsert[0]);
                        listToReinsert.RemoveAt(0);
                        if (listToReinsert.Count == 0)
                        {
                            break ;
                        }
                    }

                    WriteBackToFile(indexes[0], blocks[0], false);
                    blocks.RemoveAt(0);
                    indexes.RemoveAt(0);

                    if (listToReinsert.Count == 0)
                    {
                        return;
                    }
                }
                return;
            }

        }

        private bool ShortenFileMain(ExternalNode node)
        {
            if (node.Index != (File.Length-BlockSize))
            {
                return false;
            }

            File.SetLength(File.Length - BlockSize);
            node.Index = -1;

            if (File.Length == 0)
            {
                return true;
            }

            int previousindex = (int)(File.Length - BlockSize);
            var block = ReadBlockFromFile(previousindex, true);

            while (block.ValidRecordsCount == 0 && block.OfindexNext == -1) //dovtedy mozem mazat
            {
                if (block.ChainIndexBefore == -1 && block.ChainIndexAfter == -1) //je prvy a jediny v zretazeni
                {
                    EmptyBlocksIndex = -1;
                    File.SetLength(File.Length - BlockSize);
                    return true; //uz by nemal byt ziadny prazdny block vobec
                }

                if (block.ChainIndexBefore == -1 && block.ChainIndexAfter != -1) //je prvy v zretazeni ale ma nasledovnika
                {
                    EmptyBlocksIndex = block.ChainIndexAfter;

                    //prepisat atribut nasledovnikovi
                    var next = ReadBlockFromFile(block.ChainIndexAfter, true);
                    next.ChainIndexBefore = -1;
                    WriteBackToFile(EmptyBlocksIndex, next, true);
                }

                //je na konci zretazenia
                if (block.ChainIndexBefore != -1 && block.ChainIndexAfter == -1)
                {
                    var before = ReadBlockFromFile(block.ChainIndexBefore, true);
                    before.ChainIndexAfter = -1;
                    WriteBackToFile(block.ChainIndexBefore, before,true);
                    
                }

                //je v strede zretazenie
                if (block.ChainIndexBefore != -1 && block.ChainIndexAfter != -1)
                {
                    var next = ReadBlockFromFile(block.ChainIndexAfter, true);
                    var before = ReadBlockFromFile(block.ChainIndexBefore, true);

                    next.ChainIndexBefore = block.ChainIndexBefore;
                    before.ChainIndexAfter = block.ChainIndexAfter;

                    WriteBackToFile(block.ChainIndexAfter, next,true);
                    WriteBackToFile(block.ChainIndexBefore, before, true);
                }


                //znizim velkost file (block sa nemusi zapisovat spat)
                File.SetLength(File.Length - BlockSize);

                previousindex = (int)(File.Length - BlockSize);
                block = ReadBlockFromFile(previousindex, true);
            }

            return true;
        }

        private bool ShortenFileOF(int index, Block<T> blockD)
        {
            if (index != (FileOverflow.Length - BlockSizeOF))
            {
                return false;
            }

            FileOverflow.SetLength(FileOverflow.Length - BlockSizeOF);
            if (blockD.OfIndexBefore != -1)
            {
                var before = ReadBlockFromFile(blockD.OfIndexBefore, false);
                before.OfindexNext = -1;
                WriteBackToFile(blockD.OfIndexBefore, before, false);
            }       
            
            if (index == EmptyBlocksIndexOverflow)
            {
                EmptyBlocksIndexOverflow = -1;
            }
            
            if (FileOverflow.Length == 0)
            {
                return true;
            }

            int previousindex = (int)(FileOverflow.Length - BlockSizeOF);
            var block = ReadBlockFromFile(previousindex, false);

            while (block.ValidRecordsCount == 0 && block.OfindexNext == -1) //dovtedy mozem mazat
            {
                if (block.ChainIndexBefore == -1 && block.ChainIndexAfter == -1) //je prvy a jediny v zretazeni
                {
                    EmptyBlocksIndexOverflow = -1;
                    FileOverflow.SetLength(FileOverflow.Length - BlockSizeOF);
                    return true; //uz by nemal byt ziadny prazdny block vobec
                }

                if (block.ChainIndexBefore == -1 && block.ChainIndexAfter != -1) //je prvy v zretazeni ale ma nasledovnika
                {
                    EmptyBlocksIndexOverflow = block.ChainIndexAfter;

                    //prepisat atribut nasledovnikovi
                    var next = ReadBlockFromFile(block.ChainIndexAfter, false);
                    next.ChainIndexBefore = -1;
                    WriteBackToFile(EmptyBlocksIndexOverflow, next, false);
                }

                //je na konci zretazenia
                if (block.ChainIndexBefore != -1 && block.ChainIndexAfter == -1)
                {
                    var before = ReadBlockFromFile(block.ChainIndexBefore, false);
                    before.ChainIndexAfter = -1;
                    WriteBackToFile(block.ChainIndexBefore, before, false);

                }

                //je v strede zretazenie
                if (block.ChainIndexBefore != -1 && block.ChainIndexAfter != -1)
                {
                    var next = ReadBlockFromFile(block.ChainIndexAfter, false);
                    var before = ReadBlockFromFile(block.ChainIndexBefore, false);

                    next.ChainIndexBefore = block.ChainIndexBefore;
                    before.ChainIndexAfter = block.ChainIndexAfter;

                    WriteBackToFile(block.ChainIndexAfter, next, false);
                    WriteBackToFile(block.ChainIndexBefore, before, false);
                }


                //znizim velkost file (block sa nemusi zapisovat spat)
                FileOverflow.SetLength(FileOverflow.Length - BlockSizeOF);

                previousindex = (int)(FileOverflow.Length - BlockSizeOF);
                block = ReadBlockFromFile(previousindex, false);
            }

            return true;
        }

        public Block<T> FindBlockByHash(BitArray hash)
        {
            var levelnot = -1;
            var node = _trie.getExternalNode(hash, out levelnot);
            if (node != null)
            {
                //nacitat block z file a returnut ho
                var blockFromFile = ReadBlockFromFile(node.Index, true);
                return blockFromFile;
            }
            return null;

        }

        public Block<T> ReadBlockFromFile(int index, bool mainFile)
        {
            Block<T> block;
            if (mainFile)
            {
                block = new Block<T>(BlockFactor);
            }
            else
            {
                block = new Block<T>(BlockFactorOverflow);
            }

            byte[] bytes = new byte[block.getSize()];

            if (mainFile)
            {
                File.Seek(index, SeekOrigin.Begin);
                File.Read(bytes);

                if (File.Length < (index + bytes.Count()))
                {
                    Trace.WriteLine("Reading more than File Length.");
                }
            }
            else
            {
                FileOverflow.Seek(index, SeekOrigin.Begin);
                FileOverflow.Read(bytes);
            }
            block.fromByteArray(bytes);
            return block;
        }



        public void WriteBackToFile(int index, Block<T> block, bool mainFile)
        {
            if (mainFile)
            {
                File.Seek(index, SeekOrigin.Begin);
                File.Write(block.toByteArray());
            }
            else
            {
                FileOverflow.Seek(index, SeekOrigin.Begin);
                FileOverflow.Write(block.toByteArray());
            }
        }

        public T Find(T data)
        {
            //find hash func of data
            //find external node
            //read from file from index of the node

            T returnData;

            var level = -1;
            var hashData = data.getHash(CountHashFun);
            var node = Trie.getExternalNode(data.getHash(CountHashFun), out level);
            if (node != null)
            {

                if (node.CountOfRecords == 0)
                {
                    Trace.WriteLine("Node found by hash has no records.");
                    return default(T);
                }
                else
                {
                    var block = ReadBlockFromFile(node.Index, true);
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
                    Trace.WriteLine("Item not found in main file." + data.ToString());

                    if (block.OfindexNext != -1)
                    {
                        while (true)
                        {
                            block = ReadBlockFromFile(block.OfindexNext, false);
                            if (block == null)
                            {
                                return default(T);
                            }
                            for (int i = 0; i < block.ValidRecordsCount; i++)
                            {
                                if (i < block.ValidRecordsCount)
                                {
                                    if (data.MyEquals(block.Records.ElementAt(i)))
                                    {
                                        return block.Records.ElementAt(i);
                                    }
                                }
                            }
                            if (block.OfindexNext == -1)
                            {
                                return default(T);
                            }
                        }
                    }
                    return default(T);
                }
            }
            else
            {
                return default(T);
            }

        }

        public void WriteAllBlocksToFile()
        {
            byte[] bytes = new byte[File.Length];

            File.Seek(0, SeekOrigin.Begin);
            File.Read(bytes);

            for (int i = 0; i < bytes.Length; i += BlockSize)
            {
                byte[] actualBytes = new byte[BlockSize];

                // Copy a block-sized chunk of data to actualBytes
                for (int j = 0; j < BlockSize && (i + j) < bytes.Length; j++)
                {
                    actualBytes[j] = bytes[i + j];
                }

                var block = new Block<T>(BlockFactor);
                block.fromByteArray(actualBytes);

                this.WriteToFile("output.txt", block, i);
            }
        }
        public void WriteToFile(string filePath, Block<T> block, int address)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(address.ToString());
                writer.WriteLine(block.ValidRecordsCount.ToString() + "/" + BlockFactor.ToString());
                foreach (T record in block.Records)
                {
                    // Assuming T has a meaningful ToString method
                    writer.WriteLine(record.ToString());
                }
            }
        }

        public string GetString(bool mainFile)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bytes;
            if (mainFile)
            {
                bytes = new byte[File.Length];
                File.Seek(0, SeekOrigin.Begin);
                File.Read(bytes);

                for (int i = 0; i < bytes.Length; i += BlockSize)
                {
                    byte[] actualBytes = new byte[BlockSize];

                    // Copy a block-sized chunk of data to actualBytes
                    for (int j = 0; j < BlockSize && (i + j) < bytes.Length; j++)
                    {
                        actualBytes[j] = bytes[i + j];
                    }
                    var block = new Block<T>(BlockFactor);
                    block.fromByteArray(actualBytes);
                    sb.Append("\n----------------------------------------\n");
                    sb.Append("Address: " + i + "\n");
                    if (EmptyBlocksIndex == i)
                    {
                        sb.Append("START of the chaining of empty blocks.\n");
                        sb.Append(block.ToString(BlockFactor, true));
                    }
                    else
                    {
                        if (block.ChainIndexAfter != -1 || block.ChainIndexBefore != -1)
                        {
                            sb.Append(block.ToString(BlockFactor, true));
                        }
                        else
                        {
                            sb.Append(block.ToString(BlockFactor, false));
                        }
                    }

                }
            }
            else
            {
                bytes = new byte[FileOverflow.Length];
                FileOverflow.Seek(0, SeekOrigin.Begin);
                FileOverflow.Read(bytes);

                for (int i = 0; i < bytes.Length; i += BlockSizeOF)
                {
                    byte[] actualBytes = new byte[BlockSizeOF];

                    // Copy a block-sized chunk of data to actualBytes
                    for (int j = 0; j < BlockSizeOF && (i + j) < bytes.Length; j++)
                    {
                        actualBytes[j] = bytes[i + j];
                    }
                    var block = new Block<T>(BlockFactorOverflow);
                    block.fromByteArray(actualBytes);
                    sb.Append("\n----------------------------------------\n");
                    sb.Append("Address: " + i + "\n");
                    sb.Append("Adress to block before: " + block.OfIndexBefore.ToString() + "\n");
                    if (EmptyBlocksIndexOverflow == i)
                    {
                        sb.Append("START OF CHAINING EMPTY BLOCKS\n");
                    }

                    if (block.ChainIndexBefore != -1 || block.ChainIndexAfter != -1)
                    {
                        sb.Append(block.ToString(BlockFactorOverflow, true));
                    }
                    else
                    {
                        sb.Append(block.ToString(BlockFactorOverflow, false));
                    }


                }
            }



            return sb.ToString();
        }

        public void EditData(T data, T edited)
        {
            //find address
            //find in which file 
            //get block 
            //edit block
            //write back to that address
            Block<T> blockToEdit = null;
            int addresToWriteBack = -1;
            int file = -1; //1- main , 0 - of
            int position = -1;

            var level = -1;
            var node = Trie.getExternalNode(data.getHash(CountHashFun), out level);
            if (node != null)
            {
                var block = ReadBlockFromFile(node.Index, true);
                for (int i = 0; i < block.Records.Count; i++)
                {
                    if (i < block.ValidRecordsCount)
                    {
                        if (data.MyEquals(block.Records.ElementAt(i)))
                        {
                            blockToEdit = block;
                            addresToWriteBack = node.Index;
                            file = 1;
                            position = i;
                            break;
                        }
                    }
                }
                if (block.OfindexNext != -1)
                {
                    while (true)
                    {
                        var pom = block.OfindexNext;
                        block = ReadBlockFromFile(block.OfindexNext, false);
                        for (int i = 0; i < block.ValidRecordsCount; i++)
                        {
                            if (i < block.ValidRecordsCount)
                            {
                                if (data.MyEquals(block.Records.ElementAt(i)))
                                {
                                    blockToEdit = block;
                                    position = i;
                                    addresToWriteBack = pom;
                                    file = 0;
                                    break;
                                }
                            }
                        }
                        if (addresToWriteBack != -1)
                        {
                            break;
                        }
                    }
                }

            }

            if (addresToWriteBack == -1)
            {
                //error
                var debug = "error";
            }

            blockToEdit.Records[position] = edited;
            WriteBackToFile(addresToWriteBack, blockToEdit, file == 1 ? true : false);
        }

        public (Block<T>, int, int, int) AddDataToRecords(T data)
        {
            var level = -1;
            var node = Trie.getExternalNode(data.getHash(CountHashFun), out level);
            if (node != null)
            {
                var block = ReadBlockFromFile(node.Index, true);
                for (int i = 0; i < block.Records.Count; i++)
                {
                    if (i < block.ValidRecordsCount)
                    {
                        if (data.MyEquals(block.Records.ElementAt(i)))
                        {
                            
                                return (block, i, node.Index, 1);
                            

                        }
                    }
                }
                if (block.OfindexNext != -1)
                {
                    while (true)
                    {
                        var pom = block.OfindexNext;
                        block = ReadBlockFromFile(block.OfindexNext, false);
                        for (int i = 0; i < block.ValidRecordsCount; i++)
                        {
                            if (i < block.ValidRecordsCount)
                            {
                                if (data.MyEquals(block.Records.ElementAt(i)))
                                {
                                    return (block, i, pom, 0);
                                }
                            }
                        }
                    }
                }

            }

            return (null, -1, -1, -1);
        }


        public void SaveData(string pathTrie, string pathData, int newId, string exportDirectory) 
        {
            //hashcount
            //BF in main FIle
            //BF in OF file
            //Adresy na empty blocks in each file
            //trie a vsetky jeho externe nody + cesta k nim

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("BF:" + BlockFactor);
            sb.AppendLine("BFO:" + BlockFactorOverflow);
            sb.AppendLine("HASH:" + CountHashFun);
            sb.AppendLine("EMF:" + EmptyBlocksIndex);
            sb.AppendLine("EOF:" + EmptyBlocksIndexOverflow);
            sb.AppendLine("NextId:" + newId);

            // Save the files to the specified export directory
            string filePath = Path.Combine(exportDirectory, pathData);
            System.IO.File.WriteAllText(filePath, sb.ToString());
            Trie.SaveState(exportDirectory,pathTrie);

            // Modify the file paths for export
            string exportFilePath = Path.Combine(exportDirectory, Path.GetFileName(File.Name));
            string exportFileOverflowPath = Path.Combine(exportDirectory, Path.GetFileName(FileOverflow.Name));

            // Copy the File to the export directory
            File.Close(); // Close the original file before copying
            System.IO.File.Copy(File.Name, exportFilePath, true);
            File = new FileStream(exportFilePath, FileMode.Open, FileAccess.ReadWrite);

            // Copy the FileOverflow to the export directory
            FileOverflow.Close(); // Close the original file before copying
            System.IO.File.Copy(FileOverflow.Name, exportFileOverflowPath, true);
            FileOverflow = new FileStream(exportFileOverflowPath, FileMode.Open, FileAccess.ReadWrite);

            System.IO.File.Delete(_fileName);
            System.IO.File.Delete(_ofFileName);

        }

        public int LoadData(string triePath, string dataPath)
        {
            int returnId = -1;
            this.Trie = new Trie.Trie();

            // Read the data file
            string[] lines = System.IO.File.ReadAllLines(dataPath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "BF":
                            this.BlockFactor = int.Parse(value);
                            break;
                        case "BFO":
                            this.BlockFactorOverflow = int.Parse(value);
                            break;
                        case "HASH":
                            this.CountHashFun = int.Parse(value);
                            break;
                        case "EMF":
                            this.EmptyBlocksIndex = int.Parse(value);
                            break;
                        case "EOF":
                            this.EmptyBlocksIndexOverflow = int.Parse(value);
                            break;
                        case "NextId":
                            returnId = int.Parse(value);
                            break;
                        // Add more cases as needed for additional keys
                        default:
                            // Handle unknown keys or ignore them
                            break;
                    }
                }
            }

            // Load the trie state
            Trie.ReadLeaves(triePath);
            return returnId;
        }

    }
}
