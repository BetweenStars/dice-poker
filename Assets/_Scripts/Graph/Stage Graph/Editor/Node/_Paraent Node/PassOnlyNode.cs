using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

//기본적인 input output 밖에 없는 노드 부모
public abstract class PassOnlyNode : BaseStageNode
{
    // TODO: 필요한 UI 컴포넌트 선언 (예: TextField, EnumField 등)
    public Port outputPort;
    public PassOnlyNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName) { }

    #region element
    protected override void SetElementsByType()
    {
        // 1. 컴포넌트 및 포트 생성 (new)
        outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Out";

        nodeNameField = new Label() { text = nodeName };

        // 2. 계층 구조 등록 (nodeContainer.Add / bottomPortContainer.Add)
        nodeContainer.Add(nodeNameField);
        bottomPortContainer.Add(outputPort);
    }

    protected override void SetNodeStyleByType()
    {
        // 1. 컨테이너 레이아웃 세팅 (width, flexDirection 등)
        extensionContainer.style.backgroundColor = Color.darkGreen;
        extensionContainer.style.width = Length.Pixels(100);

        nodeContainer.style.flexDirection = FlexDirection.Column;
        nodeContainer.style.width = Length.Percent(100);

        bottomPortContainer.style.flexDirection = FlexDirection.Row;
        bottomPortContainer.style.width = Length.Percent(100);

        // 2. 컴포넌트 세부 시각 스타일 세팅
        nodeNameField.style.fontSize = Length.Pixels(24);
        nodeNameField.style.color = Color.white;
    }
    #endregion

    #region save-load
    public override BaseStageNodeData SaveToData()
    {
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

        return new PassOnlyNodeData()
        {
            nodeGuid = this.nodeGuid,
            nodeName = this.nodeName,
            nodeType = this.nodeType,
            nodePosition = this.GetPosition().position,
            nextNodeGuid = targetGuid
        };
    }

    protected override void LoadByType(BaseStageNodeData data)
    {
        PassOnlyNodeData pData = data as PassOnlyNodeData;

        if (pData == null) { Debug.LogWarning("No PassOnly Node Data"); return; }
    }
    #endregion
}