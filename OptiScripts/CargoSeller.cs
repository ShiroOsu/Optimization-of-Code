using UnityEngine;

public class CargoSeller : MonoBehaviour
{
    // TODO instead of Destroy, pool the cargo
    private PoolableObject type;

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
            PoolMe(collision.gameObject); // TODO
		}
	}

    private void PoolMe(GameObject gameObject)
    {
        ObjectPool.instance.PoolObject(type, gameObject);
    }
}
