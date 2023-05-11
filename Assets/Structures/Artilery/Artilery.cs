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
    

    public float Max_Projectile_Height;
    public float Gravity = -18;
    float Cannon_Starting_Y;
    float Cannon_Starting_Z;
    float Turret_Starting_X;
    float Turret_Starting_Z;
    float Next_Fire_Time = 0f;
    public float Reload_Time;
    public bool Ready_to_Fire = false;
    Unit U;
    // Start is called before the first frame update
    private void Awake()
    {
        Cannon_Starting_Y = Cannon.transform.localRotation.y;
        Cannon_Starting_Z = Cannon.transform.localRotation.z;
        U = GetComponent<Unit>();
    }


    private void Update()
    {


        Vector3 Target_Direction = Turret.transform.position - U.Target;
        float Speed = 1f;
        Vector3 New_Facing = Vector3.RotateTowards(Turret.transform.forward, Target_Direction, Speed * Time.deltaTime, 0.0f);
        Debug.DrawRay(Turret.transform.position, New_Facing, Color.red);
        //New_Facing = new Vector3(Mathf.Abs(New_Facing.x), Mathf.Abs(New_Facing.y), Mathf.Abs(New_Facing.z));
        Turret.transform.rotation = Quaternion.LookRotation(New_Facing);

        if(Vector3.Angle(Turret.transform.forward,Target_Direction) < 2 )
        {
            Ready_to_Fire = true;
        }
        else
        {
            Ready_to_Fire = false;
            print("Forward: "+ Turret.transform.forward + " Target:" + Target_Direction);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //print("Fire");
            //Fire();
            //Aim_to_Target();
        }
        if(Time.time >= Next_Fire_Time && U.Target != Vector3.zero)
        {
            //Aim_to_Target();
            if (Ready_to_Fire)
            {
                Fire();
                Next_Fire_Time = Time.time + Reload_Time;
            }
            
            
        }


        
        //Turret.transform.forward = -Target_Direction;
        //Vector3.Lerp(Turret.transform.forward, -Target_Direction, 20 * Time.deltaTime);
    }
    public void Aim_to_Target()
    {
        
        //Turret.transform.forward = -Target_Direction;
    }


    Vector3 Calculate_Launch_Parameters(Rigidbody Shell)
    {
        float Y_Displacement = U.Target.y - Shell.position.y;
        Vector3 XZ_Displacement = new Vector3(U.Target.x - Shell.position.x,0,  U.Target.z - Shell.position.z);

        
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
        Shell.Set_Target(U.Target);
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
