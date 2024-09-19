using UnityEngine;

public class ObjectsStatisticController : MonoBehaviour
{
	public static ObjectsStatisticController I;

	[SerializeField] ObjectsStatistic _objectsStatistic;

	private void Awake()
	{
		I = this;
		_objectsStatistic.RegisterLog();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			if (GameControls.I != null)
			{
				GameControls.I.MenuControls(!_objectsStatistic.show_console);
			}

			_objectsStatistic.show_console = !_objectsStatistic.show_console;
			_objectsStatistic.enabled = _objectsStatistic.show_console;
		}
	}

	public void ShowConsole(bool show)
	{
		if (GameControls.I != null)
		{
			GameControls.I.MenuControls(show);
		}

		_objectsStatistic.show_console = show;
		_objectsStatistic.enabled = show;
	}
}
