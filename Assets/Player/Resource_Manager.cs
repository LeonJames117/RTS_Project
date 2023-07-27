using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Resource_Manager : MonoBehaviour
{
    public int Team;
    public int Stored_Metal = 0;
    public int Stored_Power = 0;
    public TextMeshProUGUI Metal_Counter;
    public TextMeshProUGUI Power_Counter;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public bool Setup(int T, Canvas HUD)
    {
        Team = T;
        TextMeshProUGUI[] counters = HUD.GetComponentsInChildren<TextMeshProUGUI>(false);
        bool Metal_Count_Found = false;
        bool Power_Count_Found = false;
        foreach (TextMeshProUGUI counter in counters)
        {
            if(!Metal_Count_Found && counter.name == "Metal_Counter")
            {
               Metal_Counter = counter;
                Metal_Count_Found=true;
            }
            else if (!Power_Count_Found && counter.name == "Power_Counter")
            {
                Power_Counter = counter;
                Power_Count_Found=true;
            }
            if(Metal_Count_Found && Power_Count_Found)
            {
                break;
            }
        }
        if (!Power_Count_Found || !Metal_Count_Found)
        {
            print("Power/Metal count not successfully linked on player " + T);
            return false;
        }
        else
        {
            return true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Metal_Counter.text = Stored_Metal.ToString();
        Power_Counter.text = Stored_Power.ToString();
    }
}
