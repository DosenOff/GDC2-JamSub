using UnityEditor;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float shootCooldown = 1f;

    private Transform handle;
    Vector3 initialHandlePos;

    Player player;
    Guard guard;

    float scaleX;

    void OnDrawGizmos()
    {
        guard = transform.gameObject.GetComponentInParent<Guard>();
        Gizmos.DrawWireSphere(transform.position, guard.detectionRange);
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        handle = transform.GetChild(0).GetComponent<Transform>();
        initialHandlePos = handle.position;
    }

    void Update()
    {
        int layerMask = ~(1 << gameObject.layer);
        Vector2 direction = (Vector2)(player.transform.position - transform.position).normalized;
        RaycastHit2D lineOfSight = Physics2D.Raycast(transform.position, direction, guard.detectionRange, layerMask);

        if (lineOfSight.collider != null)
        {
            Debug.DrawRay(transform.position, direction * guard.detectionRange, lineOfSight.collider.CompareTag("Player") ? Color.red : Color.green);

            if (lineOfSight.collider.CompareTag("Player"))
            {
                Shoot();
                FlipGFX(direction);
            }
        }
    }

    void Shoot()
    {
        float angle = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void FlipGFX(Vector2 direction)
    {
        Debug.Log(direction.x);
        if (direction.x < 0f)
            handle.position = new Vector3(handle.position.x, -handle.position.y, handle.position.z);
        else
            handle.position = initialHandlePos;
    }
}
