using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public int Player_Num = 0;
    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 1; i < Player_Num + 1; i++)
        {
            bool Setup_Success = true;
            Player_Controller controller = gameObject.AddComponent<Player_Controller>();
            controller.gameObject.name = "Player_Controller_" + i;

            Pathfinding_Manager pathfinding_Manager = gameObject.AddComponent<Pathfinding_Manager>();
            pathfinding_Manager.gameObject.name = "Pathfinding_Manager_" + i;

            Unit_Manager unit_Manager = gameObject.AddComponent<Unit_Manager>();
            unit_Manager.gameObject.name = "Unit_Manager_" + i;
            Setup_Success = unit_Manager.Set_Pathfinding_Manager(pathfinding_Manager) && pathfinding_Manager.Setup(unit_Manager);
            if (!Setup_Success)
            {
                print("Failed to setup player " + i);
                return;
            }
            Resource_Manager resource_Manager = gameObject.AddComponent<Resource_Manager>();
            resource_Manager.gameObject.name = "Resource_Manager_" + i;

            Setup_Success = controller.Setup(unit_Manager, resource_Manager);
            if (!Setup_Success)
            {
                print("Failed to setup player " + i);
                return;
            }
        }
    }
}
