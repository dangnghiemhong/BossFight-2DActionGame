//using UnityEngine;

//public class Projectile : MonoBehaviour
//{
//    [SerializeField] private float speed;
//    private float direction;
//    private bool hit;
//    private float lifetime;

//    public static float dame;

//    private Animator anim;
//    private BoxCollider2D boxCollider;

//    private void Awake()
//    {
//        anim = GetComponent<Animator>();
//        boxCollider = GetComponent<BoxCollider2D>();
//    }
//    private void Update()
//    {
//        if (hit) return;
//        float movementSpeed = speed * Time.deltaTime * direction;
//        transform.Translate(movementSpeed, 0, 0);

//        lifetime += Time.deltaTime;
//        if (lifetime > 5) gameObject.SetActive(false);
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        // debug 
//        Debug.Log("a");
//        hit = true;
//        boxCollider.enabled = false;
//        anim.SetTrigger("explode");
//            if (collision.tag == "Enemy")
//                collision.GetComponent<Health>().TakeDamage(1);
        
//    }
//    public void SetDirection(float _direction)
//    {
//        //Debug.Log(_direction);
//        lifetime = 0;
//        direction = _direction;
//        gameObject.SetActive(true);
//        hit = false;
//        boxCollider.enabled = true;

//        float localScaleX = transform.localScale.x;
//        Debug.Log("localScale = " + localScaleX);
//        if (Mathf.Sign(localScaleX) != _direction)
//        {
//            Debug.Log("_direction = " +_direction);
//            localScaleX = -localScaleX;
//            Debug.Log("localScale =" + localScaleX);
//        }

//        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
//        Debug.Log(localScaleX);
//    }
//    private void Deactivate()
//    {
//        gameObject.SetActive(false);
//    }
//}