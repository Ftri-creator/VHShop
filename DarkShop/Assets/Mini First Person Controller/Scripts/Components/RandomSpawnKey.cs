using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnKey : MonoBehaviour
{
    public List<Transform> spawnPoints; // ������ ����� ������
    private int lastSpawnIndex = -1; // ������ ��������� ����� ������

    void Start()
    {
        SpawnObjectRandomly();
    }

    void SpawnObjectRandomly()
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);

        // ���������, ����� ������ �� ����������� � ���������� ����� ����� ��������
        while (randomIndex == lastSpawnIndex)
        {
            randomIndex = Random.Range(0, spawnPoints.Count);
        }

        lastSpawnIndex = randomIndex; // ��������� ������ ��������� ����� ������

        transform.position = spawnPoints[randomIndex].position; // ������������� ������� ������� �� �������� ��������� �����
    }

    // �����, ������� ����� ���� ������ ����� ��� ���������� �������
    public void RespawnObject()
    {
        SpawnObjectRandomly();
    }
}
