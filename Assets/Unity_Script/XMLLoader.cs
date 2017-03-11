using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using MiniJSON;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization;
using UnityEngine.SceneManagement; 

public class XMLLoader : MonoBehaviour {
	#region 読み込み関連
	string sid;
	string tid;
	string xmlid = "2";//XMLを読み込むなら1(現在非対応)、JSONを読み込むなら2
	
	string XMLFilePath = null;
	WWW getXML;
	public bool XMLHasLoad = false;

	//読み込んだデータ
	///<summary>全アクターの位置情報のリストを保持する二重リスト</summary>
	List<List<LocationData>> DataList = new List<List<LocationData>>();
	///<summary>各アクターの時間毎の座標・方向情報を保持するリスト</summary>
	List<GameObject> actorList = new List<GameObject>();

	Dictionary <string, int> midDic = new Dictionary<string, int> ();
//	Dictionary <GameObject,Vector3> posDic = new Dictionary<GameObject, Vector3> ();
	#endregion

	#region モデル関連
	public GameObject male;
	public GameObject female;
	public GameObject teacher;
	public GameObject desk;

	const string PREFAB_MODEL_PATH = "Models/";
	const string MODEL_MALE = "edBoy_fix";
	const string MODEL_FEMALE = "edGirl_fix";
	const string MODEL_MALE_TEACHER = "teacher_man_fix";
	const string MODEL_FEMALE_TEACHER = "teacher_female_fix";
	const string MODEL_DESK = "desk";

	const int MID_CHECK_SKIP_COUNT = 10;
	#endregion

	#region 状態管理
	public enum STEP
	{
		NONE 	= 0,
		LOAD 	= 1,
		READY	= 2,
		PLAY	= 3,
		PAUSE	= 4,
	}

	public float time = 0.1f;
	float audioTime =0;

	VoiceLoader voiceLoader;

	STEP step = STEP.NONE;

//	[SerializeField] int isStart = 0;
//	[SerializeField] int isPlay = 0;
//	[SerializeField] bool readiness = false;
	#endregion

	void Start () {

		if (string.IsNullOrEmpty (IDGetter.sid) || string.IsNullOrEmpty (IDGetter.tid)) {
			Debug.Log ("<color=red>[ID Error]:sid or tid is null. move to LoadScene</color>");
			SceneManager.LoadScene("LoadScene");
//			GetComponent<GUIText>().text = "ID error";
			return;

		} else {
			sid = IDGetter.sid;
			tid = IDGetter.tid;
		}

		voiceLoader = GetComponent<VoiceLoader> ();

		//ダウンロード先のJSONのパス
		XMLFilePath = "http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/readMarkersLocator2.php?session_id="+sid+"&tid="+tid+"&xml="+xmlid;
		//XMLFilePath = Application.dataPath + "/XML"+"/readMarkersLocator.php.xml"; //local
		//Debug.Log("XMLFilePath is :" + XMLFilePath);

		StartCoroutine("Load");
		StartCoroutine("Relocate",0.1f);

	}

	//位置情報をロードする関数
	private IEnumerator Load()
	{
		step = STEP.LOAD;
		Debug.Log("<color=blue>Load Start</color>");
		getXML = new WWW (XMLFilePath);
		Debug.Log("<color=blue>"+XMLFilePath+"</color>");
//		GetComponent<GUIText>().text = "Loading...";
		yield return getXML;

		if (!string.IsNullOrEmpty (getXML.error)) {
			Debug.Log (getXML);
			Debug.Log("<color=red>XML error</color>");
			Application.ExternalCall("xmlLoadFlag",-1);
			SceneManager.LoadScene("LoadScene");
		}


		#region parse data
		//データをパースする
		Debug.Log("<color=blue>Parse Start</color>");
		string json = getXML.text;
        IList familyList = (IList)Json.Deserialize(json);
        foreach(IDictionary person in familyList){
            int   mid  = Convert.ToInt32(person["mid"]);
            float ts   = Convert.ToSingle(person["TS"]);
            float posx = Convert.ToSingle(person["x"])*(float)0.10;
            float posy = 8-Convert.ToSingle(person["y"])*(float)0.10;
            float dirx = Convert.ToSingle(person["d1"]);
            int   col  = Convert.ToInt32(person["status"]);
			if (!midDic.ContainsKey ("mid" + mid)) {
				midDic.Add ("mid" + mid, 1);
			} else {
				midDic ["mid" + mid] += 1;
			}

			if (mid >= 1000) {
				if (mid - 1000 >= DataList.Count) {
					DataList.Add (new List<LocationData> ());
				} else {
//					Debug.Log ("datalistの長さ" + DataList.Count);
					DataList [mid - 1001].Add (new LocationData (mid, ts, posx, posy, col, dirx));
				}
			} else {
				if (mid >= DataList.Count) {
					DataList.Add (new List<LocationData> ());
				} else {
					DataList [mid].Add (new LocationData (mid, ts, posx, posy, col, dirx));
				}
			}
        }
		/*各アクターの位置情報のデバック用出力。あまりおおきいデータだとこのデバック出力のせいでUnityがフリーズします。
		foreach (List<LocationData> List in DataList) {
			Debug.Log(List.Count);
			foreach (LocationData Data in List){
				if(1 <= List.Count){
					Debug.Log("mid："+Data.mid+"、ts："+Data.ts+"、posx："+Data.posx+"、posy："+Data.posy);
				}
			}
		}*/
		Debug.Log("<color=blue>Parse Complete</color>");
		#endregion


		#region create 3d objects
		Debug.Log("<color=blue>Create 3d Objects</color>");
        /*3Dオブジェクトの作成*/
		foreach (List<LocationData> List in DataList) {
//			Debug.Log (List.Count);
			GameObject actor;
			if(1 <= List.Count){
				Vector3 pos    = new Vector3(List[0].posx,(float)0.3,List[0].posy);
				Quaternion dir = Quaternion.Euler(0,List[0].dirx,0);
				Vector3 tpos = new Vector3(List[0].posx,(float)1,List[0].posy);
//				Debug.LogWarning("mid"+List[0].mid+" is "+Who(List[0].mid));
				if (midDic ["mid" + List [0].mid] > MID_CHECK_SKIP_COUNT) {
					switch (Who (List [0].mid)) {
					case "male":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_MALE), pos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						actorList.Add (actor);
						break;
					case "female":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_FEMALE), pos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						actorList.Add (actor);
						break;
					case "teacher":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_MALE_TEACHER), tpos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						actorList.Add (actor);;
						break;
					case "teacher_woman":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_FEMALE_TEACHER), tpos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						actorList.Add (actor);
						break;
					case "desk":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_DESK), pos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						actorList.Add (actor);
						break;
					default:
						break;
					}
				}
			}
			
		}
		Debug.Log("<color=blue>Copmlete Create 3d Objects</color>");
		#endregion

		XMLHasLoad = true;
		Application.ExternalCall("xmlLoadFlag",1);
		time = 0;
	}



	void Update () {
		

		switch(step)
		{
		case STEP.NONE:
			return;
		case STEP.LOAD:
			if (XMLHasLoad == false) {
				Application.ExternalCall ("xmlProgress", getXML.progress);
			}
			if (voiceLoader.hasLoad == false) {
				Application.ExternalCall ("voiceProgress", voiceLoader.www.progress);
			}
			
			if (voiceLoader.hasLoad && XMLHasLoad) {
				step = STEP.READY;
			}
			break;
		case STEP.READY:
				if (Input.GetKeyDown (KeyCode.Space)) {
				playFlag (1);
				}
			break;
		case STEP.PLAY:
			time -= Time.deltaTime;
			audioTime = voiceLoader.GetComponent<AudioSource>().time;

			if(time<=0){
				Application.ExternalCall("soundPosition",voiceLoader.source.time);
				//time = 0.5f;
				time = 0.1f;
			}
			if (Input.GetKeyDown (KeyCode.Space)) {
				playFlag (0);
			}
			break;
		case STEP.PAUSE:
			break;

//			if(isStart==0 && isPlay==1){
//				isPlay = 0;
//				voiceLoader.GetComponent<AudioSource>().Pause();
//				audioTime =voiceLoader.GetComponent<AudioSource>().time;
//				//Debug.Log(voiceLoader.GetComponent<AudioSource>().time);
//			}
		}
		Debug.Log (step);
		//Debug.Log("音声ロード進捗 = "+voiceLoader.www.progress+", XMLロード進捗 = "+getXML.progress+", 再生中か = "+(isStart==0)+"データリストの長さ："+DataList.Count);
	}

	void playFlag(int flag){
//		isStart = flag;
		if (flag == 1) {
			step = STEP.PLAY;
//			isPlay = 1;
//			GetComponent<GUIText>().text = "Now playing";
			voiceLoader.GetComponent<AudioSource>().time = audioTime;
			voiceLoader.GetComponent<AudioSource>().Play ();
		}else if(flag ==0){
//			isPlay = 0;
			step = STEP.PAUSE;
			voiceLoader.GetComponent<AudioSource>().Pause();
			audioTime =voiceLoader.GetComponent<AudioSource>().time;
			//Debug.Log(voiceLoader.GetComponent<AudioSource>().time);
			
		}
	}
	
	void soundPosition(float pos){
		voiceLoader.GetComponent<AudioSource>().time = pos;
	}

	void SetSoundValue(float val){
		voiceLoader.GetComponent<AudioSource>().volume = val;
	}

	#region reloacate
	//audiotimeと0.03秒以下の誤差のLocationDataがあればそれを、なければnullを返します。
	private LocationData getActorLocation(List<LocationData> tempList){
		foreach (LocationData Data in tempList){
			if(Mathf.Abs(audioTime - Data.ts )<= 0.03){
				return Data;
			}
		}
		return null;
	}

	//time秒ごとに位置情報を更新する関数
	private IEnumerator Relocate(float time)
	{
		while(true){
		/*改善ポイント*/
//		Debug.Log(audioTime);

		foreach (List<LocationData> List in DataList) {
			
			LocationData Data = getActorLocation(List);
			//二重foreachの見た目を避け可読性をあげると以下の条件分岐が入ってしまうので、関数を使わないほうがいいかも
			if(1 <= List.Count&&Data!=null){
				GameObject actor = GameObject.Find("mid"+Data.mid.ToString());
				Vector3 tmppos = actor.transform.position;
				Vector3 tmpdir = new Vector3 (actor.transform.rotation.x, actor.transform.rotation.y, actor.transform.rotation.z);
							
				

				/*if(Data.mid>=1 && 8>=Data.mid){
//					switch(Data.col){
//					case 0:
//						actor.transform.FindChild("eye1").gameObject.SetActive(true);
//						actor.transform.FindChild("eye2").gameObject.SetActive(false);
//						actor.transform.FindChild("eye3").gameObject.SetActive(false);
//						actor.transform.FindChild("note").gameObject.SetActive(false);
//					break;
//					case 1:
//						actor.transform.FindChild("eye1").gameObject.SetActive(false);
//						actor.transform.FindChild("eye2").gameObject.SetActive(false);
//						actor.transform.FindChild("eye3").gameObject.SetActive(true);
//						actor.transform.FindChild("note").gameObject.SetActive(false);
//					break;
//					case 2:
//						actor.transform.FindChild("eye1").gameObject.SetActive(false);
//						actor.transform.FindChild("eye2").gameObject.SetActive(true);
//						actor.transform.FindChild("eye3").gameObject.SetActive(false);
//						actor.transform.FindChild("note").gameObject.SetActive(true);
//					break;
//					default:break;
//					}
				}*/

//				float movex = Data.posx - actor.transform.position.x;
//				float movez = Data.posy - actor.transform.position.z;
//				float diry = Data.dirx - actor.transform.rotation.y;
//				actor.transform.Translate(movex,0,movez,Space.World);
					var finpos = new Vector3 (Data.posx, actor.transform.position.y, Data.posy);
//				actor.transform.position = Vector3.Lerp (actor.transform.position, finpos,Time.deltaTime);

//						actor.transform.eulerAngles = new Vector3(0f,Mathf.LerpAngle(actor.transform.eulerAngles.y,diry,1f),0f);



					actor.transform.position = Vector3.Lerp (actor.transform.position, finpos,0.7f);
//					var rot = Quaternion.Euler(0,Data.dirx , 0);
				tmpdir.y = Data.dirx;
//					actor.transform.rotation = Quaternion.Slerp (actor.transform.rotation, rot, 0.5f);
						actor.transform.eulerAngles = tmpdir;
//Debug.Log("mid"+Data.mid.ToString() + "movex = "+movex+", movey = 0 , movez = "+ movez +", diry = " + diry);

			}
		}
			yield return new WaitForSeconds(time);
		}
	}

	//アクターのmidから男子生徒、女子生徒、教師、机のどれかを判断します。
	string Who(int mid){
//		Debug.Log (mid);
		string who = "";
		if (mid >= 1000)
			mid -= 1000;
		
		//20161030時点で以下のmidが+1000に変更された。IDgetterの箇所を参照し以下のcaseを+-1000する
		switch(mid){
			case 1:
			case 2:
			case 3:
			case 4:
				who ="male";
				break;
			case 5:
			case 6:
			case 7:
			case 8:
				who = "female";
				break;
			case 9:
				who = "teacher";
				break;
			case 10:
				who ="teacher_woman";
				break;
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
				who = "desk";
				break;
			default:
				break;
		}
		return who;
	}
	#endregion
}



//位置情報のクラス
public class LocationData{
	public int mid;
	public float ts;
	public float posx;
	public float posy;
	public float dirx;
	public int col;

	public LocationData(int getMid,float getTs,float getposX,float getposY,int getColor,float getdirX){
		mid = getMid;
		ts = getTs;
		posx = getposX;
		posy = getposY;
		col = getColor;
		dirx = getdirX;

	}
}