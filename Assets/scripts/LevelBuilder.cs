using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {
    [SerializeField] private GameObject m_FloorMesh;
    [SerializeField] private GameObject m_CeilingMesh;
    [SerializeField] private GameObject m_WallMesh;
    [SerializeField] private GameObject m_ColumnMesh;
    private const int m_NumberOfPropVariants = 13;
    [SerializeField] private GameObject[] m_PropMeshes = new GameObject[m_NumberOfPropVariants];
    
    void Awake()
    {
        int levelWidth = LevelData.GetLevelWidth();
        int levelHeight = LevelData.GetLevelHeight();

        for (int y = 0; y < levelHeight; ++y)
        {
            for(int x = 0; x < levelWidth; ++x)
            {
                Vector2Int tilePos = new Vector2Int(x, y);

                if (LevelData.IsWalkable(tilePos))
                {
                    Vector3 worldPos = LevelData.TilePosToWorldVec3(tilePos);
                    GameObject floorMeshClone = (GameObject)Instantiate(m_FloorMesh, worldPos, Quaternion.identity);
                    GameObject ceilingMeshClone = (GameObject)Instantiate(m_CeilingMesh, worldPos, Quaternion.identity);

                    // check neighbours

                    // west
                    if (!LevelData.IsWalkable(new Vector2Int(tilePos.x - 1, tilePos.y)))
                    {
                        float angle = 90.0f;
                        GameObject wallMeshClone = (GameObject)Instantiate(m_WallMesh, worldPos, Quaternion.AngleAxis(angle, Vector3.up));
                        GameObject columnMeshClone = (GameObject)Instantiate(m_ColumnMesh, worldPos, Quaternion.AngleAxis(angle, Vector3.up));
                    }

                    if (!LevelData.IsWalkable(new Vector2Int(tilePos.x + 1, tilePos.y)))
                    {
                        // east
                        float angle = 270.0f;
                        GameObject wallMeshClone = (GameObject)Instantiate(m_WallMesh, worldPos, Quaternion.AngleAxis(angle, Vector3.up));
                        GameObject columnMeshClone = (GameObject)Instantiate(m_ColumnMesh, worldPos, Quaternion.AngleAxis(angle, Vector3.up));
                    }

                    // north
                    if (!LevelData.IsWalkable(new Vector2Int(tilePos.x, tilePos.y + 1)))
                    {
                        float angle = 180.0f;
                        GameObject wallMeshClone = (GameObject)Instantiate(m_WallMesh, worldPos, Quaternion.AngleAxis(angle, Vector3.up));
                        GameObject columnMeshClone = (GameObject)Instantiate(m_ColumnMesh, worldPos, Quaternion.AngleAxis(angle, Vector3.up));
                    }

                    // south
                    if (!LevelData.IsWalkable(new Vector2Int(tilePos.x, tilePos.y - 1)))
                    {
                        float angle = 0.0f;
                        GameObject wallMeshClone = (GameObject)Instantiate(m_WallMesh, worldPos, Quaternion.AngleAxis(angle, Vector3.up));
                        GameObject columnMeshClone = (GameObject)Instantiate(m_ColumnMesh, worldPos, Quaternion.AngleAxis(angle, Vector3.up));
                    }

                    // place prop
                    {
                        float randomValue = Random.value;
                        randomValue *= (m_NumberOfPropVariants - 1);
                        int propIndex = Mathf.RoundToInt(randomValue);

                        randomValue = Random.value;
                        randomValue *= 3;
                        int angle = Mathf.RoundToInt(randomValue);
                        angle *= 90;
                        GameObject propMeshClone = (GameObject)Instantiate(m_PropMeshes[propIndex], worldPos, Quaternion.AngleAxis(angle, Vector3.up));
                    }
                }
            }
        }
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
