using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawProjector : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer lineRenderer;
    private KuszaController kuszaController;

    public int numPoint = 5;
    public float timebetween = 0.1f;

    public LayerMask CollidableLayers;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        kuszaController = GetComponent<KuszaController>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = (int)numPoint;
        List<Vector3> points = new List<Vector3>();
        Vector3 startPosition = kuszaController.Point.position;
        Vector3 startVelocity = kuszaController.Point.forward * (float)kuszaController.Power;
        for (float i = 0; i < numPoint; i += timebetween)
        {
            Vector3 newpoint = startPosition + i * startVelocity;
            newpoint.y = startPosition.y + startVelocity.y * i + Physics.gravity.y / 2f * i * i;
            points.Add(newpoint);

            if (Physics.OverlapSphere(newpoint, 2, CollidableLayers).Length > 0)
            {
                lineRenderer.positionCount = points.Count;
                break;
            }


        }
        lineRenderer.SetPositions(points.ToArray());

    }
}
