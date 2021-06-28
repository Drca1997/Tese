using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{

    private int index;
    private double fCost;
    private double gCost;
    private double hCost;

    private GraphNode previousPathNode;

    public GraphNode(int index)
    {
        this.index = index;
        this.gCost = 0;
        this.fCost = 0;
        this.hCost = int.MaxValue;
        previousPathNode = null;
    }

    public int Index { get => index; set => index = value; }
    public double FCost { get => fCost; set => fCost = value; }
    public double HCost { get => hCost; set => hCost = value; }
    public double GCost { get => gCost; set => gCost = value; }
    public GraphNode PreviousPathNode { get => previousPathNode; set => previousPathNode = value; }
}
