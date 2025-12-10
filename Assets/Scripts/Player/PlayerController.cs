using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public enum ColorState { Red, Green, Blue }

    [Header("Movement")]
    [SerializeField] float[] laneX = { -3.8f, 0f, 3.8f };
    [SerializeField] float laneMoveSpeed = 14f;
    int targetLane = 1;

    Rigidbody rb;

    [Header("Forward Speed")]
    [SerializeField] float forwardSpeed = 10f;

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
    [SerializeField] ScreenBorderGlow borderGlow;
 

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float jumpCooldown = 0.4f;
    float jumpTimer = 0f;
    bool jumpPressed = false;
    bool isGrounded = true;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] LayerMask groundMask;
    float rayDistance;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Image jumpCooldownImage;
    [SerializeField] RawImage nextColorImage;

    [Header("Audio")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip laneChangeSound;
    [SerializeField] AudioClip colorChangeSound;
    AudioSource audioSource;

    Vector2 touchStart;
    bool isTouching = false;
    [SerializeField] float swipeThreshold = 50f;
    [SerializeField] float tapThreshold = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();

        rayDistance = 0.45f * transform.localScale.y;

        ApplyMaterialToPlayer();
        UpdateNextColorUI();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        HandleInput();
        GroundCheck();
        HandleJumpCooldown();
        UpdateScore();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        float speed = GameManager.Instance != null ? GameManager.Instance.speed : forwardSpeed;

        Vector3 pos = rb.position;
        pos.x = Mathf.MoveTowards(pos.x, laneX[targetLane], laneMoveSpeed * Time.fixedDeltaTime);
        pos += Vector3.forward * speed * Time.fixedDeltaTime;

        rb.MovePosition(pos);
    }


    #region INPUT
    void HandleInput()
    {
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) ChangeLane(+1);
        if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            CycleColor();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            CycleColorBackward();
        }


        // Toggle first-person / third-person view
        if (Input.GetKeyDown(KeyCode.Q) && !GameManager.Instance.IsGameOver)
        {
            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            if (cam != null)
                cam.ToggleView();
        }



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

    if (EventSystem.current.IsPointerOverGameObject(t.fingerId)) return;

    if (t.phase == TouchPhase.Began)
    {
        isTouching = true;
        touchStart = t.position;
    }
    if (t.phase == TouchPhase.Ended && isTouching)
    {
        ProcessTouch(t.position - touchStart);
        isTouching = false;
    }
}
#endif


        if (jumpPressed && isGrounded && jumpTimer <= 0f)
            Jump();
    }

    void ProcessTouch(Vector2 delta)
    {
        if (delta.magnitude < tapThreshold) { CycleColor(); return; }
        if (Mathf.Abs(delta.y) > swipeThreshold && delta.y > 0) { jumpPressed = true; return; }
        if (Mathf.Abs(delta.x) > swipeThreshold) ChangeLane(delta.x > 0 ? 1 : -1);
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

    //void MoveLane()
    //{
    //    Vector3 pos = rb.position;
    //    pos.x = Mathf.MoveTowards(pos.x, laneX[targetLane], laneMoveSpeed * Time.fixedDeltaTime);
    //    rb.MovePosition(new Vector3(pos.x, rb.position.y, rb.position.z));
    //}

    //void MoveForward()
    //{
    //    float speed = GameManager.Instance != null ? GameManager.Instance.speed : forwardSpeed;
    //    rb.MovePosition(rb.position + Vector3.forward * speed * Time.fixedDeltaTime);
    //}
    #endregion

    #region JUMP
    void Jump()
    {
        if (audioSource && jumpSound) audioSource.PlayOneShot(jumpSound);

        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpPressed = false;
        jumpTimer = jumpCooldown;

        if (anim)
            anim.SetBool("isGrounded", false); // tell animator we left ground
    }

    void GroundCheck()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, rayDistance, groundMask);

        if (anim)
            anim.SetBool("isGrounded", isGrounded);

        Debug.DrawRay(groundCheckPoint.position, Vector3.down * rayDistance, isGrounded ? Color.green : Color.red);
    }

    void HandleJumpCooldown()
    {
        jumpTimer -= Time.deltaTime;

        if (jumpCooldownImage)
            jumpCooldownImage.fillAmount = Mathf.Clamp01(1f - jumpTimer / jumpCooldown);
    }
    #endregion

    #region UI / SCORE
    void UpdateScore()
    {
        if (scoreText)
            scoreText.text = Math.Floor(transform.position.z).ToString();
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

    public void CycleColorBackward()
    {
        if (audioSource && colorChangeSound)
            audioSource.PlayOneShot(colorChangeSound);

        currentColor = currentColor switch
        {
            ColorState.Red => ColorState.Blue,
            ColorState.Green => ColorState.Red,
            ColorState.Blue => ColorState.Green,
            _ => ColorState.Red
        };

        ApplyMaterialToPlayer();
        UpdateNextColorUI();
    }




    void ApplyMaterialToPlayer()
    {
        Material m = currentColor switch
        {
            ColorState.Red => redMat,
            ColorState.Green => greenMat,
            ColorState.Blue => blueMat,
            _ => redMat
        };

        foreach (Renderer r in bodyRenderers)
            r.sharedMaterial = m;

        if (borderGlow != null)
            borderGlow.SetGlowColor(m.color);
    }

    void UpdateNextColorUI()
    {
        if (!nextColorImage) return;

        nextColorImage.texture = currentColor switch
        {
            ColorState.Red => greenMat.mainTexture,
            ColorState.Green => blueMat.mainTexture,
            ColorState.Blue => redMat.mainTexture,
            _ => null
        };
    }
    #endregion
}
