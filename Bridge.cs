using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : SimulationObject
{
    [Header("Bridge:")]
    //
    public GameObject target;
    public GameObject arrow;
    public GameObject[] hitArrows;

    [Header("Opsions:")]
    //
    public Vector3[] startPositions;
    public bool startPositionRelativeToBodySize = false;
    public bool ifTargetIsNotActiveSendArrow = false;
    public bool ifInStageAlwaysSendArrow = false; 
    [HideInInspector] public bool onlyManually = false; // start step only

    [Header("Conditions:")]
    //
    public Vector2 limitsBodySizeInPercentage = new Vector2(0, 1);

    //
    private readonly List<Vector3> sP = new List<Vector3>();
    private Vector3 DefualtSP = new Vector3(0, 0, 0); // Defualt Start Position
    private bool NewPosition;
    //
    private GameObject Arrow;
    //
    private Vector3 x;
    private Vector3 y;
    private Vector3 z;

    void Start()
    {
        if (FindObjectOfType<Manger>() == null) Debug.LogError("there is no Object of type Manager in the scene!");
        if (GetComponent<Step>() == null) Debug.LogError("GameObject ('" + gameObject.name + "') have a bridge but missing a Step ccomponent!");
        //
        if (onlyManually && GetComponent<StartStep>() == null)
        {
            onlyManually = false;
            Debug.LogWarning("Step ('" + gameObject.name + "') have a bridge with onlyManually enabled but missing script StartStep \n onlyManually sets back to false");
        }
        //
        if (startPositions.Length == 0) startPositions = new Vector3[] { DefualtSP };
        else
        {
            for (int i = 0; i < startPositions.Length; i++)
            {
                NewPosition = true;
                for (int j = 0; j < sP.Count; j++)
                {
                    if (sP[j].Equals(startPositions[i]))
                    {
                        NewPosition = false;
                        Debug.LogWarning(gameObject.name + " have a bridge with 2 or more equals start positions: sP=" + startPositions[i]);
                    }
                }
                if (NewPosition)
                    sP.Add(startPositions[i]);
            }
            startPositions = sP.ToArray();
        }
        //
        if (limitsBodySizeInPercentage.x < 0) { Debug.LogWarning("Step ('" + gameObject.name + "') have a bridge with limitsBodySizeInPercentage.x(=" + limitsBodySizeInPercentage.x + ") < 0 , this value should be between 0 and 1"); limitsBodySizeInPercentage.x = 0; }
        if (limitsBodySizeInPercentage.x > 1) { Debug.LogWarning("Step ('" + gameObject.name + "') have a bridge with limitsBodySizeInPercentage.x(=" + limitsBodySizeInPercentage.x + ") > 1 , this value should be between 0 and 1"); limitsBodySizeInPercentage.x = 1; }
        if (limitsBodySizeInPercentage.y < 0) { Debug.LogWarning("Step ('" + gameObject.name + "') have a bridge with limitsBodySizeInPercentage.y(=" + limitsBodySizeInPercentage.y + ") < 0 , this value should be between 0 and 1"); limitsBodySizeInPercentage.y = 0; }
        if (limitsBodySizeInPercentage.y > 1) { Debug.LogWarning("Step ('" + gameObject.name + "') have a bridge with limitsBodySizeInPercentage.y(=" + limitsBodySizeInPercentage.y + ") > 1 , this value should be between 0 and 1"); limitsBodySizeInPercentage.y = 1; }
        //
        if (ifInStageAlwaysSendArrow) ifTargetIsNotActiveSendArrow = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ifInStageAlwaysSendArrow)
            if (collision.gameObject.GetComponent<Arrow>() != null)
                if (!collision.gameObject.GetComponent<Arrow>().Sender.Equals(gameObject))
                    InstantiateArrow(collision.gameObject); 
    }


    public void InstantiateArrow(GameObject hitArrow)
    {
        if (GetComponent<Step>().maxSize * limitsBodySizeInPercentage.x <= gameObject.transform.localScale.z && GetComponent<Step>().maxSize * limitsBodySizeInPercentage.y >= gameObject.transform.localScale.z)
        {
            if (hitArrows.Length != 0)
            {
                for (int i = 0; i < hitArrows.Length; i++)
                    if ((hitArrows[i].name + "(Clone)").Equals(hitArrow.name))
                        InstantiateArrow();
            }
            else InstantiateArrow();
        }
    }
    public void InstantiateArrow()
    {
        if (target.activeInHierarchy || (!target.activeInHierarchy && ifTargetIsNotActiveSendArrow))
            for (int i = 0; i < startPositions.Length; i++)
            {
                if (startPositionRelativeToBodySize) Arrow = Instantiate(arrow, InsPositionRelativeToBodySize(transform, startPositions[i]), transform.rotation);
                else Arrow = Instantiate(arrow, InsPosition(transform, startPositions[i]), transform.rotation);
                Arrow.GetComponent<Arrow>().ArrowSetting(target, gameObject);
            }
    }
    public Vector3 InsPositionRelativeToBodySize(Transform tr, Vector3 v)
    {
        return tr.position + tr.forward * v.z * tr.localScale.z + tr.right * v.x * tr.localScale.x + tr.up * v.y * tr.localScale.y;
    }
    public Vector3 InsPosition(Transform tr, Vector3 v)
    {
        if (v.x == 0) x = Vector3.zero; else if (v.x > 0) x = tr.right * (tr.localScale.x / 2 + v.x); else x = tr.right * (-tr.localScale.x / 2 + v.x);
        if (v.y == 0) y = Vector3.zero; else if (v.y > 0) y = tr.up * (tr.localScale.y / 2 + v.y); else y = tr.up * (-tr.localScale.y / 2 + v.y);
        if (v.z == 0) z = Vector3.zero; else if (v.z > 0) x = tr.forward * (tr.localScale.z / 2 + v.z); else z = tr.forward * (-tr.localScale.z / 2 + v.z);
        return tr.position + x + y + z;
    }
}