using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GAME : MonoBehaviour {

  public float countdownTimer = 3;
  public bool wallsClosing = true;
  public Vector2 mapSize = new Vector2(20, 20);
  public float wallThickness = 10; 
  public float wallSpeed = 0.5f;
  public float wallLerpSpeed = 1;
  public float pickupDropChance = 0.3f;
  public PickupTable pickupTable;

  public LevelObject[] levels;
  private bool isGameover;
  private int score;


  private Transform wallParent;
  private MapBorder[] mapBorders;  // Left, Top, Right, Bottom

  private Camera mainCamera;
  private Transform player;

  private int currentLevel = 0;
  private float lastLevelLoadTime;

  private Queue<EnemySpawnEvent> remainingSpawnEvents;
  private int remainingEnemies = 0;

  public enum WallType {
    Left, Top, Right, Bottom
  }

  private UiManager ui_manager;

  private static GAME _instance;

  public int CurrentLevel {
    get {
      return currentLevel;
    }
  }

  public static GAME Instance() {
    return _instance;
  }

  private void Awake() {
    _instance = this;
  }
  // Use this for initialization
  void Start() {
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

    LoadNextLevel();

    isGameover = false;
    score = 0;
    ui_manager = UiManager.Instance();
  }

  // Update is called once per frame
  void Update() {

    if (isGameover)
      if (Input.anyKeyDown)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      
    if (!isGameover)
    {
      if ((Time.time - lastLevelLoadTime) > countdownTimer && wallsClosing)
      {
        ChangeWallBorders(false, -Time.deltaTime * wallSpeed);
      }

      SetWallPositionsAndScale();

      if ((Time.time - lastLevelLoadTime) > countdownTimer && remainingEnemies == 0)
      {
        if (remainingSpawnEvents.Count == 0)
          LoadNextLevel();
        else
          NextSpawn();
      }
    }

    Rect currentMapSize = CurrentMapSize();
    if(currentMapSize.width < 0 || currentMapSize.height < 0)
    {
      GameOver();
    }
  }

  public void GameOver()
  {
    isGameover = true;
    ui_manager.GameOver();
  }

  public void IncreaseScore(int increaseAmount)
  {
    score += increaseAmount;
    ui_manager.UpdateScore(score);
  }

  public void ChangeWallBorders(bool facingPlayer, float amount) {
    float angle = player.rotation.eulerAngles.z;
    //if (!facingPlayer) {
    //  angle -= 180;
    //  if (angle < 0)
    //    angle += 360;
    //}

    //if (angle >= 0 && angle < 90) {
    //  mapBorders[(int) WallType.Top].BorderPosition += amount;
    //  mapBorders[(int) WallType.Right].BorderPosition += amount;
    //}
    //else if (angle < 180) {
    //  mapBorders[(int) WallType.Top].BorderPosition += amount;
    //  mapBorders[(int) WallType.Left].BorderPosition += amount;
    //}
    //else if (angle < 270) {
    //  mapBorders[(int) WallType.Left].BorderPosition += amount;
    //  mapBorders[(int) WallType.Bottom].BorderPosition += amount;
    //}
    //else if (angle < 360) {
    //  mapBorders[(int) WallType.Bottom].BorderPosition += amount;
    //  mapBorders[(int) WallType.Right].BorderPosition += amount;
    //}

    if (!facingPlayer)
    {
      angle += 180;
    }

    angle *= Mathf.Deg2Rad;
    Vector2 playerDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)); 
    var layermask = 1 << LayerMask.NameToLayer("Wall"); 
    RaycastHit2D hit = Physics2D.Raycast(player.transform.position, playerDir,Mathf.Infinity, layermask);
    if(hit.collider != null)
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

  private MapBorder ClosestBorder() {
    MapBorder closest = mapBorders[0];
    float minDistance = int.MaxValue;
    foreach (MapBorder border in mapBorders) {
      float distance = Vector3.Distance(border.Transform.position, player.position);
      if (distance < minDistance) {
        minDistance = distance;
        closest = border;
      }
    }

    return closest;
  }

  void SetWallPositionsAndScale() {

    for (int i = 0; i < mapBorders.Length; i++) {
      float targetScaleX = 1f;
      float targetScaleY = 1f;
      Vector3 targetPosition = Vector3.zero;
      Vector3 localScale = mapBorders[i].Transform.localScale;

      switch (mapBorders[i].Type) {
        case WallType.Left: {
            float targetWidth = mapSize.x / 2 - mapBorders[i].BorderPosition + wallThickness;
            targetPosition = new Vector3(-mapBorders[i].BorderPosition - targetWidth / 2, 0, 0);
            targetScaleX = targetWidth / mapBorders[i].Renderer.size.x;
            targetScaleY = (mapSize.y + wallThickness) / mapBorders[i].Renderer.size.y;
            break;
          };
        case WallType.Top: {
            float targetHeight = mapSize.y / 2 - mapBorders[i].BorderPosition + wallThickness;
            targetPosition = new Vector3(0, mapBorders[i].BorderPosition + targetHeight / 2, 0);
            targetScaleX = (mapSize.x + wallThickness) / mapBorders[i].Renderer.size.x;
            targetScaleY = targetHeight / mapBorders[i].Renderer.size.y;
            break;
          }
        case WallType.Right: {
            float targetWidth = mapSize.x / 2 - mapBorders[i].BorderPosition + wallThickness;
            targetPosition = new Vector3(mapBorders[i].BorderPosition + targetWidth / 2, 0, 0);
            targetScaleX = targetWidth / mapBorders[i].Renderer.size.x;
            targetScaleY = (mapSize.y + wallThickness) / mapBorders[i].Renderer.size.y;
            break;
          }
        case WallType.Bottom: {
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

  void LoadNextLevel() {
    mapBorders = new MapBorder[4];
    mapBorders[0] = new MapBorder(wallParent.Find("Left"), WallType.Left, mapSize.x / 2);
    mapBorders[1] = new MapBorder(wallParent.Find("Top"), WallType.Top, mapSize.y / 2);
    mapBorders[2] = new MapBorder(wallParent.Find("Right"), WallType.Right, mapSize.x / 2);
    mapBorders[3] = new MapBorder(wallParent.Find("Bottom"), WallType.Bottom, mapSize.y / 2);

    lastLevelLoadTime = Time.time;

    remainingSpawnEvents.Clear();

    if (CurrentLevel < levels.Length) {
      LevelObject level = levels[CurrentLevel];
      foreach (EnemySpawnEvent spawnEvent in level.enemySpawnEvents)
        remainingSpawnEvents.Enqueue(spawnEvent);

      currentLevel++;
    }
  }

  void NextSpawn() {
    EnemySpawnEvent spawnEvent = remainingSpawnEvents.Dequeue();

    Rect mapSize = CurrentMapSize();

    foreach (EnemySpawn spawn in spawnEvent.enemySpawns) {
      for (int i = 0; i<spawn.count; i++) {
        Vector3 position = new Vector3(
          Random.Range(mapSize.xMin, mapSize.xMax),
          Random.Range(mapSize.yMin, mapSize.yMax),
          0);

        Enemy enemy = Instantiate(spawn.enemy, position, Quaternion.identity).GetComponent<Enemy>();

        // Give the enemy a pickup to drop
        if(Random.value < pickupDropChance) {
          enemy.pickupDropPrefab = pickupTable.ChoosePickup();
        }

        remainingEnemies++;
      }
    }
  }

  public void EnemyDied() {
    remainingEnemies--;
  }

  public Rect CurrentMapSize() {
    return new Rect(
      -mapBorders[(int) WallType.Left].BorderPosition,
      -mapBorders[(int) WallType.Bottom].BorderPosition,
      mapBorders[(int) WallType.Left].BorderPosition + mapBorders[(int) WallType.Right].BorderPosition,
      mapBorders[(int) WallType.Top].BorderPosition + mapBorders[(int) WallType.Bottom].BorderPosition
      );
  }
 
  struct MapBorder {
    public Transform Transform { get; private set; }
    public SpriteRenderer Renderer { get; private set; }
    public WallType Type { get; private set; }

    public float BorderPosition {
      get { return borderPosition; }

      set {
        borderPosition = Mathf.Min(value, maxBorderPosition);
      }
    }

    private float borderPosition;
    private float maxBorderPosition;

    public MapBorder(Transform transform, WallType type, float borderPosition) {
      this.Transform = transform;
      this.Type = type;
      this.Renderer = transform.GetComponent<SpriteRenderer>();
      this.borderPosition = borderPosition;
      maxBorderPosition = borderPosition;
    }
  }


  public void OnDrawGizmos() {


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
