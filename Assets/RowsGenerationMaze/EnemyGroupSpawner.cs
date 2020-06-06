using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupSpawner : MonoBehaviour
{
    public Enemy EnemyPrefab;
    public Vector2 SpawnRange;
    Queue<Enemy> Enemies=new Queue<Enemy>();
   
    Coroutine spawnCoroutine;

    Transform detectedTarget;
    private void Start()
    {
        
        CreateEnemiesQueue(this, EnemyPrefab, 10);

    }

   void onEnemyHide(Enemy enemy)
   {
        enemy.gameObject.SetActive(false);
        Enemies.Enqueue(enemy);
   }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            detectedTarget = collision.transform;
            spawnCoroutine = StartCoroutine(Spawning());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            detectedTarget =null;
            StopCoroutine(spawnCoroutine);
        }
    }


    IEnumerator Spawning()
    {
        while (true)
        {
            if (Enemies.Count != 0)
            {

                Enemy e = Enemies.Dequeue();
                e.gameObject.SetActive(true);
                e.Target = detectedTarget;
            }

            yield return new WaitForSeconds(Random.Range(SpawnRange.x, SpawnRange.y));
        }
    }

    static void CreateEnemiesQueue(EnemyGroupSpawner parent, Enemy enemyPrefab, int count)
    {

        for (int i = 0; i < count; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab);
            enemy.OnDied += parent.onEnemyHide;
            enemy.OnReturnedToSpawn += parent.onEnemyHide;
            enemy.StartPoint = parent.gameObject;
            enemy.transform.SetParent(parent.transform);
            enemy.gameObject.SetActive(false);
            parent.Enemies.Enqueue(enemy); 
        }


    }
}
