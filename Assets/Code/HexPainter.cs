using System;
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
    private float originalY;
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
        originalY = myTransform.position.y;
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

    [SerializeField] Transform motionGiver;
    [SerializeField] SphereCollider collider;
    private float floorCheckInterval = 0.12f;
    private float lastFloorCheck;
    private void FixedUpdate()
    {
        
        if (rigidbody.velocity.y > 0)
        {
            rigidbody.velocity = rigidbody.velocity - (Vector3.up * rigidbody.velocity.y);
        }
        rigidbody.velocity = rigidbody.velocity.normalized * pushForce;

        // myTransform.rotation
        //myTransform.position = new Vector3(myTransform.position.x, originalY, myTransform.position.z);

        Vector3 currentPosition = myTransform.position;

        if (lastFloorCheck + floorCheckInterval < Time.time)//TODO: Might be unessssry
        {
            FloorCheck();
            lastFloorCheck = Time.time;
        }


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
            //myTransform.rotation = Vector3.ro(previousPosition, currentPosition);
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
        }
    }
    public void FloorCheck()
    {
        isOnAPotentialWall = false;
        Collider[] overlappingColliders =
            Physics.OverlapSphere(myTransform.position, collider.radius * 2f);
        for (int i = 0; i < overlappingColliders.Length; i++)
        {
            Transform t = overlappingColliders[i].transform;
            if (t.parent != null)
            {

                Hex hex = t.parent.GetComponent<Hex>();
                if (hex != null &&
                    (hex.State == HexStates.AwaitingFill))//|| hex.State == HexStates.PotentiallyFull))
                {
                    isOnAPotentialWall = true;
                    break;
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
                HexStates hexState = hex.State;
                if (hexState == HexStates.Full)
                {
                    Collide();
                }
                else if (hexState == HexStates.Empty)
                {
                    hex.ChangeState(HexStates.PotentiallyFull);
                }
                else if (hexState == HexStates.Hard)
                {
                    hex.ChangeState(HexStates.Empty);
                }
            }
        }       
    }

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
    private void Collide()
    {
        positions.Clear();
        line.positionCount = 0;
        HexMap.CalculateFill();
    }

    public static bool isOnAPotentialWall = false;

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
