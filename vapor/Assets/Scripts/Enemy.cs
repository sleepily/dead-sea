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

    private void Start()
    {
        foreach (var sprite in damageSprites)
            sprite.SetActive(false);
    }

    public void Setup()
    {
        Vector3 pos = transform.position;
        pos.x = ((int)lane - 1) * 3;
        transform.position = pos;
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

        if (pos.z < -4)
            gm.enemyManager.FlagEnemy(this);
    }

    void Sound()
    {
        source.pitch = 1 - ((int)lane + 1) / 3;
        // Debug.Log($"Pitch: {source.pitch}");

        source.Play();
    }

    public virtual void Attack()
    {
        if (damaged)
            return;

        if (beatCountdown > 0)
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
        gm.player.ModifyScore(30);
    }
}
