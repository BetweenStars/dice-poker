using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneManager : MonoBehaviour, ILoadingAsyncInitializable
{
    public static PhoneManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        homeButton.onClick.AddListener(() => OpenHomeScreen());
    }

    public void InitializeOnLoading() { OpenHomeScreen(); }

    public void OnReadyToStart()
    {

    }

    #region screen
    [SerializeField] private HomeScreen homeScreen;
    private BaseScreen currentScreen;
    public void OpenTargetScreen(BaseScreen targetScreen)
    {
        if (currentScreen != null)
        {
            currentScreen.Hide();
        }

        targetScreen.Show();
        currentScreen = targetScreen;
    }
    private void OpenHomeScreen()
    {
        OpenTargetScreen(homeScreen);
    }
    #endregion

    #region navigation-top-bar
    [SerializeField] private Button homeButton;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text batteryText;

    #endregion
}
