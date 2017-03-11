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
	void Update (){
	}
	private IEnumerator Load()
	{
		if (sid == null || tid == null)
			yield return null;
		//WWW www = new WWW("http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/voice/300_1.mp3");
		www = new WWW("http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/voice/"+sid+"_"+tid+".ogg");
		Debug.Log ("http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/voice/"+sid+"_"+tid+".ogg");

		yield return www;
		if (!string.IsNullOrEmpty (www.error)) {
			Debug.LogWarning (www.error.ToString());
			Debug.Log("voice error");
			Application.ExternalCall("xmlLoadFlag",-1);
			SceneManager.LoadScene("LoadScene");
		}
		source.clip = www.GetAudioClip();

		//Debug.Log("DownloadSize:"+www.size/1024/1024+"Mbyte");
		hasLoad = true;
		if (source.clip.length != 0) {
				Application.ExternalCall ("soundLength", source.clip.length);
		}
	}
}