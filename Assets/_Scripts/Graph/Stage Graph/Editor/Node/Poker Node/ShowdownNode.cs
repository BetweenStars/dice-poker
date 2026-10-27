public class ShowdownNode : PassOnlyNode
{
    public ShowdownNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.Showdown;
    }
}