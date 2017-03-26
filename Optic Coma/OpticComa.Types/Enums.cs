using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticComa_Types
{
    /// <summary>
    /// Tells game how the enemy will move
    /// </summary>
    public enum EnemyType
    {
        Jiggler,
        Wavey
    }
    /// <summary>
    /// Tells game how NPC will act
    /// </summary>
    public enum NPCMode
    {
        ScriptedMovement,
        Stationary,
        Dead
    }
    /// <summary>
    /// Checks for orientation of some object
    /// </summary>
    public enum Orientation
    {
        Vert,
        Horiz
    }
    /// <summary>
    /// Tells game how entity is behaving at the moment
    /// </summary>
    public enum EntitySpriteMode
    {
        Idle,
        Walking,
        Running,
        Hit
    }
    /// <summary>
    /// Order in which things are rendered on the screen
    /// </summary>
    public enum LayerDepth
    {
        HUD,
        ForegroundTiles,
        Enemy,
        Flashlight,
        Player,
        MidgroundTiles,
        BackgroundTiles,
        BGImage
    }
}
