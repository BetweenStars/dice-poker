using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using System;
using Unity.VisualScripting;

public enum CameraState
{
    Default,
    Dialogue,
    Betting,
    Table
}

public class StageCameraManager : MonoBehaviour
{
    public static StageCameraManager Instance { get; private set; }

    [SerializeField] private CinemachineBrain brain;

    [SerializeField] private CinemachineCamera defaultCamera;
    [SerializeField] private CinemachineCamera dialogueCamera;
    [SerializeField] private CinemachineCamera bettingCamera;
    [SerializeField] private CinemachineCamera tableCamera;

    [SerializeField] private CameraState currentCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetCamera(CameraState.Default);
    }

    public void SetCamera(CameraState newState, float blendDuration = 1f)
    {
        brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, blendDuration);
        defaultCamera.Priority = 0;
        dialogueCamera.Priority = 0;
        bettingCamera.Priority = 0;
        tableCamera.Priority = 0;

        switch (newState)
        {
            case CameraState.Default:
                defaultCamera.Priority = 10;
                break;
            case CameraState.Dialogue:
                dialogueCamera.Priority = 10;
                break;
            case CameraState.Betting:
                bettingCamera.Priority = 10;
                break;
            case CameraState.Table:
                tableCamera.Priority = 10;
                break;
        }
    }

    public bool IsCameraInState(CameraState state) => currentCamera == state;

    public IEnumerator ChangeCamera(CameraState newState)
    {
        if (currentCamera == newState) yield break;

        currentCamera = newState;

        //Debug.Log($"{brain.ActiveVirtualCamera.Name}");
        SetCamera(newState);

        while (!brain.IsBlending)
        {
            yield return null;
        }

        //Debug.Log($"{brain.ActiveVirtualCamera.Name}");

        while (brain.IsBlending)
        {
            yield return null;
        }
    }
}
