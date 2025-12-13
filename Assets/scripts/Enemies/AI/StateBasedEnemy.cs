

using System;
using System.Collections.Generic;

public enum EnemyState_ { Roaming, Patrolling, Chasing, ReturningToSpawn, Dead }

public abstract class StateBasedEnemy: Enemy
{
    protected Dictionary<EnemyState_, Action> stateCallbacks;

    protected EnemyState_ enemyState
    {
        get => enemyState;
        set => SetState(value);
    }

    public virtual void Start()
    {
        SetStateCallbacks();   
        enemyState = GetInitialState();
    }

    protected abstract EnemyState_ GetInitialState();

    protected virtual void SetStateCallbacks()
    {
        // default state, use as an example
        stateCallbacks.Add(EnemyState_.Dead, new Action(deadState));
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

    protected virtual void SetState(EnemyState_ enemyState) {
        this.enemyState = enemyState;
    }
}
