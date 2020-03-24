using UnityEngine;
using System.Collections.Generic;
//candy found in https://www.assetstore.unity3d.com/en/#!/content/12512
public class Candy : MonoBehaviour
{
    public int ScorePoints = 100;
    public float rotateSpeed = 50f;

    // Update is called once per frame
    internal void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
    }

    internal void OnTriggerEnter(Collider col)
    {
        UIManager.Instance.IncreaseScore(ScorePoints);
        Destroy(this.gameObject);
    }

}
