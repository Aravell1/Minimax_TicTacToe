using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    public Button[,] buttons = new Button[3, 3];

    public GameObject menu;
    public Button playButton, quitButton, autoPlayButton;
    public TMP_Text winsCount, lossCount, tiesCount, tieLossText, startText;

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                buttons[i, j] = GameObject.Find("Button(" + i.ToString() + "/" + j.ToString() + ")").GetComponent<Button>();
                //Debug.Log(buttons[i, j]);
            }
        }
    }
    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (buttons[i, j])
                {
                    int x = i;
                    int y = j;
                    buttons[i, j].onClick.AddListener(delegate { PlayTurn(x, y); });
                }
            }
        }

        if (playButton)
        {
            playButton.onClick.AddListener(delegate { PlayGame(false); });
        }
        if (autoPlayButton)
        {
            autoPlayButton.onClick.AddListener(delegate { PlayGame(true); });
        }
        if (quitButton)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    void PlayTurn(int i, int j)
    {
        //Debug.Log(i + ", " + j);
        GameManager.Instance.PlayTurn(i, j);
    }

    void PlayGame(bool autoPlay)
    {
        menu.SetActive(false);
        GameManager.Instance.RandomStart(autoPlay);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    
}
