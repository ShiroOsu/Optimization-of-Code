using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
public class CargoSpawner : MonoBehaviour
{
	public GameObject[] cargoPrefabs;
	private Transform[] spawners;
	private Dictionary<Transform, bool> spawnerOccupied;
	public LayerMask cargoLayer;
	public int spawnAmount = 10;

	public float spawnInterval = 0.1f;
	private float lastSpawn;

	private void Awake()
	{
		spawners = GetComponentsInChildren<Transform>();
		spawnerOccupied = new Dictionary<Transform, bool>();
		Assert.IsNotNull(cargoPrefabs, "assign cargo prefab in cargo spawner");
		Assert.IsNotNull(spawners, "assign spawners in cargo spawner");
		if (spawners.Length <= 1)
		{
			throw new AssertionException("Not enough spawners", "Assign more spawners for CargoSpawner");
		}
		for (int i = 0; i < spawnAmount; i++)
		{
			SpawnCargo();
		}
	}
	private void Update()
	{
		if (lastSpawn + spawnInterval < Time.time)
		{
			lastSpawn = Time.time;
			SpawnCargo();

		}
	}
	private void RefreshDictionary()
	{
		for (int i = 1; i < spawners.Length; i++)
		{
			var hit = Physics2D.CircleCast(spawners[i].position, 0.2f, Vector2.up, 0, cargoLayer);
			spawnerOccupied[spawners[i]] = hit ? true : false;
		}
	}
	public void SpawnCargo()
	{
		RefreshDictionary();
		var matches = spawnerOccupied.Where(i => i.Value == false).ToArray();
		if (matches.Length > 0)
		{
			//Debug.Log("Spawning cargo");
			var pos = matches[Random.Range(0, matches.Length)].Key.position;
			Instantiate(cargoPrefabs[Random.Range(0, cargoPrefabs.Length)], pos, Quaternion.identity, transform);
		}



		//int tries = 0;
		//while (tries < 10)
		//{
		//	var pos = spawners[Random.Range(1, spawners.Length)].position;
		//	var hit = Physics2D.CircleCast(pos, 0.1f, Vector2.up, 0, cargoLayer);
		//	if (!hit)
		//	{
		//		Instantiate(cargoPrefabs[Random.Range(0, cargoPrefabs.Length)], pos, Quaternion.identity, transform);
		//		return true;
		//	}
		//	else
		//	{
		//		tries++;
		//	}
		//}
		//return false;
	}



	private void OnDrawGizmos()
	{
		if (spawners == null)
		{
			spawners = GetComponentsInChildren<Transform>();
		}
		if (spawners != null)
		{
			if (spawners.Length > 2)
			{
				for (int i = 1; i < spawners.Length; i++)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawWireSphere(spawners[i].position, 0.3f);
				}
			}
		}

	}
}
