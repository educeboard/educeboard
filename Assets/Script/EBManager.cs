using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EBManager : MonoBehaviour {
	public static EBManager instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject go = new GameObject ("EBManager");
				_instance = go.AddComponent<EBManager> ();
			}
			return _instance;
		}
	}

	private static EBManager _instance;


	private int activeCameraId = 0;

	private Camera publicCamera;

	private Dictionary<int,Camera> privateCamDic;

	/// <summary>個別用カメラ管理用のフラグ</summary>
	public  bool isPrivateMode = false;
	public  bool isReSet = false;

	private Button BtnCameraChange;


	private 

	// Use this for initialization
	void Start()
	{
		privateCamDic = new Dictionary<int, Camera> ();
		var btn = GameObject.Find ("BtnChangeCamera");
		BtnCameraChange = btn.GetComponent<Button> (); 
		BtnCameraChange.gameObject.SetActive (false);
		BtnCameraChange.onClick.AddListener (ChangePublicMode);

	}
		
//	public  void FuncChangeCameraMode()
//	{
//		if (isPrivateMode)
//		{
//			isReSet = true;
//			BtnCameraChange.gameObject.SetActive (false);
//			isPrivateMode = false;
//		}
//		else
//		{
//			isPrivateMode = true;
//			BtnCameraChange.gameObject.SetActive (true);
//		}
//	}



	public void AddPublicCamera(Camera cam)
	{
		publicCamera = cam;
	}

	public int AddCamera(Camera cam)
	{
		int cnt = privateCamDic.Count+1;
		privateCamDic.Add (cnt, cam);
		return cnt;
	}

	public void ChangePrivateMode(int camId)
	{
		privateCamDic [camId].enabled = true;
		if(activeCameraId != 0 && activeCameraId != camId )
			privateCamDic [activeCameraId].enabled = false;
		activeCameraId = camId;
		if(!BtnCameraChange.gameObject.activeSelf)
			BtnCameraChange.gameObject.SetActive (true);
	}

	public void ChangePublicMode()
	{
		// 全体カメラのON
		publicCamera.enabled = true;
		activeCameraId = 0;
		// 個別カメラのOFF
		foreach(Camera cam in privateCamDic.Values)
		{
			cam.enabled = false;
		}
		BtnCameraChange.gameObject.SetActive (false);
	}
}
