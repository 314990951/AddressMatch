using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AddressMatch.Training
{
    public class TrainMachine
    {
        public static UInt16 LevelMask = 0x00FF;

        public static UInt16 PlaceMask = 0xFF00;

        private AddrSet _addrset;


        // TODO  uncompleted
        private bool defualtRule(State state, InsertElement el)
        {
            if ((el.Mode & PlaceMask) == (UInt16)InsertMode.OldPlace)
            {
                if (state.NodeCount == 0)
                {
                    //throw new TrainException(el, "Existed Node is not found, rule failed");
                    return false;
                }
                //LEVEL judgement
                if ((el.Mode & LevelMask) == (UInt16)InsertMode.ExactlyLevel && el.Level != LEVEL.Uncertainty)
                {
                    if (el.Level < state.MinStateLEVEL || el.Level > state.MaxStateLEVEL)
                    {
                        //throw new TrainException(el, "Existed Node is not found, rule failed");
                        return false;
                    }
                }
                //LEVEL judgement
                //if ((el.Mode & LevelMask) == (UInt16)InsertMode.DegradeLevel && el.Level != LEVEL.Uncertainty)
                //{
                //    if (el.Level < state.MinStateLEVEL)
                //    {
                //        //throw new TrainException(el, "Existed Node is not found, rule failed");
                //        return false;
                //    }
                //}
                ////LEVEL judgement
                //if ((el.Mode & LevelMask) == (UInt16)InsertMode.UpgradeLevel && el.Level != LEVEL.Uncertainty)
                //{
                //    if (el.Level > state.MaxStateLEVEL)
                //    {
                //        //throw new TrainException(el, "Existed Node is not found, rule failed");
                //        return false;
                //    }
                //}
            }
            else if ((el.Mode & PlaceMask) == (UInt16)InsertMode.NewPlace)
            {

            }
            else if ((el.Mode & PlaceMask) == (UInt16)InsertMode.AutoPlace)
            {

            }

            return true;
        }

        public TrainMachine(AddrSet addrset)
        {
            if (addrset == null)
            {
                throw new Exception("AddrSet is not initialized");
            }
            if (AddrSet.AddrGraph == null)
            {
                throw new Exception("Graph is not initialized");
            }

            _addrset = addrset;

            //LocalTrainRule = defualtRule;

            //init();
        }

        /// <summary>
        /// Train the AddrSet, string must be wrapped in InsertElement
        /// </summary>
        /// <param name="list">InsertElements' List</param>
        /// <param name="withException">Indicate whether throw exception,
        ///     if param is false, function will return 0 if train failed, if param is true, 
        ///     function will throw TrainException with detail information </param>
        /// <returns></returns>
        public int Train(List<InsertElement> list, bool withException)
        {
            if (withException)
            {
                return TrainWithException(list);
            }
            else
            {
                return TrainWithNOException(list);
            }
            
        }

        private int TrainWithException(List<InsertElement> list)
        {
            List<State> StateList = new List<State>(list.Count);

            ReaderWriterLockSlim rwlock = _addrset.GetRWlock();
            rwlock.EnterWriteLock();

            for (int i = 0; i < list.Count; i++)
            {
                State tmpState = PreliminaryCheck(list[i]);
                if (tmpState == null)
                {
                    rwlock.ExitWriteLock();
                    return 0;
                }
                StateList.Add(tmpState);
            }


            bool res = _insert(list, StateList);

            rwlock.ExitWriteLock();

            if (res)
            {
                return 1;
            }
            else
            {
                return 0;
            }

            
           
        }
        //  TODO  Uncompleted
        private int TrainWithNOException(List<InsertElement> list)
        {
            return 1;
        }

        //Preliminary check element is legality or not
        private State PreliminaryCheck(InsertElement el)
        {
            State resultState = _addrset.FindNodeInHashTable(el.Name);

            if (defualtRule(resultState, el))
            {
                return resultState;
            }
            else
            {
                throw new TrainException(el, @"element is not legality");
            }
            

        }

        // TODO  Construct transaction and support roll back?
        private bool _insert(List<InsertElement> ElementList,List<State> StateList)
        {
            int start = GetTopPostionIndex(ElementList, StateList);

            GraphNode fathernode = null;
            //if linked to root? 
            if (start == 0 && StateList[0].NodeCount == 0)
            {
                GraphNode node = new GraphNode(ElementList[0].Name, ElementList[0].Level);
                fathernode = node;
                _addrset.Insert(node, AddrSet.AddrGraph.root);
                start++;
            }
            //TODO  need test more
            fathernode = fathernode != null ? fathernode : StateList[start-1].NodeList.Single();

            for (; start < ElementList.Count; start++)
            {
                #region OldPlace
                if ((ElementList[start].Mode & PlaceMask) == (UInt16)InsertMode.OldPlace)
                {
                    if (StateList[start].NodeCount == 1)
                    {
                        fathernode = fathernode != null ? fathernode : StateList[start].NodeList.Single();
                    }
                    else if (StateList[start].NodeCount > 1)
                    {

                        List<GraphNode> searchresult = _addrset.ForwardSearchNode(delegate(GraphNode node)
                        {
                            return node.Name == ElementList[start].Name 
                                        && node.NodeLEVEL == ElementList[start].Level;
                        }, fathernode);

                        if (searchresult.Count > 1)
                        {
                            throw new TrainException(ElementList[start], "Existed Node is Multi-matched");
                        }
                        if (searchresult.Count == 0)
                        {
                            throw new TrainException(ElementList[start], "Existed Node is not founded");
                        }
                        if (searchresult.Count == 1)
                        {
                            fathernode = searchresult.Single();
                        }
                    }

                }
                #endregion

                #region NewPalce
                if ((ElementList[start].Mode & PlaceMask) == (UInt16)InsertMode.NewPlace)
                {
                    MatchHelper.Assert(fathernode == null,
                                                 @" FUCTION _insert ERROR, 
                                                         father node is null, bug may be exist in Func GetTopPostionIndex  ");

                    GraphNode node = new GraphNode(ElementList[start].Name, ElementList[start].Level);

                    //judge the Level
                    if ((ElementList[start].Mode & LevelMask) == (UInt16)InsertMode.ExactlyLevel)
                    {
                        if (fathernode.NodeLEVEL > ElementList[start].Level)
                        {
                            throw new TrainException(ElementList[start], @"LEVEL smaller than father node, insert failed");
                        }
                    }
                    if ((ElementList[start].Mode & LevelMask) == (UInt16)InsertMode.DegradeLevel 
                                     || (ElementList[start].Mode & LevelMask) == (UInt16)InsertMode.AutoLevel)
                    {
                        if (fathernode.NodeLEVEL > ElementList[start].Level)
                        {
                            node.NodeLEVEL = fathernode.NodeLEVEL != LEVEL.Uncertainty ? fathernode.NodeLEVEL + 1 : LEVEL.Uncertainty;
                        }
                    }
                    _addrset.Insert(node, fathernode);

                    fathernode = node;


                }
                #endregion

                #region AutoPlace
                if ((ElementList[start].Mode & PlaceMask) == (UInt16)InsertMode.AutoPlace)
                {
                    if ((ElementList[start].Mode & LevelMask) == (UInt16)InsertMode.ExactlyLevel)
                    {
                        GraphNode node = new GraphNode(ElementList[start].Name, ElementList[start].Level);

                        List<GraphNode> searchresult = _addrset.ForwardSearchNode(delegate(GraphNode gn)
                        {
                            return gn.Name == ElementList[start].Name
                                && gn.NodeLEVEL == ElementList[start].Level;
                        }, fathernode);

                        if (searchresult.Count >= 1)
                        {
                            fathernode = searchresult.First();
                        }
                        if (searchresult.Count == 0)
                        {
                            
                            _addrset.Insert(node, fathernode);

                            fathernode = node;
                        }
                    }

                    if ((ElementList[start].Mode & LevelMask) == (UInt16)InsertMode.DegradeLevel
                                || (ElementList[start].Mode & LevelMask) == (UInt16)InsertMode.AutoLevel)
                    {
                        GraphNode node = new GraphNode(ElementList[start].Name, ElementList[start].Level);

                        List<GraphNode> searchresult = _addrset.ForwardSearchNode(delegate(GraphNode gn)
                        {
                            return gn.Name == ElementList[start].Name;
                        }, fathernode);

                        if (searchresult.Count >= 1)
                        {
                            fathernode = searchresult.First();    
                        }

                        if (searchresult.Count == 0)
                        {
                            node.NodeLEVEL = fathernode.NodeLEVEL != LEVEL.Uncertainty ? fathernode.NodeLEVEL + 1 : LEVEL.Uncertainty;

                            _addrset.Insert(node, fathernode);

                            fathernode = node;
                        }
                    }
                }
                #endregion
            }

            return true;
        }

        private int GetTopPostionIndex(List<InsertElement> ElementList, List<State> StateList)
        {
            MatchHelper.Assert(ElementList.Count != StateList.Count,
                                                 @" FUCTION GetTopPostionIndex ERROR, 
                                                         ElementList's Length not equal to StateList's Length   ");
            int index = -1;

            for (int i = 0; i < ElementList.Count; i++)
            {
                // is new place and  index = 0, inserted node is root
                if ((ElementList[i].Mode & PlaceMask) == (UInt16)InsertMode.NewPlace)
                {
                    index = i;
                    break;
                }
                if ((ElementList[i].Mode & PlaceMask) == (UInt16)InsertMode.OldPlace)
                {
                    if (StateList[i].NodeCount >1)
                    {
                        throw new TrainException(ElementList[i], "Top Insert position is multi-matched, insert failed");
                    }
                    else if (StateList[i].NodeCount == 0)
                    {
                        throw new TrainException(ElementList[i], "Top Insert position is non-matched, insert failed");
                    }
                    else
                    {
                        index = i + 1;
                        // continue loop
                        continue;
                    }
                }
                if ((ElementList[i].Mode & PlaceMask) == (UInt16)InsertMode.AutoPlace)
                {
                    if (StateList[i].NodeCount == 1)
                    {
                        index = i + 1;
                        continue;
                    }
                    else if (StateList[i].NodeCount > 1)
                    {
                        throw new TrainException(ElementList[i], "Insert element list is not sufficient, insert failed");
                    }
                    else if (StateList[i].NodeCount == 0)
                    {
                        index = i;
                        break;
                    }
                }

            }

            MatchHelper.Assert(index == -1,
                                     @" FUCTION GetTopPostionIndex ERROR, 
                                                         judge position index error, skip all if statement  ");

            return index;

        }



    }
}
