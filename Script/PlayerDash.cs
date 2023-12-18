using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Player Data")] // Public Data
    [SerializeField] private float speed = 6f;
    [SerializeField, Tooltip("The jump force is how many unity units high the player will jump.")] private float jumpForce = 12f;

    // References
    [Header("References")]
    private Rigidbody2D rb;
    private Transform groundCheck;
    private Transform wallCheck;
    private LayerMask groundLayer = 1 << 6;
    private LayerMask wallLayer = 1 << 7;

    // Private Data
    private float x;
    private bool isFacingRight;
    private float groundCheckSize = 0.2f;

    private bool canDash = true;
    [HideInInspector] public bool isDashing;
    [SerializeField] private float dashingPower = 75f;
    private float dashingTime = .2f;
    private float dashingCooldown = 1f;

    [HideInInspector] public bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private float originalCoyote = 0.15f;
    private float coyoteTiming;

    bool hasJumped;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>(); // Define "rb" Variable as Rigidbody2D
        groundCheck = transform.Find("GroundCheck"); // Define "groundCheck" Variable as GroundCheck Object
        wallCheck = transform.Find("WallCheck"); // Define "wallCheck" Variable as WallCheck object;
    }

    void FixedUpdate() {
        Vector2 move = new Vector2(x, 0f);

        if (!isDashing && !isWallJumping) {
            rb.AddForce(move * speed);
        }

        if (!isFacingRight && x < 0f && !isWallJumping) {
            Flip(); // Flip Player Character if Moving Right & Facing Left
        } else if (isFacingRight && x > 0f && !isWallJumping) {
            Flip(); // Flip Player Character if Moving Left & Facing Right
        }

        if (IsGrounded()) {
            coyoteTiming = originalCoyote;
            hasJumped = false;
        } else if (!IsGrounded() && coyoteTiming > 0) {
            coyoteTiming -= Time.deltaTime;
        }


        if (jumpForce == 1) {
            jumpForce = 2.3f;
        } else if (jumpForce == 2) {
            jumpForce = 4.1f;
        } else if (jumpForce == 3) {
            jumpForce = 5.88f;
        }

        WallSlide();
        WallJump();
    }

    public void Walk(InputAction.CallbackContext context) {
        x = context.ReadValue<Vector2>().x; // Define "x" Variable, Dependant on what the Player is Doing

        if (x > 0 && x < 1) {
            x = 1; // Round Up "x" Variable
        } else if (x < 0 && x > -1) {
            x = -1; // Round Down "x" Variable
        }
    }

    private void Flip() {
        isFacingRight = !isFacingRight; // Change "isFacingRight" Variable to the Opposite
        Vector3 rotation = transform.rotation.eulerAngles; // Store Player Character Rotation to Vector3
        
        if (rotation.y == 0) {
            rotation.y += 180; // Add 180 to the Y-Axis Data in "rotation" Variable
        } else if (rotation.y == 180) {
            rotation.y -= 180; // Remove 180 to the Y-Axis Data in "rotation" Variable
        }

        transform.rotation = Quaternion.Euler(rotation); // Rotate Player Character
    }

    public void Jump(InputAction.CallbackContext context) {
        if (IsGrounded() || coyoteTiming > 0) {
            if (!isDashing && !hasJumped) {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * 6.375f); // Make Player Character Jump, if IsGrounded() is true
                hasJumped = true;
            }
        }

        if (context.canceled && rb.velocity.y > 0f && !isDashing) {
            rb.velocity = new Vector2(rb.velocity.x, 0); // Make Player Character Jump Higher, if Player Holds Jump Button
        }
    }

    private IEnumerator Dashing() {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        if(isFacingRight) {
            rb.AddForce(new Vector2(-1, 0) * dashingPower, 0f);
        } else if (!isFacingRight) {
            rb.AddForce(new Vector2(1, 0) * dashingPower, 0f);
        }

        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
    }

    public void Dash(InputAction.CallbackContext context) {
        if (canDash) {
            StartCoroutine(Dashing());
        }
    }

    private void WallSlide() {
        if (IsWalled() && !IsGrounded() && x != 0) {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        } else {
            isWallSliding = false;
        }
    }

    private void WallJump() {
        if (isWallSliding) {
            isWallJumping = false;

            if (isFacingRight) {
                wallJumpingDirection = 1;
            } else if (!isFacingRight) {
                wallJumpingDirection = -1;
            }

            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        } else {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f) {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (wallJumpingDirection == 1 && isFacingRight) {
                Flip();
            } else if (wallJumpingDirection == -1 && !isFacingRight) {
                Flip();
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping() {
        isWallJumping = false;
    }

    public bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckSize, groundLayer); // Check if Player is Touching the Ground
    }

    public bool IsWalled() {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer); // Check if Player is Touching the Wall;
    }
}