using UnityEngine;
public class GroundScroll : MonoBehaviour
{
	[Tooltip("Assign ground parts from left to right")]
	public GameObject[] groundTiles;
	private Transform player;
	public float tileWidth;

	private float leftMargin;
	private float rightMargin;

	public LayerMask cargoLayer;

	private void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		leftMargin = 0;
		rightMargin = 2*tileWidth;
	}

	private void FixedUpdate()
	{
        if (!(groundTiles.Length > 1))
            return;
		if (player.position.x <= leftMargin)
			ShiftLeft();
		if (player.position.x >= rightMargin)
			ShiftRight();
	}

	private void ShiftLeft()
	{
		leftMargin -= tileWidth;
		rightMargin -= tileWidth;
		var temp = groundTiles[groundTiles.Length - 1];

		//Move all objects in the zone
		var objects = GetObjectsInCollider(temp);
		foreach (var o in objects)
		{
			var objpos = o.transform.position;
			objpos.x -= 4 * tileWidth;
			o.transform.position = objpos;
		}

		//Move the zone
		for (int i = groundTiles.Length-1; i > 0; i--)
		{
			groundTiles[i] = groundTiles[i - 1];
		}
		var pos = temp.transform.position;
		pos.x -= 4*tileWidth;
		temp.transform.position = pos;
		groundTiles[0] = temp;
		
	}

	private void ShiftRight()
	{
		leftMargin += tileWidth;
		rightMargin += tileWidth;

		

		var temp = groundTiles[0];
		//Move all objects in the zone
		var objects = GetObjectsInCollider(temp);
		foreach (var o in objects)
		{
			var objpos = o.transform.position;
			objpos.x += 4 * tileWidth;
			o.transform.position = objpos;
		}
		// Move the zone
		for (int i = 0; i < groundTiles.Length - 1; i++)
		{
			groundTiles[i] = groundTiles[i + 1];
		}
		var pos = temp.transform.position;
		pos.x += 4 * tileWidth;
		temp.transform.position = pos;
		groundTiles[groundTiles.Length-1] = temp;

		
	}

	private Collider2D[] GetObjectsInCollider(GameObject go)
	{
		var col = go.GetComponent<BoxCollider2D>();


		Collider2D[] objs;
		objs = Physics2D.OverlapBoxAll(col.transform.position, col.size, 0, cargoLayer);
		return objs;
	}
}
