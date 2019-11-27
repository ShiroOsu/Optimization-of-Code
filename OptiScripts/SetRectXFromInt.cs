using UnityEngine;
using UnityEngine.UI;

public class SetRectXFromInt : MonoBehaviour
{
	// Invoke the method with starting value or it wont look good

	RectTransform rect;
	private int initialValue;
	private float initialSize;
	private void Awake()
	{
		rect = GetComponent<RectTransform>();
		initialSize = rect.rect.width;
	}

	public void SetSize(int value)
	{
		if(initialValue == 0)
		{
			initialValue = value;
		}
		var width = ((float)value / (float)initialValue) * (float)initialSize;
		rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
	}
}
