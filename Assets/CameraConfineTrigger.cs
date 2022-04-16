using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConfineTrigger : MonoBehaviour
{
    PolygonCollider2D polygonCollider2D;
    PolygonCollider2D globalPolygonCollider2D;
    CameraController cameraController;

    private void Start()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        globalPolygonCollider2D = FindObjectOfType<SceneController>().colCameraBounds;
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            StopAllCoroutines();
            cameraController.confiner.m_Damping = 4;
            cameraController.confiner.m_BoundingShape2D = polygonCollider2D;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            StartCoroutine(RemoveDamping());
            cameraController.confiner.m_BoundingShape2D = globalPolygonCollider2D;
        }
    }
    float dampingRef;
    IEnumerator RemoveDamping()
    {
        float damping = cameraController.confiner.m_Damping;
        while (damping > 0)
        {
            damping = Mathf.SmoothDamp(damping, -1, ref damping, 0.1f);
            cameraController.confiner.m_Damping = damping;
            yield return null;
        }

        cameraController.confiner.m_Damping = 0;
    }
}
