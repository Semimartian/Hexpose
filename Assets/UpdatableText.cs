using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableText : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private Animator animator;

    public void ChangeText(string value)
    {
        text.text = value;
        animator.SetTrigger("Pop");
    }

}
