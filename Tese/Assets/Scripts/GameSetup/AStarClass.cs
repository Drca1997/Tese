using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStarClass
{
    public static List<GraphNode> AStarPathFinding(Graph graph, GraphNode start, List<GraphNode> possibleGoals, Func<GraphNode, List<GraphNode>, double> heuristic)
    {
        List<GraphNode> expandableList = new List<GraphNode> { start }; //pega-se no primeiro nó
        List<GraphNode> visitedList = new List<GraphNode>();

        start.HCost = heuristic(start, possibleGoals);
        start.FCost = start.GCost + start.HCost;
       
        while (expandableList.Count != 0) //enquanto houver nós para serem visitados
        {
            /*Debug.Log("EXPANDABLE LIST:");
            DebugAStarList(expandableList);
            Debug.Log("VISITED LIST:");
            DebugAStarList(visitedList);
            */
            GraphNode node = GetLowestFCostNode(expandableList);
            //Debug.Log("SELECIONOU-SE NÓ " + node.Index);
           
            if (possibleGoals.Contains(node))  //verifica-se se é objetivo
            {
                //Debug.Log("OBJETIVO ENCONTRADO");
                //DebugAStarList(GetPath(node));
                return GetPath(node);

            }
            //se nao for nó objetivo 
            //Debug.Log("NÓ " + node.Index +"ADICIONADO Á LISTA DE VISITADOS");
            expandableList.Remove(node);
            visitedList.Add(node); //adiciona-se à lista de visitados
            //e expande-se 
            List<GraphEdge> edges = graph.EdgesAdjacencyListVector[node.Index];
            for (int i = 0; i < edges.Count; i++)
            {

                GraphNode childNode = graph.Nodes[edges[i].To];
                //Debug.Log("Expandindo para nó filho " + childNode.Index);
                if (visitedList.Contains(childNode)) //caso nó filho já tenha sido visitado
                {
                    //Debug.Log("ESTE NÓ JA FOI VISITADO, IGNORA-SE");
                    continue;
                }
                //calcula-se A*score dos nós filhos (g + h)-> valor das arestas + h do nó em causa
                double gCost = node.GCost + edges[i].Cost;

                if (childNode.HCost == int.MaxValue) //calcula hCost se ainda não o foi
                {
                    childNode.HCost = heuristic(childNode, possibleGoals);
                    //Debug.Log("H DE NÓ " + childNode.Index + "= " + childNode.HCost);
                }
                double fCost = childNode.HCost + gCost;

                //Previne casos quando nó filho ja esta na expandable list, com Fscore maior
                if (expandableList.Contains(childNode))
                {

                    if (childNode.FCost > fCost)
                    {
                        //Debug.Log("NÓ FILHO NA OPENLIST, E ENCONTRADO F MENOR");
                        childNode.FCost = fCost;
                        childNode.GCost = gCost;
                        childNode.PreviousPathNode = node;
                        //Debug.Log("NOVO F: " + childNode.FCost);
                    }
                }
                else
                {
                    //Debug.Log("NÓ " + childNode.Index+" ADICIONADO À OPENLIST, POSSIVEL DE SER EXPANDIDO");
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
    public static List<Action> AStarForPlanning(PlanningAgent agent, GraphNode start, GraphNode end, Action[] possibleActions, GoalTemplate goal)
    {
        List<GraphNode> expandableList = new List<GraphNode> { start }; //pega-se no primeiro nó
        List<GraphNode> visitedList = new List<GraphNode>();
      
        start.HCost = goal.Heuristic((ActionStateGraphNode)start, (ActionStateGraphNode)end);
        start.FCost = start.GCost + start.HCost;
        int ind = 2;

        while (expandableList.Count != 0) //enquanto houver nós para serem visitados
        {
           
            ActionStateGraphNode node = GetLowestFCostNodePlanning(expandableList);
            agent.SimulatedX = node.AgentPos[0];
            agent.SimulatedY = node.AgentPos[1];

            Debug.Log("SELECIONOU-SE NÓ " + node.Index);
            Debug.Log(node.DebugWorld());
            Debug.Log("SimX: " + agent.SimulatedX + ", SimY: " + agent.SimulatedY);
            if (goal.IsObjective(node))  //verifica-se se é objetivo
            {
                Debug.Log("OBJETIVO ENCONTRADO");
                
                return GetPlan(node);
            }
            
            
            //agent.SimulatedPlantedBomb = node.AgentPlantedBomb;
            //se nao for nó objetivo 
            Debug.Log("NÓ " + node.Index + " ADICIONADO Á LISTA DE VISITADOS");
            Debug.Log(node.DebugWorld());
            expandableList.Remove(node);
            visitedList.Add(node); //adiciona-se à lista de visitados
            //e expande-se 
            //
            //List<GraphEdge> edges = graph.EdgesAdjacencyListVector[node.Index];
            List<Action> actions = GetPossibleActions(goal, node, possibleActions);
            
            for (int i = 0; i < actions.Count; i++)
            {

                //GraphNode childNode = graph.Nodes[edges[i].To];
                Debug.Log(actions[i].GetType());
                Debug.Log(actions[i].DebugWorld());
                actions[i].UpdateEffectGrid(node.Grid);
                Debug.Log(actions[i].DebugWorld());
                Debug.Log("SimX: " + agent.SimulatedX + ", SimY: " + agent.SimulatedY);
                actions[i].Simulate();
                Debug.Log(actions[i].DebugWorld());
                Debug.Log("SimX: " + agent.SimulatedX + ", SimY: " + agent.SimulatedY);
                ActionStateGraphNode childNode = new ActionStateGraphNode(ind, agent, actions[i].Effect);
                ind++;

                Debug.Log("Expandindo para nó filho " + childNode.Index);
               
                Debug.Log(childNode.DebugWorld());
                if (ListContainsNode(visitedList, childNode)) //caso nó filho já tenha sido visitado
                {
                    Debug.Log("ESTE NÓ JA FOI VISITADO, IGNORA-SE");
                    actions[i].Revert();
                    continue;
                }
                //calcula-se A*score dos nós filhos (g + h)-> valor das arestas + h do nó em causa
                double gCost = node.GCost + actions[i].Cost;

                if (childNode.HCost == int.MaxValue) //calcula hCost se ainda não o foi
                {
                    childNode.HCost = goal.Heuristic(childNode, (ActionStateGraphNode)end);
                    //Debug.Log("H DE NÓ " + childNode.Index + "= " + childNode.HCost);
                }
                double fCost = childNode.HCost + gCost;

                //Previne casos quando nó filho ja esta na expandable list, com Fscore maior
                if (ListContainsNode(expandableList, childNode))
                {

                    if (childNode.FCost > fCost)
                    {
                        //Debug.Log("NÓ FILHO NA OPENLIST, E ENCONTRADO F MENOR");
                        childNode.FCost = fCost;
                        childNode.GCost = gCost;
                        childNode.PreviousPathNode = node;
                        childNode.ActionTakenToReachHere = actions[i];
                        //Debug.Log("NOVO F: " + childNode.FCost);
                    }
                }
                else
                {
                    //Debug.Log("NÓ " + childNode.Index + " ADICIONADO À OPENLIST, POSSIVEL DE SER EXPANDIDO");
                 
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

    private static ActionStateGraphNode GetLowestFCostNodePlanning(List<GraphNode> list)
    {
        ActionStateGraphNode best = (ActionStateGraphNode)list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].FCost < best.FCost)
            {
                best = (ActionStateGraphNode)list[i];
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

    private static List<Action> GetPlan(ActionStateGraphNode node)
    {
        
        List<Action> actionsTaken = new List<Action>();
        
        ActionStateGraphNode currentNode = node;
        while (currentNode.PreviousPathNode != null)
        {
            
            actionsTaken.Add(currentNode.ActionTakenToReachHere);
            currentNode = (ActionStateGraphNode)currentNode.PreviousPathNode;
            
        }
        actionsTaken = GetOppositeActions(actionsTaken);
        return actionsTaken;
    }

    private static List<Action> GetOppositeActions(List<Action> actions)
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

    public static int CalculateManhattanDistance(int[] start, int[] end)
    {
        return Mathf.Abs(end[0] - start[0]) + Mathf.Abs(end[1] - start[1]);
    }

    public static double ManhattanDistanceHeuristic(GraphNode node, List<GraphNode> goals)
    {
        int best = int.MaxValue;
        foreach (GraphNode goal in goals)
        {

            int dist = CalculateManhattanDistance(Utils.GetTileFromIndex(node.Index, Utils.gridWidth), Utils.GetTileFromIndex(goal.Index, Utils.gridWidth));
            if (dist < best)
            {
                best = dist;
            }
        }
        return best;
    }

    private static List<Action> GetPossibleActions(GoalTemplate goal, ActionStateGraphNode node, Action[] possibleActions)
    {
        List<Action> res = new List<Action>();
        if (goal.GetType() == typeof(GoalAttackEnemy))
        {
            if (node.Grid[node.AgentPos[0], node.AgentPos[1]] == 5)
            {
                res.Add(FindBombAction(possibleActions));
                return res;
            }
        }
        foreach (Action action in possibleActions)
        {
            if (action.CheckPreconditions())
            {
                res.Add(action);
            }
        }
        return res;
    }

    private static Action FindBombAction(Action [] possibleActions)
    {
        foreach (Action action in possibleActions)
        {
            if (action.GetType() == typeof(PlantBombAction))
            {
                return action;
            }
        }
        return null;
    }
    private static bool ListContainsNode(List<GraphNode> list, ActionStateGraphNode node)
    {
        
        foreach(ActionStateGraphNode no in list)
        {
            bool equal = true;
            foreach(int[] tile in Utils.GridIterator(no.Grid))
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

}
