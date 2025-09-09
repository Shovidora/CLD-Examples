using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [Header("Main:")]
    //
    //public Transform center;
    public Transform[] centers;
    public Transform target;
    public float minCircleTime = 20;
    public float minCircleHight = -5;
    public float minCircleRadius = 0;

    [Header("Varibals:")]
    //
    public float circleRadius = 30;
    public float circleHight = 20;
    public float circleTime = 60;
    public float manual = 10000;

    [Header("Sensitivitys:")]
    //
    public float regularSensitivity = 0.15f; // Regular Sensitivity
    public float manualSensitivity = 1200; // Manual Sensitivity

    [Header("Editor")]
    //
    private float po_x = 0;
    private float po_z = 0;
    private Vector3 p; // position, just to not make a new Vector3 every frame... , don't know why I can set directly the position...

    // Start is called before the first frame update
    private void Awake()
    {
        if (target.Equals(null)) Debug.LogError("there isn't a target for the camera '" + gameObject.name + "'");
        transform.SetPositionAndRotation(new Vector3(0, -100, 0), new Quaternion(0.7f, 0, 0, 0.7f));
        if (centers.Length == 0) centers = new Transform[] { target };
    }
    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<Manger>().play)
        {
            if (Input.GetKeyDown("x"))
            {
                //target = center;
                int j = 0;
                for (int i = 0; i < centers.Length; i++)
                    if (target == centers[i])
                        j = i + 1;
                if (j == centers.Length)
                    j = 0;
                target = centers[j];
                //
                circleRadius = 90;
                circleHight = 40;
                circleTime = 60;
                minCircleHight = -5;
            }
            switch (FindObjectOfType<Manger>().viewSet)
            {
                case (0): CircleLook(circleHight, manualSensitivity, circleRadius, manual, target); break;
                case (1): CircleLook(circleHight, circleTime, circleRadius, Time.time, target); break;   
            }
            Controller();
        }
    }
    private void Controller()
    {
        if (Input.GetKey("w")) circleHight += regularSensitivity;
        if (Input.GetKey("s")) circleHight -= regularSensitivity;
        if (Input.GetKey("z")) circleRadius += regularSensitivity;
        if (Input.GetKey("c")) circleRadius -= regularSensitivity;
        if (Input.GetKey("e")) circleTime += regularSensitivity;
        if (Input.GetKey("q")) circleTime -= regularSensitivity;
        if (Input.GetKey("d")) manual += regularSensitivity;
        if (Input.GetKey("a")) manual -= regularSensitivity;
        if (circleTime < minCircleTime) circleTime = minCircleTime;
        if (circleRadius < target.transform.localScale.magnitude && circleHight < minCircleHight) circleHight = minCircleHight;
        if (circleRadius < minCircleRadius) circleRadius = minCircleRadius;
    }
    private void CircleLook(float cH, float T, float R, float t, Transform target)
    {
        t %= T;
        float Quarter = Mathf.Sqrt(R * R / 2);
        if (t >= 0 && t < T / 4)
        {
            po_x = t * Quarter * 8 / T - Quarter;
            po_z = Mathf.Sqrt(R * R - po_x * po_x); // po_x^2<=R^2
        }
        if (t >= T / 4 && t < T / 2)
        {
            po_z = t * Quarter * -8 / T + 3 * Quarter;
            po_x = Mathf.Sqrt(R * R - po_z * po_z); // po_z^2<=R^2
        }
        if (t >= T / 2 && t < T * 3 / 4)
        {
            po_x = t * Quarter * -8 / T + 5 * Quarter;
            po_z = -Mathf.Sqrt(R * R - po_x * po_x); // po_x^2<=R^2
        }
        if (t >= T * 3 / 4 && t < T)
        {
            po_z = t * Quarter * 8 / T - 7 * Quarter;
            po_x = -Mathf.Sqrt(R * R - po_z * po_z); // po_z^2<=R^2
        }
        po_x += target.position.x; po_z += target.position.z;
        p.Set(po_x, target.position.y + cH, po_z);
        transform.position = p;
        transform.LookAt(target);
    }
}