using System;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private float playTimer = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //플레이타임 세는 부분
        if (SceneManager.GetActiveScene().name != "Title Scene") { playTimer += Time.deltaTime; }

        if (playTimer >= 1f)
        {
            playTimer -= 1f;
            DataManager.Instance.Play.playTime++;
        }
    }

    #region Scene
    public string CurrentScene;
    public string NextScene;
    public void ChangeScene(string sceneName)
    {
        NextScene = sceneName;

        SceneManager.LoadScene("Loading Scene", LoadSceneMode.Additive);
    }
    #endregion
}
