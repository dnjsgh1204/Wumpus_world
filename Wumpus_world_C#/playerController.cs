using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // ���� ����
    private Dictionary<Vector2Int, string> gridInfo = new Dictionary<Vector2Int, string>();

    // ���� ��ġ
    private Vector2Int currentPosition = new Vector2Int(0, 0);

    // ������
    public GameObject glitterPrefab;
    public GameObject breezePrefab;
    public GameObject stenchPrefab;

    // �ݵ��̸� ������ �ִ��� ����
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

            // �÷��̾���� �Ÿ��� 1�̰�, �ش� ���ڿ� ������ ���� ���� ������
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
                // �ݵ��̰� ���� ��� ���� ��ġ�� �̵�
                MoveToNextGrid();
            }
            else if (currentPosition != Vector2Int.zero)
            {
                // �ݵ��̸� ã�Ұ� ���� ���� ��ġ�� ���ƿ��� ���� ���, ���� ��ġ�� �̵�
                MoveToOrigin();
            }
            else
            {
                // �ݵ��̸� ã�Ұ� ���� ��ġ�� ���ƿ� ��� ����
                Debug.Log("Gold found! Back to origin!");
                yield break;
            }
            yield return new WaitForSeconds(0.5f); // 0.5�ʸ��� ������
        }
    }

    void MoveToNextGrid()
    {
        // �����ϰ� ���� ��ġ�� �̵�
        Vector2Int nextPosition = currentPosition + new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
        if (gridInfo.ContainsKey(nextPosition))
        {
            currentPosition = nextPosition;
            CheckAdjacentGrids();
        }
    }

    void MoveToOrigin()
    {
        // ���� ��ġ�� �̵�
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
