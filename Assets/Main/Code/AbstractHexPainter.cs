using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbstractHexPainter : MonoBehaviour
{
    private List<Vector3> positions = new List<Vector3>();
    private Vector3 previousPosition;
    [SerializeField] private SphereCollider overlapSphereCollider;

    [SerializeField] private LineRenderer line;
    [SerializeField] private Camera camera;

    [SerializeField] private Transform fingerTransform;
    [SerializeField] private Image fingerImage;
    [SerializeField] private GameObject fingerWhileTouching;
    [SerializeField] private GameObject fingerWhileHovering;


    public static AbstractHexPainter instance;
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

    private Vector3 MouseToGroundPlane(Vector3 mousePosition)
    {
        //TODO: learn from this
        Ray ray = camera.ScreenPointToRay(mousePosition);
        float rayLength = ray.origin.y / ray.direction.y;
        Vector3 result = ray.origin - (ray.direction * rayLength);
       // Debug.Log("MouseToGroundPlane: " + result);
        return result;
       
    }

    private void Start()
    {
        /*Color colour = fingerImage.color;
        colour.a = fingerAlphaWhileHovering;
        fingerImage.color = colour;*/
        fingerWhileHovering.SetActive(true);
        fingerWhileTouching.SetActive(false);

        Cursor.visible = false;
    }

    private void Update()
    {

        if (Input.GetMouseButtonUp(1))
        {

            HexMap.AwaitFillIn(0);

            /*Color colour = fingerImage.color;
            colour.a = fingerAlphaWhileHovering;
            fingerImage.color = colour;*/
            fingerWhileHovering.SetActive(true);
            fingerWhileTouching.SetActive(false);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            /* Color colour = fingerImage.color;
             colour.a = fingerAlphaWhileTouching;
             fingerImage.color = colour;*/
            fingerWhileHovering.SetActive(false);
            fingerWhileTouching.SetActive(true);
        }
        Vector3 currentMouseGroundPosition = MouseToGroundPlane(Input.mousePosition);

        Vector3 fingerPosition = camera.WorldToScreenPoint(currentMouseGroundPosition);
        fingerTransform.position = fingerPosition;
    }


    private Vector3? previousMouseGroundPosition;
    private List<Vector3> groundPositions = new List<Vector3>();
    [SerializeField] private float interpolationDistance = 1.5f;

    private void FixedUpdate()
    {
        //TODO: compare interpolation to cake party's
        Vector3 currentMouseGroundPosition = MouseToGroundPlane(Input.mousePosition);

        /*Vector3 fingerPosition = camera.WorldToScreenPoint(currentMouseGroundPosition);
        fingerTransform.position = fingerPosition;*/

        if (Input.GetMouseButton(1))
        {
            groundPositions.Clear();
            groundPositions.Add(currentMouseGroundPosition);

            if (previousMouseGroundPosition != null)
            {
                Vector3 previousMouseGroundPosition = (Vector3)this.previousMouseGroundPosition;
                float distance = Vector3.Distance(currentMouseGroundPosition, previousMouseGroundPosition);
                if (distance > interpolationDistance)
                {
                    Vector3 direction = (currentMouseGroundPosition - previousMouseGroundPosition).normalized;
                    for (float d = interpolationDistance; d < distance; d += interpolationDistance)
                    {
                        Vector3 interpolatedPoint = previousMouseGroundPosition + (d * direction);
                        groundPositions.Add(interpolatedPoint);
                    }
                }
            }
           

            FloorCheck(groundPositions,0);

            this.previousMouseGroundPosition = currentMouseGroundPosition;

        }
        else
        {
            previousMouseGroundPosition = null;
        }

        //if (lastFloorCheck + floorCheckInterval < Time.time)//TODO: Might be unessssry
        /* {
             FloorCheck();
             lastFloorCheck = Time.time;
         }*/

        //Debug.Log("isOnAPotentialWall:" + isOnAPotentialWall);



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

    public void FloorCheck(List<Vector3> positions,int currentPositionIndex)
    {
        bool changed = false;

        int count = positions.Count;
        Debug.Log("positions to check: " + count);
        overlapSphereCollider.transform.position = positions[count-1];
        for (int j = 0; j< count; j++)
        {
            Collider[] overlappingColliders =
               Physics.OverlapSphere(positions[j], overlapSphereCollider.radius);
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
                            /* case HexStates.AwaitingFill:
                                 isOnAPotentialWall = true; break;*/
                            /*case HexStates.Full:
                                Collide(); break;*/
                            case HexStates.Empty:
                                {
                                    hex.ChangeState(HexStates.PotentiallyFull);
                                    changed = true;
                                }
                                break;

                        }

                    }
                }
            }
        }
       
        if (changed)
        {

            if (HexMap.CalculateFill())
            {
                SoundNames soundName = SoundNames.AllGlocken;// hardCollision ? SoundNames.LowGlocken : SoundNames.AllGlocken;
                SoundManager.PlayOneShotSoundAt(soundName, positions[currentPositionIndex]);
            }
        }
    }

   /* private void OnCollisionEnter(Collision collision)
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


            }
            else
            {
                Enemy enemy = collision.collider.transform.parent.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log("You lost!");
                    gameObject.SetActive(false);
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
}
