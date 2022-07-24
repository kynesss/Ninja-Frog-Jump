using FrogNinja.Platforms;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<PlatformConfigurationScript> platformConfigurations;

    [SerializeField] private Transform playerSpawnPosition;

    [SerializeField] private float minYOffsetRange;
    [SerializeField] private float maxYOffsetRange;

    private List<BasePlatform> spawnedPlatforms = new List<BasePlatform>();
    private float lastSpawnedY;
    private Camera mainCamera;
    private BasePlatform lastSpawnedPlatform;
    private PlatformConfigurationScript lastConfiguration;

    public Vector3 SpawnPosition => playerSpawnPosition.position;

    public static LevelGenerator Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        mainCamera = Camera.main;

        CreateStartLevel();

        EventManager.EnterGameplay += EventManager_EnterGameplay;
    }
    private void OnDestroy()
    {
        EventManager.EnterGameplay -= EventManager_EnterGameplay;
        EventManager.PlayerPositionUpdate -= EventManager_PlayerPositionUpdate;
        EventManager.PlayerDied -= EventManager_PlayerFallenOff;
    }
    private void EventManager_EnterGameplay()
    {
        EventManager.PlayerPositionUpdate += EventManager_PlayerPositionUpdate;
        EventManager.PlayerDied += EventManager_PlayerFallenOff;
    }

    private void EventManager_PlayerFallenOff()
    {
        EventManager.PlayerPositionUpdate -= EventManager_PlayerPositionUpdate;
        EventManager.PlayerDied -= EventManager_PlayerFallenOff;
    }

    private void EventManager_PlayerPositionUpdate(Vector3 obj)
    {
        if (obj.y > lastSpawnedY / 2f)
        {
            SpawnPlatform();
        }
    }

    private void CreateStartLevel()
    {
        lastSpawnedY = playerSpawnPosition.position.y;

        for (int i = 0; i < 10; i++)
        {
            SpawnPlatform();
        }
    }
    private void SpawnPlatform()
    {
        PlatformConfigurationScript configToUse = platformConfigurations[Random.Range(0, platformConfigurations.Count)];

        if (lastConfiguration == configToUse)
        {
            configToUse = platformConfigurations.Where(x => x != configToUse).FirstOrDefault();
        }

        Vector3 spawnPosition = new Vector3(GetRandomXPosition(), lastSpawnedY + Random.Range(minYOffsetRange, maxYOffsetRange) + configToUse.defaultYIncrease, 0f);

        BasePlatform platformToSpawn = configToUse.GetRandomPlatform();

        if (lastSpawnedPlatform == platformToSpawn && configToUse.platforms.Count > 1)
        {
            platformToSpawn = configToUse.GetDifferentPlatform(platformToSpawn);
        }

        BasePlatform newPlatform = Instantiate(platformToSpawn, spawnPosition, Quaternion.identity, transform);
        spawnedPlatforms.Add(newPlatform);

        lastSpawnedY = spawnPosition.y;
        lastSpawnedPlatform = newPlatform;
        lastConfiguration = configToUse;
    }
    private float GetRandomXPosition()
    {
        float randomValue = Random.value;

        Vector3 resultPosition = mainCamera.ViewportToWorldPoint(new Vector3(randomValue, 0f, 0f));

        return resultPosition.x;
    }
    public void RestartLevel()
    {
        if (spawnedPlatforms.Count > 0)
        {
            foreach (var platform in spawnedPlatforms)
            {
                Destroy(platform);
            }

            spawnedPlatforms.Clear();
        }

        CreateStartLevel();
    }
}
