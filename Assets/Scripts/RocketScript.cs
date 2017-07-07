using UnityEngine;
using System.Collections;

//---------------------------------------------------------------------------------
// Author		: Chen Yu
// Description	: Rocket and its flying script
//---------------------------------------------------------------------------------
public class RocketScript : MonoBehaviour 
{
    //===================
    // Public Variables
    //===================
    public float RocketSpeed = 10f;
    public static Vector3 Target = Vector3.zero;
    public AudioClip hitAudio;

    //===================
    // Private Variables
    //===================
    private Animator animator;
    private Rigidbody2D r2d;
    private AudioSource sFX;

    //flying physics of rocket
    private Vector3 aimingDirection;
    private Vector3 directionNorm;
    private Vector2 turning;
    private Vector2 increasedColliderSize;
    private Vector2 origin;
    private float currentVelocity = 1.0f;
    private float acceleration = 0.1f;
    private float turnSpeed = 0.05f;

    private float launchTime = 1f;
    private bool collided = false;
    private bool recalculatedDir = false;
    //---------------------------------------------------------------------------------
    // protected mono methods. 
    // Unity5: Rigidbody, Collider, Audio and other Components need to use GetComponent<name>()
    //---------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------
    // Awake is when the file is just loaded ... for other function blah blah
    //---------------------------------------------------------------------------------
    protected void Awake() 
	{
	}

	//---------------------------------------------------------------------------------
	// Start is when blah blah
	//---------------------------------------------------------------------------------
	protected void Start() 
	{
        animator = GetComponent<Animator>();
        r2d = GetComponent<Rigidbody2D>();
        directionNorm = (Target - transform.position).normalized;
        turning = new Vector2(directionNorm.x * 0.01f, 1f);
        aimingDirection = Target;
        //prepare increase collider value size for splash dmg
        increasedColliderSize = gameObject.GetComponent<BoxCollider2D>().size * 4;
        sFX = GetComponent<AudioSource>();

        if (SystemInfo.supportsGyroscope)
        {
            origin = new Vector2();
            Input.gyro.enabled = true;
        }
        else
        {
            origin = Input.acceleration;
        }
    }
	
	//---------------------------------------------------------------------------------
	// XXX is when blah blah
	//---------------------------------------------------------------------------------
	protected void Update() 
	{
        Vector2 changeorigin = new Vector2();
        if (SystemInfo.supportsGyroscope)
        {
            origin = new Vector2(origin.x + Input.gyro.rotationRateUnbiased.y, origin.y);
            changeorigin = new Vector2(origin.x * 0.05f, changeorigin.y);
            Debug.Log(changeorigin.x);
        }
        else
        {
            changeorigin = (Vector2)Input.acceleration * 3 - origin;
        } 
        //fly up first
        launchTime = launchTime - Time.deltaTime;
        
        if (launchTime <= 0)
        {
            if (!recalculatedDir)
            {
                //get turning direction after launch up
                directionNorm = (aimingDirection - transform.position).normalized;
                turning = new Vector2(directionNorm.x * 0.01f, Mathf.Abs(directionNorm.y));
                recalculatedDir = true;
            }
            if (currentVelocity < RocketSpeed)
            {
                currentVelocity = currentVelocity + acceleration;
            }
            if (turning.x <= directionNorm.x)
            {
                turning.x = turning.x + turnSpeed;
            }
            if (turning.x > directionNorm.x)
            {
                turning.x = turning.x - turnSpeed;
            }
            turning = new Vector2(turning.x + changeorigin.x * 0.1f, turning.y + Mathf.Abs(changeorigin.x * 0.01f));
            turning.x = (turning.x > 1) ? 1 : turning.x;
            turning.x = (turning.x < -1) ? -1 : turning.x;
            turning.y = (turning.y < -1) ? -1 : turning.y;
            turning.y = (turning.y < -1) ? -1 : turning.y;
        }
        
        if (!collided)
        {
            //turning and fly faster
            r2d.velocity = turning * currentVelocity;
            Vector2 moveDirection = turning;
            float angle = (Mathf.Atan2(moveDirection.y, moveDirection.x)) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 10 && collision.gameObject.GetComponent<Animator>().GetInteger("State") != 1)
        {
            //increase collider size for splash damage
            gameObject.GetComponent<BoxCollider2D>().size = increasedColliderSize;
            Debug.Log("Rocket Collider size: " + increasedColliderSize);
            r2d.velocity = Vector2.zero;
            collided = true;
            animator.SetInteger("State", 1);
            sFX.PlayOneShot(hitAudio, 1f);
            Destroy(gameObject, 2.175f);
        }
    }

    //---------------------------------------------------------------------------------
    // FixedUpdate for Physics update
    //---------------------------------------------------------------------------------
    protected void FixedUpdate() 
	{
        var screenposition = Camera.main.WorldToViewportPoint(transform.position);
        if (screenposition.x < -0.2f || screenposition.x > 1.2f || screenposition.y < -0.2f || screenposition.y > 1.2f)
        {
            Destroy(gameObject);
        }
    }
	
	//---------------------------------------------------------------------------------
	// XXX is when blah blah
	//---------------------------------------------------------------------------------
	protected void OnDestroy()
	{
	}
}
