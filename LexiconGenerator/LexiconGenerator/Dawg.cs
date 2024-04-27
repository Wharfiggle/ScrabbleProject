using System.Runtime.CompilerServices;

[Serializable]

public class Dawg
{
    public Node Root = new Node();
    private readonly char[] charSet = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
     'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];

    private Stack<Node> uncheckedNodes = new Stack<Node>();
    private string previousWord = "" ;
    public Dawg() {
    
    }

    public void Insert(string word){
        int commonPrefix = findCommonPrefix(word);
        // Minimize
        Minimize()
        Node curNode = uncheckedNodes.Count() == 0 ? Root : uncheckedNodes.Peek(); 
        foreach (char c in word.Substring(commonPrefix, word.Length)){
            if(curNode.Children.TryGetValue(c, out Node? value)){
                curNode = value;
            } else {
                Node newNode = new Node();
                curNode.Children.Add(c, newNode);
                curNode = newNode;
            }
        }
        curNode.IsSuccessState = true;
        previousWord = word;
    }
    // Potentially return an int 
    private int findCommonPrefix(string word) {
        int searchLength = word.Length < previousWord.Length ? word.Length : previousWord.Length;
        for (int i = 0; i < searchLength; i ++) {
            if (word[i] != previousWord[i]) {
               return i; 
            }        
        }
        return 0;
    }

    private void Minimize(){

    }
    /* Commenting everything out because we are trying a different method
    public Dawg(Trie tree)
    {
        Root = tree.Root;
        FinalStates = tree.FinalStates;
        NonFinalStates = tree.AllStates;
        NonFinalStates.RemoveWhere(FilterAllStates);

        // Create a dead node that we can link all missing transitions to
        // Turn the Trie into a DFA, then go through and fill those missing transitions
        Node deadNode = new();
        foreach (char c in charSet)
        {
            deadNode.AddChild(deadNode, c);
        }
        Console.WriteLine("Filling dead transitions");
        FillDeadTransitions(ref deadNode, ref Root);
        Console.WriteLine("Running Hopcrofts");
        HopcroftsAlg(Root);
    }

    private bool FilterAllStates(Node n)
    {
        return FinalStates.Contains(n);
    }
    private void FillDeadTransitions(ref Node deadNode, ref Node curNode)
    {
        foreach (char c in charSet)
        {
            if (curNode.Children.TryGetValue(c, out Node? value))
            {
                FillDeadTransitions(ref deadNode, ref value);
            }
            else
            {
                curNode.Children.Add(c, deadNode);
            }
        }
    }

    public void HopcroftsAlg(Node root)
    {
        HashSet<Node> P = [];
        P.UnionWith(NonFinalStates);
        P.UnionWith(FinalStates);
        //HashSet<Node> W = FinalStates;
        HashSet<Node> W = [];
        W.UnionWith(FinalStates);
        HashSet<Node> X = [];
        while (W.Count != 0)
        {
            // chose a set A from W
            Node A = W.ElementAt(0);
            W.Remove(A);
            foreach (char c in charSet)
            {
                if (A.ParentChar == c)
                {
                    X.Add(A.Parent);
                }
                for(int i = 0; i < P.Count(); i++) {
                    // Potential Issue. What about multi element sets?
                    HashSet<Node> Y = [P.ElementAt(i)];
                    HashSet<Node> intersection = new HashSet<Node>(X.Intersect(Y));
                    HashSet<Node> complement = new HashSet<Node>(Y.Except(X));
                    if (intersection.Count != 0 && complement.Count != 0) {
                        // Replace Y in P by the two sets X n Y and Y \ X
                        P.RemoveWhere(Y.Contains);
                        P.Concat(intersection);
                        P.Concat(complement);

                        if(Y.IsSubsetOf(W)) {
                            W.RemoveWhere(Y.Contains);
                            W.Concat(intersection) ;
                            W.Concat(complement);
                        } else if (intersection.Count <= complement.Count) {
                            W.Concat(intersection);
                        } else {
                            W.Concat(complement);
                        } 
                    } else {
                        W.RemoveWhere(Y.Contains);
                    }
                }
            }
        }
    }

    // Same search Method as Trie
    public int Search(string word)
    {
        var curNode = Root;
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