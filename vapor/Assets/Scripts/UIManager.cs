using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameManager gm;

    public Slider inputSlider, metronomeSlider, inputOffsetSlider;
    public Button togglePlay;
    public Text textScore, textHealth;

    int lastScore, newScore = 0;
    float scoreLerpSpeed = 10f;

    public CanvasGroup fadeGroup;
    public bool isHidden = false;

    private void Start()
    {
        inputSlider.minValue = -1f;
        inputSlider.maxValue =  1;
        metronomeSlider.minValue = -1f;
        metronomeSlider.maxValue =  1f;

        inputOffsetSlider.value = gm.inputOffset;
    }

    public void UpdatePlayerHealth()
    {
        textHealth.text = $"{gm.player.hp.ToString()} Health";
    }

    public void LerpScore()
    {
        newScore = gm.player.score;

        lastScore = (int)Mathf.Ceil(Mathf.Lerp(lastScore, newScore, 1000 * Time.deltaTime / scoreLerpSpeed));

        // textScore.text = lastScore.ToString();
        textScore.text = $"{lastScore.ToString()} Score";
    }

    public void SetInputSlider(Enemy e)
    {
        float off = e.time - gm.audioManager.position;
        float mapped = Tools.Remap(gm.audioManager.position, e.time - gm.audioManager.beatLength, e.time + gm.audioManager.beatLength, -1f, 1f);
        inputSlider.value = mapped;
        // Debug.Log($"Offset from enemy: {off} ({mapped})");
    }

    private void Update()
    {
        LerpScore();

        if (Input.GetKeyDown(KeyCode.H))
        {
            isHidden = !isHidden;
            fadeGroup.alpha = isHidden ? 0f : 1f;
        }

        if (!isHidden)
            SetMetronomeSlider();
    }

    public void SetMetronomeSlider()
    {
        metronomeSlider.value = gm.audioManager.GetNearestBeatOffsetRelative();
    }
}
