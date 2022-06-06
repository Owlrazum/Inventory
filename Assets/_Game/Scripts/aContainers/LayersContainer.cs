public static class LayersContainer
{
    public const int PLAYER_COLLISION_LAYER = 6;
    public const int PLAYER_HEALTH_HIT_LAYER = 7;
    public const int PORTALS_LAYER = 8;
    public const int PORTALLABLE_CELL_LAYER = 9;
    public const int LINE_THROUGH_PORTAL_LAYER = 10;

    public const int JUMPABLES_LAYER = 11;
    public const int PROJECTILES_LAYER = 12;
    public const int WALL_RUN_ZONE_LAYER = 13;
    public const int USUAL_PLATFORMS_LAYER = 14;

    public const int MOVABLE_PLATFORMS_LAYER = 15;
    public const int PUZZLE_ITEM_LAYER = 16;
    public const int PUZZLE_HEALTH_LAYER = 17;

    public const int DASH_THROUGH_LAYER = 31;
    public const int PERSPECTIVE_WALL_JUMP_ZONE_LAYER = 32;

    public const int PLAYER_COLLISION_LAYER_MASK = 1 << PLAYER_COLLISION_LAYER;
    public const int PORTALS_LAYER_MASK = 1 << PORTALS_LAYER;
    public const int PORTALABLE_CELL_LAYER_MASK = 1 << PORTALLABLE_CELL_LAYER;
    public const int LINE_THROUGH_PORTAL_LAYER_MASK = 1 << LINE_THROUGH_PORTAL_LAYER;

    public const int PROJECTILES_LAYER_MASK = 1 << PROJECTILES_LAYER;
    public const int USUAL_PLATFORMS_LAYER_MASK = 1 << USUAL_PLATFORMS_LAYER;
    public const int MOVABLE_PLATFORMS_LAYER_MASK = 1 << MOVABLE_PLATFORMS_LAYER;
    public const int PUZZLE_MOVABLE_LAYER_MASK = 1 << PUZZLE_ITEM_LAYER;
    public const int PUZZLE_HEALTH_LAYER_MASK = 1 << PUZZLE_HEALTH_LAYER;

    public const int DASH_THROUGH_LAYER_MASK = 1 << DASH_THROUGH_LAYER;

    public const int OBSTACLES_HIT_MASK = USUAL_PLATFORMS_LAYER_MASK | MOVABLE_PLATFORMS_LAYER_MASK;
    public const int LINE_THROUGH_PORTAL_HIT_MASK = OBSTACLES_HIT_MASK | PORTALS_LAYER_MASK;

    // Not added to the TagsMananger.asset
    public const int STAIRS_LAYER = 28;

    private const int PORTALLABLE_LAYER_MASK =
        PLAYER_COLLISION_LAYER_MASK |
        PROJECTILES_LAYER_MASK |
        PUZZLE_MOVABLE_LAYER_MASK;

    public static bool IsInPortallableLayer(int layer)
    {
        if ((PORTALLABLE_LAYER_MASK & (1 << layer)) != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}