using System.Runtime.CompilerServices;

[Serializable]

public class Dawg
{
    public Node Root = new Node();
    public HashSet<Node> FinalStates { get; set; } = [];
    public HashSet<Node> NonFinalStates { get; set; } = [];
    private readonly char[] charSet = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
     'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
    
    public Dawg() {

    }

    // Trie Traversal must be done in a post-order manner
    public void Minimize(){

    }
    /*
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

    /* Hopcrofts Algorithm : https://en.wikipedia.org/wiki/DFA_minimization
P := {F, Q \ F}
W := {F, Q \ F}

while (W is not empty) do
    choose and remove a set A from W
    for each c in Σ do
        let X be the set of states for which a transition on c leads to a state in A
        for each set Y in P for which X ∩ Y is nonempty and Y \ X is nonempty do
            replace Y in P by the two sets X ∩ Y and Y \ X
            if Y is in W
                replace Y in W by the same two sets
            else
                if |X ∩ Y| <= |Y \ X|
                    add X ∩ Y to W
                else
                    add Y \ X to W
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