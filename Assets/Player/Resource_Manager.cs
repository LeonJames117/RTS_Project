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
    public Resource_Manager(int T)
    {
        Team = T;

    }

    // Update is called once per frame
    void Update()
    {
        Metal_Counter.text = Stored_Metal.ToString();
        Power_Counter.text = Stored_Power.ToString();
    }
}
