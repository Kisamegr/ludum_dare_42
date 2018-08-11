using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {

  public TextMeshProUGUI levelText;

  private TopDownGame game;
	// Use this for initialization
	void Start () {
    game = TopDownGame.Instance();
	}
	
	// Update is called once per frame
	void Update () {
    levelText.text = game.CurrentLevel >= 0
      ? "Level " + game.CurrentLevel
      : "Level -";
	}
}
