using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class GameManagement : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject SpawnPoint;
    public GameObject CameraController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Spawn()
    {
        GameObject Player = Instantiate(PlayerPrefab, SpawnPoint.transform.position, Quaternion.identity);
        CinemachineCamera Follower = CameraController.GetComponent<CinemachineCamera>();
        Follower.Follow = Player.transform;
    }

    public void Death(GameObject Player)
    {
        Destroy(Player);
        Spawn();

    }
    
    public void CheckP(Vector3 Location)
    {
        SpawnPoint.transform.position = Location;
    }
/*
    public Vector3 getCharacterStartPosition() 
    {
        return SpawnPoint.transform.position;
    }
*/
    public void EndGame()
    {
        Application.Quit();
    }
}
