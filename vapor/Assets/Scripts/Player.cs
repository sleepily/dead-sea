using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gm;

    public AudioClip soundMove, soundAttackMelee, soundAttackLaser, soundAttackBomb, soundHit, soundDie, soundBombLocked, soundGainPoints;
    public AudioSource source;

    public int score { get; private set; } = 0;

    public int hp { get; private set; } = 3;

    public enum Lane { Left, Middle, Right }

    public Lane currentLane = Lane.Middle;

    public enum PlayerAttackType { Melee, Laser, Bomb }

    bool lockAttack, lockMovement = true;
    [SerializeField] bool unlock = false;
    [SerializeField] float bombCooldown = 10f;
    float bombCooldownStart = -10f;

    // Structure

    public void Beat()
    {
        lockAttack = false;
        lockMovement = false;
    }

    private void Update()
    {
        float time = gm.audioManager.position;

        if (time < gm.audioManager.nextEnemyBeat)
            if (time > gm.audioManager.nextEnemyBeat - gm.audioManager.beatLength + (gm.judgementTime / 2))
                return;

        CheckForMovementInput();
        CheckForAttackInput();
    }

    void CheckForMovementInput()
    {
        if (gm.currentGameState == GameManager.GameState.Paused)
            return;

        if (lockMovement)
            return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(horizontalInput) > float.Epsilon)
            MoveLane(horizontalInput);
    }

    void CheckForAttackInput()
    {
        if (gm.currentGameState == GameManager.GameState.Paused)
            return;

        if (lockAttack)
            return;

        if (Input.GetKey(KeyCode.X))
            AttackMelee();

        if (Input.GetKey(KeyCode.C))
            AttackLaser();

        if (Input.GetKey(KeyCode.V))
            AttackBomb();
    }

    // Dedicated Implementations

    void MoveLane(float direction)
    {
        if (!unlock)
            lockMovement = true;

        if (direction > float.Epsilon)
        {
            if ((int)currentLane < 2)
                currentLane++;
        }
        else
            if ((int)currentLane > 0)
            currentLane--;

        Vector3 pos = transform.position;
        pos.x = ((int)currentLane - 1) * gm.laneX;
        transform.position = pos;

        // gm.ui.SetInputSlider();

        source.clip = soundMove;
        source.Play();
    }

    void AttackMelee()
    {
        List<Enemy> enemiesToAttack = gm.enemyManager.GetEnemiesOnRow(1);

        if (enemiesToAttack.Count == 0)
            return;

        source.clip = soundAttackMelee;
        AttackSuccessful(enemiesToAttack[0]);
        foreach (var e in enemiesToAttack)
            if (e.lane == currentLane)
            {
                lockAttack = true;
                e.TakeDamage(PlayerAttackType.Melee);
                return;
            }

    }

    void AttackLaser()
    {
        float currentPosition = gm.audioManager.position;

        if (currentPosition < bombCooldownStart + bombCooldown)
        {
            // @TODO: Play Bomb locked Sound
            return;
        }

        List<Enemy> enemiesToAttack = gm.enemyManager.GetEnemiesOnLane(currentLane);

        bombCooldownStart = currentPosition;

        source.clip = soundAttackLaser;

        if (enemiesToAttack.Count > 0)
            AttackSuccessful(enemiesToAttack[0]);

        foreach (var e in enemiesToAttack)
            e.TakeDamage(PlayerAttackType.Laser);

        if (!unlock)
            lockAttack = true;
    }

    void AttackBomb()
    {
        float currentPosition = gm.audioManager.position;

        if (currentPosition < bombCooldownStart + bombCooldown)
        {
            // @TODO: Play Bomb locked Sound
            return;
        }

        List<Enemy> enemiesToAttack = gm.enemyManager.GetEnemiesOnRow(1);

        if (enemiesToAttack.Count == 0)
        {
            source.clip = soundBombLocked;
            source.Play();
            return;
        }

        foreach (var e in enemiesToAttack)
            e.TakeDamage(PlayerAttackType.Bomb);

        // Not Frame Perfect, but it's just a Prototype
        bombCooldownStart = currentPosition;

        source.clip = soundAttackBomb;
        if (!unlock)
            lockAttack = true;
        // Debug.Log($"Attacked {enemiesToAttack.Count} Enemies with Bomb.");

        AttackSuccessful(enemiesToAttack[0]);
    }

    void AttackSuccessful(Enemy e)
    {
        source.Play();
        gm.ui.SetInputSlider(e);
    }

    public void ModifyScore(int change)
    {
        score += change;

        if (change <= 0)
            return;

        source.clip = soundGainPoints;
        source.PlayDelayed(.14f);
    }

    public void TakeDamage()
    {
        hp--;

        gm.ui.UpdatePlayerHealth();

        if (hp > 0)
        {
            source.clip = soundHit;
            source.Play();
            return;
        }

        Die();
    }

    void Die()
    {
        gm.SetState (GameManager.GameState.GameOver);
        gm.audioManager.Stop();
        source.clip = soundDie;
        source.Play();
    }
}
