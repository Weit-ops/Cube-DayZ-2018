using UnityEngine;

public class RespawnPackCountView : MonoBehaviour
{
	[SerializeField] int _id;
	[SerializeField] tk2dTextMesh _countLabel;

	private void OnEnable()
	{
		UpdateCount();
	}

	public void UpdateCount()
	{
		int num = 0;
		if (!DataKeeper.IsUserDummy)
		{
			PurchasedItemsBackensInfo purchasedItemsBackensInfo = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == _id);
			if (purchasedItemsBackensInfo != null)
			{
				num = purchasedItemsBackensInfo.count;
			}
		}
		_countLabel.text = num.ToString();
		_countLabel.color = ((num <= 0) ? Color.red : Color.green);
	}
}
