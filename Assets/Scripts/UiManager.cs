using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

  public TextMeshProUGUI levelText;
  public TextMeshProUGUI scoreText;
  public TextMeshProUGUI gameoverText;
  public TextMeshProUGUI timeText;

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

    UpdateTimeText();
}

  public void NextLevel(float duration)
  {

  }

  private void UpdateTimeText()
  {

    float playTime = Time.timeSinceLevelLoad;

    int mils = (int)(10 * (playTime - (int)playTime));
    int secs = (int)(playTime);
    int mins = secs / 60;
    secs = secs % 60;

    string _timeText = "";
    if (mins != 0)
    {
      _timeText = mins + ":";
    }

    _timeText += secs + "." + mils;

    timeText.text = _timeText;
  }

  public void UpdateScore(int score)
  {
    scoreText.text = "Score: " + score;
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
