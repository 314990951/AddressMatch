using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressMatch.Test
{
    public class InitTest
    {
        public static void InitFromFile()
        {
            AddrSet set = AddrSet.GetInstance();
            //foreach(GraphNode gnode in AddrSet.AddrGraph.root.NextNodeList)
            //{
            //    TableNode tnode = new TableNode(gnode);
            //    AddrSet.AddrGraph.NodeTable.Add(tnode.Name,tnode);
            //}
            set.Dump();
            //AddrSet.AddrGraph = new Graph();
            //AddrSet.AddrGraph.root = new GraphNode("root", LEVEL.Root);
            //AddrSet.AddrGraph.NodeTable = new Hashtable();
            //AddrSet.AddrGraph.NodeCount = 0;
            //AddrSet.DumpDirectory = @"D:\";
            //AddrSet set = new AddrSet(1);
            //set.Dump();
            //TableNode tnode = new TableNode(gnode);
            //AddrSet.AddrGraph.NodeTable.Add(tnode.Name,tnode);
        }
    }
}
