using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine.UI;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System;

//hide,show checkbox
//char id
//position
//speed
//sprite id

public class SpriteNode : BaseStageNode
{
    // TODO: 필요한 UI 컴포넌트 선언 (예: TextField, EnumField 등)
    public EnumField actionField;
    public EnumField charIdField;
    public IntegerField posField;
    public EnumField speedField;
    public TextField spriteIdField;

    public Port outputPort;

    public List<VisualElement> containers = new();

    public SpriteNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        this.nodeType = StageNodeType.Sprite;
    }

    #region element
    protected override void SetElementsByType()
    {
        // 1. 컴포넌트 및 포트 생성 (new)
        containers.Clear();

        VisualElement actionCont = new(); containers.Add(actionCont);
        VisualElement charIdCont = new(); containers.Add(charIdCont);
        VisualElement posCont = new(); containers.Add(posCont);
        VisualElement speedCont = new(); containers.Add(speedCont);
        VisualElement spriteCont = new(); containers.Add(spriteCont);

        actionField = new("", SpriteActionType.Show);
        charIdField = new("", CharacterId.None);
        posField = new("", 3);
        posField.RegisterCallback<FocusOutEvent>(evt =>
        {
            int clamped = Mathf.Clamp(posField.value, 1, 5);
            posField.SetValueWithoutNotify(clamped);
        });
        speedField = new("", SpriteSpeedType.Normal);
        spriteIdField = new("") { };

        Label actionLabel = new("action");
        Label charIdLabel = new("char id");
        Label posLabel = new("pos");
        Label speedLabel = new("speed");
        Label spriteIdLabel = new("sprite id");

        outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Out";

        // 2. 계층 구조 등록 (nodeContainer.Add / bottomPortContainer.Add)
        actionCont.Add(actionLabel);
        actionCont.Add(actionField);

        charIdCont.Add(charIdLabel);
        charIdCont.Add(charIdField);

        posCont.Add(posLabel);
        posCont.Add(posField);

        speedCont.Add(speedLabel);
        speedCont.Add(speedField);

        spriteCont.Add(spriteIdLabel);
        spriteCont.Add(spriteIdField);

        foreach (var cont in containers) { nodeContainer.Add(cont); }

        bottomPortContainer.Add(outputPort);
    }

    protected override void SetNodeStyleByType()
    {
        // 1. 컨테이너 레이아웃 세팅 (width, flexDirection 등)
        foreach (var cont in containers)
        {
            cont.style.flexDirection = FlexDirection.Column;
        }

        nodeContainer.style.flexDirection = FlexDirection.Row;

        // 2. 컴포넌트 세부 시각 스타일 세팅
    }
    #endregion

    #region save-load
    public override BaseStageNodeData SaveToData()
    {
        // TODO: 알맞은 자식 데이터 클래스(Data 뭉치)를 생성하여 반환하세요.
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

        return new SpriteNodeData()
        {
            nodeGuid = this.nodeGuid,
            nodeName = this.nodeName,
            nodeType = this.nodeType,
            nodePosition = this.GetPosition().position,
            spriteAction = (SpriteActionType)this.actionField.value,
            charId = (CharacterId)this.charIdField.value,
            position = this.posField.value,
            speed = (SpriteSpeedType)this.speedField.value,
            spriteId = this.spriteIdField.value,
            nextNodeGuid = targetGuid
        };
    }

    protected override void LoadByType(BaseStageNodeData data)
    {
        // TODO: 해당 노드 타입 정의 및 데이터 복구 로직 작성
        // nodeType = StageNodeType.YourType;
        SpriteNodeData nData = data as SpriteNodeData;

        if (nData == null)
        {
            Debug.LogWarning("No Sprite Node Data");
            return;
        }

        actionField.value = nData.spriteAction;
        charIdField.value = nData.charId;
        posField.value = nData.position;
        speedField.value = nData.speed;
        spriteIdField.value = nData.spriteId;
    }
    #endregion
}