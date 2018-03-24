using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour {
    // bottom left tile has coordinate (0,0)
    
    private static string m_levelMap = 
        "    3+4+ 5               " +
        "  +++  +++6              " +
        "  2 +     7              " +
        "  +    + +8           H  " +
        " +++   + +     +++D+ FG  " +
        "+++++ ++ +9A++B+ + + +   " +
        " +++  ++      +  +++E+ LM" +
        "++S++  ++    C+ +    I ++" +
        " ++1  +++       +++  + + " +
        "  0    ++ +++     + KJ++ " +
        "        +++ + +++>+    N " +
        "        +   ++    +    + " +
        "    ++    +++++>++++++ O " +
        "   >++++++^ ++  +    +++ " +
        "+++ ++      +   ++++  +P " +
        "     +   ++++      +     " +
        "+++ ++ + +         + ++  " +
        " ++ ++ +++++ ++    ++++  " +
        " ++ +    + ++++          " +
        "  ^ ++X  ^   ++          " +
        "        ++               ";
    private static int m_levelWidth = 25;
    /*private static string m_levelMap =
        " +++ " +
        "++ + " +
        "+  ++" +
        "++++ ";
    private static int m_levelWidth = 5;*/
    private static int m_levelHeight = m_levelMap.Length/m_levelWidth;

    public static int GetLevelWidth()
    {
        return m_levelWidth;
    }

    public static int GetLevelHeight()
    {
        return m_levelHeight;
    }

    public static Vector3 TilePosToWorldVec3(Vector2Int tilePos)
    {
        Vector3 result;
        result.x = 2 * tilePos.x;
        result.y = 0;
        result.z = 2 * tilePos.y;
        return result;
    }

    public static bool IsWalkable(Vector2Int tilePos)
    {
        bool result = false;

        // check bounds
        if (tilePos.x >= 0 && tilePos.x < m_levelWidth && tilePos.y >= 0 && tilePos.y < m_levelHeight)
        {
            int stringIndex = (m_levelHeight - tilePos.y - 1) * LevelData.m_levelWidth + tilePos.x;
            result = (LevelData.m_levelMap.Substring(stringIndex, 1) != " ");
        }

        return result;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
