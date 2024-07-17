using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;


// Ÿ���� ������ ��� ������
class FarmingData
{
    public Seed seed; // Ÿ���� ������ ���� ����
    //public bool seedOnTile; // Ÿ�� ���� ������ �ִ��� ���� Ȯ�ο�(������ ������ ���� �� �� ����)
    public bool plowEnableState; // ���� �� �� �ִ� �������� ���� Ȯ�ο�(���� �� ���� ����)
    public bool plantEnableState; // ������ ���� �� �ִ� �������� ���� Ȯ�ο�
    public bool harvestEnableState; // �۹��� �� �ڶ� �������� ���� Ȯ�ο�
    public Button stateButton; // Ÿ���� ������ Ÿ�� ���� �ߵ��� �ϴ� ��ư
    public Button[] buttons; // [0]: plow ��ư, [1]: plant ��ư, [2]: harvest ��ư

    public string currentState = "None"; // ���� ����(�ʱ⿡�� �ƹ��͵� �� �� ���´ϱ� None ����.. -> plow: �� ���� ����, plant: ���� ���� ����, harvest: �� �ڶ� ����)
}


public class FarmingManager : MonoBehaviour
{
    [Header("Game Data")]
    public Camera mainCamera; // ���콺 ��ǥ�� ���� ���� ��ǥ�� ��ȯ�ϱ� ���� �ʿ��� ����(ī�޶� ������Ʈ �Ҵ����� ��)
    public SeedContainer seedContainer; // ���� ���� ������ �������� ���� �ʿ��� ����(���� �����̳� ���� ������Ʈ �Ҵ����� ��)
    public FruitContainer fruitContainer; // ��Ȯ�� ������ �����ϱ� ���� �ʿ��� ����(���� �����̳� ���� ������Ʈ �Ҵ����� ��)

    [Header("Tile")]
    public TileBase grassTile; // �� ���� �� ����
    public TileBase farmTile; // �� �� �� ����
    public TileBase plantTile; // ���� ���� �� ����
    public TileBase harvestTile; // ���� �� �ڶ� ����
    public Vector3Int prevSelectTile; // ���� Ŭ���� Ÿ��

    [Header("Tilemap")]
    public Tilemap farmEnableZoneTilemap; // ��� ���� ������ ��Ÿ���� Ÿ�ϸ�
    public Tilemap farmTilemap; // ��¥�� ���� Ÿ���� ���¿� ���� Ÿ���� ����Ǵ� Ÿ�ϸ�

    [Header("Farm interaction Button")]
    // ��ư�� ���������� ����� ���� ���� �������� �����ؼ� �� ��.
    public GameObject[] buttonPrefabs; // [0]: plow ��ư, [1]: plant ��ư, [2]: harvest ��ư
    public Canvas buttonParent; // ��ư ������ �� �θ� �����ϱ� ���� ����

    [Header("Farm interaction Panel")]
    public GameObject growTimePanel; // �� �ڶ����� ���� �ð� �����ִ� �ǳ�
    public Text growTimeText; // �� �ڶ����� ���� �ð�

    [Header("Farming Data")]
    public Vector2 clickPosition; // ���� ���콺 ��ġ�� ���� ���� ��ġ�� �ٲ㼭 ����
    public Vector3Int cellPosition; // ���� ���� ��ġ�� Ÿ�� ���� Ÿ�� �� ��ġ�� ��ȯ
    Dictionary<Vector3Int, FarmingData> farmingData;


    private void Awake()
    {
        farmingData = new Dictionary<Vector3Int, FarmingData>();
        clickPosition = Vector2.zero;
    }

    private void Start()
    {
        // ��� ���� ������ farmingData �� ������ ����.
        foreach (Vector3Int pos in farmEnableZoneTilemap.cellBounds.allPositionsWithin)
        {
            if (!farmEnableZoneTilemap.HasTile(pos)) continue;

            // ����Ƽ������ new �� ������ class �� MonoBehaviour �� ��� ������ �� ��.
            farmingData[pos] = new FarmingData();
            farmingData[pos].buttons = new Button[3]; // [0]: plow ��ư, [1]: plant ��ư, [2]: harvest ��ư

            // �� Ÿ�ϸ��� �� ���� ��ư�� ������ �����ϵ���..
            for (int i=0; i<buttonPrefabs.Length; i++)
            {
                // Ŭ���� ������ ���ϱ� ���ؼ� ���� ������ �����س��� �� ������ �����..
                int index = i;
                Vector3Int tilePos = pos;
                farmingData[pos].buttons[i] = CreateButton(index, tilePos).GetComponent<Button>();

                if (index==0)
                {
                    // ��ư�� �Լ��� �����س���(tilePos �� ���� �����س���)
                    farmingData[tilePos].buttons[index].onClick.AddListener(() => PlowTile(tilePos));
                } 
                else if (index==1)
                {
                    farmingData[tilePos].buttons[index].onClick.AddListener(() => PlantTile(tilePos));
                } 
                else if (index==2)
                {
                    farmingData[tilePos].buttons[index].onClick.AddListener(() => HarvestTile(tilePos));
                }
            }

            // �� ó������ plow ��ư�� �����ϰ� �ֵ���
            farmingData[pos].stateButton = farmingData[pos].buttons[0];

            prevSelectTile = pos;
        }
    }

    void Update()
    {
        // ��ư ������ �� �ڿ� �ִ� Ÿ�� �� �������� �ϱ� ���� ����..
        if (IsPointerOverUIObject()) return;

        // ���� ���� ���콺Ű�� ������..
        if (Input.GetMouseButtonDown(0))
        {
            growTimePanel.SetActive(false); // ������ ������ ���� �ǳ� ��������..

            // ���� �ƹ��͵� �� �� ���´� plow ��ư�� ����, ���� ���´� ��ư���� plant ��ư�� ���´�.
            // �ٸ� ���� Ŭ���ϸ� ���� Ŭ���� ���� ��ư�� �� ������ �ϹǷ� SetActive �� �Ⱥ��̰� �����Ѵ�..
            // ��Ȯ�ϱ� ��ư�� ������ �� �ڶ�� ��� ��������..
            if (farmEnableZoneTilemap.HasTile(prevSelectTile))
            {
                if (farmingData[prevSelectTile].currentState == "None" || farmingData[prevSelectTile].currentState == "plow")
                {
                    farmingData[prevSelectTile].stateButton.gameObject.SetActive(false);
                }
            }

            // ���� ���콺 ��ġ�� ���� ���� ��ġ�� �ٲ㼭 ����
            clickPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // ���� ���� ��ġ�� Ÿ�� ���� Ÿ�� �� ��ġ�� ��ȯ
            cellPosition = farmTilemap.WorldToCell(clickPosition);
            

            foreach (Vector3Int pos in farmingData.Keys)
            {
                // �����س��� Ÿ�� �߿� ���� ���콺�� Ŭ���� ��ġ�� ���� Ÿ���� ������
                if (pos == cellPosition)
                {
                    // ���� �� ���� ���¸� ������ �� ��ư �� �� �ֵ���
                    if (farmingData[cellPosition].plowEnableState)
                    {
                        farmingData[cellPosition].stateButton.gameObject.SetActive(true);
                    } 
                    else
                    {
                        // ������ �� �ɾ��� ���� �� �Ǵ� ������ �� �ڶ��� �� ��ư �� �� �ֵ���
                        if (farmingData[cellPosition].seed == null || (farmingData[cellPosition].seed.isGrown))
                        {
                            farmingData[cellPosition].stateButton.gameObject.SetActive(true);
                        }
                        // ������ �ڶ�� ���̸� ���� �ð� ��Ÿ���� �ǳ� �ߵ���
                        else if (!farmingData[cellPosition].seed.isGrown)
                        {
                            // �ǳ� ��ġ�� ���� Ŭ���� Ÿ�� ��ġ��..
                            growTimePanel.transform.position = mainCamera.WorldToScreenPoint(farmTilemap.CellToWorld(cellPosition)) + new Vector3(0, 50, 0);
                            growTimePanel.SetActive(true);
                            growTimeText.text = "�����ð�\n" + (int)(farmingData[cellPosition].seed.growTime - farmingData[cellPosition].seed.currentTime);
                        }
                    }

                    // ���� ������ Ÿ���� ��ư�� Ȱ��ȭ �ǵ���..
                    //farmingData[cellPosition].stateButton.gameObject.SetActive(true);
                    // �Ʒ� ���ó�� �ϸ� ������. ������ ����??
                    // --> GameObject �� ������Ʈ�� �ƴ϶� ������ ���� ���̾���... �̹� ��ȸ�� �˰� �ƴ�.
                    // --> �׳� gameObject �� �ٷ� ���� ������Ʈ�� ������ �� �־���.
                    // --> �⺻���ΰ� ��԰� �־���.. �̹��� �� �������..
                    //farmingData[cellPosition].stateButton.GetComponent<GameObject>().SetActive(true);
                }
            }

            prevSelectTile = cellPosition; // ���� ���� Ÿ���� ������ ���� Ÿ�� ��ġ�� �����ϴ� ������ ����.. 
        }

        // �ڶ�µ� ���� �ð��� ��� ������Ʈ �Ǿ�� �ϹǷ�..
        if (farmEnableZoneTilemap.HasTile(cellPosition) && farmingData[cellPosition].seed != null)
        {
            if (!farmingData[cellPosition].seed.isGrown)
                growTimeText.text = "�����ð�\n" + (int)(farmingData[cellPosition].seed.growTime - farmingData[cellPosition].seed.currentTime);
            else
                growTimePanel.SetActive(false); // �� �ڶ�� �����ð� ��Ÿ���� �ǳ� ��������..
        }



        foreach (Vector3Int pos in farmingData.Keys)
        {
            if (farmingData[pos].seed != null)
            {
                if (farmingData[pos].seed.isGrown)
                {
                    farmTilemap.SetTile(pos, harvestTile); // Ÿ���� ������ �� �ڶ� ���·� ����
                    farmingData[pos].harvestEnableState = true; // �۹� ��Ȯ�� �� �ִ� ����
                    farmingData[pos].stateButton.gameObject.SetActive(true); // ��Ȯ�ϱ� ��ư�� �׻� ���־�� ��
                    farmingData[pos].currentState = "harvest";
                }
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public GameObject CreateButton(int buttonNumber, Vector3Int pos)
    {
        // Ÿ�� �� �ʱ� ������ �� ���� �Լ�
        // Ÿ�ϸ��� ��ư�� �̸� �������� ����� ����

        GameObject button = Instantiate(buttonPrefabs[buttonNumber], buttonParent.transform);

        // �� ��ǥ�� ���� ��ǥ�� �ٲ㼭 ����
        Vector3 worldPos = farmTilemap.CellToWorld(pos);
        // ���� ��ǥ�� ��ũ�� ��ǥ�� �ٲ㼭 ����
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        // ��ư�� ��ǥ ����
        button.transform.position = screenPos;
        button.transform.position += new Vector3(0, 50, 0);

        return button;
    }
    
    public void PlowTile(Vector3Int pos)
    {
        // ���� ���� �Լ�

        farmTilemap.SetTile(pos, farmTile); // Ÿ�� ����� �� �� ���·� �ٲ��ֱ�
        farmingData[pos].plowEnableState = false; // �������ϱ� ���� �� �� ���� ���¸� ��Ÿ���� ���� false �� �� ����
        farmingData[pos].plantEnableState = true; // ������ ���� �� �ִ� ���¸� ��Ÿ���� ���� true �� �� ����
        farmingData[pos].currentState = "plow"; // ���� ���´ϱ� plow �� �ٲ��ֱ�

        farmingData[pos].stateButton.gameObject.SetActive(false); // ��ư �� �� �������ϱ� ��������..

        farmingData[pos].stateButton = farmingData[pos].buttons[1]; // plant ��ư���� ����..
    }

    public void PlantTile(Vector3Int pos)
    {
        // ������ �ɴ� �Լ�
        // ������ �� �Լ��� �Ű������� ���� �ε����� �����༭ GetSeed �Լ��� �Ű������� ���� ��.
        farmingData[pos].seed = seedContainer.GetSeed(0).GetComponent<Seed>();

        farmTilemap.SetTile(pos, plantTile); // Ÿ�� ����� ���� ���� ���·� �ٲ��ֱ�
        farmingData[pos].plantEnableState = true; // ������ ���� �� ���� ���¸� ��Ÿ���� ���� false �� ����
        farmingData[pos].currentState = "plant"; // ���� ���� ���´ϱ� plant �� �ٲ��ֱ�

        farmingData[pos].stateButton.gameObject.SetActive(false); // ��ư �� �� �������ϱ� ��������..

        farmingData[pos].stateButton = farmingData[pos].buttons[2]; // harvest ��ư�� ������ �ֵ���..
    }

    public void HarvestTile(Vector3Int pos)
    {
        // ������ ��Ȯ�ϴ� �Լ�
        // ������ �� �Լ��� �Ű������� ���� �ε��� �����༭ GetFruit �Լ��� �Ű������� ���� ��.

        farmingData[pos].plowEnableState = true;
        farmingData[pos].currentState = "None"; // ������ ��Ȯ�� ���´ϱ� None ���� �ٲ��ֱ�
        
        fruitContainer.GetFruit(farmingData[pos].seed.seedIdx); // ������ �ε����� ���� ���� ����

        farmingData[pos].stateButton.gameObject.SetActive(false); // ��ư �� �� �������ϱ� ��������..
        farmingData[pos].stateButton = farmingData[pos].buttons[0]; // plow ��ư�� ������ �ֵ���..

        farmingData[pos].seed = null; // ��Ȯ �Ϸ� �����ϱ� Ÿ���� seed ������ �ٽ� null �� �������ֱ�..

        farmTilemap.SetTile(pos, grassTile); // Ÿ�� ����� �ʱ� �����Ƿ� �ٲ��ֱ�
    }
}
