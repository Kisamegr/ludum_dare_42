using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownGame : MonoBehaviour {

  public float countdown;
  public float maxCountdown;

  private MapBorder[] mapBorders;  // Left, Top, Right, Bottom

  private Camera mainCamera;

  // Use this for initialization
  void Start() {
    mainCamera = Camera.main;

    countdown = 100;
    maxCountdown = 100;

    Vector2 screenWorldSize = ScreenWorldSize();
    Transform wallParent = GameObject.Find("Walls").transform;

    mapBorders = new MapBorder[4];
    mapBorders[0] = new MapBorder(wallParent.Find("Left"), MapBorder.WallType.Left, screenWorldSize.x/2);
    mapBorders[1] = new MapBorder(wallParent.Find("Top"), MapBorder.WallType.Top, screenWorldSize.y/2);
    mapBorders[2] = new MapBorder(wallParent.Find("Right"), MapBorder.WallType.Right, screenWorldSize.x/2);
    mapBorders[3] = new MapBorder(wallParent.Find("Bottom"), MapBorder.WallType.Bottom, screenWorldSize.y/2);
  }

  // Update is called once per frame
  void Update() {
    for (int i = 0; i<mapBorders.Length; i++) {
      mapBorders[i].BorderPosition -= Time.deltaTime/5;
    }

    CalculateWalls();
  }

  void CalculateWalls() {
    Vector3 screenWorldSize = ScreenWorldSize();

    for (int i = 0; i<mapBorders.Length; i++) {
      float targetScaleX = 0.02f;
      float targetScaleY = 0.02f;
      Vector3 targetPosition = Vector3.zero;
      Vector3 localScale = mapBorders[i].Transform.localScale;

      switch (mapBorders[i].Type) {
        case MapBorder.WallType.Left: {
            float targetWidth = screenWorldSize.x/2 - mapBorders[i].BorderPosition;
            targetPosition = new Vector3(-mapBorders[i].BorderPosition - targetWidth/2, 0, 0);
            targetScaleX = targetWidth / mapBorders[i].Renderer.size.x;
            targetScaleY = screenWorldSize.y / mapBorders[i].Renderer.size.y;
            break;
          };
        case MapBorder.WallType.Top: {
            float targetHeight = screenWorldSize.y/2 - mapBorders[i].BorderPosition;
            targetPosition = new Vector3(0, mapBorders[i].BorderPosition + targetHeight/2, 0);
            targetScaleX = screenWorldSize.x / mapBorders[i].Renderer.size.x;
            targetScaleY = targetHeight / mapBorders[i].Renderer.size.y;
            break;
          }
        case MapBorder.WallType.Right: {
            float targetWidth = screenWorldSize.x/2 - mapBorders[i].BorderPosition;
            targetPosition = new Vector3(mapBorders[i].BorderPosition + targetWidth, 0, 0);
            targetScaleX = targetWidth / mapBorders[i].Renderer.size.x;
            targetScaleY = screenWorldSize.y / mapBorders[i].Renderer.size.y;
            break;
          }
        case MapBorder.WallType.Bottom: {
            float targetHeight = screenWorldSize.y/2 - mapBorders[i].BorderPosition;
            targetPosition = new Vector3(0, -mapBorders[i].BorderPosition - targetHeight, 0);
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
    public enum WallType {
      Left, Top, Right, Bottom
    }

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
