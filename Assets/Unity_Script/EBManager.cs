using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	


	/// <summary>個別用カメラ管理用のフラグ</summary>
	public  bool isPrivateMode = false;
	public  bool isReSet = false;

	private Button BtnCameraChange;

	// Use this for initialization
	void Start()
	{
		var btn = GameObject.Find ("BtnChangeCamera");
		BtnCameraChange = btn.GetComponent<Button> (); 
		BtnCameraChange.gameObject.SetActive (false);
		BtnCameraChange.onClick.AddListener (FuncChangeCameraMode);

	}
		
	public  void FuncChangeCameraMode()
	{
		if (isPrivateMode)
		{
			isReSet = true;
			BtnCameraChange.gameObject.SetActive (false);
			isPrivateMode = false;
		}
		else
		{
			isPrivateMode = true;
			BtnCameraChange.gameObject.SetActive (true);
		}
	}
}
