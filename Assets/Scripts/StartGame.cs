using UnityEngine;

public class StartGame : MonoBehaviour
{
	public GameObject Player;
	public float BridgeTimer = 30f;
	public GameObject Bridge1;
	public GameObject Bridge2;
	public GameObject Bridge3;
	public GameObject Bridge4;
	public GameObject Bridge5;
	public GameObject Bridge6;
	public GameObject Bridge7;
	public GameObject Bridge8;

	private void Start()
	{
		
	}

	private void Update()
	{
		BridgeTimer -= Time.deltaTime;
		if (BridgeTimer <= 0f)
		{
			EnableBridge(Bridge1);
			EnableBridge(Bridge2);
			EnableBridge(Bridge3);
			EnableBridge(Bridge4);
			EnableBridge(Bridge5);
			EnableBridge(Bridge6);
			EnableBridge(Bridge7);
			EnableBridge(Bridge8);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Player")
		{
			Debug.Log("Quick registration done. Spawn player in personal waiting room");
			Player.transform.position = new Vector3(-190f, 101f, -45f);
		}
	}

	private void EnableBridge(GameObject bridge)
	{
		bridge.SetActive(true);
	}
}
