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
	int packId = 0;
	int packNum = 1;
	
	string XMLFilePath = null;
	string JsonFilePath = null;
	WWW getXML;
	public bool XMLHasLoad = false;

	//読み込んだデータ
	///<summary>全アクターの位置情報のリストを保持する二重リスト</summary>
	List<List<LocationData>> DataList = new List<List<LocationData>>();
	///<summary>各アクターの時間毎の座標・方向情報を保持するリスト</summary>
//	List<GameObject> actorList = new List<GameObject>();

//	List<Dictionary <string,string>> jsonData = new List<Dictionary<string, string>>();

	Dictionary <string, ActorData> actorDict = new Dictionary<string, ActorData> ();
//	Dictionary <GameObject,Vector3> posDic = new Dictionary<GameObject, Vector3> ();
	#endregion

	#region モデル関連
	//モデルの参照
	public GameObject male;
	public GameObject female;
	public GameObject teacher;
	public GameObject desk;

	//PrefabPath指定
	const string PREFAB_MODEL_PATH = "Models/";
	const string MODEL_MALE = "edBoy_fix";
	const string MODEL_FEMALE = "edGirl_fix";
	const string MODEL_MALE_TEACHER = "teacher_man_fix";
	const string MODEL_FEMALE_TEACHER = "teacher_female_fix";
	const string MODEL_DESK = "desk";

	//ノイズMIDを排除する値(この値以上カウントされるとデータとして適用する)
	const int MID_CHECK_SKIP_COUNT = 0;

	//閾値(この値以上になると更新される値)の設定
	const float ROT_CHECK_SKIP_VALUE = 0f;	// Rotationの閾値
	const float POS_CHECK_SKIP_VALUE = 0f;	// Positionの閾値

	const float MOVE_SPEED = 5f;
	const float ROTATION_SPEED = 5f;
	#endregion

	#region 状態管理
	/// <summary>遷移状態</summary>
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
	bool isSeekMove = false;

	VoiceLoader voiceLoader;
	STEP step = STEP.NONE;

	#endregion

	void Start () {


		#if !UNITY_EDITOR && UNITY_WEBGL
//		WebGLInput.captureAllKeyboardInput = false;
		#endif
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
//		XMLFilePath = "http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/readMarkersLocator2.php?session_id="+sid+"&tid="+tid+"&xml="+xmlid;
		XMLFilePath = "http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/readMarkersLocator5.php?session_id="+sid+"&tid="+tid+"&xml="+xmlid+"&json=1";

//		XMLFilePath = "http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/readMarkersLocator4.php?session_id="+sid+"&tid="+tid+"&xml="+xmlid+"&json=1";
				JsonFilePath = "http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/readMarkersLocator5.php?session_id="+sid+"&tid="+tid+"&xml="+xmlid+"&json=1&pack_id=";
		//XMLFilePath = Application.dataPath + "/XML"+"/readMarkersLocator.php.xml"; //local
		//Debug.Log("XMLFilePath is :" + XMLFilePath);

		StartCoroutine("Load");
	}


	[Serializable]
	public class data
	{
		public List<table> Items;
	}
		
	[Serializable]
	public class Serialization<T>
	{
		[SerializeField]
		List<T> target;
		public List<T> ToList() { return target; }

		public Serialization(List<T> target)
		{
			this.target = target;
		}
	}

	[Serializable]
	public class table
	{
		public int mid;		//	オブジェクトの識別id
		public float TS;	//タイムスタンプ
		public float x;		//x座標
		public float y;		//z座標
		public float d1;	//yのRotation
		public int color;	//0:普通,1:寝る,2:真面目
		public int status;	// 調整後のTS
	}

	//位置情報をロードする関数
	private IEnumerator Load()
	{
		step = STEP.LOAD;
		Debug.Log("<color=blue>Load Start</color>");
		getXML = new WWW (XMLFilePath);
		Debug.Log("<color=blue>"+XMLFilePath+"</color>");
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
		packNum = int.Parse(json);

		this.JsonCheck();

		#endregion
	}


	//jsonデータを取りに行く
	void JsonCheck()
	{
//		Debug.Log ("packID:"+packId);
			var url = string.Format ("{0}{1}", JsonFilePath, packId.ToString ());
			StartCoroutine (JsonRequest (url));
			packId++;
	}

	void NextData()
	{
//		Debug.LogWarning ("packId:"+packId+",PackNum:"+packNum);
		if (packId < packNum)
		{
			var url = string.Format ("{0}{1}", JsonFilePath, packId.ToString ());
			StartCoroutine (JsonRequest (url));
			packId++;
		}
	}

	private IEnumerator JsonRequest(string url)
	{
//		packId = num.ToString ();
//		Debug.Log (num);
		var request = new WWW (url);
		Debug.Log ("<color=blue>"+request.url+"</color>");
		yield return request;

		if (!string.IsNullOrEmpty (request.error))
		{
			Debug.Log ("エラー発生");
			Application.ExternalCall("xmlLoadFlag",-1);
			SceneManager.LoadScene("LoadScene");
		}
		string json = request.text;
		Debug.Log (request.text.Length+","+request.url);
//		var jsonData = JsonUtility.FromJson<data> (json);
		StartCoroutine (JsonParse(json));
	}

	private IEnumerator JsonParse(string jsonTxt)
	{
		var jsonData = JsonUtility.FromJson<data> (jsonTxt);

//		if (dic.ContainsKey ("Items"))
//		{
//			Debug.Log ("contain key");
//			jsonData = dic["Items"] as List<table>;
//		}

//		var cnt = 0;

//		while (jsonData.Items.Count > 0)
//		{
//			Debug.Log (cnt);
//			cnt++;
//			if (cnt > 100)
//				break;
			yield return null;
//		}
//		if (cnt > 100)
//		{	
			transformData (jsonData.Items);

			NextData ();
//		}
	}

	void transformData(List<table> list)
	{
//		Debug.LogWarning (list.Count);
		foreach (var person in list)
		{
			int   mid  = person.mid;
			float ts   = person.TS;
			float posx = person.x*(float)0.10;
			float posy = person.y*(float)0.10;
			float roty = person.d1;
			int   col  = person.color;
//			Debug.LogError (col);

			if (!actorDict.ContainsKey ("mid" + mid))
			{
				var actorData = new ActorData(1,null);
				actorDict.Add ("mid" + mid,actorData);
			}
			else
			{
				actorDict ["mid" + mid].counter += 1;
			}

			if (mid >= 1000) 
			{
				if (mid - 1000 >= DataList.Count)
				{
					DataList.Add (new List<LocationData> ());
				}
				else
				{
					//					Debug.Log ("datalistの長さ" + DataList.Count);
					DataList [mid - 1001].Add (new LocationData (mid, ts, posx, posy, col, roty));
				}
			}
			else
			{
				if (mid >= DataList.Count)
				{
					DataList.Add (new List<LocationData> ());
				}
				else
				{
					DataList [mid].Add (new LocationData (mid, ts, posx, posy, col, roty));
				}
			}
//			Debug.Log ("<color=red>mid:"+person.mid+",ts:"+person.TS+",PosX:"+person.x+"</color>");
		}
//		Debug.LogError (DataList.Count);
		if (!XMLHasLoad && packId == packNum)
		{
//			foreach (var data in DataList[0])
//			{
//				Debug.Log (data.ts);
//			}
			InitModel ();
		}
	}

	void InitModel()
	{
		Debug.Log("<color=blue>Copmlete Create 3d Objects</color>");
		foreach (List<LocationData> List in DataList)
		{
			
			if (1 <= List.Count)
			{
				
				Vector3 pos = new Vector3 (List [0].posX, (float)0.3, List [0].posZ);
				Quaternion dir = Quaternion.Euler (0, List [0].angleY, 0);
				Vector3 tpos = new Vector3 (List [0].posX, (float)1, List [0].posZ);
//				Debug.Log ("mid" + List [0].mid.ToString ());
				if (actorDict ["mid" + List [0].mid].counter > MID_CHECK_SKIP_COUNT)
				{
					GameObject actor = null;
					switch (Who (List [0].mid))
					{
					case "male":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_MALE), pos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						break;
					case "female":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_FEMALE), pos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						break;
					case "teacher":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_MALE_TEACHER), tpos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						break;
					case "teacher_woman":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_FEMALE_TEACHER), tpos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						break;
					case "desk":
						actor = Instantiate (Resources.Load (PREFAB_MODEL_PATH + MODEL_DESK), pos, dir) as GameObject;
						actor.name = "mid" + List [0].mid.ToString ();
						break;
					default:
						break;
					}

					if (actorDict ["mid" + List [0].mid].objActor == null) {
						actorDict ["mid" + List [0].mid].objActor = actor;
						actorDict ["mid" + List [0].mid].controller = actor.GetComponent<ModelController> ();
					}
				}
			}
		}

		//初回処理
//		if (!XMLHasLoad)
		if (!XMLHasLoad && packId == packNum)
		{
			Debug.LogError ("JsonLoaded");
			StartCoroutine("Relocate",0.1f);
			XMLHasLoad = true;
			time = 0;
		}
	}

	void Update () {
		// Application.ExternalCall はJS側の関数を呼んでいます。

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
//				if (voiceLoader.www.progress == 1f) {
//					StartCoroutine(voiceLoader.CallSoundLength());
//				}
			}
			
			if (voiceLoader.hasLoad && XMLHasLoad) {
				step = STEP.READY;
				Application.ExternalCall("xmlLoadFlag",1);
				Debug.Log ("Ready");
				EBManager.instance.SetInitialCameta ();
				ChangeKeyBoardInput (true);

			}
			break;
		case STEP.READY:
			#if UNITY_EDITOR
				if (Input.GetKeyDown (KeyCode.Space)) {
				playFlag (1);
				}
			#endif
			break;
		case STEP.PLAY:
			time -= Time.deltaTime;
//			Debug.LogWarning (voiceLoader.source.time);
			audioTime = voiceLoader.source.time;

//			Debug.Log ("SourceTime:"+voiceLoader.source.time +",ClipLength:"+voiceLoader.source.clip.length +",AudioTime:"+audioTime);
			Application.ExternalCall ("soundPosition", voiceLoader.source.time);
			if ((int)this.voiceLoader.source.time >= (int)voiceLoader.source.clip.length)
			{
				Debug.LogError ("再生終了");
				foreach(var data in actorDict.Values)
				{
					if(data.controller != null)
						data.controller.ResetStatus ();
				}
				endFlag ();
//				Application.ExternalCall ("soundPosition", voiceLoader.source.clip.length);
			}
			else if(time<=0)
			{
				Debug.Log ("timeFix");
				//time = 0.5f;
				time = 0.1f;
			}

			#if UNITY_EDITOR
			if (Input.GetKeyDown (KeyCode.Space)) {
				playFlag (0);
			}
			if (Input.GetKeyDown (KeyCode.S)) {
				isSeekMove = true;
				DebugReset();
			}
			#endif
			break;
		case STEP.PAUSE:
			#if UNITY_EDITOR
			if (Input.GetKeyDown (KeyCode.Space)) {
				playFlag (1);
			}
			#endif
			break;

//			if(isStart==0 && isPlay==1){
//				isPlay = 0;
//				voiceLoader.GetComponent<AudioSource>().Pause();
//				audioTime =voiceLoader.GetComponent<AudioSource>().time;
//				//Debug.Log(voiceLoader.GetComponent<AudioSource>().time);
//			}
		}
//		Debug.Log (step);
		//Debug.Log("音声ロード進捗 = "+voiceLoader.www.progress+", XMLロード進捗 = "+getXML.progress+", 再生中か = "+(isStart==0)+"データリストの長さ："+DataList.Count);
	}

	void playFlag(int flag){
//		Debug.Log ("IN:playFlag:"+flag+",step:"+step);
		if (flag == 1 && step == STEP.READY ||flag == 1 && step == STEP.PAUSE) {
			ChangeKeyBoardInput (true);
			Debug.LogError("start1:" +voiceLoader.source.time );
			Debug.LogError("audioTime:"+audioTime);
			voiceLoader.source.time = audioTime;
			Debug.LogError("start2:" +voiceLoader.source.time );
			voiceLoader.source.Play ();
//			voiceLoader.source.UnPause ();
			step = STEP.PLAY;
			isSeekMove = false;
		}else if(flag ==0){
			ChangeKeyBoardInput (false);
//			isPlay = 0;
			step = STEP.PAUSE;
			isSeekMove = true;
//			
			//! パーティクルの停止
			foreach(var data in actorDict.Values)
			{
				if(data.controller != null)
					data.controller.ResetStatus ();
			}

//			voiceLoader.source.Pause ();
			voiceLoader.source.Stop ();

			Debug.LogError("stop:" +voiceLoader.source.time );
			audioTime =voiceLoader.source.time;
            Debug.LogError("stopAudioTime:" + audioTime);
			//Debug.Log(voiceLoader.GetComponent<AudioSource>().time);
			
		}
//		Debug.Log ("Out:playFlag:"+flag+",step:"+step);
	}

	void endFlag()
	{
		Application.ExternalCall ("endFlag", 0);
		Debug.Log ("called endFlag!");
		playFlag (0);
	}
	
	void soundPosition(float pos){
		audioTime = pos;
		voiceLoader.source.time = pos;
		Debug.LogError ("setsoudpos:"+voiceLoader.source.time);
		isSeekMove = true;
		StartCoroutine ("waitSeek");
		Debug.Log ("<JS側からの制御> soundPosition:"+pos);
	}

	void SetSoundValue(float val){
		voiceLoader.source.volume = val;
		Debug.Log ("<JS側からの制御> SetSoundValue:"+val);
	}

	void ChangeKeyBoardInput(bool isUse)
	{
		#if !UNITY_EDITOR && UNITY_WEBGL
		WebGLInput.captureAllKeyboardInput = isUse;
		Debug.Log ("KeyBoardInput:"+WebGLInput.captureAllKeyboardInput);
		#endif
	}

	void DebugReset()
	{
		soundPosition (0.0f);
		playFlag (1);
	}

	IEnumerator waitSeek()
	{
		yield return new WaitForSeconds (0.5f);
		isSeekMove = false;
		Debug.Log ("seek move completed");
	}

	#region reloacate
	//audiotimeと0.03秒以下の誤差のLocationDataがあればそれを、なければnullを返します。
	private LocationData getActorLocation(List<LocationData> tempList){
		foreach (LocationData Data in tempList){
//			return Data;
			if(Mathf.Abs(audioTime - Data.ts)<= 0.03){
				
				return Data;
			}
		}

		return null;
	}

	//time秒ごとに位置情報を更新する関数
	private IEnumerator Relocate(float time)
	{
		Debug.Log ("<color=red>Relocate</color>");
		while(true)
		{
			foreach (List<LocationData> list in DataList)
			{
			LocationData Data = getActorLocation(list);

				//二重foreachの見た目を避け可読性をあげると以下の条件分岐が入ってしまうので、関数を使わないほうがいいかも
				if(1 <= list.Count&&Data!=null)
				{
//					GameObject actor = GameObject.Find("mid"+Data.mid.ToString());
					GameObject actor = actorDict["mid"+Data.mid].objActor;
					Vector3 tmpPos = actor.transform.position;
					Vector3 tmpAngle = new Vector3 (actor.transform.eulerAngles.x, actor.transform.eulerAngles.y, actor.transform.eulerAngles.z);

					Vector3 dataPos = new Vector3 (Data.posX, actor.transform.position.y, Data.posZ);

//					bool isMoveX = Mathf.Abs(tmpPos.x - dataPos.x) > POS_CHECK_SKIP_VALUE;
//					bool isMoveZ = Mathf.Abs(tmpPos.z - dataPos.z) > POS_CHECK_SKIP_VALUE;
//					if(!isMoveX && !isMoveZ)
//						Debug.LogError ("POS_SKIP");
//					Vector3 updatePos = isMoveX || isMoveZ ? dataPos : tmpPos;
					Vector3 updatePos = dataPos;



					//マイナスをプラスにする (ex. -80 => 280
					Data.angleY =  Data.angleY < 0 ? 360f +Data.angleY : Data.angleY;
					bool isChangeRotion = Mathf.Abs(tmpAngle.y - Data.angleY) > ROT_CHECK_SKIP_VALUE;
					if (isChangeRotion)
					{
						tmpAngle.y = Data.angleY;
					}

					//positionとrotation更新
					if (isSeekMove)
					{
						actor.transform.localPosition = updatePos;
						actor.transform.localRotation = Quaternion.Euler(tmpAngle);
					}
					else
					{
						iTween.MoveTo (actor, updatePos, MOVE_SPEED);
						iTween.RotateTo (actor, tmpAngle, ROTATION_SPEED);
					}
						
					if (actorDict ["mid" + Data.mid].controller != null)
					{
						actorDict ["mid" + Data.mid].controller.ChangeStatus (Data.col);
					} 

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
public class LocationData
{
	public int mid;		//	オブジェクトの識別id
	public float ts;	//タイムスタンプ
	public float posX;	//x座標
	public float posZ;	//z座標
	public float angleY;	//yのRotation
	public int col;

	public LocationData(int getMid,float getTs,float getposX,float getposZ,int getColor,float getrotY)
	{
		mid = getMid;
		ts = getTs;
		posX = getposX;
		posZ = getposZ;
		col = getColor;
		angleY = getrotY;
	}
}

//Actorクラス
public class ActorData
{
	public int counter;
	public GameObject objActor;
	public ModelController controller;

	public ActorData(int cnt,GameObject obj)
	{
		counter = cnt;
		objActor = obj;
	}
}