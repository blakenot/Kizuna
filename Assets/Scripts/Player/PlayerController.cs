using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum ColorState { Red, Green, Blue }

    [Header("Movement")]
    [SerializeField] float[] laneX = { -4f, 0f, 4f };
    [SerializeField] float laneMoveSpeed = 14f;
    [SerializeField] float forwardSpeed = 10f;
    int targetLane = 1;
    Rigidbody rb;

    [Header("Animator")]
    [SerializeField] Animator anim;

    [Header("Materials")]
    [SerializeField] Material redMat;
    [SerializeField] Material greenMat;
    [SerializeField] Material blueMat;

    [Header("Color System")]
    [SerializeField] Renderer[] bodyRenderers;
    [SerializeField] public ColorState currentColor = ColorState.Red;
    public ColorState CurrentColor => currentColor;


    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Image jumpCooldownImage;
    [SerializeField] RawImage nextColorImage;

    [Header("Jump Settings")]
    [SerializeField] float jumpHeight = 7f;
    [SerializeField] float jumpForwardBoost = 10f;
    [SerializeField] float jumpCooldown = 1f;
    float jumpTimer = 0f;
    bool canJump = true;
    bool isGrounded = true;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundMask;

    [Header("Audio")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip laneChangeSound;
    [SerializeField] AudioClip colorChangeSound;
    AudioSource audioSource;

    // Touch controls
    Vector2 touchStart;
    bool isTouching = false;
    [SerializeField] float swipeThreshold = 50f;
    [SerializeField] float tapThreshold = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.freezeRotation = true;

        audioSource = GetComponent<AudioSource>();

        ApplyMaterialToPlayer();
        UpdateNextColorUI();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        HandleInput();
        GroundCheck();
        HandleJumpCooldown();

        if (scoreText)
            scoreText.text = Math.Floor(transform.position.z).ToString();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        float targetXPos = laneX[targetLane];
        Vector3 pos = rb.position;

        pos.x = Mathf.MoveTowards(pos.x, targetXPos, laneMoveSpeed * Time.fixedDeltaTime);

        float forwardVel = GameManager.Instance != null ? GameManager.Instance.speed : forwardSpeed;

        rb.MovePosition(new Vector3(pos.x, pos.y, pos.z + forwardVel * Time.fixedDeltaTime));
    }


    #region INPUT
    void HandleInput()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) ChangeLane(+1);
        if (Input.GetKeyDown(KeyCode.Space)) TryJump();

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            isTouching = true;
            touchStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && isTouching)
        {
            ProcessTouch((Vector2)Input.mousePosition - touchStart);
            isTouching = false;
        }
#else
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                isTouching = true;
                touchStart = t.position;
            }
            else if (t.phase == TouchPhase.Ended && isTouching)
            {
                ProcessTouch(t.position - touchStart);
                isTouching = false;
            }
        }
#endif
    }

    void ProcessTouch(Vector2 delta)
    {
        if (delta.magnitude < tapThreshold) { CycleColor(); return; }
        if (Mathf.Abs(delta.y) > swipeThreshold && delta.y > 0) { TryJump(); return; }
        if (Mathf.Abs(delta.x) > swipeThreshold) ChangeLane(delta.x > 0 ? +1 : -1);
    }
    #endregion

    #region MOVEMENT
    void ChangeLane(int direction)
    {
        int previous = targetLane;
        targetLane = Mathf.Clamp(targetLane + direction, 0, laneX.Length - 1);

        if (targetLane != previous && laneChangeSound && audioSource)
            audioSource.PlayOneShot(laneChangeSound);
    }

    void TryJump()
    {
        if (!canJump || !isGrounded) return;
        Jump();
    }

    void Jump()
    {
        if (audioSource && jumpSound) audioSource.PlayOneShot(jumpSound);

        canJump = false;
        jumpTimer = jumpCooldown;

        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        rb.AddForce(Vector3.forward * jumpForwardBoost, ForceMode.Impulse);

        if (anim)
        {
            anim.SetBool("IsJumping", true);
            anim.SetTrigger("Jump");
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundMask);
        if (anim) anim.SetBool("IsJumping", !isGrounded);
    }

    void HandleJumpCooldown()
    {
        if (!canJump)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f && isGrounded)
            {
                canJump = true;
                jumpTimer = 0f;
            }
        }

        if (jumpCooldownImage)
            jumpCooldownImage.fillAmount = canJump ? 1f : 1f - (jumpTimer / jumpCooldown);
    }
    #endregion

    #region COLOR
    public void CycleColor()
    {
        if (audioSource && colorChangeSound) audioSource.PlayOneShot(colorChangeSound);

        currentColor = currentColor switch
        {
            ColorState.Red => ColorState.Green,
            ColorState.Green => ColorState.Blue,
            ColorState.Blue => ColorState.Red,
            _ => ColorState.Red
        };

        ApplyMaterialToPlayer();
        UpdateNextColorUI();
    }

    void ApplyMaterialToPlayer()
    {
        Material targetMat = currentColor switch
        {
            ColorState.Red => redMat,
            ColorState.Green => greenMat,
            ColorState.Blue => blueMat,
            _ => redMat
        };

        foreach (Renderer r in bodyRenderers)
            if (r != null) r.sharedMaterial = targetMat; // NO Instancing
    }

    void UpdateNextColorUI()
    {
        if (!nextColorImage) return;

        ColorState nextColor = currentColor switch
        {
            ColorState.Red => ColorState.Green,
            ColorState.Green => ColorState.Blue,
            ColorState.Blue => ColorState.Red,
            _ => ColorState.Red
        };

        nextColorImage.texture = nextColor switch
        {
            ColorState.Red => redMat?.mainTexture,
            ColorState.Green => greenMat?.mainTexture,
            ColorState.Blue => blueMat?.mainTexture,
            _ => null
        };
    }
    #endregion
}
