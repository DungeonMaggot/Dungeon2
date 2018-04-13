using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] protected LevelLayoutInitializer m_levelDataInitializer;
    [SerializeField] protected LevelGeometryBuilder m_levelGeometryBuilder;

    // level
    private LevelLayout m_levelLayout;

    // player and NPCs
    private const uint m_maxEntities = 10;
    private GameObject[] m_entities = new GameObject[m_maxEntities];
    private Vector2Int[] m_tileReservations = new Vector2Int[m_maxEntities];    // (-1, -1) means "not reserved" == "free"
    private int m_entityCount = 0;

    //
    // player and NPC registration
    //
    public void RegisterEntity(GameObject entity)
    {
        if (m_entityCount < m_maxEntities)
        {
            m_entities[m_entityCount] = entity;
            ++m_entityCount;
        }
    }

    //
    // level read-only access
    //
    public LevelLayout Level()
    {
        return m_levelLayout;
    }

    //
    // tile reservation methods, used for movement
    //
    public void ReserveTilePos(GameObject entity, Vector2Int tilePos)
    {
        for (int entityIndex = 0; entityIndex < m_entityCount; ++entityIndex)
        {
            if (entity == m_entities[entityIndex])
            {
                m_tileReservations[entityIndex] = tilePos;
            }
        }
    }

    public void ClearTileReservation(GameObject entity)
    {
        for (int entityIndex = 0; entityIndex < m_entityCount; ++entityIndex)
        {
            if (entity == m_entities[entityIndex])
            {
                ClearTileReservation(entityIndex);
            }
        }
    }

    public void ClearTileReservation(int index)
    {
        m_tileReservations[index] = new Vector2Int(-1, -1);
    }

    public void ClearAllTileReservations()
    {
        for (int reservationIndex = 0; reservationIndex < m_entityCount; ++reservationIndex)
        {
            ClearTileReservation(reservationIndex);
        }
    }

    //
    // tile queries
    //
    public bool IsTileOccupied(Vector2 tilePos)
    {
        bool result = false;

        for (int entityIndex = 0; entityIndex < m_entityCount; ++entityIndex)
        {
            Entity entity = m_entities[entityIndex].GetComponent(typeof(Entity)) as Entity;
            if (tilePos == entity.GetTilePosition() && entity.IsAlive())
            {
                result = true;
            }
            if (tilePos == m_tileReservations[entityIndex])
            {
                result = true;
            }
        }

        return result;
    }

    public void DamageEntityOnTile(Vector2Int attackTilePos, int hitpoints)
    {
        for (int entityIndex = 0; entityIndex < m_entityCount; ++entityIndex)
        {
            Entity entity = m_entities[entityIndex].GetComponent(typeof(Entity)) as Entity;
            if (attackTilePos == entity.GetTilePosition())
            {
                entity.TakeDamage(hitpoints);
            }
        }
    }

    public Entity GetEntityAtTilePos(Vector2Int tilePos)
    {
        Entity result = null;

        for (int entityIndex = 0; entityIndex < m_entityCount; ++entityIndex)
        {
            Entity entity = m_entities[entityIndex].GetComponent(typeof(Entity)) as Entity;
            if (tilePos == entity.GetTilePosition())
            {
                result = entity;
            }
        }

        return result;
    }

    public Player GetPlayerAtTilePos(Vector2Int tilePos)
    {
        Player result = null;

        for (int entityIndex = 0; entityIndex < m_entityCount; ++entityIndex)
        {
            Player player = m_entities[entityIndex].GetComponent(typeof(Player)) as Player;
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

    public Player LookToFindPlayer(Vector2Int tilePos, Vector2Int lookDirection)
    {
        Player result = null;

        for (Vector2Int testPos = tilePos + lookDirection; m_levelLayout.IsWalkable(testPos); testPos += lookDirection)
        {
            result = GetPlayerAtTilePos(testPos);
            if (result)
            {
                break;
            }
        }

        return result;
    }

    private void Awake()
    {
        // check editor-supplied properties
        if(!m_levelDataInitializer)
        {
            Debug.Log("Error: Level Data Initializer missing, cannot create level.");
            return;
        }
        if(!m_levelGeometryBuilder)
        {
            Debug.Log("Error: Level Geometry Builder missing, cannot create level.");
            return;
        }

        // initialize level
        m_levelLayout = m_levelDataInitializer.ProvideLevelLayout();    // set level layout string, width and height
        m_levelGeometryBuilder.Build(m_levelLayout);                    // create level geometry

        // entities
        this.ClearAllTileReservations();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
