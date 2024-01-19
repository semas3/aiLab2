using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridedDistributor : MonoBehaviour
{
    public GameObject prefab;
    public Vector2 size;
    public Vector2Int count;
    public Color gizmoColor = Color.red;

    public GameObject[] prefabsPool = new GameObject[0];
    private readonly HashSet<Vector2> spawned = new();

    void Start()
    {
    }
    private void ResizePrefabsPool(int newSize)
    {
        if (newSize < (prefabsPool?.Length ?? 0)) return;
        var oldPool = prefabsPool;
        prefabsPool = new GameObject[newSize];
        oldPool?.CopyTo(prefabsPool, 0);
        for (int i = oldPool?.Length ?? 0; i < newSize; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            prefabsPool[i] = obj;
        }
    }
    public ReadOnlySpan<GameObject> Respawn(int countItems)
    {
        if (countItems > count.x * count.y)
            throw new ArgumentException($"countItems shoud be less than grid area, actual: {countItems} > {count.x} * {count.y}");
        ResizePrefabsPool(countItems);
        var oldActive = spawned.Count;
        spawned.Clear();
        for (var i = 0; i < countItems; i++)
        {
            var gridPoint = randomSpawnPoint();
            while (spawned.Contains(gridPoint)) gridPoint = randomSpawnPoint();
            spawned.Add(gridPoint);
            var item = prefabsPool[i];
            item.transform.localPosition = gridToCoords(gridPoint);
            item.SetActive(true);
        }
        for (var i = countItems; i < oldActive; i++)
            prefabsPool[i].SetActive(false);
        return prefabsPool.AsSpan(0, countItems);
    }
    private Vector2Int randomSpawnPoint()
    {
        return new Vector2Int(Random.Range(0, count.x), Random.Range(0, count.y));
    }
    private Vector3 gridToCoords(Vector2Int v)
    {
        var stepX = count.x > 1 ? size.x / (count.x - 1) : 0;
        var stepY = count.y > 1 ? size.y / (count.y - 1) : 0;
        var shiftX = count.x > 1 ? -size.x / 2 : 0;
        var shiftY = count.y > 1 ? -size.y / 2 : 0;
        return new Vector3(v.x * stepX + shiftX, prefab.transform.localPosition.y, v.y * stepY + shiftY);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        var center = transform.position;
        for (var x = 0; x < count.x; x++)
            for (var y = 0; y < count.y; y++)
                Gizmos.DrawWireCube(
                    center + gridToCoords(new Vector2Int(x, y)),
                    prefab.transform.localScale
                );
    }
}
