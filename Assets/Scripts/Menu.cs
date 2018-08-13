using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

  public Animator uiAnimator;

  public void OnPlayClicked() {
    SceneManager.LoadScene("TopDown 1");
  }

  public void OnCreditsClicked() {
    Debug.Log("SJOWOWOWOW");
    uiAnimator.SetTrigger("showCredits");
  }

  public void OnBackClicked() {
    uiAnimator.SetTrigger("hideCredits");
  }

}
