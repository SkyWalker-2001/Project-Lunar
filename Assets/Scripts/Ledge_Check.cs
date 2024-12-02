using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ledge_Check : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Player player;

    private bool can_Detected;

    private void Update()
    {
        if (can_Detected)
            player.ledge_Detection = Physics2D.OverlapCircle(transform.position,radius,whatIsGround);
    }

   private void OnTriggerEnter2D(Collider2D collision)
   {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            can_Detected = false;
        }
   }

    private void OnTriggerExit2D(Collider2D collision)
   {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            can_Detected = true;
        }
   }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
