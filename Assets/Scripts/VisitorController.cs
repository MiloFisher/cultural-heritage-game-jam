using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorController : MonoBehaviour
{
    public float speed;

    private Transform leftBorder;
    private Transform rightBorder;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private GameObject shadow;

    private bool grounded;
    private bool movingUp;

    private float groundLevel = -2;
    private float moveAmount;
    private float upperWalkHeight = -1;
    private float lowerWalkHeight = -3.5f;
    private bool topCollided;
    private bool bottomCollided;

    private int walkDirection = -1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        shadow = transform.GetChild(0).gameObject;
        leftBorder = GameObject.FindGameObjectWithTag("Left Border").transform;
        rightBorder = GameObject.FindGameObjectWithTag("Right Border").transform;
        groundLevel = transform.localPosition.y;
        StartCoroutine(VisitorAI());
    }

    void Update()
    {
        Movement();
        SetMoveState();
    }

    private void FixedUpdate()
    {
        grounded = transform.position.y <= groundLevel;
        if (grounded)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
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
            transform.position = new Vector3(transform.position.x, groundLevel, transform.position.z);
        }
        sr.sortingOrder = -Mathf.FloorToInt(shadow.transform.position.y * 100);
    }

    private IEnumerator VisitorAI()
    {
        for(; ; )
        {
            int moving = Random.Range(0, 5);
            if (moving == 0)
            {
                walkDirection = Random.Range(0, 7);
            }
            else
            {
                walkDirection = -1;
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void Movement()
    {
        moveAmount = 0.5f * speed * Time.deltaTime;
        rb.velocity = new Vector2(0, rb.velocity.y);
        movingUp = false;
        sr.flipX = false;

        if (transform.position.y < upperWalkHeight && (walkDirection == 1 || walkDirection == 2 || walkDirection == 3) && !topCollided)
        {
            // Move Up
            sr.flipX = true;
            movingUp = true;
            transform.position += new Vector3(0, moveAmount, 0);
            float scale = 0.08f * (-transform.position.y - 1) + 0.9f;
            transform.localScale = new Vector3(scale, scale, 0);
            groundLevel += moveAmount;
        }
        if (transform.position.y > lowerWalkHeight && (walkDirection == 5 || walkDirection == 6 || walkDirection == 7) && !bottomCollided)
        {
            // Move Down
            sr.flipX = true;
            movingUp = true;
            transform.position -= new Vector3(0, moveAmount, 0);
            float scale = 0.08f * (-transform.position.y - 1) + 0.9f;
            transform.localScale = new Vector3(scale, scale, 0);
            groundLevel -= moveAmount;
        }
        // Move Left
        if (transform.position.x > leftBorder.position.x - 9 && (walkDirection == 3 || walkDirection == 4 || walkDirection == 5))
        {
            // Set left velocity
            rb.velocity -= (Vector2)transform.right * speed;
            // Set animation direction to 0 (left)
            sr.flipX = false;
        }
        // Move Right
        if (transform.position.x < rightBorder.position.x + 9 && (walkDirection == 1 || walkDirection == 0 || walkDirection == 7))
        {
            // Set right velocity   
            rb.velocity += (Vector2)transform.right * speed;
            // Set animation direction to 1 (right)
            sr.flipX = true;
        }
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
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        bool hitOnTopOrBottom = true;
        float y = collision.contacts[0].point.y;
        for (int i = 1; i < collision.contactCount; i++)
        {
            if (collision.contacts[i].point.y != y)
            {
                hitOnTopOrBottom = false;
                break;
            }
        }
        if (hitOnTopOrBottom)
        {
            topCollided = transform.position.y + GetComponent<BoxCollider2D>().offset.y < y;
            bottomCollided = transform.position.y + GetComponent<BoxCollider2D>().offset.y > y;
        }
        else
        {
            topCollided = false;
            bottomCollided = false;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        topCollided = false;
        bottomCollided = false;
    }
}
