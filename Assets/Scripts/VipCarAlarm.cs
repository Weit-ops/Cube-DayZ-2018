using System.Collections;
using UnityEngine;

public class VipCarAlarm : MonoBehaviour
{
	public GameObject Alarm;

	public bool alarmWork;

	public void StartAlarm()
	{
		if (!alarmWork)
		{
			StartCoroutine("AlarmSound");
		}
	}

	private IEnumerator AlarmSound()
	{
		alarmWork = true;
		Alarm.SetActive(true);
		yield return new WaitForSeconds(3f);
		CarWrapper cw = GetComponent<CarWrapper>();
		if (cw != null)
		{
			cw.StartChangeCarVipToCommon();
		}
		Alarm.SetActive(false);
		yield return new WaitForSeconds(1f);
		alarmWork = false;
	}
}
