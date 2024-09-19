using Photon;

public class BaseWorldObjectAction : MonoBehaviour
{
	public virtual bool CanUse
	{
		get
		{
			return true;
		}
	}

	public virtual void Use()
	{
		
	}

	public virtual void OnHit()
	{
		
	}

	public virtual void OnObjectDestroy()
	{
		
	}
}
