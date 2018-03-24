using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityActions
{
    Idle,
    Move,
    Rotate,
    Attack,
    Die,
    Dead
}

public enum MoveDirection
{
    Forward,
    Backward,
    Left,
    Right
}

public enum TurnDirection
{
    Left,
    Right
}

public abstract class Entity : MonoBehaviour {
    [SerializeField] protected Vector2Int m_tilePos = new Vector2Int(0, 0);
    [SerializeField] protected Vector2Int m_faceDirection = new Vector2Int(0, 1);
    [SerializeField] protected int m_hitpoints = 3;
    [SerializeField] protected int m_damage = 1;
    [SerializeField] protected float m_moveSeconds = 1.0f;
    [SerializeField] protected float m_rotateSeconds = 1.0f;
    [SerializeField] protected float m_attackSeconds = 1.0f;
    [SerializeField] protected float m_dieSeconds = 1.0f;
    [SerializeField] protected AudioSource m_AudioSource;
    [SerializeField] protected AudioClip m_AttackSound1;
    [SerializeField] protected AudioClip m_AttackSound2;
    [SerializeField] protected AudioClip m_AttackSound3;
    [SerializeField] protected AudioClip m_DeathSound;
    [SerializeField] protected AudioClip m_WalkSound;
    
    protected EntityActions m_currentAction = EntityActions.Idle;
    private Vector2Int m_moveTargetPos;
    private Vector2Int m_faceDirectionTarget;
    private Vector2Int m_attackTargetTile;
    protected float m_actionTimer = 0.0f;

    public Vector2Int GetTilePosition()
    {
        return m_tilePos;
    }

    public bool IsAlive()
    {
        return (m_currentAction != EntityActions.Dead && m_currentAction != EntityActions.Die) ? true : false;
    }

    public void TakeDamage(int damage)
    {
        if (IsAlive())
        {
            m_hitpoints -= damage;
            if (m_hitpoints <= 0)
            {
                m_hitpoints = 0;
                m_currentAction = EntityActions.Die;
                m_actionTimer = m_dieSeconds;
                Die();
            }
        }
    }

    private bool UpdateActionTimer(float deltaTime)
    {
        bool result = false;

        if (m_actionTimer > 0.0f)
        {
            m_actionTimer -= deltaTime;
            if (m_actionTimer <= 0.0f)
            {
                m_actionTimer = 0.0f;
                result = true;
            }
        }

        return result;
    }

    Vector2Int RotateVec2IntBy90Degrees(Vector2Int vector, TurnDirection direction)
    {
        Vector2Int result = new Vector2Int(vector.y, vector.x);
        if (direction == TurnDirection.Left && result.y == 0)
        {
            result.x *= -1;
        }
        else if (direction == TurnDirection.Right && result.x == 0)
        {
            result.y *= -1;
        }

        return result;
    }

    protected void Move(MoveDirection direction)
    {
        if (m_currentAction == EntityActions.Idle)
        {
            Vector2Int moveDirection = m_faceDirection;
            switch(direction)
            {
                default:
                    { } break;
                case MoveDirection.Backward:
                    {
                        moveDirection *= -1;
                    } break;
                case MoveDirection.Left:
                    {
                        moveDirection = RotateVec2IntBy90Degrees(moveDirection, TurnDirection.Left);
                    } break;
                case MoveDirection.Right:
                    {
                        moveDirection = RotateVec2IntBy90Degrees(moveDirection, TurnDirection.Right);
                    } break;
            }
            m_moveTargetPos = m_tilePos + moveDirection;
            if (LevelData.IsWalkable(m_moveTargetPos) && !EntityRegister.IsTileOccupied(m_moveTargetPos))
            {
                m_currentAction = EntityActions.Move;
                m_actionTimer = m_moveSeconds;
                m_AudioSource.PlayOneShot(m_WalkSound);
                EntityRegister.ReserveTilePos(this.gameObject, m_moveTargetPos);
            }
        }
    }

    private void MoveAnimate()
    {
        Vector3 current = LevelData.TilePosToWorldVec3(m_tilePos);
        Vector3 target = LevelData.TilePosToWorldVec3(m_moveTargetPos);
        transform.position = Vector3.Lerp(current, target, 1.0f - m_actionTimer);
    }

    protected void Rotate(TurnDirection direction)
    {
        if(m_currentAction == EntityActions.Idle)
        {
            m_faceDirectionTarget = RotateVec2IntBy90Degrees(m_faceDirection, direction);
            m_currentAction = EntityActions.Rotate;
            m_actionTimer = m_rotateSeconds;
        }
    }

    private void RotateAnimate()
    {
        Vector3 currentDirection = new Vector3(m_faceDirection.x, 0, m_faceDirection.y);
        Vector3 targetDirection = new Vector3(m_faceDirectionTarget.x, 0, m_faceDirectionTarget.y);
        Quaternion currentRotation = Quaternion.LookRotation(currentDirection, Vector3.up);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, 1.0f - m_actionTimer);
    }

    protected void Attack()
    {
        if (m_currentAction == EntityActions.Idle)
        {
            m_currentAction = EntityActions.Attack;
            m_actionTimer = m_attackSeconds;
            m_attackTargetTile = m_tilePos + m_faceDirection;

            float randomValue = Random.value;
            if (randomValue >= 0.0f && randomValue < 0.33f)
            {
                m_AudioSource.PlayOneShot(m_AttackSound1);
            }
            if (randomValue >= 0.33f && randomValue < 0.66f)
            {
                m_AudioSource.PlayOneShot(m_AttackSound2);
            }
            else
            {
                m_AudioSource.PlayOneShot(m_AttackSound3);
            }
        }
    }

    protected virtual void Die()
    {
        m_AudioSource.PlayOneShot(m_DeathSound);
    }

    protected abstract void AttackAnimate();
    protected abstract void DieAnimate();
    protected abstract void Dead();

	// Use this for initialization
	public virtual void Start ()
    {
        transform.position = LevelData.TilePosToWorldVec3(m_tilePos);
        transform.rotation = Quaternion.LookRotation(new Vector3(m_faceDirection.x, 0, m_faceDirection.y), Vector3.up);

        EntityRegister.RegisterEntity(this.gameObject);
	}
	
	// Update is called once per frame
	public virtual void Update ()
    {
        bool done = UpdateActionTimer(Time.deltaTime);

        switch(m_currentAction)
        {
        default:
            { } break;
        case EntityActions.Move:
            {
                MoveAnimate();
                if(done)
                {
                    m_tilePos = m_moveTargetPos;
                    EntityRegister.ClearTileReservation(this.gameObject);
                }
            } break;
        case EntityActions.Rotate:
            {
                RotateAnimate();
                if(done)
                {
                    m_faceDirection = m_faceDirectionTarget;
                }
            } break;
        case EntityActions.Attack:
            {
                AttackAnimate();
                if(done)
                {
                    EntityRegister.DamageEntityOnTile(m_attackTargetTile, m_damage);
                }
            } break;
        case EntityActions.Die:
            {
                DieAnimate();
                if(done)
                {
                    Dead();
                }
            } break;
        }

        if(done)
        {
            if (m_hitpoints <= 0)
            {
                m_currentAction = EntityActions.Dead;
            }
            else
            {
                m_currentAction = EntityActions.Idle;
            }
        }
	}
}
