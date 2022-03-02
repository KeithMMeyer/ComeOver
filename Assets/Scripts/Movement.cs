using UnityEngine;

public class Movement : MonoBehaviour
{

    public float UnitsPerSecond; // speed
    public float UnitsPerSecondPerSecond; // acceleration
    public bool XAxis;
    public bool YAxis;
    public bool ZAxis;

    private bool stopping;

    // Update is called on a regular basis
    void FixedUpdate()
    {
        if (stopping)
        {
            if (Mathf.Sign(UnitsPerSecond) != Mathf.Sign(UnitsPerSecond + (UnitsPerSecondPerSecond / 50f)))
            {
                UnitsPerSecond = 0;
                UnitsPerSecondPerSecond = 0;
                stopping = false;
            }
        }

        UnitsPerSecond += (UnitsPerSecondPerSecond / 50f); // updates speed with accleration
        float unitsPerFrame = UnitsPerSecond / 50.0f; // converts ups to upf
        Vector3 position = transform.position; // gets current position of object

        if (XAxis)
            position.x += unitsPerFrame;
        if (YAxis)
            position.y += unitsPerFrame;
        if (ZAxis)
            position.z += unitsPerFrame;

        transform.position = position; // sets the object to the new position
    }

    // Stops the object instantly
    public void Stop()
    {
        UnitsPerSecond = 0;
        UnitsPerSecondPerSecond = 0;
    }

    // Stops the object over n seconds with constant deceleration
    public void StopInSeconds(float seconds)
    {
        UnitsPerSecondPerSecond = -UnitsPerSecond / seconds;
        stopping = true;
    }

    // Stops the object over n units with constant deceleration
    public void StopInUnits(float units)
    {
        UnitsPerSecondPerSecond = -UnitsPerSecond / (2 * units / UnitsPerSecond);
        stopping = true;
    }

}
