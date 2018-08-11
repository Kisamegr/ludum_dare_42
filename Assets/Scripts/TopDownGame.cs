using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownGame : MonoBehaviour {

  public float countdown;
  public float maxCountdown;
  public float wallSpeed = 0.5f;

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
    mapBorders[0] = new MapBorder(wallParent.Find("Left"), WallType.Left, screenWorldSize.x/2);
    mapBorders[1] = new MapBorder(wallParent.Find("Top"), WallType.Top, screenWorldSize.y/2);
    mapBorders[2] = new MapBorder(wallParent.Find("Right"), WallType.Right, screenWorldSize.x/2);
    mapBorders[3] = new MapBorder(wallParent.Find("Bottom"), WallType.Bottom, screenWorldSize.y/2);
  }

  // Update is called once per frame
  void Update() {
    ChangeWallBorders(true, Time.deltaTime * wallSpeed);
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
      mapBorders[(int) WallType.Top].BorderPosition -= amount;
      mapBorders[(int) WallType.Right].BorderPosition -= amount;
    }
    else if (angle < 180) {
      mapBorders[(int) WallType.Top].BorderPosition -= amount;
      mapBorders[(int) WallType.Left].BorderPosition -= amount;
    }
    else if (angle < 270) {
      mapBorders[(int) WallType.Left].BorderPosition -= amount;
      mapBorders[(int) WallType.Bottom].BorderPosition -= amount;
    }
    else if (angle < 360) {
      mapBorders[(int) WallType.Bottom].BorderPosition -= amount;
      mapBorders[(int) WallType.Right].BorderPosition -= amount;
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
    Vector3 screenWorldSize = ScreenWorldSize();

    for (int i = 0; i<mapBorders.Length; i++) {
      float targetScaleX = 1f;
      float targetScaleY = 1f;
      Vector3 targetPosition = Vector3.zero;
      Vector3 localScale = mapBorders[i].Transform.localScale;

      switch (mapBorders[i].Type) {
        case WallType.Left: {
            float targetWidth = screenWorldSize.x/2 - mapBorders[i].BorderPosition;
            targetPosition = new Vector3(-mapBorders[i].BorderPosition - targetWidth/2, 0, 0);
            targetScaleX = targetWidth / mapBorders[i].Renderer.size.x;
            targetScaleY = screenWorldSize.y / mapBorders[i].Renderer.size.y;
            break;
          };
        case WallType.Top: {
            float targetHeight = screenWorldSize.y/2 - mapBorders[i].BorderPosition;
            targetPosition = new Vector3(0, mapBorders[i].BorderPosition + targetHeight/2, 0);
            targetScaleX = screenWorldSize.x / mapBorders[i].Renderer.size.x;
            targetScaleY = targetHeight / mapBorders[i].Renderer.size.y;
            break;
          }
        case WallType.Right: {
            float targetWidth = screenWorldSize.x/2 - mapBorders[i].BorderPosition;
            targetPosition = new Vector3(mapBorders[i].BorderPosition + targetWidth/2, 0, 0);
            targetScaleX = targetWidth / mapBorders[i].Renderer.size.x;
            targetScaleY = screenWorldSize.y / mapBorders[i].Renderer.size.y;
            break;
          }
        case WallType.Bottom: {
            float targetHeight = screenWorldSize.y/2 - mapBorders[i].BorderPosition;
            targetPosition = new Vector3(0, -mapBorders[i].BorderPosition - targetHeight/2, 0);
            targetScaleX = screenWorldSize.x / mapBorders[i].Renderer.size.x;
            targetScaleY = targetHeight / mapBorders[i].Renderer.size.y;
            break;
          }
      }

      mapBorders[i].Transform.localScale = new Vector3(targetScaleX, targetScaleY, localScale.z);
      mapBorders[i].Transform.position = targetPosition;
    }
  }

  private Vector2 ScreenWorldSize() {
    Vector2 size;
    size.y = Camera.main.orthographicSize * 2.0f;
    size.x = size.y * Camera.main.aspect;
    return size;
  }

  struct MapBorder {
    public float BorderPosition { get; set; }
    public Transform Transform { get; private set; }
    public SpriteRenderer Renderer { get; private set; }
    public WallType Type { get; private set; }

    public MapBorder(Transform transform, WallType type, float borderPosition) {
      this.Transform = transform;
      this.Type = type;
      this.Renderer = transform.GetComponent<SpriteRenderer>();
      this.BorderPosition = borderPosition;
    }
  }

}
