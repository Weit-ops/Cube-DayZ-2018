using UnityEngine;

public class CompasCubeDayZ
{
    // Token: 0x06000395 RID: 917 RVA: 0x00021914 File Offset: 0x0001FB14
    public static CompasCubeDayZ.GeneralDirection GetDirection(Vector3 pointOfOrigin, Vector3 vectorToTest)
    {
        CompasCubeDayZ.GeneralDirection result = CompasCubeDayZ.GeneralDirection.None;
        float num = float.PositiveInfinity;
        Vector3 vector = pointOfOrigin + vectorToTest;
        float num2 = Mathf.Abs((pointOfOrigin + Vector3.forward - vectorToTest).magnitude);
        if (num2 < num)
        {
            num = num2;
            result = CompasCubeDayZ.GeneralDirection.North;
        }
        num2 = Mathf.Abs((pointOfOrigin - Vector3.forward - vectorToTest).magnitude);
        if (num2 < num)
        {
            num = num2;
            result = CompasCubeDayZ.GeneralDirection.South;
        }
        num2 = Mathf.Abs((pointOfOrigin + Vector3.left - vectorToTest).magnitude);
        if (num2 < num)
        {
            num = num2;
            result = CompasCubeDayZ.GeneralDirection.West;
        }
        num2 = Mathf.Abs((pointOfOrigin + Vector3.right - vectorToTest).magnitude);
        if (num2 < num)
        {
            result = CompasCubeDayZ.GeneralDirection.East;
        }
        return result;
    }

    // Token: 0x020000A0 RID: 160
    public enum GeneralDirection
    {
        // Token: 0x04000693 RID: 1683
        None,
        // Token: 0x04000694 RID: 1684
        North,
        // Token: 0x04000695 RID: 1685
        South,
        // Token: 0x04000696 RID: 1686
        West,
        // Token: 0x04000697 RID: 1687
        East,
        // Token: 0x04000698 RID: 1688
        Up,
        // Token: 0x04000699 RID: 1689
        Down
    }
}

public class UIVersion : MonoBehaviour
{
	public static string GetMinorVersion()
	{
		return DataKeeper.BuildVersion + " a023";
	}

	private void Start()
	{
		GetComponent<tk2dTextMesh>().text = GetMinorVersion();
	}

	private void Update()
	{
		CalculateCompasValue();
	}
    private void OnGUI()
    {
        //GUILayout.Label("\n\n" + CompasCubeDayZ.GetDirection(GameControls.I.Player.transform.TransformPoint(Vector3.forward), GameControls.I.Player.transform.TransformDirection(Vector3.forward)));
    }
	private void CalculateCompasValue()
	{
	}
}
