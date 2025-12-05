//using System;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class PlayerController : MonoBehaviour
//{
//    public enum ColorState { Red, Green, Blue }
//    [SerializeField] float[] laneX = new float[] { -4f, 0f, 4f };
//    int targetLane = 1;

//    [Header("Movement")]
//    [SerializeField] float laneMoveSpeed = 14f;
//    [SerializeField] float forwardSpeed = 10f;
//    Rigidbody rb;

//    [Header("Animator")]
//    [SerializeField] Animator anim;

//    [Header("Tile Materials UI")]
//    [SerializeField] Material redMat;
//    [SerializeField] Material greenMat;
//    [SerializeField] Material blueMat;

//    [Header("Jump Settings")]
//    [SerializeField] float jumpHeight = 7f;
//    [SerializeField] float jumpForwardBoost = 10f;
//    [SerializeField] float jumpCooldown = 1f;

//    bool canJump = true;
//    bool isGrounded = true;
//    float jumpTimer = 0f;

//    [Header("Ground Check")]
//    [SerializeField] Transform groundCheckPoint;
//    [SerializeField] float groundCheckRadius = 0.2f;
//    [SerializeField] LayerMask groundMask;

//    [Header("Color System")]
//    [SerializeField] Renderer[] bodyRenderers;
//    [SerializeField] public ColorState currentColor = ColorState.Red;
//    public ColorState CurrentColor => currentColor;

//    [Header("UI")]
//    [SerializeField] TextMeshProUGUI scoreText;

//    [Header("Jump UI")]
//    [SerializeField] Image jumpCooldownImage;

//    [Header("Next Color UI")]
//    [SerializeField] RawImage nextColorImage;

//    [Header("Audio")]
//    [SerializeField] AudioClip jumpSound;
//    [SerializeField] AudioClip laneChangeSound;
//    [SerializeField] AudioClip colorChangeSound;

//    AudioSource audioSource;

//    Vector2 touchStart;
//    bool isTouching = false;
//    [SerializeField] float swipeThreshold = 50f;
//    [SerializeField] float tapThreshold = 10f;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        if (rb != null) rb.freezeRotation = true;
//        audioSource = GetComponent<AudioSource>();
//        UpdatePlayerColorVisual();
//        UpdateNextColorUI();
//    }

//    void Update()
//    {
//        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

//        HandleInput();
//        GroundCheck();
//        HandleJumpCooldown();
//        AnimationControl();

//        if (scoreText) scoreText.text = Math.Floor(transform.position.z).ToString();
//    }

//    void FixedUpdate()
//    {
//        if (rb == null) return;

//        float targetX = laneX[Mathf.Clamp(targetLane, 0, laneX.Length - 1)];
//        Vector3 v = rb.linearVelocity;

//        v.x = (targetX - rb.position.x) * laneMoveSpeed;
//        v.z = GameManager.Instance != null ? GameManager.Instance.speed : forwardSpeed;

//        rb.linearVelocity = v;
//    }

//    void HandleInput()
//    {
//        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
//        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

//        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) ChangeLane(-1);
//        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) ChangeLane(+1);
//        if (Input.GetKeyDown(KeyCode.Space)) TryJump();
//    }

//    void ProcessTouch(Vector2 delta)
//    {
//        if (delta.magnitude < tapThreshold)
//        {
//            CycleColor();
//            return;
//        }
//        if (Mathf.Abs(delta.y) > swipeThreshold && delta.y > 0)
//        {
//            TryJump();
//            return;
//        }

//        if (Mathf.Abs(delta.x) > swipeThreshold)
//        {
//            if (delta.x > 0) ChangeLane(+1);
//            else ChangeLane(-1);
//        }
//    }

//    void ChangeLane(int dir)
//    {
//        int previousLane = targetLane;
//        targetLane = Mathf.Clamp(targetLane + dir, 0, laneX.Length - 1);

//        if (targetLane != previousLane && laneChangeSound && audioSource)
//            audioSource.PlayOneShot(laneChangeSound);
//    }

//    void TryJump()
//    {
//        if (!canJump || !isGrounded) return;
//        Jump();
//    }

//    void Jump()
//    {
//        if (audioSource && jumpSound) audioSource.PlayOneShot(jumpSound);
//        canJump = false;
//        jumpTimer = jumpCooldown;

//        if (rb != null)
//        {
//            Vector3 v = rb.linearVelocity;
//            v.y = 0f;
//            rb.linearVelocity = v;

//            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
//            rb.AddForce(Vector3.forward * jumpForwardBoost, ForceMode.Impulse);
//        }

//        if (anim)
//        {
//            anim.SetBool("IsJumping", true); // Only here when jumping actually starts
//        }
//    }

//    void GroundCheck()
//    {
//        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundMask);

//        if (isGrounded && anim)
//            anim.SetBool("IsJumping", false); // Reset when landing
//    }

//    void HandleJumpCooldown()
//    {
//        if (!canJump)
//        {
//            jumpTimer -= Time.deltaTime;
//            if (jumpTimer <= 0f && isGrounded)
//            {
//                canJump = true;
//                jumpTimer = 0f;
//            }
//        }

//        if (jumpCooldownImage)
//            jumpCooldownImage.fillAmount = canJump ? 1f : 1f - (jumpTimer / jumpCooldown);
//    }

//    void AnimationControl()
//    {
//        if (!anim) return;

//        anim.SetFloat("Speed", GameManager.Instance != null ? GameManager.Instance.speed : forwardSpeed);
//    }

//    // ------- COLOR HANDLING (unchanged) -------
//    public void CycleColor()
//    {
//        if (audioSource && colorChangeSound) audioSource.PlayOneShot(colorChangeSound);
//        currentColor = currentColor switch
//        {
//            ColorState.Red => ColorState.Green,
//            ColorState.Green => ColorState.Blue,
//            ColorState.Blue => ColorState.Red,
//            _ => ColorState.Red
//        };
//        UpdatePlayerColorVisual();
//        UpdateNextColorUI();
//    }

//    ColorState GetNextColor()
//    {
//        return currentColor switch
//        {
//            ColorState.Red => ColorState.Green,
//            ColorState.Green => ColorState.Blue,
//            ColorState.Blue => ColorState.Red,
//            _ => ColorState.Red
//        };
//    }

//    void UpdateNextColorUI()
//    {
//        if (!nextColorImage) return;
//        ColorState nextColor = GetNextColor();
//        if (nextColor == ColorState.Red) nextColorImage.texture = redMat != null ? redMat.mainTexture : null;
//        else if (nextColor == ColorState.Green) nextColorImage.texture = greenMat != null ? greenMat.mainTexture : null;
//        else if (nextColor == ColorState.Blue) nextColorImage.texture = blueMat != null ? blueMat.mainTexture : null;
//    }

//    void UpdatePlayerColorVisual()
//    {
//        if (bodyRenderers == null || bodyRenderers.Length == 0) return;

//        Color c = currentColor switch
//        {
//            ColorState.Green => Color.green,
//            ColorState.Blue => Color.blue,
//            _ => Color.red
//        };

//        foreach (Renderer r in bodyRenderers)
//        {
//            if (r == null) continue;
//            Material m = new Material(r.material);
//            m.color = c;
//            r.material = m;
//        }
//    }
//}
