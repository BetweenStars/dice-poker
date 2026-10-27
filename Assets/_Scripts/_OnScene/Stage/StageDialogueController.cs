using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot
{
    public CharacterAsset profile;
    public GameObject obj;
    public Image image;
}

public class StageDialogueController : MonoBehaviour
{
    public GameObject slotRoot;
    public Dictionary<int, CharacterSlot> characterSlots = new();
    public GameObject slotPrefab;

    public Transform[] originPositions = new Transform[5];

    public void ShowSlot(CharacterId charId, string spriteId, int pos = 3, float speed = 0.5f)
    {
        if (characterSlots.ContainsKey(pos)) { Debug.Log("nonokey"); return; }
        if (pos == 0) { Debug.LogWarning("Position index can't be 0"); return; }

        CharacterSlot slot = new()
        {
            profile = ResourceManager.Instance.GetCharacterAsset(charId),
            obj = Instantiate(slotPrefab, slotRoot.transform),
        };

        slot.obj.GetComponent<RectTransform>().anchoredPosition = originPositions[pos - 1].GetComponent<RectTransform>().anchoredPosition;

        slot.image = slot.obj.GetComponentInChildren<Image>();

        slot.image.sprite = ResourceManager.Instance.GetCharacterPortraitSprite(charId, spriteId);

        characterSlots.Add(pos, slot);
    }
    public void HideSlot(CharacterId charId)
    {
        var slot = characterSlots.FirstOrDefault(s => s.Value.profile.id == charId);

        if (slot.Value == null) return;

        Destroy(slot.Value.obj);
        characterSlots.Remove(slot.Key);
    }

    public void ChangeSprite(CharacterId charId, string spriteId)
    {
        CharacterSlot slot = characterSlots.FirstOrDefault(s => s.Value.profile.id == charId).Value;

        if (slot == null) { Debug.LogWarning("No char to change sprite"); return; }

        slot.image.sprite = ResourceManager.Instance.GetCharacterPortraitSprite(charId, spriteId);
    }

    public void MoveSlot(CharacterId charId, int pos, float speed)
    {
        if (characterSlots.ContainsKey(pos)) { Debug.Log("Slot is full"); return; }

        var pair = characterSlots.FirstOrDefault(s => s.Value.profile.id == charId);
        if (pair.Value == null) { Debug.LogWarning($"Can't find character in slot{pos}"); return; }

        CharacterSlot slot = pair.Value;
        int oldKey = pair.Key;

        characterSlots.Remove(oldKey);
        characterSlots[pos] = slot;

        slot.obj.transform.position = originPositions[pos - 1].position;
    }

    public int GetPositionByCharId(CharacterId charId) => characterSlots.FirstOrDefault(p => p.Value.profile.id == charId).Key;

    //public float dialougeYPos=25;
    public float bettingYPos = 150;
    public float tableYPos = 400;
    public IEnumerator MoveViewCoroutine(CameraState state, float duration)
    {
        List<CharacterSlot> slots = characterSlots.Values.ToList();

        if (slots.Count == 0) yield break;

        float targetYPos = 0f;
        float targetScale = 2.0f;

        switch (state)
        {
            case CameraState.Default:
            case CameraState.Dialogue:
                targetYPos = originPositions[2]
                    .GetComponentInChildren<RectTransform>()
                    .anchoredPosition.y;
                targetScale = 2.0f;
                break;

            case CameraState.Betting:
                targetYPos = bettingYPos;
                targetScale = 1.8f;
                break;

            case CameraState.Table:
                targetYPos = tableYPos;
                targetScale = 2.2f;
                break;
        }

        // 시작 위치 저장
        Dictionary<RectTransform, Vector2> startPositions = new();
        Dictionary<RectTransform, float> startScales = new();

        foreach (var slot in slots)
        {
            var rect = slot.obj.GetComponent<RectTransform>();
            startPositions.Add(rect, rect.anchoredPosition);
            startScales.Add(rect, rect.localScale.x);
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Ease In Out

            t = Mathf.SmoothStep(0f, 1f, t);

            foreach (var slot in slots)
            {
                var rect = slot.obj.GetComponent<RectTransform>();

                Vector2 startPos = startPositions[rect];

                rect.anchoredPosition = new Vector2(
                    startPos.x,
                    Mathf.Lerp(startPos.y, targetYPos, t)
                );
                rect.localScale = Vector3.one *
                    Mathf.Lerp(startScales[rect], targetScale, t);
            }
            yield return null;
        }

        foreach (var slot in slots)
        {
            var rect = slot.obj.GetComponent<RectTransform>();
            Vector2 pos = rect.anchoredPosition;
            pos.y = targetYPos;
            rect.anchoredPosition = pos;
            rect.localScale = Vector3.one * targetScale;
        }
    }

    #region initialize
    public void Initialize() { }
    public void Reset() { }
    #endregion
}
