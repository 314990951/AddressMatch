using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressMatch
{
    [Serializable()]
    public class State
    {
        public LEVEL MaxStateLEVEL;

        public LEVEL MinStateLEVEL;

        public List<GraphNode> NodeList;

        public int NodeCount;

        public string Name;

        public State()
        {
            MaxStateLEVEL = LEVEL.Default;
            MinStateLEVEL = LEVEL.Default;
            NodeList = new List<GraphNode>();
            NodeCount = 0;
            Name = "";
        }

    }



    public enum LEVEL
    {
        Default = -1,
        Root = 0,
        Contry = 1,
        Province = 2,
        City = 3,
        Zone = 4,
        Street = 5,
        Building = 6,
        Other = 7,
        Uncertainty = 8,
    }
}
