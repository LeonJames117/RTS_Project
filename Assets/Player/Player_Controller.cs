using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
// Game Management
    [Header("Game Management")]
    public int m_Team;
    public Resource_Manager m_resource_manager;
    public Unit_Manager m_unit_manager;
    // Camera
    [Header("Camera")]
    public Camera Camera_Ref;
    public GameObject Floor;
    [Range(0f, 1f)]
    public float Zoom_Sensitivity = 0.5f;
    [Range(0f, 1f)]
    public float Pan_Sensitivity = 0.2f;
    public float Max_Zoom_Height = 30;
    public float Min_Zoom_Height = 10;
    
    // Unit Selection
    [Header("Unit Management")]
    
    Vector3 Mouse_Start_Pos;
    public LayerMask Selectable;
    public LayerMask Ground;
    // Base Building
    [Header("Base Building")]
    public Pathfinding_Grid Grid;
    bool Building_Mode = false;
    Structure_Base Selected_Building;
    Material Start_Mat;
    public Material Can_Build_Material;
    public Material Cannot_Build_Material;
    public LayerMask Build_Blocking_Layer;
    bool Build_Blocked = false;
    
    public Player_Controller(int Team)
    {
        Team = m_Team;
    }
    public bool Setup(Unit_Manager UM,Resource_Manager RM)
    {
        bool success = true;
        if(UM.Team == m_Team)
        {
           m_unit_manager = UM;
        }
        else
        {
            success = false;
        }
        if(RM.Team == m_Team)
        {
            m_resource_manager = RM;
        }
        else
        {
            success = false;
        }
        return success;
    }
    
    void Update()
    {
        // Camera Controls
        if (Input.GetKey(KeyCode.W))
        {// Pan up
            print("Key Pressed: W");
            float New_Z = Camera_Ref.transform.position.z + Pan_Sensitivity;
            Camera_Ref.transform.position = new Vector3(Camera_Ref.transform.position.x, Camera_Ref.transform.position.y, New_Z);
        }
        if (Input.GetKey(KeyCode.S))
        {// Pan Down
            print("Key Pressed: S");
            float New_Z = Camera_Ref.transform.position.z - Pan_Sensitivity;
            Camera_Ref.transform.position = new Vector3(Camera_Ref.transform.position.x, Camera_Ref.transform.position.y, New_Z);
        }
        if (Input.GetKey(KeyCode.D))
        {// Pan Right
            print("Key Pressed: A");
            float New_X = Camera_Ref.transform.position.x + Pan_Sensitivity;
            Camera_Ref.transform.position = new Vector3(New_X, Camera_Ref.transform.position.y, Camera_Ref.transform.position.z);
        }
        if (Input.GetKey(KeyCode.A))
        {// Pan Left
            print("Key Pressed: D");
            float New_X = Camera_Ref.transform.position.x - Pan_Sensitivity;
            Camera_Ref.transform.position = new Vector3(New_X, Camera_Ref.transform.position.y, Camera_Ref.transform.position.z);

        }
        if (Input.mouseScrollDelta.y > 0)
        {// Zoom in
            float New_Y = Camera_Ref.transform.position.y - Zoom_Sensitivity;
            
            if (New_Y < Floor.transform.position.y + Min_Zoom_Height)
            {
                New_Y = Floor.transform.position.y + Min_Zoom_Height;
            }
            Camera_Ref.transform.position = new Vector3(Camera_Ref.transform.position.x, New_Y, Camera_Ref.transform.position.z);
        }
        if (Input.mouseScrollDelta.y < 0)
        {// Zoom out
            float New_Y = Camera_Ref.transform.position.y + Zoom_Sensitivity;
            if (New_Y > Floor.transform.position.y + Max_Zoom_Height)
            {
                New_Y = Floor.transform.position.y + Max_Zoom_Height;
            }
            Camera_Ref.transform.position = new Vector3(Camera_Ref.transform.position.x, New_Y, Camera_Ref.transform.position.z);
        }

        // Unit Selection + Ordering controls
        if (Input.GetMouseButtonDown(0))// Left Mouse pressed
        {
            Ray ray = Camera_Ref.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit Hit,Mathf.Infinity,Selectable))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {// Shift Left Clicking
                    Unit Selected_Unit = Hit.collider.gameObject.GetComponent<Unit>();
                    if (!m_unit_manager.Selected_Units.Contains(Selected_Unit))
                    {
                        
                        Shift_Select(Selected_Unit);
                    }
                    else
                    {
                        Deselect_Sngle(Selected_Unit);
                    }
                    
                }
                else
                {// Single Left Clicking
                    Single_Select(Hit.collider.gameObject.GetComponent<Unit>());
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
        if (Input.GetMouseButtonDown(1) && m_unit_manager.Selected_Units.Count !=0)
        {
            Ray ray = Camera_Ref.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit Hit, Mathf.Infinity, Ground) && m_unit_manager.Selected_Units.Count > 0)
            {// If player right clicks on the ground and has units selected
                foreach (Unit unit in m_unit_manager.Selected_Units)
                {
                    if (!unit.Is_Structre)
                    {
                        if(Input.GetKey(KeyCode.LeftShift))
                        {// Shift right click
                            if (!unit.Following_Path)
                            {// If the unit is not currently following a path treat as normal click without clearing the order queue
                                unit.Target = Hit.point;
                                unit.Update_Path();
                            }
                            else
                            {// If unit is following a path add the new target to it's order queue
                                unit.Order_Queue.Add(Hit.point);
                            }  
                        }
                        else
                        {// Basic right click
                            unit.Order_Queue.Clear();
                            unit.Target = Hit.point;
                            unit.Update_Path();
                        } 
                    }
                    else
                    {// If a structure set target still for gun structres and factory produced units
                        unit.Target = Hit.point;
                    } 
                }
            }
        }
        if (Building_Mode)
        {
            Ray ray = Camera_Ref.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit Hit, Mathf.Infinity, Ground))
            {
                
                Selected_Building.transform.position = Grid.Find_Node_By_Pos(Hit.point).Pos;
                //print("Ground hit: " + Hit.point);
            }
            else
            {
                //print("No Ground hit");
            }
            Collider[] colliders = Physics.OverlapBox(Selected_Building.transform.position, Selected_Building.transform.localScale/2, Quaternion.identity, Build_Blocking_Layer);
            if(m_resource_manager.Stored_Metal < Selected_Building.Metal_Cost || m_resource_manager.Stored_Power < Selected_Building.Power_Cost)
            {
                Selected_Building.GetComponent<Renderer>().material = Cannot_Build_Material;
            }
            else if(colliders.Length > 0)
            {
                
                print("Colliders " + colliders.Length);
                foreach (Collider Blocker in colliders)
                {
                    print("Collided with " + Blocker.name);
                    if (Blocker.gameObject != Selected_Building.gameObject)
                    {
                        Selected_Building.GetComponent<Renderer>().material = Cannot_Build_Material;
                        print("Build blocked by " + colliders[0].name);
                        Build_Blocked = true;
                        break;
                    }
                    else
                    {
                        print("Build not blocked by " + Blocker.name);
                        Selected_Building.GetComponent<Renderer>().material = Can_Build_Material;
                        Build_Blocked = false;
                    }
                }
                    
            }
            else
            {
                Selected_Building.GetComponent<Renderer>().material = Can_Build_Material;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("Start Rotation = " + Selected_Building.transform.rotation);
                Selected_Building.transform.Rotate(0, 45, 0);
                print("New Rotation = " + Selected_Building.transform.rotation);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                print("Start Rotation = " + Selected_Building.transform.rotation);
                Selected_Building.transform.Rotate(0, -45, 0);
                print("New Rotation = " + Selected_Building.transform.rotation);
            }

            if (Input.GetMouseButtonDown(0) && !Build_Blocked && m_resource_manager.Stored_Metal>= Selected_Building.Metal_Cost && m_resource_manager.Stored_Power >= Selected_Building.Power_Cost)
            {
                Selected_Building.GetComponent<Renderer>().material = Start_Mat;
                m_resource_manager.Stored_Metal -= Selected_Building.Metal_Cost;
                m_resource_manager.Stored_Power -= Selected_Building.Power_Cost;
                Building_Mode = false;
                print("Build Mode False");
            }
        }
    }

    //Selection
   void Single_Select(Unit Unit_to_Add)
    {// Selcting one unit without wanting to have any others selected
        Clear_Selection();
        print("Single Select");
        m_unit_manager.Selected_Units.Add(Unit_to_Add); 
        Unit_to_Add.Selection_Graphic.SetActive(true);
    }

    void Shift_Select(Unit Unit_to_Add)
    {// Selecting one unit with the intention to select more
        if (!m_unit_manager.Selected_Units.Contains(Unit_to_Add))
        {
            m_unit_manager.Selected_Units.Add(Unit_to_Add);
            Unit_to_Add.Selection_Graphic.SetActive(true);
        }
        else
        {
            m_unit_manager.Selected_Units.Remove(Unit_to_Add);
            Unit_to_Add.Selection_Graphic.SetActive(false);
        }
    }
    
    void Drag_Select(Unit Unit_to_Add)
    {// Select all units within an area (Not yet implimented)

    }

    void Clear_Selection()
    {// Clear the current selction
        foreach(Unit unit in m_unit_manager.Selected_Units)
        {
            unit.Selection_Graphic.SetActive(false);
        }
        m_unit_manager.Selected_Units.Clear();
    }

    void Deselect_Sngle(Unit Unit_to_Remove)
    {// Remove a specifed unit from current selection
        Unit_to_Remove.Selection_Graphic.SetActive(false);
        m_unit_manager.Selected_Units.Remove(Unit_to_Remove);  
    }
    
    public void Enter_Build_Mode(Structure_Base Structure_To_Build)
    {
        Structure_Base New_Building = Instantiate(Structure_To_Build);
        Selected_Building = New_Building;
        Start_Mat = New_Building.GetComponent<MeshRenderer>().material;
        New_Building.GetComponent<Renderer>().material = Can_Build_Material;
        Building_Mode = true;
    }
}
