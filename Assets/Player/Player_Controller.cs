using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    // Camera
    [Header("Camera")]
    public Camera Camera_Ref;
    public GameObject Floor;
    [Range(0f, 1f)]
    public float Zoom_Sensitivity = 0.5f;
    [Range(0f, 1f)]
    public float Pan_Sensitivity = 0.2f;

    // Unit Selection
    [Header("Unit Management")]
    public Unit_Manager U_Manager;
    Vector3 Mouse_Start_Pos;
    public LayerMask Selectable;
    public LayerMask Ground;
    
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
            Camera_Ref.transform.position = new Vector3(Camera_Ref.transform.position.x, New_Y, Camera_Ref.transform.position.z);
            if (New_Y < Floor.transform.position.y + 5)
            {
                New_Y = Floor.transform.position.y + 5;
            }
        }
        if (Input.mouseScrollDelta.y < 0)
        {// Zoom out
            float New_Y = Camera_Ref.transform.position.y + Zoom_Sensitivity;
            if (New_Y < Floor.transform.position.y + 5)
            {
                New_Y = Floor.transform.position.y + 5;
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
                    if (!U_Manager.Selected_Units.Contains(Selected_Unit))
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
        if (Input.GetMouseButtonDown(1) && U_Manager.Selected_Units.Count !=0)
        {
            Ray ray = Camera_Ref.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit Hit, Mathf.Infinity, Ground) && U_Manager.Selected_Units.Count > 0)
            {// If player right clicks on the ground and has units selected
                foreach (Unit unit in U_Manager.Selected_Units)
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
    }

    //Selection
   void Single_Select(Unit Unit_to_Add)
    {// Selcting one unit without wanting to have any others selected
        Clear_Selection();
        print("Single Select");
        U_Manager.Selected_Units.Add(Unit_to_Add); 
        Unit_to_Add.Selection_Graphic.SetActive(true);
    }

    void Shift_Select(Unit Unit_to_Add)
    {// Selecting one unit with the intention to select more
        if (!U_Manager.Selected_Units.Contains(Unit_to_Add))
        {
            U_Manager.Selected_Units.Add(Unit_to_Add);
            Unit_to_Add.Selection_Graphic.SetActive(true);
        }
        else
        {
            U_Manager.Selected_Units.Remove(Unit_to_Add);
            Unit_to_Add.Selection_Graphic.SetActive(false);
        }
    }
    
    void Drag_Select(Unit Unit_to_Add)
    {// Select all units within an area (Not yet implimented)

    }

    void Clear_Selection()
    {// Clear the current selction
        foreach(Unit unit in U_Manager.Selected_Units)
        {
            unit.Selection_Graphic.SetActive(false);
        }
        U_Manager.Selected_Units.Clear();
    }

    void Deselect_Sngle(Unit Unit_to_Remove)
    {// Remove a specifed unit from current selection
        Unit_to_Remove.Selection_Graphic.SetActive(false);
        U_Manager.Selected_Units.Remove(Unit_to_Remove);  
    }
    
}
