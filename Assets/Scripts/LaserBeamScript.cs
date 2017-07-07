using UnityEngine;
using System.Collections;

//---------------------------------------------------------------------------------
// Author		: Chen Yu
// Description	: Script for the laser beam prefab
//---------------------------------------------------------------------------------
public class LaserBeamScript : MonoBehaviour 
{
    //===================
    // Public Variables
    //===================
    public float BeamSpeed = 20f;
    public AudioClip hitAudio;
    public AudioClip hitMetalAudio;
    //===================
    // Private Variables
    //===================
    private Animator animator;
    private Rigidbody2D r2d;
    private AudioSource sFX;

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

        r2d.velocity = r2d.velocity * BeamSpeed;

        sFX = GetComponent<AudioSource>();

        //Turn it based on the direction of movement
        Vector2 moveDirection = r2d.velocity;
        float angle = (Mathf.Atan2(moveDirection.y, moveDirection.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
	
	//---------------------------------------------------------------------------------
	// XXX is when blah blah
	//---------------------------------------------------------------------------------
	protected void Update() 
	{
        //Turn it based on the direction of movement very frame
        Vector2 moveDirection = r2d.velocity;
        float angle = (Mathf.Atan2(moveDirection.y, moveDirection.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void FixedUpdate()
    {
        var screenposition = Camera.main.WorldToViewportPoint(transform.position);
        if (screenposition.x < 0 || screenposition.x > 1 || screenposition.y < 0 || screenposition.y > 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        //Check if collided with a meteor
        if (collision.gameObject.layer == 10 && collision.gameObject.GetComponent<Animator>().GetInteger("State") != 1)
        {
            r2d.velocity = r2d.velocity * 0;
            animator.SetInteger("State", 1);
            
            if (collision.gameObject.tag != "MeteorLead")
            {
                sFX.PlayOneShot(hitAudio, 1f);
            }
            //Play metal sound when hit lead meteor
            else if (collision.gameObject.tag == "MeteorLead")
            {
                sFX.PlayOneShot(hitMetalAudio, 1f);
            }
            //Destroy after collision
            Destroy(gameObject, 0.5f);
        }
    }
	
	//---------------------------------------------------------------------------------
	// XXX is when blah blah
	//---------------------------------------------------------------------------------
	protected void OnDestroy()
	{
	}
}
