using UnityEngine;

public class CargoSeller : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Cargo"))
		{
			GuideSystem.instance?.SoldCargo();
			LevelManager.instance.LevelScore += collision.transform.GetComponent<Cargo>().value;
			LevelManager.instance?.cargoSpawner.SpawnCargo();
			Magnetizable _mag = collision.GetComponentInChildren<Magnetizable>();
			if (_mag.magnetizedTo != null)
				_mag.DeMagnetize();
            AudioManager.instance.Play("Reward");
			Destroy(collision.gameObject);
		}
	}
}
