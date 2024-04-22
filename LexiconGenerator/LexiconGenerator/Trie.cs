[Serializable]
public class Trie
{
    public Node Root { get; set; } = new Node();
    public HashSet<Node> FinalStates {get; set;} = [];
    public HashSet<Node> AllStates {get; set;} = [];
    public void AddWord(string word)
    {
        Node curNode = Root;

        foreach (char c in word.Trim())
        {
            curNode.Children[c] = new Node
            {
                Parent = curNode
            };
            curNode = curNode.Children[c];
            AllStates.Add(curNode);
        }
        curNode.IsSuccessState = true;
        FinalStates.Add(curNode);
    }

    public bool Search(string word)
    {
        var curNode = Root;
        foreach (char c in word.Trim())
        {
            if (!curNode.Children.TryGetValue(c, out Node? value))
            {
                return false;
            }
            curNode = value;
        }
        return curNode.IsSuccessState;
    }
}