using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressMatch.Test
{
    public class DemoTest
    {
        public DemoTest()
        {
            //start
            AddrSet addrset = AddrSet.GetInstance();

            //Match
            MatchMachine m = new MatchMachine(addrset);

            MatchResult result = m.Match(new string[] { "B" });


            MatchHelper.PrintResult(result);

            //Train   ----------TODO


            //close
            //addrset.Dump();

            TableNode t = AddrSet.AddrGraph.NodeTable["B"] as TableNode;
            bool flag = result.Result.Equals(t.GNode);
            Console.WriteLine("XXX  " + flag);

        }
    }
}
