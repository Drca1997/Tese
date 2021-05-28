using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    private static bool pathfindingVerbose = false;
    private static bool planningVerbose = false;

    #region Pathfinding
    public static List<GraphNode> AStarPathFinding(Graph graph, GraphNode start, List<GraphNode> possibleGoals, Func<GraphNode, List<GraphNode>, double> heuristic)
    {
        List<GraphNode> expandableList = new List<GraphNode> { start }; //pega-se no primeiro nó
        List<GraphNode> visitedList = new List<GraphNode>();

        start.HCost = heuristic(start, possibleGoals);
        start.FCost = start.GCost + start.HCost;

        while (expandableList.Count != 0){ //enquanto houver nós para serem visitados
        
            if (pathfindingVerbose)
            {
                Debug.Log("EXPANDABLE LIST:");
                DebugAStarList(expandableList);
                Debug.Log("VISITED LIST:");
                DebugAStarList(visitedList);
            
            }


            GraphNode node = GetLowestFCostNode(expandableList);
            if (pathfindingVerbose)
                Debug.Log("SELECIONOU-SE NÓ " + node.Index);

            if (possibleGoals.Contains(node))  //verifica-se se é objetivo
            {
                if (pathfindingVerbose)
                {
                    Debug.Log("OBJETIVO ENCONTRADO");
                    DebugAStarList(GetPath(node));
                }
                return GetPath(node);

            }
            //se nao for nó objetivo 
            if (pathfindingVerbose)
                Debug.Log("NÓ " + node.Index +"ADICIONADO Á LISTA DE VISITADOS");
            expandableList.Remove(node);
            visitedList.Add(node); //adiciona-se à lista de visitados
            //e expande-se 
            List<GraphEdge> edges = graph.EdgesAdjacencyListVector[node.Index];
            for (int i = 0; i < edges.Count; i++)
            {

                GraphNode childNode = graph.Nodes[edges[i].To];
                if (pathfindingVerbose)
                    Debug.Log("Expandindo para nó filho " + childNode.Index);
                if (visitedList.Contains(childNode)) //caso nó filho já tenha sido visitado
                {
                    if (pathfindingVerbose) 
                        Debug.Log("ESTE NÓ JA FOI VISITADO, IGNORA-SE");
                    continue;
                }
                //calcula-se A*score dos nós filhos (g + h)-> valor das arestas + h do nó em causa
                double gCost = node.GCost + edges[i].Cost;

                if (childNode.HCost == int.MaxValue) //calcula hCost se ainda não o foi
                {
                    childNode.HCost = heuristic(childNode, possibleGoals);
                    if (pathfindingVerbose) 
                        Debug.Log("H DE NÓ " + childNode.Index + "= " + childNode.HCost);
                }
                double fCost = childNode.HCost + gCost;

                //Previne casos quando nó filho ja esta na expandable list, com Fscore maior
                if (expandableList.Contains(childNode))
                {

                    if (childNode.FCost > fCost)
                    {
                        if (pathfindingVerbose)
                            Debug.Log("NÓ FILHO NA OPENLIST, E ENCONTRADO F MENOR");
                        childNode.FCost = fCost;
                        childNode.GCost = gCost;
                        childNode.PreviousPathNode = node;
                        if (pathfindingVerbose) 
                            Debug.Log("NOVO F: " + childNode.FCost);
                    }
                }
                else
                {
                    if (pathfindingVerbose) 
                        Debug.Log("NÓ " + childNode.Index+" ADICIONADO À OPENLIST, POSSIVEL DE SER EXPANDIDO");
                    childNode.FCost = fCost;
                    childNode.GCost = gCost;
                    childNode.PreviousPathNode = node;
                    expandableList.Add(childNode); //adiciona-se nós filhos à lista de nós expandíveis
                }
            }
        }
        Debug.Log("Falha a encontrar caminho");
        return null; //se já se visitou todos os nós possíveis e nenhum deles é objetivo
    }

    //Passível de optimização, efetuando procura binária
    private static GraphNode GetLowestFCostNode(List<GraphNode> list)
    {
        GraphNode best = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].FCost < best.FCost)
            {
                best = list[i];
            }
        }
        return best;
    }

    private static List<GraphNode> GetPath(GraphNode goalNode)
    {
        List<GraphNode> solution = new List<GraphNode>();
        solution.Add(goalNode);
        GraphNode currentNode = goalNode;
        while (currentNode.PreviousPathNode != null)
        {
            solution.Add(currentNode.PreviousPathNode);
            currentNode = currentNode.PreviousPathNode;
        }
        solution.Reverse();
        return solution;
    }
    #endregion

    public static List<SymbolicAction> AStarForPlanning(PlanningSyntheticPlayer agent, GraphNode start, GraphNode end, SymbolicAction[] possibleActions, Goal goal)
    {
        List<GraphNode> expandableList = new List<GraphNode> { start }; //pega-se no primeiro nó
        List<GraphNode> visitedList = new List<GraphNode>();

        start.HCost = goal.Heuristic((WorldNode)start, (WorldNode)end);
        start.FCost = start.GCost + start.HCost;
        int ind = 2;

        while (expandableList.Count != 0) //enquanto houver nós para serem visitados
        {

            WorldNode node = GetLowestFCostNodePlanning(expandableList);
            agent.SimulatedX = node.AgentPos[0];
            agent.SimulatedY = node.AgentPos[1];

            if (planningVerbose)
            {
                Debug.Log("SELECIONOU-SE NÓ " + node.Index);
                Debug.Log(node.DebugWorld());
                Debug.Log("SimX: " + agent.SimulatedX + ", SimY: " + agent.SimulatedY);
            }
           
            if (goal.IsObjective(node))  //verifica-se se é objetivo
            {
                if (planningVerbose)
                    Debug.LogWarning("OBJETIVO ENCONTRADO");

                return GetPlan(node);
            }


            //agent.SimulatedPlantedBomb = node.AgentPlantedBomb;
            //se nao for nó objetivo
            if (planningVerbose)
                Debug.Log("NÓ " + node.Index + " ADICIONADO Á LISTA DE VISITADOS");
           
            expandableList.Remove(node);
            visitedList.Add(node); //adiciona-se à lista de visitados
            //e expande-se 
            //
            //List<GraphEdge> edges = graph.EdgesAdjacencyListVector[node.Index];
            List<SymbolicAction> actions = GetPossibleActions(goal, node, possibleActions);

            for (int i = 0; i < actions.Count; i++)
            {

                //GraphNode childNode = graph.Nodes[edges[i].To];
                if (planningVerbose)
                {
                    Debug.Log(actions[i].GetType());
                    
                }
                   
                actions[i].UpdateEffectGrid(node.Grid);
                
                actions[i].Simulate();
                if (planningVerbose)
                {
                    Debug.Log(actions[i].DebugWorld());
                    Debug.Log("SimX: " + agent.SimulatedX + ", SimY: " + agent.SimulatedY);
                }
                
                WorldNode childNode = new WorldNode(ind, agent, actions[i].Effect);
                ind++;

                if (planningVerbose)
                {
                    Debug.Log("Expandindo para nó filho " + childNode.Index);

                    Debug.Log(childNode.DebugWorld());
                }
               
                if (ListContainsNode(visitedList, childNode)) //caso nó filho já tenha sido visitado
                {
                    if (planningVerbose)
                        Debug.Log("ESTE NÓ JA FOI VISITADO, IGNORA-SE");
                    actions[i].Revert();
                    continue;
                }
                //calcula-se A*score dos nós filhos (g + h)-> valor das arestas + h do nó em causa
                double gCost = node.GCost + actions[i].Cost;

                if (childNode.HCost == int.MaxValue) //calcula hCost se ainda não o foi
                {
                    childNode.HCost = goal.Heuristic(childNode, (WorldNode)end);
                    if (planningVerbose)
                        Debug.Log("H DE NÓ " + childNode.Index + "= " + childNode.HCost);
                }
                double fCost = childNode.HCost + gCost;

                //Previne casos quando nó filho ja esta na expandable list, com Fscore maior
                if (ListContainsNode(expandableList, childNode))
                {

                    if (childNode.FCost > fCost)
                    {
                        if (planningVerbose) 
                            Debug.Log("NÓ FILHO NA OPENLIST, E ENCONTRADO F MENOR");
                        childNode.FCost = fCost;
                        childNode.GCost = gCost;
                        childNode.PreviousPathNode = node;
                        childNode.ActionTakenToReachHere = actions[i];
                        if (planningVerbose) 
                            Debug.Log("NOVO F: " + childNode.FCost);
                    }
                }
                else
                {
                    if (planningVerbose) 
                        Debug.Log("NÓ " + childNode.Index + " ADICIONADO À OPENLIST, POSSIVEL DE SER EXPANDIDO");

                    childNode.FCost = fCost;
                    childNode.GCost = gCost;
                    childNode.PreviousPathNode = node;
                    childNode.ActionTakenToReachHere = actions[i];
                    expandableList.Add(childNode); //adiciona-se nós filhos à lista de nós expandíveis
                }
                actions[i].Revert();
            }
        }
        Debug.Log("FALHA AO ENCONTRAR PLANO");
        return null; //se já se visitou todos os nós possíveis e nenhum deles é objetivo
    }

    private static List<SymbolicAction> GetPossibleActions(Goal goal, WorldNode node, SymbolicAction[] possibleActions)
    {
        List<SymbolicAction> res = new List<SymbolicAction>();
        /*if (goal.GetType() == typeof(AttackEnemyGoal))
        {
            if (node.Grid[node.AgentPos[0], node.AgentPos[1]] == 8) //Tile.PlayerNBomb
            {
                res.Add(FindBombAction(possibleActions));
                return res;
            }
        }*/
        
        foreach (SymbolicAction action in possibleActions)
        {
            if (action.CheckPreconditions(node.Grid))
            {
                //Debug.Log("AÇAO POSSIVEL NO NO " + node.Index + SyntheticPlayerUtils.ActionToString(action.RawAction));   
                res.Add(action);
            }
        }
        return res;
    }

    /*
    private static SymbolicAction FindBombAction(SymbolicAction[] possibleActions)
    {
        foreach (SymbolicAction action in possibleActions)
        {
            if (action.GetType() == typeof(PlantBombAction))
            {
                return action;
            }
        }
        return null;
    }*/

    private static List<SymbolicAction> GetPlan(WorldNode node)
    {

        List<SymbolicAction> actionsTaken = new List<SymbolicAction>();

        WorldNode currentNode = node;
        while (currentNode.PreviousPathNode != null)
        {

            actionsTaken.Add(currentNode.ActionTakenToReachHere);
            currentNode = (WorldNode)currentNode.PreviousPathNode;

        }
        actionsTaken = GetOppositeActions(actionsTaken);
        return actionsTaken;
    }

    private static WorldNode GetLowestFCostNodePlanning(List<GraphNode> list)
    {
        WorldNode best = (WorldNode)list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].FCost < best.FCost)
            {
                best = (WorldNode)list[i];
            }
        }
        return best;

    }

    private static List<SymbolicAction> GetOppositeActions(List<SymbolicAction> actions)
    {

        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].OppositeAction != null)
            {
                actions[i] = actions[i].OppositeAction;
            }
        }
        return actions;
    }

    private static bool ListContainsNode(List<GraphNode> list, WorldNode node)
    {

        foreach (WorldNode no in list)
        {
            bool equal = true;
            foreach (int[] tile in Utils.GridIterator(no.Grid))
            {
                if (no.Grid[tile[0], tile[1]] != node.Grid[tile[0], tile[1]])
                {
                    equal = false;
                    break;
                }
            }
            if (equal)
            {
                return true;
            }
        }
        return false;
    }

    public static double ManhattanDistanceHeuristic(GraphNode node, List<GraphNode> goals)
    {
        int best = int.MaxValue;
        foreach (GraphNode goal in goals)
        {
            
            int dist = SyntheticPlayerUtils.CalculateManhattanDistance(SyntheticPlayerUtils.GetTileFromIndex(node.Index, SyntheticPlayerUtils.gridWidth), SyntheticPlayerUtils.GetTileFromIndex(goal.Index, SyntheticPlayerUtils.gridWidth));
            if (dist < best)
            {
                best = dist;
            }
        }
        return best;
    }

    public static void DebugAStarList(List<GraphNode> list)
    {

        Debug.Log("IN THIS LIST: (with " + list.Count + " nodes)");
        string result = null;
        foreach (GraphNode node in list)
        {
            result += "||NODE " + node.Index + " (F = " + node.FCost
                + ", G = " + node.GCost + ", H = " + node.HCost;
        }

        Debug.Log(result);
    }
}
