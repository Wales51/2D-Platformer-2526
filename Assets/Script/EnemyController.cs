using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using System;
using UnityEngine.InputSystem.Processors;
public class EnemyController : MonoBehaviour
{
    public int Direction;

    public float Speed;

    private Collider2D FloorChecker;

    private Collider2D WallChecker;

    private Rigidbody2D rb;

    private Animator AC;

    private ContactFilter2D Overfilter = new ContactFilter2D().NoFilter();

    private List<Collider2D> results = new List<Collider2D>();

    private Transform OverlapParent;

    public float MaxDistance;

    private float StartX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        AC = GetComponent<Animator>();
        StartX = transform.position.x;
        OverlapParent = transform.Find("OverlapParent");
        FloorChecker = OverlapParent.transform.Find("FloorChecker").gameObject.GetComponent<Collider2D>();
        WallChecker = OverlapParent.transform.Find("WallChecker").gameObject.GetComponent<Collider2D>();
        if (Direction == -1)
        {
        OverlapParent.localScale = Vector3.Scale(OverlapParent.localScale,new Vector3(-1,1,1));
        }
    }

    void Update()
    {
        AC.SetFloat("VelocityX",rb.linearVelocityX);
        AC.SetInteger("Direction",Direction);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Speed!=0)
        {
            TurnChecks();
            rb.linearVelocityX = Direction*Speed;
        }
    }
    void TurnChecks()
    {
        Boolean flip = false;
        //overlaps = GroundPoint.Overlap(new List<Collider2D> { });
        FloorChecker.Overlap(Overfilter, results);
        Boolean FloorFlag = false;
        foreach (Collider2D lap in results)
        {
            if (lap.tag == "Ground")
            {
                FloorFlag = true;
            }
        }
        if (FloorFlag == false)
        {
            flip = true;
        }
        WallChecker.Overlap(Overfilter,results);
        foreach (Collider2D lap in results)
        {
            if (lap.tag == "Ground")
            {
                flip = true;
            }
        }
        if (transform.position.x >= (StartX+MaxDistance) | transform.position.x <= (StartX-MaxDistance))
        {
            flip=true;
        }
        if (flip == true)
        {
            OverlapParent.localScale = Vector3.Scale(OverlapParent.localScale,new Vector3(-1,1,1));
            Direction = Direction * -1;
        }
    }
}
