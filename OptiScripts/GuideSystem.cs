using UnityEngine;

public class GuideSystem : MonoBehaviour
{
	public static GuideSystem instance;
	public GameObject arrow;
	public GameObject cargoText;
	public GameObject startText;
	public GameObject onPickupCargoText;
	public GameObject onSellText;
	private Transform player;
	private Transform hatch;
	private Transform nearestCargo;
	private GuideState state = GuideState.ToCargo;

	// Radius to scan for nearest cargo
	private float radius = 20f;
	public LayerMask cargoLayer;
	public enum GuideState
	{
		Intro, ToCargo, ToHatch, DropCargo, None, Disabled
	}


	private void Awake()
	{
		if (!instance)
		{
			instance = this;

		}
		else
		{
			Destroy(gameObject);
		}
		player = GameObject.FindGameObjectWithTag("Player").transform;
		hatch = GameObject.FindGameObjectWithTag("Hatch").transform;
	}

	private void Update()
	{
		// Makeshift state machine for guiding the player first time starting
		if (state != GuideState.Disabled)
		{
			if (!player)
			{
				// If player dies, prevent errors by destroying guide system and setting the empty reference to this
				Destroy(gameObject);
				player = transform;
			}
			if (Input.GetKeyDown(KeyCode.H))
			{
				state = GuideState.Disabled;
				arrow.SetActive(false);
				cargoText.SetActive(false);
				startText.SetActive(false);
				onSellText.SetActive(false);
				onPickupCargoText.SetActive(false);
			}

			switch (state)
			{
				case GuideState.ToCargo:
					startText.SetActive(true);
					onPickupCargoText.SetActive(false);
					var cols = Physics2D.CircleCastAll(transform.position, radius, Vector2.down, 0, cargoLayer);
					if (cols.Length > 0)
					{
						nearestCargo = NearestCargo(cols);
						arrow.SetActive(true);
						RotateArrow(nearestCargo.position);

						var hits = Physics2D.CircleCastAll(player.position, 2f, Vector2.zero, 0, cargoLayer);
						foreach (var hit in hits)
						{
							if (hit.collider.CompareTag("Cargo"))
								state = GuideState.ToHatch;
						}
					}
					else
					{
						arrow.SetActive(false);
					}
					transform.position = player.transform.position;
					break;
				case GuideState.ToHatch:

					if (!Physics2D.CircleCast(player.position, 2f, Vector2.zero, 0, cargoLayer))
					{
						state = GuideState.ToCargo;
					}

					startText.SetActive(false);
					onPickupCargoText.SetActive(true);
					RotateArrow(hatch.position);
					transform.position = player.transform.position;
					break;
				case GuideState.DropCargo:
					arrow.SetActive(true);
					RotateArrow(hatch.position);
					transform.position = player.transform.position;
					break;
				case GuideState.None:
					transform.position = player.transform.position;
					arrow.SetActive(false);
					cargoText.SetActive(false);
					break;
				default:
					break;
			}
		}
	}
	public void EnteredHatchCollider()
	{
		if (state == GuideState.ToHatch)
		{
			state = GuideState.DropCargo;
			cargoText.SetActive(false);

			cargoText.SetActive(true);
			onPickupCargoText.SetActive(false);
		}
	}
	public void SoldCargo()
	{
		if (state == GuideState.DropCargo)
		{

			arrow.SetActive(false);
			cargoText.SetActive(false);
			onSellText.SetActive(true);
			state = GuideState.None;
			Invoke("HideSellText", 5f);
		}

	}
	private void RotateArrow(Vector3 target)
	{
		Vector2 _direction = target - transform.position;
		float _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
		Quaternion q = Quaternion.AngleAxis(_angle, Vector3.forward);
		arrow.transform.rotation = q;
	}

	private Transform NearestCargo(RaycastHit2D[] hits)
	{
		Transform nearest = null;
		float bestDist = float.MaxValue;
		foreach (var hit in hits)
		{
			if (Vector2.Distance(player.transform.position, hit.transform.position) < bestDist)
			{
				nearest = hit.transform;
				bestDist = Vector2.Distance(player.transform.position, hit.transform.position);
			}
		}

		return nearest;
	}

	private void HideSellText()
	{
		onSellText.SetActive(false);
		state = GuideState.Disabled;
	}
}
