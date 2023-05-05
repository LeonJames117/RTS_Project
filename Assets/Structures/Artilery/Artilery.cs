using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Artilery : Structure_Base
{

    public GameObject Cannon;
    
    // Start is called before the first frame update
    private void Awake()
    {
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            print("Key Pressed");
            float New_Roation = (float)(Cannon.transform.rotation.x + 0.2);
            Quaternion TR = new Quaternion(New_Roation, 0, 0, 0);
            Cannon.transform.localRotation = Quaternion.Lerp(Cannon.transform.localRotation, TR,0.1f);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            print("Key Pressed");
            float New_Roation = (float)(Cannon.transform.rotation.x - 0.2);
            Quaternion TR = new Quaternion(-New_Roation, 0, 0, 0);
            Cannon.transform.localRotation = Quaternion.Lerp(Cannon.transform.localRotation, TR, 0.1f);
            

        }
    }
}
