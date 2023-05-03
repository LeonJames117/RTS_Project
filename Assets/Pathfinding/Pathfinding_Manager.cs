using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
public class Pathfinding_Manager : MonoBehaviour
{
    public Pathfinding_Grid Path_Grid;
    Unit_Manager Unit_Manager;
    private void Awake()
    {
        //Path_Grid = GetComponent<Pathfinding_Grid>();
        Unit_Manager = GetComponent<Unit_Manager>();
    }

    public void Start_Pathfinding(Vector3 Start, Vector3 Target)
    {
        StartCoroutine(Find_New_Path(Start, Target));
    }

    IEnumerator Find_New_Path(Vector3 Start, Vector3 Target)
    {
        Stopwatch SW = new Stopwatch();
        SW.Start();
        Vector3[] Path_Waypoints;
        bool Path_Found = false;
        Node StartN = Path_Grid.Find_Node_By_Pos(Start);
        Node TargetN = Path_Grid.Find_Node_By_Pos(Target);

        if(StartN.Walkable && TargetN.Walkable)// Only attempts to find path if it is possbile to reach the end
        {
            Heap<Node> Open_Nodes = new Heap<Node>(Path_Grid.Node_Num);
            HashSet<Node> Closed_Nodes = new HashSet<Node>();

            Open_Nodes.Add(StartN);

            while (Open_Nodes.Count > 0)
            {
                Node CurrentN = Open_Nodes.Remove_First_Item();
                Closed_Nodes.Add(CurrentN);


                //print("there are " + Open_Nodes.Count + " Open Nodes");
                //print("There are " + Closed_Nodes.Count + " Closed Nodes");

                if (CurrentN == TargetN)
                {
                    Path_Found = true;
                    SW.Stop();
                    print("Path Found in " + SW.ElapsedMilliseconds + " Ms");
                    break;
                }

                List<Node> N_List = Path_Grid.Find_Node_Neighbours(CurrentN);

                foreach (Node Neighbour in N_List)
                {
                    //print("Scanning Neighbours...");
                    if (!Neighbour.Walkable || Closed_Nodes.Contains(Neighbour))
                    {
                        // print("Neighbour not walkable");
                        continue;
                    }

                    int New_Move_Cost = CurrentN.G_Cost + Path_Grid.Distance_Between_Nodes(CurrentN, Neighbour);
                    if (New_Move_Cost < Neighbour.G_Cost || !Open_Nodes.Contains(Neighbour))
                    {
                        //print("Possible Candidate found");
                        Neighbour.G_Cost = (New_Move_Cost);
                        Neighbour.H_Cost = (Path_Grid.Distance_Between_Nodes(Neighbour, TargetN));
                        Neighbour.Parent = CurrentN;

                        if (!Open_Nodes.Contains(Neighbour))
                        {
                            //print("Node added to open list");
                            Open_Nodes.Add(Neighbour);

                        }
                        else
                        {
                            Open_Nodes.UpdateItem(Neighbour);
                        }
                    }

                }
            }

        }
        yield return null;
        if (Path_Found)
        {
            Path_Waypoints = Retrace_Found_Path(StartN, TargetN);
            Unit_Manager.Finished_Path_Processing(Path_Waypoints, Path_Found);
        }
    }

    Vector3[] Retrace_Found_Path(Node Start, Node End)
    {
        //print("Retrace started"); 
        List<Node> Path = new List<Node>();
        Node Current_Node = End;
        if (Current_Node == Start)
        {
            print("Retrace set to start");
        }
        while (Current_Node != Start)
        {
            if (!Path.Contains(Current_Node))
            {
                Path.Add(Current_Node);
                //print("Node x:" + Current_Node.Grid_X + " Y: " + Current_Node.Grid_Y + "H: "+ Current_Node.H_Cost + " G: " + Current_Node.G_Cost + " F: " + Current_Node.F_Cost + " Added to Path");
                Current_Node = Current_Node.Parent;
            }


        }
        //Path.Add(Current_Node);
        Vector3[] Path_Waypoints = Find_Waypoints_in_Path(Path);
        Array.Reverse(Path_Waypoints);
        return Path_Waypoints;
        //print("There are " + Path.Count + " Nodes in the path");
       
    }

    Vector3[] Find_Waypoints_in_Path(List<Node> Path)
    {
        List<Vector3> Waypoints = new List<Vector3>();
        Vector2 Previous_Driections = Vector2.zero;
        for(int i = 1; i < Path.Count; i++)
        {
            Vector2 New_Direction = new Vector2(Path[i - 1].Grid_X - Path[i].Grid_X, Path[i - 1].Grid_Y - Path[i].Grid_Y);
            if(New_Direction != Previous_Driections)
            {
                Waypoints.Add(Path[i].Pos);
            }
            Previous_Driections = New_Direction;
        }
        return Waypoints.ToArray();
    }



}
