using UnityEngine;

public class ReskinHandManager : MonoBehaviour
{
	public GameObject[] Cubes;
	public GameObject[] Reskin;
	public WeaponBehavior Wb;

	private void Awake()
	{
		if (JsSpeeker.I.ReskinType != 0)
		{
			Wb = base.gameObject.GetComponent<WeaponBehavior>();
			if (Wb != null && Reskin[0] != null)
			{
				Wb.weaponMesh = Reskin[0];
			}
		}
	}

	private void OnEnable()
	{
		if (JsSpeeker.I != null)
		{
			if (JsSpeeker.I.ReskinType == ReskinGameType.Default)
			{
				ToggleArray(Reskin, false);
				ToggleArray(Cubes, true);
			}
			else
			{
				ToggleArray(Cubes, false);
				ToggleArray(Reskin, true);
			}
		}
	}

	private void ToggleArray(GameObject[] array, bool flag)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
	}
}
