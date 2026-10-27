public class OpenNode : PassOnlyNode
{
    public OpenNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.Open;
    }
}