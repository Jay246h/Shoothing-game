using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public GameObject[] EnemyObjects;
    public Transform[] SpawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject GameOverSet;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;
    void Awake()
    {
        spawnList = new List<Spawn>();
        ReadSpawnFile();
    }
    void ReadSpawnFile()
    {
        //#1. 변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2. 리스폰 파일 읽기
        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if (line == null)
                break;

            //#3. 리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }
        //#4. 텍스트 파일 닫기
        stringReader.Close();

        //#5. 첫번째 스폰 딜레이 적용
        nextSpawnDelay = spawnList[0].delay;

    }
    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > nextSpawnDelay)
        {
            SpawnEnemy();
            nextSpawnDelay = Random.Range(0.5f, 3.0f);
            curSpawnDelay = 0;
        }
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = Instantiate(EnemyObjects[ranEnemy],
                    SpawnPoints[ranPoint].position,
                    SpawnPoints[ranPoint].rotation);
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        if (ranPoint == 5 || ranPoint == 6)
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -2);
        }
        else if (ranPoint == 7 || ranPoint == 8)
        {
            enemy.transform.Rotate(Vector3.forward * 90);

            rigid.velocity = new Vector2(enemyLogic.speed, -2);
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }
    }
    public void UpdateLifeIcon(int life)
    {
        for (int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }
        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }
    public void UpdateBoomIcon(int boom)
    {
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0);
        }
        for (int index = 0; index < boom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);
        }
    }
    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }
    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }
    public void gameOver()
    {
        GameOverSet.SetActive(true);
    }
    public void GameRetry()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
