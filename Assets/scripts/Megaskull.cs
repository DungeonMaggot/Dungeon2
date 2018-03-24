using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Megaskull : Entity {
    [SerializeField] private GameObject m_MegaskullMesh;
    [SerializeField] private GameObject m_MegaskullCounterText;
    private Vector3 m_MegaskullMeshPositionDefault;
    private Quaternion m_MegaskullMeshRotationDefault;
    private Vector3 m_MegaskullMeshPositionAttack = new Vector3(-0.13f, 0.22f, 1.05f);
    private Quaternion m_MegaskullMeshRotationAttack = Quaternion.Euler(8.59f, -5.5f, -9.28f);
    private Vector2Int m_lastKnownPlayerPos = new Vector2Int(-1, -1);
    private float m_waitSeconds = 2.0f;

    protected override void AttackAnimate()
    {
        if (m_actionTimer >= (m_attackSeconds / 2))
        {
            float interpolationValue = 1.0f - ((m_actionTimer - (m_attackSeconds / 2)) * 2) / m_attackSeconds;
            m_MegaskullMesh.transform.localPosition = Vector3.Lerp(m_MegaskullMeshPositionDefault, m_MegaskullMeshPositionAttack, interpolationValue);
            m_MegaskullMesh.transform.localRotation = Quaternion.Slerp(m_MegaskullMeshRotationDefault, m_MegaskullMeshRotationAttack, interpolationValue);
        }
        else
        {
            float interpolationValue = 1.0f - (m_actionTimer * 2) / m_attackSeconds;
            m_MegaskullMesh.transform.localPosition = Vector3.Lerp(m_MegaskullMeshPositionAttack, m_MegaskullMeshPositionDefault, interpolationValue);
            m_MegaskullMesh.transform.localRotation = Quaternion.Slerp(m_MegaskullMeshRotationAttack, m_MegaskullMeshRotationDefault, interpolationValue);
        }
    }

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
        Vector3 scale = m_MegaskullMesh.transform.localScale;
        scale.y = m_actionTimer;
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
	public override void Start () {
        base.Start();

        Text megaskullCounter = (Text)m_MegaskullCounterText.GetComponent(typeof(Text));
        if(megaskullCounter)
        {
            int count = int.Parse(megaskullCounter.text);
            ++count;
            megaskullCounter.text = count.ToString();
        }

        m_MegaskullMeshPositionDefault = m_MegaskullMesh.transform.localPosition;
        m_MegaskullMeshRotationDefault = m_MegaskullMesh.transform.localRotation;
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();

        if (m_currentAction == EntityActions.Idle)
        {
            Player player = EntityRegister.LookToFindPlayer(m_tilePos, m_faceDirection);
            if (player && player.IsAlive())
            {
                m_actionTimer = 0.0f;
                m_lastKnownPlayerPos = player.GetTilePosition();
                Vector2Int deltaVec = m_lastKnownPlayerPos - m_tilePos;
                int distanceToPlayer = (int)deltaVec.magnitude;
                if (distanceToPlayer > 1)
                {
                    Move(MoveDirection.Forward);
                }
                else if (distanceToPlayer == 1)
                {
                    Attack();
                }
            }
            else
            {
                if(m_lastKnownPlayerPos.x >= 0 && m_lastKnownPlayerPos.y >= 0)
                {
                    Vector2Int deltaVec = m_lastKnownPlayerPos - m_tilePos;
                    int distanceToLastKnownPos = (int)deltaVec.magnitude;
                    if(distanceToLastKnownPos > 0)
                    {
                        Move(MoveDirection.Forward);
                    }
                    else if(m_actionTimer <= 0.0f)
                    {
                        if(Random.value < 0.8f)
                        {
                        Rotate(TurnDirection.Left);
                        }
                        else
                        {
                            Rotate(TurnDirection.Right);
                        }
                        if (LevelData.IsWalkable(m_tilePos + m_faceDirection))
                        {
                            m_actionTimer = m_waitSeconds;
                        }
                    }
                }
            }
        }
	}
}
