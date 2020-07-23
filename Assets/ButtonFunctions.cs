﻿using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
    private MenuManager menuManager;

    // Start is called before the first frame update
    void Start()
    {
        menuManager = FindObjectOfType<MenuManager>();
    }

    public void OpenMenu(string menu)
    {
        menuManager.OpenMenu(menu);
    }
}
