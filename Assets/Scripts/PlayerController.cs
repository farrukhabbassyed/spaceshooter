using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public enum WeaponType
    {
        StandardBullet,
        HitscanLaser
    }

    [Header("Aiming Settings")]
    [SerializeField] private LayerMask mousePlaneLayer;
    [SerializeField] private float rotationSpeed = 25f;

    [Header("Movement & Boost Settings")]
    [SerializeField] private float accelerationForce = 35f;
    [SerializeField] private float maxSpeed = 15f;
    [Range(0f, 1f)]
    [SerializeField] private float lateralDamping = 0.85f;

    [Header("Weapon Setup & Selection")]
    public WeaponType currentWeapon = WeaponType.StandardBullet;
    [SerializeField] private Transform firePoint;

    [Header("Projectile Cannon Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float projectileFireRate = 0.15f;
    [SerializeField] private AudioClip cannonFireSound;
    [SerializeField] private GameObject cannonMuzzleFlash;

    [Header("Hitscan Laser Settings")]
    [SerializeField] private LayerMask hitscanLayers;
    [SerializeField] private float laserRange = 35f;
    [SerializeField] private float laserFireRate = 0.1f;
    [SerializeField] private LineRenderer laserLineRenderer;
    [SerializeField] private float laserBeamDuration = 0.05f;
    [SerializeField] private GameObject laserHitEffectPrefab;
    [SerializeField] private AudioClip laserFireSound;

    [Header("Audio Components")]
    [SerializeField] private AudioSource audioSource;

    private Rigidbody rb;
    private Camera mainCamera;
    private float nextFireTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (laserLineRenderer == null) laserLineRenderer = GetComponent<LineRenderer>();

        if (laserLineRenderer != null)
        {
            laserLineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        HandleAiming();
        HandleWeaponSwitching();
        HandleFiring();
    }

    private void FixedUpdate()
    {
        HandleBoost();
    }

    private void HandleAiming()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = Vector3.zero;
        bool pointFound = false;

        // 1. Primary Check: Try Raycast against specified Mouse Plane LayerMask
        if (mousePlaneLayer != 0 && Physics.Raycast(ray, out RaycastHit hitInfo, 300f, mousePlaneLayer))
        {
            targetPoint = hitInfo.point;
            pointFound = true;
        }
        else
        {
            // 2. Fallback Check: Mathematical plane at the player's height (always works without colliders/layers)
            Plane playerPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
            if (playerPlane.Raycast(ray, out float enter))
            {
                targetPoint = ray.GetPoint(enter);
                pointFound = true;
            }
        }

        if (pointFound)
        {
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
        // Right Mouse Button (Input 1) accelerates
        if (Input.GetMouseButton(1))
        {
            rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);

            Vector3 forwardVelocity = Vector3.Project(rb.linearVelocity, transform.forward);
            Vector3 rightVelocity = Vector3.Project(rb.linearVelocity, transform.right);

            rb.linearVelocity = forwardVelocity + (rightVelocity * (1f - lateralDamping));

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }

    private void HandleWeaponSwitching()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f || Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = WeaponType.StandardBullet;
        }
        else if (scrollInput < 0f || Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = WeaponType.HitscanLaser;
        }
    }

    private void HandleFiring()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (currentWeapon == WeaponType.StandardBullet)
            {
                nextFireTime = Time.time + projectileFireRate;
                ShootProjectile();
            }
            else if (currentWeapon == WeaponType.HitscanLaser)
            {
                nextFireTime = Time.time + laserFireRate;
                ShootLaser();
            }
        }
    }

    private void ShootProjectile()
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + transform.forward;
        Quaternion spawnRot = transform.rotation;

        Instantiate(bulletPrefab, spawnPos, spawnRot);

        if (cannonMuzzleFlash != null)
        {
            GameObject flash = Instantiate(cannonMuzzleFlash, spawnPos, spawnRot);
            Destroy(flash, 1f);
        }

        if (cannonFireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(cannonFireSound);
        }
    }

    private void ShootLaser()
    {
        Vector3 origin = firePoint != null ? firePoint.position : transform.position + transform.forward;
        Vector3 direction = transform.forward;
        Vector3 endPoint = origin + direction * laserRange;

        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, laserRange, hitscanLayers))
        {
            endPoint = hitInfo.point;

            // Check for Debris or Chaser Enemy targets
            Debris debris = hitInfo.collider.GetComponent<Debris>();
            EnemyChaser enemy = hitInfo.collider.GetComponent<EnemyChaser>();

            if (debris != null)
{
    debris.DestroyDebris(); // Triggers score, explosion VFX, and destroys the GameObject
}
else if (enemy != null)
{
    enemy.DestroyEnemy();   // Triggers score, explosion VFX, and destroys the GameObject
}

            if (laserHitEffectPrefab != null)
            {
                GameObject hitVfx = Instantiate(laserHitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(hitVfx, 1f);
            }
        }

        if (laserLineRenderer != null)
        {
            StartCoroutine(RenderLaserBeam(origin, endPoint));
        }

        if (laserFireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(laserFireSound);
        }
    }

    private IEnumerator RenderLaserBeam(Vector3 startPos, Vector3 endPos)
    {
        laserLineRenderer.enabled = true;
        laserLineRenderer.SetPosition(0, startPos);
        laserLineRenderer.SetPosition(1, endPos);

        yield return new WaitForSeconds(laserBeamDuration);

        laserLineRenderer.enabled = false;
    }
}