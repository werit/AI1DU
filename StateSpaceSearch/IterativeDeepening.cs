using System;
using System.Collections;
using System.Collections.Generic;

namespace StateSpaceSearch
{
    class IterativeDeepening : SearchEngine
    {
        private class StateDto
        {
            public StateDto(int depth,State state)
            {
                Depth = depth;
                State = state;
                WasExpanded = false;
            }
            public int Depth { get; set; }
            public State State { get; set; }
            public bool WasExpanded { get; set; }
        }
        

        public override void search(State s)
        {
            branchingCount = 1;
            branchingSum = 1;
            maxGVal = 0;
            var openNodeStack = new Stack();
            printMessage("--- search started ---", quiet);
            start = DateTime.Now;
            for (var cutOff = 1; cutOff < int.MaxValue; cutOff++)
            {
                State currentState = null;
                predecessor = new Dictionary<State, State>();
                gValues.Clear();
                openNodes.clear();
                
                openNodes.insert(0, s);
                openNodeStack.Push(new StateDto(0,s));
                gValues.Add(s, new StateInformation());
                predecessor.Add(s, null);
                var wasCutOffReached = false;
            
                while (openNodeStack.Count > 0)
                {
                    var stateDto = (StateDto)openNodeStack.Peek();
                    if (stateDto.WasExpanded)
                    {
                        openNodeStack.Pop();
                        predecessor.Remove(stateDto.State);
                        continue;
                    }
                    currentState = stateDto.State;
                    /*if (gValues[currentState].isClosed)
                        continue;
                    addToClosedList(currentState);*/
                    if (currentState.isFinal())
                    {
                        DateTime end = DateTime.Now;
                        searchTime = (end - start);
                        int GVAL = stateDto.Depth;
                        printMessage("search ended in " + searchTime.TotalSeconds + " seconds, plan length: " + GVAL, quiet);
                        printSearchStats(quiet);
                        this.result = extractSolution(currentState);
                        this.solutionCost = GVAL;
                        printMessage("--- search ended ---", quiet);
                        return;
                    }
                    int currentGValue = stateDto.Depth;
                    if (currentGValue + 1 > cutOff)
                    {
                        wasCutOffReached = true;
                        openNodeStack.Pop();
                        predecessor.Remove(stateDto.State);
                        continue;
                    }
                    currentState.getSuccessors(successors);
                    stateDto.WasExpanded = true;
                    branchingSum += successors.Count;
                    branchingCount++;
                    
                    foreach (var item in successors)
                    {
                        State state = item.state;
                        
                        //gValues.Add(s, new StateInformation(gValue));
                        if (!predecessor.ContainsKey(state))
                        {
                            int gVal = currentGValue + item.cost;
                            openNodeStack.Push(new StateDto(gVal,state));
                            predecessor.Add(state, currentState);
                        }

                        //openNodes.insert(gValue, s);
                        //addToOpenList(state, gVal, currentState);
                    }

                }
                printSearchStats(quiet);
                if (!wasCutOffReached) break;
            }
            printMessage("No solution exists.", quiet);
        }
    }
}