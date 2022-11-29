using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;

    [SerializeField]
    private bool isInvincible = false;
    private float timeSinceHit = 0;
    public float invincibilityTime = 1f;

    Animator animator;

    [SerializeField]
    private int _maxHealth = 100;

    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            //if health < 0, character died
            if(_health <= 0)
            {
                isAlive = false;
                Debug.Log("Dead");
            }
        }
    }

    private bool _isAlive = true;

    public bool isAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool("isAlive", _isAlive);
        }
    }

    public bool LockMovement
    {
        get
        {
            return animator.GetBool("lockMovement");
        }
        set
        {
            animator.SetBool("lockMovement", value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _isAlive = true;
    }

    private void Update()
    {
            Invincible();
    }

    public void Invincible()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                //remove invincibility
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    // returns whether the damageable took damage or not
    public bool Hit(int damage, Vector2 knockback)
    {
        if(isAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            //notify other subsribed components that the damageable was hit to handle the knockback
            animator.SetTrigger("hit");
            LockMovement = true;
            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            return true;
        }

        //unable to be hit
        return false;
    }

    public bool Heal(int healthRestore)
    {
        if (isAlive && Health < MaxHealth)
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;

            CharacterEvents.characterHealed(gameObject, actualHeal);

            return true;
        }

        return false;
    }
}