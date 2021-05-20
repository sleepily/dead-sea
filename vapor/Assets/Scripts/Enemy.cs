using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameManager gm;

    public AudioClip soundMove, soundAttack, soundDie;
    public AudioSource source;

    public List<GameObject> damageSprites;

    public Player.Lane lane;

    public int beatCountdown = 4;
    bool damaged = false;

    public long time = -1;

    private void Start()
    {
        foreach (var sprite in damageSprites)
            sprite.SetActive(false);
    }

    public void Setup()
    {
        Vector3 pos = transform.position;
        pos.x = ((int)lane - 1) * gm.laneX;
        transform.position = pos;

        // time = (pos.z - gm.playerZ) / gm.stepZ;
        time = (long)(gm.audioManager.position + ((beatCountdown - 1) * gm.audioManager.beatLength));
    }

    public void Beat()
    {
        Move();
        Attack();
        Sound();
    }

    void Move()
    {
        if (damaged)
            return;

        Vector3 pos = transform.position;
        pos.z -= 2;
        transform.position = pos;

        beatCountdown--;

        source.clip = soundMove;

        if (gm.audioManager.position > time + gm.audioManager.beatLength / 2 || pos.z < -5.5f)
            gm.enemyManager.FlagEnemy(this);
    }

    void Sound()
    {
        source.pitch = Tools.Remap((int)lane, 0, 2, .666f, 1.333f);
        source.panStereo = Tools.Remap((int)lane, 0, 2, .2f, .8f);
        // Debug.Log($"Pitch: {source.pitch}");

        source.Play();
    }

    public virtual void Attack()
    {
        if (damaged)
            return;

        if (beatCountdown != 0)
            return;

        if (lane != gm.player.currentLane)
            return;

        source.clip = soundAttack;
        // Debug.Log("Attack Player");
        gm.player.TakeDamage();
    }

    public void TakeDamage(Player.PlayerAttackType attackType)
    {
        damaged = true;
        damageSprites[(int)attackType].SetActive(true);
        gm.enemyManager.FlagEnemy(this);
        source.clip = soundDie;
        source.PlayDelayed(.06f);
        gm.player.ModifyScore(30);
    }
}
