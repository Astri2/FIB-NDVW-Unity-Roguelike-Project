using System;
using UnityEngine;

public class EnemyHorizontalMovement : StateBasedEnemy
{

    [Header("Movement Settings")]
    [SerializeField] protected float speed = 2f; // Movement speed
    [SerializeField] protected int direction = 1;

    protected override EnemyState_ GetInitialState()
    {
        return EnemyState_.Patrolling;
    }

    protected override void SetStateCallbacks()
    {
        base.SetStateCallbacks();
        stateCallbacks.Add(EnemyState_.Patrolling, new Action(PatrolingState));
    }

    ///// States implementation
    protected virtual void PatrolingState()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }


    ///// Unity callbacks
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Walls")) return;
        direction *= -1;
    }
}