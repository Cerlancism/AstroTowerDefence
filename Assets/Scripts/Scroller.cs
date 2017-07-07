//#define LOG_TRACE_INFO
//#define LOG_EXTRA_INFO

using UnityEngine;
using System.Collections;

//---------------------------------------------------------------------------------
// Author		: XXX
// Date  		: 2015-05-12
// Modified By	: YYY
// Modified Date: 2015-05-12
// Description	: This is where you write a summary of what the role of this file.
//---------------------------------------------------------------------------------
public class Scroller : MonoBehaviour 
{
    //===================
    // Public Variables
    //===================


    //===================
    // Private Variables
    //===================
    bool repeated = false;
    private Vector2 skySize;

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
        skySize = GetMaxBounds(gameObject).size;
    }
	
	//---------------------------------------------------------------------------------
	// XXX is when blah blah
	//---------------------------------------------------------------------------------
	protected void Update() 
	{
        //Scrolling skybackground with translation and cloning
        transform.Translate(Vector2.left / 100 * Time.deltaTime * 60);

        if (transform.position.x < -10.01 && repeated == false)
        {
            repeated = true;
            GameObject replaced = Instantiate(gameObject, transform.position + (transform.right * skySize.x), transform.rotation);
            //prevent gameobject name becoming ever (clone)(clone)(clone).... since its cloning itself
            replaced.name = "SkyBackground(Replaced)";
        }
        if (transform.position.x < -35.37)
        {
            Destroy(gameObject);
        }
	}

	//---------------------------------------------------------------------------------
	// FixedUpdate for Physics update
	//---------------------------------------------------------------------------------
	protected void FixedUpdate() 
	{
	}
	
	//---------------------------------------------------------------------------------
	// XXX is when blah blah
	//---------------------------------------------------------------------------------
	protected void OnDestroy()
	{
	}

    private Bounds GetMaxBounds(GameObject g)
    {
        var b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }
}
