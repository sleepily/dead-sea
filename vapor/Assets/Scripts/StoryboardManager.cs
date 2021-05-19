using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StoryboardManager : MonoBehaviour
{
    public GameManager gm;

    public PlayableDirector director;
    public Transform stage;

    public List<Decoration> decorationPrefabs;
    public List<Decoration> decorationOnStage;

    void Start()
    {
        foreach (var deco in decorationOnStage)
            deco.gm = gm;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Beat()
    {
        MoveDecorations();
    }

    private void MoveDecorations()
    {
        foreach (var deco in decorationOnStage)
            deco.Beat();
    }

    public void Play()
    {
        director.Play();
    }

    public void TogglePause()
    {
        director.Pause();
    }

    public void Stop()
    {
        director.Stop();
    }
}
