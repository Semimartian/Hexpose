using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    private bool isDragging;
    private Vector3 lastMousePosition;
    // Start is called before the first frame update
    void Start()
    {

    }

    private bool IsDragButtonDown()
    {
        return (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2));
    }
    private bool IsDragButtonUp()
    {
        return (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(2));
    }

    private void Update()
    {
        Vector3 translate = new Vector3
        (Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.Translate(translate * Time.deltaTime * movementSpeed, Space.World);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayLength = ray.origin.y / ray.direction.y;
        Vector3 hitPosition = ray.origin - (ray.direction * rayLength);

        if (IsDragButtonDown())
        {
            isDragging = true;
            lastMousePosition = hitPosition;
        }
        else if (IsDragButtonUp())
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 difference = lastMousePosition - hitPosition;
            Camera.main.transform.Translate(difference, Space.World);
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);//Why?
            rayLength = ray.origin.y / ray.direction.y;//why?

            lastMousePosition = ray.origin - (ray.direction * rayLength);

        }

        float scrollAmount = Input.GetAxisRaw("Mouse ScrollWheel")*-1;
        if (Mathf.Abs(scrollAmount) > 0.01f)
        {
            
            Vector3 direction = Camera.main.transform.position - hitPosition;
            if(!(scrollAmount < 0 && Camera.main.transform.position.y < 2))
            {
                Camera.main.transform.Translate(direction * scrollAmount, Space.World);

            }
        }
    }
}
