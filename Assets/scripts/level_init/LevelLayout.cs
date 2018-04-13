using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: bottom left tile has coordinate (0,0)
public class LevelLayout
{
    // Attributes
    private string m_layoutString;
    private Vector2Int m_levelSize;
    private bool m_isInitialized;

    // Constructors
    public LevelLayout(string levelString, int levelWidth)
    {
        m_layoutString = levelString;
        int height = m_layoutString.Length / levelWidth;
        m_levelSize = new Vector2Int(levelWidth, height);
        m_isInitialized = true;
    }

    public static Vector3 TilePosToWorldVec3(Vector2Int tilePos)
    {
        Vector3 result;
        result.x = 2 * tilePos.x;
        result.y = 0;
        result.z = 2 * tilePos.y;
        return result;
    }

    //
    // level queries
    //
    public bool IsInitialized()
    {
        return m_isInitialized;
    }

    public int GetWidth()
    {
        return m_levelSize.x;
    }

    public int GetHeight()
    {
        return m_levelSize.y;
    }

    public Vector2Int GetSize()
    {
        return m_levelSize;
    }

    public bool IsWalkable(Vector2Int tilePos)
    {
        bool result = false;

        // check bounds
        if (tilePos.x >= 0 && tilePos.x < m_levelSize.x && tilePos.y >= 0 && tilePos.y < m_levelSize.y)
        {
            int stringIndex = (m_levelSize.y - tilePos.y - 1) * m_levelSize.x + tilePos.x;
            result = (m_layoutString.Substring(stringIndex, 1) != " ");
        }

        return result;
    }
};