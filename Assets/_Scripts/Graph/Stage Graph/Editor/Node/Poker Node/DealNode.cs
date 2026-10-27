public class DealNode : PassOnlyNode
{
    public DealNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.Deal;
    }
}