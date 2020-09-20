using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
//using System;
//using System.Linq;

public struct HexCoordinates
{
    public short q;
    public short r;
    public HexCoordinates(short q, short r)
    {
        this.q = q;
        this.r = r;
    }
}

public enum HexTypes : byte
{
    Floor = 0, Hexposed=1, Hard=2,
}

public enum HexStates:byte
{
    Empty=0,Full,PotentiallyFull, AwaitingFill//, Hard
}

public class Hex : MonoBehaviour
{
    private HexComponent component;
    public sbyte fillMark;
    private HexStates state;
    public HexStates State
    {
        get { return state; }
    }

    private bool isHard = false;
    public bool IsHard
    {
        get { return isHard; }
    }

    private Hex[] neighbours;

    public  byte Q;
    public  byte R;

    public int S()
    {
        return -(Q + R);
    }

    #region Position:
    static readonly float HEX_WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
    static readonly float HEX_RADIUS = 1f;
    static readonly float HEX_HEIGHT = HEX_RADIUS * 2;
    static readonly float HEX_WIDTH = HEX_HEIGHT * HEX_WIDTH_MULTIPLIER;
    static readonly float HEX_HORIZONTAL_SPACING = HEX_WIDTH;
    static readonly float HEX_VERTICAL_SPACING = HEX_HEIGHT * 0.75f;
    static readonly float HEX_SIZE_MULTIPLIER = 0.62f;
    public static readonly float HEX_LOW_Y = 0;
    private const float HEX_HIGH_Y = 2.6f;
    private const float RISE_PER_SECOND = 8f;
    private const float FALL_PER_SECOND = 10f;

    #endregion
    // [SerializeField]int s;
    public void Construct(int q, int r, HexComponent hexComponent,ushort materialIndex )
    {
        Q = (byte)q;
        R = (byte)r;
        component = hexComponent;
        component.meshRenderer.material = HexMap.instance.emptyHexMat;
        transform.localScale = HEX_SIZE_MULTIPLIER * Vector3.one;
        this.materialIndex = materialIndex;
        //s = S();
        //  SetHeight(height);
        // compressedProperties = (byte)(hasResources ? (compressedProperties | HAS_RESOURCES) : (compressedProperties & ~HAS_RESOURCES));
        // compressedProperties = (byte)(isWater ? (compressedProperties | IS_WATER) : (compressedProperties & ~IS_WATER));
        //S = -(q + r);
    }

    /* public static short GetNewSignature()
     {
         return (short)UnityEngine.Random.Range(DEFAULT_SIGNATURE+1, short.MaxValue);
     }*/

    public override string ToString()
    {
        return "Q " + Q + "R" + R;
    }

    public Vector3 PositionInWorld()
    {
        return  PositionInWorld(Q, R);
    }

    public static Vector3 PositionInWorld(int q, int r)
    {
        return new Vector3
            (HEX_HORIZONTAL_SPACING * (q + (r / 2f))
            , HEX_LOW_Y,
            HEX_VERTICAL_SPACING * r)*HEX_SIZE_MULTIPLIER;
    }

    public static HexCoordinates GetHexCoordinates(Vector3 position)
    {
        short r = (short)Mathf.RoundToInt(/*(float)Mathf.CeilToInt*/(position.z) / HEX_VERTICAL_SPACING);
        short q = (short)Mathf.RoundToInt((/*(float)Mathf.CeilToInt*/(position.x) / HEX_HORIZONTAL_SPACING) - (r / 2f));

        return new HexCoordinates(q, r);
    }

    public static int Distance(Hex a, Hex b)
    {
        return Mathf.Max
            (Mathf.Abs(a.Q - b.Q), Mathf.Abs(a.R - b.R), Mathf.Abs(a.S() - b.S()));
    }
    public Hex[] GetNeighbours()//GetAdjacentHexes()
    {
        if(this.neighbours == null)
        {
            List<Hex> neighbours = new List<Hex>();

            neighbours.Add(HexMap.GetHex(Q + 1, R));
            neighbours.Add(HexMap.GetHex(Q - 1, R));
            neighbours.Add(HexMap.GetHex(Q, R + 1));
            neighbours.Add(HexMap.GetHex(Q, R - 1));
            neighbours.Add(HexMap.GetHex(Q + 1, R - 1));
            neighbours.Add(HexMap.GetHex(Q - 1, R + 1));

            List<Hex> realNeighbours = new List<Hex>();
            foreach (Hex hex in neighbours)
            {
                if(hex != null)
                {
                    realNeighbours.Add(hex);
                }
            }

            this.neighbours = realNeighbours.ToArray();
        }
        return this.neighbours;
    }

    /*public void PaintIn(Color32 colour)
    {
        component.meshRenderer.material.color = colour;
    }*/
    private ushort materialIndex;
    private void PaintIn(Material mat)
    {
        component.meshRenderer.material = mat;
    }

    private void MarkAsPotentiallyFull()
    {
       /* if (state == HexStates.Full)
        {
            Debug.LogError("Already painted!");
            return;
        }*/
        component.meshRenderer.material = HexMap.instance.highLightedHexMat;
    }

    private IEnumerator MarkAsAwiatingFill()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, 0.5f));
        if(state == HexStates.AwaitingFill)
        {
           // if(UnityEngine.Random.Range(0, 6) == 0)
            {
               // SoundManager.PlayOneShotSoundAt(SoundNames.Pop, transform.position);

            }
            component.meshRenderer.material = HexMap.instance.awaitingFillHexMat;

        }
    }

    private void Fill()
    {
        StartCoroutine(Rise());
        component.meshRenderer.material = HexMap.instance.GetHexColouredMaterial(materialIndex);
    }

    public void Harden()
    {
        isHard = true;
        StartCoroutine(Rise());
        component.meshRenderer.material = HexMap.instance.hardHexMat;
    }
    public void Soften()
    {
        isHard = false;
        StartCoroutine(Fall());
    }

    private void MarkAsEmpty()
    {
        StartCoroutine(Fall());
        component.meshRenderer.material = HexMap.instance.emptyHexMat;
    }

    public void ChangeState(HexStates state)
    {
        if(state == this.state)
        {
            Debug.LogError("This is already my state!");
            return;
        }
        this.state = state;
        switch (state)
        {
            case HexStates.PotentiallyFull:
                MarkAsPotentiallyFull();
                break;
            case HexStates.AwaitingFill:
               StartCoroutine( MarkAsAwiatingFill());
                break;
            case HexStates.Full:
                Fill();
                break;
           /* case HexStates.Hard:
                Harden();
                break;*/
            case HexStates.Empty:
                MarkAsEmpty();
                break;
        }
    }
    private IEnumerator Rise()
    {
        float speed = RISE_PER_SECOND + UnityEngine.Random.Range(-3f, 3f);
        //yield return new WaitForSeconds(UnityEngine.Random.Range(0, 0.18f));
        //if (false)
        {
            Transform myTransform = transform;
            while (myTransform.position.y < HEX_HIGH_Y)
            {
                myTransform.position += Vector3.up * speed * Time.deltaTime;
                yield return null;
            }

            myTransform.position = PositionInWorld() + (HEX_HIGH_Y * Vector3.up);
        }
    }

    private IEnumerator Fall()
    {
        float speed = FALL_PER_SECOND;// + UnityEngine.Random.Range(-3f, 3f);
        //yield return new WaitForSeconds(UnityEngine.Random.Range(0, 0.18f));
        //if (false)
        {
            Transform myTransform = transform;
            while (myTransform.position.y > HEX_LOW_Y)
            {
                myTransform.position -= (Vector3.up * speed * Time.deltaTime);
                yield return null;
            }

            myTransform.position = PositionInWorld(); 
        }
    }
}

