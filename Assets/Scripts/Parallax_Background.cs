using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax_Background : MonoBehaviour
{
    private GameObject cam; // Reference to the main camera for tracking movement

    [SerializeField] private float parallax_Effect; // Parallax effect intensity (how much the background moves relative to the camera)

    private float length;     // Width of the background image
    private float x_Position; // Initial x position of the background

    void Start()
    {
        // Find the main camera by name and assign it
        cam = GameObject.Find("Main Camera");

        // Get the width of the background image from the SpriteRenderer's bounds
        length = GetComponent<SpriteRenderer>().bounds.size.x;

        // Set initial x position
        x_Position = transform.position.x;
    }

    void Update() 
    {
        // Calculate how far the camera has moved relative to the background's initial position
        float distanceMoved = cam.transform.position.x * (1 - parallax_Effect);

        // Calculate how much the background should move based on parallax effect
        float distanceToMove = cam.transform.position.x * parallax_Effect;    

        // Update the background's x position for the parallax effect
        transform.position = new Vector3(x_Position + distanceToMove, transform.position.y);    

        // If the camera has moved beyond the current background, reposition the background to create an infinite scrolling effect
        if(distanceMoved > x_Position + length)
        {
            x_Position += length; // Move the x position by the length of the background image
        }
    }
}