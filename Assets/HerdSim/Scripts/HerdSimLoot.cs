using UnityEngine;

public class HerdSimLoot : MonoBehaviour
{
	[SerializeField] string _peltId;
	[SerializeField] [Range(0f, 100f)] int _dropPeltChance;
	[SerializeField] string _meatId;
	[SerializeField] string _moldyMeatId;
	[SerializeField] byte _minPeltCount;
	[SerializeField] byte _maxPeltCount;
	[SerializeField] byte _minMeatCount;
	[SerializeField] byte _maxMeatCount;

	public void DropMeat(bool killedByPlayer)
	{
		string text = _meatId;
		if (killedByPlayer)
		{
			WorldController.I.PlayerStatistic.CreatureKills++;
		}
		else if (!string.IsNullOrEmpty(_moldyMeatId))
		{
			text = _moldyMeatId;
		}
		if (!string.IsNullOrEmpty(text))
		{
			byte b = (byte)UnityEngine.Random.Range(_minMeatCount, _maxMeatCount + 1);
			HerdSimLootController.I.photonView.RPC("DropLoot", PhotonTargets.MasterClient, text, b, base.transform.position);
		}
		if (!string.IsNullOrEmpty(_peltId))
		{
			int num = UnityEngine.Random.Range(0, 100);
			if (num < _dropPeltChance)
			{
				byte b2 = (byte)UnityEngine.Random.Range(_minMeatCount, _maxMeatCount + 1);
				HerdSimLootController.I.photonView.RPC("DropLoot", PhotonTargets.MasterClient, _peltId, b2, base.transform.position);
			}
		}
	}
}
