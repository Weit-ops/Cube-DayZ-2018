using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RCCReset_Scene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(ControlFreak2.CF2Input.GetKeyUp(KeyCode.R)){
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
	
	}
}
