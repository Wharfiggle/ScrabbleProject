using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;

/* Creates the Trie
Trie tree = new Trie();

// Fill the trie with words
try {
    StreamReader sr = new StreamReader("sampleWords.txt");

    string line = sr.ReadLine();
    while (line != null)
    {
        tree.AddWord(line);
        line = sr.ReadLine();
    }
    sr.Close();
} catch (Exception e) {
    Console.WriteLine("Exception: " + e.Message);
}*/
// Test if it correctly identifies those words later

// Deserialize the Trie from a file 

try {
    
    StreamReader sr = new StreamReader("sampleWords.txt");
    
    StreamReader sr2 = new StreamReader("TrieSer.json");
    string line = sr2.ReadLine();
    Trie? tree = JsonSerializer.Deserialize<Trie>(line);
    hopcroftsAlg(tree);
    line = sr.ReadLine();
    while (line != null)
    {
        Console.WriteLine("Trie recognizes word: " + line + "? " + tree.Search(line));
        line = sr.ReadLine();
    }
    sr.Close();
} catch (Exception e) {
    Console.WriteLine("Exception: " + e.Message);
}

//SerializeObj(tree);

static void SerializeObj(Trie t) {
    string jsonString = JsonSerializer.Serialize(t);
    Console.WriteLine(jsonString);
    File.WriteAllText("TrieSer.json", jsonString);
}

static void hopcroftsAlg(Trie tree) {

}