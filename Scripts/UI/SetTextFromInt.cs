using UnityEngine;
using UnityEngine.UI;

public class SetTextFromInt : MonoBehaviour
{
	Text text;

	string originalText;
	private void Awake()
	{
		text = GetComponent<Text>();
	}

	public void SetText(int value)
	{
		text.text = value.ToString("N0");
	}
}
