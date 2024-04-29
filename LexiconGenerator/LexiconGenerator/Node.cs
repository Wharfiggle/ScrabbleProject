using System;
using System.Collections.Generic;

[Serializable]
public class Node : IEquatable<Node>
{
    public bool IsSuccessState { get; set; } = false;
    public Node? Parent {get; set;} = null;
 
    // Denotes the Character on the edge leading into the node
    public char ParentChar {get; set;}
    private readonly char[] charSet = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
     'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
    // Denotes the char that the transition from the parent is on.
    public Dictionary<char, Node> Children { get; set; } = new Dictionary<char, Node>();

    public bool Equals(Node? other)
    {
        if (other == null)
        {
            return false;
        }
        if (other.ParentChar != ParentChar){
            return false;
        }
        string mySubtree = findSubtreeString(this);
        string otherSubtree = findSubtreeString(other);
        bool comparison = mySubtree.Equals(otherSubtree);
        return comparison;
    }
    private string findSubtreeString(Node root)
    {
        string result = "";
        foreach (char c in charSet)
        {
            if (root.Children.TryGetValue(c, out Node? value))
            {
                // Stops an infinite loop caused by the dead node 
                if (value.ParentChar == '\0')
                {
                    result += '0';
                }
                else
                {
                    result += value.ParentChar;
                    findSubtreeString(value);
                }
            }
        }
        return result;
    }
    public override bool Equals(object? obj) => Equals(obj as Node);
    public override int GetHashCode() => (IsSuccessState, ParentChar, Parent).GetHashCode();
}