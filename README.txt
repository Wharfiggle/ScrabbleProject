Formal Language Project created by: Derek Kedrowski, Elijah Southman, and Evan Dewitt

To run, open the ScrabbleProject/ScrabbleProject folder in vscode, install the required extensions, click on one of the .cs files, 
then hit the expand arrow on the play button at the top right and click "Run project associated with this file."

Required vscode extensions:

C# -Microsoft
C# Dev Kit -Microsoft
MonoGame Content Builder (Editor) -mangrimen


If you want to change the number of players or change the distribution of human players, you can navigate to the ScrabbleProject/ScrabbleProject/Content/PlayerConfig.txt file and edit each line appropriately.
*note: the first player must be a 'player' not a 'cpu'
"player" = a human player
"cpu" = an AI controlled player
"none" = no player
Any lines beyond the fourth line will do nothing, and if there are less than four lines it will be assumed that the rest of the lines are "none." Any invalid lines will also be interpreted as "none."


The MGCB Editor lets you add things like sprites, new fonts, etc to the project.
Type "dotnet mgcb-editor" in the vscode terminal to run the editor.
Then, in the editor, do file > open and select ScrabbleProject\Content\Content.mgcb to edit the project.