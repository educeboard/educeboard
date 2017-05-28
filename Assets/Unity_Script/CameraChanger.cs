using UnityEngine;
using System.Collections;



public class CameraChanger : MonoBehaviour {
	[SerializeField]
	GameObject MyCamera;

	void Start(){
//		MyCamera = transform.FindChild("Camera").gameObject;
	}

	void Update(){
//		if(Input.GetKey(KeyCode.Escape)){
//			MyCamera.SetActive(false);
//		}
		if(EBManager.instance.isReSet){
			MyCamera.SetActive(false);
			Invoke ("ResetFlg",1.0f);
		}
	}

	void ResetFlg()
	{
		if(EBManager.instance.isReSet == true)
			EBManager.instance.isReSet = false;
	}

	void OnMouseDown()
	{
		EBManager.instance.FuncChangeCameraMode ();
		MyCamera.SetActive (true);
	}
}