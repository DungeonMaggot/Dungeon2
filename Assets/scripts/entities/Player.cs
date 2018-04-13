using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character {
    [SerializeField] private GameObject m_Sword;
    [SerializeField] private GameObject m_HitpointsText;

    protected override void Die()
    {
        base.Die();
    }

    protected override void DieAnimate()
    {

    }

    protected override void Dead()
    {
        Debug.Log("Player dead!");
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        
        Text text = (Text)m_HitpointsText.GetComponent(typeof(Text));
        if(text)
        {
            text.text = m_hitpoints.ToString();
        }

        if(Input.GetButtonDown("StepForward"))
        {
            BeginMove(MoveDirection.Forward);
        }
        if (Input.GetButtonDown("StepBackward"))
        {
            BeginMove(MoveDirection.Backward);
        }
        if (Input.GetButtonDown("StepLeft"))
        {
            BeginMove(MoveDirection.Left);
        }
        if (Input.GetButtonDown("StepRight"))
        {
            BeginMove(MoveDirection.Right);
        }

        if(Input.GetButtonDown("TurnLeft"))
        {
            BeginRotation(TurnDirection.Left);
        }
        if (Input.GetButtonDown("TurnRight"))
        {
            BeginRotation(TurnDirection.Right);
        }

        if(Input.GetButtonDown("Attack"))
        {
            BeginAttack();
        }

        if(Input.GetButtonDown("Use"))
        {
            m_gameManagerReference.ContextualInteraction(m_tilePos, m_facingDirection);
        }
	}
}
