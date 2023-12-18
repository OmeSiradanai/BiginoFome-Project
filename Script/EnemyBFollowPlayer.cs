using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBFollowPlayer : MonoBehaviour
{
    public float speed;
    public float lineOfSite;
    public float shootingRange;
    private Transform player;
    public bool directionLookEnabled = true;
    Rigidbody2D rb;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer > shootingRange)
        {
            animator.SetBool("isMoving", true);
            Movement();
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    void Movement()
    {
        
        transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
