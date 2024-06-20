using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject wumpusPrefab;
    public GameObject goldPrefab;
    public GameObject pitPrefab;
    public int pitCount = 3;

    void Start()
    {
        PlaceGameElement(playerPrefab, new Vector3(0, 0, -1)); // Z�� ��ǥ -1�� ����
        PlaceGameElement(wumpusPrefab, -1); // Z�� ��ǥ -1�� ����
        PlaceGameElement(goldPrefab, -1); // Z�� ��ǥ -1�� ����
        for (int i = 0; i < pitCount; i++)
        {
            PlaceGameElement(pitPrefab, -1); // Z�� ��ǥ -1�� ����
        }
    }

    void PlaceGameElement(GameObject elementPrefab, float z)
    {
        int x, y;
        do
        {
            x = Random.Range(0, 4);
            y = Random.Range(0, 4);
        } while (x == 0 && y == 0); // �÷��̾� ��ġ�� ��ġ�� �ʵ��� ��

        GameObject element = Instantiate(elementPrefab, new Vector3(x, y, z), Quaternion.identity);
        element.GetComponent<SpriteRenderer>().sortingLayerName = "Objects"; // Sorting Layer ����
        element.GetComponent<SpriteRenderer>().sortingOrder = 1; // Order in Layer ����
    }

    void PlaceGameElement(GameObject elementPrefab, Vector3 position)
    {
        GameObject element = Instantiate(elementPrefab, position, Quaternion.identity);
        element.GetComponent<SpriteRenderer>().sortingLayerName = "Player"; // Sorting Layer ����
        element.GetComponent<SpriteRenderer>().sortingOrder = 1; // Order in Layer ����
    }
}
