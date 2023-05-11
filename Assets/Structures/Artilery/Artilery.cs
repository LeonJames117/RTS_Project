using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Artilery : Structure_Base
{

    public GameObject Cannon;
    public GameObject Turret;
    public GameObject Projectile;
    public Transform Fire_Point;
    public Transform Target;

    public float Max_Projectile_Height = 25;
    public float Gravity = -18;
    float Cannon_Starting_Y;
    float Cannon_Starting_Z;
    float Turret_Starting_X;
    float Turret_Starting_Z;
    // Start is called before the first frame update
    private void Awake()
    {
        Cannon_Starting_Y = Cannon.transform.localRotation.y;
        Cannon_Starting_Z = Cannon.transform.localRotation.z;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            print("Key Pressed: W");
            float New_Roation = (float)(Cannon.transform.localRotation.x + 0.2);
            
            Quaternion TR = new Quaternion(New_Roation, Cannon_Starting_Y, Cannon_Starting_Z, 1);
            print("New Rotation target = " + TR);
            Cannon.transform.localRotation = Quaternion.Slerp(Cannon.transform.localRotation,TR,10 * Time.deltaTime);
            print("Actual new Rotation = " + Cannon.transform.rotation);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            print("Key Pressed: S");
            float New_Roation = (float)(Cannon.transform.localRotation.x - 0.2);
            Quaternion TR = new Quaternion(New_Roation, Cannon_Starting_Y, Cannon_Starting_Z, 1);
            print("New Rotation target = " + TR);
            Cannon.transform.localRotation = Quaternion.Slerp(Cannon.transform.localRotation, TR, 10 * Time.deltaTime);
            print("Actual new Rotation = " + Cannon.transform.rotation);

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            print("Key Pressed: A");
            float New_Roation = (float)(Turret.transform.localRotation.y + 0.2);

            Quaternion TR = new Quaternion(Turret_Starting_X, New_Roation, Turret_Starting_Z, 1);
            print("New Rotation target = " + TR);
            Turret.transform.localRotation = Quaternion.Slerp(Turret.transform.localRotation, TR, 10 * Time.deltaTime);
            print("Actual new Rotation = " + Cannon.transform.rotation);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            print("Key Pressed: D");
            float New_Roation = (float)(Turret.transform.localRotation.y - 0.2);
            Quaternion TR = new Quaternion(Turret_Starting_X, New_Roation, Turret_Starting_Z, 1);
            print("New Rotation target = " + TR);
            Turret.transform.localRotation = Quaternion.Slerp(Turret.transform.localRotation, TR, 10 * Time.deltaTime);
            print("Actual new Rotation = " + Cannon.transform.rotation);

        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Fire");
            Fire();
        }
       
    }
    public void Aim_to_Target(Transform Target)
    {

    }


    Vector3 Calculate_Launch_Parameters(Rigidbody Shell)
    {
        float Y_Displacement = Target.position.y - Shell.position.y;
        Vector3 XZ_Displacement = new Vector3(Target.position.x - Shell.position.x,0,  Target.position.z - Shell.position.z);

        
        float Flight_Time = Mathf.Sqrt(-2 * Max_Projectile_Height / Gravity) + Mathf.Sqrt(2 * (Y_Displacement - Max_Projectile_Height) / Gravity);
        Vector3 Y_Velocity = Vector3.up * Mathf.Sqrt(-2 * Gravity * Max_Projectile_Height); //Kinematic Equation
        Vector3 XZ_Velocity = XZ_Displacement / Flight_Time;

        return XZ_Velocity + Y_Velocity;
    }

    public void Fire()
    {

        
        var temp = Instantiate(Projectile, Fire_Point.position, Fire_Point.rotation);
        //temp.transform.forward = Fire_Point.forward;
        Rigidbody RigidBod = temp.GetComponent<Rigidbody>();
        Projectile Shell = temp.GetComponent<Projectile>();
        temp.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Physics.gravity = Vector3.up * Gravity;
        Shell.Set_Target(Target);
        RigidBod.velocity = Calculate_Launch_Parameters(RigidBod);
        print("Shell_Vel = "+ Calculate_Launch_Parameters(RigidBod));



        //var temp = Instantiate(Projectile, Fire_Point.position,Fire_Point.rotation);
        //temp.transform.forward = Fire_Point.forward;
        //Rigidbody RigidBod = temp.GetComponentInChildren<Rigidbody>();
        //temp.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //Vector2 vel = RigidBod.velocity;

        //Vector3 Force_to_Add = (Target.position - RigidBod.transform.position);
        //Vector3 Launch_Velocity = new Vector3(0, Force/5, Force);
        //print("Force = " + Force_to_Add);
        //Vector3 Force_Vector = Fire_Point.forward;
        //Fire_Point.forward.Scale(Launch_Velocity);
        //RigidBod.AddRelativeForce(Launch_Velocity, ForceMode.Impulse);


        //PTemp.rigidbody.velocity = Fire_Point.forward * 700f;


    }
}
