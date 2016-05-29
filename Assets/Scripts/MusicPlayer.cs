using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{

	private static MusicPlayer _instance;

	public void OnEnable()
	{
		_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void Awake()
	{
		if (_instance) DestroyImmediate(gameObject);
	}
}
