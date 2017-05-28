using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class IDGetter : MonoBehaviour {

	public static string sid =null;
	public static string tid =null;

	void Start () {
		#if !UNITY_EDITOR && UNITY_WEBGL
//		WebGLInput.captureAllKeyboardInput = false;
		#endif
		sid = null;
		tid = null;
		Application.ExternalCall ("UnityLoadFlag", 1);
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<GUIText>().text = "sid:"+sid+"\ntid:"+tid;
//		Debug.Log (!string.IsNullOrEmpty (sid)+"||"+!string.IsNullOrEmpty (tid));
		if (string.IsNullOrEmpty (sid) == false && string.IsNullOrEmpty (tid) == false){
			InitCall ();
//			Debug.Log(sid+","+tid);

		}

		//デバッグ用,Aキーを押したらsid300,tid1を入れる
		if (Input.GetKeyDown(KeyCode.A)) {
//			sid = "361782";
//			sid = "361764";
//			sid = "361745";
//			sid = "361822";
//			sid = "300"; // テストデータ(主要テストデータ。模擬授業)
//			sid = "289"; // テストデータ(人形の向きの調査など)
			//sid = "361806"; //mid + 1000ver
//			sid = "361897";
//			sid = "361927";
//			sid = "361930";
//			sid = "361958";
//			sid = "361806";
			sid = "361957";
			tid ="1";
		}

	}

	void InitCall()
	{
		SceneManager.LoadScene("EduceBoard");
//		if (sid != null && tid != null) {
//			
//		}
	}

	void GetSID(string getSID){
		sid = getSID;
	}
	void GetTID(string getTID){
		tid = getTID;
	}


}