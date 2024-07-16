using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DragNDrop : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase enter; // 특정 타일 저장
    public List<TileBase> selectableTiles; // 선택 가능한 타일 리스트
    public List<Vector3Int> allowedPositions; // 타일을 옮길 수 있는 특정 위치들
    public Vector3Int minRange; // 타일을 둘 수 있는 범위 최소값
    public Vector3Int maxRange; // 타일을 둘 수 있는 범위 최대값

    private Vector3Int selectedTilePosition;
    private TileBase selectedTile;
    private bool isDragging = false;
    private bool canMoveTiles = false; // 일반 타일 움직임 활성화 상태
    private bool canMoveTileEnter = false; // 'enter' 타일 움직임 활성화 상태
    private GameObject previewTile;
    private GameObject targetTile; // 반투명 타일 미리 보기

    public GameObject Enterance; // Enterance 오브젝트
    public GameObject Counter; // Counter 오브젝트
    public int counterPositionX = 2;
    public int counterPositionY = 3;

    private int targetZ = 0; // 초기 targetZ 값

    void Update()
    {
        HandleInput();

        if (!canMoveTiles && !canMoveTileEnter)
        {
            return; // 타일 움직임이 비활성화된 경우, 이후 로직을 실행하지 않음
        }

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        if (cellPos.z != targetZ)
        {
            Debug.Log("Invalid Z coordinate, ignoring.");
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown(cellPos);
        }

        if (isDragging)
        {
            UpdateDragging(cellPos);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            OnMouseUp(cellPos);
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleMoveTiles();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMoveTileEnter();
        }
    }

    void ToggleMoveTiles()
    {
        canMoveTiles = !canMoveTiles;
        if (canMoveTiles)
        {
            canMoveTileEnter = false;
            targetZ = 4;
        }
        else
        {
            targetZ = 0;
        }
        Debug.Log($"Tile movement {(canMoveTiles ? "enabled" : "disabled")}, targetZ set to {targetZ}");

        if (isDragging)
        {
            ResetDragging();
        }
    }

    void ToggleMoveTileEnter()
    {
        canMoveTileEnter = !canMoveTileEnter;
        if (canMoveTileEnter)
        {
            canMoveTiles = false;
            targetZ = 4;
        }
        else
        {
            targetZ = 0;
        }
        Debug.Log($"Tile movement with 'enter' tile {(canMoveTileEnter ? "enabled" : "disabled")}, targetZ set to {targetZ}");

        if (isDragging)
        {
            ResetDragging();
        }
    }

    void OnMouseDown(Vector3Int cellPos)
    {
        if (tilemap.HasTile(cellPos))
        {
            TileBase clickedTile = tilemap.GetTile(cellPos);

            if ((canMoveTiles && !selectableTiles.Contains(clickedTile)) ||
                (canMoveTileEnter && clickedTile != enter))
            {
                Debug.Log("Tile is not selectable or does not match 'enter', ignoring.");
                return;
            }

            selectedTilePosition = cellPos;
            selectedTile = clickedTile;
            isDragging = true;
            Debug.Log("Selected tile at: " + selectedTilePosition);
            tilemap.SetTile(selectedTilePosition, null);
        }
    }

    void UpdateDragging(Vector3Int cellPos)
    {
        if (targetTile == null)
        {
            CreateTargetTile();
        }
        Vector3 cellSize = tilemap.cellSize;
        Vector3 offset = new Vector3(-cellSize.x /2, -cellSize.y / 2, 0);
        Vector3 tilePosition = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor + offset;
        targetTile.transform.position = tilePosition;
    }

    void OnMouseUp(Vector3Int cellPos)
    {
        Debug.Log("Mouse Up at: " + cellPos);
        Destroy(targetTile);
        targetTile = null;

        if (!tilemap.HasTile(cellPos) &&
            ((canMoveTiles && IsWithinRange(cellPos, minRange, maxRange)) ||
             (canMoveTileEnter && allowedPositions.Contains(cellPos))))
        {
            tilemap.SetTile(cellPos, selectedTile);
            Debug.Log("Tile placed at: " + cellPos);
            if (canMoveTileEnter && selectedTile == enter)
            {   
                Vector3 cellSize = tilemap.cellSize;
                Vector3 offset = new Vector3(0, -cellSize.y, 0);
                Vector3 enterancePosition = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor + offset;
                Enterance.transform.position = enterancePosition;
                Vector3 offset2 = new Vector3(-counterPositionX*cellSize.x, counterPositionY*cellSize.y, 0);
                Vector3 counterPosition = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor + offset2;
                Counter.transform.position = counterPosition;
                Debug.Log("Enterance moved to: " + enterancePosition);
            }
        }
        else
        {
            tilemap.SetTile(selectedTilePosition, selectedTile);
            Debug.Log("Target position not allowed or occupied, reverting to original position.");
            
        }
        isDragging = false;
    }

    void CreateTargetTile()
    {
        targetTile = new GameObject("TargetTile");
        var spriteRenderer = targetTile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = ((Tile)selectedTile).sprite;
        spriteRenderer.color = new Color(1f, 1f, 0f, 0.5f); // 노란색 반투명으로 변경
        spriteRenderer.sortingOrder = 9;
    }

    void ResetDragging()
    {
        tilemap.SetTile(selectedTilePosition, selectedTile);
        if (previewTile != null)
        {
            Destroy(previewTile);
            previewTile = null;
        }
        if (targetTile != null)
        {
            Destroy(targetTile);
            targetTile = null;
        }
        isDragging = false;
        Debug.Log("Movement cancelled and tile returned to original position.");
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = targetZ;
        return mouseWorldPos;
    }

    bool IsWithinRange(Vector3Int position, Vector3Int min, Vector3Int max)
    {
        return position.x >= min.x && position.x <= max.x &&
               position.y >= min.y && position.y <= max.y &&
               position.z == min.z; // Z축은 고정
    }
}
