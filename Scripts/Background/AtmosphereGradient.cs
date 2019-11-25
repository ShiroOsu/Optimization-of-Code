using UnityEngine;
using UnityEngine.Assertions;

public class AtmosphereGradient : MonoBehaviour
{
    [SerializeField] private GameObject player = null;
    [SerializeField] private SpriteRenderer spriteRenderer = null;

	void Start()
	{
		if (spriteRenderer == null)
			spriteRenderer = GetComponent<SpriteRenderer>();

		Assert.IsNotNull(spriteRenderer, "Could not find spriteRenderer");
		Assert.IsNotNull(player, "Player not attached");
	}

	void Update()
	{
        UpdateGradient();
	}

    // Change transparency of atmosphere the higher the player travels.
    private void UpdateGradient()
    {
        Color c = spriteRenderer.color;
        if (player)
        {
            float newA = (255f - player.transform.position.y * 5) / 255f;
            spriteRenderer.color = new Color(c.r, c.g, c.b, newA);
        }
    }
}