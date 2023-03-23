using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // Start is called before the first frame update
    public bool Walkable;
    public Vector3 Pos;
    public int Grid_X, Grid_Y;

    public int G_Cost;
    public int H_Cost;
    public Node Parent;

    public int F_Cost { get { return G_Cost + H_Cost; } }
    public Node(bool walkable, Vector3 pos, int grid_X, int grid_y)
    {
        Walkable = walkable;
        Pos = pos;
        Grid_X = grid_X;
        Grid_Y = grid_y;
    }
}
