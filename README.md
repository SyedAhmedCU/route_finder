# Route Finder - WPF App
- A desktop application using WPF .NET framework to find all possible paths from a fire-station to a target location where a fire broke out. The map of the fire district is provided as integer number pairs for adjacent street corners in a text format. Output is saved as a text file consist of  separate lines for each route.
- The input text is filtered into a graph which is a dictionary of lists consist of all the nodes as a key and their value is a list of all their neighbouring nodes. All the paths are found using a recursive and iterative algorithm known as depth-first search where starting from a given node (fire station), it iterates over all possible paths until it reaches the goal node (fire).


![route-finder](https://user-images.githubusercontent.com/55814513/189977731-09ba6ac0-a0cd-42f7-a84e-44e5730b5961.png)
