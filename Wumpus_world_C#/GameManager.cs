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
        PlaceGameElement(playerPrefab, new Vector3(0, 0, -1)); // Z축 좌표 -1로 설정
        PlaceGameElement(wumpusPrefab, -1); // Z축 좌표 -1로 설정
        PlaceGameElement(goldPrefab, -1); // Z축 좌표 -1로 설정
        for (int i = 0; i < pitCount; i++)
        {
            PlaceGameElement(pitPrefab, -1); // Z축 좌표 -1로 설정
        }
    }

    void PlaceGameElement(GameObject elementPrefab, float z)
    {
        int x, y;
        do
        {
            x = Random.Range(0, 4);
            y = Random.Range(0, 4);
        } while (x == 0 && y == 0); // 플레이어 위치와 겹치지 않도록 함

        GameObject element = Instantiate(elementPrefab, new Vector3(x, y, z), Quaternion.identity);
        element.GetComponent<SpriteRenderer>().sortingLayerName = "Objects"; // Sorting Layer 설정
        element.GetComponent<SpriteRenderer>().sortingOrder = 1; // Order in Layer 설정
    }

    void PlaceGameElement(GameObject elementPrefab, Vector3 position)
    {
        GameObject element = Instantiate(elementPrefab, position, Quaternion.identity);
        element.GetComponent<SpriteRenderer>().sortingLayerName = "Player"; // Sorting Layer 설정
        element.GetComponent<SpriteRenderer>().sortingOrder = 1; // Order in Layer 설정
    }
}
