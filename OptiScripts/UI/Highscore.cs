using UnityEngine;
using UnityEngine.UI;

public class Highscore : MonoBehaviour
{
	public GameObject names;
	public GameObject scores;
	private Text[] nameFields;
	private Text[] scoreFields;

	private void Awake()
	{
		nameFields = names.GetComponentsInChildren<Text>();
		scoreFields = scores.GetComponentsInChildren<Text>();
	}

	private void OnEnable()
	{
		for(int i = 0; i<scoreFields.Length; i++)
		{
			nameFields[i].text = PlayerPrefs.GetString($"scorename{i}", "nobody") + " :";
			scoreFields[i].text = PlayerPrefs.GetInt($"score{i}", -1).ToString("N0");
		}
	}
}
