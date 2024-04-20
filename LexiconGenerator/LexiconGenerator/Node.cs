using System;
using System.Collections.Generic;

[Serializable]
public class Node
{
    public bool IsSuccessState { get; set; } = false;

    public Dictionary<char, Node> Children { get; set; } = new Dictionary<char, Node>();


    public void AddChild(Node newChild, char letter)
    {
        try
        {
            Children.Add(letter, newChild);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("An element is already mapped to letter: " + letter);
        }
    }

}