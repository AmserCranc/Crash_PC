public class gool
{
#region gool flags
/* events */
    public const uint EVENT_JUMPED_ON       = 0;
    public const uint EVENT_HIT             = 0x300;
    public const uint EVENT_SPIN_HIT        = 0x400;
    public const uint EVENT_TRIGGERED       = 0x800;
    public const uint EVENT_FALL_KILL       = 0x900;
    public const uint EVENT_BOX_STACK_BREAK = 0x900;
    public const uint EVENT_HIT_INVINCIBLE  = 0xA00;
    public const uint EVENT_STATUS          = 0xF00;
    public const uint EVENT_COMBO           = 0x1000;
    public const uint EVENT_RESPAWN         = 0x1300;
    public const uint EVENT_EAT             = 0x1400;
    public const uint EVENT_BOUNCE          = 0x1500;
    public const uint EVENT_WARP            = 0x1600;
    public const uint EVENT_SQUASH          = 0x1900;
    public const uint EVENT_TERMINATE       = 0x1A00;
    public const uint EVENT_WIN_BOSS        = 0x1D00;
    public const uint EVENT_FLING           = 0x1D00;
    public const uint EVENT_EXPLODE         = 0x1E00;
    public const uint EVENT_BURN            = 0x1F00;
    public const uint EVENT_DROWN           = 0x2100;
    public const uint EVENT_SHOCK           = 0x2300;
    public const uint EVENT_HIT_FENCE       = 0x2300;
    public const uint EVENT_BOULDER_SQUASH  = 0x2500;
    public const uint EVENT_LEVEL_END       = 0x2900;
    public const uint EVENT_PLAYER_DAMAGE   = 0x2B00;

/* status a flags */
    public const uint FLAG_GROUNDLAND        = 0x1;
    public const uint FLAG_TOWARD_GOAL       = 0x4;
    public const uint FLAG_TROT_DIR          = 0x8;
    public const uint FLAG_CHANGE_PATH_DIR   = 0x10;
    public const uint FLAG_FIRST_FRAME       = 0x20;
    public const uint FLAG_HIT_AT_BOTTOM     = 0x80;
    public const uint FLAG_INVALID_PATH      = 0x200;
    public const uint FLAG_DYING             = 0x400;
    public const uint FLAG_REACHED_TROT      = 0x800;
    public const uint FLAG_PLAYER_D_COLLIDER = 0x1000;
    public const uint FLAG_LBOUND_INVALID    = 0x8000;
    public const uint FLAG_KEEP_EVENT_STACK  = 0x20000;
    public const uint FLAG_ON_FLOOR          = 0x40000;
    public const uint FLAG_ATOP_OBJECT       = 0x200000;

/* status b flags */
    public const uint FLAG_ROT_Y             = 0x1;
    public const uint FLAG_TRACK_PATH_ROT    = 0x2;
    public const uint FLAG_TRACK_PATH_SIGN   = 0x4;
    public const uint FLAG_STOPPED_BY_SOLID  = 0x8;
    public const uint FLAG_COLLIDABLE        = 0x10;
    public const uint FLAG_COLLIDEABLE       = 0x10;
    public const uint FLAG_GRAVITY           = 0x20;
    public const uint FLAG_TRANS_MOTION      = 0x40;
    public const uint FLAG_DPAD_CONTROL      = 0x80;
    public const uint FLAG_INVISIBLE         = 0x100;
    public const uint FLAG_2D                = 0x200;
    public const uint FLAG_ROT_X             = 0x2000;
    public const uint FLAG_SOLID_GROUND      = 0x4000;
    public const uint FLAG_ORIENT_ON_PATH    = 0x8000;
    public const uint FLAG_SOLID_SIDES       = 0x10000;
    public const uint FLAG_SOLID_TOP         = 0x20000;
    public const uint FLAG_ROT_Y2            = 0x80000;
    public const uint FLAG_FORCE_UPDATE      = 0x2000000;
    public const uint FLAG_HAS_SHADOW        = 0x4000000;
    public const uint FLAG_STRING_CENTER     = 0x4000000;
    public const uint FLAG_SOLID_BOTTOM      = 0x8000000;
    public const uint FLAG_STALL             = 0x10000000;
    public const uint FLAG_PAUSED            = 0x20000000;  /* GOOL_DEBUG extension only */
    public const uint FLAG_STEP              = 0x40000000;  /* GOOL_DEBUG extension only */

/* state/status c flags */
    public const uint FLAG_GROUND_STATE      = 0x4;
    public const uint FLAG_AIR_STATE         = 0x8;
    public const uint FLAG_FLING_STATE       = 0x10;  /* rotate state? */
    public const uint FLAG_COLOVERRIDE_STATE = 0x800;
    public const uint FLAG_MENUTEXT_STATE    = 0x20000;
    public const uint FLAG_NOTERM_STATE      = 0x40000;
    public const uint FLAG_EXPENDABLE_STATE  = 0x80000;

/* spawn flags */
    public const uint FLAG_SPAWNED           = 0x1;
    public const uint FLAG_ACTIVE_SPAWN      = 0x8;

/* display flags */
    public const uint FLAG_DISPLAY_WORLDS    = 0x1;
    public const uint FLAG_CAM_UPDATE        = 0x2;
    public const uint FLAG_DISPLAY           = 0x4;
    public const uint FLAG_ANIMATE           = 0x8;
    public const uint FLAG_DISPLAY_C1        = 0x10;
    public const uint FLAG_ANIMATE_C1        = 0x20;
    public const uint FLAG_DISPLAY_C356      = 0x40;
    public const uint FLAG_ANIMATE_C356      = 0x80;
    public const uint FLAG_ANIMATE_C2        = 0x100;
    public const uint FLAG_DISPLAY_C2        = 0x200;
    public const uint FLAG_ANIMATE_C4        = 0x400;
    public const uint FLAG_DISPLAY_C4        = 0x800;
    public const uint FLAG_COUNT_DRAWS       = 0x1000;
    public const uint FLAG_DISPLAY_SOLID_BGC = 0x2000;
    public const uint FLAG_FORCE_DISP_MENUS  = 0x4000;
    public const uint FLAG_FORCE_ANIM_MENUS  = 0x8000;
    public const uint FLAG_SPIN_DEATH        = 0x10000;
    public const uint FLAG_DISPLAY_IMAGES    = 0x20000;
    public const uint FLAG_SPIN_ACCEL        = 0x40000;
    public const uint FLAG_DISPLAY_UNK       = 0x200000;

    public const uint FLAG_DISPANIM_ALL      = 0x3C00F;
    public const uint FLAG_DISPANIM_OBJECTS  = 0xFFFC;


/* interpreter flags */
    public const uint FLAG_SUSPEND_ON_RET    = 0x1;
    public const uint FLAG_SUSPEND_ON_RETLNK = 0x2;
    public const uint FLAG_SUSPEND_ON_ANIM   = 0x4;
    public const uint FLAG_EVENT_SERVICE     = 0x8;
    public const uint FLAG_STATUS_PRESERVE   = 0x10;
    public const uint FLAG_RETURN_EVENT      = 0x20;

/* constants */
    public const uint OBJECT_COUNT           = 96;
    public const uint LEVEL_SPAWN_COUNT      = 3592;
    public const uint SPAWN_COUNT            = 304;
#endregion
}
