using UnityEngine;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;

    private Rigidbody rb;
    private bool isGrounded;
    private int jumpCount = 0;  
    private bool isDashing = false;

    private Vector3 moveDirection;
    private float dashTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isDashing)
        {
            HandleMovement();
            HandleJump();
        }

        HandleDash();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move relative to camera direction
        Vector3 moveInput = new Vector3(horizontal, 0, vertical);
        moveDirection = Camera.main.transform.TransformDirection(moveInput);
        moveDirection.y = 0;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || jumpCount <= 1)  
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                isGrounded = false;
                jumpCount++;
            }
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        dashTime = dashDuration;
        Vector3 dashDirection = moveDirection.normalized * dashSpeed;

        while (dashTime > 0)
        {
            rb.linearVelocity = dashDirection;
            dashTime -= Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        rb.linearVelocity = Vector3.zero; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))  
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }
}
