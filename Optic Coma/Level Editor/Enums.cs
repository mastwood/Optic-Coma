using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level_Editor
{
    public enum LayerMode
    {
        Foreground,
        Midground,
        Background,
        Combined
    }
    public enum Tool
    {
        Draw,
        Edit,
        SelectObj,
        EnemySpawner,
        HitBox,
        HitTri
    }
}
