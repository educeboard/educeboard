using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EBManager : MonoBehaviour {

	/// <summary>個別用カメラ管理用のフラグ</summary>
	public static bool isPrivateMode = false;
	public static bool isReSet = false;
	[SerializeField]
	public Button BtnCameraChange;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isPrivateMode){
			//カメラが個別モードの時に表示
			if (!BtnCameraChange.IsActive()) {
				BtnCameraChange.gameObject.SetActive (true);
			}
		}
	
	}

	public void FuncChangeCameraMode()
	{
		if(isPrivateMode){
			isReSet = true;
			BtnCameraChange.gameObject.SetActive (false);
		}
	}
}
