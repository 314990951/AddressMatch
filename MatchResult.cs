using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressMatch
{
    public enum MatchResultState
    {
        SUCCESS = 1,
        NOTFOUND = 2,
        NOTMATCHED = 3,
        MULTIMATCHED =4,
        UNKNOWNFAILED = 5,
    }

    public class MatchResult
    {

        public GraphNode Result;
        

        public MatchResultState ResultState;

    }
}
