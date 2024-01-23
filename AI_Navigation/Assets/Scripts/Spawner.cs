using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<AnimalSettings> animals;
    [SerializeField] private Terrain _terrain;
    private float _terrainWidth;
    private float _terrainLength;

    private void Start()
    {
        // _terrain = Terrain.activeTerrain;

        if (_terrain)
        {
            _terrainWidth = _terrain.terrainData.size.x;
            _terrainLength = _terrain.terrainData.size.z;
            
            SpawnAnimals();
        }
    }

    private void SpawnAnimals()
    {
        foreach (var animal in animals)
        {
            for (int i = 0; i < animal.spawnCount; i++)
            {
                var spawnPosition = GetRandomSpawnPosition();
                Instantiate(animal.animalPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var x = Random.Range(0, _terrainWidth);
        var z = Random.Range(0, _terrainLength);
        var y = _terrain.SampleHeight(new Vector3(x, 0, z));
        
        return new Vector3(x, y, z);
    }
    
    
} // class

[Serializable]
public class AnimalSettings
{
    public GameObject animalPrefab;
    public int spawnCount = 10;
}
