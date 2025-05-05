using UnityEngine;
using UnityEngine.Tilemaps;

public class CorruptTilemapOnMove : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase corruptedTile;

    private Vector3Int lastTilePos;

    void Update()
    {
        Vector3Int currentTilePos = tilemap.WorldToCell(transform.position);

        if (currentTilePos != lastTilePos)
        {
            if (tilemap.GetTile(currentTilePos) != corruptedTile)
            {
                tilemap.SetTile(currentTilePos, corruptedTile);
            }

            lastTilePos = currentTilePos;
        }
    }
}
