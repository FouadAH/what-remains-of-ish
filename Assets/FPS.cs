using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
	public TMPro.TMP_Text FPSText;
	[SerializeField] private float _hudRefreshRate = 1f;

	private float _timer;

	//private void Update()
	//{
	//	if (Time.unscaledTime > _timer)
	//	{
	//		int fps = (int)(1f / Time.unscaledDeltaTime);
	//		FPSText.text = fps.ToString();
	//		_timer = Time.unscaledTime + _hudRefreshRate;
	//	}
	//}

	IEnumerator Start()
	{
		GUI.depth = 2;
		while (true)
		{
			if (Time.timeScale == 1)
			{
				yield return new WaitForSeconds(0.1f);
				_timer = (1 / Time.deltaTime);
				FPSText.text = "FPS :" + (Mathf.Round(_timer));
			}
			else
			{
				FPSText.text = "Pause";
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
}

