using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

  public Animator uiAnimator;

  public void OnPlayClicked() {
    SceneManager.LoadScene("TopDown 1");
  }

  public void OnCreditsClicked() {
    uiAnimator.SetTrigger("showCredits");
  }

  public void OnCreditsBackClicked() {
    uiAnimator.SetTrigger("hideCredits");
  }

  public void OnHowToClicked() {
    uiAnimator.SetTrigger("showHow");
  }

  public void OnHowToBackClicked() {
    uiAnimator.SetTrigger("hideHow");
  }

}
