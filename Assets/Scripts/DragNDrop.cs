using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DragNDrop : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile highlightTile; // 하이라이트 타일을 표시하기 위한 타일 (예: 노란색)
    private Vector3Int selectedTilePosition;
    private TileBase selectedTile;
    private bool isDragging = false;
    private GameObject previewTile;
    private Vector3Int previousHighlightedPosition;
    private int targetZ = 4; // 이동 가능한 타일맵의 Z 좌표

    // 선택 가능한 타일 리스트
    public List<TileBase> selectableTiles;

    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        // Z 좌표가 targetZ와 동일하지 않으면 선택/이동하지 않음
        if (cellPos.z != targetZ)
        {
            Debug.Log("Invalid Z coordinate, ignoring.");
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Down at: " + cellPos);

            if (tilemap.HasTile(cellPos))
            {
                TileBase clickedTile = tilemap.GetTile(cellPos);
                
                // 타일이 선택 가능한 리스트에 있는지 확인
                if (!selectableTiles.Contains(clickedTile))
                {
                    Debug.Log("Tile is not selectable, ignoring.");
                    return;
                }

                selectedTilePosition = cellPos;
                selectedTile = clickedTile;
                isDragging = true;
                Debug.Log("Selected tile at: " + selectedTilePosition);

                // 드래그 시각적 피드백을 위한 프리뷰 타일 생성
                if (previewTile == null)
                {
                    previewTile = new GameObject("TilePreview");
                    var spriteRenderer = previewTile.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = ((Tile)selectedTile).sprite;
                    spriteRenderer.color = Color.yellow; // 노란색으로 변경
                    spriteRenderer.sortingOrder = 10; // 프리뷰 타일이 맨 앞에 보이도록
                }

                // 선택된 타일을 임시로 숨김
                tilemap.SetTile(selectedTilePosition, null);
            }
        }

        if (isDragging)
        {
            // 드래그 중인 타일의 위치 업데이트
            if (previewTile != null)
            {
                // 타일의 위치를 마우스 위치 또는 셀 위치에 맞춰 조정
                Vector3 tilePosition = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor;
                // 왼쪽 대각선 위로 이동
                Vector3 cellSize = tilemap.cellSize;
                Vector3 offset = new Vector3(-cellSize.x/2, cellSize.y, 0);
                previewTile.transform.position = tilePosition + offset;
            }

            // 이전 하이라이트 타일 제거
            if (previousHighlightedPosition != cellPos)
            {
                if (tilemap.GetTile(previousHighlightedPosition) == highlightTile)
                {
                    tilemap.SetTile(previousHighlightedPosition, null);
                }
            }

            // 새로운 하이라이트 타일 표시
            if (tilemap.GetTile(cellPos) == null)
            {
                tilemap.SetTile(cellPos, highlightTile);
                previousHighlightedPosition = cellPos;
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Debug.Log("Mouse Up at: " + cellPos);
            
            // 하이라이트 타일 제거
            if (tilemap.GetTile(previousHighlightedPosition) == highlightTile)
            {
                tilemap.SetTile(previousHighlightedPosition, null);
            }

            // 드래그 종료 시 타일 배치 (대상 위치에 다른 타일이 없는 경우에만)
            if (!tilemap.HasTile(cellPos))
            {
                tilemap.SetTile(cellPos, selectedTile);
                Debug.Log("Tile placed at: " + cellPos);
            }
            else
            {
                // 원래 위치로 타일 복원
                tilemap.SetTile(selectedTilePosition, selectedTile);
                Debug.Log("Target position occupied, reverting to original position.");
            }

            // 프리뷰 타일 제거
            if (previewTile != null)
            {
                Destroy(previewTile);
                previewTile = null;
            }

           
            isDragging = false;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        // 마우스 위치를 월드 좌표로 변환, Z축을 고려
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z); // 카메라의 Z축 위치로부터의 거리
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = targetZ; // 타일맵의 Z축을 고정
        //Debug.Log("Mouse World Position: " + mouseWorldPos);
        return mouseWorldPos;
    }
}