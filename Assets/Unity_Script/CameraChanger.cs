using UnityEngine;
using System.Collections;



public class CameraChanger : MonoBehaviour {
	[SerializeField]
	GameObject MyCamera;

	void Start(){
//		MyCamera = transform.FindChild("Camera").gameObject;
	}

	void Update(){
		if(Input.GetKey(KeyCode.Escape)){
			MyCamera.SetActive(false);
		}
	}

	void OnMouseDown(){
		MyCamera.SetActive (true);
	}
}