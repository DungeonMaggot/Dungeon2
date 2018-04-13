using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {
    [SerializeField] protected GameManager m_gameManagerReference;
    [SerializeField] protected Vector2Int m_tilePos;
    [SerializeField] protected Vector2Int m_facingDirection;
    protected float m_actionTimer = 0.0f;
    protected bool m_actionMidEventTriggered = false;
    protected bool m_actionDone = false;

    public Vector2Int GetTilePosition()
    {
        return m_tilePos;
    }

    public Vector2Int GetFacingDirection()
    {
        return m_facingDirection;
    }

    private void UpdateActionTimer(float deltaTime)
    {
        m_actionDone = false;

        if (m_actionTimer > 0.0f)
        {
            m_actionTimer -= deltaTime;
            if (m_actionTimer <= 0.0f)
            {
                m_actionTimer = 0.0f;
                m_actionDone = true;
            }
        }
    }

    public virtual void TakeDamage(int damage, Vector2Int damageDir)
    {
        // does nothing by default
    }

	// Use this for initialization
	protected virtual void Start ()
    {
        transform.position = LevelLayout.TilePosToWorldVec3(m_tilePos);
        if (m_facingDirection.magnitude == 1.0f)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(m_facingDirection.x, 0, m_facingDirection.y), Vector3.up);
        }
        else if (m_facingDirection.magnitude != 0.0f)
        {
            Debug.LogWarning("Entity has a facing direction of " + m_facingDirection + " (magnitude is neither 0 nor 1).");
        }

        if (!m_gameManagerReference)
        {
            Debug.LogError("Error: Entity " + this.gameObject + " has an empty GameManager reference!");
        }
        m_gameManagerReference.RegisterEntity(this.gameObject);
	}
	
	// Update is called once per frame
	protected virtual void Update ()
    {
        UpdateActionTimer(Time.deltaTime);
	}
}
