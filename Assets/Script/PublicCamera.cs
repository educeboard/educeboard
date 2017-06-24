using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicCamera : MonoBehaviour {
	[SerializeField]
	private Camera publicCamera;

	void Start ()
	{
		Initialize ();
	}

	void Initialize()
	{
		EBManager.instance.AddPublicCamera (publicCamera);
	}
}
