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
                predecessor = new Dictionary<State, State>();
                openNodeStack.Push(new StateDto(0,s));
                
                predecessor.Add(s, null);
                var wasCutOffReached = false;
            
                while (openNodeStack.Count > 0)
                {
                    var stateDto = (StateDto)openNodeStack.Peek();
                    if (stateDto.WasExpanded)
                    {
                        RemoveDeadBranchState(openNodeStack, stateDto);
                        continue;
                    }
                    var currentState = stateDto.State;
                    if (currentState.isFinal())
                    {
                        var end = DateTime.Now;
                        searchTime = (end - start);
                        var GVAL = stateDto.Depth;
                        printMessage("search ended in " + searchTime.TotalSeconds + " seconds, plan length: " + GVAL, quiet);
                        printSearchStats(quiet);
                        result = extractSolution(currentState);
                        solutionCost = GVAL;
                        printMessage("--- search ended ---", quiet);
                        return;
                    }
                    var currentGValue = stateDto.Depth;
                    if (currentGValue + 1 > cutOff)
                    {
                        // set cut off reached let know we can try again with greater depth
                        wasCutOffReached = true;
                        RemoveDeadBranchState(openNodeStack, stateDto);
                        continue;
                    }
                    currentState.getSuccessors(successors);
                    stateDto.WasExpanded = true;
                    branchingSum += successors.Count;
                    branchingCount++;
                    
                    foreach (var item in successors)
                    {
                        var state = item.state;

                        if (predecessor.ContainsKey(state)) continue;
                        var gVal = currentGValue + item.cost;
                        openNodeStack.Push(new StateDto(gVal,state));
                        predecessor.Add(state, currentState);
                    }

                }
                printSearchStats(quiet);
                if (!wasCutOffReached) break;
            }
            printMessage("No solution exists.", quiet);
        }

        private void RemoveDeadBranchState(Stack openNodeStack, StateDto stateDto)
        {
            openNodeStack.Pop();
            predecessor.Remove(stateDto.State);
        }
    }
}