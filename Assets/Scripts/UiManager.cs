using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UiManager : MonoBehaviour
{

    public float scoreSizeDamping = 0.2f;
    public float scoreSizeMaxScale = 2;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI timeText;
    public GameObject explosionPrefab;
    public TextMeshProUGUI countdownText;
    public Animator countdownAnimator;

    public GameObject LinePrefab;

    public Image skillImage;
    public Texture2D cursorTexture;
    public Image manaBar;



    private GAME game;
    private static UiManager _instance;

    public bool countdownPlaying = false;
    private float countdownLeft = 0;
     

    private Vector3 originalScoreScale;

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
        originalScoreScale = scoreText.transform.localScale;

        CreateGrid();
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

    }

    void CreateGrid()
    {
        float W = GAME.Instance().mapSize.x / 2 + 5;
        float H = GAME.Instance().mapSize.y / 2 + 5;


        int noGrids = 20;

        float w = 2 * W / noGrids;
        float h = 2 * H / noGrids;

        var grid = new GameObject("Grid");

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
            lr.SetPositions(new Vector3[] { new Vector3(-W + i * w, -H, 0), new Vector3(-W + i * w, H, 0) });
        }


    }

    // Update is called once per frame
    void Update()
    {
        levelText.text = game.CurrentLevel >= 0
          ? "Level " + game.CurrentLevel
          : "Level -";

        if (!GAME.Instance().IsGameOver())
            UpdateTimeText();

        if (countdownPlaying)
        {
            countdownText.text = Mathf.CeilToInt(countdownLeft).ToString();
            countdownLeft -= Time.deltaTime;

            //Play Second timer;
            if (prevSec != Mathf.CeilToInt(countdownLeft))
            {
                if (Mathf.CeilToInt(countdownLeft) != 0)
                    transform.GetChild(0).GetComponents<AudioSource>()[1].Play();

                prevSec = Mathf.CeilToInt(countdownLeft);
            }

            if (countdownLeft <= 0)
            {
                countdownPlaying = false;
                countdownText.gameObject.SetActive(false);
                countdownAnimator.StopPlayback();
                if (GAME.Instance().CurrentLevel == 0)
                {
                    transform.GetChild(0).GetComponents<AudioSource>()[0].Play();
                }
                GAME.Instance().LoadNextLevel();
            }

        }
        scoreText.transform.localScale = Vector3.Lerp(
          scoreText.transform.localScale,
          originalScoreScale,
          Time.deltaTime / scoreSizeDamping);
    }


    public void NextLevel(float duration)
    {
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

        timeText.text = "Time: " + _timeText;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = /*"Score: " +*/ score.ToString();
        scoreText.transform.localScale = originalScoreScale * scoreSizeMaxScale;
    }

    public void GameOver()
    {
        //TODO: Fadeout
        transform.GetChild(0).GetComponents<AudioSource>()[0].Stop();

        //transform.GetChild(0).GetComponents<AudioSource>()[3].Play();
        //transform.GetChild(0).GetComponents<AudioSource>()[0].clip = transform.GetChild(0).GetComponents<AudioSource>()[3].clip;
        //transform.GetChild(0).GetComponents<AudioSource>()[0].Play();

        gameOverPanel.SetActive(true);
    }

    public void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMenuClicked()
    {
        SceneManager.LoadScene("Menu");
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

    public void SetSecondarySkill(SkillObject secondarySkill)
    {
        //manaBar.color = Color.yellow;
        skillImage.sprite = secondarySkill.icon;
        skillImage.color = secondarySkill.icon_color;
        manaBar.color = secondarySkill.icon_color;
    } 

    public void UpdateMana(float percentage)
    {
        manaBar.fillAmount = percentage;
    }
}
