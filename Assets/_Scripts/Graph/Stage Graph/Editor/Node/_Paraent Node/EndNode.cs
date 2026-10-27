using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public abstract class EndNode : BaseStageNode
{
    // TODO: 필요한 UI 컴포넌트 선언 (예: TextField, EnumField 등)

    public EndNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName) { }

    #region element
    protected override void SetElementsByType()
    {
        // 1. 컴포넌트 및 포트 생성 (new)
        nodeNameField = new Label() { text = nodeName };

        nodeContainer.Add(nodeNameField);

        // 2. 계층 구조 등록 (nodeContainer.Add / bottomPortContainer.Add)
    }

    protected override void SetNodeStyleByType()
    {
        // 1. 컨테이너 레이아웃 세팅 (width, flexDirection 등)
        extensionContainer.style.backgroundColor = Color.softRed;
        extensionContainer.style.width = 200;

        // 2. 컴포넌트 세부 시각 스타일 세팅
    }
    #endregion

    #region save-load
    public override BaseStageNodeData SaveToData()
    {
        return new EndNodeData()
        {
            nodeGuid = this.nodeGuid,
            nodeName = this.nodeName,
            nodeType = this.nodeType,
            nodePosition = this.GetPosition().position
        };
    }

    protected override void LoadByType(BaseStageNodeData data)
    {
        EndNodeData endData = data as EndNodeData;

        if (endData == null) { Debug.LogWarning("No End Node Data"); }
    }
    #endregion
}