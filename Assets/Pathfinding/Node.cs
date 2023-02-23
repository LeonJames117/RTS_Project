using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // Start is called before the first frame update
    public bool Walkable;
    public Vector3 Pos;
    public Node(bool walkable, Vector3 pos)
    {
        Walkable = walkable;
        Pos = pos;
    }
}
