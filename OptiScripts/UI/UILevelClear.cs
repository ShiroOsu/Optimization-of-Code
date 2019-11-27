using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UILevelClear : MonoBehaviour
{
	public GameObject title;
	public GameObject score;
	public GameObject nextLevel;
	public GameObject submitScore;
	public GameObject inputName;
	public GameObject menu;

	public GameObject nextLevelHighlight;
	public GameObject submitHighlight;
    public GameObject menuHighlight;
    public Image transition;
	private InputField nameInput;

	public Text scoreText;
	private void Awake()
	{
		StartCoroutine(ShowMenu());
		nameInput = inputName.GetComponentInChildren<InputField>();
	}

	private void ShowTitle()
	{
		title.SetActive(true);
	}

	private void ShowScore()
	{
		score.SetActive(true);

	}
	private void ShowNextLevelButton()
	{
		nextLevel.SetActive(true);
	}
	private void ShowScoreButton()
	{
		inputName.SetActive(true);
		submitScore.SetActive(true);
	}

	#region Mouse Events
	public void NextLevelOver()
	{
		nextLevelHighlight?.SetActive(true);
	}

	public void NextLevelExit()
	{
		nextLevelHighlight?.SetActive(false);
	}
	public void NextLevelClick()
	{
		nextLevelHighlight?.SetActive(false);
		StartCoroutine(FadeToLevel(1f));
	}

	public void SubmitOver()
	{
		submitHighlight?.SetActive(true);
	}

	public void SubmitExit()
	{
		submitHighlight?.SetActive(false);
	}
	public void SubmitClick()
	{
		submitHighlight?.SetActive(false);
		if (nameInput.text.Length > 0)
		{
			if (GameManager.instance)
			{
				GameManager.instance.SaveScore(GameManager.instance.TotalScore, nameInput.text);
			}
		}
		StartCoroutine(FadeToMenu(1f));
	}

    public void MenuOver()
    {
        menuHighlight?.SetActive(true);
    }

    public void MenuExit()
    {
        menuHighlight?.SetActive(false);
    }
    public void MenuClick()
    {
        menuHighlight?.SetActive(false);
        StartCoroutine(FadeToMenu(1f));
    }
    #endregion


    private IEnumerator ShowMenu()
	{
		yield return new WaitForSeconds(0.5f);
		ShowTitle();
		
		yield return new WaitForSeconds(0.5f);
		ShowScore();
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(Scramble(scoreText, GameManager.instance != null ? GameManager.instance.TotalScore : 12345));

	}



	private IEnumerator FadeToLevel(float duration)
	{
		transition.gameObject.SetActive(true);
		transition.raycastTarget = true;
		float elapsedTime = 0;
		while (elapsedTime < duration)
		{
			transition.color = new Color(0, 0, 0, elapsedTime / duration);
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}
		elapsedTime = 0;
		GameManager.instance?.LoadLevel();
	}
	private IEnumerator FadeToMenu(float duration)
	{
		transition.gameObject.SetActive(true);
		transition.raycastTarget = true;
		float elapsedTime = 0;
		while (elapsedTime < duration)
		{
			transition.color = new Color(0, 0, 0, elapsedTime / duration);
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}
		GameManager.instance?.LoadMenu();
	}

	// Scramble the score text before showing it for arcade-y feel :)
	private IEnumerator Scramble(Text text, int score)
	{
		string scoreStr = "" + score;
		string temp = "";
		string scoreBuilder = "";
		for (int width = 0; width < scoreStr.Length; width++)
		{
			for (int i = 0; i < 20; i++)
			{
				temp = scoreBuilder;
				for (int j = 0; j < scoreStr.Length - width; j++)
				{
					temp += "" + Random.Range(0, 10);
				}
				text.text = temp;
				yield return new WaitForEndOfFrame();
			}
			scoreBuilder += scoreStr[width];

		}
		text.text = scoreStr;
		yield return new WaitForSeconds(0.5f);
		ShowNextLevelButton();
		yield return new WaitForSeconds(0.5f);
		menu.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		ShowScoreButton();
	}

}
