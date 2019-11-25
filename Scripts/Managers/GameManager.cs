using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public float leftSpawnLimit;
	public float rightSpawnLimit;
	public int cratesPerLevel = 10;
	public string levelScene;
	public string levelClearScene;
	public string menuScene;
	public int TotalScore { get; set; }
	private CursorLockMode oldLockMode;
	private void Awake()
	{
		if (!instance)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			Assert.IsNotNull(levelScene, "Assign scene name for game manager");
			Assert.IsNotNull(levelClearScene, "Assign scene name for game manager");
			Assert.IsNotNull(menuScene, "Assign scene name for game manager");
			if(!PlayerPrefs.HasKey("score0"))
			{
				for(int i = 0; i<10; i++)
				{
					PlayerPrefs.SetString("scorename" + i, "nobody");
					PlayerPrefs.SetInt("score" + i, 0);
				}
				PlayerPrefs.Save();
				Debug.Log("writing default scores");
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void LoadLevel()
	{
		oldLockMode = Cursor.lockState;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		SceneManager.LoadScene(levelScene);
		TotalScore = 0;
		StartCoroutine(FadeInScene(2f));
	}
	public void EndLevel(int score)
	{
		TotalScore += score;

		StartCoroutine(FadeOutScene(1.5f));
		Invoke("LoadLevelClear", 1.6f);

		// todo call on exit level, transition between levels
	}

	public void LoadMenu()
	{
		SceneManager.LoadScene(menuScene);
	}

	public void LoadLevelClear()
	{
		Cursor.lockState = oldLockMode;
		Cursor.visible = true;
		SceneManager.LoadScene(levelClearScene);
	}

	// Saves a score to playerprefs if its better than any of current highscores
	public void SaveScore(int newScore, string name)
	{
		int[] scores = new int[10];
		for (int i = 0; i < scores.Length; i++)
		{
			scores[i] = PlayerPrefs.GetInt($"score{i}");
		}

		int newRecordIndex = -1;
		for (int i = 0; i < scores.Length; i++)
		{
			if (newScore > scores[i])
			{
				Debug.Log("new record index " + i);
				newRecordIndex = i;
				break;
			}
		}
		if (newRecordIndex > -1)
		{
			for (int i = scores.Length - 2; i > newRecordIndex; i--)
			{
				PlayerPrefs.SetString($"scorename{i}", PlayerPrefs.GetString($"scorename{i - 1}"));
				PlayerPrefs.SetInt($"score{i}", scores[i - 1]);
			}
			
		}
		PlayerPrefs.SetString("scorename" + newRecordIndex, name);
		PlayerPrefs.SetInt("score" + newRecordIndex, newScore);
		PlayerPrefs.Save();
		Debug.Log("writing score");
	}

	public void FadeToScene()
	{
		StartCoroutine(FadeInScene(2f));
	}
	private IEnumerator FadeInScene(float time)
	{
		Image transition = null;

		int tries = 0;
		while (transition == null)
		{
			tries++;
			transition = GameObject.Find("transitionFader")?.GetComponent<Image>();
			yield return new WaitForSeconds(0.1f);
			if (tries > 100)
			{
				Debug.Log("Cant find transition fader in level scene");
				break;
			}
		}

		transition.raycastTarget = true;
		float elapsedTime = 0;
		float duration = time;
		while (elapsedTime < duration)
		{
			transition.color = new Color(0, 0, 0, 1 - (elapsedTime / duration));
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}
		transition.raycastTarget = false;

	}

	private IEnumerator FadeOutScene(float time)
	{
		Image transition = null;

		int tries = 0;
		while (transition == null)
		{
			tries++;
			transition = GameObject.Find("transitionFader")?.GetComponent<Image>();
			yield return new WaitForSeconds(0.1f);
			if (tries > 100)
			{
				Debug.Log("Cant find transition fader in level scene");
				break;
			}
		}

		transition.raycastTarget = true;
		float elapsedTime = 0;
		float duration = time;

		while (elapsedTime < duration)
		{
			transition.color = new Color(0, 0, 0, elapsedTime / duration);
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
		}
	}
}
