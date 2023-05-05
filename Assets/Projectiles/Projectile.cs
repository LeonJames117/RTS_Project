using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{

    public Pathfinding_Grid Grid;
    int Blast_Radius = 2;
    bool Incoming_Resolved=false;
    public Unit_Manager Unit_Manager;
    Ray ray;
    RaycastHit hit;
    List<Node> Nodes_In_Fire;
    // Start is called before the first frame update
    void Start()
    {

        Nodes_In_Fire = new List<Node>();


    }

    void On_Incoming(Vector3 Impact_Site)
    {
        Node Impact_Node = Grid.Find_Node_By_Pos(Impact_Site);
        
        print("Impact Node X " + Impact_Node.Grid_X + " Impact Node Y "+ Impact_Node.Grid_Y);
        int i = 0;
        for (int x = -Blast_Radius; x < Blast_Radius; x++)
        {
            for (int y = -Blast_Radius; y < Blast_Radius; y++)
            {
                int NodeX = Impact_Node.Grid_X + x;
                int NodeY = Impact_Node.Grid_Y + y;
                if(Grid.Find_Node_By_Grid(NodeX, NodeY) != null)
                {
                    Nodes_In_Fire.Add(Grid.Find_Node_By_Grid(NodeX, NodeY));
                    print("Node X: " + Nodes_In_Fire[i].Grid_X + " Impact Node Y: " + Nodes_In_Fire[i].Grid_Y + " Added to Nodes in fire");
                }
                else
                {
                    print("Null Node in fire");
                }
                    
            }
            i++;
        }

        foreach(Node Node in Nodes_In_Fire)
        {
            Node.Threat = 20;
        }
        foreach(Unit unit in Unit_Manager.All_Units)
        {
            unit.Update_Path();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Incoming_Resolved && Physics.Raycast(transform.position,transform.TransformDirection(Vector3.down), out hit, 30))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
            print("Raycast Hit");
            On_Incoming(hit.point);
            Incoming_Resolved = true;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(Node Node in Nodes_In_Fire)
        {
            Gizmos.DrawCube(Node.Pos, Vector3.one * (0.5f - .1f));
        }
    }
}
