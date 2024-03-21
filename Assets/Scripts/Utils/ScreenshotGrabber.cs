#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class ScreenshotGrabber
{
    [MenuItem("Screenshot/Grab")]
    public static void Grab()
    {
        string filePath = "C:/Users/Fouad/Desktop/WRoI Media/Screenshots/";
        string fileName = "Screenshot_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Second + DateTime.Now.Millisecond+ ".png";
        ScreenCapture.CaptureScreenshot(filePath + fileName, 1);
    }
}
#endif