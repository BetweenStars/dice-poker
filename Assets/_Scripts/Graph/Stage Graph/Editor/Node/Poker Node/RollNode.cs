public class RollNode : PassOnlyNode
{
    public RollNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.Roll;
    }
}
