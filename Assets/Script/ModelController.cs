using UnityEngine;
using System.Collections;



public class ModelController : MonoBehaviour {
	[SerializeField]
	Camera camera;
	int camId;

	[SerializeField]
	GameObject [] particles;

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

	// 0:通常,1:寝る,2:真面目
	public void ChangeStatus(int status)
	{
		if (status < 0) return;

		for (int i = 0; i < particles.Length; i++)
		{
			particles [i].SetActive (status-1 == i);
		}
	}
}