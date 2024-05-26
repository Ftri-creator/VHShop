using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnKey : MonoBehaviour
{
    public List<Transform> spawnPoints; // Список точек спавна
    private int lastSpawnIndex = -1; // Индекс последней точки спавна

    void Start()
    {
        SpawnObjectRandomly();
    }

    void SpawnObjectRandomly()
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);

        // Проверяем, чтобы объект не заспавнился в предыдущей точке после рестарта
        while (randomIndex == lastSpawnIndex)
        {
            randomIndex = Random.Range(0, spawnPoints.Count);
        }

        lastSpawnIndex = randomIndex; // Обновляем индекс последней точки спавна

        transform.position = spawnPoints[randomIndex].position; // Устанавливаем позицию объекта на случайно выбранную точку
    }

    // Метод, который может быть вызван извне для переспавна объекта
    public void RespawnObject()
    {
        SpawnObjectRandomly();
    }
}
