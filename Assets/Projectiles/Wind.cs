using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{

    public Collider Area_of_effect;
    public float Wind_Speed;
    // Start is called before the first frame update

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.GetComponent<Projectile>() != null)
        {
            print("Projectile in wind");
            Vector3 Wind_Vector = new Vector3(Wind_Speed, 0, 0);
            collision.rigidbody.velocity += Wind_Vector;
        }
        
    }

}
