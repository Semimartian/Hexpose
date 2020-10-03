using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbstractHexPainter : MonoBehaviour
{
    public enum PaintMode
    {
        EnemiesLocked, EnemiesKilled, EnemiesNeutralised
    }
    public static PaintMode paintMode
    {
        private set;
        get;
    }

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

        paintMode = PaintMode.EnemiesLocked;
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
        if (GameManager.GameOver)
        {
            return;
        }
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
           // Debug.Log("Mouse button unpressed");
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
        //Debug.Log("positions to check: " + count);
        overlapSphereCollider.transform.position = positions[count-1];
        for (int j = 0; j< count; j++)
        {
            Vector3 top = positions[j] ;
            Vector3 bottom = positions[j] + new Vector3(0, -3, 0);

            Collider[] overlappingColliders =Physics.OverlapCapsule
                (bottom, top, overlapSphereCollider.radius);

            //Debug.Log("positions[j]: " + positions[j]);
           // Debug.Log("overlapSphereCollider.radius: " + overlapSphereCollider.radius);

            //Debug.Log("overlappingColliders.Length: " + overlappingColliders.Length);
            for (int i = 0; i < overlappingColliders.Length; i++)
            {
                Transform t = overlappingColliders[i].transform;
                //if (t.parent != null)
                {

                    Hex hex = t/*.parent*/.GetComponent<Hex>();
                    if (hex != null)
                    {
                        
                        HexStates hexState = hex.State;
                        if(hexState!= HexStates.Full)//TODO: I dont like the full/enemy deal here
                        {
                            if (hex.Specialty == HexSpecialties.Enemy)
                            {
                                Debug.Log("An enemy is touching a sensitive hexagon...");
                                HexMap.InfectPlayerPath(hex);
                                return;
                            }


                            switch (hexState)
                            {
                                /* case HexStates.AwaitingFill:
                                     isOnAPotentialWall = true; break;*/
                                /*case HexStates.Full:
                                    Collide(); break;*/
                                case HexStates.Empty:
                                    {
                                        hex.ChangeState(HexStates.Path);
                                        changed = true;
                                    }
                                    break;

                            }

                            Vector3 hexPosition = hex.transform.position;
                            hexPosition.y = Hex.HEX_PRESSED_Y;
                            hex.transform.position = hexPosition;
                        }
                       

                    }
                    else
                    {
                        HexBomb bomb = t/*.parent*/.GetComponent<HexBomb>();
                        if (bomb != null)
                        {
                            bomb.Explode();
                        }
                    }
                }
            }
        }
       
        if (changed)
        {
            SoundManager.PlayOneShotSoundAt(SoundNames.HexPressed, positions[currentPositionIndex]);// t.position);

            if (HexMap.CalculateFill())
            {
                SoundManager.PlayOneShotSoundAt(SoundNames.AllGlocken, positions[currentPositionIndex]);
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
