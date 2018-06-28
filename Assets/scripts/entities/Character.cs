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

public abstract class Character : Entity
{
    [SerializeField] protected int m_hitpoints = 3;
    [SerializeField] protected int m_damage = 1;
    [SerializeField] protected float m_moveSeconds = 1.0f;
    [SerializeField] protected float m_rotateSeconds = 1.0f;
    [SerializeField] protected float m_attackSeconds = 1.0f;
    [SerializeField] protected float m_dieSeconds = 1.0f;
    [SerializeField] protected GameObject m_WeaponMesh;
    [SerializeField] protected Transform m_WeaponAttackTransform;
    [SerializeField] protected AudioSource m_AudioSource;
    [SerializeField] protected AudioClip m_AttackSound1;
    [SerializeField] protected AudioClip m_AttackSound2;
    [SerializeField] protected AudioClip m_AttackSound3;
    [SerializeField] protected AudioClip m_AttackSound4;
    [SerializeField] protected AudioClip m_DeathSound;
    [SerializeField] protected AudioClip m_StepSound1;
    [SerializeField] protected AudioClip m_StepSound2;
    [SerializeField] protected AudioClip m_StepSound3;
    [SerializeField] protected AudioClip m_StepSound4;
    [SerializeField] protected AudioClip m_TakeDamage1;
    [SerializeField] protected AudioClip m_TakeDamage2;
    [SerializeField] protected AudioClip m_TakeDamage3;

    protected EntityActions m_currentAction = EntityActions.Idle;
    private Vector2Int m_moveTargetPos;
    private Vector2Int m_faceDirectionTarget;
    private Vector2Int m_attackTargetTile;
    protected Vector2Int m_incomingDamageDir = new Vector2Int(0, 0);
    protected Vector3 m_WeaponDefaultLocalPosition;
    protected Quaternion m_WeaponDefaultLocalRotation;

    public Vector2Int GetMoveTargetPos()
    {
        return m_moveTargetPos;
    }

    public bool IsAlive()
    {
        return (m_currentAction != EntityActions.Dead && m_currentAction != EntityActions.Die) ? true : false;
    }

    public override void TakeDamage(int damage, Vector2Int damageDir, Vector2Int attackTilePos)
    {
        if (IsAlive())
        {
            // "take damage" sound
            float randomValue = Random.value;
            if (randomValue >= 0.0f && randomValue < 0.33f)
            {
                m_AudioSource.PlayOneShot(m_TakeDamage1);
            }
            else if (randomValue >= 0.33f && randomValue < 0.66f)
            {
                m_AudioSource.PlayOneShot(m_TakeDamage2);
            }
            else
            {
                m_AudioSource.PlayOneShot(m_TakeDamage3);
            }

            m_hitpoints -= damage;
            m_incomingDamageDir = damageDir;
            if (m_hitpoints <= 0)
            {
                m_hitpoints = 0;
                m_currentAction = EntityActions.Die;
                m_actionTimer = m_dieSeconds;
                Die();
            }
        }
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

    protected void BeginMove(MoveDirection direction)
    {
        if (m_currentAction == EntityActions.Idle)
        {
            Vector2Int moveDirection = m_facingDirection;
            switch (direction)
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

            if (m_gameManagerReference.IsMoveValid(m_tilePos, m_moveTargetPos))
            {
                m_currentAction = EntityActions.Move;
                m_actionTimer = m_moveSeconds;
                float randomValue = Random.value;
                if (randomValue >= 0.0f && randomValue < 0.25f)
                {
                    m_AudioSource.PlayOneShot(m_StepSound1);
                }
                else if (randomValue >= 0.25f && randomValue < 0.50f)
                {
                    m_AudioSource.PlayOneShot(m_StepSound2);
                }
                else if (randomValue >= 0.50f && randomValue < 0.75f)
                {
                    m_AudioSource.PlayOneShot(m_StepSound3);
                }
                else
                {
                    m_AudioSource.PlayOneShot(m_StepSound4);
                }
                m_gameManagerReference.ReserveTilePos(this.gameObject, m_moveTargetPos);
            }
        }
    }

    private void AnimateMove()
    {
        float interpolationValue = 1.0f - (m_actionTimer / m_moveSeconds);
        Vector3 current = LevelLayout.TilePosToWorldVec3(m_tilePos);
        Vector3 target = LevelLayout.TilePosToWorldVec3(m_moveTargetPos);
        transform.position = Vector3.Lerp(current, target, interpolationValue);
    }

    protected void BeginRotation(TurnDirection direction)
    {
        if (m_currentAction == EntityActions.Idle)
        {
            m_faceDirectionTarget = RotateVec2IntBy90Degrees(m_facingDirection, direction);
            m_currentAction = EntityActions.Rotate;
            m_actionTimer = m_rotateSeconds;
        }
    }

    private void AnimateRotation()
    {
        float interpolationValue = 1.0f - (m_actionTimer / m_rotateSeconds);
        Vector3 currentDirection = new Vector3(m_facingDirection.x, 0, m_facingDirection.y);
        Vector3 targetDirection = new Vector3(m_faceDirectionTarget.x, 0, m_faceDirectionTarget.y);
        Quaternion currentRotation = Quaternion.LookRotation(currentDirection, Vector3.up);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, interpolationValue);
    }

    protected void BeginAttack()
    {
        if (m_currentAction == EntityActions.Idle)
        {
            m_currentAction = EntityActions.Attack;
            m_actionTimer = m_attackSeconds;
            m_actionMidEventTriggered = false;
            m_attackTargetTile = m_tilePos + m_facingDirection;

            float randomValue = Random.value;
            if (randomValue >= 0.0f && randomValue < 0.25f)
            {
                m_AudioSource.PlayOneShot(m_AttackSound1);
            }
            else if (randomValue >= 0.25f && randomValue < 0.50f)
            {
                m_AudioSource.PlayOneShot(m_AttackSound2);
            }
            else if (randomValue >= 0.50f && randomValue < 0.75f)
            {
                m_AudioSource.PlayOneShot(m_AttackSound3);
            }
            else
            {
                m_AudioSource.PlayOneShot(m_AttackSound4);
            }
        }
    }

    protected void AnimateAttack()
    {
        if (m_WeaponMesh && m_WeaponAttackTransform)
        {
            if (m_actionTimer >= (m_attackSeconds / 2))
            {
                // interpolate towards the end point of the attack animation
                float interpolationValue = 1.0f - ((m_actionTimer - (m_attackSeconds / 2)) * 2) / m_attackSeconds;
                m_WeaponMesh.transform.localPosition = Vector3.Lerp(m_WeaponDefaultLocalPosition, m_WeaponAttackTransform.localPosition, interpolationValue);
                m_WeaponMesh.transform.localRotation = Quaternion.Slerp(m_WeaponDefaultLocalRotation, m_WeaponAttackTransform.localRotation, interpolationValue);
            }
            else
            {
                // trigger attack "event"
                if (!m_actionMidEventTriggered)
                {
                    m_gameManagerReference.DamageEntityOnTile(m_attackTargetTile, m_facingDirection, m_damage);
                    m_actionMidEventTriggered = true;
                }

                // interpolate back towards the default pause
                float interpolationValue = 1.0f - (m_actionTimer * 2) / m_attackSeconds;
                m_WeaponMesh.transform.localPosition = Vector3.Lerp(m_WeaponAttackTransform.localPosition, m_WeaponDefaultLocalPosition, interpolationValue);
                m_WeaponMesh.transform.localRotation = Quaternion.Slerp(m_WeaponAttackTransform.localRotation, m_WeaponDefaultLocalRotation, interpolationValue);
            }
        }
    }

    protected virtual void Die()
    {
        m_AudioSource.PlayOneShot(m_DeathSound);
    }

    protected abstract void DieAnimate();
    protected abstract void Dead();

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        if (m_WeaponMesh)
        {
            m_WeaponDefaultLocalPosition = m_WeaponMesh.transform.localPosition;
            m_WeaponDefaultLocalRotation = m_WeaponMesh.transform.localRotation;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        switch (m_currentAction)
        {
            default:
                { } break;
            case EntityActions.Move:
                {
                    AnimateMove();
                    if (m_actionDone)
                    {
                        m_tilePos = m_moveTargetPos;
                        m_gameManagerReference.ClearTileReservation(this.gameObject);
                    }
                } break;
            case EntityActions.Rotate:
                {
                    AnimateRotation();
                    if (m_actionDone)
                    {
                        m_facingDirection = m_faceDirectionTarget;
                    }
                } break;
            case EntityActions.Attack:
                {
                    AnimateAttack();
                } break;
            case EntityActions.Die:
                {
                    DieAnimate();
                    if (m_actionDone)
                    {
                        Dead();
                    }
                } break;
        }

        if (m_actionDone)
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
