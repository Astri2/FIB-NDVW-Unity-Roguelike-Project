

using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState_ { Roaming, Patrolling, Chasing, ReturningToSpawn, Dead, Stun }

public abstract class StateBasedEnemy: Enemy
{
    private Dictionary<EnemyState_, Action> stateCallbacks = new();

    [SerializeField] private EnemyState_ enemyState;
    
    public override void Start()
    {
        SetStateCallbacks();   
        base.Start();
        enemyState = GetInitialState();
    }

    protected abstract EnemyState_ GetInitialState();

    protected virtual void SetStateCallbacks()
    {
        // default state, use as an example
        RegisterState(EnemyState_.Dead, new Action(deadState));
    }

    protected virtual void RegisterState(EnemyState_ state, Action action)
    {
        stateCallbacks.Add(state, action);
    }

    public void Update()
    {
        PreUpdate();

        stateCallbacks[enemyState].DynamicInvoke();

        PostUpdate();
    }

    // To override if your enemy has specific pre state execution logic
    protected virtual void PreUpdate() { 
        if(GetHP() <= 0) enemyState = EnemyState_.Dead;
    }

    // To override if your enemy has post state execution logic
    protected virtual void PostUpdate() { }

    protected virtual void deadState()
    {
        // allert the linked chests that the enemy is dead
        foreach (Chest chest in GetChests())
        {
            chest.RemoveEnemy(this);
        }
        Destroy(gameObject);
    }

    protected EnemyState_ GetState() => this.enemyState;

    protected virtual void SetState(EnemyState_ enemyState) {
        this.enemyState = enemyState;
    }
}
