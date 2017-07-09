using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicWaveScript : MonoBehaviour {

    public float Speed;
    public float GrowX;
    public float GrowY;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(0, Speed, 0);
        transform.localScale = new Vector2(transform.localScale.x + GrowX, transform.localScale.y + GrowY);
        var spriteColor = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, spriteColor.a - 0.005f);

        if (Camera.main.WorldToViewportPoint(transform.position).y > 1.5f)
        {
            Destroy(gameObject);
        }
    }
}
