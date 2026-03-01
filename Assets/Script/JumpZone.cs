using System;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
public class JumpZone : MonoBehaviour
{
    private Collider2D Zone;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private float BounceForce;
    private void Awake() 
    {
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
        
            other.gameObject.GetComponent<PlayerController>().Push(new Vector2(0,BounceForce));            
        }
    }
    // Update is called once per frame

}
