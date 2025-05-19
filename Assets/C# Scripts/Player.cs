using UnityEngine;

public class Player : MonoBehaviour
{
    public float walkingSpeed;
    public float skateboardingSpeed;
    private float speed;
    public float walkingSmoothingSpeed;
    public float skateboardingSmoothingSpeed;
    private float smoothingSpeed;

    float smoothedInputX = 0;
    float smoothedInputY = 0;

    public GameObject skateboard;
    public GameObject skateboardCol;
    public bool isSkateBoarding = false;

    public Rigidbody2D rb;

    private void Awake()
    {
        skateboard = GameObject.Find("Skateboard");
    }

    private void Start()
    {
        skateboard.SetActive(false);
        rb = GetComponent<Rigidbody2D>();

        speed = walkingSpeed;
        smoothingSpeed = walkingSmoothingSpeed;
    }

    private void Update()
    {
        float rawInputX = Input.GetAxisRaw("Horizontal");
        float rawInputY = Input.GetAxisRaw("Vertical");

        smoothedInputX = Mathf.MoveTowards(smoothedInputX, rawInputX, smoothingSpeed * Time.deltaTime);
        smoothedInputY = Mathf.MoveTowards(smoothedInputY, rawInputY, smoothingSpeed * Time.deltaTime);

        CheckSkateboarding();
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = new Vector2(smoothedInputX, smoothedInputY);
        Vector2 targetVelocity = Vector2.ClampMagnitude(moveInput, 1f) * speed;
        rb.linearVelocity = targetVelocity;

        float angle = Mathf.Atan2(smoothedInputY, smoothedInputX) * Mathf.Rad2Deg;

        if (isSkateBoarding)
        {
            if (smoothedInputX <= 0f && smoothedInputY != 0f)
                transform.rotation = Quaternion.Euler(0f, 0f, angle - 180f);

            else if (smoothedInputX != 0f && smoothedInputY == 0f)
                transform.rotation = Quaternion.identity;

            else if (smoothedInputX == 0f && smoothedInputY != 0f)
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            else
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void CheckSkateboarding()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSkateBoarding = !isSkateBoarding;

            if (isSkateBoarding)
            {
                speed = skateboardingSpeed;
                smoothingSpeed = skateboardingSmoothingSpeed;
                skateboard.SetActive(true);
                skateboardCol.SetActive(true);
            }

            else
            {
                speed = walkingSpeed;
                smoothingSpeed = walkingSmoothingSpeed;
                skateboard.SetActive(false);
                skateboardCol.SetActive(false);
            }
        }
    }
}
