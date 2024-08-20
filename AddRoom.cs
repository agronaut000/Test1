using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRoom : MonoBehaviour
{

    [Header("Walls")]
    public GameObject[] doors;
    public GameObject wallEffect;
    public GameObject closedDoor;

    [Header("Enemies")]
    public GameObject[] enemyTypes;
    public Transform[] enemySpawners;

    [Header("Powerups")]
    public GameObject shield;
    public GameObject healthPotion;

    [HideInInspector] public List<GameObject> enemies;

    private RoomVariants variants;
    private bool spawned;
    private bool wallsDestroyed;

    private void Awake()
    {
        variants = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomVariants>();
    }
    private void Start()
    {
        variants.rooms.Add(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !spawned)
        {
            spawned = true;
            foreach (Transform spawner in enemySpawners)
            {
                int rand = Random.Range(0, 11);
                if (rand < 9)
                {
                    GameObject enemyType = enemyTypes[Random.Range(0, enemyTypes.Length)];
                    GameObject enemy = Instantiate(enemyType, spawner.position, Quaternion.identity) as GameObject;
                    enemy.transform.parent = transform;
                    enemies.Add(enemy);
                }
                else if (rand == 9)
                {
                    Instantiate(healthPotion, spawner.position, Quaternion.identity);
                }
                else if (rand == 10)
                {
                    Instantiate(shield, spawner.position, Quaternion.identity);
                }
            }
            StartCoroutine(CheckEnemies());
        }
        else if(other.CompareTag("Player") && spawned)
        {
            foreach(GameObject enemy in enemies)
            {
                enemy.GetComponent<Enemy>().playerNotInRoom = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spawned)
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<Enemy>().playerNotInRoom = true;
            }
        }
    }

    IEnumerator CheckEnemies()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => enemies.Count == 0);
        DestroyDoors();
    }

    public void DestroyDoors()
    {
        foreach (GameObject door in doors)
        {
            if (door != null && door.transform.childCount != 0)
            {
                Instantiate(wallEffect, door.transform.position, Quaternion.identity);
                Destroy(door);
            }
        }
        wallsDestroyed = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (wallsDestroyed && other.CompareTag("Door"))
        {
            Destroy(other.gameObject);
        }
    }
}
