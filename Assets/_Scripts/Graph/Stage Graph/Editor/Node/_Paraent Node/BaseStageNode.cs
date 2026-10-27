using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;

public static class StageNodeConfigs
{
    public const int TITLE_FONT_SIZE = 14;
    public const int ELEMENT_FONT_SIZE = 10;
}

public abstract class BaseStageNode : Node
{
    public string nodeGuid;
    public string nodeName;
    public StageNodeType nodeType;

    public VisualElement topPortContainer = new();
    public VisualElement bottomPortContainer = new();
    public VisualElement nodeContainer = new();

    public Label nodeNameField;
    public Port inputPort;

    public BaseStageNode(string nodeGuid, string nodeName)
    {
        this.nodeGuid = nodeGuid;
        this.nodeName = nodeName;

        SetDefaultElements();
        SetElementsByType();

        SetDefaultNodeStyle();
        SetNodeStyleByType();

        extensionContainer.Insert(0, topPortContainer);
        extensionContainer.Add(nodeContainer);
        extensionContainer.Add(bottomPortContainer);

        Highlight(0, Color.white, false);

        //스타일 적용
        RefreshExpandedState();
        RefreshPorts();
    }

    #region element
    private void SetDefaultNodeStyle()
    {
        inputContainer.style.display = DisplayStyle.None;
        outputContainer.style.display = DisplayStyle.None;
        titleContainer.style.display = DisplayStyle.None;

        topPortContainer.style.justifyContent = Justify.Center;
        topPortContainer.style.alignItems = Align.Center;

        bottomPortContainer.style.alignItems = Align.Center;

        //노드 바탕 스타일
        extensionContainer.style.flexDirection = FlexDirection.Column;

        extensionContainer.style.maxWidth = 400;
        extensionContainer.style.minWidth = 50;
        extensionContainer.style.width = 400;

        extensionContainer.style.backgroundColor = Color.gray3;
        extensionContainer.style.paddingRight = 5;
        extensionContainer.style.paddingLeft = 5;
        extensionContainer.style.paddingTop = 5;
        extensionContainer.style.paddingBottom = 5;
    }
    private void SetDefaultElements()
    {
        inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "In";

        topPortContainer.Add(inputPort);
    }
    protected abstract void SetElementsByType();
    protected abstract void SetNodeStyleByType();
    #endregion

    #region  save-load
    public abstract BaseStageNodeData SaveToData();
    public BaseStageNode LoadFromData(BaseStageNodeData data)
    {
        nodeGuid = data.nodeGuid;
        nodeName = data.nodeName;
        nodeType = data.nodeType;

        if (nodeNameField != null)
            nodeNameField.text = nodeName;

        SetPosition(new Rect(data.nodePosition, Vector2.zero));

        LoadByType(data);

        return this;
    }
    protected abstract void LoadByType(BaseStageNodeData data);
    #endregion

    #region highlight
    public void Highlight(int width, Color color, bool on = true)
    {
        if (on)
        {
            mainContainer.style.borderTopWidth = width;
            mainContainer.style.borderBottomWidth = width;
            mainContainer.style.borderRightWidth = width;
            mainContainer.style.borderLeftWidth = width;

            mainContainer.style.borderTopColor = color;
            mainContainer.style.borderBottomColor = color;
            mainContainer.style.borderRightColor = color;
            mainContainer.style.borderLeftColor = color;
        }
        else
        {
            mainContainer.style.borderTopWidth = 0;
            mainContainer.style.borderBottomWidth = 0;
            mainContainer.style.borderRightWidth = 0;
            mainContainer.style.borderLeftWidth = 0;
        }
    }
    #endregion
}