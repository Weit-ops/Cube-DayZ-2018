using UnityEngine;

public class SkinColor : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite _skin;

	[SerializeField]
	private tk2dUIToggleButton _toggleButton;

	[SerializeField]
	private GameObject _vipBlock;

	public tk2dUIToggleButton Toggle
	{
		get
		{
			return _toggleButton;
		}
	}

	public void SetColor(Color color, bool showVipBlock)
	{
		_skin.color = color;
		_vipBlock.SetActive(showVipBlock);
	}
}
