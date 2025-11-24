using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    // public Vector3 spawnPoint = new Vector3(0, 5, 0);

    // `(a, b, c)` where:
    //  * `a` represents left (-1) to right (+1) movement
    //  * `b` represents negative or positive w-axis movement
    //  * `c` represents backward (-1) or forward (+1) movement
    private Vector3 moveInput;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool onGround;

    private Rigidbody rb;

    [Header("Camera Look")]
    public float mouseSensitivity = 50f;
    public Transform cameraTransform;

    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        RotateCamera();

        onGround = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        rb.linearDamping = onGround ? groundDrag : 0f;
    }

    private void RotateCamera()
    {
        Vector2 mouseMove = lookInput * mouseSensitivity * Time.deltaTime;

        yaw += mouseMove.x;
        pitch -= mouseMove.y;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector3>();
        // Debug.Log(moveInput);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && onGround)
        {
            Jump();
        }
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
        // Debug.Log(lookInput);
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.z)).normalized;
        
        float wMove = moveInput.y;
        GameManager.Instance.slicingPlane.offset += wMove * moveSpeed / 100f;

        float multiplier = onGround ? 1f : airMultiplier;

        rb.AddForce(moveDirection * moveSpeed * 10f * multiplier, ForceMode.Force);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }
}