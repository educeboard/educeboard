using UnityEngine;
using System.Collections;



public class CameraChanger : MonoBehaviour {
	[SerializeField]
	Camera camera;
	int camId;

	void Start()
	{
		this.Initialize ();
	}

	// 初期化処理
	void Initialize()
	{
		this.camId = EBManager.instance.AddCamera (this.camera);
	}

	void OnMouseDown()
	{
		EBManager.instance.ChangePrivateMode (this.camId);
	}
}