using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNode
{
    public Vector2 Position;
    public int Row;
    public int Column;
    public bool Walkable;
}

[ExecuteInEditMode]
public class WorldDecomposition : MonoBehaviour
{

    [SerializeField] private LayerMask CollidingLayer;
    [SerializeField] private Transform Origin;
    [SerializeField] private SpriteRenderer Floor;
    [SerializeField] private float CircleRadius = 1f;
    private int width;
    private int height;
    private const int NodeSize = 3;
    public int totalRows;
    public int totalColumns;
    public WorldNode[,] WorldState;
    private Vector3 StartPoint;

    void Start()
    {
        width = (int)Floor.size.x;
        height = (int)Floor.size.y;
        totalRows = height / NodeSize;
        totalColumns = width / NodeSize;
        WorldState = new WorldNode[ totalRows , totalColumns ];
        DecomposeWorld();
    }

    private void
    DecomposeWorld()
    {
        float startX = Origin.position.x - width / 2;
        float startY = Origin.position.y - height / 2;
        float nodeCenter = NodeSize / 2;

        for( int column = 0; column < totalColumns; column++ )
        {
            for( int row = 0; row < totalRows; row++ )
            {
                float x = startX + nodeCenter + ( NodeSize * column );
                float y = startY + nodeCenter + ( NodeSize * row );

                if( WorldState[ row, column ] == null )
                {
                    WorldState[ row, column ] = new WorldNode();
                }

                Vector2 point = new Vector2( x, y);

                if( Physics2D.CircleCast( point, CircleRadius, Vector2.zero, 0, CollidingLayer ) )
                {
                    WorldState[ row, column ].Walkable = false;
                }
                else
                {
                    WorldState[ row, column ].Walkable = true;
                }

                WorldState[ row, column ].Position = point;
                WorldState[ row, column ].Row = row;
                WorldState[ row, column ].Column = column;
            }
        }
    }

    public WorldNode
    GetWorldNodeFromPosition( Vector2 Position )
    {
        float startX = Origin.position.x - width / 2;
        float startY = Origin.position.y - height / 2;
        float nodeCenter = NodeSize / 2;

        int row = (int)((Position.y - startY - nodeCenter ) / NodeSize);
        int column = (int)((Position.x - startX - nodeCenter ) / NodeSize);

        return WorldState[ row, column ];
    }

    public WorldNode
    FindNearestPathableNode( in WorldNode node )
    {
        WorldNode nearestNode = null;
        int searchRow = node.Row;
        int searchColumn = node.Column;
        //TODO: We need to add each node in a list and then check the list to check if the location may be a corner then we do diagonal checks
        for(int i = -1; i < 1; i++ )//Searching Horizontal Nodes
        {
            if( i == 0 ) continue;
            if( WorldState[ searchRow, searchColumn - i ].Walkable )
            {
                nearestNode = WorldState[ searchRow, searchColumn - i ];
            }
        }

        for( int j = -1; j < 1; j++ )//Searching Vertical Nodes
        {
            if( j == 0 ) continue;
            if( WorldState[ searchRow - j, searchColumn ].Walkable )
            {
                nearestNode = WorldState[ searchRow - j, searchColumn ];
            }
        }

        for(int i = -1; i < 1; i++ )
        {
            for( int j = -1; j < 1; j++ )// Checking Diagonals
            {
                if( i == 0 || j == 0 ) continue;
                else
                {
                    if( WorldState[ searchRow - j, searchColumn - i ].Walkable )
                    {
                        nearestNode = WorldState[ searchRow - j, searchColumn - i ];
                    }
                }
            }
        }

        return nearestNode;
    }

    private void
    OnDrawGizmosSelected()
    {

        if( WorldState != null )
        {
            for( int row = 0; row < totalRows; row++ )
            {
                for( int column = 0; column < totalColumns; column++ )
                {
                    if( WorldState[ row, column ] != null )
                    {
                        if( WorldState[ row, column ].Walkable )
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawSphere( WorldState[ row, column ].Position, CircleRadius );
                        }
                        else
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere( WorldState[ row, column ].Position, CircleRadius );
                        }
                    }
                }
            }
        }
    }
}
