using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class VoiceLoader : MonoBehaviour {
	public bool hasLoad = false;
	
	string sid;
	string tid;
	public WWW www;
	public AudioSource source=null;

	void Start () {

		source = GetComponent<AudioSource> ();
		sid = IDGetter.sid;
		tid = IDGetter.tid;
		StartCoroutine("Load");
	}

	private IEnumerator Load()
	{
		hasLoad = false;
		if (sid == null || tid == null)
			yield return null;
//		WWW www = new WWW("http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/voice/300_1.mp3");
		#if !UNITY_EDITOR
		www = new WWW("http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/voice/"+sid+"_"+tid+".mp3");
		Debug.Log ("http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/voice/"+sid+"_"+tid+".mp3");
		#else
		www = new WWW("http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/voice/"+sid+"_"+tid+".ogg");
		Debug.Log ("http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/voice/"+sid+"_"+tid+".ogg");
		#endif


		yield return www;
//		Debug.Log (www.progress);
		if (!string.IsNullOrEmpty (www.error)) {
			Debug.LogWarning (www.error.ToString());
			Debug.Log("voice error");
//			Application.ExternalCall("xmlLoadFlag",-1);
//			SceneManager.LoadScene("LoadScene");
		}
		#if !UNITY_EDITOR
		Debug.LogError("Load:MEPG");
		source.clip = www.GetAudioClip(true,false,AudioType.MPEG);
		#else
		Debug.LogError("Load:OGG");
		source.clip = www.GetAudioClip(true,false,AudioType.OGGVORBIS);
		#endif
		//Debug.Log("DownloadSize:"+www.size/1024/1024+"Mbyte");
		while (true)
		{
			if (source.clip.length > 0f) {
				Debug.Log ("CallsoundLength:" + source.clip.length);
				Application.ExternalCall ("soundLength", source.clip.length);
				hasLoad = true;
				break;
			} else {
				yield return null;
			}
		}
	}

//	public IEnumerator CallSoundLength()
//	{
//		yield return null;
//
//		Debug.Log ("soundLength:" + source.clip.length);
//		Application.ExternalCall ("soundLength", source.clip.length);
//		hasLoad = true;
//	}
}