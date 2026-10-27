public class AnteNode : PassOnlyNode
{
    public AnteNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.Ante;
    }
}