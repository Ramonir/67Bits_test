using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject Enemy;
    [SerializeField] private Vector2 spawnTime;
    [SerializeField] private Vector2 spawnPos;
    [SerializeField] private int enemyLimit;
    private int enemyCount;
    

    private void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy(){
        StartCoroutine(CoroutineSpawnEnemy());
    }

    IEnumerator CoroutineSpawnEnemy()
    {
        if (enemyCount < enemyLimit) { // Caso tenha menos inimigos que o limite
            Spawn(Enemy);
            AddEnemyCount(1);
        }
        yield return new WaitForSeconds(Random.Range(spawnTime.x, spawnTime.y));
        SpawnEnemy();
    }

    // Update contador de inimigos
    public void AddEnemyCount(int value){
        enemyCount = enemyCount + value;
    }

    void Spawn(GameObject obj)
    {
        MoveSpawner();
        var go = GameManager.Instance.ObjectPooler.SpawnFromPool(obj, transform.position, transform.rotation);
    }

    // Move o Spawner para a unidade ser gerada
    void MoveSpawner()
    {
        var newPos = transform.position;

        var variant = Random.Range(spawnPos.x, spawnPos.y);
        newPos[0] = variant;
        variant = Random.Range(spawnPos.x, spawnPos.y);
        newPos[2] = variant;
        
        transform.position = newPos;
    }

}
