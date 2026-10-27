using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dice : MonoBehaviour
{
    private Rigidbody rb;

    private int diceValue;
    public float moveThreshold = 0.05f;
    public float rotateThreshold = 0.05f;

    [SerializeField] private List<GameObject> eyeVectors;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Roll()
    {
        rb.isKinematic = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 force =
            transform.forward +
            transform.up * UnityEngine.Random.Range(-0.1f, 0.2f) -
            transform.right * UnityEngine.Random.Range(0.17f, 0.8f);

        force = force.normalized * UnityEngine.Random.Range(0.6f, 0.9f);

        Debug.Log(force.magnitude);

        rb.AddForce(force, ForceMode.Impulse);

        Vector3 torque = UnityEngine.Random.insideUnitSphere * 10f;

        rb.AddTorque(torque, ForceMode.Impulse);

        Debug.Log($"dice force : {force}");
        rb.AddForce(force, ForceMode.Impulse);
    }

    public IEnumerator WaitForStop()
    {
        yield return new WaitForSeconds(0.1f);

        while (rb.linearVelocity.magnitude > moveThreshold || rb.angularVelocity.magnitude > rotateThreshold)
        {
            yield return null;
        }

        int topEyeIndex = 0;

        for (int i = 1; i < 6; i++)
        {
            if (eyeVectors[i].transform.position.y > eyeVectors[topEyeIndex].transform.position.y) { topEyeIndex = i; }
        }

        diceValue = topEyeIndex + 1;
    }

    //그냥 move rotate는 tweener을 반환해 sequence에 넣거나 안기다려도 되는 움직임에 이용
    //delay move rotate는 각 행동을 기다리게 할때
    //delay move and rotate는 불필요하다 생각
    public Tweener Move(Vector3 pos, float duration = 0)
    {
        return transform.DOMove(pos, duration);
    }

    public IEnumerator DelayMove(Vector3 pos, float duration)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(transform.DOMove(pos, duration));

        yield return seq.WaitForCompletion();
    }
    public Tweener Rotate(Vector3 rot, float duration = 0)
    {
        return transform.DORotate(rot, duration);
    }
    public IEnumerator DelayRotate(Vector3 rot, float duration)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(transform.DORotate(rot, duration));

        yield return seq.WaitForCompletion();
    }
    public void Fix() => rb.isKinematic = true;

    public void SetValue(int value) => diceValue = value;
    public void SetKinematic(bool isKinematic) => rb.isKinematic = isKinematic;

    public int GetDiceValue()
    {
        return diceValue;
    }
}
