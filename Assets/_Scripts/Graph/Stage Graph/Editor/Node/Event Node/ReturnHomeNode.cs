using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class ReturnHomeNode : EndNode
{
    // TODO: 필요한 UI 컴포넌트 선언 (예: TextField, EnumField 등)

    public ReturnHomeNode(string nodeGuid, string nodeName) : base(nodeGuid, nodeName)
    {
        nodeType = StageNodeType.ReturnHome;
    }
}