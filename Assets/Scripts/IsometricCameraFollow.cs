using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    [Header("Target & Positioning")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 15f, -15f);
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Dynamic Boost Effects")]
    [SerializeField] private Camera cam;
    [SerializeField] private float baseOrthographicSize = 10f;
    [SerializeField] private float boostOrthographicSize = 13f;
    [SerializeField] private float zoomSpeed = 3f;

    private Rigidbody targetRb;

    private void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (target != null) targetRb = target.GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Smooth camera movement following player
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Dynamic Zoom based on player right-click boost / speed
        HandleDynamicZoom();
    }

    private void HandleDynamicZoom()
    {
        bool isBoosting = Input.GetMouseButton(1);
        float targetSize = isBoosting ? boostOrthographicSize : baseOrthographicSize;

        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
        }
        else
        {
            // Perspective camera field of view fallback
            float baseFOV = 60f;
            float boostFOV = 72f;
            float targetFOV = isBoosting ? boostFOV : baseFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        }
    }
}