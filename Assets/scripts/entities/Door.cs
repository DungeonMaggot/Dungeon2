using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorMovement
{
    None,
    Opening,
    Closing
};

public class Door : Entity {
    [SerializeField] protected GameObject m_doorMesh;
    [SerializeField] protected Transform m_doorOpenTransform;
    [SerializeField] protected float m_moveSeconds = 2.0f;
    [SerializeField] protected float m_passableThreshold = 0.7f;

    protected Vector3 m_doorOpenLocalPos;
    protected Vector3 m_doorClosedLocalPos;
    protected float m_open = 0.0f;
    protected DoorMovement m_doorMovement = DoorMovement.None;

    public bool IsPassable()
    {
        return (m_open >= m_passableThreshold);
    }

    public Vector2Int GetNeighbouringTile()
    {
        return m_tilePos + m_facingDirection;
    }

    public bool IsOpen()
    {
        return (m_open <= 0.0f);
    }

    public bool IsClosed()
    {
        return !IsOpen();
    }

    public void Open()
    {
        BeginMove(DoorMovement.Opening);
    }

    public void Close()
    {
        BeginMove(DoorMovement.Closing);
    }

    public void Toggle()
    {
        if(m_doorMovement == DoorMovement.Opening)
        {
            Close();
        }
        else if(m_doorMovement == DoorMovement.Closing)
        {
            Open();
        }
    }

    public void BeginMove(DoorMovement desiredMovement)
    {
        if(desiredMovement == DoorMovement.None)
        {
            Debug.LogWarning("Requested door movement of 'none'.");
        }
        else if(desiredMovement != m_doorMovement)
        {
            m_actionTimer = m_moveSeconds;
        }
    }

    public void AnimateMove()
    {
        float interpolationValue = 1.0f - (m_actionTimer / m_moveSeconds);
        if(m_doorMovement == DoorMovement.Opening)
        {
            m_open = interpolationValue;
            m_doorMesh.transform.localPosition = Vector3.Lerp(m_doorClosedLocalPos, m_doorOpenLocalPos, interpolationValue); ;
        }
        else // closing
        {
            m_open = 1.0f - interpolationValue;
            m_doorMesh.transform.localPosition = Vector3.Lerp(m_doorOpenLocalPos, m_doorClosedLocalPos, interpolationValue);
        }
    }

	// Use this for initialization
	protected override void Start () {
        base.Start();

        m_doorOpenLocalPos = m_doorOpenTransform.localPosition;
        m_doorClosedLocalPos = m_doorMesh.transform.localPosition;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        if(m_doorMovement != DoorMovement.None)
        {
            if (m_actionDone)
            {
                m_doorMovement = DoorMovement.None;
            }
            else
            {
                AnimateMove();
            }
        }
	}
}
