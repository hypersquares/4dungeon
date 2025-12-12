using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float wSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float maxAirSpeed = 10f;

    [Header("Ground Pound")]
    public float groundPoundForce = 100f;
    public float groundPoundMovementReduction = 0.1f;
    private bool isGroundPounding = false;


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
    public float lookAcceleration = 0.15f;
    public float lookDeceleration = 0.2f;
    public float maxLookSpeed = 200f;
    public Transform cameraTransform;

    [Header("Camera Bob")]
    public float bobAmplitude = 0.05f;
    public float bobRollAmplitude = 0.05f;
    public float bobHorizontalAmplitude = -0.1f;
    public float bobFrequency = 10f;
    public float bobSmoothing = 10f;

    private Vector2 lookInput;
    private Vector2 currentLookVelocity;
    private float yaw;
    private float pitch;
    private Vector3 originalCameraLocalPosition;
    private float bobTimer;
    private float currentBobOffset;
    private float currentBobRoll;
    private float currentBobHorizontal;
    private Vector3 initialSpawnPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        originalCameraLocalPosition = cameraTransform.localPosition;
        initialSpawnPosition = transform.position;
    }

    private void Update()
    {
        RaycastHit hitGround;
        onGround = Physics.SphereCast(transform.position, 0.5f, Vector3.down, out hitGround, playerHeight * 0.5f + 0.3f, whatIsGround);
        rb.linearDamping = onGround ? groundDrag : 0f;

        isGroundPounding = Keyboard.current.leftShiftKey.isPressed;

    }

    private void RotateCamera()
    {
        Vector2 targetLookVelocity = lookInput * mouseSensitivity;
        
        if (targetLookVelocity.magnitude > 0.01f)
        {
            currentLookVelocity = Vector2.Lerp(
                currentLookVelocity, 
                targetLookVelocity, 
                lookAcceleration * Time.deltaTime
            );
            
            Mathf.Clamp(currentLookVelocity.magnitude, 0, maxLookSpeed);
        }
        else
        {
            currentLookVelocity = Vector2.Lerp(
                currentLookVelocity, 
                Vector2.zero, 
                lookDeceleration * Time.deltaTime
            );
        }

        yaw += currentLookVelocity.x;
        pitch -= currentLookVelocity.y;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, currentBobRoll);
    }

    private void CameraBob()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        bool isMoving = speed > 0.1f && onGround && !isGroundPounding;
        
        if (isMoving)
        {
            float speedMultiplier = Mathf.Clamp01(speed / moveSpeed);
            float baseTime = bobTimer * bobFrequency;
            float halfTime = baseTime * 0.5f;
            
            float targetBob = Mathf.Sin(baseTime) * bobAmplitude * speedMultiplier;
            float targetRoll = Mathf.Sin(halfTime) * bobRollAmplitude * speedMultiplier;
            float targetHorizontal = Mathf.Sin(halfTime + Mathf.PI * 0.5f) * bobHorizontalAmplitude * speedMultiplier;
            
            currentBobOffset = Mathf.Lerp(currentBobOffset, targetBob, Time.fixedDeltaTime * bobSmoothing);
            currentBobRoll = Mathf.Lerp(currentBobRoll, targetRoll, Time.fixedDeltaTime * bobSmoothing);
            currentBobHorizontal = Mathf.Lerp(currentBobHorizontal, targetHorizontal, Time.fixedDeltaTime * bobSmoothing);
            
            bobTimer += Time.fixedDeltaTime;
        }
        else
        {
            currentBobOffset = Mathf.Lerp(currentBobOffset, 0f, Time.fixedDeltaTime * bobSmoothing);
            currentBobRoll = Mathf.Lerp(currentBobRoll, 0f, Time.fixedDeltaTime * bobSmoothing);
            currentBobHorizontal = Mathf.Lerp(currentBobHorizontal, 0f, Time.fixedDeltaTime * bobSmoothing);
            bobTimer = 0f;
        }
        
        cameraTransform.localPosition = originalCameraLocalPosition + 
            Vector3.up * currentBobOffset + 
            Vector3.right * currentBobHorizontal;
    }

    private void FixedUpdate()
    {
        RotateCamera();

        if (isGroundPounding && !onGround)
        {
            rb.AddForce(Vector3.down * groundPoundForce, ForceMode.Force);
        }
        
        if (!onGround)
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > maxAirSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxAirSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }
        MovePlayer();
        CameraBob();
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

    public void OnResetPos()
    {
            ResetToSpawn();
    }

    private void MovePlayer()
    {
        float wMove = moveInput.y;
        GameManager.Instance.SlicingPlaneOffset += wMove * wSpeed;
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.z;

        float multiplier = isGroundPounding ? 
            groundPoundMovementReduction : (onGround ? 1f : airMultiplier);
        
        rb.AddForce(moveDirection * moveSpeed * multiplier * (1 / Time.fixedDeltaTime), ForceMode.Force);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetToSpawn()
    {
        transform.position = initialSpawnPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        yaw = 0f;
        pitch = 0f;
        currentBobOffset = 0f;
        currentBobRoll = 0f;
        currentBobHorizontal = 0f;
        bobTimer = 0f;
    }
}