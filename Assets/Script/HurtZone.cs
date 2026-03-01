using System;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
public class HurtZone : MonoBehaviour
{
    private Collider2D Zone;
    public int test = 0;
    private float Direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake() 
    {
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Hurt");
            Direction = other.gameObject.transform.position.x - transform.position.x;
            other.gameObject.GetComponent<PlayerController>().Hurt();
            if (Direction<=0)
            {
                other.gameObject.GetComponent<PlayerController>().Push(new Vector2(-7,5));
            }
            else
            {
                other.gameObject.GetComponent<PlayerController>().Push(new Vector2(7,5));
            }
        }
    }
    // Update is called once per frame

}
