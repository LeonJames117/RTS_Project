using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : Heap_Item_Interface<Node>
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

    //Heap
    int Heap_Ind;
    public int Heap_Index {
        get {
            return Heap_Ind;
        }
        set {
            Heap_Ind = value;
        }
    }

    public int CompareTo(Node OtherN)
    {
        int Compare = F_Cost.CompareTo(OtherN.F_Cost);
        if(Compare == 0)
        {
            Compare = H_Cost.CompareTo(OtherN.H_Cost);
        }
        return -Compare;// Int CompareTo returns 1 if higher, so needs to be inverted
    }
}
