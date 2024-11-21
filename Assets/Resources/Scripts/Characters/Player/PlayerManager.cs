using Game;
using System.Collections.Generic;
using UnityEngine;
using static SaveManager;

public class PlayerManager : Character
{
    public enum State
    {
        Idle,
        Attack,
        Inventory
    }

    private static PlayerManager _instance;

    //Components and fields
    private PlayerAnimator playerAnimator;
    private PlayerInput playerInput;
    private ProjectileSpawner projectileSpawner;

    private State currentPlayerState;

    [SerializeField]
    private SlotManager slotManager;

    Animator animator;

    private ActionItemSO currentAction;

    [SerializeField]
    private HealthBar healthBar;

    private bool isAttacking;
    private bool canAttack;

    [SerializeField]
    private float attackTimer;

    #region Properties

    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("PlayerManager instance is null.");
            }
            return _instance;
        }
    }

    public State CurrentPlayerState
    {
        get => currentPlayerState;
        set
        {
            if (currentPlayerState == value) return; // Avoid redundant updates

            currentPlayerState = value;

            // Execute behavior based on the new state
            switch (value)
            {
                case State.Idle:
                    CanAttack = true;
                    break;

                case State.Attack:
                    break;

                case State.Inventory:
                    CanAttack = false;
                    break;

                // Handle other states as needed
                default:
                    CanAttack = false;
                    break;
            }
        }
    }

    public ActionItemSO CurrentAction
    {
        get => currentAction;
        set
        {
            if (currentAction == value) return; // No need to reassign if the value hasn't changed

            ActionItemSO previousAction = currentAction;
            currentAction = value;

            // Reset the attack timer if the action changes
            if (previousAction != currentAction)
            {
                attackTimer = 0;
            }
        }
    }

    public bool IsAttacking
    {
        get => isAttacking;
        private set
        {
            isAttacking = value;
            if (value == true)
            {
                currentPlayerState = State.Attack;
            }
        }
    }

    public bool CanAttack
    {
        get => canAttack;
        set
        {
            canAttack = value;
        }
    }

    #endregion


    private void Awake()
    {
        // Singleton setup
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected override void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerInput = GetComponent<PlayerInput>();
        projectileSpawner = GetComponentInChildren<ProjectileSpawner>();

        slotManager = GetComponentInChildren<SlotManager>();

        UpdateActionSlots();
        
        animator = GetComponentInChildren<Animator>();

        //Health setup
        base.Start();
        healthBar.SetMaxHealth(maxHealth);
    }

    public void Update()
    {
        HandleAttackDelay();
        slotManager.HandleCooldowns();
        playerAnimator.HandleAnimations(IsAttacking);

        if (IsAttacking)
        {
            playerInput.HandleAttackRotation();
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        healthBar.SetHealth(health);
    }


    public override void Attack(int attackIndex)
    {
        if (CanAttack)
        {
            // Check if the index is valid
            if (attackIndex >= 0 && attackIndex < slotManager.actionSlots.Count)
            {
                // Get the item from the corresponding action slot
                ActionItemSO actionItem = slotManager.actionSlots[attackIndex].item as ActionItemSO;

                if (actionItem != null && actionItem.timerCooldown <= 0) // Check cooldown is finished
                {
                    CurrentAction = actionItem;

                    if (actionItem is AttackTypeSO attackTypeAction)
                    {
                        attackTypeAction.PerformAction(animator);
                        IsAttacking = true;
                        projectileSpawner.projectile = attackTypeAction.projectile;

                        // Reset the cooldown after attack
                        actionItem.timerCooldown = actionItem.cooldown;
                    }
                }
                else
                {
                    Debug.Log("Action is on cooldown");
                }
            }
            else
            {
                Debug.Log("Not a valid index");
            }
        }
    }



    public void UpdateActionSlots()
    {
        // Access the SaveManager singleton
        var saveData = SaveManager.Instance.LoadSavedData("savefile.json");

        // Now call SetUpSlots with the saved action slots
        if (saveData != null)
        {
            slotManager.SetUpSlots();
        }
    }


    private void HandleAttackDelay()
    {
        if (IsAttacking && currentAction != null)
        {
            if (currentAction is AttackTypeSO attackTypeAction)
            {
                if (attackTimer <= attackTypeAction.attackDelay)
                {
                    attackTimer += Time.deltaTime;
                }
                else
                {
                    attackTimer = 0;
                    currentAction = null;
                    IsAttacking = false;
                    currentPlayerState = State.Idle;
                }
            }
        }
    }

    protected override void Die()
    {
        Debug.Log("Player is dead");
    }
}
