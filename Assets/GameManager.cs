using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  /* 
    [SerializeField] Transform hexPainter;
    
    private void FixedUpdate()
    {
        return;
        RaycastHit raycastHit;
        Vector3 hexPainterPosition = hexPainter.transform.position;
        Physics.Raycast(hexPainterPosition, Vector3.up * -1, out raycastHit,3);
        if (raycastHit.collider != null)
        {
            Debug.Log("raycastHit.collider != null");
            Transform t = raycastHit.collider.transform;
            
            Hex hex = t.parent.GetComponent<Hex>();
            if (hex != null)
            {
                hex.Paint(false);
            }
        }*/

        //int newLineCount = line.positionCount++;
       // line.SetPosition(newLineCount - 1, hexPainterPosition);
    
}
