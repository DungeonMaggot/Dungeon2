using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Megaskull : Character {
    [SerializeField] private GameObject m_MegaskullMesh;
    [SerializeField] private GameObject m_MegaskullCounterText;
    private Vector2Int m_lastKnownPlayerPos = new Vector2Int(-1, -1);
    private float m_waitSeconds = 2.0f;

    protected override void Die()
    {
        Text megaskullCounter = (Text)m_MegaskullCounterText.GetComponent(typeof(Text));
        if (megaskullCounter)
        {
            int count = int.Parse(megaskullCounter.text);
            --count;
            megaskullCounter.text = count.ToString();
        }
        base.Die();
    }

    protected override void DieAnimate()
    {
        float interpolationValue = m_actionTimer / m_dieSeconds;
        Vector3 scale = m_MegaskullMesh.transform.localScale;
        scale.y = interpolationValue;
        m_MegaskullMesh.transform.localScale = scale;
    }

    protected override void Dead()
    {
        MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
        if(mesh)
        {
            mesh.enabled = false;
        }
    }

	// Use this for initialization
	protected override void Start () {
        base.Start();

        Text megaskullCounter = (Text)m_MegaskullCounterText.GetComponent(typeof(Text));
        if(megaskullCounter)
        {
            int count = int.Parse(megaskullCounter.text);
            ++count;
            megaskullCounter.text = count.ToString();
        }
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        if (m_currentAction == EntityActions.Idle)
        {
            // pursue or attack player, if visible
            Player player = m_gameManagerReference.LookToFindPlayer(m_tilePos, m_facingDirection);
            if (player && player.IsAlive())
            {
                m_actionTimer = 0.0f;
                m_lastKnownPlayerPos = player.GetTilePosition();
                Vector2Int deltaVec = m_lastKnownPlayerPos - m_tilePos;
                int distanceToPlayer = (int)deltaVec.magnitude;
                if (distanceToPlayer > 1)
                {
                    BeginMove(MoveDirection.Forward);
                }
                else if (distanceToPlayer == 1)
                {
                    BeginAttack();
                }
            }
            else if (m_incomingDamageDir.magnitude != 0.0f)
            {
                // turn to face source of incoming damage
                Vector2Int directionToFace = Vector2Int.Scale(m_incomingDamageDir, new Vector2Int(-1, -1));
                int innerProduct = m_facingDirection.x * directionToFace.x + m_facingDirection.y * directionToFace.y;
                if (innerProduct == 1)
                {
                    m_incomingDamageDir.Set(0, 0);
                }
                else if(innerProduct == -1)
                {
                    if(Random.value < 0.5f)
                    {
                        BeginRotation(TurnDirection.Left);
                    }
                    else
                    {
                        BeginRotation(TurnDirection.Right);
                    }
                }
                else // inner product == 0
                {
                    if (m_facingDirection.y != 0)
                    {
                        if (m_facingDirection.y != directionToFace.x)
                        {
                            BeginRotation(TurnDirection.Left);
                        }
                        else
                        {
                            BeginRotation(TurnDirection.Right);
                        }
                    }
                    else // x != 0
                    {
                        if (m_facingDirection.x == directionToFace.y)
                        {
                            BeginRotation(TurnDirection.Left);
                        }
                        else
                        {
                            BeginRotation(TurnDirection.Right);
                        }
                    }
                }
            }
            else 
            {
                // go to last known player position, if possible
                if(m_lastKnownPlayerPos.x >= 0 && m_lastKnownPlayerPos.y >= 0)
                {
                    Vector2Int deltaVec = m_lastKnownPlayerPos - m_tilePos;
                    int distanceToLastKnownPos = (int)deltaVec.magnitude;
                    if(distanceToLastKnownPos > 0)
                    {
                        BeginMove(MoveDirection.Forward);
                    }
                    else if(m_actionTimer <= 0.0f)
                    {
                        if(Random.value < 0.8f)
                        {
                            BeginRotation(TurnDirection.Left);
                        }
                        else
                        {
                            BeginRotation(TurnDirection.Right);
                        }
                        if (m_gameManagerReference.Level().IsWalkable(m_tilePos + m_facingDirection))
                        {
                            m_actionTimer = m_waitSeconds;
                        }
                    }
                }
            }
        }
	}
}
