using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkingSpeed = 15f;
    public float skateboardingSpeed = 30f;
    public float walkingSmoothingSpeed = 10f;
    public float skateboardingSmoothingSpeed = 14f;

    private float speed;
    private float smoothingSpeed;
    private float smoothedInputX = 0f;
    private float smoothedInputY = 0f;

    [Header("Skateboard References")]
    public GameObject skateboardObj;
    public GameObject skateboardCol;
    public Transform skateboardLoc;

    private GameObject skateboardGFX;
    private GameObject detectSkateboard;
    private Skateboard skateboard;
    public Rigidbody2D rb;

    [Header("State Flags")]
    public bool isSkateboarding = false;
    public bool isChargingSkateboard = false;
    public bool holding = false;

    [Header("Charging Settings")]
    public float chargingAcceleration = 50f;
    public float maxThrowSpeed = 25f;
    private float currentThrowSpeed = 0f;
    private float chargeTime = 0f;
    public float maxChargeTime = 2f;

    public enum SkateboardState { Idle, Charging, Thrown, Riding }
    public SkateboardState currentState = SkateboardState.Idle;
    private Dictionary<SkateboardState, System.Action> stateActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        skateboard = skateboardObj.GetComponent<Skateboard>();
        skateboardGFX = skateboardObj.transform.Find("GFX").gameObject;

        skateboardGFX.SetActive(false);
        speed = walkingSpeed;
        smoothingSpeed = walkingSmoothingSpeed;

        stateActions = new Dictionary<SkateboardState, System.Action>
        {
            { SkateboardState.Idle, HandleIdleState },
            { SkateboardState.Charging, HandleChargingState },
            { SkateboardState.Thrown, HandleThrownState },
            { SkateboardState.Riding, HandleRidingState }
        };

        detectSkateboard = GameObject.Find("DetectSkateboard");
    }

    private void Update()
    {
        float rawInputX = Input.GetAxisRaw("Horizontal");
        float rawInputY = Input.GetAxisRaw("Vertical");

        smoothedInputX = Mathf.MoveTowards(smoothedInputX, rawInputX, smoothingSpeed * Time.deltaTime);
        smoothedInputY = Mathf.MoveTowards(smoothedInputY, rawInputY, smoothingSpeed * Time.deltaTime);

        if (!isChargingSkateboard && holding)
            SkateboardToggle();

        CheckMouse();
        stateActions[currentState]?.Invoke();
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = new Vector2(smoothedInputX, smoothedInputY);
        Vector2 targetVelocity = Vector2.ClampMagnitude(moveInput, 1f) * speed;
        rb.linearVelocity = targetVelocity;

        HandleRotation();
    }

    private void HandleRotation()
    {
        if (!isSkateboarding)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        float angle = Mathf.Atan2(smoothedInputY, smoothedInputX) * Mathf.Rad2Deg;
        if (smoothedInputX == 0f && smoothedInputY != 0f)
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        else if (smoothedInputX != 0f)
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void SkateboardToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!isSkateboarding)
                currentState = SkateboardState.Riding;
            else
                currentState = SkateboardState.Idle;
        }
    }

    private void CheckMouse()
    {
        if (holding)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentState = SkateboardState.Charging;
                isChargingSkateboard = true;
                chargeTime = 0f;
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
        speed = walkingSpeed;
        smoothingSpeed = walkingSmoothingSpeed;

        isSkateboarding = false;
        currentThrowSpeed = 0f;

        skateboardGFX.SetActive(false);
        skateboardCol.SetActive(false);
        skateboardObj.GetComponent<Collider2D>().enabled = false;

        skateboardObj.transform.position = skateboardLoc.position;
        skateboardObj.transform.rotation = transform.rotation;
    }

    private void HandleRidingState()
    {
        speed = skateboardingSpeed;
        smoothingSpeed = skateboardingSmoothingSpeed;

        isSkateboarding = true;
        currentThrowSpeed = 0f;

        skateboardGFX.SetActive(true);
        skateboardCol.SetActive(true);
        skateboardObj.GetComponent<Collider2D>().enabled = false;

        skateboardObj.transform.position = skateboardLoc.position;
        skateboardObj.transform.rotation = transform.rotation;
    }

    private void HandleChargingState()
    {
        chargeTime += Time.deltaTime;
        chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        float chargePercent = chargeTime / maxChargeTime;
        currentThrowSpeed = chargePercent * maxThrowSpeed;

        speed = Mathf.Lerp(speed, 0f, chargingAcceleration * Time.deltaTime);

        isSkateboarding = false;
        skateboardGFX.SetActive(true);
        skateboardCol.SetActive(false);
        skateboardObj.GetComponent<Collider2D>().enabled = true;

        skateboardObj.transform.position = skateboardLoc.position;
        skateboardObj.transform.Rotate(0f, 0f, currentThrowSpeed * Time.deltaTime * 100f);
    }

    private void HandleThrownState()
    {
        Rigidbody2D skateboardRb = skateboardObj.GetComponent<Rigidbody2D>();
        skateboardObj.GetComponent<Collider2D>().enabled = true;

        holding = false;
        isSkateboarding = false;

        speed = walkingSpeed;
        smoothingSpeed = walkingSmoothingSpeed;

        Vector2 throwDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;

        skateboardRb.linearVelocity = throwDir * currentThrowSpeed;
        skateboardRb.angularVelocity = currentThrowSpeed * 30f;

        StartCoroutine(WaitDetectSkateboard());

        chargeTime = 0f;
        currentThrowSpeed = 0f;
    }

    IEnumerator WaitDetectSkateboard()
    {
        detectSkateboard.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        detectSkateboard.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == skateboardObj)
        {
            holding = true;
            currentState = SkateboardState.Idle;
        }
    }
}
