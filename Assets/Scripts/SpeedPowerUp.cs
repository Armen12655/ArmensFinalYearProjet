using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{

    public float Speed = 8.5f;
    public float rotateSpeed = 50f;

    // Update is called once per frame
    internal void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
    }

    internal void OnTriggerEnter(Collider col)
    {
        UIManager.Instance.changeCharacterSpeed(Speed);
        UIManager.Instance.ExecuteAfterTime(10);
        UIManager.Instance.changeCharacterSpeed(6.0f);
        Destroy(this.gameObject);

    }

}
