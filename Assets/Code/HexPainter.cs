﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HexPainter : MonoBehaviour
{
    private List<Vector3> positions = new List<Vector3>();
    private Vector3 previousPosition;
    private Transform myTransform;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private GameObject UIObject;


    public static HexPainter instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Tried to instantiate more than one Bee!");
            return;
        }

    }

    void Start()
    {
         myTransform = transform;
         previousPosition = transform.position;
    }

    [SerializeField] private Rigidbody rigidbody;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            rigidbody.velocity *= 2;
        }

       if (Input.GetMouseButtonUp(0))
        {
            pushRequest = true;
        }
    }

    [SerializeField] private Transform motionGiver;
    [SerializeField] private SphereCollider overlapSphereCollider;
    private float floorCheckInterval = 0.08f;
    private float lastFloorCheck;
    private void FixedUpdate()
    {
        
        if (rigidbody.velocity.y > 0)
        {
            rigidbody.velocity = rigidbody.velocity - (Vector3.up * rigidbody.velocity.y);
        }
        rigidbody.velocity = rigidbody.velocity.normalized * pushForce;

        //myTransform.position = new Vector3(myTransform.position.x, originalY, myTransform.position.z);

        Vector3 currentPosition = myTransform.position;

        FloorCheck();

        //if (lastFloorCheck + floorCheckInterval < Time.time)//TODO: Might be unessssry
       /* {
            FloorCheck();
            lastFloorCheck = Time.time;
        }*/

        //Debug.Log("isOnAPotentialWall:" + isOnAPotentialWall);
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = -(motionGiver.position - myTransform.position).normalized;
            direction.y = 0;
            myTransform.LookAt(currentPosition + direction);
        }

        float distanceFromPreviousPosition = Vector3.Distance(currentPosition, previousPosition);
        if (distanceFromPreviousPosition > 0.25f)
        {
            positions.Add(currentPosition);
            line.positionCount = positions.Count;
            line.SetPositions(positions.ToArray());

            myTransform.LookAt(currentPosition + rigidbody.velocity.normalized);
            previousPosition = currentPosition;
        }

        ManageMotionGiver();
        /*Collider[] colliders = Physics.OverlapSphere(currentPosition, 1);
        for (int i = 0; i < colliders.Length; i++)
        {
            Hex hex =  colliders[i].GetComponent<Hex>();
            if (hex != null)
            {
                Debug.Log("hex found");
                if (hex.Painted)
                {
                    Collide();
                }
            }
        }*/

    }

    private bool pushRequest = false;

    [SerializeField] private float pushForce =55f;

    private void ManageMotionGiver()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.z += Vector3.Distance(Camera.main.transform.position, motionGiver.position);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
       /* Debug.Log("mousePosition" + Input.mousePosition.ToString());

        Debug.Log("mouseWorldPosition" + mouseWorldPosition.ToString());*/

        // mouseWorldPosition.z = 
        motionGiver.position = new Vector3
            (mouseWorldPosition.x, myTransform.position.y, mouseWorldPosition.z);

        if (pushRequest)
        {
            pushRequest = false;
            Vector3 direction = -(motionGiver.position - myTransform.position).normalized;
            direction.y = 0;
            Vector3 force = direction * pushForce;
           // transform.rotation = Quaternion.Euler(Vector3.zero);
            //rigidbody.rotation = Quaternion.Euler(Vector3.zero);
            rigidbody.AddForce(force,ForceMode.Impulse);
            UIObject.SetActive(false);
            SoundManager.PlayOneShotSoundAt(SoundNames.BallSent, myTransform.position);
        }
    }

    public void FloorCheck()
    {
        isOnAPotentialWall = false;
        Vector3 overlapSpherePosition = myTransform.position;
        overlapSpherePosition.y = Hex.HEX_LOW_Y;
        Collider[] overlappingColliders =
            Physics.OverlapSphere(overlapSpherePosition, overlapSphereCollider.radius );
        for (int i = 0; i < overlappingColliders.Length; i++)
        {
            Transform t = overlappingColliders[i].transform;
            if (t.parent != null)
            {

                Hex hex = t.parent.GetComponent<Hex>();
                if (hex != null)
                {

                    HexStates hexState = hex.State;
                    switch (hexState)
                    {
                        case HexStates.AwaitingFill:
                            isOnAPotentialWall = true; break;
                        /*case HexStates.Full:
                            Collide(); break;*/
                        case HexStates.Empty:
                            hex.ChangeState(HexStates.PotentiallyFull); break;
                       /*case HexStates.Hard:
                            hex.ChangeState(HexStates.Empty); break;*/

                    }

                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //return;
        if (collision.collider != null && collision.collider.transform.parent!=null)
        {

            Hex hex = collision.collider.transform.parent.GetComponent<Hex>();
            if (hex != null)
            {
                bool playBounceAnimation = false;
                bool isHard = hex.IsHard;
                if (hex.State == HexStates.Full || isHard)
                {
                    if (isHard)
                    {
                        hex.Soften();
                    }
                    Collide();
                    playBounceAnimation = true;
                }

                if (playBounceAnimation)
                {
                    DoCollisionViewStuff(isHard);
                }

            }
        }       
    }

    private int frameCount;
    [SerializeField] AudioSource audioSource;
    private void DoCollisionViewStuff(bool hardCollision)
    {
        int currentFrameCount = Time.frameCount;
        if (currentFrameCount == frameCount)
        {
            return;
        }
        frameCount = currentFrameCount;
        animator.SetTrigger("Bounce");
        collisionParticles.Play();
        //SoundManager.PlayOneShotSoundAt(SoundNames.WallHit, audioSource);
        SoundNames soundName = hardCollision ? SoundNames.LowGlocken : SoundNames.AllGlocken;
        SoundManager.PlayOneShotSoundAt(soundName, transform.position);

    }

    private void Collide()
    {
        positions.Clear();
        line.positionCount = 0;
        HexMap.CalculateFill();
    }

    public static bool isOnAPotentialWall = false;


    /*private void OnCollisionExit(Collision collision)
{
    if (collision.collider != null && collision.collider.transform.parent != null)
    {

        Hex hex = collision.collider.transform.parent.GetComponent<Hex>();
        if (hex != null)
        {
            HexStates hexState = hex.State;
            if (hexState == HexStates.PotentiallyFull)
            {
                isOnAPotentialWall = false;
            }

        }
    }
}*/
    /* private void OnCollisionStay(Collision collision)
     {
         if (collision.collider != null && collision.collider.transform.parent != null)
         {

             Hex hex = collision.collider.transform.parent.GetComponent<Hex>();
             if (hex != null)
             {
                 if(hex.State == HexStates.PotentiallyFull)
                 {
                     isOnAPotentialWall = true;
                 }
             }

     }*/
}
