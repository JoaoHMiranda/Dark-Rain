using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("O Que Spawnar")]
    public GameObject enemyPrefab; 

    [Header("Configurações de Spawn")]
    public float spawnRadius = 15f; 
    public float spawnRate = 1.0f;  

    private float nextSpawnTime;

    // --- MUDANÇA AQUI ---
    void Start()
    {
        // Já cria o primeiro inimigo assim que o jogo abre
        SpawnEnemy();
        
        // Define que o próximo só vem depois do tempo configurado
        nextSpawnTime = Time.time + spawnRate;
    }
    // --------------------

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnEnemy()
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y) * spawnRadius;
        spawnPosition.y = 1f; 

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta; 
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}