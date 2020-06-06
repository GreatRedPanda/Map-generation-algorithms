using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
 public   Rigidbody2D rigidbody2D;

    Vector2 velocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 diff = worldPos - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);



        Vector2 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //Vector2 targetVel =  input;
        //Vector2 newVel= Vector2.Lerp(rigidbody2D.velocity, targetVel, Speed * Time.deltaTime);

        rigidbody2D.velocity = input * Speed * Time.deltaTime ;
    }
}
