using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



public class MaterialAnimator : MonoBehaviour
{
    [Serializable]
    private class MaterialAnimationBlock
    {
        public Material mat;
        [HideInInspector] public float currentTime;
        public AnimationCurve curve;
        public Color colour;
        public string colourName = "_EmissionColor";
    }

    [SerializeField] private MaterialAnimationBlock[] materialAnimationBlocks;

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < materialAnimationBlocks.Length; i++)
        {
            //TODO: you can do it with modulu or somethin
            MaterialAnimationBlock block = materialAnimationBlocks[i];
            block.currentTime += deltaTime;
            if(block.currentTime > block.curve.keys[block.curve.length - 1].time)
            {
                block.currentTime = 0;
            }

            Color colour = block.colour;
            Vector4 colourVector = new Vector4
                (block.colour.r, block.colour.g, block.colour.b, 0);
            block.mat.SetColor
                (block.colourName, colourVector * block.curve.Evaluate(block.currentTime));
        }
    }
}
