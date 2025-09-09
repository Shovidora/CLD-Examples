using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Step : SimulationObject
{
    [Header("Step:")]
    //
    //
    public float maxSize = 15;
    public float minSize = 5;
    public Vector4[] positionsByStages;
    public float MoveSpeed = 1f;
    public GameObject Show; // should be a prefab...
    public GameObject DisableSimulasion; // prefab to instantiate
    public GameObject EnableSimulasion; // prefab to instantiate
    //

    [Header("The Great Editor")]
    private Vector3 startSize = Vector3.one * 10;
    private const float minSizeC = 3;
    private Bridge[] bridges;
    private readonly List<Bridge> bridgesOn = new List<Bridge>();
    private Arrow[] sendArrows;
    private readonly List<Vector4> positionsByStagesTMP = new List<Vector4>(); 
    private float positionByStageWTMP;
    public bool showB = false;
    private Vector3 targetPosition;
    //

    void Start()
    {
        if (FindObjectOfType<Manger>() == null) Debug.LogError("there is no Object of type Manager in the scene!");
        if (GetComponent<Bridge>() == null) Debug.LogError("Step ('" + gameObject.name + "') without a Bridge component!");
        if (minSize < minSizeC) { minSize = minSizeC; Debug.LogWarning("you set minSize too low for step: " + gameObject.name + "\n (minSize >= 3)"); }
        //
        targetPosition = transform.position;
        //
        if (positionsByStages.Length == 0) positionsByStages = new Vector4[] { transform.position };
        else
        {
            if (positionsByStages.Length == 1)
            {
                if (positionsByStages[0].w == 0) Debug.LogWarning("step ('" + gameObject.name + "') have one position by stage sets for all stages \n to use this parameter correctly: 1 <= positionsByStages[0].w <= max stage = " + (FindObjectOfType<Manger>().stages.Length - 1));
                else positionsByStages = new Vector4[] { new Vector4(transform.position.x, transform.position.y, transform.position.z, positionsByStages[0].w - 1), positionsByStages[0] };
            }
            else
            {
                bool alreadyExists;
                for (int i = 0; i < positionsByStages.Length; i++)
                {
                    positionByStageWTMP = positionsByStages[i].w;
                    if (positionByStageWTMP != (int)positionsByStages[i].w)
                    {
                        Debug.LogWarning("Step ('" + gameObject.name + "') positionsByStages[" + i + "].w isn't an integer \n positionsByStages[" + i + "].w sets to " + (int)positionsByStages[i].w + " \n make sure alwayes to sets this kind of values (stages) to an integers");
                        positionsByStages[i].w = (int)positionsByStages[i].w;
                    }
                    alreadyExists = false;
                    for (int j = 0; j < positionsByStagesTMP.Count; j++)
                        if (positionsByStages[i].w == positionsByStagesTMP[j].w)
                            alreadyExists = true;
                    if (!alreadyExists) positionsByStagesTMP.Add(positionsByStages[i]);
                    else Debug.LogWarning("Step ('" + gameObject.name + "') have 2 or more positions that sets to the same stage, make sure there is only one position for this step on stage " + positionsByStages[i].w);
                }
                positionsByStages = positionsByStagesTMP.ToArray();
            }
        }
        //
    }
    void Update()
    {
        if (transform.localScale.z > maxSize) transform.localScale = Vector3.one * maxSize;
        if (transform.localScale.z < minSize) transform.localScale = Vector3.one * minSize;
        //
        sendArrows = FindObjectsOfType<Arrow>();
        for (int i = 0; i < sendArrows.Length; i++)
            if (sendArrows[i].Sender.Equals(gameObject))
                Physics.IgnoreCollision(sendArrows[i].gameObject.GetComponent<SphereCollider>(), gameObject.GetComponent<Collider>());
        //
        if (transform.position != targetPosition) transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Arrow>() != null)
        {
            Arrow hitArrow = collision.gameObject.GetComponent<Arrow>();
            if (!hitArrow.Sender.Equals(gameObject))
            {
                if (transform.localScale.z + hitArrow.cS < minSize) transform.localScale = Vector3.one * minSize;
                else if (transform.localScale.z + hitArrow.cS > maxSize) transform.localScale = Vector3.one * maxSize;
                else transform.localScale += Vector3.one * hitArrow.cS;
                //if (FindObjectOfType<Manger>().CheckMaxArrows()) // To make the stepts send all their bridges even if the number of arrows will pass the maximum
                for (int i = 0; i < bridges.Length; i++)
                    if (FindObjectOfType<Manger>().maxArrows >= FindObjectsOfType<Arrow>().Length && !bridges[i].onlyManually)
                        bridges[i].InstantiateArrow(hitArrow.gameObject);
                if (bridges.Length == 0) Debug.Log("Step: '" + gameObject.name + "' gat an arrow but have 0 active bridges \n bridges.Length == 0");
                Destroy(collision.gameObject);
            }
        }
    }

    public void ResetSize()
    {
        transform.localScale = startSize;
    }
    public void SetStartSize()
    {
        startSize = transform.localScale;
    }
    public void OnChangeStage()
    {
        DefualtStages();
        //
        gameObject.SetActive(InStage());
        //
        bridges = GetComponents<Bridge>();
        for (int i = 0; i < bridges.Length; i++)
        {
            bridges[i].DefualtStages();
            if (!bridges[i].target.activeInHierarchy) bridges[i].enabled = false;
            else bridges[i].enabled = true;
            bridges[i].enabled = bridges[i].InStage(); 
        }
        bridgesOn.Clear();
        for (int i = 0; i < bridges.Length; i++)
            if (bridges[i].enabled)
                bridgesOn.Add(bridges[i]);
        bridges = bridgesOn.ToArray();
        //
        for (int i = 0; i < positionsByStages.Length; i++)
            if (positionsByStages[i].w == FindObjectOfType<Manger>().Stage)
                targetPosition = positionsByStages[i]; //transform.position = positionsByStages[i];
    }
    private void OnMouseDown() // (0 - left mouse button) 
    {
        if (Input.GetKey("r")) InstantiateMarkSign(gameObject);
        else { showB = !showB; if (Show != null) Show.SetActive(showB && Show != null); }
    }

    private void OnDisable()
    {
        if (DisableSimulasion != null && FindObjectOfType<Manger>().Stage != 0) Instantiate(DisableSimulasion, transform.position, transform.rotation);
    }

    private void OnEnable()
    {
        if (EnableSimulasion != null && FindObjectOfType<Manger>().Stage != 0) Instantiate(EnableSimulasion, transform.position, transform.rotation);
    }

}