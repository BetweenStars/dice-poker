using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartNode : BaseStageNode
{
    public Port outputPort;
    public StartNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName) { }

    protected override void SetElementsByType()
    {
        outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        outputPort.portName = "Out";

        nodeNameField = new Label() { text = nodeName };

        nodeContainer.Add(nodeNameField);

        bottomPortContainer.Add(outputPort);
    }
    protected override void SetNodeStyleByType()
    {
        extensionContainer.style.backgroundColor = Color.softRed;
        extensionContainer.style.width = 200;

        topPortContainer.style.display = DisplayStyle.None;

        nodeContainer.style.alignItems = Align.Center;

        bottomPortContainer.style.alignItems = Align.Center;

        nodeNameField.style.fontSize = 30;
    }

    protected override void LoadByType(BaseStageNodeData data)
    {
        StartNodeData sData = data as StartNodeData;

        if (sData == null) { Debug.LogWarning("No StartNode Data"); return; }
    }

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

        return new StartNodeData()
        {
            nodeGuid = this.nodeGuid,
            nodeName = this.nodeName,
            nodeType = this.nodeType,
            nodePosition = this.GetPosition().position,
            nextNodeGuid = targetGuid
        };
    }
}