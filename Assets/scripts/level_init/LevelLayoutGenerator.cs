using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayoutGenerator : LevelLayoutInitializer {

    // NOTE: Level coordinate (x = 0, y = 0) is the bottom left corner of the map.

    public override LevelLayout ProvideLevelLayout()
    {
        // BEGIN TEST - create simple level
        string layoutString =
            " + " +
            "+++" +
            "+ +";
        int levelWidth = 3;
        LevelLayout result = new LevelLayout(layoutString, levelWidth);
        // END TEST - create simple level

        return result;
    }
}
