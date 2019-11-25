using UnityEngine;
using UnityEngine.UI;

public class HideObjectInInspector : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		gameObject.GetComponent<Image>().color = new Color(0,0,0,0);
	}
}
