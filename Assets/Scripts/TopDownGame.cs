using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownGame : MonoBehaviour {

  public bool wallsClosing = true;
  public Vector2 mapSize = new Vector2(20, 20);
  public float wallThickness = 10;
  public float countdown;
  public float maxCountdown;
  public float wallSpeed = 0.5f;
  public float wallLerpSpeed = 1;

  private MapBorder[] mapBorders;  // Left, Top, Right, Bottom

  private Camera mainCamera;
  private Transform player;

  public enum WallType {
    Left, Top, Right, Bottom
  }

  // Use this for initialization
  void Start() {
    mainCamera = Camera.main;
    player = GameObject.FindGameObjectWithTag("Player").transform;

    countdown = 100;
    maxCountdown = 100;

    Vector2 screenWorldSize = ScreenWorldSize();
    Transform wallParent = GameObject.Find("Walls").transform;

    mapBorders = new MapBorder[4];
    mapBorders[0] = new MapBorder(wallParent.Find("Left"), WallType.Left, mapSize.x / 2);
    mapBorders[1] = new MapBorder(wallParent.Find("Top"), WallType.Top, mapSize.y / 2);
    mapBorders[2] = new MapBorder(wallParent.Find("Right"), WallType.Right, mapSize.x / 2);
    mapBorders[3] = new MapBorder(wallParent.Find("Bottom"), WallType.Bottom, mapSize.y / 2);


    wallParent.GetComponent<PolygonCollider2D>().points = new Vector2[] {
      new Vector2(-mapSize.x / 2, -mapSize.y / 2),
      new Vector2(-mapSize.x / 2, mapSize.y / 2),
      new Vector2(mapSize.x / 2, mapSize.y / 2),
      new Vector2(mapSize.x / 2, -mapSize.y / 2) };
  }

  // Update is called once per frame
  void Update() {
    if (wallsClosing)
      ChangeWallBorders(true, -Time.deltaTime * wallSpeed);

    if (Input.GetMouseButton(1))
      ChangeWallBorders(true, 10);

    SetWallPositionsAndScale();
  }

  public void ChangeWallBorders(bool facingPlayer, float amount) {
    float angle = player.rotation.eulerAngles.z;
    if (!facingPlayer) {
      angle -= 180;
      if (angle < 0)
        angle += 360;
    }

    if (angle >= 0 && angle < 90) {
      mapBorders[(int) WallType.Top].BorderPosition += amount;
      mapBorders[(int) WallType.Right].BorderPosition += amount;
    }
    else if (angle < 180) {
      mapBorders[(int) WallType.Top].BorderPosition += amount;
      mapBorders[(int) WallType.Left].BorderPosition += amount;
    }
    else if (angle < 270) {
      mapBorders[(int) WallType.Left].BorderPosition += amount;
      mapBorders[(int) WallType.Bottom].BorderPosition += amount;
    }
    else if (angle < 360) {
      mapBorders[(int) WallType.Bottom].BorderPosition += amount;
      mapBorders[(int) WallType.Right].BorderPosition += amount;
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

  public Rect MapSize() {
    return new Rect(
      mapBorders[(int) WallType.Left].BorderPosition - mapBorders[(int) WallType.Right].BorderPosition,
      mapBorders[(int) WallType.Top].BorderPosition - mapBorders[(int) WallType.Bottom].BorderPosition,
      mapBorders[(int) WallType.Left].BorderPosition + mapBorders[(int) WallType.Right].BorderPosition,
      mapBorders[(int) WallType.Top].BorderPosition + mapBorders[(int) WallType.Bottom].BorderPosition
      );
  }

  private Vector2 ScreenWorldSize() {
    Vector2 size;
    size.y = Camera.main.orthographicSize * 2.0f;
    size.x = size.y * Camera.main.aspect;
    return size;
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
