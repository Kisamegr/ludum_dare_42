using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

  public TextMeshProUGUI levelText;

  public TextMeshProUGUI gameoverText;

  private TopDownGame game;

  private static UiManager _instance;

  public static UiManager Instance()
  {
    return _instance;
  }

  private void Awake()
  {
    _instance = this;
  }

  // Use this for initialization
  void Start()
  {
    game = TopDownGame.Instance();
  }

  // Update is called once per frame
  void Update()
  {
    levelText.text = game.CurrentLevel >= 0
      ? "Level " + game.CurrentLevel
      : "Level -";
  }

  public void NextLevel(float duration)
  {

  }

  public void GameOver()
  {
    //TODO: Fadeout
    gameoverText.gameObject.SetActive(true);
  }

  public void FadeIn(float duration)
  {

  }

  public void FadeOut(float duration)
  {

  }

}
