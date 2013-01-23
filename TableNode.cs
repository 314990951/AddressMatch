using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressMatch
{
    [Serializable()]
    public class TableNode
    {
        private string _name;
        private GraphNode _gNode;
        private TableNode _next;
        private TableNode _prev;

        private TableNode() { }

        public TableNode(GraphNode gnode)
        {
            _gNode = gnode;
            _next = null;
            _prev = null;
            _name = gnode.Name;
        }

        #region  --------------------------property---------------------------
       
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public TableNode Next
        {
            get { return _next; }
            set { _next = value; }
        }

        public TableNode Prev
        {
            get { return _prev; }
            set { _prev = value; }
        }

        public GraphNode GNode
        {
            get { return _gNode; }
            set { _gNode = value; }
        }

        #endregion


    }
}
