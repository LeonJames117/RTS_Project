using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{

    public Pathfinding_Grid Grid;
    public int Blast_Radius = 5;
    bool Incoming_Resolved=false;
    public Unit_Manager Unit_Manager;
    Ray ray;
    RaycastHit hit;
    public Rigidbody rigidbody;
    public Vector3 Starting_Velocity;
    float Previous_Y = -1;
    Vector3 Target;

    // Warning System + Path projection;
    [Range(10, 100)]
    public int Projection_Points =25;
    [Range(0.01f, 0.25f)]
    public float Time_Between_Points = 0.1f;
    public LineRenderer Projection_Line;
    public LayerMask Projectile_Colison_Mask;
    List<Node> Nodes_In_Fire;
    public int Warning_Distance = 20;
    Vector3 Current_Impact_Point;
    // Start is called before the first frame update
    void Start()
    {

        Nodes_In_Fire = new List<Node>();
        

    }

    public void Set_Target(Vector3 target)
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
            if (!unit.Is_Structre && unit.Following_Path)
            {
                unit.Update_Path();
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3.Distance(transform.position,Target) < Warning_Distance
        // Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Warning_Distance)

        
        transform.LookAt(rigidbody.velocity);
        Draw_Projection();
        if (!Incoming_Resolved && Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Warning_Distance))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
            //print("Raycast Hit");
            On_Incoming(Current_Impact_Point);
            Incoming_Resolved = true;
        }
        else
        {
            Previous_Y = transform.position.y;
        }

        
    }


    void Draw_Projection()
    {
        Projection_Line.enabled = true;
        Projection_Line.positionCount = Mathf.CeilToInt(Projection_Points / Time_Between_Points) + 1;
        Vector3 Start_of_Line = transform.position;
        Vector3 Velocity = rigidbody.velocity;
        int i =0;
        Projection_Line.SetPosition(i, Start_of_Line);
        for(float time = 0; time<  Projection_Points; time += Time_Between_Points)
        {
            i++;
            Vector3 Line_Point = Start_of_Line + time * Velocity; // Moves along X and Z
            Line_Point.y = Start_of_Line.y + Velocity.y * time + (Physics.gravity.y / 2f * time * time); // Kinematic quation for displacement of an object over time using gravity as acceleration

            Projection_Line.SetPosition(i,Line_Point);

            Vector3 Last_Line_Pos = Projection_Line.GetPosition(i - 1);
            if (Physics.Raycast(Last_Line_Pos,(Line_Point - Last_Line_Pos).normalized, out RaycastHit Hit, (Line_Point - Last_Line_Pos).magnitude, Projectile_Colison_Mask))
            {
                Projection_Line.SetPosition(i, Hit.point);
                Projection_Line.positionCount = i + 1;
                Current_Impact_Point = Hit.point;
                return;
            }
            

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.GetComponent<Wind>())
        {
            if (Incoming_Resolved)
            {
                foreach (Node N in Nodes_In_Fire)
                {
                    N.Threat -= 20;
                }
            }
            Destroy(gameObject);
        }
        
        
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
