using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ChoiceNode : BaseStageNode
{
    public VisualElement nodeNameContainer = new();
    public VisualElement choiceContainer = new();

    public struct ChoicePortData
    {
        public Port port;
        public TextField textField;
    }

    public Button addChoiceButton = new();
    public List<ChoicePortData> choicePorts = new();

    public ChoiceNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.Choice;
    }
    #region element
    protected override void SetElementsByType()
    {
        nodeNameContainer = new();

        nodeNameField = new Label() { text = nodeName };
        addChoiceButton = new Button(() => { AddChoicePort(); }) { text = "+" };

        nodeNameContainer.Add(nodeNameField);
        nodeNameContainer.Add(addChoiceButton);

        nodeContainer.Add(nodeNameContainer);
        nodeContainer.Add(choiceContainer);

        nodeContainer.Add(bottomPortContainer);
    }
    protected override void SetNodeStyleByType()
    {
        //아웃풋 포트 컨테이너
        bottomPortContainer.style.flexDirection = FlexDirection.Row;
        bottomPortContainer.style.justifyContent = Justify.SpaceAround;
        bottomPortContainer.style.width = Length.Percent(100);

        //노드 이름
        nodeNameContainer.style.flexDirection = FlexDirection.Row;
        nodeNameContainer.style.alignItems = Align.Center;
        nodeNameContainer.style.justifyContent = Justify.SpaceBetween;
        nodeNameContainer.style.width = Length.Percent(100);
        nodeNameContainer.style.marginBottom = 10;
        nodeNameContainer.style.marginLeft = 2;

        nodeNameField.style.fontSize = StageNodeConfigs.TITLE_FONT_SIZE;

        //add choice button
        addChoiceButton.style.alignSelf = Align.FlexEnd;
        addChoiceButton.style.flexGrow = 0;
        addChoiceButton.style.flexShrink = 0;
        addChoiceButton.style.minWidth = 15;
        addChoiceButton.style.minWidth = 15;

        addChoiceButton.style.marginTop = 0;
        addChoiceButton.style.marginBottom = 0;
        addChoiceButton.style.marginLeft = 0;
        addChoiceButton.style.marginRight = 2;

        addChoiceButton.style.paddingLeft = 0;
        addChoiceButton.style.paddingRight = 0;
        addChoiceButton.style.paddingBottom = 0;
        addChoiceButton.style.paddingTop = 0;
    }
    #endregion

    #region choice port
    public void AddChoicePort(string overwrittenName = "새 선택지")
    {
        // 출력 포트 생성
        var port = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        port.portName = $"{choicePorts.Count + 1}"; // 이름은 필드 옆에 두므로 비웁니다.

        // 포트 삭제 버튼
        var deleteButton = new Button(() => RemoveChoicePort(port)) { text = "-" };

        // 선택지 텍스트 입력 필드
        var choiceTextField = new TextField() { value = overwrittenName, style = { flexGrow = 1 } };

        // 포트 컨테이너 구성 (필드 + 삭제버튼 + 포트)
        var customContainer = new VisualElement();
        customContainer.style.flexDirection = FlexDirection.Row;
        customContainer.Add(choiceTextField);
        customContainer.Add(deleteButton);
        choiceContainer.Add(customContainer);

        bottomPortContainer.Add(port);

        choicePorts.Add(new ChoicePortData { port = port, textField = choiceTextField });

        RefreshPorts();
        RefreshExpandedState();
    }
    private void RemoveChoicePort(Port port)
    {
        int index = choicePorts.FindIndex(x => x.port == port);
        if (index == -1) return;

        if (port.connected)
        {
            var connections = new List<Edge>(port.connections);
            foreach (var edge in connections)
            {
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                edge.RemoveFromHierarchy();
            }
        }

        bottomPortContainer.Remove(port);
        choiceContainer.RemoveAt(index);
        // 4. 리스트에서 데이터 제거
        choicePorts.RemoveAt(index);

        for (int i = 0; i < choicePorts.Count; i++)
        {
            choicePorts[i].port.portName = (i + 1).ToString();
        }

        // UI 갱신 필수
        RefreshPorts();
        RefreshExpandedState();
    }
    #endregion

    #region save-load
    public override BaseStageNodeData SaveToData()
    {
        List<ChoiceOptionData> options = new List<ChoiceOptionData>();

        foreach (var op in choicePorts)
        {
            string targetGuid = string.Empty;

            // 🌟 포트에 선이 연결되어 있다면 목적지 노드를 추적합니다.
            if (op.port != null && op.port.connected)
            {
                // 단선 연결이므로 첫 번째 연결선(Edge)을 가져옴
                var edge = op.port.connections.FirstOrDefault();
                if (edge != null && edge.input != null)
                {
                    // 선의 목적지 입력 포트를 가진 주인 노드를 우리 베이스 노드로 형변환
                    if (edge.input.node is BaseStageNode targetNode)
                    {
                        targetGuid = targetNode.nodeGuid; // 목적지 주민번호 획득!
                    }
                }
            }

            // 리스트에 데이터 추가
            options.Add(new ChoiceOptionData()
            {
                optionText = op.textField.value, // 유저가 텍스트 필드에 쓴 문자열
                nextNodeGuid = targetGuid        // 방금 구한 목적지 노드 GUID
            });
        }

        // 최종 데이터 뭉치 반환
        return new ChoiceNodeData()
        {
            nodeGuid = this.nodeGuid,
            nodeName = this.nodeName,
            nodePosition = this.GetPosition().position,
            nodeType = this.nodeType,
            choiceOptions = options // 위에서 조립한 리stit 밀어넣기
        };
    }
    protected override void LoadByType(BaseStageNodeData data)
    {
        ChoiceNodeData cData = data as ChoiceNodeData;

        if (cData == null) { Debug.LogError("No Choice Data"); return; }

        nodeType = StageNodeType.Choice;

        foreach (var op in cData.choiceOptions)
        {
            AddChoicePort(op.optionText);
        }
    }
    #endregion
}