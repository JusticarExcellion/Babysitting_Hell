using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public WorldNode worldNode;
    public int FScore;
    public Node Parent;

    public Node()
    {
        worldNode = null;
        FScore = 0;
        Parent = null;
    }
}


public class MinHeap
{
    List<Node> Heap = new List<Node>();
    public void
        Enqueue( Node n )
        {
            Heap.Add( n );
            int index = Heap.Count - 1;
            while( index > 0 )
            {
                int parentIndex = ( index - 1) / 2;
                if( Heap[parentIndex].FScore > n.FScore )
                {
                    //NOTE: Swap
                    Node temp = Heap[index];
                    Heap[index] = Heap[parentIndex];
                    Heap[parentIndex] = temp;

                    index = parentIndex;
                }
                else
                {
                    break;
                }

            }
        }

    public Node
        Dequeue()
        {
            if( Heap.Count == 0 ) return null;

            Node min = Heap[0];
            Node LastNode = Heap[ Heap.Count - 1 ];

            Heap[0] = LastNode;
            Heap.RemoveAt( Heap.Count - 1 );
            int index = 0;

            while( true )
            {
                int leftChildIndex = 2 * index + 1;
                int rightChildIndex = 2 * index + 2;
                int smallest = index;

                if( leftChildIndex < Heap.Count && Heap[leftChildIndex].FScore < Heap[smallest].FScore )
                {
                    smallest = leftChildIndex;
                }

                if( rightChildIndex < Heap.Count && Heap[rightChildIndex].FScore < Heap[smallest].FScore )
                {
                    smallest = rightChildIndex;
                }

                if( smallest != index )
                {
                    //NOTE: Swap
                    Node temp = Heap[index];
                    Heap[index] = Heap[smallest];
                    Heap[smallest] = temp;

                    index = smallest;
                }
                else
                {
                    break;
                }
            }

            //Debug.Log( "Node No. 0, Node Value = " + min.FScore );

            return min;
        }


    public int
        HeapSize()
        {
            return Heap.Count;
        }

    public void
        printHeap()
        {
            int index = 0;
            Node n = Heap[index];

            int leftIndex = ( index * 2 ) + 1;
            int rightIndex = (index * 2) + 2;

            Debug.Log( "Node No. " + index + " Node FScore = " + n.FScore );

            if( leftIndex < Heap.Count )
            {
                printNode( leftIndex );
            }

            if( rightIndex < Heap.Count )
            {
                printNode( rightIndex );
            }
        }

    private void
        printNode( int index )
        {
            Node n = Heap[index];

            int leftIndex = ( index * 2 ) + 1;
            int rightIndex = (index * 2) + 2;

            Debug.Log( "Node No. " + index + " Node FScore = " + n.FScore );

            if( leftIndex < Heap.Count )
            {
                Debug.Log("Left Node:");
                printNode( leftIndex );
            }

            if( rightIndex < Heap.Count )
            {
                Debug.Log("Right Node:");
                printNode( rightIndex );
            }
        }
}

public class AStar : MonoBehaviour
{
    public static List<Vector2>
        Path( in WorldNode[,] worldState, int totalRows, int totalColumns, in WorldNode startWorldNode, in WorldNode goalWorldNode )
        {
            if( goalWorldNode == null ) return null;

            //Getting the row and column
            int goalRow = goalWorldNode.Row;
            int goalColumn = goalWorldNode.Column;

            if( !goalWorldNode.Walkable ) //NOTE: if the goal is unreachable
            {
                Debug.Log("Goal Unreachable");
                return null;
            }

            //***********Setup***********//
            Node startNode = new Node();
            startNode.worldNode = startWorldNode;
            startNode.FScore = 0;
            startNode.Parent = null;

            List<Vector2> finalPath = null;

            MinHeap OpenList = new MinHeap();
            OpenList.Enqueue( startNode );

            List<Node> ClosedList = new List<Node>();
            Node currentNode;
            bool Searching = true;

            while( Searching )
            {
                currentNode = OpenList.Dequeue();

                //If there are no more nodes in the open list then there is no path to the target
                if( currentNode == null )
                {
                    Debug.Log("No Path Found!!!");
                    finalPath = null; //NOTE: NO PATH FOUND
                    break;
                }

                int currentRow = currentNode.worldNode.Row;
                int currentColumn = currentNode.worldNode.Column;

                if( currentRow == goalRow && currentColumn == goalColumn ) //NOTE: FOUND GOAL
                {
                    Debug.Log("Path Found!!!"); // Generating the path to the goal
                    GeneratePath( in currentNode, out finalPath );
                    break;
                }

                //Adding all of the surrounding nodes
                for(int rowOffset = -1; rowOffset <= 1; rowOffset++)
                {
                    int searchRow = currentRow + rowOffset;
                    if( searchRow < 0 || searchRow >= totalRows )
                    { //NOTE: if the search row is outside the boundary then skip the row
                        continue;
                    }

                    for(int columnOffset = -1; columnOffset <= 1; columnOffset++)
                    {
                        int searchColumn = currentColumn + columnOffset;

                        if( searchColumn < 0 || searchColumn >= totalColumns )
                        { //NOTE: if the search column is outside the boundaries skip
                            continue;
                        }

                        if( rowOffset == 0 && columnOffset == 0)
                        { //NOTE: Skip searching the current node
                            continue;
                        }

                        if( !worldState[ searchRow, searchColumn].Walkable )
                        { //NOTE: If the node isn't walkable move on
                            continue;
                        }

                        WorldNode searchNode = worldState[ searchRow, searchColumn ];
                        Node newNode = new Node();

                        newNode.worldNode = searchNode;
                        newNode.Parent = currentNode;

                        //Checking for already searched nodes
                        Node temp = newNode;
                        while( temp.Parent == null )
                        {
                            if( temp.worldNode == searchNode )
                            {
                                continue;
                            }
                            temp = temp.Parent;
                        }

                        //****Calculating the F-Score****//

                        //Manhattan-Distance
                        int HValue = 0;
                        HValue += Mathf.Abs( goalRow - currentRow ); // Getting the difference between the rows
                        HValue += Mathf.Abs( goalColumn - currentColumn ); // Getting the difference between the colmuns
                        HValue *= 10;

                        int GCost = currentNode.FScore;

                        if( columnOffset != 0 || rowOffset != 0 )
                        {
                            GCost += 14; // Diagonal GCost
                        }
                        else
                        {
                            GCost += 10; // Horizontal GCost
                        }

                        if( HValue != 0 ) // if the node isn't the goal node then we calculate the FScore
                        {
                            newNode.FScore = GCost + HValue;
                        }
                        else // If it is the goal then we set it to 0 to flag to the algorithm that this is the goal
                        {
                            newNode.FScore = 0;
                        }

                        OpenList.Enqueue( newNode );

                    }
                }

                ClosedList.Add( currentNode );
            }

            //Cleaning up
            OpenList = null;
            ClosedList = null;
            currentNode = null;

            return finalPath;
        }

    public static void
        GeneratePath( in Node EndNode, out List<Vector2> FinalPath ) // Generates the entire path from the last node
        {
            FinalPath = new List<Vector2>();
            Node Current = EndNode;
            while( Current.Parent != null )
            {
                //Debug.Log("Adding new node on path: " + Current.Row + ", " + Current.Column + "; Position: " + Current.Position );
                FinalPath.Add( Current.worldNode.Position );
                Current = Current.Parent;
            }
            FinalPath.Reverse();
        }

}

