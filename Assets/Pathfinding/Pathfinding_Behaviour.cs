using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding_Behaviour : MonoBehaviour
{
    Pathfinding_Grid Path_Grid;
    public Transform Seeker,Target;
    private void Awake()
    {
        Path_Grid = GetComponent<Pathfinding_Grid>();
    }

    private void Update()
    {
        Find_New_Path(Seeker.position, Target.position);
    }
    void Retrace_Found_Path(Node Start, Node End)
    { 
        List<Node> Path = new List<Node>();
        Node Current_Node = End;
        if (Current_Node == Start)
        {
            print("Retrace set to start");
        }
        while (Current_Node != Start)
        {
            Path.Add(Current_Node);
            Current_Node = Current_Node.Parent;
            print("Loop");
        }
        Path.Reverse();
        print(Path.Count);
        Path_Grid.Path = Path;
    }

    void Find_New_Path(Vector3 Start, Vector3 Target)
    {
        if(Start == Target)
        {
            print("Start = Target");
        }
        Node StartN = Path_Grid.Find_Node_By_Pos(Start);
        Node TargetN = Path_Grid.Find_Node_By_Pos(Target);

        //print("StartN X= " + StartN.Grid_X + " Y = " + StartN.Grid_Y);
        //print("TargetN X= " + TargetN.Grid_X + " Y = " + TargetN.Grid_Y);

        List<Node> Open_Nodes = new List<Node>();
        HashSet<Node> Closed_Nodes = new HashSet<Node>();

        Open_Nodes.Add(StartN);

        while(Open_Nodes.Count > 0)
        {
            Node CurrentN = Open_Nodes[0];
            for (int i = 1; i < Open_Nodes.Count; i++)
            {
                if (Open_Nodes[i].F_Cost < CurrentN.F_Cost || Open_Nodes[i].F_Cost == CurrentN.F_Cost && Open_Nodes[i].H_Cost < CurrentN.H_Cost)
                {
                    CurrentN = Open_Nodes[i];
                    print("Node x:" + CurrentN.Grid_X + " Y: " + CurrentN.Grid_Y + " Added to Path");
                }
            }
            Open_Nodes.Remove(CurrentN);
            Closed_Nodes.Add(CurrentN);

            if (CurrentN == TargetN)
            {
                Retrace_Found_Path(StartN, TargetN);
                Open_Nodes.Clear();
                Closed_Nodes.Clear();
                print("Path Found");
                return;
            }
            foreach(Node Neighbour in Path_Grid.Find_Node_Neighbours(CurrentN))
            {
                if(!Neighbour.Walkable && Closed_Nodes.Contains(Neighbour))
                {
                    print("Node not walkable");
                    continue;
                }

                int New_Move_Cost = CurrentN.G_Cost + Path_Grid.Distance_Between_Nodes(CurrentN, Neighbour);
                if(New_Move_Cost < Neighbour.G_Cost || !Open_Nodes.Contains(Neighbour))
                {
                    Neighbour.G_Cost = New_Move_Cost;
                    Neighbour.H_Cost = Path_Grid.Distance_Between_Nodes(Neighbour,TargetN);
                    Neighbour.Parent = CurrentN;

                    if(!Open_Nodes.Contains(Neighbour))
                    {
                        print("Node added to open list");
                        Open_Nodes.Add(Neighbour);

                    }
                }

            }
        }    

    }
   
}
