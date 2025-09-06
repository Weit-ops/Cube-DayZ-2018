using UnityEngine;

public class VipCarManager : MonoBehaviour
{
	public GameObject[] VipCars;

	[SerializeField]
	private GameObject _vipCarMenu;

	public static VipCarManager I;

	public bool InTheCar;

	private bool showMenuFlag = true;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	private void Update()
	{
		if (InTheCar && ControlFreak2.CF2Input.GetKeyDown(KeyCode.B))
		{
			ShowSelectMenu(showMenuFlag);
		}
	}

	public void ShowSelectMenu(bool flag)
	{
		_vipCarMenu.SetActive(flag);
		showMenuFlag = !showMenuFlag;
	}
}
