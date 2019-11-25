using UnityEngine;
using System.Collections;
public class EnemySpawner : MonoBehaviour
{
	public float respawnTime;
    [SerializeField]
    private Vector3 positionOffset;
	private float elapsedTime;
	public GameObject enemyPrefab;
	private bool enemyDead = false;
	private bool isVisible = false;
	private bool respawning = false;
    private float shieldHP = 0f;
    private float shieldIncrease = 50f;


	private void Awake()
	{
		Spawn();
	}
	public void StartSpawn()
	{
		Debug.Log("start spawn");
		enemyDead = true;
	}

	private GameObject Spawn()
	{
		var go = Instantiate(enemyPrefab, transform.position + positionOffset.magnitude * transform.up, Quaternion.identity, transform);
        return go;
	}

	private IEnumerator Respawn()
	{
		elapsedTime = 0f;
		bool spawned = false;
		while (!spawned)
		{
			Debug.Log("waiting");
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
                Debug.Log(go.name);
                GameObject shieldGO = go.transform.Find("Shield")?.gameObject;
                if (shieldGO == null)
                {
                    Debug.Log("Enemy did not contain shield: " + gameObject.name);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
