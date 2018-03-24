using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity {
    [SerializeField] private GameObject m_Sword;
    [SerializeField] private GameObject m_HitpointsText;
    private Vector3 m_swordPositionDefault;
    private Quaternion m_swordRotationDefault;
    private Vector3 m_swordPositionAttack = new Vector3(-0.2f, 0.5f, 0.7f);
    private Quaternion m_swordRotationAttack = Quaternion.Euler(172.0f, -10.0f, 170.0f);

    protected override void AttackAnimate()
    {
        if(m_actionTimer >= (m_attackSeconds/2))
        {
            float interpolationValue = 1.0f - ((m_actionTimer - (m_attackSeconds / 2)) * 2)/m_attackSeconds;
            m_Sword.transform.localPosition = Vector3.Lerp(m_swordPositionDefault, m_swordPositionAttack, interpolationValue);
            m_Sword.transform.localRotation = Quaternion.Slerp(m_swordRotationDefault, m_swordRotationAttack, interpolationValue);
        }
        else
        {
            float interpolationValue = 1.0f - (m_actionTimer * 2)/m_attackSeconds;
            m_Sword.transform.localPosition = Vector3.Lerp(m_swordPositionAttack, m_swordPositionDefault, interpolationValue);
            m_Sword.transform.localRotation = Quaternion.Slerp(m_swordRotationAttack, m_swordRotationDefault, interpolationValue);
        }
    }

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

        m_swordPositionDefault = m_Sword.transform.localPosition;
        m_swordRotationDefault = m_Sword.transform.localRotation;
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
        
        Text text = (Text)m_HitpointsText.GetComponent(typeof(Text));
        Debug.Log(text);
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
