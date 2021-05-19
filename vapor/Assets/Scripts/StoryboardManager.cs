using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StoryboardManager : MonoBehaviour
{
    public GameManager gm;

    public PlayableDirector director;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
