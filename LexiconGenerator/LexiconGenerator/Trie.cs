public class Trie {
    private readonly TrieNode root = new TrieNode();

    public void addWord(string word) {
        TrieNode curNode = root;

        foreach (char c in word) {
            if(!curNode.children.ContainsKey(c)) {
                curNode.children.Add(c, new TrieNode());
            }
        }
        curNode.isSuccessState = true;
    }

    public bool search(string word) {
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