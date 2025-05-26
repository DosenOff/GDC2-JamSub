/*
using UnityEngine;

public class SkateBoardBounce : MonoBehaviour
{
}
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float walkingSpeed;
    public float skateboardingSpeed;
    public float skateboardChargingSpeed;
    public float speed;

    public float walkingSmoothingSpeed;
    public float skateboardingSmoothingSpeed;
    private float smoothingSpeed;

    float smoothedInputX = 0;
    float smoothedInputY = 0;

    public Rigidbody2D rb;


    // Skateboard Logic
    public SkateboardState currentState = SkateboardState.Idle;
    private Dictionary<SkateboardState, Action> stateActions;

    private Transform skateboardLoc;

    public GameObject skateboardObj;
    public GameObject skateboardCol;

    public bool isSkateboarding = false;
    public bool isChargingSkateboard = false;

    // Acceleration for both rotation and throwStrength
    public float chargingAcceleration = 50f;

    public float maxThrowSpeed = 25f;
    [SerializeField] private float currentThrowSpeed;

    public bool holding = false;

    GameObject skateboardGFX;
    Skateboard skateboard;

    private GameObject detectSkateboard;

    private void Awake()
    {
        skateboardObj = GameObject.Find("Skateboard");
        skateboard = skateboardObj.GetComponent<Skateboard>();
        skateboardGFX = skateboardObj.transform.Find("GFX").gameObject;
        skateboardLoc = GameObject.Find("SkateBoardLocation").GetComponent<Transform>();

        rb = GetComponent<Rigidbody2D>();
        skateboardGFX.SetActive(false);

        speed = walkingSpeed;
        smoothingSpeed = walkingSmoothingSpeed;

        //Dictionary initialization
        stateActions = new Dictionary<SkateboardState, Action>
        {
            { SkateboardState.Idle, HandleIdleState },
            { SkateboardState.Charging, HandleChargingState },
            { SkateboardState.Thrown, HandleThrownState },
            { SkateboardState.Riding, HandleRidingState },
        };

        detectSkateboard = GameObject.Find("DetectSkateboard");
    }

    private void Update()
    {
        float rawInputX = Input.GetAxisRaw("Horizontal");
        float rawInputY = Input.GetAxisRaw("Vertical");

        smoothedInputX = Mathf.MoveTowards(smoothedInputX, rawInputX, smoothingSpeed * Time.deltaTime);
        smoothedInputY = Mathf.MoveTowards(smoothedInputY, rawInputY, smoothingSpeed * Time.deltaTime);

        if (!isChargingSkateboard && !holding) // Prevent toggling while charging or throwing
            SkateboardToggle();

        CheckMouse();
        stateActions[currentState]?.Invoke();
    }

    // Handles rotation if isSkateboarding || !isSkateboarding
    private void FixedUpdate()
    {
        Vector2 moveInput = new Vector2(smoothedInputX, smoothedInputY);
        Vector2 targetVelocity = Vector2.ClampMagnitude(moveInput, 1f) * speed;
        rb.linearVelocity = targetVelocity;

        float angle = Mathf.Atan2(smoothedInputY, smoothedInputX) * Mathf.Rad2Deg;

        if (isSkateboarding)
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

    public enum SkateboardState
    {
        Idle,
        Charging,
        Thrown,
        Riding
    }

    void SkateboardToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && holding)
        {
            if (!isSkateboarding)
                currentState = SkateboardState.Riding;

            else if (isSkateboarding)
                currentState = SkateboardState.Idle;
        }
    }

    void CheckMouse()
    {
        if (holding)
        {
            if (Input.GetMouseButton(0))
            {
                currentState = SkateboardState.Charging;
                isChargingSkateboard = true;
            }

            else if (Input.GetMouseButtonUp(0))
            {
                currentState = SkateboardState.Thrown;
                isChargingSkateboard = false;
            }
        }
    }

    private void HandleIdleState()
    {
        skateboardCol.SetActive(false);
        skateboardObj.GetComponent<Collider2D>().enabled = false;

        speed = walkingSpeed;
        currentThrowSpeed = 0f;

        isSkateboarding = false;
        skateboardGFX.SetActive(false);

        skateboardObj.transform.position = skateboardLoc.transform.position;
        skateboardObj.transform.rotation = transform.rotation;
    }

    private void HandleRidingState()
    {
        skateboardCol.SetActive(true);
        skateboardObj.GetComponent<Collider2D>().enabled = false;

        speed = skateboardingSpeed;
        currentThrowSpeed = 0f;

        isSkateboarding = true;
        skateboardGFX.SetActive(true);

        skateboardObj.transform.position = skateboardLoc.transform.position;
        skateboardObj.transform.rotation = transform.rotation;
    }

    private void HandleChargingState()
    {
        skateboardCol.SetActive(false);
        skateboardObj.GetComponent<Collider2D>().enabled = true;

        isSkateboarding = false;
        skateboardGFX.SetActive(true);
        skateboard.transform.position = skateboardLoc.transform.position;

        currentThrowSpeed = Mathf.Lerp(0f, maxThrowSpeed, chargingAcceleration * Time.deltaTime);
        skateboard.transform.Rotate(0f, 0f, currentThrowSpeed);

        speed = Mathf.Lerp(speed, 0f, chargingAcceleration * Time.deltaTime);
    }

    private void HandleThrownState()
    {
        skateboardObj.GetComponent<Collider2D>().enabled = true;
        StartCoroutine(WaitDetectSkateboard());

        holding = false;
        isSkateboarding = false;

        speed = walkingSpeed;

        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        skateboardObj.GetComponent<Rigidbody2D>().linearVelocity = mousePos.normalized * currentThrowSpeed;

        currentThrowSpeed = Mathf.Lerp(currentThrowSpeed, 0f, chargingAcceleration * Time.deltaTime);

        skateboard.transform.Rotate(0f, 0f, currentThrowSpeed);
    }

    IEnumerator WaitDetectSkateboard()
    {
        detectSkateboard.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        detectSkateboard.SetActive(true);
    }
}
*/

