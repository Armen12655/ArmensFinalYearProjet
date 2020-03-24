using UnityEngine;
using System.Collections.Generic;
public class PowerUps : MonoBehaviour
{
    public int ScoreMultipliers;
    public float rotateSpeed = 50f;

    // Update is called once per frame
    internal void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
    }

    internal void OnTriggerEnter(Collider col)
    {
        UIManager.Instance.IncreaseScore(ScoreMultipliers);
        Destroy(this.gameObject);
    }

}
