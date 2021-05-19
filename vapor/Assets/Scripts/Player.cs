using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gm;

    public AudioClip soundMove, soundAttackMelee, soundAttackLaser, soundAttackBomb, soundHit, soundDie, soundBombLocked;
    public AudioSource source;

    int score = 0;

    int hp = 3;

    public enum Lane { Left, Middle, Right }

    public Lane currentLane = Lane.Middle;

    public enum PlayerAttackType { Melee, Laser, Bomb }

    bool lockAttack, lockMovement = true;
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

        if (Input.GetKeyDown(KeyCode.X))
            AttackMelee();

        if (Input.GetKeyDown(KeyCode.C))
            AttackLaser();

        if (Input.GetKeyDown(KeyCode.V))
            AttackBomb();
    }

    // Dedicated Implementations

    void MoveLane(float direction)
    {
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
        pos.x = ((int)currentLane - 1) * 3;
        transform.position = pos;

        source.clip = soundMove;
        source.Play();
    }

    void AttackMelee()
    {
        lockAttack = true;

        source.clip = soundAttackMelee;

        List<Enemy> enemiesToAttack = gm.enemyManager.GetEnemiesOnRow(1);

        if (enemiesToAttack.Count == 0)
            return;

        foreach (var e in enemiesToAttack)
            if (e.lane == currentLane)
            {
                e.TakeDamage(PlayerAttackType.Melee);
                source.Play();
                return;
            }
    }

    void AttackLaser()
    {
        lockAttack = true;

        if (gm.audioManager.position < bombCooldownStart + bombCooldown)
        {
            // @TODO: Play Bomb locked Sound
            return;
        }

        List<Enemy> enemiesToAttack = gm.enemyManager.GetEnemiesOnLane(currentLane);

        if (enemiesToAttack.Count == 0)
        {
            return;
        }

        foreach (var e in enemiesToAttack)
            e.TakeDamage(PlayerAttackType.Laser);

        source.clip = soundAttackLaser;
        source.Play();

    }

    void AttackBomb()
    {
        lockAttack = true;

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
        bombCooldownStart = currentPosition + bombCooldown;

        source.clip = soundAttackBomb;
        source.Play();

        // Debug.Log($"Attacked {enemiesToAttack.Count} Enemies with Bomb.");
    }

    public void ModifyScore(int change)
    {
        score += change;
    }

    public void TakeDamage()
    {
        hp--;

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
        gm.SetState(GameManager.GameState.GameOver);
        gm.audioManager.Stop();
        source.clip = soundDie;
        source.Play();
    }
}
