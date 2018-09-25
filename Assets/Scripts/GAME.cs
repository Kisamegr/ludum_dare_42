using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class GAME : MonoBehaviour
{


    public float countdownTimer = 3;
    public float waveCountdownTimer = 1;
    public bool wallsClosing = true;

    public Vector2 initialMapSize = new Vector2(25, 25);
    public Vector2 minMapSize = new Vector2(20, 20);
    public Vector2 mapSize = new Vector2(20, 20);
    public float reducePerLevel = 0.4f;

    public float wallThickness = 10;
    public float wallSpeed = 0.5f;
    public float wallLerpSpeed = 1;
    public float pickupDropChance = 0.3f;
    public PickupTable pickupTable;

    public List<LevelObject> levels;
    private bool isGameover;
    private int score;


    private Transform wallParent;
    private MapBorder[] mapBorders;  // Left, Top, Right, Bottom

    private Camera mainCamera;
    private Transform player;

    private int currentLevel = 0;
    private int currentWave = 0;
    private float lastLevelLoadTime;

    private Queue<EnemySpawnEvent> remainingSpawnEvents;

    bool startedNextWave = false;

    public enum WallType
    {
        Left, Top, Right, Bottom
    }

    private UiManager ui_manager;

    private static GAME _instance;

    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }
    }

    public static GAME Instance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
        mapSize.x = initialMapSize.x;
        mapSize.y = initialMapSize.y;
    }
    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        remainingSpawnEvents = new Queue<EnemySpawnEvent>();

        wallParent = GameObject.Find("Walls").transform;

        float showMargin = 2f;
        wallParent.GetComponent<PolygonCollider2D>().points = new Vector2[] {
      new Vector2(-mapSize.x / 2 - showMargin, -mapSize.y / 2 - showMargin),
      new Vector2(-mapSize.x / 2 - showMargin, mapSize.y / 2 + showMargin),
      new Vector2(mapSize.x / 2 + showMargin, mapSize.y / 2 + showMargin),
      new Vector2(mapSize.x / 2 + showMargin, -mapSize.y / 2 - showMargin) };

        ResetWalls();

        UiManager.Instance().NextLevel(countdownTimer);
        lastLevelLoadTime = Time.time;

        isGameover = false;
        score = 0;
        ui_manager = UiManager.Instance();



    }

    // Update is called once per frame
    void Update()
    {

        //if (isGameover)
        //  if (Input.anyKeyDown)
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (!isGameover)
        {
            if ((Time.time - lastLevelLoadTime) > countdownTimer && wallsClosing)
            {
                ChangeWallBorders(false, -Time.deltaTime * wallSpeed);
            }

            SetWallPositionsAndScale();

            Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
            if ((Time.time - lastLevelLoadTime) > countdownTimer && enemies.Length == 0)
            {
                if (remainingSpawnEvents.Count == 0)
                {
                    EndLevel();
                }
                else if (!startedNextWave)
                {
                    Invoke("NextWaveSpawn", currentWave == 0 ? 0 : waveCountdownTimer);
                    startedNextWave = true;
                }
            }
        }

        Rect currentMapSize = CurrentMapSize();
        if (currentMapSize.width < 0 || currentMapSize.height < 0)
        {
            GameOver();
        }
    }

    public void EndLevel()
    {
        mapSize.x -= reducePerLevel;
        if (mapSize.x < minMapSize.x)
            mapSize.x = minMapSize.x;

        mapSize.y -= reducePerLevel;
        if (mapSize.y < minMapSize.y)
            mapSize.y = minMapSize.y;


        ResetWalls();
        UiManager.Instance().NextLevel(countdownTimer);
        lastLevelLoadTime = Time.time;
    }

    public void GameOver()
    {
        isGameover = true;
        ui_manager.GameOver();
    }

    public bool IsGameOver()
    {
        return isGameover;
    }

    public void IncreaseScore(int increaseAmount)
    {
        score += increaseAmount;
        ui_manager.UpdateScore(score);
    }

    public void ChangeWallBorders(bool facingPlayer, float amount)
    {
        float angle = player.rotation.eulerAngles.z;

        if (!facingPlayer)
        {
            angle += 180;
        }

        angle *= Mathf.Deg2Rad;
        Vector2 playerDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        var layermask = 1 << LayerMask.NameToLayer("Wall");
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, playerDir, Mathf.Infinity, layermask);
        if (hit.collider != null)
        {
            switch (hit.collider.name)
            {
                case "Left":
                    mapBorders[(int)WallType.Left].BorderPosition += amount;
                    break;
                case "Right":
                    mapBorders[(int)WallType.Right].BorderPosition += amount;
                    break;
                case "Top":
                    mapBorders[(int)WallType.Top].BorderPosition += amount;
                    break;
                case "Bottom":
                    mapBorders[(int)WallType.Bottom].BorderPosition += amount;
                    break;
                default:
                    break;
            }
        }
    }

    private MapBorder ClosestBorder()
    {
        MapBorder closest = mapBorders[0];
        float minDistance = int.MaxValue;
        foreach (MapBorder border in mapBorders)
        {
            float distance = Vector3.Distance(border.Transform.position, player.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = border;
            }
        }

        return closest;
    }

    void SetWallPositionsAndScale()
    {

        for (int i = 0; i < mapBorders.Length; i++)
        {
            float targetScaleX = 1f;
            float targetScaleY = 1f;
            Vector3 targetPosition = Vector3.zero;
            Vector3 localScale = mapBorders[i].Transform.localScale;

            switch (mapBorders[i].Type)
            {
                case WallType.Left:
                    {
                        float targetWidth = mapSize.x / 2 - mapBorders[i].BorderPosition + wallThickness;
                        targetPosition = new Vector3(-mapBorders[i].BorderPosition - targetWidth / 2, 0, 0);
                        targetScaleX = targetWidth / mapBorders[i].Renderer.size.x;
                        targetScaleY = (mapSize.y + wallThickness) / mapBorders[i].Renderer.size.y;
                        break;
                    };
                case WallType.Top:
                    {
                        float targetHeight = mapSize.y / 2 - mapBorders[i].BorderPosition + wallThickness;
                        targetPosition = new Vector3(0, mapBorders[i].BorderPosition + targetHeight / 2, 0);
                        targetScaleX = (mapSize.x + wallThickness) / mapBorders[i].Renderer.size.x;
                        targetScaleY = targetHeight / mapBorders[i].Renderer.size.y;
                        break;
                    }
                case WallType.Right:
                    {
                        float targetWidth = mapSize.x / 2 - mapBorders[i].BorderPosition + wallThickness;
                        targetPosition = new Vector3(mapBorders[i].BorderPosition + targetWidth / 2, 0, 0);
                        targetScaleX = targetWidth / mapBorders[i].Renderer.size.x;
                        targetScaleY = (mapSize.y + wallThickness) / mapBorders[i].Renderer.size.y;
                        break;
                    }
                case WallType.Bottom:
                    {
                        float targetHeight = mapSize.y / 2 - mapBorders[i].BorderPosition + wallThickness;
                        targetPosition = new Vector3(0, -mapBorders[i].BorderPosition - targetHeight / 2, 0);
                        targetScaleX = (mapSize.x + wallThickness) / mapBorders[i].Renderer.size.x;
                        targetScaleY = targetHeight / mapBorders[i].Renderer.size.y;
                        break;
                    }
            }

            Vector3 targetLocalScale = new Vector3(targetScaleX, targetScaleY, localScale.z);

            mapBorders[i].Transform.localScale = Vector3.Lerp(
              localScale,
              targetLocalScale,
              Time.deltaTime);

            mapBorders[i].Transform.position = Vector3.Lerp(
              mapBorders[i].Transform.position,
              targetPosition,
              Time.deltaTime * wallLerpSpeed);
        }
    }

    private void ResetWalls()
    {
        mapBorders = new MapBorder[4];
        mapBorders[0] = new MapBorder(wallParent.Find("Left"), WallType.Left, mapSize.x / 2);
        mapBorders[1] = new MapBorder(wallParent.Find("Top"), WallType.Top, mapSize.y / 2);
        mapBorders[2] = new MapBorder(wallParent.Find("Right"), WallType.Right, mapSize.x / 2);
        mapBorders[3] = new MapBorder(wallParent.Find("Bottom"), WallType.Bottom, mapSize.y / 2);
    }

    public void LoadNextLevel()
    {

        remainingSpawnEvents.Clear();
        currentWave = 0;

        //Generate new level - increase power
        if (CurrentLevel == levels.Count)
        {
            int nextLvl = (levels.Count - 1) - 2;
            LevelObject newLevel = (LevelObject)levels[nextLvl].Clone();
            Debug.Log(newLevel.name);
            foreach (var spawnEvent in newLevel.enemySpawnEvents)
            {
                for (int i = 0; i < spawnEvent.enemySpawns.Length; i++)
                {
          
                    if (spawnEvent.enemySpawns[i].enemy.GetComponent<Enemy>().Type() == EnemyType.Wall)
                    {
                        spawnEvent.enemySpawns[i].count += 1;
                    }
                    else
                    {
                        spawnEvent.enemySpawns[i].count += 2;
                    }
                }
            }
            levels.Add(newLevel);
        }

        LevelObject level = levels[CurrentLevel];
        foreach (EnemySpawnEvent spawnEvent in level.enemySpawnEvents)
            remainingSpawnEvents.Enqueue(spawnEvent);

        currentLevel++;
    }

    void NextWaveSpawn()
    {
        EnemySpawnEvent spawnEvent = remainingSpawnEvents.Dequeue();

        Rect mapSize = CurrentMapSize();

        float safeDist = 7;
        Rect safeArea = new Rect(player.transform.position.x - safeDist, player.transform.position.y - safeDist, safeDist, safeDist);
        int maxTries = 3;

        foreach (EnemySpawn spawn in spawnEvent.enemySpawns)
        {
            for (int i = 0; i < spawn.count; i++)
            {
                Vector3 position = new Vector3(
                  Random.Range(mapSize.xMin, mapSize.xMax),
                  Random.Range(mapSize.yMin, mapSize.yMax),
                  0);
                int tries = 0;
                while (safeArea.Contains(position) && tries < maxTries)
                {
                    position = new Vector3(
                    Random.Range(mapSize.xMin, mapSize.xMax),
                    Random.Range(mapSize.yMin, mapSize.yMax),
                    0);
                    tries += 1;
                }

                Enemy enemy = Instantiate(spawn.enemy, position, Quaternion.identity).GetComponent<Enemy>();
                if (enemy.Type() == EnemyType.Wall)
                {
                    enemy.transform.rotation = Quaternion.Euler(0, 0, Random.value * 360);
                }

                // Give the enemy a pickup to drop
                if (Random.value < pickupDropChance)
                {
                    enemy.pickupDropPrefab = pickupTable.ChoosePickup();
                }

            }
        }
        //Play "wave spawn" sound
        transform.GetChild(0).GetComponents<AudioSource>()[2].Play();

        currentWave++;
        startedNextWave = false;
    }

    public Rect CurrentMapSize()
    {
        return new Rect(
          mapBorders[(int)WallType.Left].Collider.bounds.max.x,
          mapBorders[(int)WallType.Bottom].Collider.bounds.max.y,
          mapBorders[(int)WallType.Right].Collider.bounds.min.x - mapBorders[(int)WallType.Left].Collider.bounds.max.x,
          mapBorders[(int)WallType.Top].Collider.bounds.min.y - mapBorders[(int)WallType.Bottom].Collider.bounds.max.y);

    }

    struct MapBorder
    {
        public Transform Transform { get; private set; }
        public SpriteRenderer Renderer { get; private set; }
        public WallType Type { get; private set; }
        public Collider2D Collider;

        public float BorderPosition
        {
            get { return borderPosition; }

            set
            {
                borderPosition = Mathf.Min(value, maxBorderPosition);
            }
        }

        private float borderPosition;
        private float maxBorderPosition;

        public MapBorder(Transform transform, WallType type, float borderPosition)
        {
            this.Transform = transform;
            this.Type = type;
            this.Renderer = transform.GetComponent<SpriteRenderer>();
            this.borderPosition = borderPosition;
            this.Collider = transform.GetComponent<Collider2D>();
            maxBorderPosition = borderPosition;
        }
    }

    


    public void OnDrawGizmos()
    {


        //left
        Gizmos.DrawLine(new Vector2(-mapSize.x / 2, -mapSize.y / 2),
                        new Vector2(-mapSize.x / 2, mapSize.y / 2));

        Gizmos.DrawLine(new Vector2(mapSize.x / 2, -mapSize.y / 2),
                        new Vector2(mapSize.x / 2, mapSize.y / 2));

        Gizmos.DrawLine(new Vector2(-mapSize.x / 2, -mapSize.y / 2),
                        new Vector2(mapSize.x / 2, -mapSize.y / 2));

        Gizmos.DrawLine(new Vector2(-mapSize.x / 2, mapSize.y / 2),
                        new Vector2(mapSize.x / 2, mapSize.y / 2));


    }
}
