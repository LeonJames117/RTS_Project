using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utility_Scripts;
namespace Units
{
    public class Unit : MonoBehaviour
    {
        // Start is called before the first frame update
        [FormerlySerializedAs("Target")] public Vector3 target;
        [FormerlySerializedAs("Speed")] public float speed = 10;
        [FormerlySerializedAs("Current_Path")] public Vector3[] currentPath;
        [FormerlySerializedAs("Current_Waypoint_Index")] public int currentWaypointIndex;
        private const float RotationSpeed = 3;
        [FormerlySerializedAs("U_Man")] public UnitManager uMan;
        [FormerlySerializedAs("Selection_Graphic")] public GameObject selectionGraphic;
        [FormerlySerializedAs("Following_Path")] public bool followingPath;
        [FormerlySerializedAs("Is_Structure")] public bool isStructure = false;
        [FormerlySerializedAs("Unit_Type")] [SerializeField] SharedTypes.UnitType unitType;
        [FormerlySerializedAs("Order_Queue")] public List<Vector3> orderQueue = new List<Vector3>();
      
       

        public Unit(string unitType)
        {
            
        }

        void Start()
        {
            uMan.allUnits.Add(this);
            if(GetComponent<Structure_Base>() != null)
            {
                isStructure = true;
            }
            selectionGraphic.SetActive(false);

            
        }

        private void Update()
        {
            
        }
        public void Update_Path()
        {
            UnitManager.RequestPath(transform.position,target, unitType, On_Path_Found);
            print("Unit Requested path");
        }
        public void On_Path_Found(Vector3[]newPath,bool pathFound)
        {
            if(pathFound)
            {
                if (newPath != currentPath)
                {
                    currentPath = newPath;
                    StopCoroutine(nameof(Follow_Path));
                    StartCoroutine(nameof(Follow_Path));
                }
            
            }
        }
        public void OnDrawGizmos()
        {
            if (currentPath == null || !followingPath) return;
            for (var i = currentWaypointIndex; i < currentPath.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(currentPath[i], Vector3.one);

                Gizmos.DrawLine(i == currentWaypointIndex ? transform.position : currentPath[i - 1], currentPath[i]);
            }
        }


        IEnumerator Follow_Path()
        {
        

            followingPath = true;
            var currentWaypoint = currentPath[0];
            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    currentWaypointIndex++;
                    if (currentWaypointIndex >= currentPath.Length)
                    {
                        print("Finished Path");
                        currentWaypointIndex = 0;
                        Stop_Following_Path();
                        yield break;
                    }
                    currentWaypoint = currentPath[currentWaypointIndex];
                }
            

                Vector3 targetDir = currentWaypoint - this.transform.position;
                float step = RotationSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
                transform.rotation = Quaternion.LookRotation(newDir);

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

                yield return null;
            }
        }

        void Stop_Following_Path()
        {

        
            StopCoroutine(nameof(Follow_Path));
            followingPath = false;
            if (orderQueue.Count <= 0) return;
            target = orderQueue[0];
            orderQueue.RemoveAt(0);
            Update_Path();
        }

    }
}



   

