using System;
using System.Collections.Generic;

[Serializable]
public class Node : IEquatable<Node>
{
    public bool IsSuccessState { get; set; } = false;
    //public Node? Parent {get; set;} = null;
 
    // Denotes the Character on the edge leading into the node
    //public char ParentChar {get; set;}
    public List<char> ParentChars {get; set;} = [];
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
        if (other.ParentChars != ParentChars){
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
                
                foreach (char character in value.ParentChars) {
                    result += character;
                }
                findSubtreeString(value);
            }
        }
        return result;
    }
    public override bool Equals(object? obj) => Equals(obj as Node);
    public override int GetHashCode() => (IsSuccessState, ParentChars).GetHashCode();
}