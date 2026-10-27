using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public Card[] cards;
    public List<Card> shuffleCards = new();

    [Header("Spread Angle Fields")]
    public float radius = 8;
    public float angle = 10;
    public float spreadOffset = 0.01f;

    [Header("Spread Speed Fields")]
    public float spreadDuration = 1f;
    public float spreadAccel = 0.7f;
    public float spreadPower = 2.3f;
    public float spreadMin = 0.1f;

    #region deck-functions
    public IEnumerator Shuffle()
    {
        yield return Gather(0.5f);
        yield return Spread();
        yield return Gather(0.5f);
    }
    public IEnumerator Gather(float duration)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < cards.Length; i++)
        {
            Vector3 pos = transform.position + new Vector3(0, i * 0.0005f, 0);

            seq.Join(cards[i].transform.DOMove(pos, duration)).SetSpeedBased();

            float randomYAngle = UnityEngine.Random.Range(-1f, 1f);

            Vector3 randomRotation = new Vector3(0f, randomYAngle, 0f);

            seq.Join(cards[i].transform.DOLocalRotate(randomRotation, 0f));
        }

        yield return seq.WaitForCompletion();

        yield return null;
    }

    /// <summary>
    /// index는 카드 위에서 몇번째 카드인지
    /// </summary>
    /// <param name="index">index번째 카드를</param>
    /// <param name="target">위치로</param>
    /// <returns></returns>
    public IEnumerator MoveCardTo(int index, Transform target)
    {
        Sequence drawSeq = DOTween.Sequence();

        drawSeq.Join(cards[cards.Length - 1 - index].transform.DOMove(target.position, 0.5f));
        drawSeq.Join(cards[cards.Length - 1 - index].transform.DORotateQuaternion(target.rotation, 0.5f));

        yield return drawSeq.WaitForCompletion();
    }
    public IEnumerator Spread()
    {
        Sequence startSeq = DOTween.Sequence();

        Vector3 startPos = GetRadiusPosition(radius, angle, 0);
        Vector3 startRot = GetRadiusRotation(radius, angle, 0);

        foreach (var card in cards)
        {
            startPos.y = card.transform.localPosition.y;
            startSeq.Join(card.transform.DOLocalMove(startPos, 0.5f));
            startSeq.Join(card.transform.DOLocalRotate(startRot, 0.5f));
        }

        yield return startSeq.WaitForCompletion();

        int N = cards.Length;
        float[] delays = new float[N];
        //i/(N-1)의 1.2제곱인데 밑에 보정값 1이하를 줘서 값의 편차를 줄이고 총합이 2가 되도록 보정

        float totalDelay = 0f;
        for (int i = 0; i < N; i++)
        {
            delays[i] = spreadAccel * Mathf.Pow((float)i / (N - 1), spreadPower) + spreadMin;
            totalDelay += delays[i];

            //Debug.Log(delays[i]);
        }

        float total2 = 0;
        for (int i = 0; i < N; i++)
        {
            delays[i] = spreadDuration * (delays[i] / totalDelay);
            total2 += delays[i];
            //Debug.Log(delays[i]);
        }

        //Debug.Log($"total delay after adjust : {total2}s");

        for (int i = 0; i < cards.Length; i++)
        {
            Sequence spreadSeq = DOTween.Sequence();

            Vector3 bottomPos = GetRadiusPosition(radius, angle, i);
            Vector3 bottomRot = GetRadiusRotation(radius, angle, i);

            for (int j = i; j < cards.Length; j++)
            {
                if (i != j)
                {
                    bottomPos.y = cards[i].transform.localPosition.y;
                    bottomRot.z = 0;
                }
                spreadSeq.Join(cards[j].transform.DOLocalMove(bottomPos, delays[i]));
                spreadSeq.Join(cards[j].transform.DOLocalRotate(bottomRot, delays[i]));
            }

            yield return spreadSeq.WaitForCompletion();
        }
    }
    #endregion

    #region deck-utilities
    private Vector3 GetRadiusPosition(float radius, float angle, int i)
    {
        float a = Mathf.Lerp(angle * 0.5f, -angle * 0.5f, (float)i / (cards.Length - 1));
        a += 90;

        float x = Mathf.Cos(a * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(a * Mathf.Deg2Rad) * radius;

        float h = 0.004f;
        if (i == 0) h = 0;


        Vector3 newLocalPos = new Vector3(x, h, y - radius + spreadOffset);

        return newLocalPos;
    }

    private Vector3 GetRadiusRotation(float radius, float angle, int i)
    {
        float a = Mathf.Lerp(angle * 0.5f, -angle * 0.5f, (float)i / (cards.Length - 1));
        a += 90;

        float tilt = -5f;
        if (i == 0) { tilt = 0; }

        Vector3 newLocalRot = new Vector3(0, -(a - 90), tilt);

        return newLocalRot;
    }
    #endregion
}