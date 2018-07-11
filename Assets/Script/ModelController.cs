using UnityEngine;
using System.Collections;



public class ModelController : MonoBehaviour {
	[SerializeField]
	Camera camera;
	[SerializeField]
	Transform cameraRoot;
	int camId;

	private float angleSpeed = 0.5f;
	private float heightSpeed = 0.1f;

	//! キャラクター初期化用Position
	private Vector3 StartPos = new Vector3 ();
	//! キャラクター初期化用Angle
	private Vector3 StartAngle = new Vector3 ();

	//! カメラの初期化用Angle
	private Vector3 initialCameraAngle = new Vector3 ();
	//! カメラの調整用Angle
	private Vector3 newAngle = new Vector3 ();

	//! カメラの初期化用Position
	private Vector3 initialCameraPos = new Vector3 ();
	//! カメラの調整用Position
	private Vector3 newCameraPos = new Vector3 ();

	[SerializeField]
	GameObject [] particles;

	[SerializeField]
	GameObject [] faces;

	void Start()
	{
		this.Initialize ();
	}

	void Update()
	{
		if (!this.camera.enabled)
			return;
		
		if (Input.GetKey (KeyCode.LeftArrow))
		{
//			Debug.LogError ("left");
			this.newAngle.y -= angleSpeed;
//			this.camera.gameObject.transform.localEulerAngles = newAngle;
			this.cameraRoot.localEulerAngles = newAngle;
		}
		if (Input.GetKey (KeyCode.RightArrow))
		{
//			Debug.LogError ("right");
			this.newAngle.y += angleSpeed;
//			this.camera.gameObject.transform.localEulerAngles = newAngle;
			this.cameraRoot.localEulerAngles = newAngle;
		}
		if (Input.GetKey (KeyCode.UpArrow))
		{
			if (this.newCameraPos.y > 3.5f)
				return;
			this.newCameraPos.y += heightSpeed;
//			this.camera.gameObject.transform.localPosition = newCameraPos;
			this.cameraRoot.localPosition = newCameraPos;
		}
		if (Input.GetKey (KeyCode.DownArrow))
		{
			if (this.newCameraPos.y < 2f)
				return;
			this.newCameraPos.y -= heightSpeed;
//			this.camera.gameObject.transform.localPosition = newCameraPos;
			this.cameraRoot.localPosition = newCameraPos;
		}
		if (Input.GetKeyDown (KeyCode.R))
		{
			ResetCameraPos ();
		}
	}

	// 初期化処理
	void Initialize()
	{
		//! カメラをマネージャーに登録
		this.camId = EBManager.instance.AddCamera (this.camera);
		//! カメラアングル初期化用
		this.initialCameraAngle = this.cameraRoot.localEulerAngles;
		this.newAngle = this.initialCameraAngle;
		//! カメラポジション初期化用
		this.initialCameraPos = this.cameraRoot.localPosition;
		this.newCameraPos = this.initialCameraPos;
		//! キャラクター初期化用
		this.StartPos = this.gameObject.transform.localPosition;
		this.StartAngle = this.gameObject.transform.localEulerAngles;
	}

	//! クリックされたら視点切り替え
	void OnMouseDown()
	{
		EBManager.instance.ChangePrivateMode (this.camId);
	}

	// 0:通常,1:寝る,2:真面目
	public void ChangeStatus(int status)
	{
		if (status < 0) return;

		if (particles.Length == 0 || faces.Length == 0) return;

		for (int i = 0; i < particles.Length; i++)
		{
			particles [i].SetActive (status-1 == i);
		}

		for (int i = 0; i < faces.Length; i++)
		{
			faces [i].SetActive (status == i);
		}
	}

	//! ステータスを初期化
	public void ResetStatus()
	{
		if (particles.Length == 0) return;

		for (int i = 0; i < particles.Length; i++)
		{
			particles [i].SetActive (false);
		}

		if (faces.Length == 0) return;

		for (int i = 0; i < faces.Length; i++)
		{
			faces [i].SetActive (i == 0);
		}
	}

	public void ResetStartPos()
	{
		this.gameObject.transform.localPosition = this.StartPos;
		this.gameObject.transform.localEulerAngles = this.StartAngle;
		ResetStatus ();
	}

	//! カメラのPositionとAngleを初期化
	private void ResetCameraPos()
	{
		this.cameraRoot.localEulerAngles = initialCameraAngle;
		newAngle = initialCameraAngle;

		this.cameraRoot.localPosition = initialCameraPos;
		newCameraPos = initialCameraPos;
	}
}