using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Artilery : Structure_Base
{
    // Prefab setup
    public GameObject Cannon;
    public GameObject Turret;
    public GameObject Projectile;
    public Transform Fire_Point;
    Unit U;

    // Projectile Physics
    public float Max_Projectile_Height;
    public float Gravity = -18;

    // Reload Cooldown
    float Next_Fire_Time = 0f;
    public float Reload_Time;
    public bool Ready_to_Fire = false;

    // Cannon Rotation (not yet implimented)
    float Cannon_Starting_Y;
    float Cannon_Starting_Z;
    float Turret_Starting_X;
    float Turret_Starting_Z;

    // Start is called before the first frame update
    private void Awake()
    {
        Cannon_Starting_Y = Cannon.transform.localRotation.y;
        Cannon_Starting_Z = Cannon.transform.localRotation.z;
        U = GetComponent<Unit>();
    }


    private void Update()
    {
        //Rotate Turret to face target
        Vector3 Target_Direction = Turret.transform.position - U.Target;
        float Speed = 1f;
        Vector3 New_Facing = Vector3.RotateTowards(Turret.transform.forward, Target_Direction, Speed * Time.deltaTime, 0.0f);
        Debug.DrawRay(Turret.transform.position, New_Facing, Color.red);
        Turret.transform.rotation = Quaternion.LookRotation(New_Facing);

        if(Vector3.Angle(Turret.transform.forward,Target_Direction) < 2 )
        {// if turret is faceing target
            Ready_to_Fire = true;
        }
        else
        {
            Ready_to_Fire = false;
        }
        if(Time.time >= Next_Fire_Time && U.Target != Vector3.zero)
        {// If the currtent time is later than the next time the cannon is able to fire after being reloaded
            if (Ready_to_Fire)
            {// And it is facing the target
                Fire();
                Next_Fire_Time = Time.time + Reload_Time;
            }
        }
    }

    Vector3 Calculate_Launch_Parameters(Rigidbody Shell)
    {// Calculates the velocity neccicary to hit the target location whilst traveling in an arc peaking at Max_Projectile_Height
        float Y_Displacement = U.Target.y - Shell.position.y;
        Vector3 XZ_Displacement = new Vector3(U.Target.x - Shell.position.x,0,  U.Target.z - Shell.position.z);
        float Flight_Time = Mathf.Sqrt(-2 * Max_Projectile_Height / Gravity) + Mathf.Sqrt(2 * (Y_Displacement - Max_Projectile_Height) / Gravity);
        Vector3 Y_Velocity = Vector3.up * Mathf.Sqrt(-2 * Gravity * Max_Projectile_Height); //Kinematic Equation
        Vector3 XZ_Velocity = XZ_Displacement / Flight_Time;
        return XZ_Velocity + Y_Velocity;
    }
    public void Fire()
    {// Creates a proejctile at the end of the cannon, sets it's target and applys the needed velocity to reach that location
        var temp = Instantiate(Projectile, Fire_Point.position, Fire_Point.rotation);
        Rigidbody RigidBod = temp.GetComponent<Rigidbody>();
        RigidBod.transform.forward = Fire_Point.transform.forward;
        Projectile Shell = temp.GetComponent<Projectile>();
        temp.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Physics.gravity = Vector3.up * Gravity;
        Shell.Set_Target(U.Target);
        RigidBod.velocity = Calculate_Launch_Parameters(RigidBod);
        Shell.Starting_Velocity = Calculate_Launch_Parameters(RigidBod);
        print("Shell_Vel = "+ Calculate_Launch_Parameters(RigidBod));
    }
}
