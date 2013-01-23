using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressMatch
{
    public delegate bool MatchRule(State state, GraphNode node);

    public class MatchMachine
    {
        
        private  MatchRule LocalMatchRule;

        private AddrSet _addrset;

        private bool defualtRule(State state, GraphNode nextNode)
        {
            if (nextNode.NodeLEVEL == LEVEL.Uncertainty || state.MinStateLEVEL == LEVEL.Uncertainty)
            {
                return true;
            }
            if (nextNode.NodeLEVEL > state.MinStateLEVEL)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public MatchMachine(AddrSet addrset)
        {
            if (addrset == null)
            {
                throw new Exception("AddrSet未初始化");
            }
            if (AddrSet.AddrGraph == null)
            {
                throw new Exception("Graph未初始化"); 
            }

            _addrset = addrset;

            init();
        }

        private void init()
        {
            LocalMatchRule = new MatchRule(defualtRule);
            //MatchStack = new Stack<State>();
        }

        

        public MatchResult Match(String[] s)
        {
            Stack<State> MatchStack = new Stack<State>();

            MatchResult result = new MatchResult();
            
            s.Reverse();
            //保存第一个匹配
            State firstState = new State();

            for (int i = 0; i < s.Count();i++)
            {
                State correntState = _addrset.FindNodeInHashTable(s[i]);
                if (i ==0)
                {
                    firstState = correntState;
                }
                if (correntState.NodeCount == 0)
                {
                    result.ResultState = MatchResultState.NOTFOUND;
                    return result;
                }
                //MatchStack.
                if (MatchStack.Count > 0)
                {
                    FilterState(correntState, MatchStack.Peek());
                    if (correntState.NodeCount == 0)
                    {
                        result.ResultState = MatchResultState.NOTMATCHED;
                        return result;
                    }
                }
                MatchStack.Push(correntState);
            }
            if (MatchStack.Count == 0)
            {
                result.ResultState = MatchResultState.NOTFOUND;
                return result;
            }
            if (MatchStack.Peek().NodeCount > 1)
            {
                result.ResultState = MatchResultState.MULTIMATCHED;
                return result;
            }
            if (firstState.NodeCount == 1)
            {
                result.Result = firstState.NodeList.First();
                result.ResultState = MatchResultState.SUCCESS;
                return result;
            }

            List<GraphNode> resList;
            State TopState = MatchStack.Pop();
            do
            {
                State nextState = MatchStack.Pop();
                resList = 
                    _addrset.ForwardSearchNode(delegate(GraphNode node)
                {
                    return node.Name == nextState.Name;
                },
                TopState.NodeList);
                
                //if (resList.Count > 1)
                //{
                //    result.ResultState = MatchResultState.MULTIMATCHED;
                //    return result;
                //}
            } while (MatchStack.Count > 0);

            result.Result = resList.First();
            result.ResultState = MatchResultState.SUCCESS;

            return result;

        }

        public MatchResult FuzzyMatch()
        {

            return new MatchResult();
        }

        private State FilterState(State correntState,State preState)
        {
            correntState.NodeList.RemoveAll(delegate(GraphNode node)
            {
                return !LocalMatchRule(preState, node);
            });
            if (correntState.NodeList.Count() == 0)
            {
                correntState.MaxStateLEVEL = LEVEL.Uncertainty;
                correntState.MinStateLEVEL = LEVEL.Uncertainty;
                correntState.NodeCount = 0;
                correntState.NodeList = null;
            }
            else
            {
                //--------------------TODO 待改进
                LEVEL min = correntState.NodeList.First().NodeLEVEL;
                LEVEL max = correntState.NodeList.First().NodeLEVEL;

                foreach (GraphNode node in correntState.NodeList)
                {
                    min = min < node.NodeLEVEL ? min : node.NodeLEVEL;
                    max = max > node.NodeLEVEL ? max : node.NodeLEVEL;
                }

                correntState.MaxStateLEVEL = max;
                correntState.MinStateLEVEL = min;
                correntState.NodeCount = correntState.NodeList.Count();

            }
            return correntState;
        }
        

        #region -----------------------GET or SET-------------------------

        public void SetMatchRule(MatchRule rule)
        {
            LocalMatchRule = rule;
        }


        #endregion

  
    }
}
