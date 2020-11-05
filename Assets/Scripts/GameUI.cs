using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    public GameObject player;
    bool gameIsOver;


    // Start is called before the first frame update
    void Start()
    {
        Guard.onGuardHasSpottedPlayer += ShowGameLoseUI;

        player.GetComponent<Player>().OnReachedEndOfLevel += ShowGameWinUI;

    }

    // Update is called once per frame
    void Update()
    {
      if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void ShowGameWinUI()
    {
        OnGameOver(gameWinUI);
    }
    void ShowGameLoseUI()
    {
        OnGameOver(gameLoseUI);
    }

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameIsOver = true;
        Guard.onGuardHasSpottedPlayer -= ShowGameLoseUI;
        player.GetComponent<Player>().OnReachedEndOfLevel -= ShowGameWinUI;
    }

}
