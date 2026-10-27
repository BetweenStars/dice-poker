public class DrawNode : PassOnlyNode
{
    public DrawNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.Draw;
    }
}