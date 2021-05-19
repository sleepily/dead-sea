using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioManager audioManager;
    public EnemyManager enemyManager;
    public StoryboardManager storyboardManager;
    public UIManager ui;

    public Player player;

    public bool endlessMode = false;
    public float playerZ = -5f;
    public float stepZ = -2f;
    public float laneX = 3f;
    [Range(-100f, 100f)] public float inputOffset = 0f;
    [Range(17f, 80f)] public float judgementTime = 50f;

    public enum GameState { Menu, Playing, Paused, Win, GameOver }

    public GameState currentGameState { get; private set; } = GameState.Menu; // 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckStartStop();
        CheckPause();
    }

    void CheckStartStop()
    {
        if (currentGameState == GameState.Playing ||
            currentGameState == GameState.Paused)
            return;

        if (!Input.GetKeyDown(KeyCode.Return))
            return;

        if (!audioManager.LoadSongAudio())
            return;

        currentGameState = GameState.Playing;
        storyboardManager.Play();
        audioManager.PlaySong();
    }

    void CheckPause()
    {
        if (currentGameState == GameState.Menu ||
            currentGameState == GameState.Win ||
            currentGameState == GameState.GameOver)
            return;

        if (Input.GetKeyDown(KeyCode.P) && audioManager.position > float.Epsilon)
            if (audioManager.source.isPlaying)
            {
                storyboardManager.TogglePause();
                audioManager.TogglePause();
            }
    }

    public void SetState(GameManager.GameState state)
    {
        currentGameState = state;

        switch (state)
        {
            case GameState.Menu:
                audioManager.Stop();
                storyboardManager.Stop();
                break;
            case GameState.Playing:
                break;
            case GameState.Paused:
                break;
            case GameState.Win:
                break;
            case GameState.GameOver:
                audioManager.Stop();
                storyboardManager.Stop();
                break;
            default:
                break;
        }
    }
}
