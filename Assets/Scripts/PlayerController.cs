using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool particlesEnabled;
    public GameObject dog;

    public float speed;
    public float jumpHeight;
    public string returnScene;
    public string nextScene;

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
    public bool grounded;
    private bool movingUp;
    public bool jumping;
    private bool jumpPause;

    private float groundLevel = -2;
    private float moveAmount;
    private GameObject shadow;
    private float shadowY;
    private Vector3 shadowScale;
    private Vector3 shadowPosition;
    private float upperWalkHeight = -1;
    private float lowerWalkHeight = -3.5f;
    private bool topCollided;
    private bool bottomCollided;
    private bool attacking;
    private int knockback;

    public int health;
    private bool immune;

    private bool leftParticle;
    private bool rightParticle;

    private ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        shadow = transform.GetChild(0).gameObject;
        shadowScale = shadow.transform.localScale;
        shadowPosition = shadow.transform.localPosition;
        leftBorder = GameObject.FindGameObjectWithTag("Left Border").transform;
        rightBorder = GameObject.FindGameObjectWithTag("Right Border").transform;
        groundLevel = transform.localPosition.y;
        health = inventory.flaskHealth;
        ps = GetComponent<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        grounded = transform.position.y <= groundLevel && !jumpPause;
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
        if (knockback > 0)
            knockback--;
        else if (knockback < 0)
            knockback++;
    }

    private void Update()
    {
        Movement();
        SetMoveState();
    }

    private void LateUpdate()
    {
        if (grounded)
        {
            transform.position = new Vector3(transform.position.x, groundLevel, transform.position.z);
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
        CameraTrack();
    }

    private void GameOver()
    {
        GameObject.FindGameObjectWithTag("Fail").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("Scene Changer").GetComponent<SceneChanger>().ChangeScene("Town");
    }

    private void Movement()
    {
        moveAmount = 0.5f * speed * Time.deltaTime;
        rb.velocity = new Vector2(knockback, rb.velocity.y);
        movingUp = false;
        leftParticle = false;
        rightParticle = false;

        // Jump
        //if (Input.GetKeyDown(KeyCode.Space) && grounded)
        //{
        //    // Set shadow values prior to jumping
        //    shadowY = shadow.transform.position.y;
        //    shadowScale = shadow.transform.localScale;
        //    shadowPosition = shadow.transform.localPosition;
        //    // Set upward velocity
        //    rb.velocity += jumpHeight * transform.localScale.y * (Vector2)transform.up;
        //    // Call Dog jump simultaneously
        //    if (dog)
        //    {
        //        dog.GetComponent<DogController>().Jump(jumpHeight * transform.localScale.y);
        //    }
        //    // Set jumping flag
        //    jumping = true;
        //    StartCoroutine(JumpTimer());
        //}

        // Attack
        if (Input.GetMouseButtonDown(0))
        {
            if (!attacking)
            {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x <= transform.position.x)
                    anim.SetInteger("direction", 0);
                else
                    anim.SetInteger("direction", 1);
                StartCoroutine(Attack());
            }
        }
        if (Input.GetKey(KeyCode.W) && !jumping && transform.position.y < upperWalkHeight && !topCollided && !attacking)
        {
            // Move Up
            anim.SetInteger("direction", 1);
            movingUp = true;
            transform.position += new Vector3(0, moveAmount, 0);
            float scale = 0.08f * (-transform.position.y - 1) + 0.9f;
            transform.localScale = new Vector3(scale, scale, 0);
            groundLevel += moveAmount;
            if (dog)
            {
                dog.GetComponent<DogController>().ChangeDepth(scale, moveAmount, 0, anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Run_Left"));
            }
        }
        if (Input.GetKey(KeyCode.S) && !jumping && transform.position.y > lowerWalkHeight && !bottomCollided && !attacking)
        {
            // Move Down
            anim.SetInteger("direction", 1);
            movingUp = true;
            transform.position -= new Vector3(0, moveAmount, 0);
            float scale = 0.08f * (-transform.position.y - 1) + 0.9f;
            transform.localScale = new Vector3(scale, scale, 0);
            groundLevel -= moveAmount;
            if (dog)
            {
                dog.GetComponent<DogController>().ChangeDepth(scale, moveAmount, 1, anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Run_Left"));
            }
        }
        // Move Left
        if (Input.GetKey(KeyCode.A) && transform.position.x > leftBorder.position.x - 9 && !attacking)
        {
            // Set left velocity
            rb.velocity -= (Vector2)transform.right * speed;
            // Set animation direction to 0 (left)
            anim.SetInteger("direction", 0);
            rightParticle = true;
        }
        // Move Right
        if (Input.GetKey(KeyCode.D) && transform.position.x < rightBorder.position.x + 9 && !attacking)
        {
            // Set right velocity
            rb.velocity += (Vector2)transform.right * speed;
            // Set animation direction to 1 (right)
            anim.SetInteger("direction", 1);
            leftParticle = true;
        }
        // If left of map, return
        if(transform.position.x <= leftBorder.position.x - 9 && returnScene != "")
        {
            GameObject.FindGameObjectWithTag("Scene Changer").GetComponent<SceneChanger>().ChangeScene(returnScene);
        }
        // If right of map, next
        if (transform.position.x >= rightBorder.position.x + 9 && nextScene != "" && inventory.objectiveCompleted)
        {
            GameObject.FindGameObjectWithTag("Scene Changer").GetComponent<SceneChanger>().ChangeScene(nextScene);
        }
        if (!(rightParticle && leftParticle) && particlesEnabled)
        {
            leftWalkParticles.SetActive(leftParticle);
            rightWalkParticles.SetActive(rightParticle);
            if (leftParticle && !leftWalkParticles.GetComponent<ParticleSystem>().isPlaying)
                leftWalkParticles.GetComponent<ParticleSystem>().Play();
            if (rightParticle && !rightWalkParticles.GetComponent<ParticleSystem>().isPlaying)
                rightWalkParticles.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            leftWalkParticles.SetActive(false);
            rightWalkParticles.SetActive(false);
        }
    }

    IEnumerator Attack()
    {
        GetComponent<AudioSource>().Play();
        attacking = true;
        anim.SetBool("attacking", attacking);
        anim.SetTrigger("setAttacking");

        yield return new WaitForSeconds(.15f);
        leftAttack.SetActive(anim.GetInteger("direction") == 0);
        rightAttack.SetActive(anim.GetInteger("direction") == 1);

        yield return new WaitForFixedUpdate();
        leftAttack.transform.localPosition -= new Vector3(.1f, 0, 0);
        rightAttack.transform.localPosition += new Vector3(.1f, 0, 0);

        yield return new WaitForSeconds(.25f);
        leftAttack.SetActive(false);
        rightAttack.SetActive(false);
        leftAttack.transform.localPosition += new Vector3(.1f, 0, 0);
        rightAttack.transform.localPosition -= new Vector3(.1f, 0, 0);

        yield return new WaitForSeconds(.25f);
        attacking = false;
        anim.SetBool("attacking", attacking);
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
            if (anim.GetInteger("moveState") != 1 || (rb.velocity.x > 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Run_Left")) || (rb.velocity.x < 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Run_Right")))
                anim.SetTrigger("updateMoveState");
            anim.SetInteger("moveState", 1);
        }
    }

    private void CameraTrack()
    {
        if (transform.position.x < leftBorder.position.x)
        {
            Camera.main.transform.position = leftBorder.position;
        }
        else if (transform.position.x > rightBorder.position.x)
        {
            Camera.main.transform.position = rightBorder.position;
        }
        else
        {
            Camera.main.transform.position = new Vector3(transform.position.x, 0, -10);
        }
    }

    IEnumerator JumpTimer()
    {
        jumpPause = true;
        yield return new WaitForSeconds(.25f);
        jumpPause = false;
    }

    private void GetHit(int direction)
    {
        if (!immune)
        {
            shadow.GetComponent<AudioSource>().Play();
            ps.Play();
            health -= inventory.globalLevel;
            knockback += direction * 10;
            StartCoroutine(WhileHit(direction));
            if (health <= 0)
                GameOver();
        }
    }

    IEnumerator WhileHit(int direction)
    {
        immune = true;
        float degree = 25;
        transform.Rotate(new Vector3(0, 0, -degree * direction));
        shadow.transform.Rotate(new Vector3(0, degree * direction, 0));
        sr.color = new Color(.6f, 0, 0);
        yield return new WaitForSeconds(.25f);
        transform.Rotate(new Vector3(0, 0, degree * direction));
        shadow.transform.Rotate(new Vector3(0, -degree * direction, 0));
        sr.color = new Color(1, 1, 1);
        immune = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.GetComponent<ItemDrop>())
        {
            //Debug.Log("Collected: " + col.GetComponent<ItemDrop>().item.itemName);
            inventory.AddItem(col.GetComponent<ItemDrop>().item);
            Destroy(col.gameObject);
        }
        else if (col.GetComponent<NPCController>())
        {
            col.GetComponent<NPCController>().DisplayMessage();
        }
        else if (col.GetComponent<Podium>())
        {
            col.GetComponent<Podium>().DisplayMessage();
        }
        else if (col.CompareTag("Enemy Left Attack"))
        {
            GetHit(-1);
        }
        else if (col.CompareTag("Enemy Right Attack"))
        {
            GetHit(1);
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
        } else
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
