using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 Target;
    public float Speed = 10;
    public Vector3[] Current_Path;
    public int Current_Waypoint_Index;
    float Rotation_Speed = 3;
    public Unit_Manager U_Man;
    public GameObject Selection_Graphic;
    public bool Following_Path;
    public bool Is_Structre = false;


   


    public List<Vector3> Order_Queue = new List<Vector3>();
    void Start()
    {
        U_Man.All_Units.Add(this);
        if(GetComponent<Structure_Base>() != null)
        {
            Is_Structre = true;
        }
        Selection_Graphic.SetActive(false);

    }

    private void Update()
    {
        
    }
    public void Update_Path()
    {
        Unit_Manager.RequestPath(transform.position, Target, On_Path_Found);
        print("Pathfinding Started");
    }
    public void On_Path_Found(Vector3[]New_Path,bool Path_Found)
    {
        if(Path_Found)
        {
            if (New_Path != Current_Path)
            {
                Current_Path = New_Path;
                StopCoroutine("Follow_Path");
                StartCoroutine("Follow_Path");
            }
            
        }
    }
    public void OnDrawGizmos()
    {
        if (Current_Path != null && Following_Path)
        {
            for (int i = Current_Waypoint_Index; i < Current_Path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(Current_Path[i], Vector3.one);

                if (i == Current_Waypoint_Index)
                {
                    Gizmos.DrawLine(transform.position, Current_Path[i]);
                }
                else
                {
                    Gizmos.DrawLine(Current_Path[i - 1], Current_Path[i]);
                }
            }
        }
    }


    IEnumerator Follow_Path()
    {
        

        Following_Path = true;
        Vector3 Current_Waypoint = Current_Path[0];
        while (true)
        {
            if (transform.position == Current_Waypoint)
            {
                Current_Waypoint_Index++;
                if (Current_Waypoint_Index >= Current_Path.Length)
                {
                    print("Finshed Path");
                    Current_Waypoint_Index = 0;
                    Stop_Following_Path();
                    yield break;
                }
                Current_Waypoint = Current_Path[Current_Waypoint_Index];
            }
            

            Vector3 targetDir = Current_Waypoint - this.transform.position;
            float step = Rotation_Speed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);

            transform.position = Vector3.MoveTowards(transform.position, Current_Waypoint, Speed * Time.deltaTime);

            yield return null;
        }
    }

    void Stop_Following_Path()
    {

        
        StopCoroutine("Follow_Path");
        Following_Path = false;
        if(Order_Queue.Count > 0)
        {
            Target = Order_Queue[0];
            Order_Queue.RemoveAt(0);
            Update_Path();
        }
    }

}



   

