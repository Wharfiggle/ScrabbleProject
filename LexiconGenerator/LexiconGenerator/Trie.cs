[Serializable]
public class Trie {
    public  Node root {get; set;} = new TrieNode();

    public void AddWord(string word) {
        Node curNode = root;

        foreach (char c in word) {
            if(!curNode.children.ContainsKey(c)) {
                curNode.children.Add(c, new Node());
            }
            curNode = curNode.children[c];
        }
        curNode.isSuccessState = true;
    }

    public bool Search(string word) {
        var curNode = root;
        foreach (char c in word) {
            if(!curNode.children.ContainsKey(c)){
                return false;
            }
            curNode = curNode.children[c];
        }
        return true;
    }
}