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
  public GameObject explosionPrefab;
  public GameObject overtextPrefab;

  private GAME game;
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
    game = GAME.Instance();
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

  public void MakeExplosion(Vector2 position, int noParticles, Color c, float speedMultiplier = 1f)
  {
    GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
    ParticleSystem ps = explosion.GetComponent<ParticleSystem>();

    ps.emission.SetBurst(0, new ParticleSystem.Burst(0, noParticles));

    var main = ps.main;
    c.a = main.startColor.color.a;
    main.startColor = c;

    main.startSpeedMultiplier = speedMultiplier;

    Destroy(explosion, 3);
  }

}
