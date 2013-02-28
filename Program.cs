#undef DEBUG
#define DEBUG

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddressMatch.Test;

namespace AddressMatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            Console.ReadLine();
        }

        static void Test()
        {
            //CustomTest.TestRef();
            InitTest.InitFromFile();
        }
    }
}
