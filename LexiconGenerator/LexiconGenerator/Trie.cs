[Serializable]
public class Trie
{
    public Node root { get; set; } = new Node();

    public void AddWord(string word)
    {
        Node curNode = root;

        foreach (char c in word)
        {
            if (!curNode.Children.ContainsKey(c))
            {
                curNode.Children.Add(c, new Node());
            }
            curNode = curNode.Children[c];
        }
        curNode.IsSuccessState = true;
    }

    public bool Search(string word)
    {
        var curNode = root;
        foreach (char c in word)
        {
            if (!curNode.Children.ContainsKey(c))
            {
                return false;
            }
            curNode = curNode.Children[c];
        }
        return curNode.IsSuccessState;
    }
}