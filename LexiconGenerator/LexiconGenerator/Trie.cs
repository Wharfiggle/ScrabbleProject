[Serializable]
public class Trie
{
    public Node Root { get; set; } = new Node();
    public HashSet<Node> FinalStates { get; set; } = [];
    public HashSet<Node> AllStates { get; set; } = [];
    public void AddWord(string word)
    {
        Node curNode = Root;

        foreach (char c in word)
        {
            if (curNode.Children.TryGetValue(c, out Node? value))  {
                curNode = value;
            } else {
                Node newNode = new()
                {
                    Parent = curNode,
                    ParentChar = c
                }; 
                curNode.Children.Add(c, newNode);
                curNode = curNode.Children[c];
                AllStates.Add(curNode);
            }
            
        }
        curNode.IsSuccessState = true;
        FinalStates.Add(curNode);
    }

    // 0 = false,
    // 1 = true,
    // -1 = search "Fell Off" Trie
    public int Search(string word)
    {
        Node curNode = Root;
        foreach (char c in word.Trim())
        {
            if (!curNode.Children.TryGetValue(c, out Node? value))
            {
                return -1;
            }
            curNode = value;
        }
        if (curNode.IsSuccessState)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}