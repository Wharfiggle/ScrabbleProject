[Serializable]

public class Dawg {
    public Node root {get; set} = new Node();
    private char[] charSet = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
    // Constructor creates the DAWG from a tree
    public Dawg(Trie tree) {
        root = tree.root;
        Node deadNode = new Node();
        foreach (char c in charSet) {
            deadNode.children.Add(c, deadNode);
        }
        fillDeadTransitions(deadNode);
    }

    private void fillDeadTransitions(Node deadNode) {

    }
    private void getNoTransitions(Node deadNode, Node currNode) {
        foreach (char c in charSet) {
            if (!currNode.children.ContainsKey(c)){
                currNode.children.Add(c, deadNode);
            }
        }
    }
}

// https://stackoverflow.com/questions/14025709/how-to-create-a-dawg