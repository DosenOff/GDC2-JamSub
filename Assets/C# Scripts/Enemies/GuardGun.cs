using UnityEditor;
using UnityEngine;

public class GuardGun : MonoBehaviour
{
    public float detectionRange = 30f;
    public GameObject bulletPrefab;

    public Transform bulletSpawnPoint;
    public float rotationalSmoothing = 10f;
    public float shootCooldown = 1f;
    [SerializeField] private float currentShootCooldown;
    [SerializeField] private float fakePredictionTime = 999999f;

    public LayerMask layersToDetect;

    private Transform handle;
    Vector3 initialHandlePos;
    bool isFlipped = false;

    Player player;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        handle = transform.GetChild(0).GetComponent<Transform>();
        initialHandlePos = handle.localPosition;
        currentShootCooldown = shootCooldown;
    }

    void Update()
    {
        Vector2 direction = (Vector2)(player.transform.position - transform.position).normalized;
        RaycastHit2D lineOfSight = Physics2D.Raycast(transform.position, direction, detectionRange, layersToDetect);

        if (lineOfSight.collider != null)
        {
            Debug.DrawRay(transform.position, direction * detectionRange, lineOfSight.collider.CompareTag("Player") ? Color.red : Color.green);

            if (lineOfSight.collider.CompareTag("Player"))
            {
                Calculate();
                FlipGFX(direction);
            }
        }

        currentShootCooldown -= Time.deltaTime;
    }

    void Calculate()
    {
        /// could optimize; gets bullet speed
        bulletPrefab.TryGetComponent<GuardBullet>(out GuardBullet guardBullet);
        float bulletSpeed = guardBullet.speed;

        /// predicts target location
        float predictionTime = Vector3.Distance(player.transform.position, transform.position) / bulletSpeed;
        fakePredictionTime = predictionTime;

        Vector2 predictedPos = (Vector2)player.transform.position + (player.rb.linearVelocity * predictionTime);

        float currentAngle = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        float predAngle = Mathf.Atan2(predictedPos.y - transform.position.y, predictedPos.x - transform.position.x) * Mathf.Rad2Deg;

        float NormalizeAngle(float angle)
        {
            angle = angle % 360f;
            if (angle < 0) angle += 360f;
            return angle;
        }

        float ShortestAngleDistance(float fromAngle, float toAngle)
        {
            float difference = (toAngle - fromAngle + 540f) % 360f - 180f;
            return difference;
        }

        float currentAngleNorm = NormalizeAngle(currentAngle);
        float predAngleNorm = NormalizeAngle(predAngle);

        float angleDiff = ShortestAngleDistance(currentAngleNorm, predAngleNorm);

        float randomAngle = currentAngleNorm + angleDiff * Random.Range(0f, 1f);
        randomAngle = NormalizeAngle(randomAngle);

        Quaternion finalRotation = Quaternion.Euler(0f, 0f, randomAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationalSmoothing * Time.deltaTime);

        if (currentShootCooldown <= 0f)
        {
            currentShootCooldown = shootCooldown;
            Shoot(finalRotation);
        }
    }

    void Shoot(Quaternion rot)
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, rot);
    }

    void FlipGFX(Vector2 direction)
    {
        if (direction.x < 0f && !isFlipped)
        {
            handle.localPosition = new Vector3(handle.localPosition.x, -handle.localPosition.y, handle.localPosition.z);
            isFlipped = true;
        }
        else if (direction.x > 0f && isFlipped)
        {
            handle.localPosition = initialHandlePos;
            isFlipped = false;
        }
    }
}
