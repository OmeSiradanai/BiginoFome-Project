using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{

    public MovementJoystick movementJoystick;
    public float Speed;
    public Animator animator;
    private Rigidbody2D rb;

    [SerializeField] private bool isRight = true;
    [SerializeField] private float dashSpeed = 1000.0f;
    [SerializeField] private bool isDashing = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SkillDash();
        }
    }


    void FixedUpdate()
    {
        if (movementJoystick.joystickVec.y != 0)
        {
            rb.velocity = new Vector2(movementJoystick.joystickVec.x * Speed, movementJoystick.joystickVec.y * Speed);
            animator.SetBool("isMoving", true);
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);
        }

        if (movementJoystick.joystickVec.x > 0.1f)
        {
            isRight = true;
            transform.Translate(Vector2.right * Speed * Time.deltaTime);
            transform.eulerAngles = new Vector2(0, 0);
        }
        else if (movementJoystick.joystickVec.x < -0.1f)
        {
            isRight = false;
            transform.Translate(Vector2.right * Speed * Time.deltaTime);
            transform.eulerAngles = new Vector2(0, 180);
        }
    }

    public void SkillDash()
    {
        isDashing = true;
        animator.SetBool("isDash", true);
        if (isRight)
        {
            //Right
            rb.AddForce(Vector2.right * dashSpeed);
        }
        else
        {
            //Left
            rb.AddForce(Vector2.left * dashSpeed);
        }
        StartCoroutine(StopDash());
    }

    IEnumerator StopDash()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isDash", false);
        isDashing = false;
    }
}