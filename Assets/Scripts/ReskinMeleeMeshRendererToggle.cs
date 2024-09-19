using UnityEngine;

public class ReskinMeleeMeshRendererToggle : MonoBehaviour
{
	public MeshRenderer[] Cubes;
	public MeshRenderer[] Reskin;

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

	private void ToggleArray(MeshRenderer[] array, bool flag)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = flag;
		}
	}
}
