using System;
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
        }
    }
}
