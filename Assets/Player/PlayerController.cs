using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // Camera
        [FormerlySerializedAs("Camera_Ref")] [Header("Camera")]
        public Camera cameraRef;
        [FormerlySerializedAs("Floor")] public GameObject floor;
        [FormerlySerializedAs("Zoom_Sensitivity")] [Range(0f, 1f)]
        public float zoomSensitivity = 0.5f;
        [FormerlySerializedAs("Pan_Sensitivity")] [Range(0f, 1f)]
        public float panSensitivity = 0.2f;
        [FormerlySerializedAs("Max_Zoom_Height")] public float maxZoomHeight = 30;
        [FormerlySerializedAs("Min_Zoom_Height")] public float minZoomHeight = 10;
        // Unit Selection
        [FormerlySerializedAs("U_Manager")] [Header("Unit Management")]
        public UnitManager uManager;
        Vector3 _mouseStartPos;
        [FormerlySerializedAs("Selectable")] public LayerMask selectable;
        [FormerlySerializedAs("Ground")] public LayerMask ground;
        // Base Building
        [Header("Base Building")]
        bool _buildingMode = false;
        Structure_Base _selectedBuilding;
        [FormerlySerializedAs("Grid")] public Pathfinding_Grid grid;
        Material _startMat;
        [FormerlySerializedAs("Can_Build")] public Material canBuild;
        [FormerlySerializedAs("Cannot_Build")] public Material cannotBuild;
        [FormerlySerializedAs("Ignore_Build_Block")] public LayerMask ignoreBuildBlock;
        bool _buildBlocked = false;

        void Update()
        {
            // Camera Controls
            if (Input.GetKey(KeyCode.W))
            {// Pan up
                print("Key Pressed: W");
                float newZ = cameraRef.transform.position.z + panSensitivity;
                cameraRef.transform.position = new Vector3(cameraRef.transform.position.x, cameraRef.transform.position.y, newZ);
            }
            if (Input.GetKey(KeyCode.S))
            {// Pan Down
                print("Key Pressed: S");
                float newZ = cameraRef.transform.position.z - panSensitivity;
                cameraRef.transform.position = new Vector3(cameraRef.transform.position.x, cameraRef.transform.position.y, newZ);
            }
            if (Input.GetKey(KeyCode.D))
            {// Pan Right
                print("Key Pressed: A");
                float newX = cameraRef.transform.position.x + panSensitivity;
                cameraRef.transform.position = new Vector3(newX, cameraRef.transform.position.y, cameraRef.transform.position.z);
            }
            if (Input.GetKey(KeyCode.A))
            {// Pan Left
                print("Key Pressed: D");
                float newX = cameraRef.transform.position.x - panSensitivity;
                cameraRef.transform.position = new Vector3(newX, cameraRef.transform.position.y, cameraRef.transform.position.z);

            }
            if (Input.mouseScrollDelta.y > 0)
            {// Zoom in
                float newY = cameraRef.transform.position.y - zoomSensitivity;
            
                if (newY < floor.transform.position.y + minZoomHeight)
                {
                    newY = floor.transform.position.y + minZoomHeight;
                }
                cameraRef.transform.position = new Vector3(cameraRef.transform.position.x, newY, cameraRef.transform.position.z);
            }
            if (Input.mouseScrollDelta.y < 0)
            {// Zoom out
                float newY = cameraRef.transform.position.y + zoomSensitivity;
                if (newY > floor.transform.position.y + maxZoomHeight)
                {
                    newY = floor.transform.position.y + maxZoomHeight;
                }
                cameraRef.transform.position = new Vector3(cameraRef.transform.position.x, newY, cameraRef.transform.position.z);
            }

            // Unit Selection + Ordering controls
            if (Input.GetMouseButtonDown(0))// Left Mouse pressed
            {
                Ray ray = cameraRef.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity,selectable))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {// Shift Left Clicking
                        Unit selectedUnit = hit.collider.gameObject.GetComponent<Unit>();
                        if (!uManager.selectedUnits.Contains(selectedUnit))
                        {
                        
                            Shift_Select(selectedUnit);
                        }
                        else
                        {
                            Deselect_Sngle(selectedUnit);
                        }
                    
                    }
                    else
                    {// Single Left Clicking
                        Single_Select(hit.collider.gameObject.GetComponent<Unit>());
                    }
                }
                else
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {//Single Left Click on ground
                        Clear_Selection();
                    }
                
                }
            }
            if (Input.GetMouseButtonUp(0))// Left Mouse Released
            {
                //print("Box Dimentsions = " + Mouse_Start_Pos + Get_Mouse_World_Pos());
            
            }
            if (Input.GetMouseButtonDown(1) && uManager.selectedUnits.Count !=0)
            {
                Ray ray = cameraRef.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground) && uManager.selectedUnits.Count > 0)
                {// If player right clicks on the ground and has units selected
                    foreach (Unit unit in uManager.selectedUnits)
                    {
                        if (!unit.isStructure)
                        {
                            if(Input.GetKey(KeyCode.LeftShift))
                            {// Shift right click
                                if (!unit.followingPath)
                                {// If the unit is not currently following a path treat as normal click without clearing the order queue
                                    unit.target = hit.point;
                                    unit.Update_Path();
                                }
                                else
                                {// If unit is following a path add the new target to it's order queue
                                    unit.orderQueue.Add(hit.point);
                                }  
                            }
                            else
                            {// Basic right click
                                unit.orderQueue.Clear();
                                unit.target = hit.point;
                                unit.Update_Path();
                            } 
                        }
                        else
                        {// If a structure set target still for gun structres and factory produced units
                            unit.target = hit.point;
                        } 
                    }
                }
            }
            if (_buildingMode)
            {
                Ray ray = cameraRef.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
                {
                
                    _selectedBuilding.transform.position = grid.Find_Node_By_Pos(hit.point).Pos;
                    //print("Ground hit: " + Hit.point);
                }
                else
                {
                    //print("No Ground hit");
                }
                Collider[] colliders = Physics.OverlapBox(_selectedBuilding.transform.position, _selectedBuilding.transform.localScale/2, Quaternion.identity, ignoreBuildBlock);
                if(colliders.Length > 0)
                {
                
                    print("Colliders " + colliders.Length);
                    foreach (Collider blocker in colliders)
                    {
                        print("Collided with " + blocker.name);
                        if (blocker.name != _selectedBuilding.name)
                        {
                            _selectedBuilding.GetComponent<Renderer>().material = cannotBuild;
                            print("Build blocked by " + colliders[0].name);
                            _buildBlocked = true;
                            break;
                        }
                        else
                        {
                            print("Build not blocked by " + blocker.name);
                            _selectedBuilding.GetComponent<Renderer>().material = canBuild;
                            _buildBlocked = false;
                        }
                    }
                    
                }
                else
                {
                    _selectedBuilding.GetComponent<Renderer>().material = canBuild;
                }

                if (Input.GetMouseButtonDown(0) && !_buildBlocked)
                {
                    _selectedBuilding.GetComponent<Renderer>().material = _startMat;
                    _buildingMode = false;
                    print("Build Mode False");
                }
            }
        }

        //Selection
        void Single_Select(Unit unitToAdd)
        {// Selcting one unit without wanting to have any others selected
            Clear_Selection();
            print("Single Select");
            uManager.selectedUnits.Add(unitToAdd); 
            unitToAdd.selectionGraphic.SetActive(true);
        }

        void Shift_Select(Unit unitToAdd)
        {// Selecting one unit with the intention to select more
            if (!uManager.selectedUnits.Contains(unitToAdd))
            {
                uManager.selectedUnits.Add(unitToAdd);
                unitToAdd.selectionGraphic.SetActive(true);
            }
            else
            {
                uManager.selectedUnits.Remove(unitToAdd);
                unitToAdd.selectionGraphic.SetActive(false);
            }
        }
    
        void Drag_Select(Unit unitToAdd)
        {// Select all units within an area (Not yet implimented)

        }

        void Clear_Selection()
        {// Clear the current selction
            foreach(Unit unit in uManager.selectedUnits)
            {
                unit.selectionGraphic.SetActive(false);
            }
            uManager.selectedUnits.Clear();
        }

        void Deselect_Sngle(Unit unitToRemove)
        {// Remove a specifed unit from current selection
            unitToRemove.selectionGraphic.SetActive(false);
            uManager.selectedUnits.Remove(unitToRemove);  
        }
    
        public void Enter_Build_Mode(Structure_Base structureToBuild)
        {
            Structure_Base newBuilding = Instantiate(structureToBuild);
            _selectedBuilding = newBuilding;
            _startMat = newBuilding.GetComponent<MeshRenderer>().material;
            newBuilding.GetComponent<Renderer>().material = canBuild;
            _buildingMode = true;
        }
    }
}
