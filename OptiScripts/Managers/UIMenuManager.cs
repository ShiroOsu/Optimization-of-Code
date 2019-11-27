using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class UIMenuManager : MonoBehaviour
{
	public GameObject mainMenu;
	public GameObject startHighlight;
	public GameObject instructionsHighlight;
	public GameObject scoresHighlight;
	public Image transition;

	public GameObject instructions;
	public GameObject highScores;


	AudioSource audioSource;

	public AudioClip blip;
	public AudioClip accept;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>(); // Move out

	}

	public void StartMouseOver()
	{
		audioSource.PlayOneShot(blip);
		startHighlight?.SetActive(true);
	}

	public void StartMouseExit()
	{
		startHighlight?.SetActive(false);
	}
	public void StartClick()
	{
		audioSource.PlayOneShot(accept);
		startHighlight?.SetActive(false);
		StartCoroutine(FadeIn(StartGame, 1f));
	}

	public void InstructionsMouseOver()
	{
		audioSource.PlayOneShot(blip);
		instructionsHighlight?.SetActive(true);
	}
	public void InstructionsClick()
	{
		audioSource.PlayOneShot(accept);
		instructionsHighlight?.SetActive(false);
		StartCoroutine(FadeIn(ShowInstructions, 0.5f));
	}
	public void InstructionsMouseExit()
	{
		instructionsHighlight?.SetActive(false);
	}

	public void InstructionsReturn()
	{
		audioSource.PlayOneShot(blip);
		StartCoroutine(FadeBack(HideInstructions, 0.5f));
	}

	public void ScoresMouseOver()
	{
		audioSource.PlayOneShot(blip);
		scoresHighlight?.SetActive(true);
	}
	public void ScoresClick()
	{
		audioSource.PlayOneShot(accept);
		scoresHighlight?.SetActive(false);
		StartCoroutine(FadeIn(ShowHighScores, 0.5f));
	}
	public void ScoresMouseExit()
	{
		scoresHighlight?.SetActive(false);
	}

	public void ScoresReturn()
	{
		audioSource.PlayOneShot(blip);
		StartCoroutine(FadeBack(HideHighScores, 0.5f));
	}

    // TODO - same function twice
	private IEnumerator FadeIn(Func<int> methodName, float duration)
	{
		transition.raycastTarget = true;
		float elapsedTime = 0;
		while (elapsedTime < duration)
		{
			transition.color = new Color(0, 0, 0, elapsedTime / duration);
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}
		elapsedTime = 0;
		mainMenu.SetActive(false);
		transition.raycastTarget = false;
		methodName();
		while (elapsedTime < duration)
		{
			transition.color = new Color(0, 0, 0, 1 - (elapsedTime / duration));
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}

	}

	private IEnumerator FadeBack(Func<int> methodName, float duration)
	{
		float elapsedTime = 0;

		while (elapsedTime < duration)
		{
			transition.color = new Color(0, 0, 0, elapsedTime / duration);
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}
		elapsedTime = 0;
		mainMenu.SetActive(true);
		methodName();
		while (elapsedTime < duration)
		{
			transition.color = new Color(0, 0, 0, 1 - (elapsedTime / duration));
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}


		transition.raycastTarget = false;

	}

	private int StartGame()
	{
		GameManager.instance.LoadLevel();
		return 0;
	}
	private int ShowInstructions()
	{
		Debug.Log("Showing instructions");
		instructions.SetActive(true);
		return 0;
	}
	private int HideInstructions()
	{
		Debug.Log("Hiding instructions");
		instructions.SetActive(false);
		return 0;
	}
	private int ShowHighScores()
	{
		Debug.Log("Showing high scores");
		highScores.SetActive(true);
		return 0;
	}
	private int HideHighScores()
	{
		Debug.Log("Showing high scores");
		highScores.SetActive(false);
		return 0;
	}


}
