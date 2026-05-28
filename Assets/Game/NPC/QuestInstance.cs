using System.Collections.Generic;

[System.Serializable]
public class QuestInstance
{
    public QuestData questData;

    public QuestState state;

    // OBJECTIVE PROGRESS

    public List<int> progress =
        new List<int>();
}