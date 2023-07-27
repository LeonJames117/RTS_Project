using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public int Player_Num = 0;
    public Canvas HUD_ref;
    public GameObject Map_Floor;
    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 1; i < Player_Num + 1; i++)
        {
            bool Setup_Success = true;
            GameObject player = new GameObject();
            player.name = "Player " + i;
            Player_Controller controller = player.AddComponent<Player_Controller>();
            c
            controller.m_Team = i;

            GameObject player_cam = new GameObject();
            Camera Cam = player_cam.AddComponent<Camera>();
            player_cam.name = "Player_" + i + "_Camera";
            Canvas HUD = Instantiate(HUD_ref, transform);
            HUD.worldCamera = Cam;

            Pathfinding_Manager pathfinding_Manager = player.AddComponent<Pathfinding_Manager>();
            pathfinding_Manager.Team = i;

            Unit_Manager unit_Manager = player.AddComponent<Unit_Manager>();
            unit_Manager.Team = i;

            Setup_Success = unit_Manager.Set_Pathfinding_Manager(pathfinding_Manager) && pathfinding_Manager.Setup(unit_Manager);
            if (!Setup_Success)
            {
                print("Failed to setup player " + i);
                return;
            }
            Resource_Manager resource_Manager = player.AddComponent<Resource_Manager>();
            resource_Manager.Setup(i, HUD);
            

            Setup_Success = controller.Setup(unit_Manager, resource_Manager,Cam,Map_Floor);
            if (!Setup_Success)
            {
                print("Failed to setup player " + i);
                return;
            }
        }
    }
}
