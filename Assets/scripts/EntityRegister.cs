using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRegister : MonoBehaviour {

    private static GameObject[] m_Entities = new GameObject[10];
    private static Vector2Int[] m_TileReservations = new Vector2Int[10];
    private static int m_EntityCount = 0;

    public static void ReserveTilePos(GameObject entity, Vector2Int tilePos)
    {
        for(int entityIndex = 0; entityIndex < m_EntityCount; ++entityIndex)
        {
            if(entity == m_Entities[entityIndex])
            {
                m_TileReservations[entityIndex] = tilePos;
            }
        }
    }

    public static void ClearTileReservation(GameObject entity)
    {
        for (int entityIndex = 0; entityIndex < m_EntityCount; ++entityIndex)
        {
            if (entity == m_Entities[entityIndex])
            {
                ClearTileReservation(entityIndex);
            }
        }
    }

    public static void ClearTileReservation(int index)
    {
        Debug.Log(m_TileReservations[index]);
        m_TileReservations[index] = new Vector2Int(-1, -1);
    }

    public static void DamageEntityOnTile(Vector2Int attackTilePos, int hitpoints)
    {
        for (int entityIndex = 0; entityIndex < m_EntityCount; ++entityIndex)
        {
            Entity entity = m_Entities[entityIndex].GetComponent(typeof(Entity)) as Entity;
            if (attackTilePos == entity.GetTilePosition())
            {
                entity.TakeDamage(hitpoints);
            }
        }
    }

    public static bool IsTileOccupied(Vector2 tilePos)
    {
        bool result = false;

        for (int entityIndex = 0; entityIndex < m_EntityCount; ++entityIndex)
        {
            Entity entity = m_Entities[entityIndex].GetComponent(typeof(Entity)) as Entity;
            if (tilePos == entity.GetTilePosition() && entity.IsAlive())
            {
                result = true;
            }
            if(tilePos == m_TileReservations[entityIndex])
            {
                result = true;
            }
        }

        return result;
    }

    public static Entity GetEntityAtTilePos(Vector2Int tilePos)
    {
        Entity result = null;

        for (int entityIndex = 0; entityIndex < m_EntityCount; ++entityIndex)
        {
            Entity entity = m_Entities[entityIndex].GetComponent(typeof(Entity)) as Entity;
            if (tilePos == entity.GetTilePosition())
            {
                result = entity;
            }
        }

        return result;
    }

    public static Player GetPlayerAtTilePos(Vector2Int tilePos)
    {
        Player result = null;

        for (int entityIndex = 0; entityIndex < m_EntityCount; ++entityIndex)
        {
            Player player = m_Entities[entityIndex].GetComponent(typeof(Player)) as Player;
            if (player)
            {
                if (tilePos == player.GetTilePosition())
                {
                    result = player;
                    break;
                }
            }
        }

        return result;
    }

    public static void RegisterEntity(GameObject entity)
    {
        if(m_EntityCount < 10)
        {
            m_Entities[m_EntityCount] = entity;
            ++m_EntityCount;
        }
    }

    public static Player LookToFindPlayer(Vector2Int tilePos, Vector2Int lookDirection)
    {
        Player result = null;

        for (Vector2Int testPos = tilePos + lookDirection; LevelData.IsWalkable(testPos); testPos += lookDirection )
        {
            result = GetPlayerAtTilePos(testPos);
            if (result)
            {
                break;
            }
        }

        return result;
    }

	// Use this for initialization
	static void Start () {
		for(int reservationIndex = 0; reservationIndex < m_EntityCount; ++reservationIndex)
        {
            ClearTileReservation(reservationIndex);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
