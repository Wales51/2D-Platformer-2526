using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private Collider2D Zone;
    public int test = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake() 
    {
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Hurt");
            other.gameObject.GetComponent<PlayerController>().DeathZoneKill();
        }
    }
    // Update is called once per frame

}
