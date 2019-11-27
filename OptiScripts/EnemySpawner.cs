using UnityEngine;
using System.Collections;
public class EnemySpawner : MonoBehaviour
{
    // TODO Use an objectPool 
    private GameObject temp;
    private PoolableObject type;

	public float respawnTime;
    [SerializeField] private Vector3 positionOffset;
	private float elapsedTime;
	public GameObject enemyPrefab;
	private bool enemyDead = false;
	private bool isVisible = false;
	private bool respawning = false;
    private float shieldHP = 0f;
    private float shieldIncrease = 50f;


	private void Awake()
	{
        type = PoolableObject.Enemy;
		Spawn();
	}
	public void StartSpawn()
	{
		enemyDead = true;
	}

	private GameObject Spawn()
	{
        // TODO
        temp = ObjectPool.instance.GetObjectForType(type);
        temp.transform.position = (transform.position + positionOffset.magnitude * transform.up);
        temp.transform.rotation = Quaternion.identity;
        return temp;
	}

	private IEnumerator Respawn()
	{
		elapsedTime = 0f;
		bool spawned = false;
		while (!spawned)
		{
			yield return new WaitForSeconds(0.1f);
			if (!isVisible)
			{
				elapsedTime += 0.1f;
			}
			if (elapsedTime > respawnTime && !isVisible)
			{
				respawning = false;
				spawned = true;
                enemyDead = false;
				GameObject go = Spawn();
                GameObject shieldGO = go.transform.Find("Shield")?.gameObject;
                if (shieldGO == null)
                {
                    break;
                }
                Shield s = shieldGO.GetComponent<Shield>();
                shieldHP += shieldIncrease;
                s.SetShieldHP(shieldHP);
                s.ActivateShield();
                
			}
		}
	}

	private void OnBecameInvisible()
	{
		isVisible = false;
		if (!respawning && enemyDead)
		{
			respawning = true;
			StartCoroutine(Respawn());
		}
	}

	private void OnBecameVisible()
	{
		isVisible = true;
	}
}
