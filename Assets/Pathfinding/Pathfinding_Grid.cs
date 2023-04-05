using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding_Grid : MonoBehaviour
{
    public LayerMask Unwalkable;
    public Vector2 Grid_Size;
    public float Node_Rad;
    Node[,] Grid_Array;
    public bool Show_Debug = false;

    float Node_Diameter;
    int Grid_Length_X, Grid_Length_Y;
    public List<Node> Path;
    private void OnDrawGizmos()
    {
        //Debug Display
        Gizmos.DrawWireCube(transform.position, new Vector3(Grid_Size.x, 1, Grid_Size.y));
        if (Show_Debug)
        {
            if (Grid_Array != null)
            {
                foreach (Node node in Grid_Array)
                {
                    Gizmos.color = (node.Walkable ? Color.green : Color.red);
                    //if (Path != null)
                    //{
                        //print("Path not null");
                        if (Path.Count > 0)
                        {
                            if (Path.Contains(node))
                            {
                                Gizmos.color = Color.black;
                            }
                        }
                    //}
                    else
                    {
                        //print("Null Path");
                    }
                        
                    
                    Gizmos.DrawCube(node.Pos, Vector3.one * (Node_Diameter - .1f));
                }
            }
        }

       
    }
    void Populate_Grid()
    { //Add nodes to grid
        Grid_Array = new Node[Grid_Length_X, Grid_Length_Y];
        Vector3 Grid_Bottom_Left = transform.position - Vector3.right * Grid_Size.x / 2 - Vector3.forward * Grid_Size.y/2;
        for (int x = 0; x < Grid_Length_X; x++)
        {
            for (int y = 0; y < Grid_Length_Y; y++)
            {
                Vector3 Node_Pos = Grid_Bottom_Left + Vector3.right * (x * Node_Diameter + Node_Rad) + Vector3.forward * (y + Node_Rad);
                bool Obstructed = !Physics.CheckSphere(Node_Pos, Node_Rad,Unwalkable);
                Grid_Array[x, y] = new Node(Obstructed, Node_Pos,x,y);
            }
        }
    }

    public Node Find_Node_By_Pos(Vector3 Pos)
    {// Find a specfic node based on a positon input
        float X_Percent = (Pos.x + Grid_Length_X / 2) / Grid_Length_X;
        float Y_Percent = (Pos.z + Grid_Length_Y / 2) / Grid_Length_Y;
        X_Percent = Mathf.Clamp01(X_Percent);
        Y_Percent = Mathf.Clamp01(Y_Percent);

        int Grid_X = Mathf.RoundToInt((Grid_Length_X - 1) * X_Percent);
        int Grid_Y = Mathf.RoundToInt((Grid_Length_Y - 1) * Y_Percent);
        Mathf.RoundToInt(Grid_X);
        Mathf.RoundToInt(Grid_Y);

        return Grid_Array[Grid_X,Grid_Y];
    }
    
    public List<Node> Find_Node_Neighbours(Node Target)
    {
        List<Node> N_List = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int Current_X = Target.Grid_X + x;
                int Current_Y = Target.Grid_Y + y;
                
                //Is node neighbour
                if (Current_X >= 0 && Current_X < Grid_Size.x && Current_Y >= 0 && Current_Y < Grid_Size.y)
                {
                    N_List.Add(Grid_Array[Current_X, Current_Y]);
                }



            }
        }
        return N_List;
    }

    public int Distance_Between_Nodes(Node N_1,Node N_2)
    {
        int X_Distance = Mathf.Abs(N_1.Grid_X - N_2.Grid_X);
        int Y_Distance = Mathf.Abs(N_1.Grid_Y - N_2.Grid_Y);

        if(X_Distance > Y_Distance)
        {
            return 14 * Y_Distance - 10 * X_Distance - Y_Distance;
        }
        else
        {
            return 14 * X_Distance - 10 * Y_Distance - X_Distance;
        }
    }

    private void Start()    
    {
        Node_Diameter = Node_Rad * 2;
        Grid_Length_X = Mathf.RoundToInt (Grid_Size.x/Node_Diameter);
        Grid_Length_Y = Mathf.RoundToInt (Grid_Size.y/Node_Diameter);

        Populate_Grid();
        
    }


    private void Update()
    {
        foreach (Node n in Grid_Array)
        {
            if (!Physics.CheckSphere(n.Pos, Node_Rad, Unwalkable))
            {
                n.Walkable = true;
            }
            else
            {
                n.Walkable = false;
            }
        }
    }

}
