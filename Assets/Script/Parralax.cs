using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Parralax : MonoBehaviour
{
    private GameManagement GameManager;
    private Transform CameraT;
    private Vector2 SelfOffset;
    public Vector2 ParralaxSpeed;

    private Vector3 ParralaxOffset;

    public Vector3 CharacterStartPosition;
    private Vector3 StartPos;
    private Vector3 OldCamPos;
    private Vector3 NewCamPos;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent <GameManagement>();
    //    GameManager = GameManager.GetComponent <GameManagement>();
        StartPos = this.gameObject.transform.position;
        CameraT = GameObject.Find("Main Camera").transform;
        CharacterStartPosition = GameManager.SpawnPoint.transform.position;
        CharacterStartPosition.z=-10;
    }

    private void LateUpdate() 
    {
        NewCamPos = CameraT.position;
        this.gameObject.transform.position = StartPos + Vector3.Scale((NewCamPos-CharacterStartPosition), ParralaxSpeed);
        
    }
}
