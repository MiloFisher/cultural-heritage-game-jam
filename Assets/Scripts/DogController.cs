using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    public bool particlesEnabled;
    public GameObject player;
    public float offset;

    public GameObject leftAttack;
    public GameObject rightAttack;

    public GameObject leftWalkParticles;
    public GameObject rightWalkParticles;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private GameObject shadow;
    private Animator anim;

    private float groundLevel = -2;
    private float dogOffset = -0.06f;
    private bool grounded;
    private bool jumping;
    private bool jumpPause;
    private float shadowY;
    private bool movingUp;
    private Vector3 shadowScale;
    private Vector3 shadowPosition;
    private bool attackOnCooldown;
    private bool attackPrimed;
    private bool primedDirection;

    private bool leftParticle;
    private bool rightParticle;

    // Start is called before the first frame update
    void Start()
    {
        shadow = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        shadowScale = shadow.transform.localScale;
        shadowPosition = shadow.transform.localPosition;
        groundLevel = transform.localPosition.y;
        PrimeAttack();
    }

    void Update()
    {
        leftParticle = false;
        rightParticle = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (player.transform.position.x > transform.position.x + offset)
        {
            // Move Right
            rb.velocity += (Vector2)transform.right * player.GetComponent<PlayerController>().speed;
            sr.flipX = true;
            leftParticle = true;
            if (primedDirection != sr.flipX && !attackOnCooldown)
            {
                PrimeAttack();
            }
        }
        if (player.transform.position.x < transform.position.x - offset)
        {
            // Move Left
            rb.velocity -= (Vector2)transform.right * player.GetComponent<PlayerController>().speed;
            sr.flipX = false;
            rightParticle = true;
            if (primedDirection != sr.flipX && !attackOnCooldown)
            {
                PrimeAttack();
            }
        }
        SetMoveState();

        if(!attackPrimed && !attackOnCooldown)
        {
            PrimeAttack();
        }
        if (!(rightParticle && leftParticle) && particlesEnabled)
        {
            leftWalkParticles.SetActive(leftParticle);
            rightWalkParticles.SetActive(rightParticle);
        }
        else
        {
            leftWalkParticles.SetActive(false);
            rightWalkParticles.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        grounded = transform.position.y <= groundLevel + dogOffset && !jumpPause;
        if (grounded)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            jumping = false;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    private void LateUpdate()
    {
        if (grounded)
        {
            transform.position = new Vector3(transform.position.x, groundLevel + dogOffset, transform.position.z);
        }
        if (jumping)
        {
            float dist = rb.velocity.y * 0.15f * Time.deltaTime;
            shadow.transform.position = new Vector3(shadow.transform.position.x, shadowY, 0);
            shadow.transform.localScale += new Vector3(dist, dist, 0);
        }
        else
        {
            shadow.transform.localScale = shadowScale;
            shadow.transform.localPosition = shadowPosition;
        }
        sr.sortingOrder = -Mathf.FloorToInt(shadow.transform.position.y * 100);
        leftWalkParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = sr.sortingOrder;
        rightWalkParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = sr.sortingOrder;
    }

    public void PrimeAttack()
    {
        attackPrimed = true;
        primedDirection = sr.flipX;
        leftAttack.SetActive(!sr.flipX);
        rightAttack.SetActive(sr.flipX);
        StartCoroutine(SetUp());
    }

    public void ResolveAttack()
    {
        attackPrimed = false;
        leftAttack.SetActive(false);
        rightAttack.SetActive(false);
        StartCoroutine(Cooldown());
    }

    IEnumerator SetUp()
    {
        yield return new WaitForFixedUpdate();
        leftAttack.transform.localPosition -= new Vector3(.1f, 0, 0);
        rightAttack.transform.localPosition += new Vector3(.1f, 0, 0);
        yield return new WaitForFixedUpdate();
        leftAttack.transform.localPosition += new Vector3(.1f, 0, 0);
        rightAttack.transform.localPosition -= new Vector3(.1f, 0, 0);
    }

    IEnumerator Cooldown()
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(3);
        attackOnCooldown = false;
    }

    public void Jump(float jumpHeight)
    {
        rb.velocity += (Vector2)transform.up * jumpHeight;
        StartCoroutine(JumpTimer());
        jumping = true;
        shadowY = shadow.transform.position.y;
        shadowScale = shadow.transform.localScale;
        shadowPosition = shadow.transform.localPosition;
    }

    public void ChangeDepth(float scale, float moveAmount, int direction, bool facingLeft)
    {
        if(direction == 0)
        {
            movingUp = true;
            transform.localScale = new Vector3(scale, scale, 0);
            transform.position += new Vector3(0, moveAmount, 0);
            groundLevel += moveAmount;
        }
        else
        {
            movingUp = true;
            transform.localScale = new Vector3(scale, scale, 0);
            transform.position -= new Vector3(0, moveAmount, 0);
            groundLevel -= moveAmount;
        }
        if (rb.velocity.x == 0)
            sr.flipX = !facingLeft;
    }

    IEnumerator JumpTimer()
    {
        jumpPause = true;
        yield return new WaitForSeconds(.25f);
        jumpPause = false;
    }

    private void SetMoveState()
    {
        if (!movingUp && rb.velocity.x == 0)
        {
            // Idle
            if (anim.GetInteger("moveState") != 0)
                anim.SetTrigger("updateMoveState");
            anim.SetInteger("moveState", 0);
        }
        else
        {
            // Moving
            if (anim.GetInteger("moveState") != 1)
                anim.SetTrigger("updateMoveState");
            anim.SetInteger("moveState", 1);
        }
        movingUp = false;
    }
}
