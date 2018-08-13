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
  public TextMeshProUGUI countdownText;
  public Animator countdownAnimator;

  public GameObject LinePrefab;

  public Transform weaponUiOverlay;
  public GameObject weaponUiPrefab;


  private GAME game;
  private static UiManager _instance;

  public bool countdownPlaying = false;
  private float countdownLeft = 0;

  private Queue<Image> weaponUiImages;

  public static UiManager Instance()
  {
    return _instance;
  }

  private void Awake()
  {
    _instance = this; 
  }

  private int prevSec;

  // Use this for initialization
  void Start()
  {
    game = GAME.Instance();
    weaponUiImages = new Queue<Image>();

    CreateGrid();

  }

  void CreateGrid()
  {
    float W = GAME.Instance().mapSize.x / 2 + 5;
    float H = GAME.Instance().mapSize.y / 2 + 5;


    int noGrids = 20;

    float w = 2 * W / noGrids;
    float h = 2 * H / noGrids;

    var grid = new GameObject();

    for (int i = 0; i < noGrids; i++)
    {
      GameObject lrGO = Instantiate(LinePrefab, parent: grid.transform);
      LineRenderer lr = lrGO.GetComponent<LineRenderer>();
      lr.SetPositions(new Vector3[] { new Vector3(-W, -H + i * h, 0), new Vector3(W, -H + i * h, 0) });
    }

    for (int i = 0; i < noGrids; i++)
    {
      GameObject lrGO = Instantiate(LinePrefab, parent: grid.transform);
      LineRenderer lr = lrGO.GetComponent<LineRenderer>();
      lr.SetPositions(new Vector3[] { new Vector3(-W + i*w , -H, 0), new Vector3(-W + i * w, H, 0) });
    }


  }

  // Update is called once per frame
  void Update()
  {
    levelText.text = game.CurrentLevel >= 0
      ? "Level " + game.CurrentLevel
      : "Level -";

    UpdateTimeText();

    if(countdownPlaying) {
      countdownText.text = Mathf.CeilToInt(countdownLeft).ToString();
      countdownLeft -= Time.deltaTime;

      //Play Second timer;
      if(prevSec != Mathf.CeilToInt(countdownLeft))
      {
        if (Mathf.CeilToInt(countdownLeft) != 0)
          transform.GetChild(0).GetComponents<AudioSource>()[1].Play();
        
        prevSec = Mathf.CeilToInt(countdownLeft);
      }

      if(countdownLeft <= 0) {
        countdownPlaying = false;
        countdownText.gameObject.SetActive(false);
        countdownAnimator.StopPlayback();
        if(GAME.Instance().CurrentLevel == 0)
        {
          transform.GetChild(0).GetComponents<AudioSource>()[0].Play();
        }
        else
        {
          transform.GetChild(0).GetComponents<AudioSource>()[0].volume *= 3;
        }
        GAME.Instance().LoadNextLevel();
      }
    }
  }
  

  public void NextLevel(float duration) {
    transform.GetChild(0).GetComponents<AudioSource>()[0].volume /= 3;
    countdownLeft = duration;
    countdownPlaying = true;
    countdownText.gameObject.SetActive(true);
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
    transform.GetChild(0).GetComponents<AudioSource>()[0].Stop();

    //transform.GetChild(0).GetComponents<AudioSource>()[3].Play();
    //transform.GetChild(0).GetComponents<AudioSource>()[0].clip = transform.GetChild(0).GetComponents<AudioSource>()[3].clip;
    //transform.GetChild(0).GetComponents<AudioSource>()[0].Play();

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

  public void SetSpecialWeapon(Sprite sprite, Color color, int ammo) {
    foreach (Image weaponUi in weaponUiImages)
      Destroy(weaponUi.gameObject);

    weaponUiImages.Clear();

    for(int i=0; i<ammo; i++) {
      Image image = Instantiate(weaponUiPrefab, weaponUiOverlay).GetComponent<Image>();

      image.sprite = sprite;
      image.color = color;
      weaponUiImages.Enqueue(image);
    }
  }

  public void DecreaseAmmo() {
    Destroy(weaponUiImages.Dequeue());
  }

}
