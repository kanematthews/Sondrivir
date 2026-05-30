using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using System;
using System.Collections.Generic;
using System.Linq;

/*
=========================================================
SONDRIVIR MMO
UNIFIED NPC GRAPH SYSTEM
=========================================================

ONE FILE.
DELETE ALL OLD GRAPH FILES.

KEEP:
- NPCinteraction
- NPCDialogueUI
- QuestManager
- QuestData
- ItemData
- Merchant systems

DELETE:
- NPCDialogueData
- NPCGraphView
- NPCDialogueGraphWindow
- NodeType
- GraphNodeData
- GraphEdgeData
- ALL OLD GRAPH FILES

=========================================================
*/

public class UnifiedNPCGraphEditor : EditorWindow
{
    // =====================================================
    // MENU
    // =====================================================

    [MenuItem("MMO/Unified NPC Graph")]
    public static void Open()
    {
        UnifiedNPCGraphEditor window =
            GetWindow<UnifiedNPCGraphEditor>();

        window.titleContent =
            new GUIContent(
                "NPC Graph");
    }

    // =====================================================
    // DATA
    // =====================================================

    private NPCDialogueGraphData currentGraph;

    private NPCGraphView graphView;

    private ObjectField graphField;

    // =====================================================
    // ENABLE
    // =====================================================

    private void OnEnable()
    {
        ConstructGraph();

        GenerateToolbar();
    }

    // =====================================================
    // DISABLE
    // =====================================================

    private void OnDisable()
    {
        if (graphView != null)
        {
            rootVisualElement.Remove(
                graphView);
        }
    }

    // =====================================================
    // GRAPH
    // =====================================================

    private void ConstructGraph()
    {
        graphView =
            new NPCGraphView(this);

        graphView.StretchToParentSize();

        rootVisualElement.Add(
            graphView);
    }

    // =====================================================
    // TOOLBAR
    // =====================================================

    private void GenerateToolbar()
    {
        Toolbar toolbar =
            new Toolbar();

        graphField =
            new ObjectField();

        graphField.objectType =
            typeof(
                NPCDialogueGraphData);

        graphField.RegisterValueChangedCallback(
            evt =>
            {
                currentGraph =
                    evt.newValue
                    as NPCDialogueGraphData;

                LoadGraph();
            });

        toolbar.Add(graphField);

        AddNodeButton(
            toolbar,
            "Dialogue",
            NodeType.Dialogue);

        AddNodeButton(
            toolbar,
            "Quest Offer",
            NodeType.QuestOffer);

        AddNodeButton(
            toolbar,
            "Quest Check",
            NodeType.QuestCheck);

        AddNodeButton(
            toolbar,
            "Quest Turn In",
            NodeType.QuestTurnIn);

        AddNodeButton(
            toolbar,
            "Trade",
            NodeType.Trade);

        AddNodeButton(
            toolbar,
            "Condition",
            NodeType.Condition);

        AddNodeButton(
            toolbar,
            "End",
            NodeType.End);

        // -------------------------------------------------

        Button saveButton =
            new Button(() =>
            {
                SaveGraph();
            });

        saveButton.text =
            "Save";

        toolbar.Add(saveButton);

        // -------------------------------------------------

        Button autoLayoutButton =
            new Button(() =>
            {
                graphView.AutoLayout();
            });

        autoLayoutButton.text =
            "Auto Layout";

        toolbar.Add(autoLayoutButton);

        rootVisualElement.Add(
            toolbar);
    }

    // =====================================================
    // ADD NODE BUTTON
    // =====================================================

    private void AddNodeButton(
        Toolbar toolbar,
        string title,
        NodeType type)
    {
        Button button =
            new Button(() =>
            {
                CreateNode(type);
            });

        button.text =
            title;

        toolbar.Add(button);
    }

    // =====================================================
    // CREATE NODE
    // =====================================================

    private void CreateNode(
        NodeType type)
    {
        if (currentGraph == null)
        {
            Debug.LogWarning(
                "No graph selected.");

            return;
        }

        GraphNodeData node =
            new GraphNodeData();

        node.guid =
            Guid.NewGuid().ToString();

        node.nodeType =
            type;

        node.title =
            type.ToString();

        node.position =
            Vector2.zero;

        currentGraph.nodes.Add(
            node);

        graphView.CreateNodeView(
            node);

        EditorUtility.SetDirty(
            currentGraph);
    }

    // =====================================================
    // LOAD GRAPH
    // =====================================================

    private void LoadGraph()
    {
        graphView.ClearGraph();

        if (currentGraph == null)
        {
            return;
        }

        foreach (GraphNodeData node
            in currentGraph.nodes)
        {
            graphView.CreateNodeView(
                node);
        }

        graphView.RestoreEdges();
    }

    // =====================================================
    // SAVE GRAPH
    // =====================================================

    private void SaveGraph()
    {
        if (currentGraph == null)
        {
            return;
        }

        graphView.SaveEdges();

        EditorUtility.SetDirty(
            currentGraph);

        AssetDatabase.SaveAssets();

        Debug.Log(
            "NPC Graph Saved");
    }

    // =====================================================
    // GRAPH VIEW
    // =====================================================

    public class NPCGraphView
        : GraphView
    {
        private UnifiedNPCGraphEditor editor;

        private List<NPCNodeView>
            nodeViews =
                new List<NPCNodeView>();

        // =================================================

        public NPCGraphView(
            UnifiedNPCGraphEditor window)
        {
            editor = window;

            style.flexGrow = 1;

            SetupZoom(
                0.05f,
                3f);

            this.AddManipulator(
                new ContentDragger());

            this.AddManipulator(
                new SelectionDragger());

            this.AddManipulator(
                new RectangleSelector());

            GridBackground grid =
                new GridBackground();

            Insert(0, grid);

            grid.StretchToParentSize();
        }

        // =================================================

        public void ClearGraph()
        {
            DeleteElements(
                graphElements);

            nodeViews.Clear();
        }

        // =================================================

        public void CreateNodeView(
            GraphNodeData data)
        {
            NPCNodeView view =
                new NPCNodeView(
                    data);

            AddElement(view);

            nodeViews.Add(view);
        }

        // =================================================

        public void SaveEdges()
        {
            if (editor.currentGraph == null)
            {
                return;
            }

            editor.currentGraph
                .edges.Clear();

            foreach (Edge edge
                in edges)
            {
                NPCNodeView output =
                    edge.output.node
                    as NPCNodeView;

                NPCNodeView input =
                    edge.input.node
                    as NPCNodeView;

                if (
                    output == null ||
                    input == null)
                {
                    continue;
                }

                GraphEdgeData data =
                    new GraphEdgeData();

                data.outputGUID =
                    output.data.guid;

                data.inputGUID =
                    input.data.guid;

                data.choiceText =
                    "Continue";

                editor.currentGraph
                    .edges.Add(data);
            }

            foreach (NPCNodeView node
                in nodeViews)
            {
                node.data.position =
                    node.GetPosition()
                    .position;
            }
        }

        // =================================================

        public void RestoreEdges()
        {
            foreach (GraphEdgeData edge
                in editor.currentGraph
                    .edges)
            {
                NPCNodeView output =
                    nodeViews.Find(
                        x =>
                        x.data.guid ==
                        edge.outputGUID);

                NPCNodeView input =
                    nodeViews.Find(
                        x =>
                        x.data.guid ==
                        edge.inputGUID);

                if (
                    output == null ||
                    input == null)
                {
                    continue;
                }

                Edge graphEdge =
                    output.outputPort
                    .ConnectTo(
                        input.inputPort);

                AddElement(
                    graphEdge);
            }
        }

        // =================================================

        
        // =================================================

        public override List<Port> GetCompatiblePorts(
            Port startPort,
            NodeAdapter nodeAdapter)
        {
            return ports
                .ToList()
                .Where(
                    endPort =>
                        endPort != startPort &&
                        endPort.direction != startPort.direction &&
                        endPort.node != startPort.node)
                .ToList();
        }

public void AutoLayout()
        {
            float x = 0;
            float y = 0;

            foreach (NPCNodeView node
                in nodeViews)
            {
                node.SetPosition(
                    new Rect(
                        x,
                        y,
                        520,
                        760));

                x += 700;

                if (x > 5000)
                {
                    x = 0;
                    y += 900;
                }
            }
        }
    }

    // =====================================================
    // NODE VIEW
    // =====================================================

    public class NPCNodeView
        : Node
    {
        public GraphNodeData data;

        public Port inputPort;

        public Port outputPort;

        // =================================================

        public NPCNodeView(
            GraphNodeData nodeData)
        {
            data = nodeData;

            title =
                "[" +
                data.nodeType
                    .ToString()
                    .ToUpper() +
                "] " +
                data.title;

            style.width = 520;

            style.height = 760;

            SetColor();

            CreatePorts();

            CreateContents();

            SetPosition(
                new Rect(
                    data.position,
                    new Vector2(
                        520,
                        760)));

            RefreshExpandedState();

            RefreshPorts();
        }

        // =================================================

        private void SetColor()
        {
            Color color =
                Color.gray;

            switch (data.nodeType)
            {
                case NodeType.Dialogue:

                    color =
                        new Color(
                            0.35f,
                            0.35f,
                            0.35f);

                    break;

                case NodeType.QuestOffer:

                    color =
                        new Color(
                            0.85f,
                            0.65f,
                            0.15f);

                    break;

                case NodeType.QuestCheck:

                    color =
                        new Color(
                            0.9f,
                            0.45f,
                            0.15f);

                    break;

                case NodeType.QuestTurnIn:

                    color =
                        new Color(
                            0.2f,
                            0.8f,
                            0.2f);

                    break;

                case NodeType.Trade:

                    color =
                        new Color(
                            0.2f,
                            0.5f,
                            0.9f);

                    break;

                case NodeType.Condition:

                    color =
                        new Color(
                            0.7f,
                            0.3f,
                            0.8f);

                    break;

                case NodeType.End:

                    color =
                        new Color(
                            0.8f,
                            0.2f,
                            0.2f);

                    break;
            }

            titleContainer
                .style
                .backgroundColor =
                    new StyleColor(
                        color);
        }

        // =================================================

        private void CreatePorts()
        {
            inputPort =
                InstantiatePort(
                    Orientation.Horizontal,
                    Direction.Input,
                    Port.Capacity.Multi,
                    typeof(float));

            inputPort.portName =
                "Input";

            inputContainer.Add(
                inputPort);

            outputPort =
                InstantiatePort(
                    Orientation.Horizontal,
                    Direction.Output,
                    Port.Capacity.Multi,
                    typeof(float));

            outputPort.portName =
                "Output";

            outputContainer.Add(
                outputPort);
        }

        // =================================================

        private void CreateContents()
        {
            ScrollView scroll =
                new ScrollView();

            scroll.style.height =
                650;

            // -------------------------------------------------

            TextField titleField =
                new TextField(
                    "Title");

            titleField.value =
                data.title;

            titleField.RegisterValueChangedCallback(
                evt =>
                {
                    data.title =
                        evt.newValue;

                    title =
                        "[" +
                        data.nodeType
                            .ToString()
                            .ToUpper() +
                        "] " +
                        data.title;
                });

            scroll.Add(titleField);

            // -------------------------------------------------

            TextField npcText =
                new TextField(
                    "NPC Dialogue");

            npcText.multiline =
                true;

            npcText.style.height =
                120;

            npcText.value =
                data.npcText;

            npcText.RegisterValueChangedCallback(
                evt =>
                {
                    data.npcText =
                        evt.newValue;
                });

            scroll.Add(npcText);

            // -------------------------------------------------

            AddHeader(
                scroll,
                "QUEST");

            ObjectField questDataField =
                new ObjectField(
                    "Quest Data");

            questDataField.objectType =
                typeof(QuestData);

            questDataField.value =
                data.questData;

            questDataField
                .RegisterValueChangedCallback(
                    evt =>
                    {
                        data.questData =
                            evt.newValue
                            as QuestData;
                    });

            scroll.Add(questDataField);

            TextField enemyID =
                new TextField(
                    "Enemy ID");

            enemyID.value =
                data.enemyID;

            enemyID.RegisterValueChangedCallback(
                evt =>
                {
                    data.enemyID =
                        evt.newValue;
                });

            scroll.Add(enemyID);

            IntegerField killCount =
                new IntegerField(
                    "Kill Count");

            killCount.value =
                data.killCount;

            killCount.RegisterValueChangedCallback(
                evt =>
                {
                    data.killCount =
                        evt.newValue;
                });

            scroll.Add(killCount);

            // -------------------------------------------------

            AddHeader(
                scroll,
                "REWARDS");

            IntegerField xp =
                new IntegerField(
                    "XP Reward");

            xp.value =
                data.xpReward;

            xp.RegisterValueChangedCallback(
                evt =>
                {
                    data.xpReward =
                        evt.newValue;
                });

            scroll.Add(xp);

            IntegerField gold =
                new IntegerField(
                    "Gold Reward");

            gold.value =
                data.goldReward;

            gold.RegisterValueChangedCallback(
                evt =>
                {
                    data.goldReward =
                        evt.newValue;
                });

            scroll.Add(gold);

            // -------------------------------------------------

            AddHeader(
                scroll,
                "TRADE");

            Toggle trade =
                new Toggle(
                    "Opens Trade");

            trade.value =
                data.opensTrade;

            trade.RegisterValueChangedCallback(
                evt =>
                {
                    data.opensTrade =
                        evt.newValue;
                });

            scroll.Add(trade);

            TextField merchant =
                new TextField(
                    "Merchant ID");

            merchant.value =
                data.merchantID;

            merchant.RegisterValueChangedCallback(
                evt =>
                {
                    data.merchantID =
                        evt.newValue;
                });

            scroll.Add(merchant);

            // -------------------------------------------------

            AddHeader(
                scroll,
                "CONDITIONS");

            TextField requiredQuest =
                new TextField(
                    "Required Quest");

            requiredQuest.value =
                data.requiredQuestID;

            requiredQuest.RegisterValueChangedCallback(
                evt =>
                {
                    data.requiredQuestID =
                        evt.newValue;
                });

            scroll.Add(requiredQuest);

            Toggle requiresComplete =
                new Toggle(
                    "Requires Completion");

            requiresComplete.value =
                data.requiresQuestComplete;

            requiresComplete.RegisterValueChangedCallback(
                evt =>
                {
                    data.requiresQuestComplete =
                        evt.newValue;
                });

            scroll.Add(
                requiresComplete);

            // -------------------------------------------------

            Toggle closeDialogue =
                new Toggle(
                    "Close Dialogue");

            closeDialogue.value =
                data.closeDialogue;

            closeDialogue.RegisterValueChangedCallback(
                evt =>
                {
                    data.closeDialogue =
                        evt.newValue;
                });

            scroll.Add(closeDialogue);

            extensionContainer.Add(
                scroll);
        }

        // =================================================

        private void AddHeader(
            ScrollView scroll,
            string text)
        {
            Label label =
                new Label(text);

            label.style.unityFontStyleAndWeight =
                FontStyle.Bold;

            label.style.marginTop =
                10;

            scroll.Add(label);
        }
    }

    // =====================================================
    // ENUMS
    // =====================================================

    public enum NodeType
    {
        Dialogue,
        QuestOffer,
        QuestCheck,
        QuestTurnIn,
        Trade,
        Condition,
        End
    }

    // =====================================================
    // NODE DATA
    // =====================================================

    [Serializable]
    public class GraphNodeData
    {
        public string guid;

        public NodeType nodeType;

        public string title;

        [TextArea(4, 10)]
        public string npcText;

        public Vector2 position;

        // QUEST DATA ASSET

        public QuestData questData;

        // QUEST (legacy / quick-ref fields)

        public string enemyID;

        public int killCount;

        // REWARDS

        public int xpReward;

        public int goldReward;

        // TRADE

        public bool opensTrade;

        public string merchantID;

        // CONDITIONS

        public string requiredQuestID;

        public bool requiresQuestComplete;

        // FLOW

        public bool closeDialogue;
    }

    // =====================================================
    // EDGE DATA
    // =====================================================

    [Serializable]
    public class GraphEdgeData
    {
        public string outputGUID;

        public string inputGUID;

        public string choiceText;
    }
}

// =========================================================
// GRAPH ASSET
// =========================================================

[CreateAssetMenu(
    fileName = "NPCDialogueGraph",
    menuName = "MMO/NPC Graph")]
public class NPCDialogueGraphData
    : ScriptableObject
{
    public string npcName;

    public List<
        UnifiedNPCGraphEditor
        .GraphNodeData>
            nodes =
                new List<
                    UnifiedNPCGraphEditor
                    .GraphNodeData>();

    public List<
        UnifiedNPCGraphEditor
        .GraphEdgeData>
            edges =
                new List<
                    UnifiedNPCGraphEditor
                    .GraphEdgeData>();
}