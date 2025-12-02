using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;

    public Camera playerCamera;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed = 10f;

    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    // SCORE
    private float score = 0f;
    public Text scoreText;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move();
        Rotate();
        Jump();
        UpdateAnimator();
    }

    // === Ground Check ===
    void GroundCheck()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(origin, Vector3.down * groundCheckDistance, Color.red);
    }

    // === Movement ===
    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;

        Vector3 horizontalVelocity = moveDirection * moveSpeed;

        Vector3 finalVelocity = new Vector3(
            horizontalVelocity.x,
            rb.velocity.y,
            horizontalVelocity.z
        );

        rb.velocity = finalVelocity;
    }

    // === Rotate ===
    void Rotate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }

    // === Jump ===
    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            animator.SetTrigger("jumpTrigger");
        }
    }

    // === Animator ===
    void UpdateAnimator()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        bool isMoving = moveX != 0 || moveZ != 0;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isGrounded", isGrounded);
    }

    // === Trigger untuk Point ===
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Point"))
        {
            Destroy(other.gameObject);
            AddScore();
        }
    }

    void AddScore()
    {
        score++;

        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (score == 15)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
