using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedLevelLayoutProvider : LevelLayoutInitializer {
    public uint m_LevelIndex = 0;

    // NOTE: Coordinate (x = 0, y = 0) is the bottom left corner.
    private LevelLayout[] m_layouts = new LevelLayout[] {
        new LevelLayout(
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
            "        ++               ",
            25),     // width
        new LevelLayout(
            " +++ " +
            "++S++" +
            " +++ " +
            "  +  " +
            "  +  " +
            "  +  " +
            "  +++" +
            "   ++" +
            "+++++" +
            "+    " +
            "+++ +" +
            " ++++" +
            " ++ +" +
            "  + +" +
            "    +",
            5),  // width
    };

    public override LevelLayout ProvideLevelLayout()
    {
        if (m_LevelIndex >= m_layouts.Length)
        {
            Debug.Log("Error: Attempted to select invalid invalid level index: " + m_LevelIndex +
                        " , max index is " + (m_layouts.Length - 1));
            return new LevelLayout("", 0);
        }

        return m_layouts[m_LevelIndex];
    }
}
