using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DialogueNode : BaseStageNode
{
    public EnumField charIdField;
    public TextField textField;

    // 포트 정의 (선 연결용)
    public Port outputPort;
    public DialogueNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.Dialogue;
    }

    #region element
    protected override void SetElementsByType()
    {
        nodeNameField = new Label() { text = nodeName };
        charIdField = new EnumField("Char ID", CharacterId.None);
        textField = new TextField("Dialogue") { value = "대사를 입력하세요...", multiline = true };

        //bottom port
        outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Out";

        nodeContainer.Add(nodeNameField);
        nodeContainer.Add(charIdField);
        nodeContainer.Add(textField);

        bottomPortContainer.Add(outputPort);
    }
    protected override void SetNodeStyleByType()
    {
        nodeContainer.style.flexDirection = FlexDirection.Column;
        nodeContainer.style.width = Length.Percent(100);
        //nodeContainer.style.marginTop = 5;
        //nodeContainer.style.marginBottom = 5;

        bottomPortContainer.style.justifyContent = Justify.Center;
        bottomPortContainer.style.alignItems = Align.Center;

        //노드 이름
        nodeNameField.style.marginBottom = 10;
        nodeNameField.style.marginLeft = 2;
        nodeNameField.style.fontSize = new StyleLength(StageNodeConfigs.TITLE_FONT_SIZE);

        //캐릭터
        charIdField.labelElement.style.minWidth = 60;
        charIdField.labelElement.style.width = 60;
        charIdField.style.width = 160;
        charIdField.style.marginBottom = 2;

        //대사
        //textField.RegisterValueChangedCallback(()=>{});
        textField.labelElement.style.minWidth = 60;
        textField.labelElement.style.width = 60;
        textField.style.whiteSpace = WhiteSpace.Normal;
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

        return new DialogueNodeData()
        {
            nodeGuid = this.nodeGuid,
            nodeName = this.nodeName,
            nodeType = this.nodeType,
            nodePosition = this.GetPosition().position,
            charId = (CharacterId)this.charIdField.value,
            text = this.textField.value,
            nextNodeGuid = targetGuid
        };
    }
    protected override void LoadByType(BaseStageNodeData data)
    {
        DialogueNodeData dData = data as DialogueNodeData;

        if (dData == null) { Debug.LogError("No Dialogue Data"); return; }

        textField.value = dData.text;
        charIdField.value = dData.charId;
    }
    #endregion
}