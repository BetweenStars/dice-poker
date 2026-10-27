using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StageEditorWindow : EditorWindow
{
    private StageGraphView _graphView;
    private const string TempSavePath = "Assets/_Scripts/Graph/Stage Graph/Editor/StageEditor_TempCache.asset";

    [MenuItem("Window/Dialogue Graph")]
    public static void OpenWindow()
    {
        var window = GetWindow<StageEditorWindow>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void CreateInitializeUI()
    {
        // 1. 상단 툴바 추가 및 버튼 배치
        var toolbar = new VisualElement { style = { flexDirection = FlexDirection.Row, height = 30, backgroundColor = new Color(0.2f, 0.2f, 0.2f) } };

        // [저장 버튼]
        Button saveButton = new Button(OnClickSave) { text = "SO로 내보내기" };
        saveButton.style.backgroundColor = new Color(0.2f, 0.5f, 0.2f);
        saveButton.style.color = Color.white;
        toolbar.Add(saveButton);

        // [로드 버튼]
        Button loadButton = new Button(OnClickLoad) { text = "SO 가져오기" };
        loadButton.style.backgroundColor = new Color(0.2f, 0.3f, 0.5f);
        loadButton.style.color = Color.white;
        toolbar.Add(loadButton);

        Button clearButton = new Button(OnClickClear) { text = "초기화" };
        clearButton.style.backgroundColor = Color.darkGoldenRod;
        clearButton.style.color = Color.white;
        toolbar.Add(clearButton);

        rootVisualElement.Add(toolbar);

        // 2. 그래프 뷰 영역 추가
        _graphView = new StageGraphView();
        _graphView.StretchToParentSize();
        _graphView.style.top = 30;
        rootVisualElement.Add(_graphView);
    }

    private void OnEnable()
    {
        rootVisualElement.Clear();
        CreateInitializeUI();

        StageNodeController.OnStageChanged += _graphView.LoadGraphFromSO;
        StageNodeController.OnNodeChanged += _graphView.HighlightNode;

        // 🌟 컴파일 복구 팝업: 컴파일 후 임시 캐시가 있다면 자동으로 복구합니다.
        EditorApplication.delayCall += () =>
        {
            EpisodeAsset tempCache = AssetDatabase.LoadAssetAtPath<EpisodeAsset>(TempSavePath);

            if (tempCache != null)
            {
                _graphView.LoadGraphFromSO(tempCache);
                AssetDatabase.DeleteAsset(TempSavePath); // 복구 후 임시 파일 제거
            }
        };
    }

    private void OnDisable()
    {
        if (_graphView == null) return;

        // 🌟 [추가 방어선] 캔버스에 노드가 하나도 없다면 캐시 저장을 패스합니다.
        // (Clear 직후 창이 닫힐 때 빈 파일이 다시 생성되는 것을 원천 차단)
        if (_graphView.nodes == null || _graphView.nodes.ToList().Count == 0)
            return;

        // 코드가 수정되어 컴파일될 때 현재 캔버스 상태를 임시 저장
        _graphView.SaveGraphToSO(TempSavePath, isSilent: true);

        StageNodeController.OnStageChanged -= _graphView.LoadGraphFromSO;
        StageNodeController.OnNodeChanged -= _graphView.HighlightNode;
    }

    private void OnClickSave()
    {
        string savePath = EditorUtility.SaveFilePanelInProject("시나리오 SO 저장", "Assets/Data/StageEventDatas/New Stage Event Data", "asset", "저장할 SO 파일을 선택하세요.");
        if (!string.IsNullOrEmpty(savePath))
        {
            _graphView.SaveGraphToSO(savePath);
        }
    }

    private void OnClickLoad()
    {
        string loadPath = EditorUtility.OpenFilePanel("시나리오 SO 로드",
        "Assets/Data/StageEventDatas", // 프로젝트의 Assets 폴더를 기본 시작 위치로 지정
        "asset");

        if (!string.IsNullOrEmpty(loadPath))
        {
            int assetsIndex = loadPath.IndexOf("Assets");

            if (assetsIndex != -1)
            {
                string relativePath = loadPath.Substring(assetsIndex);

                EpisodeAsset containerSO = AssetDatabase.LoadAssetAtPath<EpisodeAsset>(relativePath);
                if (containerSO != null)
                {
                    _graphView.LoadGraphFromSO(containerSO);
                }
            }
        }
    }


    private void OnClickClear()
    {
        _graphView.ClearAll();

        if (AssetDatabase.LoadAssetAtPath<EpisodeAsset>(TempSavePath) != null)
        {
            AssetDatabase.DeleteAsset(TempSavePath);
            AssetDatabase.Refresh();
            Debug.Log("[StageEditor] 화면 초기화와 함께 임시 캐시 파일이 삭제되었습니다.");
        }
    }
}