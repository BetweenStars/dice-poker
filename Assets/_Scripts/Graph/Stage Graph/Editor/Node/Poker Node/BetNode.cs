using Codice.CM.WorkspaceServer.DataStore;

public class BetNode : PassOnlyNode
{
    public BetNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.Bet;
    }
}