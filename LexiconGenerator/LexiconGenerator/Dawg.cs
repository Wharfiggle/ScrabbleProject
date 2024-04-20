[Serializable]

public class Dawg
{
    public Node Root = new Node();
    private readonly char[] charSet = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
    // Constructor creates the DAWG from a tree
    public Dawg(Trie tree)
    {
        Root = tree.root;

        // Create a dead node that we can link all missing transitions to
        // Turn the Trie into a DFA, then go through and fill those missing transitions
        Node deadNode = new();
        foreach (char c in charSet)
        {
            deadNode.AddChild(deadNode, c);
        }
        Console.WriteLine("Filling dead transitions");
        fillDeadTransitions(ref deadNode, ref Root);
        Console.WriteLine("Finding Unreachable States");
        //findUnreachableStates(Root);
    }

    private void fillDeadTransitions(ref Node deadNode, ref Node curNode)
    {
        foreach (char c in charSet)
        {
            if (curNode.Children.ContainsKey(c))
            {
                Node childNode = curNode.Children[c];
                fillDeadTransitions(ref deadNode, ref childNode);
            }
            else
            {
                curNode.AddChild(deadNode, c);
            }
        }
    }

    public HopcroftsAlg()
    {

    }
    /* 
    public void findUnreachableStates(Node start)
    {
        Node[] reachableStates = [Root];
        Node[] newStates = [Root];
        do{
            Node[] temp = [];
            foreach (Node q in newStates) {
                foreach (char c in charSet) {

                }
            }
        }while(newStates.Length != 0);
    }
    */
}

// https://stackoverflow.com/questions/14025709/how-to-create-a-dawg