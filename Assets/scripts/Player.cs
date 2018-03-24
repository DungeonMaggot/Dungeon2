using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity {
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

	// Use this for initialization
    public override void Start()
    {
        base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
        
        Text text = (Text)m_HitpointsText.GetComponent(typeof(Text));
        if(text)
        {
            text.text = m_hitpoints.ToString();
        }

        if(Input.GetButtonDown("StepForward"))
        {
            base.Move(MoveDirection.Forward);
        }
        if (Input.GetButtonDown("StepBackward"))
        {
            base.Move(MoveDirection.Backward);
        }
        if (Input.GetButtonDown("StepLeft"))
        {
            base.Move(MoveDirection.Left);
        }
        if (Input.GetButtonDown("StepRight"))
        {
            base.Move(MoveDirection.Right);
        }

        if(Input.GetButtonDown("TurnLeft"))
        {
            base.Rotate(TurnDirection.Left);
        }
        if (Input.GetButtonDown("TurnRight"))
        {
            base.Rotate(TurnDirection.Right);
        }

        if(Input.GetButtonDown("Attack"))
        {
            base.Attack();
        }
	}
}
