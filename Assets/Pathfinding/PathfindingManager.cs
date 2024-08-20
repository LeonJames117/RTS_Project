using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Units;
using UnityEngine;
using Utility_Scripts;

namespace Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        public Pathfinding_Grid Land_Path_Grid;
        public Pathfinding_Grid Air_Path_Grid;
        public Pathfinding_Grid Sea_Path_Grid;
        UnitManager Unit_Manager;
        private void Awake()
        {
            //Path_Grid = GetComponent<Pathfinding_Grid>();
            Unit_Manager = GetComponent<UnitManager>();
        }

        public void Start_Pathfinding(Vector3 Start, Vector3 Target, SharedTypes.UnitType unitType)
        {
            StartCoroutine(Find_New_Path(Start, Target,unitType));
        }

        private void Update()
        {
        
        }
        IEnumerator Find_New_Path(Vector3 Start, Vector3 Target, SharedTypes.UnitType unitType)
        {
            print("Pathfinding Started");
            Pathfinding_Grid Path_Grid;
            if (unitType == SharedTypes.UnitType.Land)
            {
                Path_Grid = Land_Path_Grid;
            }
            else if (unitType == SharedTypes.UnitType.Air)
            {
                Path_Grid = Air_Path_Grid;
            }
            else if (unitType == SharedTypes.UnitType.Sea)
            {
                Path_Grid = Sea_Path_Grid;
            }
            else
            {
                print("Error setting Path_Grid from unit type");
                yield break;
            }
            Stopwatch SW = new Stopwatch();
            SW.Start();
            Vector3[] Path_Waypoints;
            bool Path_Found = false;
            Node StartN = Path_Grid.Find_Node_By_Pos(Start);
            Node TargetN = Path_Grid.Find_Node_By_Pos(Target);
            if (StartN == null || TargetN == null)
            {
                print("Start or end Node is NULL");
                yield break;
            }

            if (!TargetN.Walkable) // Redirects the target to the closest walkable node, or cancels the pathfinding if a new path is taking too long to find
            {
                print("Node cannot be reached: Redirecting to walkable node");
                List<Node> nodeNeighbours = Path_Grid.Find_Node_Neighbours(TargetN); 
                Node furthestCheckedNode = nodeNeighbours[0];
                List<Node> checkedNodes = new List<Node>();
                Node candidateNode = null;
                float checkRadius = 0;
                int i = 0;
                const int allowance = 100;// Sets how many groups of nodes can be checked before the system gives up for performance’s sake(This should be a rare edge case)
                while (candidateNode==null || i<allowance)
                {
                    foreach (var node in nodeNeighbours)
                    {
                        if (checkedNodes.Contains(node)) continue;
                        print("Inspecting node: " + node.Pos);
                        float distance = Vector3.Distance(node.Pos,TargetN.Pos);
                        if (distance > checkRadius)// slowly expands how far away from the original target will be checked until a walkable candidate is found
                        {
                            if (node.Walkable)
                            {
                                candidateNode = node;
                            }
                            if (!checkedNodes.Contains(node))
                            {
                                furthestCheckedNode = node;
                                checkRadius = distance;
                            }
                        }
                        if (node.Walkable)//prioritises walkable over distance
                        {
                            candidateNode = node;
                        }
                    }
                    if (candidateNode != null) // A walkable node has been found
                    {
                        print("Candidate found: " + candidateNode.Pos + " walkable " + candidateNode.Walkable);
                        break;
                    }
                    print("Node " + furthestCheckedNode.Pos + "is the current closest node and walkable is: " + furthestCheckedNode.Walkable);
                    if (!furthestCheckedNode.Walkable) // No walkable node is found, prevents previously checked sets of nodes from being checked again, thus expanding the search radius
                    {
                        nodeNeighbours = Path_Grid.Find_Node_Neighbours(furthestCheckedNode); 
                        checkedNodes.Add(furthestCheckedNode);
                        print("Next closest node cannot be reached: attempting redirection # "+ i +" at " + SW.ElapsedMilliseconds + " milliseconds");
                    }
                    
                    print("Checked " + (i+1) + " sets of nodes, current closest node is: " + furthestCheckedNode.Pos + " and walkable is " + furthestCheckedNode.Walkable);
                    i++;
                    if(i==allowance) break;
                }
                if (i == allowance) // Stops pathfinding if redirection is taking too long
                {
                    print("Redirection not possible: canceling pathfinding");
                    Unit_Manager.Finished_Path_Processing(Array.Empty<Vector3>(), false);
                    yield break;
                }
                TargetN = candidateNode;
                print("Redirection complete in " + SW.ElapsedMilliseconds + " milliseconds");
            }

            Heap<Node> Open_Nodes = new Heap<Node>(Path_Grid.Node_Num);
                HashSet<Node> Closed_Nodes = new HashSet<Node>();

                Open_Nodes.Add(StartN);

                while (Open_Nodes.Count > 0)
                {
                    Node CurrentN = Open_Nodes.Remove_First_Item();
                    Closed_Nodes.Add(CurrentN);

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
                        if (!Neighbour.Walkable || Closed_Nodes.Contains(Neighbour))
                        {// Ignore non walkable or already closed nodes
                            continue;
                        }

                        int New_Move_Cost = CurrentN.G_Cost + Path_Grid.Distance_Between_Nodes(CurrentN, Neighbour) + Neighbour.Threat;
                        if (New_Move_Cost < Neighbour.G_Cost || !Open_Nodes.Contains(Neighbour))
                        {
                            //print("Possible Candidate found");
                            Neighbour.G_Cost = (New_Move_Cost);
                            Neighbour.H_Cost = (Path_Grid.Distance_Between_Nodes(Neighbour, TargetN));
                            Neighbour.Parent = CurrentN;

                            if (!Open_Nodes.Contains(Neighbour))
                            {
                                Open_Nodes.Add(Neighbour);

                            }
                            else
                            {
                                Open_Nodes.UpdateItem(Neighbour);
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
                
                    Current_Node = Current_Node.Parent;
                }


            }
            Vector3[] Path_Waypoints = Find_Waypoints_in_Path(Path);
            Array.Reverse(Path_Waypoints);
            return Path_Waypoints;
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

        /*public bool Check_Path(Vector3 Target)
    {
        if(!Path_Grid.Find_Node_By_Pos(Target).Walkable)
        {
            return false;
        }
        return true;
    }*/

    }
}
