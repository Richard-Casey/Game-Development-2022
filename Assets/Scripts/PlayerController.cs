using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public AudioSource audioSource;

    [SerializeField]
    private float MovementSpeed;

    [SerializeField]
    private float JumpForce;

    private float xMove;
    private bool jumpTrigger;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private Animator anim;

    private readonly Vector2 leftOffset = new Vector2(-.1f, 0.0f);
    private readonly Vector2 rightOffset = new Vector2(.1f, 0.0f);

    private LayerMask groundMask;
    private int xScale = 1;

    private Vector3 velocity;
    [SerializeField]
    private float movementSmoothing = .015f;

    private bool facingRight = true;

    #region Event Handlers
    // event handlers for the input system
    public void OnMove(InputAction.CallbackContext context)
    {
        // we only deal with the horizontal values here
        float tempx = context.ReadValue<Vector2>().x;
        xMove = tempx * MovementSpeed;
        anim.SetFloat("XMove", Mathf.Abs(tempx));
        if (tempx < 0 && facingRight)
        {
            Flip();
            facingRight = false;
        }
        else if (tempx > 0 && !facingRight)
        {
            Flip();
            facingRight = true;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // the action phase "canceled" represents a button *release*
        if (context.action.phase == InputActionPhase.Started)
            jumpTrigger = true;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();

        groundMask = LayerMask.GetMask("Ground");
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        // Escape key DOES NOT WORK in editor mode through UNITY - trust this code works though.
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    


private bool CanJump()
    {
        // fire three raycasts, if all fail to hit something, we cannot jump
        Vector2 origin = new Vector2(transform.position.x, circleCollider.bounds.max.y);

        // fire a ray downwards at the edges of the sprite and down the middle
        RaycastHit2D left = Physics2D.Raycast(origin + leftOffset, Vector2.down, 0.25f, groundMask);
        RaycastHit2D center = Physics2D.Raycast(origin, Vector2.down, 0.25f, groundMask);
        RaycastHit2D right = Physics2D.Raycast(origin + rightOffset, Vector2.down, 0.25f, groundMask);

        // we can jump if either of the edges and the center ray have hit an object
        return (left.collider != null && center.collider != null) || (right.collider != null && center.collider != null);
    }

    private void FixedUpdate()
    {
        if(jumpTrigger)
        {
            if (CanJump())
            {
                // add an impulse force to throw our character into the sky
                rb.AddForce(new Vector2(0.0f, JumpForce), 
                    ForceMode2D.Impulse);
                // play the jump sound
                audioSource.Play();
            }
            // reset the trigger so we don't keep jumping.
            jumpTrigger = false;
        }

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(xMove, rb.velocity.y);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
    }
}
