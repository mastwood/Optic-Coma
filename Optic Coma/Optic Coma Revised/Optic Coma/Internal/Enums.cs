using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optic_Coma
{
    public enum RenderLayer
    {
        Bottom,
        BackgroundImage,
        BackgroundTiles,
        MidgroundTiles,
        Player,
        Enemies,
        ForegroundTiles,
        HUD,
        Top
    }
    public enum HUDButtonState
    {
        Idle,
        MouseOver,
        MouseClick,
        Greyed
    }
}
