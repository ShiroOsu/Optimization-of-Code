using UnityEngine;

public class Hatch : MonoBehaviour
{
	Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		animator.SetBool("hatchOpen", false);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			GuideSystem.instance?.EnteredHatchCollider();
			animator.SetBool("hatchOpen", true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
			animator.SetBool("hatchOpen", false);
	}
}
