using Structures;
using Units;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnitBase = Units.UnitBase;
using MobileUnit = Units.MobileUnit;

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
        [FormerlySerializedAs("Unwalkable")] public LayerMask unwalkableLayer;
        [FormerlySerializedAs("Ground")] public LayerMask ground;
        // Base Building
        [Header("Base Building")]
        bool _buildingMode = false;
        StructureBase _selectedBuilding;
        [FormerlySerializedAs("Grid")] public Pathfinding_Grid grid;
        Material _startMat;
        [FormerlySerializedAs("Can_Build")] public Material canBuild;
        [FormerlySerializedAs("Cannot_Build")] public Material cannotBuild;
        public LayerMask Block_Building;
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
                print("Mouse Down");
                Ray ray = cameraRef.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity,unwalkableLayer) && hit.collider.gameObject.GetComponent<UnitBase>()!=null)
                { 
                    print("Unit Selction started");
                    UnitBase selectedUnit = hit.collider.gameObject.GetComponent<UnitBase>();
                    Selection_Eligibility(selectedUnit);
                    if (Input.GetKey(KeyCode.LeftShift))
                    {// Shift Left Clicking
                            
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
                        Single_Select(hit.collider.gameObject.GetComponent<UnitBase>());
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
                    foreach (UnitBase unit in uManager.selectedUnits)
                    {
                        if (unit is MobileUnit)
                        {
                            MobileUnit MobUnit = (MobileUnit) unit;
                            if(Input.GetKey(KeyCode.LeftShift))
                            {// Shift right click
                                if (!MobUnit.followingPath)
                                {// If the unit is not currently following a path treat as normal click without clearing the order queue
                                    MobUnit.target = hit.point;
                                    MobUnit.Update_Path();
                                }
                                else
                                {// If unit is following a path add the new target to it's order queue
                                    MobUnit.orderQueue.Add(hit.point);
                                }  
                            }
                            else
                            {// Basic right click
                                unit.orderQueue.Clear();
                                unit.target = hit.point;
                                MobUnit.Update_Path();
                                print("Unit Ordered");
                                if (_buildingMode)
                                {
                                    _buildingMode = false;
                                    print("Exited Building");
                                }
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
                Collider[] colliders = Physics.OverlapBox(_selectedBuilding.transform.position, _selectedBuilding.transform.localScale/2, Quaternion.identity, Block_Building);
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
                        
                        print("Build not blocked by " + blocker.name);
                        _selectedBuilding.GetComponent<Renderer>().material = canBuild;
                        
                        
                    }
                    
                }
                else
                {
                    _selectedBuilding.GetComponent<Renderer>().material = canBuild;
                    _buildBlocked = false;
                }

                if (Input.GetMouseButtonDown(0) && !_buildBlocked)
                {
                    _selectedBuilding.GetComponent<Renderer>().material = _startMat;
                    //_selectedBuilding.gameObject.layer = unwalkableLayer;
                    _buildingMode = false;
                    print("Exited Building");
                }
                else if (Input.GetMouseButtonDown(0) && _buildBlocked)
                {
                    print("Cannot Build, Number of coliders: " + colliders.Length);
                }
            }
        }

        //Selection
        void Single_Select(UnitBase unitToAdd)
        {// Selcting one unit without wanting to have any others selected
            Clear_Selection();
            print("Single Select");
            uManager.selectedUnits.Add(unitToAdd); 
            unitToAdd.selectionGraphic.SetActive(true);
        }

        void Shift_Select(UnitBase unitToAdd)
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
            foreach(UnitBase unit in uManager.selectedUnits)
            {
                unit.selectionGraphic.SetActive(false);
            }
            uManager.selectedUnits.Clear();
        }

        void Deselect_Sngle(UnitBase unitToRemove)
        {// Remove a specifed unit from current selection
            unitToRemove.selectionGraphic.SetActive(false);
            uManager.selectedUnits.Remove(unitToRemove);  
        }

        bool Selection_Eligibility(UnitBase unit)
        {
            if (unit.CompareTag("Selectable")) return true;
            print("Could not select " + unit.name + " Tag: "+unit.tag);
            return false;
        }
        public void Enter_Build_Mode(StructureBase structureToBuild)
        {
            StructureBase newBuilding = Instantiate(structureToBuild);
            _selectedBuilding = newBuilding;
            _startMat = newBuilding.GetComponent<MeshRenderer>().material;
            newBuilding.GetComponent<Renderer>().material = canBuild;
            _buildingMode = true;
            print("Entered Building");
        }
    }
}
