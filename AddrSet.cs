using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AddressMatch
{
    // is not thread-safe
    public class AddrSet
    {

        public static Graph AddrGraph;

        public static string DumpDirectory;

        public static string DiskFilePath;

        private bool _initialized = false;

        private static AddrSet _instance;

        //lock for Single Instance
        private static object SingleInstanceLock = new object();

        //rwlock
        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        #region -----------------------Init -------------------------

        public AddrSet(int a)
        {

        }
        private AddrSet()
        {
            init();

            if (!_ResumeFromDisk())
            {
                throw new Exception("Failed to resume from disk");
            }

            _initialized = true;
            
        }

        //Parameter configuration
        private void init()
        {
            //backup's directory 
            DumpDirectory = @"D:\";

            //path of disk file to be resumed
            DiskFilePath = @"D:\Test.dat";
        }

        private bool _ResumeFromDisk()
        {
            if (!File.Exists(DiskFilePath))
            {
                throw new IOException("there's no data file is found in " + DiskFilePath);
            }
            Stream stream = null;
            try
            {
                stream = new FileStream(DiskFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
                BinaryFormatter formatter = new BinaryFormatter();
                AddrGraph = (Graph)formatter.Deserialize(stream);
                stream.Close();
            }
            catch (System.Exception ex)
            {
                if (stream != null)
                    stream.Close();
                throw new Exception("Deserialization failed! Message: " + ex.Message);
            }
            if (AddrGraph != null && AddrGraph.NodeTable != null && AddrGraph.root != null)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }


        #endregion

        #region -----------------------property-------------------------

        public static AddrSet GetInstance()
        {
            if (_instance == null)
            {
                lock (SingleInstanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new AddrSet();
                        return _instance;
                    }
                }
            }

            return _instance;

        }

        public ReaderWriterLockSlim GetRWlock()
        {
            return this.rwLock;
        }

        public bool Initialized
        {
            get { return _initialized; }
        }

        #endregion

        #region -----------------------Persistence-------------------------

        //------Flush to Disk -------------TODO  Add Header[] to file? e.g. Version, CRC, TimeStamp.....
        /// <summary>
        /// Dump to Disk, location is default directory. 
        /// </summary>
        /// <returns></returns>
        public  bool Dump()
        {

            if (DumpDirectory == "" || DumpDirectory == null)
            {
                throw new Exception("The instance is not correctly initialized");
            }

            string dumpfile = getFileNameToDump();
            Stream stream = null;
            try
            {
                stream = new FileStream(DumpDirectory + dumpfile, FileMode.Create, FileAccess.Write, FileShare.None);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, AddrGraph);
                stream.Close();
            }
            catch (System.Exception ex)
            {
                if (stream != null)
                    stream.Close();
                throw new Exception("serialization failed! Message: " + ex.Message);
            }

            Console.WriteLine(" Graph serialize  success!");

            return true;
        }

        private string getFileNameToDump()
        {
            string filename = "AddrSetFile-" + DateTime.Now.ToString("yyyy-MM-dd") + "-" +
                                        DateTime.Now.Hour.ToString();
            int i = 0;
            while (File.Exists(DumpDirectory + filename))
            {
                filename += i.ToString();
                i++;
            }

            filename += ".dat";
            return filename;
        }

        #endregion

        #region -----------------------Retrieval-------------------------

        /// <summary>
        /// Retrieval the graph from root, Depth-First Traversal
        /// </summary>
        /// <param name="p"> find matched node in condition supported predicate delegate </param>
        /// <returns>collections of result</returns>
        public  List<GraphNode> RetrievalGraph(Predicate<GraphNode> p)
        {
            List<GraphNode> result = new List<GraphNode>();

            MultiMatchInNext(p, AddrGraph.root, ref result);

            return result;
        }

        private  void MultiMatchInNext(Predicate<GraphNode> p, GraphNode node, ref List<GraphNode> result)
        {
            if (p(node) && !result.Contains(node))
            {
                result.Add(node);
            }
            if (node.NextNodeList == null || node.NextNodeList.Count == 0)
            {
                return;
            }
            foreach (GraphNode nxt_node in node.NextNodeList)
            {
                MultiMatchInNext(p, nxt_node, ref result);
            }
        }

        /// <summary>
        /// Search matched node (multi-source) through specific delegate, from up to bottom.
        /// </summary>
        /// <param name="p">find matched node in condition supported predicate delegate</param>
        /// <param name="sourceNodeList"></param>
        /// <returns>collections of result</returns>
        public  List<GraphNode> ForwardSearchNode(Predicate<GraphNode> p, IList<GraphNode> sourceNodeList)
        {
            List<GraphNode> result = new List<GraphNode>();
            
            foreach (var sourceNode in sourceNodeList)
            {
                MultiMatchInNext(p, sourceNode, ref result);
            }
            
            return result;
        }

        /// <summary>
        /// Search matched node with single source node through specific delegate, from up to bottom.
        /// </summary>
        /// <param name="p">find matched node in condition supported predicate delegate</param>
        /// <param name="sourceNode"></param>
        /// <returns>collections of result</returns>
        public List<GraphNode> ForwardSearchNode(Predicate<GraphNode> p, GraphNode sourceNode)
        {
            List<GraphNode> result = new List<GraphNode>();

            MultiMatchInNext(p, sourceNode, ref result);

            return result;
        }


        #endregion


        #region -----------------------GraphNodeOperation------------------------
        /// <summary>
        /// Insert a node into graph
        /// </summary>
        /// <param name="NewNode">The node to be inserted</param>
        /// <param name="FatherNode">The node's father node</param>
        /// <returns></returns>
        public bool Insert(GraphNode NewNode,GraphNode FatherNode)
        {
            if (NewNode == null || FatherNode ==null || FatherNode.NextNodeList == null)
            {
                return false;
            }
            if (NewNode.NodeLEVEL <= FatherNode.NodeLEVEL && NewNode.NodeLEVEL != LEVEL.Uncertainty)
            {
                return false;
            }

            TableNode tnode = new TableNode(NewNode);
            Hashtable table = AddrSet.AddrGraph.NodeTable;

            //Add to NodeTable
            if (table.Contains(tnode.Name))
            {
                AppendTableNodeList((TableNode)table[tnode.Name], tnode);
            }
            else
            {
                table.Add(tnode.Name, tnode);
            }

            //Linked to Graph
            FatherNode.NextNodeList.Add(NewNode);

            AddrGraph.NodeCount++;

            return true;
        }
        
        // Need Retrieval the Whole Graph!        --------TODO: Improve?   NEED TESTED
        /// <summary>
        /// Delete a node from graph
        /// </summary>
        /// <param name="node">The node to be deleted</param>
        /// <returns></returns>
        public bool Delete(GraphNode node)
        {
            //Delete from NodeTable
            Hashtable table = AddrSet.AddrGraph.NodeTable;
            TableNode tnodelist = table[node.Name] as TableNode;

            //find the relevant node in NodeList
            TableNode tnode = tnodelist;
            while (tnode.Next != null)
            {
                if (tnode.GNode.ID == node.ID)
                {
                    break;
                }
                tnode = tnode.Next;
            }
            if (tnode.GNode.ID != node.ID)
            {
                //Not found in NodeList
                return false;
            }

            if (tnode.Prev == null)          // this node is head
            {
                tnode.Next.Prev = null;
                table.Remove(node.Name);
                table.Add(node.Name,tnode.Next);
            }
            else if(tnode.Next == null)   //this node is tail
            {
                tnode.Prev.Next = null;
            }
            else
            {
                tnode.Prev.Next = tnode.Next;
                tnode.Next.Prev = tnode.Prev;
            }

            //Delete from Graph
            List<GraphNode> gnodelist = RetrievalGraph(delegate(GraphNode p)
            {
                if (p.NextNodeList.Contains(node))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }); 

            foreach (GraphNode resultnode in gnodelist)
            {
                resultnode.NextNodeList.Remove(node);
                AddrGraph.NodeCount--;
            }

            return true;
        }

        /// <summary>
        /// Rename a node in graph
        /// </summary>
        /// <param name="node">The node to be renamed</param>
        /// <param name="name">new name</param>
        /// <returns></returns>
        public bool ReName(GraphNode node, string name)
        {
            List<GraphNode> gnodelist = FindGNodeListInHashTable(node.Name);
            foreach (GraphNode gnode in gnodelist)
            {
                if (gnode.ID == node.ID)
                {
                    gnode.Name = name;
                }
            }

            return true;
        }


        private void AppendTableNodeList(TableNode head,TableNode node)
        {
            TableNode current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = node;
            node.Next = null;
            node.Prev = current;
        }

        #endregion



        #region -----------------------Query In HashTable-------------------------
        /// <summary>
        /// Find node with specific name in HashTable
        /// </summary>
        /// <param name="name">name of node</param>
        /// <returns>query state</returns>
        public State FindNodeInHashTable(string name)
        {
            State state = new State();
            if (AddrGraph.NodeTable[name] == null)
            {
                state.Name = name;
                state.MaxStateLEVEL = LEVEL.Uncertainty;
                state.MinStateLEVEL = LEVEL.Uncertainty;
                state.NodeCount = 0;
                state.NodeList = null;
                return state;
            }
            TableNode node = AddrGraph.NodeTable[name] as TableNode;
            LEVEL min = node.GNode.NodeLEVEL;
            LEVEL max = node.GNode.NodeLEVEL;
            state.NodeList.Add(node.GNode);
            int i = 1;
            while (node.Next != null)
            {
                min = min < node.Next.GNode.NodeLEVEL ? min : node.Next.GNode.NodeLEVEL;
                max = max > node.Next.GNode.NodeLEVEL ? max : node.Next.GNode.NodeLEVEL;
                state.NodeList.Add(node.Next.GNode);
                node = node.Next;
                i++;
            }
            state.Name = name;
            state.NodeCount = i;

            return state;

        }

        /// <summary>
        /// Find node with specific name in HashTable
        /// </summary>
        /// <param name="name">name of node</param>
        /// <returns>collection of result</returns>
        public List<GraphNode> FindGNodeListInHashTable(string name)
        {
            List<GraphNode> resultList = new List<GraphNode>();

            TableNode node = AddrGraph.NodeTable[name] as TableNode;
            resultList.Add(node.GNode);

            while (node.Next != null)
            {
                resultList.Add(node.Next.GNode);
                node = node.Next;
            }

            return resultList;

        }
        #endregion


        #region -----------------------rwLock dashboard-------------------------
        public int GetCurrentReadCount()
        {
            return rwLock.CurrentReadCount;
        }

        public int GetWaitingReadCount()
        {
            return rwLock.WaitingReadCount;
        }

        public int GetWaitingWriteCount()
        {
            return rwLock.WaitingWriteCount;
        }

        #endregion

    }


}
