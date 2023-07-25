using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit_Manager : MonoBehaviour
{
    public int Team;
    Queue<Path_Request> Request_Queue = new Queue<Path_Request>();
    Path_Request Current_Request;
    static Unit_Manager instance;
    Pathfinding_Manager Pathfinding;
    public List<Unit> All_Units = new List<Unit>();
    public List<Unit> Selected_Units = new List<Unit>();
    List<Vector3> Order_Queue = new List<Vector3>();
    bool Currently_Processing_Path;
    private void Awake()
    {
        instance = this;

    }
    public Unit_Manager(int T)
    {
        Team = T;
    }
    public bool Set_Pathfinding_Manager(Pathfinding_Manager PM)
    {
        bool Success = true;
        if(PM.Team == Team)
        {
            Pathfinding = PM;
        }
        else
        {
            print("Failed to assign path manager "+ PM.Team + " to unit manager team " + Team);
            Success = false;
        }
        return Success;
    }
        
    
    public static void RequestPath(Vector3 Start, Vector3 Target, Action<Vector3[], bool> Callback)
    {
        Path_Request New_Request = new Path_Request(Start,Target,Callback);
        instance.Request_Queue.Enqueue(New_Request);
        instance.Process_Next();
    }

    void Process_Next()
    {
        if(!Currently_Processing_Path && Request_Queue.Count > 0)
        {
            Current_Request = Request_Queue.Dequeue();// Also removes item from queue
            Currently_Processing_Path = true;
            Pathfinding.Start_Pathfinding(Current_Request.Start, Current_Request.Target);
        }
    }

    public void Finished_Path_Processing(Vector3[] Path, bool Success)
    {
        Current_Request.Callback(Path,Success);
        Currently_Processing_Path=false;
        Process_Next();
    }

    

    struct Path_Request
    {
        public Vector3 Start;
        public Vector3 Target;
        public Action<Vector3[],bool> Callback;
        public Path_Request(Vector3 start,Vector3 end, Action<Vector3[],bool> callback)
        {
            Start = start;
            Target = end;
            Callback = callback;
        }
    }
}
