using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool gameisPaused = false;
    public GameObject pauseMenuPanel;
    public DefaultInput input;
    public InputAction menu,weapon;

    private void Awake()
    {
        input = new DefaultInput();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameisPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {   
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        gameisPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void Pause()
    {        
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameisPaused = true;
    }
}
