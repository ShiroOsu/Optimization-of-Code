using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Tesla : MonoBehaviour
{
	public float rotationSpeed;
	public float speed;
	public float leftBounds;
	public float rightBounds;
	private void Awake()
	{
		var rb = GetComponent<Rigidbody2D>();
		rb.angularVelocity = rotationSpeed;
		rb.velocity = new Vector2(speed, 0f);
	}

	private void Update()
	{
		if (transform.position.x > rightBounds)
		{
			var pos = transform.position;
			pos.x = leftBounds;
			transform.position = pos;
		}
	}
}
