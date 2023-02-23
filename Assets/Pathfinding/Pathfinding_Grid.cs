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
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(Grid_Size.x, 1, Grid_Size.y));
        if (Show_Debug)
        {
            if (Grid_Array != null)
            {
                foreach (Node node in Grid_Array)
                {
                    Gizmos.color = (node.Walkable ? Color.green : Color.red);
                    Gizmos.DrawCube(node.Pos, Vector3.one * (Node_Diameter - .1f));
                }
            }
        }

       
    }
    void Populate_Grid()
    {
        Grid_Array = new Node[Grid_Length_X, Grid_Length_Y];
        Vector3 Grid_Bottom_Left = transform.position - Vector3.right * Grid_Size.x / 2 - Vector3.forward * Grid_Size.y/2;
        for (int x = 0; x < Grid_Length_X; x++)
        {
            for (int y = 0; y < Grid_Length_Y; y++)
            {
                Vector3 Node_Pos = Grid_Bottom_Left + Vector3.right * (x * Node_Diameter + Node_Rad) + Vector3.forward * (y + Node_Rad);
                bool Obstructed = !Physics.CheckSphere(Node_Pos, Node_Rad,Unwalkable);
                Grid_Array[x, y] = new Node(Obstructed, Node_Pos);
            }
        }
    }

    public Node Find_Node_By_Pos(Vector3 Pos)
    {
        float X_Percent = (Pos.x + Grid_Length_X / 2 / Grid_Length_X);
        float Y_Percent = (Pos.z + Grid_Length_Y / 2 / Grid_Length_Y);
        X_Percent = Mathf.Clamp01(X_Percent);
        Y_Percent = Mathf.Clamp01(Y_Percent);

        int Grid_X = (Grid_Length_X - 1) * (int)X_Percent;
        int Grid_Y = (Grid_Length_Y - 1) * (int)Y_Percent;
        Mathf.RoundToInt(Grid_X);
        Mathf.RoundToInt(Grid_Y);

        return Grid_Array[Grid_X,Grid_Y];
    }

    private void Start()    
    {
        Node_Diameter = Node_Rad * 2;
        Grid_Length_X = Mathf.RoundToInt (Grid_Size.x/Node_Diameter);
        Grid_Length_Y = Mathf.RoundToInt (Grid_Size.y/Node_Diameter);

        Populate_Grid();
        
    }

  
}
