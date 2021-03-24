using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphEdge 
{
    private int from;
    private int to;
    private double cost;
    
    

    public GraphEdge(int from, int to, double cost)
    {
        this.from = from;
        this.to = to;
        this.cost = cost;
    }

    public GraphEdge(int from, int to)
    {
        this.from = from;
        this.to = to;
        this.cost = 1.0;
    }

    public int From { get => from; set => from = value; }
    public int To { get => to; set => to = value; }
    public double Cost { get => cost; set => cost = value; }
    
}
