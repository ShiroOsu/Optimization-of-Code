using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class IntEvent : UnityEvent<int>
{
}
public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;
	//private float timer = 100;
	private int levelScore = 0;

	public IntEvent timeChanged;
	public IntEvent scoreChanged;

    public GameObject player;
	public CargoSpawner cargoSpawner;

	private bool endOnce = false;

	
	public int LevelScore {
		get => levelScore;
		set
		{
			levelScore = value;
			scoreChanged.Invoke(levelScore);
		}
	}


	private void Awake()
	{
		if (!instance)
		{
			instance = this;
			
			cargoSpawner = FindObjectOfType<CargoSpawner>();
			// If starting the game from level scene, turn off ui fader
			if (!GameManager.instance)
			{
				var transition = GameObject.Find("transitionFader")?.GetComponent<Image>();
				transition.color = new Color(0, 0, 0, 0);
			}
		}
		else
		{
			Destroy(gameObject);
		}
        player = GameObject.FindWithTag("Player");
	}

	private void FixedUpdate()
	{
		//timer -= Time.fixedDeltaTime;
		//timeChanged.Invoke((int)timer);

		if(player == null && !endOnce)
		{
			endOnce = true;
			if(GameManager.instance)
			{
				GameManager.instance.EndLevel(LevelScore);
			}
			//Debug.Log("Player died, game over");
		}
	}
}
