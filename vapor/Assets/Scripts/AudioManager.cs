using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameManager gm;

    [Header("References")]

    public AudioClip clip;
    public AudioSource source;

    [Header("Preferences")]

    [SerializeField] long offset = 0;
    float stretchedOffset = 0f;
    public float bpm = 140f;
    [Range(.1f, 1.5f)] public float songSpeed = 1f;

    float positionWithoutOffset, nextEnemyBeat, nextPlayerBeat = 0f;
    [HideInInspector] public float position = 0f;
    float beatLength = 0f;
    float songLength = 0f;
    bool loopFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        beatLength = (60000f / bpm);
    }

    public void PlaySong()
    {
        if (!source)
        {
            Debug.Log("No Source");
            return;
        }

        if (source.isPlaying)
        {
            Debug.Log("Is Playing already");
            return;
        }

        if (source.clip.loadState != AudioDataLoadState.Loaded)
        {
            // LoadSongAudio();
            return;
        }

        ApplySongSpeed();

        source.loop = gm.endlessMode;

        source.Play();
        nextPlayerBeat = -50;
        nextEnemyBeat = 0;
    }

    void ApplySongSpeed()
    {
        source.pitch = songSpeed;

        stretchedOffset = (long)(offset / songSpeed);
        // songLength /= songSpeed;
        // beatLength /= songSpeed;

        Debug.Log($"Pitch: {songSpeed}\tSong Length: {songLength}\tOffset: {stretchedOffset}\tEffective Beat Length: {beatLength / songSpeed}");
    }

    public void TogglePause()
    {
        if (!source)
        {
            Debug.Log("No Source");
            return;
        }

        if (gm.currentGameState == GameManager.GameState.Paused)
        {
            gm.SetState(GameManager.GameState.Playing);
            source.UnPause();
        }

        if (gm.currentGameState == GameManager.GameState.Playing)
        {
            gm.SetState(GameManager.GameState.Paused);
            source.Pause();
        }
    }

    public void Stop()
    {
        if (!source)
        {
            Debug.Log("No Source");
            return;
        }

        if (!source.isPlaying)
            return;

        source.Stop();
    }

    public bool LoadSongAudio()
    {
        source.clip = clip;

        if (!source.clip.LoadAudioData())
            return false;

        songLength = source.clip.length * 1000;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTime();
    }

    void CheckTime()
    {
        if (!source.isPlaying)
            return;

        positionWithoutOffset = (source.time * 1000);

        position = positionWithoutOffset + stretchedOffset;

        Debug.Log($"Time: {position}");

        if (loopFlag)
        {
            if (position > songLength / 2)
                return;

            loopFlag = false;
        }

        if (position >= nextPlayerBeat)
        {
            nextPlayerBeat += beatLength;
            gm.player.Beat();
            gm.enemyManager.ClearFlaggedEnemies();
            // Debug.Log("Player Beat");
        }

        if (position >= nextEnemyBeat)
        {
            nextEnemyBeat += beatLength;
            gm.enemyManager.Beat();
            // Debug.Log("Enemy Beat");

            if (nextEnemyBeat >= songLength - stretchedOffset)
            {
                nextEnemyBeat -= songLength;
                nextPlayerBeat += beatLength;
                nextPlayerBeat -= songLength;
                loopFlag = true;
                // Debug.Log($"Time: {source.time}. Resetting: Next Enemy Beat {(int)nextEnemyBeat}, Player Beat {(int)nextPlayerBeat}");
            }
        }

        if (!gm.endlessMode)
            if (position >= songLength)
            {
                gm.SetState(GameManager.GameState.Win);
                Debug.Log("You Win!");
            }
    }
}
