using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class IDGetter : MonoBehaviour {

	public static string sid =null;
	public static string tid =null;

	void Start () {
		#if !UNITY_EDITOR && UNITY_WEBGL
		WebGLInput.captureAllKeyboardInput = false;
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

		// デバッグ用
		if (Input.GetKeyDown(KeyCode.A))
		{
//			sid = "361947";	// 小峰さん
//			sid = "361943"; // 中馬さん
//			sid = "361944"; // 香田さん
//			sid = "361957";	// 木本さん
//			sid = "361958";
//			sid = "362169";
//			sid = "362173";
			sid = "362228";
//			sid = "362175";


			// pass hoge1234
			tid = "1";
//			tid = "6";
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