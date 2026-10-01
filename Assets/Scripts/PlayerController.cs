using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Aiming Settings")]
    [SerializeField] private LayerMask mousePlaneLayer;
    [SerializeField] private float rotationSpeed = 25f;

    [Header("Movement & Boost Settings")]
    [SerializeField] private float accelerationForce = 35f;
    [SerializeField] private float maxSpeed = 15f;
    [Tooltip("0 = Max Drift/Ice, 1 = Sharp Instant Turn Response")]
    [Range(0f, 1f)]
    [SerializeField] private float lateralDamping = 0.85f;

    private Rigidbody rb;
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleAiming();
    }

    private void FixedUpdate()
    {
        HandleBoost();
    }

    private void HandleAiming()
    {
        // Cast a ray from camera through mouse cursor position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, mousePlaneLayer))
        {
            // Lock target Y coordinate to ship height to prevent tilting
            Vector3 targetPoint = hitInfo.point;
            targetPoint.y = transform.position.y;

            Vector3 lookDir = targetPoint - transform.position;

            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleBoost()
    {
        // Right Mouse Button (Input 1) accelerates in the facing direction
        if (Input.GetMouseButton(1))
        {
            // Apply forward force along the ship's facing direction
            rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);

            // Separate forward and sideways velocity components
            Vector3 forwardVelocity = Vector3.Project(rb.linearVelocity, transform.forward);
            Vector3 rightVelocity = Vector3.Project(rb.linearVelocity, transform.right);

            // Dampen sideways velocity to tighten turning radius and reduce heavy outward drift
            rb.linearVelocity = forwardVelocity + (rightVelocity * (1f - lateralDamping));

            // Clamp total speed to maxSpeed cap
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }
}