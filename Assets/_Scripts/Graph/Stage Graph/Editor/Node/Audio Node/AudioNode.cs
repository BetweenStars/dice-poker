using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class AudioNode : BaseStageNode
{
    // TODO: 필요한 UI 컴포넌트 선언 (예: TextField, EnumField 등)
    public EnumField audioTypeField;
    public ObjectField clipField;

    public Port outputPort;

    public AudioNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        this.nodeType = StageNodeType.Audio;
    }

    #region element
    protected override void SetElementsByType()
    {
        // 1. 컴포넌트 및 포트 생성 (new)
        audioTypeField = new EnumField("", StageAudioType.BGM);
        clipField = new ObjectField("Target Asset") { objectType = typeof(AudioClip) };

        outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));

        // 2. 계층 구조 등록 (nodeContainer.Add / bottomPortContainer.Add)
        nodeContainer.Add(audioTypeField);
        nodeContainer.Add(clipField);

        bottomPortContainer.Add(outputPort);
    }

    protected override void SetNodeStyleByType()
    {
        // 1. 컨테이너 레이아웃 세팅 (width, flexDirection 등)

        // 2. 컴포넌트 세부 시각 스타일 세팅
    }
    #endregion

    #region save-load
    public override BaseStageNodeData SaveToData()
    {
        // TODO: 알맞은 자식 데이터 클래스(Data 뭉치)를 생성하여 반환하세요.

        //참조 에셋 오브젝트 찾기
        UnityEngine.Object selectedAsset = clipField.value;
        AssetReferenceT<AudioClip> reference = null;

        if (selectedAsset != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedAsset);
            string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);

            if (!string.IsNullOrEmpty(assetGuid))
            {
                reference = new AssetReferenceT<AudioClip>(assetGuid);
            }
        }

        //다음 노드 guid 찾기
        string targetGuid = string.Empty;

        if (outputPort != null && outputPort.connected)
        {
            var edge = outputPort.connections.FirstOrDefault();

            if (edge != null && edge.input != null)
            {
                if (edge.input.node is BaseStageNode targetNode)
                {
                    targetGuid = targetNode.nodeGuid;
                }
            }
        }

        //공용 노드 데이터
        return new AudioNodeData()
        {
            nodeGuid = this.nodeGuid,
            nodeName = this.nodeName,
            nodeType = this.nodeType,
            nodePosition = this.GetPosition().position,
            audioType = (StageAudioType)this.audioTypeField.value,
            clipRef = reference,
            nextNodeGuid = targetGuid
        };
    }

    protected override void LoadByType(BaseStageNodeData data)
    {
        // TODO: 해당 노드 타입 정의 및 데이터 복구 로직 작성
        // nodeType = StageNodeType.YourType;

        AudioNodeData adData = data as AudioNodeData;

        audioTypeField.value = adData.audioType;

        //AssetReference를 Object로
        if (adData.clipRef == null || string.IsNullOrEmpty(adData.clipRef.AssetGUID))
        {
            clipField.value = null;
        }
        else
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(adData.clipRef.AssetGUID);

            if (!string.IsNullOrEmpty(assetPath))
            {
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                clipField.value = asset;
            }
            else
            {
                Debug.LogWarning($"Asset Not found at GUID : {adData.clipRef.AssetGUID}");
                clipField.value = null;
            }
        }
    }
    #endregion
}