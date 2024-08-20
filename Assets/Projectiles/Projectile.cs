using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {

        [FormerlySerializedAs("Grid")] public Pathfinding_Grid grid;
        [FormerlySerializedAs("Blast_Radius")] public int blastRadius = 5;
        bool _incomingResolved=false;
        [FormerlySerializedAs("Unit_Manager")] public UnitManager unitManager;
        Ray _ray;
        RaycastHit _hit;
        public new Rigidbody rigidbody;
        [FormerlySerializedAs("Starting_Velocity")] public Vector3 startingVelocity;
        float _previousY = -1;
        Vector3 _target;

        // Warning System + Path projection;
        [FormerlySerializedAs("Projection_Points")] [Range(10, 100)]
        public int projectionPoints =25;
        [FormerlySerializedAs("Time_Between_Points")] [Range(0.01f, 0.25f)]
        public float timeBetweenPoints = 0.1f;
        [FormerlySerializedAs("Projection_Line")] public LineRenderer projectionLine;
        [FormerlySerializedAs("Projectile_Colison_Mask")] public LayerMask projectileColisonMask;
        List<Node> _nodesInFire;
        [FormerlySerializedAs("Warning_Distance")] public int warningDistance = 20;
        Vector3 _currentImpactPoint;
        // Start is called before the first frame update
        void Start()
        {

            _nodesInFire = new List<Node>();
        

        }

        public void Set_Target(Vector3 target)
        {
            _target = target;
        }
    
        private void Awake()
        {
            unitManager = FindObjectOfType<UnitManager>();
            grid = FindObjectOfType<Pathfinding_Grid>();
            rigidbody = GetComponent<Rigidbody>();
            //rigidbody.useGravity = false;
        }

        void On_Incoming(Vector3 impactSite)
        {
            Node impactNode = grid.Find_Node_By_Pos(impactSite);
        
            int i = 0;
            for (int x = -blastRadius; x < blastRadius; x++)
            {// Find all Nodes within blast radius in both directions of the impact site
                for (int y = -blastRadius; y < blastRadius; y++)
                {
                    int NodeX = impactNode.Grid_X + x;
                    int NodeY = impactNode.Grid_Y + y;
                    if(grid.Find_Node_By_Grid(NodeX, NodeY) != null)
                    {
                        _nodesInFire.Add(grid.Find_Node_By_Grid(NodeX, NodeY));
                        print("Node X: " + _nodesInFire[i].Grid_X + " Impact Node Y: " + _nodesInFire[i].Grid_Y + " Added to Nodes in fire");
                    }
                    else
                    {
                        print("Null Node in fire");
                    }
                    
                }
                i++;
            }

            foreach(Node Node in _nodesInFire)
            {// Increase Node's threat Values
                Node.Threat += 20;
            }
            foreach(MobileUnit unit in unitManager.allUnits)
            {
                if (unit.followingPath)
                {// if the unit is not a structure and is currently following a path, make it update it's path
                    unit.Update_Path();
                }
            
            }
        }

        // Update is called once per frame
        void Update()
        {
        
            transform.LookAt(rigidbody.velocity);
            Draw_Projection();
            if (!_incomingResolved && Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _hit, warningDistance))
            {
                On_Incoming(_currentImpactPoint);
                _incomingResolved = true;
            }
            else
            {
                _previousY = transform.position.y;
            }

        
        }


        void Draw_Projection()
        {
            projectionLine.enabled = true;
            projectionLine.positionCount = Mathf.CeilToInt(projectionPoints / timeBetweenPoints) + 1;
            Vector3 Start_of_Line = transform.position;
            Vector3 Velocity = rigidbody.velocity;
            int i =0;
            projectionLine.SetPosition(i, Start_of_Line);
            for(float time = 0; time<  projectionPoints; time += timeBetweenPoints)
            {
                i++;
                Vector3 Line_Point = Start_of_Line + time * Velocity; // Moves along X and Z
                Line_Point.y = Start_of_Line.y + Velocity.y * time + (Physics.gravity.y / 2f * time * time); // Kinematic quation for displacement of an object over time using gravity as acceleration

                projectionLine.SetPosition(i,Line_Point);

                Vector3 Last_Line_Pos = projectionLine.GetPosition(i - 1);
                if (Physics.Raycast(Last_Line_Pos,(Line_Point - Last_Line_Pos).normalized, out RaycastHit Hit, (Line_Point - Last_Line_Pos).magnitude, projectileColisonMask))
                {
                    projectionLine.SetPosition(i, Hit.point);
                    projectionLine.positionCount = i + 1;
                    _currentImpactPoint = Hit.point;
                    return;
                }
            

            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.GetComponent<Wind>())
            {// Removes threat from any affected nodes after the projectile has landed and then destroys it
                if (_incomingResolved)
                {
                    foreach (Node N in _nodesInFire)
                    {
                        N.Threat -= 20;
                    }
                }
                Destroy(gameObject);
            }
        
        
        }


        private void OnDrawGizmos()
        {
            if(_incomingResolved)
            {
                Gizmos.color = Color.red;
                foreach (Node Node in _nodesInFire)
                {
                    Gizmos.DrawCube(Node.Pos, Vector3.one * (0.5f - .1f));
                }
            }
        
        }
    }
}
