using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
	public TMPro.TMP_Text FPSText;

	[SerializeField, Range(0.1f, 2f)]
	float sampleDuration = 1f;

	int frames;
	float duration, bestDuration = float.MaxValue, worstDuration;

	void Update()
	{
		float frameDuration = Time.unscaledDeltaTime;
		frames += 1;
		duration += frameDuration;

		if (frameDuration < bestDuration)
		{
			bestDuration = frameDuration;
		}
		if (frameDuration > worstDuration)
		{
			worstDuration = frameDuration;
		}

		if (duration >= sampleDuration)
		{
			FPSText.SetText( "FPS\n{0:0}\n{1:0}\n{2:0}", (int)(1f / bestDuration), (int)(frames / duration), (int)(1f / worstDuration));
			frames = 0;
			duration = 0f;
			bestDuration = float.MaxValue;
			worstDuration = 0f;
		}
	}
}
