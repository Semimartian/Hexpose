using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates
{
    Intro,InPlay, BadGameOver,GoodGameOver
}

public class GameManager : MonoBehaviour
{
    //[SerializeField] private bool abstractPlayer;
    private static GameManager instance ;

    public static readonly bool ABSTRACT_PLAYER = true;//  instance.abstractPlayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Tried to instantiate more than one map!");
            return;
        }

        GameState = GameStates.Intro;
    }

    private static GameStates DT_gameState;
    public static GameStates GameState
    {
        get { return DT_gameState; }
        set 
        {
            DT_gameState = value;
            DoOnGameStateChange(); 
        }
    }

    public static bool GameOver
    {
        get 
        { 
            return (GameState == GameStates.BadGameOver || GameState == GameStates.GoodGameOver); 
        }

    }

    private static void DoOnGameStateChange()
    {
        switch (GameState)
        {
            case GameStates.GoodGameOver:
                {
                    WinScene.PlayScene();
                    HexMap.PlayWinScene();
                }
                break;

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl))
        {
            var scene = SceneManager.GetActiveScene();
            
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}