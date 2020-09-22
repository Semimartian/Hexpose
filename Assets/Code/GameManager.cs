using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameStates
{
    Intro,InPlay,GameOver
}


public class GameManager : MonoBehaviour
{
    public static readonly bool ABSTRACT_PLAYER = true;// instance.abstractPlayer;

    private static GameStates gameState;
    public static GameStates GameState
    {
        get { return gameState; }
        set { gameState = value; }
    }

    private void Awake()
    {
        GameState = GameStates.Intro;
    }
}