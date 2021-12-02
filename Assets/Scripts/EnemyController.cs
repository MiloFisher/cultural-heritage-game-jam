using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool particlesEnabled;
    public float speed;
    public float distanceFromTarget;

    private GameObject player;
    private Transform playerShadow;
    public GameObject leftAttack;
    public GameObject rightAttack;

    public GameObject leftWalkParticles;
    public GameObject rightWalkParticles;

    private Transform leftBorder;
    private Transform rightBorder;

    private Inventory inventory;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private GameObject shadow;

    private bool grounded;
    private bool movingUp;

    private float groundLevel = -2;
    private float moveAmount;
    private const float upperWalkHeight = -1;
    private const float lowerWalkHeight = -3.5f;
    private bool topCollided;
    private bool bottomCollided;
    private int knockback;
    private bool attacking;
    private bool attackCooldown;
    public bool interrupted;

    private int health;
    private const int aggroRange = 18;

    private bool leftParticle;
    private bool rightParticle;

    private ParticleSystem ps;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerShadow = player.transform.GetChild(0);
        shadow = transform.GetChild(0).gameObject;
        leftBorder = GameObject.FindGameObjectWithTag("Left Border").transform;
        rightBorder = GameObject.FindGameObjectWithTag("Right Border").transform;
        groundLevel = transform.localPosition.y;
        health = 3 + inventory.globalLevel;
        ps = GetComponent<ParticleSystem>();
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
        if (knockback > 0)
            knockback--;
        else if (knockback < 0)
            knockback++;
    }

    private void LateUpdate()
    {
        if (grounded)
        {
            transform.position = new Vector3(transform.position.x, groundLevel, transform.position.z);
        }
        sr.sortingOrder = -Mathf.FloorToInt(shadow.transform.position.y * 100);
        leftWalkParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = sr.sortingOrder;
        rightWalkParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = sr.sortingOrder;
        GetComponent<ParticleSystemRenderer>().sortingOrder = sr.sortingOrder;
    }

    private void Movement()
    {
        moveAmount = 0.5f * speed * Time.deltaTime;
        rb.velocity = new Vector2(knockback, rb.velocity.y);
        movingUp = false;
        leftParticle = false;
        rightParticle = false;

        if (Mathf.Abs(playerShadow.position.x - shadow.transform.position.x) > aggroRange)
        {
            return;
        }

        if (Mathf.Abs(playerShadow.position.x - shadow.transform.position.x) <= distanceFromTarget && Mathf.Abs(playerShadow.position.y - shadow.transform.position.y) <= distanceFromTarget * 0.1f)
        {
            if (!attacking && !attackCooldown && !interrupted)
            {
                StartCoroutine(Attack());
            }
            return;
        }

        if (transform.position.y < upperWalkHeight && playerShadow.position.y > shadow.transform.position.y + distanceFromTarget * 0.1f && !topCollided && knockback == 0)
        {
            // Move Up
            anim.SetInteger("direction", playerShadow.position.x > shadow.transform.position.x ? 1 : 0);
            movingUp = true;
            transform.position += new Vector3(0, moveAmount, 0);
            float scale = 0.08f * (-transform.position.y - 1) + 0.9f;
            transform.localScale = new Vector3(scale, scale, 0);
            groundLevel += moveAmount;
        }
        if (transform.position.y > lowerWalkHeight && playerShadow.position.y < shadow.transform.position.y - distanceFromTarget * 0.1f && !bottomCollided && knockback == 0)
        {
            // Move Down
            anim.SetInteger("direction", playerShadow.position.x > shadow.transform.position.x ? 1 : 0);
            movingUp = true;
            transform.position -= new Vector3(0, moveAmount, 0);
            float scale = 0.08f * (-transform.position.y - 1) + 0.9f;
            transform.localScale = new Vector3(scale, scale, 0);
            groundLevel -= moveAmount;
        }
        // Move Left
        if (transform.position.x > leftBorder.position.x - 9 && playerShadow.position.x < shadow.transform.position.x - distanceFromTarget && knockback == 0)
        {
            // Set left velocity
            rb.velocity -= (Vector2)transform.right * speed;
            // Set animation direction to 0 (left)
            anim.SetInteger("direction", 0);
            rightParticle = true;
        }
        // Move Right
        if (transform.position.x < rightBorder.position.x + 9 && playerShadow.position.x > shadow.transform.position.x + distanceFromTarget && knockback == 0)
        {
            // Set right velocity   
            rb.velocity += (Vector2)transform.right * speed;
            // Set animation direction to 1 (right)
            anim.SetInteger("direction", 1);
            leftParticle = true;
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
            if (anim.GetInteger("moveState") != 1 || (rb.velocity.x > 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Knife_Looter_Run_Left")) || (rb.velocity.x < 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Knife_Looter_Run_Right")))
                anim.SetTrigger("updateMoveState");
            anim.SetInteger("moveState", 1);
        }
    }

    IEnumerator Attack()
    {
        GetComponent<AudioSource>().Play();
        attackCooldown = true;
        attacking = true;
        anim.SetBool("attacking", attacking);
        anim.SetTrigger("setAttacking");

        yield return new WaitForSeconds(.15f);
        leftAttack.SetActive(anim.GetInteger("direction") == 0);
        rightAttack.SetActive(anim.GetInteger("direction") == 1);

        yield return new WaitForFixedUpdate();
        leftAttack.transform.localPosition -= new Vector3(.1f, 0, 0);
        rightAttack.transform.localPosition += new Vector3(.1f, 0, 0);

        yield return new WaitForSeconds(.15f);
        leftAttack.SetActive(false);
        rightAttack.SetActive(false);
        leftAttack.transform.localPosition += new Vector3(.1f, 0, 0);
        rightAttack.transform.localPosition -= new Vector3(.1f, 0, 0);

        yield return new WaitForSeconds(.35f);
        attacking = false;
        anim.SetBool("attacking", attacking);

        yield return new WaitForSeconds(.5f);
        attackCooldown = false;
    }

    private void GetHit(string source, int direction)
    {
        shadow.GetComponent<AudioSource>().Play();
        if (source == "Player")
            health -= inventory.pickaxeAttack;
        else if(source == "Dog")
            health -= inventory.dogAttack;
        knockback += direction * 10;
        ps.Play();
        StartCoroutine(WhileHit(direction));
        if (interrupted)
            StopCoroutine(Interrupt());
        StartCoroutine(Interrupt());
    }

    IEnumerator WhileHit(int direction)
    {
        float degree = 25;
        transform.Rotate(new Vector3(0, 0, -degree * direction));
        shadow.transform.Rotate(new Vector3(0, degree * direction, 0));
        sr.color = new Color(.6f, 0, 0);
        yield return new WaitForSeconds(.25f);
        transform.Rotate(new Vector3(0, 0, degree * direction));
        shadow.transform.Rotate(new Vector3(0, -degree * direction, 0));
        sr.color = new Color(1, 1, 1);
        if(health <= 0)
        {
            if(Random.Range(0, 10) < 2) // 20% chance to drop an item 
                inventory.DropItem(transform.position - new Vector3(0, .5f, 0), transform.localScale, sr.sortingOrder);
            Destroy(gameObject);
        }
    }

    IEnumerator Interrupt()
    {
        for(int i = 0; i < 15; i++)
        {
            interrupted = true;
            yield return new WaitForSeconds(.1f);
        }
        interrupted = false;
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player Left Attack"))
        {
            GetHit("Player", -1);
        }
        else if (col.CompareTag("Player Right Attack"))
        {
            GetHit("Player", 1);
        }
        else if (col.CompareTag("Dog Left Attack"))
        {
            GetHit("Dog", -1);
            player.GetComponent<PlayerController>().dog.GetComponent<DogController>().ResolveAttack();
        }
        else if (col.CompareTag("Dog Right Attack"))
        {
            GetHit("Dog", 1);
            player.GetComponent<PlayerController>().dog.GetComponent<DogController>().ResolveAttack();
        }
    }
}
