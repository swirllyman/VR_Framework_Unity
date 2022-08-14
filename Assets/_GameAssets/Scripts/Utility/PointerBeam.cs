using UnityEngine;

public class PointerBeam : MonoBehaviour
{
    public Transform hitTransform;
    public Renderer hitMarkerRend;
    public float lineWidth = .0075f;
    [Range(0.0f, 1.0f)]
    public float hitMarkerScale = 1.0f;
    [SerializeField] Renderer myRend;
    Transform prevParent;

    public Vector3 StartPosition { get; set; }
    public Vector3 EndPosition { get; set; }

    private void Awake()
    {
        myRend.enabled = false;
        hitMarkerRend.enabled = false;
        hitMarkerRend.transform.localScale = Vector3.one * hitMarkerScale;
        prevParent = transform.parent;
    }

    public void Display(Vector3 start, Vector3 end, bool showMarker = true)
    {
        StartPosition = start;
        EndPosition = end;
        transform.parent = PlayerPlatform.singleton.transform;
        hitTransform.transform.parent = PlayerPlatform.singleton.transform;
        hitMarkerRend.transform.localScale = Vector3.one * hitMarkerScale;

        float distance = Vector3.Distance(start, end) / 10;
        myRend.transform.position = (start + end) / 2;
        myRend.transform.localScale = new Vector3(lineWidth, 1, distance);
        myRend.transform.rotation = Quaternion.LookRotation(end - start);
        hitTransform.transform.position = end;

        hitMarkerRend.enabled = showMarker;
        myRend.enabled = true;
    }

    public void Stop()
    {
        transform.parent = prevParent;
        myRend.enabled = false;
        hitMarkerRend.enabled = false;
        hitTransform.transform.parent = transform;
    }
}