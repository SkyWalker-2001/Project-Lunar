using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Parallax_Background : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallax_Effect;

    private float length;
    private float x_Position;

    void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;

        x_Position = transform.position.x;
    }

   void Update() 
    {
        float distanceMoved = cam.transform.position.x * (1 - parallax_Effect);
        float distanceToMove = cam.transform.position.x * parallax_Effect;    

        transform.position = new Vector3(x_Position + distanceToMove, transform.position.y);    

        if(distanceMoved > x_Position + length )
        {
            x_Position = x_Position + length;
        }
    }
}
