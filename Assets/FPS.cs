using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
	public TMPro.TMP_Text FPSText;
	[SerializeField] private float _hudRefreshRate = 1f;

	private float _timer;

	private void Update()
	{
		if (Time.unscaledTime > _timer)
		{
			int fps = (int)(1f / Time.unscaledDeltaTime);
			FPSText.text = fps.ToString();
			_timer = Time.unscaledTime + _hudRefreshRate;
		}
	}
}

