using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class Enter : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject draggableTile;
    public GameObject entrance;
    public Tile highlightTile;

    private Vector3Int selectedTilePosition;
    private bool isDragging = false;

    void Update()
    {
        // P 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = tilemap.WorldToCell(mouseWorldPos);

            if (tilemap.HasTile(gridPosition))
            {
                StartDragging(gridPosition);
            }
        }

        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggableTile.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);

            if (Input.GetMouseButtonUp(0))
            {
                Vector3Int gridPosition = tilemap.WorldToCell(mouseWorldPos);
                if (tilemap.HasTile(gridPosition))
                {
                    PlaceTile(gridPosition);
                }
                else
                {
                    CancelDragging();
                }
            }
        }
    }

    void StartDragging(Vector3Int position)
    {
        isDragging = true;
        selectedTilePosition = position;
        draggableTile.transform.position = tilemap.GetCellCenterWorld(position);
        draggableTile.SetActive(true);
    }

    void PlaceTile(Vector3Int position)
    {
        isDragging = false;
        draggableTile.SetActive(false);
        
        if (tilemap.HasTile(position))
        {
            tilemap.SetTile(selectedTilePosition, highlightTile);
            entrance.transform.position = tilemap.GetCellCenterWorld(position);
        }
    }

    void CancelDragging()
    {
        isDragging = false;
        draggableTile.SetActive(false);
    }
    
}