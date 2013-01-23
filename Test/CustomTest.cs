using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddressMatch;
using AddressMatch.Training;

namespace AddressMatch.Test
{
    public class CustomTest
    {
        public static void TestRef()
        {
            UInt16 a = 0x200;
            InsertElement e = new InsertElement("", LEVEL.Building, InsertMode.AutoLevel | InsertMode.AutoPlace);
            Console.WriteLine("a is  = " + Convert.ToString(a,2));


            MatchHelper.Assert(true, "aaaa");
            Console.ReadLine();
        }

    }
}
