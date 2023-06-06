using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawStretch : MonoBehaviour
{
    public RectTransform _pawPosition;
    public RectTransform _startPosition;
    public bool mirrorZ = true;
    public Material _armMat;
    public float width;
    GameObject Line;

    private void Update()
    {
        Destroy(Line);
        Line = new GameObject("Line");
        Line.transform.parent = gameObject.transform.parent;
        LineRenderer lineRenderer = Line.AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = _armMat;
        lineRenderer.sortingOrder = 1;

        lineRenderer.SetPosition(0, _startPosition.position);
        lineRenderer.SetPosition(1, _pawPosition.position);
    }


}
