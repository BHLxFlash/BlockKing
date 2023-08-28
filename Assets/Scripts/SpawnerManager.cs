using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public float beatTempo;
    public bool isStarted = false;
    private bool ongoingGame = true;
    private Coroutine spawningEnemyCoroutine;

    private GameObject enemyNorth;
    private GameObject enemySouth;
    private GameObject enemyEast;
    private GameObject enemyWest;

    [SerializeField] GameObject enemy;
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject[] deathZones;

    public float timeToReachDeathZone = 3f;


    // Start is called before the first frame update
    void Start()
    {
        //spawningEnemyCoroutine = StartCoroutine(SpawningEnemy(beatTempo));
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SpawnEnemyRandom()
    {
        int intSpawner = Random.Range(0,4);
        switch (intSpawner)
        {
            case 0:
                //North
                enemyNorth = Instantiate(enemy, spawnPoints[0].transform);
                enemyNorth.GetComponent<EnemyBehavior>().SetTargetPosition(deathZones[0].transform.position);
                enemyNorth.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.North;
                enemyNorth.GetComponent<EnemyBehavior>().TimeToReachTarget = timeToReachDeathZone;
                enemyNorth.transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
            case 1:
                //East
                enemyEast = Instantiate(enemy, spawnPoints[1].transform);
                enemyEast.GetComponent<EnemyBehavior>().SetTargetPosition(deathZones[1].transform.position);
                enemyEast.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.East;
                enemyEast.GetComponent<EnemyBehavior>().TimeToReachTarget = timeToReachDeathZone;
                enemyEast.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                //South
                enemySouth = Instantiate(enemy, spawnPoints[2].transform);
                enemySouth.GetComponent<EnemyBehavior>().SetTargetPosition(deathZones[2].transform.position);
                enemySouth.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.South;
                enemySouth.GetComponent<EnemyBehavior>().TimeToReachTarget = timeToReachDeathZone;
                enemySouth.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case 3:
                //West
                enemyWest = Instantiate(enemy, spawnPoints[3].transform);
                enemyWest.GetComponent<EnemyBehavior>().SetTargetPosition(deathZones[3].transform.position);
                enemyWest.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.West;
                enemyWest.GetComponent<EnemyBehavior>().TimeToReachTarget = timeToReachDeathZone;
                enemyWest.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
        }
    }

    IEnumerator SpawningEnemy(float beat)
    {
        
        float beatDivided = beat / 60f;
        while (ongoingGame)
        {
            int intSpawner = Random.Range(0, 7);
            Debug.Log("intSpawner : " + intSpawner);
            switch (intSpawner)
            {
                case 0:
                    //North
                    enemyNorth = Instantiate(enemy, spawnPoints[0].transform);
                    enemyNorth.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.North;
                    enemyNorth.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    break;
                case 1:
                    //South
                    enemySouth = Instantiate(enemy, spawnPoints[1].transform);
                    enemySouth.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.South;
                    enemySouth.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    break;
                case 2:
                    //East
                    enemyEast = Instantiate(enemy, spawnPoints[2].transform);
                    enemyEast.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.East;
                    enemyEast.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    break;
                case 3:
                    //West
                    enemyWest = Instantiate(enemy, spawnPoints[3].transform);
                    enemyWest.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.West;
                    enemyWest.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    break;

                case 4:
                    //North East
                    enemyNorth = Instantiate(enemy, spawnPoints[0].transform);
                    enemyNorth.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.North;
                    enemyNorth.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    enemyEast = Instantiate(enemy, spawnPoints[2].transform);
                    enemyEast.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.East;
                    enemyEast.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    break;
                case 5:
                    //South East
                    enemySouth = Instantiate(enemy, spawnPoints[1].transform);
                    enemySouth.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.South;
                    enemySouth.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    enemyEast = Instantiate(enemy, spawnPoints[2].transform);
                    enemyEast.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.East;
                    enemyEast.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    break;
                case 6:
                    //North West
                    enemyNorth = Instantiate(enemy, spawnPoints[0].transform);
                    enemyNorth.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.North;
                    enemyNorth.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    enemyWest = Instantiate(enemy, spawnPoints[3].transform);
                    enemyWest.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.West;
                    enemyWest.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    break;
                case 7:
                    //South West
                    enemySouth = Instantiate(enemy, spawnPoints[1].transform);
                    enemySouth.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.South;
                    enemySouth.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    enemyWest = Instantiate(enemy, spawnPoints[3].transform);
                    enemyWest.GetComponent<EnemyBehavior>().ComingFrom = EnemyBehavior.Direction.West;
                    enemyWest.GetComponent<EnemyBehavior>().BeatTempo = beatDivided;
                    break;
            }
            yield return new WaitForSeconds(beatDivided);

        }
    }
}
