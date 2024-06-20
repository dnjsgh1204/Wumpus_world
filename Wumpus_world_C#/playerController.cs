using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // 격자 정보
    private Dictionary<Vector2Int, string> gridInfo = new Dictionary<Vector2Int, string>();

    // 현재 위치
    private Vector2Int currentPosition = new Vector2Int(0, 0);

    // 프리팹
    public GameObject glitterPrefab;
    public GameObject breezePrefab;
    public GameObject stenchPrefab;

    // 금덩이를 가지고 있는지 여부
    private bool hasGold = false;

    void Start()
    {
        InitializeGridInfo();
        CheckAdjacentGrids();
        StartCoroutine(FindGold());
    }

    void InitializeGridInfo()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int gridPosition = currentPosition + new Vector2Int(x, y);
                if (gridPosition != currentPosition)
                {
                    gridInfo[gridPosition] = (Random.Range(0, 10) < 3) ? "Gold" : "";
                    gridInfo[gridPosition] += (Random.Range(0, 10) < 3) ? "Breeze" : "";
                    gridInfo[gridPosition] += (Random.Range(0, 10) < 3) ? "Stench" : "";
                }
            }
        }
    }

    void CheckAdjacentGrids()
    {
        foreach (KeyValuePair<Vector2Int, string> kvp in gridInfo)
        {
            Vector2Int adjacentPosition = kvp.Key;
            string content = kvp.Value;

            // 플레이어와의 거리가 1이고, 해당 격자에 내용이 있을 때만 보여줌
            if ((Mathf.Abs(adjacentPosition.x - currentPosition.x) == 1 || Mathf.Abs(adjacentPosition.y - currentPosition.y) == 1) && content != "")
            {
                if (content.Contains("Gold"))
                {
                    Instantiate(glitterPrefab, new Vector3(adjacentPosition.x, adjacentPosition.y, 0), Quaternion.identity);
                }

                if (content.Contains("Breeze"))
                {
                    Instantiate(breezePrefab, new Vector3(adjacentPosition.x, adjacentPosition.y, 0), Quaternion.identity);
                }

                if (content.Contains("Stench"))
                {
                    Instantiate(stenchPrefab, new Vector3(adjacentPosition.x, adjacentPosition.y, 0), Quaternion.identity);
                }
            }
        }
    }

    IEnumerator FindGold()
    {
        while (true)
        {
            if (!hasGold)
            {
                // 금덩이가 없는 경우 다음 위치로 이동
                MoveToNextGrid();
            }
            else if (currentPosition != Vector2Int.zero)
            {
                // 금덩이를 찾았고 아직 시작 위치로 돌아오지 않은 경우, 시작 위치로 이동
                MoveToOrigin();
            }
            else
            {
                // 금덩이를 찾았고 시작 위치로 돌아온 경우 종료
                Debug.Log("Gold found! Back to origin!");
                yield break;
            }
            yield return new WaitForSeconds(0.5f); // 0.5초마다 움직임
        }
    }

    void MoveToNextGrid()
    {
        // 랜덤하게 다음 위치로 이동
        Vector2Int nextPosition = currentPosition + new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
        if (gridInfo.ContainsKey(nextPosition))
        {
            currentPosition = nextPosition;
            CheckAdjacentGrids();
        }
    }

    void MoveToOrigin()
    {
        // 시작 위치로 이동
        Vector2Int nextPosition = new Vector2Int(
            (currentPosition.x == 0) ? 0 : (currentPosition.x > 0) ? currentPosition.x - 1 : currentPosition.x + 1,
            (currentPosition.y == 0) ? 0 : (currentPosition.y > 0) ? currentPosition.y - 1 : currentPosition.y + 1
        );
        if (gridInfo.ContainsKey(nextPosition))
        {
            currentPosition = nextPosition;
            CheckAdjacentGrids();
        }
    }
}
