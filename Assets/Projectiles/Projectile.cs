using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{

    public Pathfinding_Grid Grid;
    int Blast_Radius = 5;
    bool Incoming_Resolved=false;
    public Unit_Manager Unit_Manager;
    Ray ray;
    RaycastHit hit;
    public Rigidbody rigidbody;
    float Previous_Y = -1;
    Transform Target;
    List<Node> Nodes_In_Fire;
    public int Warning_Distance = 20;
    // Start is called before the first frame update
    void Start()
    {

        Nodes_In_Fire = new List<Node>();
        

    }

    public void Set_Target(Transform target)
    {
        Target = target;
    }
    private void Awake()
    {
        Unit_Manager = FindObjectOfType<Unit_Manager>();
        Grid = FindObjectOfType<Pathfinding_Grid>();
        rigidbody = GetComponent<Rigidbody>();
        //rigidbody.useGravity = false;
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
            Node.Threat += 20;
        }
        foreach(Unit unit in Unit_Manager.All_Units)
        {
            unit.Update_Path();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Warning_Distance)
        if (!Incoming_Resolved && Vector3.Distance(transform.position,Target.position) < Warning_Distance && Previous_Y > transform.position.y)
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
            //print("Raycast Hit");
            On_Incoming(Target.position);
            Incoming_Resolved = true;
        }
        else
        {
            Previous_Y = transform.position.y;
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        rigidbody.isKinematic = true;
    }


    private void OnDrawGizmos()
    {
        if(Incoming_Resolved)
        {
            Gizmos.color = Color.red;
            foreach (Node Node in Nodes_In_Fire)
            {
                Gizmos.DrawCube(Node.Pos, Vector3.one * (0.5f - .1f));
            }
        }
        
    }
}
