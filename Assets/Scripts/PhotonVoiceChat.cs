using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonVoiceRecorder))]
[RequireComponent(typeof(PhotonVoiceSpeaker))]
public class PhotonVoiceChat : Photon.MonoBehaviour
{
	public PhotonVoiceRecorder recorder;
	public KeyCode voiceKey = KeyCode.V;

	void Update()
	{
		if (GetVoiceButton() && base.photonView.isMine)
		{
			recorder.Transmit = !recorder.Transmit;
		}
	}

	bool GetVoiceButton()
	{
		return Input.GetKeyDown(voiceKey);
	}
}