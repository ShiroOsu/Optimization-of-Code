using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HighscoreButtonListener : MonoBehaviour
{
    [SerializeField]
    private UIMenuManager menuManager = null;

    private void Awake()
    {
        Assert.IsNotNull(menuManager, "Menumanager not found");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            menuManager.ScoresReturn();
        }
    }
}
