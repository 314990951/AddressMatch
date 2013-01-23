using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddressMatch;
using AddressMatch.Training;

namespace AddressMatch.Demo
{
    public class Demo
    {
        public Demo() { }

        public void Match()
        {
            //start
            AddrSet addrset = AddrSet.GetInstance();

            //Match
            MatchMachine m = new MatchMachine(addrset);

            // Custom the MatchRule
            //m.SetMatchRule(rule);

            MatchResult result = m.Match(new string[] { "B" });


            MatchHelper.PrintResult(result);

            //close
            //addrset.Dump();
        }

        public void Train()
        {
            //start
            AddrSet addrset = AddrSet.GetInstance();

            TrainMachine m = new TrainMachine(addrset);

            List<InsertElement> list = new List<InsertElement>();

            list.Add(new InsertElement("武汉", LEVEL.City, InsertMode.AutoPlace & InsertMode.ExactlyLevel));

            list.Add(new InsertElement("理工大", LEVEL.Other, InsertMode.AutoPlace & InsertMode.ExactlyLevel));

            list.Add(new InsertElement("屋檐下", LEVEL.Uncertainty, InsertMode.AutoPlace & InsertMode.ExactlyLevel));

            m.Train(list, true);

            //close
            //addrset.Dump();


        }
    }
}
