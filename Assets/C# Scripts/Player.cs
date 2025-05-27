using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    public GameObject deathEffect;

    public SpriteRenderer skin;
    private Color skinColor;
    public float flashDuration = 0.1f;

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
    private Skateboard skateboard;
    public Rigidbody2D rb;
    private Rigidbody2D skateboardRb;

    [Header("State Flags")]
    public bool isSkateboarding = false;
    public bool isChargingSkateboard = false;
    public bool holding = false;

    [Header("Charging Settings")]
    public float chargingAcceleration = 50f;
    public float maxThrowSpeed = 25f;
    public float currentThrowSpeed = 0f;
    private float chargeTime = 0f;
    public float maxChargeTime = 2f;
    public float rotFactor = 0.5f;
    public float deaccelerationFactor = 3f;
    private Vector2 throwDir;
    private bool forceAdded = false;
    private float chargingAngularVelocity = 0f;

    public enum SkateboardState { Idle, Charging, Thrown, Riding }
    public SkateboardState currentState = SkateboardState.Idle;
    private Dictionary<SkateboardState, System.Action> stateActions;

    [Header("Score Settings")]
    public TMP_Text scoreText;
    public int score = 0;

    public GameObject canvas;

    public AudioClip deathClip;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        skateboard = skateboardObj.GetComponent<Skateboard>();
        skateboardGFX = skateboardObj.transform.Find("GFX").gameObject; 
        skateboardRb = skateboardObj.GetComponent<Rigidbody2D>();

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
    }

    private void Start()
    {
        currentHealth = maxHealth;
        audioSource = GameObject.Find("GlobalManager").GetComponent<AudioSource>();
        skinColor = skin.color;
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

        healthBar.SetHealth(currentHealth);
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
        
        if (smoothedInputX <= 0f && smoothedInputY != 0f)
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 180f);

        else if (smoothedInputX != 0f && smoothedInputY == 0f)
            transform.rotation = Quaternion.identity;

        else if (smoothedInputX == 0f && smoothedInputY != 0f)
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        else
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
        if (!holding)
            return;

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

    private void HandleIdleState()
    {
        speed = walkingSpeed;
        smoothingSpeed = walkingSmoothingSpeed;

        isSkateboarding = false;
        currentThrowSpeed = 0f;
        forceAdded = false;

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

        chargingAngularVelocity = Mathf.Lerp(chargingAngularVelocity, currentThrowSpeed * 30f, Time.deltaTime * 10f);
        skateboardRb.angularVelocity = chargingAngularVelocity;

        throwDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)skateboard.transform.position).normalized;
    }

    private void HandleThrownState()
    {
        skateboardObj.GetComponent<Collider2D>().enabled = true;

        holding = false;
        isSkateboarding = false;
        skateboardGFX.SetActive(true);

        speed = walkingSpeed;
        smoothingSpeed = walkingSmoothingSpeed;

        if (!forceAdded)
        {
            skateboardRb.linearVelocity = throwDir * currentThrowSpeed;
            skateboardRb.angularVelocity = currentThrowSpeed * 30f;
            forceAdded = true;
        }

        chargeTime = 0f;
        currentThrowSpeed = Mathf.Max(0f, currentThrowSpeed -= Time.deltaTime * deaccelerationFactor);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        FlashWhite();

        if (currentHealth <= 0)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            canvas.SetActive(false);

            audioSource.Stop();
            audioSource.clip = deathClip;
            audioSource.loop = false;
            audioSource.Play();

            StartCoroutine(Delay());
        }
    }

    void FlashWhite()
    {
        skin.color = Color.white;
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        yield return new WaitForSeconds(flashDuration);
        skin.color = skinColor;
    }

    IEnumerator Delay ()
    {
        yield return new WaitForSeconds(flashDuration);
        gameObject.SetActive(false);
    }

    public void AddScore(int addScore)
    {
        score += addScore;
        scoreText.text = score.ToString();
    }
}
