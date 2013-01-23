using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressMatch
{
    [Serializable()]
    public class GraphNode
    {
        private string _id;

        private string _name;

        private LEVEL _nodelevel;

        public List<GraphNode> NextNodeList;

        #region  --------------------------property---------------------------
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public LEVEL NodeLEVEL
        {
            get { return _nodelevel; }
            set { _nodelevel = value; }
        }

        #endregion


        #region -----------------------Construction-------------------------
        
        private GraphNode() { }

        public GraphNode(string name,LEVEL level)
        {
            string _id = System.Guid.NewGuid().ToString();
            _name = name;
            _nodelevel = level;
            NextNodeList = new List<GraphNode>();
            
        }

        public GraphNode(string name, LEVEL level, List<GraphNode> nextNodeList)
        {
            string _id = System.Guid.NewGuid().ToString();
            _name = name;
            _nodelevel = level;
            NextNodeList = nextNodeList;
        }

        #endregion

    }
}
