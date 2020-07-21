using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
 public   Rigidbody2D rigidbody2D;

    Vector2 velocity;

    public Grid WorldGrid;
    SpriteRenderer[] spriteRenderers;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 diff = worldPos - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        Vector3 scale = Vector3.one;
      //  if (diff.y < 0)
         //   rot_z = 360 - rot_z;
        float rotationZ = 0;

        if (rot_z > 0 && rot_z < 90)
            scale = new Vector3(-1, 1, 1);
        else if (rot_z > 90 && rot_z < 180)
          scale = Vector3.one;



        // transform.rotation = Quaternion.Euler(0f, 0f, rotationZ-90);

        transform.localScale = scale;

        Vector2 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        rigidbody2D.velocity = input * Speed * Time.deltaTime ;

        Vector3 pos = transform.position;
        pos.z = WorldGrid.WorldToCell(pos).y * 0.16f;
        transform.position = pos;
        foreach (var t in spriteRenderers)
        {
           // t.transform.localPosition += Vector3.forward * pos.z;
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name+"   "+collision.GetContact(0).point);


        InteractableTilemap it = collision.transform.GetComponent<InteractableTilemap>();

        if (it != null)
            it.Interact(transform.position);
    }
}
