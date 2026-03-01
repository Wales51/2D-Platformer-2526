using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Collider2D Zone;
    private Animator Anim;
    //private bool triggered = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Zone = this.gameObject.GetComponent<Collider2D>();
        Anim = this.gameObject.GetComponent<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("CheckPoint");
            other.gameObject.GetComponent<PlayerController>().SpawnLoc(transform.position);
            Zone.enabled = false;
            Anim.SetTrigger("SpawnSet");
        }
    }
    // Update is called once per frame

}
