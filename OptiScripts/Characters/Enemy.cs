public class Enemy : Character
{
    protected float damage;
    protected float missileHitDamage;

	private void OnDestroy()
	{
		var spawner = GetComponentInParent<EnemySpawner>();
		if(spawner)
		{
			spawner.StartSpawn();
		}
	}
}
