using Unity.Mathematics;
using UnityEngine.Events;
using UnityEngine;

public class FlyingPlayerMover : MonoBehaviour
{
    [Header("Levitation Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float verticalSpeed = 5f;
    [SerializeField] private float wSpeed = 0.2f;
    [SerializeField] private float drag = 1f;
    [SerializeField] private float seconds_till_desired = 0.1f;

    private Rigidbody rb;

    [Header("Camera Look")]
    [SerializeField] private float mouseSensitivity = 2f;

    public UnityEvent<float> WChanged = new();

    private float yaw;
    private float pitch;

    private Pose cam_target;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleWAxisMovement();
    }

    private void UpdatePlayerLook()
    {
        var prog = math.min(1, Time.deltaTime / seconds_till_desired);
        Camera.main.transform.position = Camera.main.transform.position + prog * (cam_target.position - Camera.main.transform.position);
        Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, cam_target.rotation, prog);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        cam_target.rotation = Quaternion.Euler(pitch, yaw, 0f);
        // Camera.main.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void HandleWAxisMovement()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            GameManager.Instance.SlicingPlaneOffset -= wSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            GameManager.Instance.SlicingPlaneOffset += wSpeed * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyDrag();
        cam_target.position = gameObject.transform.position;
        UpdatePlayerLook();
        // Camera.main.transform.localRotation *= transform.localRotation;
    }

    private void MovePlayer()
    {
        Vector2 moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) moveInput.y += 1f;
        if (Input.GetKey(KeyCode.S)) moveInput.y -= 1f;
        if (Input.GetKey(KeyCode.A)) moveInput.x -= 1f;
        if (Input.GetKey(KeyCode.D)) moveInput.x += 1f;

        moveInput = moveInput.normalized;

        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.Space)) verticalInput = 1f;
        if (Input.GetKey(KeyCode.LeftShift)) verticalInput -= 1f;

        Vector3 horizontalVelocity = moveDirection * moveSpeed;
        Vector3 verticalVelocity = Vector3.up * verticalInput * verticalSpeed;
        Vector3 targetVelocity = horizontalVelocity + verticalVelocity;

        rb.AddForce((targetVelocity - rb.linearVelocity) * moveSpeed, ForceMode.Force);
    }

    private void ApplyDrag()
    {
        rb.linearVelocity *= (1f - drag * Time.fixedDeltaTime);
    }
}
