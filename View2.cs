using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View2 : MonoBehaviour
{
    public float speed = 5;
    public float sensitiviyY = 75;
    public float sensitiviyX = 125;
    public Transform cam;
    private Vector3 tmp;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitiviyX * Time.deltaTime);
            cam.Rotate(Vector3.right * -Input.GetAxis("Mouse Y") * sensitiviyY * Time.deltaTime);
        }
        if (Input.GetKey("w")) transform.position += transform.forward * speed * Time.deltaTime;
        if (Input.GetKey("s")) transform.position -= transform.forward * speed * Time.deltaTime;
        if (Input.GetKey("d")) transform.position += transform.right * speed * Time.deltaTime;
        if (Input.GetKey("a")) transform.position -= transform.right * speed * Time.deltaTime;
        if (Input.GetKey("e")) transform.position += transform.up * speed * Time.deltaTime;
        if (Input.GetKey("q")) transform.position -= transform.up * speed * Time.deltaTime;
    }

    public void OnActive(Transform tr)
    {
        transform.position = tr.position;
        transform.rotation = new Quaternion();
        tmp = new Vector3(tr.eulerAngles.x, 0, 0);
        cam.eulerAngles = tmp;
        tmp = new Vector3(0, tr.eulerAngles.y, 0);
        transform.eulerAngles = tmp;
    }
}
