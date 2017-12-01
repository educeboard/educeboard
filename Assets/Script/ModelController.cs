using UnityEngine;
using System.Collections;



public class ModelController : MonoBehaviour {
	[SerializeField]
	Camera camera;
	int camId;

	[SerializeField]
	GameObject [] particles;

	[SerializeField]
	GameObject [] faces;

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
}