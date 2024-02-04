//using UnityEngine;
//using System.Collections;


//public class PlayerMovement : MonoBehaviour
//{
//    // private bool ignoreLayerCollision;

//    [Header("Movement Parameters")]
//    [SerializeField] private float speed;
//    [SerializeField] private float jumpPower;

//    [Header("Coyote Time")]
//    [SerializeField] private float coyoteTime; //How much time the player can hang in the air before jumping
//    private float coyoteCounter; //How much time passed since the player ran off the edge

//    [Header("Multiple Jumps")]
//    [SerializeField] private int extraJumps;
//    private int jumpCounter;

//    [Header("Wall Jumping")]
//    [SerializeField] private float wallJumpX; //Horizontal wall jump force
//    [SerializeField] private float wallJumpY; //Vertical wall jump force

//    [Header("Layers")]
//    [SerializeField] private LayerMask groundLayer;
//    [SerializeField] private LayerMask wallLayer;

//    [Header("Player information")]
//    [SerializeField] public float maxHealth;
//    [SerializeField] public float currentHealth;
//    [SerializeField] public float maxMana;
//    [SerializeField] public float currentMana;
//    [SerializeField] public float manaRegenRate;
//    [SerializeField] public float manaCost;
//    [SerializeField] public static float attackDamage;
//    [SerializeField] private float attackRange;
//    private bool dead;

//    [Header("Player attack")]
//    [SerializeField] private float attackCooldown;
//    [SerializeField] private Transform firePoint;
//    [SerializeField] private Transform flamePoint;
//    [SerializeField] private GameObject[] fireballs;
//    [SerializeField] private GameObject[] flame;

//    [SerializeField] private LayerMask enemyLayer;
//    private float cooldownTimer = Mathf.Infinity;

//    [Header("iFrames")]
//    [SerializeField] private float iFramesDuration;
//    [SerializeField] private int numberOfFlashes;
//    private SpriteRenderer spriteRend;

//    [Header("Components")]
//    [SerializeField] private Behaviour[] components;

//    private Rigidbody2D body;
//    private bool invulnerable;
//    public Animator anim;
//    private BoxCollider2D boxCollider;
//    private float wallJumpCooldown;
//    private float horizontalInput;
//    private float verticalInput;
//    private bool isFlying = false;
//    public string newPosition;
//    public static PlayerMovement instance;


//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            if (instance != this)
//            {
//                Destroy(gameObject);
//            }
//        }
//        DontDestroyOnLoad(gameObject);
//    }
//    private void Start()
//    {
//        //Grab references for rigidbody and animator from object
//        body = GetComponent<Rigidbody2D>();
//        anim = GetComponent<Animator>();
//        boxCollider = GetComponent<BoxCollider2D>();
//        spriteRend = GetComponent<SpriteRenderer>();
//        currentMana = maxMana;
//        currentHealth = maxHealth;
//    }

//    private void Update()
//    {
//        //Debug.Log(verticalInput);
//        if (currentHealth < 0)
//        {
//            currentHealth = 0;
//        }
//        if (currentMana < 0)
//        {
//            currentMana = 0;
//        }
//        if (currentMana > maxMana)
//        {
//            currentMana = maxMana;
//        }
//        if (currentHealth > maxHealth)
//        {
//            currentHealth = maxHealth;
//        }
//        horizontalInput = Input.GetAxis("Horizontal");
//        verticalInput = Input.GetAxis("Vertical");
//        RegenerateMana();

//        //Flip player when moving left-right
//        if (horizontalInput > 0.01f)
//            transform.localScale = Vector3.one;
//        else if (horizontalInput < -0.01f)
//            transform.localScale = new Vector3(-1, 1, 1);

//        //Set animator parameters
//        anim.SetBool("run", horizontalInput != 0);
//        anim.SetBool("grounded", isGrounded());

//        //Jump
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            if (isFlying == false)
//            {
//                Jump();
//            }
//        }



//        //Adjustable jump height
//        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
//            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

//        if (onWall())
//        {
//            body.gravityScale = 1;
//            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
//        }
//        else
//        {
//            body.gravityScale = 2;
//            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

//            if (isGrounded())
//            {
//                coyoteCounter = coyoteTime; //Reset coyote counter when on the ground
//                jumpCounter = extraJumps; //Reset jump counter to extra jump value
//            }
//            else
//                coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////////// Attack 

//        if (Input.GetKeyDown(KeyCode.Q) && cooldownTimer > attackCooldown)
//        {
//            Attack();
//        }
//        if (Input.GetKeyDown(KeyCode.W) && canCast() && cooldownTimer > attackCooldown)
//        {
//            CastSpell();
//        }
//        if (isFlying == true)
//        {
//            // transform.position = new Vector3(transform.position.x, 2, 0);
//            body.gravityScale = 0;
//        }
//        if (!isGrounded() && Input.GetKeyDown(KeyCode.E))
//        {
//            if (isFlying == true)
//            {
//                isFlying = false;
//                anim.SetBool("fly", false);
//                body.gravityScale += Time.deltaTime;
//            }
//            else
//            {
//                fly();
//            }
//        }

//        cooldownTimer += Time.deltaTime;
//    }

//    private void fly()
//    {

//        anim.SetBool("fly", true);
//        isFlying = true;

//        body.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
//    }
//    private void Jump()
//    {
//        if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0) return;
//        //If coyote counter is 0 or less and not on the wall and don't have any extra jumps don't do anything
//        anim.SetTrigger("jump");
//        if (onWall())
//            WallJump();
//        else
//        {
//            if (isGrounded())
//                body.velocity = new Vector2(body.velocity.x, jumpPower);
//            else
//            {
//                //If not on the ground and coyote counter bigger than 0 do a normal jump
//                if (coyoteCounter > 0)
//                    body.velocity = new Vector2(body.velocity.x, jumpPower);
//                else
//                {
//                    if (jumpCounter > 0) //If we have extra jumps then jump and decrease the jump counter
//                    {
//                        body.velocity = new Vector2(body.velocity.x, jumpPower);
//                        jumpCounter--;
//                    }
//                }
//            }

//            //Reset coyote counter to 0 to avoid double jumps
//            coyoteCounter = 0;
//        }
//    }

//    private void WallJump()
//    {
//        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
//        wallJumpCooldown = 0;
//    }


//    private bool isGrounded()
//    {
//        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
//        return raycastHit.collider != null;
//    }
//    private bool onWall()
//    {
//        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
//        return raycastHit.collider != null;
//    }
//    public bool canAttack()
//    {
//        return horizontalInput == 0 && isGrounded() && !onWall();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {

//        switch (collision.tag)
//        {
//            // case "flyingEnemy":
//            //     TakeDamage(dameEnemy1);
//            //     break;
//            // case "meleeEnemy":
//            //     TakeDamage(dameEnemy2);
//            //     break;
//            case "Enemy":
//                PlayerDamage(1);
//                break;
//            default:
//                break;
//        }

//    }
//    public void PlayerDamage(float _damage)
//    {
//        if (invulnerable) return;
//        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, maxHealth);


//        if (currentHealth > 0)
//        {
//            Debug.Log("Player damaged");
//            anim.SetTrigger("hurt");
//            StartCoroutine(Invunerability());
//        }
//        else
//        {
//            if (!dead)
//            {
//                anim.SetTrigger("die");

//                //Deactivate all attached component classes
//                foreach (Behaviour component in components)
//                    component.enabled = false;

//                dead = true;
//            }
//        }
//    }
//    public void AddHealth(float _value)
//    {
//        currentHealth = Mathf.Clamp(currentHealth + _value, 0, maxHealth);

//    }
//    private IEnumerator Invunerability()
//    {
//        invulnerable = true;
//        Physics2D.IgnoreLayerCollision(10, 11, true);
//        for (int i = 0; i < numberOfFlashes; i++)
//        {
//            spriteRend.color = new Color(1, 0, 0, 0.5f);
//            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
//            spriteRend.color = Color.white;
//            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
//        }
//        Physics2D.IgnoreLayerCollision(10, 11, false);
//        invulnerable = false;
//    }

//    private void Deactivate()
//    {
//        gameObject.SetActive(false);
//    }
//    private void CastSpell()
//    {
//        anim.SetTrigger("castSpell");
//        cooldownTimer = 0;

//        flame[FindFlame()].transform.position = flamePoint.position;
//        flame[FindFlame()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
//        // Sử dụng mana khi cast spell
//        useMana(1);
//    }

//    private void Attack()
//    {
//        anim.SetTrigger("attack");
//        cooldownTimer = 0;

//        fireballs[FindFireball()].transform.position = firePoint.position;
//        fireballs[FindFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
//    }
//    private int FindFireball()
//    {
//        for (int i = 0; i < fireballs.Length; i++)
//        {
//            if (!fireballs[i].activeInHierarchy)
//                return i;
//        }
//        return 0;
//    }
//    private int FindFlame()
//    {
//        for (int i = 0; i < flame.Length; i++)
//        {
//            if (!flame[i].activeInHierarchy)
//                return i;
//        }
//        return 0;
//    }

//    public bool canCast()
//    {
//        if (currentMana >= manaCost)
//        {
//            return true;
//        }
//        return false;
//    }
//    private void RegenerateMana()
//    {
//        // Hàm này sẽ được gọi mỗi frame để tăng mana theo tốc độ manaRegenRate
//        currentMana += manaRegenRate * Time.deltaTime;
//        currentMana = Mathf.Clamp(currentMana, 0, maxMana); // Đảm bảo không vượt quá giới hạn maxMana
//    }

//    public void useMana(float manaCost)
//    {
//        if (currentMana >= manaCost)
//        {
//            currentMana -= manaCost;
//        }
//        else
//        {
//            // Xử lý khi không đủ mana (nếu cần)

//        }
//        currentMana = Mathf.Max(currentMana, 0);
//    }

//}