using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

//새 노드 만들면 해야할 것
//노드 생성, 노드 추상 클래스 구현
//Create" "Node()구현
//로드함수에서 생성,데이터동기화 + 선(엣지)재구축
//저장할때 ConainerSO 에서 해당 리스트 클리어하기 

public class StageGraphView : GraphView
{
    public StageGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        SetupContextMenu();
    }
    private void SetupContextMenu()
    {
        this.AddManipulator(new ContextualMenuManipulator(menuEvent =>
        {
            Vector2 mousePosition = menuEvent.mousePosition;
            Vector2 graphMousePosition = contentViewContainer.WorldToLocal(mousePosition);

            // 💬 기본 노드 세트
            menuEvent.menu.AppendAction("Node/대화 노드 추가", action =>
            {
                CreateDialogueNode("Dialogue", graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/선택지 노드 추가", action =>
            {
                CreateChoiceNode("Choice", graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/시작 노드 추가", action =>
            {
                CreateStartNode(graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/귀환 노드 추가", action =>
            {
                CreateEndNode(StageNodeType.ReturnHome, "Return Home", graphMousePosition);
            });
            menuEvent.menu.AppendAction("Node/오디오 노드 추가", action =>
            {
                CreateAudioNode("Audio", graphMousePosition);
            });

            // 🃏 Game 진행 관련 PassOnlyNode 세트
            menuEvent.menu.AppendAction("Node/Game/Ante 노드 추가", action =>
            {
                CreatePassOnlyNode(StageNodeType.Ante, "Ante", graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/Game/Deal 노드 추가", action =>
            {
                CreatePassOnlyNode(StageNodeType.Deal, "Deal", graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/Game/Draw 노드 추가", action =>
            {
                CreatePassOnlyNode(StageNodeType.Draw, "Draw", graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/Game/Bet 노드 추가", action =>
            {
                CreatePassOnlyNode(StageNodeType.Bet, "Bet", graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/Game/Roll 노드 추가", action =>
            {
                CreatePassOnlyNode(StageNodeType.Roll, "Roll", graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/Game/Open 노드 추가", action =>
            {
                CreatePassOnlyNode(StageNodeType.Open, "Open", graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/Game/Showdown 노드 추가", action =>
            {
                CreatePassOnlyNode(StageNodeType.Showdown, "Showdown", graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/Game/Create Default Game Nodes", action =>
            {
                CreateDefaultGameFlow(graphMousePosition);
            });

            menuEvent.menu.AppendAction("Node/스프라이트 노드 추가", action =>
            {
                CreateSpriteNode("Sprite", graphMousePosition);
            });
        }));
    }

    private void CreateDefaultGameFlow(Vector2 startPosition)
    {
        // 1. 생성할 게임 흐름의 순서를 순수 노드 타입 리스트로 미리 정의합니다. (총 11개)
        List<StageNodeType> sequence = new List<StageNodeType>()
    {
        StageNodeType.Ante,
        StageNodeType.Deal,
        StageNodeType.Bet,
        StageNodeType.Roll,
        StageNodeType.Bet,
        StageNodeType.Draw,
        StageNodeType.Bet,
        StageNodeType.Roll,
        StageNodeType.Bet,
        StageNodeType.Open,
        StageNodeType.Showdown
    };

        // 선 연결을 추적하기 위해 바로 직전에 만든 노드를 기억할 변수
        PassOnlyNode previousNode = null;

        // 노드가 오른쪽으로 계단식 배열되도록 줄 세울 간격 세팅
        float xOffset = 0f; // 노드 가로 크기(220px)보다 살짝 크게 잡아서 여유 부여
        float yOffset = 100f;

        // 2. 리스트를 돌면서 순서대로 노드 생성 및 조립
        for (int i = 0; i < sequence.Count; i++)
        {
            StageNodeType currentType = sequence[i];

            // 각 루프마다 오른쪽으로 xOffset만큼 밀어서 배치 좌표 계산
            Vector2 nodePosition = new Vector2(startPosition.x + xOffset, startPosition.y + (i * yOffset));

            // 3. 타입에 맞게 C# 객체 생성 (기존 CreatePassOnlyNode 내부의 switch문과 동일)
            PassOnlyNode currentNode = null;
            switch (currentType)
            {
                case StageNodeType.Ante: currentNode = new AnteNode(GetNewGuid(), "Ante"); break;
                case StageNodeType.Deal: currentNode = new DealNode(GetNewGuid(), "Deal"); break;
                case StageNodeType.Draw: currentNode = new DrawNode(GetNewGuid(), "Draw"); break;
                case StageNodeType.Bet: currentNode = new BetNode(GetNewGuid(), "Bet"); break;
                case StageNodeType.Roll: currentNode = new RollNode(GetNewGuid(), "Roll"); break;
                case StageNodeType.Open: currentNode = new OpenNode(GetNewGuid(), "Open"); break;
                case StageNodeType.Showdown: currentNode = new ShowdownNode(GetNewGuid(), "Showdown"); break;
            }

            if (currentNode == null) continue;

            // 4. 캔버스(GraphView)에 육체 배치
            currentNode.SetPosition(new Rect(nodePosition, Vector2.zero));
            AddElement(currentNode);

            // 🌟 5. [핵심] 이전 노드가 존재한다면 기차처럼 선(Edge)을 연결합니다.
            if (previousNode != null)
            {
                Port output = previousNode.outputPort; // 직전 노드의 Out 포트
                Port input = currentNode.inputPort;    // 방금 만든 노드의 In 포트

                ConnectPorts(output, input);
            }

            // 방금 만든 노드를 다음 루프의 '이전 노드'로 인수인계합니다.
            previousNode = currentNode;
        }

        // 6. 모든 노드가 배치되었으니 좌측 상단 카운터 라벨 리프레시
        UpdateNodeCount();
    }

    public void UpdateNodeCount()
    {

    }
    // --- [데이터 내보내기 / SAVE] ---
    public void SaveGraphToSO(string savePath, bool isSilent = false)
    {
        EpisodeAsset containerSO = AssetDatabase.LoadAssetAtPath<EpisodeAsset>(savePath);
        if (containerSO == null)
        {
            containerSO = ScriptableObject.CreateInstance<EpisodeAsset>();
            AssetDatabase.CreateAsset(containerSO, savePath);
        }
        else
        {
            // 🌟 [수정 및 추가] 세이브 시작 전 모든 바구니를 완전히 비워줍니다!
            containerSO.startNodeData = null;
            containerSO.dialogueNodes.Clear();
            containerSO.choiceNodes.Clear();
            containerSO.passOnlyNodes.Clear(); // 🎯 무한 증식을 막는 핵심 Key!
            containerSO.endNodes.Clear();
            containerSO.spriteNodes.Clear();
            containerSO.audioNodes.Clear();
        }

        // 1. 캔버스의 모든 노드 순회하며 데이터 추출
        nodes.ForEach(graphNode =>
        {
            if (graphNode is BaseStageNode stageNode)
            {
                var data = stageNode.SaveToData();

                if (data is StartNodeData startData)
                {
                    containerSO.startNodeData = startData;
                }
                else if (data is DialogueNodeData dialogueData)
                {
                    containerSO.dialogueNodes.Add(dialogueData);
                }
                else if (data is ChoiceNodeData choiceData)
                {
                    containerSO.choiceNodes.Add(choiceData);
                }
                else if (data is PassOnlyNodeData passData)
                {
                    containerSO.passOnlyNodes.Add(passData);
                }
                else if (data is EndNodeData endData)
                {
                    containerSO.endNodes.Add(endData);
                }
                else if (data is SpriteNodeData spData)
                {
                    containerSO.spriteNodes.Add(spData);
                }
                else if (data is AudioNodeData audioData)
                {
                    containerSO.audioNodes.Add(audioData);
                }
            }
        });

        // 2. 파일 변경 내역 저장 고지
        EditorUtility.SetDirty(containerSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (!isSilent)
        {
            EditorUtility.DisplayDialog("완료", "성공적으로 저장되었습니다.", "확인");
        }
    }

    // --- [데이터 가져오기 / LOAD] ---
    public void LoadGraphFromSO(EpisodeAsset containerSO)
    {
        // 1. 기존 화면 청소
        DeleteElements(graphElements);

        // 노드들을 GUID로 임시 보관해 둘 주소록 (선 연결용)
        Dictionary<string, BaseStageNode> createdNodes = new Dictionary<string, BaseStageNode>();

        //시작 노드 복구
        StartNodeData sData = containerSO.startNodeData;

        if (containerSO.startNodeData != null && !string.IsNullOrEmpty(containerSO.startNodeData.nodeGuid))
        {
            var sNode = new StartNode(sData.nodeGuid, sData.nodeName);
            sNode.LoadFromData(sData);
            AddElement(sNode);
            createdNodes.Add(sNode.nodeGuid, sNode);
        }

        // 2. 대화 노드 복구
        foreach (var dData in containerSO.dialogueNodes)
        {
            // 🌟 지난 답변대로 생성자의 마지막 인자에 기존 GUID를 강제로 꽂아주어야 안 깨집니다!
            var node = new DialogueNode(dData.nodeGuid, dData.nodeName);
            node.LoadFromData(dData);
            AddElement(node);
            createdNodes.Add(node.nodeGuid, node);
        }


        // 3. 선택지 노드 복구
        foreach (var cData in containerSO.choiceNodes)
        {
            var node = new ChoiceNode(cData.nodeGuid, cData.nodeName);
            node.LoadFromData(cData);
            AddElement(node);
            createdNodes.Add(node.nodeGuid, node);
        }

        foreach (var adData in containerSO.audioNodes)
        {
            var node = new AudioNode(adData.nodeGuid, adData.nodeName);
            node.LoadFromData(adData);
            AddElement(node);
            createdNodes.Add(node.nodeGuid, node);
        }

        foreach (var spData in containerSO.spriteNodes)
        {
            var node = new SpriteNode(spData.nodeGuid, spData.nodeName);
            node.LoadFromData(spData);
            AddElement(node);
            createdNodes.Add(node.nodeGuid, node);
        }

        foreach (var passData in containerSO.passOnlyNodes)
        {
            BaseStageNode node = null;

            // 🌟 데이터에 저장된 진짜 타입을 보고 알맞은 자식 객체를 찍어냅니다.
            switch (passData.nodeType)
            {
                case StageNodeType.Ante:
                    node = new AnteNode(passData.nodeGuid, passData.nodeName);
                    break;

                case StageNodeType.Deal:
                    node = new DealNode(passData.nodeGuid, passData.nodeName);
                    break;

                case StageNodeType.Draw:
                    node = new DrawNode(passData.nodeGuid, passData.nodeName);
                    break;

                case StageNodeType.Bet:
                    node = new BetNode(passData.nodeGuid, passData.nodeName);
                    break;

                case StageNodeType.Roll:
                    node = new RollNode(passData.nodeGuid, passData.nodeName);
                    break;

                case StageNodeType.Open:
                    node = new OpenNode(passData.nodeGuid, passData.nodeName);
                    break;

                case StageNodeType.Showdown:
                    node = new ShowdownNode(passData.nodeGuid, passData.nodeName);
                    break;

                default:
                    Debug.LogWarning($"[Load] 지원하지 않는 패스 노드 타입입니다: {passData.nodeType}");
                    continue; // 지원하지 않는 타입이면 다음 루프로 패스
            }


            // 🌟 공통 데이터 동기화 및 캔버스(GraphView) 등록 처리
            if (node != null)
            {
                node.LoadFromData(passData);
                AddElement(node);

                // 선(Edge) 연결 복구 때 GUID로 이 노드를 찾을 수 있도록 주소록에 등록
                createdNodes.Add(node.nodeGuid, node);
            }
        }

        foreach (var endData in containerSO.endNodes)
        {
            BaseStageNode node = null;

            switch (endData.nodeType)
            {
                case StageNodeType.ReturnHome:
                    node = new ReturnHomeNode(endData.nodeGuid, endData.nodeName);
                    break;
            }

            if (node != null)
            {
                node.LoadFromData(endData);
                AddElement(node);

                // 선(Edge) 연결 복구 때 GUID로 이 노드를 찾을 수 있도록 주소록에 등록
                createdNodes.Add(node.nodeGuid, node);
            }
        }

        // 4. 🌟 선(Edge) 연결 복구 (가장 중요)
        //시작 노드 선 연결
        if (!string.IsNullOrEmpty(sData.nextNodeGuid))
        {
            if (createdNodes.TryGetValue(sData.nodeGuid, out var sourceNode) &&
                 createdNodes.TryGetValue(sData.nextNodeGuid, out var targetNode))
            {
                Port output = (sourceNode as StartNode).outputPort;
                Port input = targetNode.inputPort;
                ConnectPorts(output, input);
            }
        }

        // 대화 노드 선 연결
        foreach (var dData in containerSO.dialogueNodes)
        {
            if (string.IsNullOrEmpty(dData.nextNodeGuid)) continue;

            if (createdNodes.TryGetValue(dData.nodeGuid, out var sourceNode) &&
                createdNodes.TryGetValue(dData.nextNodeGuid, out var targetNode))
            {
                Port output = (sourceNode as DialogueNode).outputPort;
                Port input = targetNode.inputPort;
                ConnectPorts(output, input);
            }
        }

        foreach (var adData in containerSO.audioNodes)
        {
            if (string.IsNullOrEmpty(adData.nextNodeGuid)) continue;

            if (createdNodes.TryGetValue(adData.nodeGuid, out var sourceNode) &&
                createdNodes.TryGetValue(adData.nextNodeGuid, out var targetNode))
            {
                Port output = (sourceNode as AudioNode).outputPort;
                Port input = targetNode.inputPort;
                ConnectPorts(output, input);
            }
        }

        foreach (var spData in containerSO.spriteNodes)
        {
            if (string.IsNullOrEmpty(spData.nextNodeGuid)) continue;

            if (createdNodes.TryGetValue(spData.nodeGuid, out var sourceNode) &&
               createdNodes.TryGetValue(spData.nextNodeGuid, out var targetNode))
            {
                Port output = (sourceNode as SpriteNode).outputPort;
                Port input = targetNode.inputPort;
                ConnectPorts(output, input);
            }
        }

        // 선택지 노드 선 연결
        foreach (var cData in containerSO.choiceNodes)
        {
            if (createdNodes.TryGetValue(cData.nodeGuid, out var sourceNode))
            {
                // 선택지 노드 내부의 우측 출력 포트들을 순서대로 리스트화
                var outputPorts = (sourceNode as ChoiceNode).bottomPortContainer.Query<Port>().ToList();

                for (int i = 0; i < cData.choiceOptions.Count; i++)
                {
                    string targetGuid = cData.choiceOptions[i].nextNodeGuid;
                    if (string.IsNullOrEmpty(targetGuid)) continue;

                    if (createdNodes.TryGetValue(targetGuid, out var targetNode))
                    {
                        Port output = outputPorts[i];
                        Port input = targetNode.inputPort;
                        ConnectPorts(output, input);
                    }
                }
            }
        }

        foreach (var pData in containerSO.passOnlyNodes)
        {
            if (string.IsNullOrEmpty(pData.nextNodeGuid)) continue;

            if (createdNodes.TryGetValue(pData.nodeGuid, out var sourceNode) &&
                createdNodes.TryGetValue(pData.nextNodeGuid, out var targetNode))
            {
                Port output = (sourceNode as PassOnlyNode).outputPort;
                Port input = targetNode.inputPort;
                ConnectPorts(output, input);
            }
        }
    }

    private void ConnectPorts(Port output, Port input)
    {
        if (output == null || input == null) return;
        Edge edge = output.ConnectTo(input);
        AddElement(edge);
    }

    #region create-node
    // --- 노드 생성부 생략 (기존과 동일하되 GUID 인자 수용 가능하도록 연결) ---
    private void CreateStartNode(Vector2 position)
    {
        if (nodes.ToList().Any(node => node is StartNode))
        {
            Debug.LogWarning("There is alreay Start Node Exist");
            return;
        }

        var node = new StartNode(GetNewGuid(), "Start Node");
        node.SetPosition(new Rect(position, Vector2.zero));
        AddElement(node);
    }
    private void CreateDialogueNode(string nodeName, Vector2 position)
    {
        var node = new DialogueNode(GetNewGuid(), nodeName);
        node.SetPosition(new Rect(position, Vector2.zero));
        AddElement(node);
    }

    private void CreateChoiceNode(string nodeName, Vector2 position)
    {
        var node = new ChoiceNode(GetNewGuid(), nodeName);
        node.SetPosition(new Rect(position, Vector2.zero));
        node.AddChoicePort("선택지 1");
        node.AddChoicePort("선택지 2");
        AddElement(node);
    }

    private void CreateAudioNode(string nodeName, Vector2 position)
    {
        var node = new AudioNode(GetNewGuid(), nodeName);
        node.SetPosition(new Rect(position, Vector2.zero));
        AddElement(node);
    }

    private void CreateSpriteNode(string nodeName, Vector2 position)
    {
        var node = new SpriteNode(GetNewGuid(), nodeName);
        node.SetPosition(new Rect(position, Vector2.zero));
        AddElement(node);
    }

    private void CreatePassOnlyNode(StageNodeType type, string nodeName, Vector2 position)
    {
        BaseStageNode node = null;

        switch (type)
        {
            case StageNodeType.Ante:
                node = new AnteNode(GetNewGuid(), nodeName);
                break;

            case StageNodeType.Deal:
                node = new DealNode(GetNewGuid(), nodeName);
                break;

            case StageNodeType.Draw:
                node = new DrawNode(GetNewGuid(), nodeName);
                break;

            case StageNodeType.Bet:
                node = new BetNode(GetNewGuid(), nodeName);
                break;

            case StageNodeType.Roll:
                node = new RollNode(GetNewGuid(), nodeName);
                break;

            case StageNodeType.Open:
                node = new OpenNode(GetNewGuid(), nodeName);
                break;

            case StageNodeType.Showdown:
                node = new ShowdownNode(GetNewGuid(), nodeName);
                break;

            default:
                Debug.LogWarning($"[CreatePassOnlyNode] 지원하지 않는 패스 노드 타입입니다: {type}");
                return;
        }

        // 🌟 분기문 아래에서 공통 로직(좌표 설정, 엘리먼트 추가, 개수 갱신)을 한 방에 처리합니다.
        if (node != null)
        {
            node.SetPosition(new Rect(position, Vector2.zero));
            AddElement(node);

            // 앞서 추가했던 좌측 상단 카운터 UI 실시간 갱신 함수가 있다면 여기서 호출!
            UpdateNodeCount();
        }
    }

    private void CreateEndNode(StageNodeType type, string nodeName, Vector2 position)
    {
        BaseStageNode node = null;

        switch (type)
        {
            case StageNodeType.ReturnHome:
                node = new ReturnHomeNode(GetNewGuid(), nodeName);
                break;

            default:
                Debug.LogWarning($"[CreateEndNode] 지원하지 않는 엔드 노드 타입입니다: {type}");
                return;
        }

        if (node != null)
        {
            node.SetPosition(new Rect(position, Vector2.zero));
            AddElement(node);

            UpdateNodeCount();
        }
    }
    #endregion

    private string GetNewGuid() => System.Guid.NewGuid().ToString();

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
            {
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }

    public void HighlightNode(string guid)
    {
        foreach (var n in nodes.ToList().OfType<BaseStageNode>())
        {
            n.Highlight(0, Color.white, false);
        }

        if (string.IsNullOrEmpty(guid)) return;

        var node = nodes.ToList().OfType<BaseStageNode>().FirstOrDefault(node => node.nodeGuid == guid);
        node.Highlight(2, Color.white);
    }

    public void ClearAll()
    {
        DeleteElements(graphElements);
    }
}