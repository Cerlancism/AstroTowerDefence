using System.Collections;
using UnityEngine;

//---------------------------------------------------------------------------------
// Author		: Chen Yu
// Description	: Mechanics of all spawned meteors
//---------------------------------------------------------------------------------
public class MeteorGeneralScript : MonoBehaviour 
{
    //===================
    // Public Variables
    //===================
    public GameObject MeteorChild2Clone;
    public int HitPoints = 0;
    public int ResourceUnits;
    public int Damage;
    public AudioClip ExplodeAudio;
    public AudioClip AtmAudio;

    //===================
    // Private Variables
    //===================
    private Animator animator;
    private Rigidbody2D r2d;
    private AudioSource sFX;

    private float generaltoDestoryTTL = 15f;
    private float smallMeteorTTL = 1.5f;
    private float tinyMeteorTTL = 2f;

    private bool deflected = false;
    //---------------------------------------------------------------------------------
    // protected mono methods. 
    // Unity5: Rigidbody, Collider, Audio and other Components need to use GetComponent<name>()
    //---------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------
    // Start is when blah blah
    //---------------------------------------------------------------------------------
    void Start() 
	{
        animator = GetComponent<Animator>();
        r2d = GetComponent<Rigidbody2D>();
        //Meteors will always appear moving right to left direction
        r2d.AddForce(Vector2.left * Random.Range(r2d.mass * 10, r2d.mass * 50));

        sFX = GetComponent<AudioSource>();

    }
	
	//---------------------------------------------------------------------------------
	// XXX is when blah blah
	//---------------------------------------------------------------------------------
	protected void Update() 
	{
        //Rotating tiny meteor for its tail to look realistic
        if (gameObject.tag == "MeteorTiny")
        {
            tinyMeteorTTL = tinyMeteorTTL - Time.deltaTime;
            Vector2 moveDirection = r2d.velocity;
            float angle = (Mathf.Atan2(moveDirection.y, moveDirection.x)+45) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (tinyMeteorTTL <= 0)
            {
                tinyMeteorTTL = 2f;
                DestroyMeteorTiny();
            }
        }

        if (gameObject.tag == "MeteorSmall")
        {
            if (animator.GetInteger("State") == 0)
            {
                r2d.rotation = (r2d.rotation + 10) % 360;
            }
            //Small meteors will burn down to tiny meteor if they entered from the atmophere layer
            if (animator.GetInteger("State") == 2)
            {
                smallMeteorTTL = smallMeteorTTL - Time.deltaTime;

                Vector2 moveDirection = r2d.velocity;
                float angle = (Mathf.Atan2(moveDirection.y, moveDirection.x) + 45) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            if (smallMeteorTTL <= 0)
            {
                DestroyMeteorSmall();
            }
        }
    }

    private void FixedUpdate()
    {
        //Timer to determine if the meteor is invisible to on screen and to be destroyed to reclaim memeory
        generaltoDestoryTTL = generaltoDestoryTTL - Time.deltaTime;

        //Destroy if already below surface
        if (gameObject.transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        //Destory the meteor if it fly off the screen
        if (generaltoDestoryTTL <= 0)
        {
            Destroy(gameObject);
            Debug.Log("A" + gameObject.tag + "OnBecameInvisible() too long and destoryed");
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        int layer = other.gameObject.layer;
        
        //Tiny meteors destroys each other when hit
        if (gameObject.tag == "MeteorTiny" && other.gameObject.tag != "MeteorSmall")
        {
            DestroyMeteorTiny();
            return;
        }

        //Ground collision of meteors
        if (layer == LayerMask.NameToLayer("Ground"))
        {
            animator.SetInteger("State", 1);
            gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            r2d.velocity = Vector2.zero;


            if (gameObject.tag != "MeteorTiny")
            {
                GlobalController.ShakeGround();
                GlobalController.ChangeResource(ResourceUnits);
                GlobalController.ChangeGroundIntegrity(-Damage);
                sFX.PlayOneShot(AtmAudio, 0.6f);
            }
            Destroy(gameObject, 2);
            return;
        }

        if (layer == LayerMask.NameToLayer("Towers"))
        {
            Physics2D.IgnoreCollision(other.collider, gameObject.GetComponent<Collider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Entering Atmosphere
        int layer = collision.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Sky"))
        {

            //Reduce falling speed of meteors upon entering atmosphere
            if (deflected == false)
            {
                r2d.AddForce(Vector2.up * (r2d.mass * 20));
                deflected = true;
            }
            

            //Adds tail to small meteor after entering atmosphere
            if (gameObject.tag == "MeteorSmall")
            {
                sFX.PlayOneShot(AtmAudio, 0.5f);
                animator.SetInteger("State", 2);
                r2d.angularVelocity = 1;
            }
            return;
        }

        //Getting hit by laser beam
        if (collision.gameObject.tag == "LaserBeam")
        {
            //MeteorLead is immune to lasers
            if (gameObject.tag != "MeteorLead")
            {
                //Laser Beam deals 1 dmg singly to non lead meteors
                if (HitPoints <= 1)
                {
                    //Disable Physics for explosion
                    gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                    collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    DestroyMeteor();
                }
                else
                {
                    HitPoints--;
                }
            }
            return;
        }

        if (collision.gameObject.tag == "Rocket")
        {
            //Rocket deals 4 splash dmg for a small aoe
            if (HitPoints <= 4)
            {
                gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                DestroyMeteor();
            }
            else
            {
                HitPoints = HitPoints - 4;
            }
        }

        if (collision.gameObject.tag == "SonicWave")
        {
            if (HitPoints <= 1)
            {
                gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                DestroyMeteor();
            }
            else if (HitPoints <= 3)
            {
                HitPoints--;
            }
        }
    }
    //Methods to destroy meteors and spawn its children
    private void DestroyMeteor()
    {
        switch (gameObject.tag)
        {
            case "MeteorTiny":
                DestroyMeteorTiny();
                break;
            case "MeteorSmall":
                DestroyMeteorSmall();
                break;
            case "MeteorMid":
                DestroyMeteorMid();
                break;
            case "MeteorBig":
                DestroyMeteorBigType();
                break;
            case "MeteorBigger":
                DestroyMeteorBigType();
                break;
            case "MeteorBiggest":
                DestroyMeteorBigType();
                break;
            case "MeteorLead":
                DestroyMeteorBigType();
                break;
        }

    }

    private void DestroyMeteorTiny()
    {
        sFX.PlayOneShot(ExplodeAudio, 0.2f);
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        animator.SetInteger("State", 1);
        GlobalController.ChangeResource(ResourceUnits);
        Destroy(gameObject, 60.0f / 12.0f);
    }

    private void DestroyMeteorSmall()
    {
        sFX.PlayOneShot(ExplodeAudio, 0.3f);
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        smallMeteorTTL = 2f;
        animator.SetInteger("State", 1);
        GameObject child = Instantiate(MeteorChild2Clone, transform.position, (Quaternion.identity));
        child.GetComponent<Rigidbody2D>().velocity = r2d.velocity;
        Destroy(gameObject, 2);
    }

    private void DestroyMeteorMid()
    {
        sFX.PlayOneShot(ExplodeAudio, 0.3f);
        animator.SetInteger("State", 1);
        GameObject child = Instantiate(MeteorChild2Clone, transform.position, (Quaternion.identity));
        child.GetComponent<Rigidbody2D>().velocity = r2d.velocity;
        Destroy(gameObject, 2);
    }

    private void DestroyMeteorBigType()
    {
        sFX.PlayOneShot(ExplodeAudio, 1f);
        animator.SetInteger("State", 1);
        //Breaking into 2 mid meteor
        Destroy(gameObject, 2);
        GameObject child = Instantiate(MeteorChild2Clone, transform.position + Vector3.left/5, (Quaternion.identity));
        GameObject child2 = Instantiate(MeteorChild2Clone, transform.position + Vector3.right/5, (Quaternion.identity));
        child.GetComponent<Rigidbody2D>().velocity = r2d.velocity;
        child2.GetComponent<Rigidbody2D>().velocity = r2d.velocity;
        child.GetComponent<Rigidbody2D>().AddForce(Vector2.left * Random.Range(r2d.mass * r2d.mass, r2d.mass * r2d.mass * 4));
        child2.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Random.Range(r2d.mass * r2d.mass, r2d.mass* r2d.mass * 8));
    }

}
