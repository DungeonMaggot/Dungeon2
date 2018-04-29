using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Toolbelt
{
    public Toolbelt()
    {

    }
    public int rand_Int_of_List(List<int> set)//memo at me: learn Templates... c++ AND c#
    {
        int answer = set[Random.Range(0, set.Count - 1)];
        return answer;
    }
    public Vertex rand_Vertex_of_List(List<Vertex> set)//memo at me: learn Templates... c++ AND c#
    {
        Vertex answer = set[Random.Range(0, set.Count - 1)];
        return answer;
    }
}

class Vertex
{
    public List<Vertex> m_neighbors;
    public Vector2Int m_position;
    public bool m_start;
    public bool m_end;

    public Vertex(Vector2Int position_, bool start_, bool end_)
    {
        m_neighbors = new List<Vertex>();
        m_position = position_;

        m_start = start_;
        m_end = end_;
    }
}

class Chunk
{
    //Positional Variables
    //Origin of the Macro Cell
    Vector2Int m_origin;
    //Dimension of Tiles in the Cell xyz
    Vector2Int m_size_chunk;
    //Min Max of Vertex in a Cell
    Vector2Int m_amount_vertex_min_max;
    //entry from the cell before this chunk
    Vector2Int m_entry_pos;
    //exit form this cell in the next
    Vector2Int m_exit_pos;
    //amount vertex of the generation
    int m_amount_vertex;
    //list of Vertex
    List<Vertex> m_vertex_in_chunk;

    // Default Constructor
    public Chunk(Vector2Int origen_, Vector2Int size_chunk_, Vector2Int entry_pos_, Vector2Int exit_pos_, Vector2Int amount_vertex_min_max_)
    {
        //Ini Variables
        m_origin = origen_;

        m_size_chunk = size_chunk_;

        m_entry_pos = entry_pos_;

        m_exit_pos = exit_pos_;

        m_amount_vertex_min_max = amount_vertex_min_max_;

        m_amount_vertex = Random.Range(m_amount_vertex_min_max.x, m_amount_vertex_min_max.y);

        m_vertex_in_chunk = new List<Vertex>();

        

        //place the vertex in the Chunk
        for (int i = 0; i < m_amount_vertex; i++)
        {
            //add Entry
            if (i == 0)
            {
                m_vertex_in_chunk.Add(new Vertex(m_entry_pos, true, false));
            }
            //add Exit
            if (i == m_amount_vertex - 1)
            {

                m_vertex_in_chunk.Add(new Vertex(m_exit_pos, false, true));
            }
            //generating random XYZ
            else
            {
                int rand_X = Random.Range(m_origin.x + 1, m_size_chunk.x + m_origin.x - 1);
                int rand_Y = Random.Range(m_origin.y + 1, m_size_chunk.y + m_origin.y - 1);

                Vector2Int position = new Vector2Int(rand_X, rand_Y);
                m_vertex_in_chunk.Add(new Vertex(position, false, false));
            }
            
        }

        //generating Way mesh
        int last_vertex = 0;
        int new_vertex = 1;

        for (int i = 2; i < m_vertex_in_chunk.Count; i++)
        {
            m_vertex_in_chunk[last_vertex].m_neighbors.Add(m_vertex_in_chunk[new_vertex]);
            m_vertex_in_chunk[new_vertex].m_neighbors.Add(m_vertex_in_chunk[last_vertex]);

            last_vertex = new_vertex;
            new_vertex++;
        }
        //connect end to path
        m_vertex_in_chunk[m_vertex_in_chunk.Count - 1].m_neighbors.Add(m_vertex_in_chunk[last_vertex]);
        m_vertex_in_chunk[last_vertex].m_neighbors.Add(m_vertex_in_chunk[m_vertex_in_chunk.Count - 1]);

    }

    //--Getters--
    public Vector2Int amount_vertex_min_max_getter()
    {
        return m_amount_vertex_min_max;
    }
    public Vector2Int origin_getter()
    {
        return m_origin;
    }
    public Vector2Int size_chunk_getter()
    {
        return m_size_chunk;
    }
    public Vector2Int entry_getter()
    {
        return m_entry_pos;
    }
    public Vector2Int exit_getter()
    {
        return m_exit_pos;
    }
    public List<Vertex> vertex_chunk_getter()
    {
        return m_vertex_in_chunk;
    }
}

class Chunks_content_Manager
{

    Vector2Int m_size_chunk_array;

    Vector2Int m_size_chunk;

    Vector2Int m_amount_vertex_min_max;

    List<Chunk> m_map_chunk;

    int m_amount_tiels_in_chunk;

    public Chunks_content_Manager(Vector2Int m_size_chunk_array_, Vector2Int m_size_chunk_, Vector2Int m_amount_vertex_min_max_)
    {
        //Ini Variables
        m_size_chunk_array = m_size_chunk_array_;
        m_size_chunk = m_size_chunk_;
        m_amount_vertex_min_max = m_amount_vertex_min_max_;

        m_amount_tiels_in_chunk = m_size_chunk_array.x * m_size_chunk_array.y * m_size_chunk.x * m_size_chunk.y;

        //Working Variables
        List<Vertex> room_map = new List<Vertex>();
        m_map_chunk = new List<Chunk>();

        Vector2Int current_origen;
        Vector2Int current_size_chunk = m_size_chunk;
        Vector2Int current_entry;
        Vector2Int current_exit;

        Vector2Int current_amount_vertex_min_max = m_amount_vertex_min_max; //option for variation?

        //generating chunks
        for (int i = 0; i < m_size_chunk_array.y; i++)//y
        {
            for (int j = 0; j < m_size_chunk_array.x; j++)//x
            {
                //define currend_origon
                //memo at me Mathf.RoundToInt is doof... by .5 it gives back the even number
                current_origen = new Vector2Int(m_size_chunk.x * j, m_size_chunk.y * i);

                // define Entry and define Exit of the Chunk

                // define entrance/exit tile candidates
                Vector2Int nord_chunk_joint =       new Vector2Int(   current_origen.x + Mathf.RoundToInt((float)m_size_chunk.x / 2f),
                                                    current_origen.y);

                Vector2Int east_chunk_joint =       new Vector2Int(   current_origen.x + m_size_chunk.x,
                                                    current_origen.y + Mathf.RoundToInt((float)m_size_chunk.y / 2f));

                Vector2Int south_chunk_joint =      new Vector2Int(  current_origen.x + Mathf.RoundToInt((float)m_size_chunk.x / 2f),
                                                    current_origen.y + m_size_chunk.y);

                Vector2Int west_chunk_joint =       new Vector2Int(   current_origen.x,
                                                    current_origen.y + Mathf.RoundToInt((float)m_size_chunk.y / 2f));

                //upper left corner (start corner)
                if (j == 0 && i == 0)
                {
                    current_entry = new Vector2Int(0, 0);

                    current_exit = east_chunk_joint;
                }
                //all chunks which are not on the x borders
                else if (j < m_size_chunk_array.x - 1 && j > 0)
                {
                    current_entry = west_chunk_joint;

                    current_exit = east_chunk_joint;
                }
                //all chunks on the right and have a even index
                else if (j == m_size_chunk_array.x - 1 && i % 2 == 0)
                {
                    current_entry = west_chunk_joint;

                    current_exit = south_chunk_joint;
                }
                // all chunks on the right and have a odd index
                else if (j == m_size_chunk_array.x - 1 && i % 2 != 0)
                {
                    current_entry = nord_chunk_joint;

                    current_exit = west_chunk_joint;
                }
                //all chunks on the left and have a even index
                else if (j == 0 && i % 2 == 0)
                {
                    current_entry = nord_chunk_joint;

                    current_exit = east_chunk_joint;
                }
                // all chunks on the left and have a odd index
                else if (j == 0 && i % 2 != 0)
                {
                    current_entry = east_chunk_joint;

                    current_exit = south_chunk_joint;
                }
                // 0 0 if fail
                else
                {
                    Debug.Log("Warning: indecisive Chunk/n" + " x: " + j + "/n y: " + i);

                    current_entry = new Vector2Int(0, 0);

                    current_exit = new Vector2Int(0, 0);
                }
                //fill the Chunk map
                m_map_chunk.Add(new Chunk(current_origen, current_size_chunk, current_entry, current_exit, current_amount_vertex_min_max));
            }

        }
    }

    //--Getters--
    public Vector2Int amount_vertex_min_maxsetter_getter()
    {
        return m_amount_vertex_min_max;
    }
    public Vector2Int size_chunk_array_getter()
    {
        return m_size_chunk_array;
    }
    public Vector2Int size_chunk_getter()
    {
        return m_size_chunk;
    }
    public List<Chunk> map_chunk_getter()
    {
        return m_map_chunk;
    }

    //--Setters--
    public void amount_vertex_min_max_setter(Vector2Int m_amount_vertex_min_max_)
    {
        m_amount_vertex_min_max = m_amount_vertex_min_max_;
    }
    public void size_chunk_array_setter(Vector2Int size_chunk_array_)
    {
        m_size_chunk_array = size_chunk_array_;
    }
    public void size_chunk_setter(Vector2Int m_size_chunk_)
    {
        m_size_chunk = m_size_chunk_;
    }
}

class String_Map_builder
{
    string m_flor_symbol = "+";
    Vector2Int m_amount_vertex_min_max;
    Vector2Int m_size_chunk_array;
    Vector2Int m_size_chunk;

    List<Chunk> m_chunk_map;
    List<string> m_string_map;



    public String_Map_builder(Vector2Int m_size_chunk_array_, Vector2Int m_size_chunk_, Vector2Int m_amount_vertex_min_max_)
    {
        //Ini Variables
        m_size_chunk_array = m_size_chunk_array_;
        m_size_chunk = m_size_chunk_;
        m_amount_vertex_min_max = m_amount_vertex_min_max_;

        Chunks_content_Manager abst_map = new Chunks_content_Manager(m_size_chunk_array, m_size_chunk, m_amount_vertex_min_max);
        m_chunk_map = abst_map.map_chunk_getter();

        //build ini map
        m_string_map = build_string_map();



    }
    //--Getters--
    public Vector2Int amount_vertex_min_max_getter()
    {
        return m_amount_vertex_min_max;
    }
    public Vector2Int size_chunk_array_getter()
    {
        return m_size_chunk_array;
    }
    public Vector2Int size_chunk_getter()
    {
        return m_size_chunk;
    }
    public List<string> string_map_getter()
    {
        return m_string_map;
    }

    //--Setters--
    public void amount_vertex_min_max_setter(Vector2Int m_amount_vertex_min_max_)
    {
        m_amount_vertex_min_max = m_amount_vertex_min_max_;
    }
    public void size_chunk_array_setter(Vector2Int size_chunk_array_)
    {
        m_size_chunk_array = size_chunk_array_;
    }
    public void size_chunk_setter(Vector2Int m_size_chunk_)
    {
        m_size_chunk = m_size_chunk_;
    }

    //additional Functions
    public List<string> build_string_map() //memo at me: we need a better way to save maps...
    {
        //-----convert to one string
        List<string> current_string_map = new List<string>();
        //build a blank map
        for (int y = 0; y < m_size_chunk_array.y * m_size_chunk.y; y++)
        {
            for (int x = 0; x < m_size_chunk_array.x * m_size_chunk.x; x++)
            {
                current_string_map.Add(" ");
            }

        }

        //fill map whit ways
        //for every chunk
        for (int array_y = 0; array_y < m_size_chunk_array.y; array_y++)
        {
            for (int array_x = 0; array_x < m_size_chunk_array.x; array_x++)
            {
                Chunk current_chunk = m_chunk_map[array_x + array_y * m_size_chunk_array.x];
                int amount_of_points = current_chunk.vertex_chunk_getter().Count;
                Vertex last_vertex = new Vertex(new Vector2Int(-1, -1), false, false);

                //for every vertex in cell
                for (int i = 0; i < amount_of_points; i++)
                {
                    Vertex current_vertex = current_chunk.vertex_chunk_getter()[i];
                    Vector2Int current_position = current_vertex.m_position;

                    //for every Neighbors
                    for (int j = 0; j < current_vertex.m_neighbors.Count; j++)
                    {
                        Vector2Int next_position = current_vertex.m_neighbors[j].m_position;

                        //skip already processed Edges 
                        if (next_position != last_vertex.m_position)
                        {
                            //filling the way left and right
                            while (current_position.x != next_position.x)
                            {
                                current_string_map[current_position.x + current_position.y * m_size_chunk.x * m_size_chunk_array.x] = m_flor_symbol;
                                //it go´s left
                                if ((next_position.x - current_position.x) < 0)
                                {
                                    current_position.x -= 1;
                                }
                                //it go´s right
                                else if ((next_position.x - current_position.x) > 0)
                                {
                                    current_position.x += 1;
                                }
                            }
                            //filling the way up and down
                            while (current_position.y != next_position.y)
                            {
                                current_string_map[current_position.x + current_position.y * m_size_chunk.x * m_size_chunk_array.x] = m_flor_symbol;
                                //it go´s down
                                if ((next_position.y - current_position.y) < 0)
                                {
                                    current_position.y -= 1;
                                }
                                //it go´s up 
                                else if ((next_position.y - current_position.y) > 0)
                                {
                                    current_position.y += 1;
                                }
                            }
                            //remember last vertex
                            last_vertex = current_vertex;
                        }                    
                    }
                }
            }
        }
        return current_string_map;
    }
}



public class LevelLayoutGenerator : LevelLayoutInitializer {

    // NOTE: Level coordinate (x = 0, y = 0) is the bottom left corner of the map.

    public Vector2Int m_size_chunk_array_ = new Vector2Int(2, 2);
    public Vector2Int m_size_chunk_ = new Vector2Int(10, 10);
    public Vector2Int m_amount_vertex_min_max_ = new Vector2Int(3, 6);
    List<string> current_map;

    public override LevelLayout ProvideLevelLayout()
    {
        String_Map_builder builder = new String_Map_builder(m_size_chunk_array_, m_size_chunk_, m_amount_vertex_min_max_);
        current_map = builder.string_map_getter();

        string layout_String = "";
        
        //convert List in to needed format
        for (int i = 0; i < current_map.Count; i++)
        {
            layout_String += current_map[i];
        }
        //give result
        LevelLayout result = new LevelLayout(layout_String, m_size_chunk_array_.x * m_size_chunk_.x);

        return result;
    }
}
