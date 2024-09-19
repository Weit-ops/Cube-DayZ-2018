using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FriendManager : MonoBehaviour {

	public Text t;
	public Image i;

	public void setUpImage(byte[] photo)
	{
		var tex=new Texture2D(200,200);
		tex.LoadImage(photo);
		i.sprite=Sprite.Create(tex,new Rect(0,0,200,200),new Vector2(0.5f,0.5f));

	}


}
