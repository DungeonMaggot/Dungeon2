using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Toolbeld
{
    public Toolbeld()
    {

    }

    public int int_of_list(List<int> set)//memo at me: lern Templates... c++ AND c#
    {
        int answer = set[Random.Range(0, set.Count - 1)];
        return answer;
    }

    public Vertex_info Vertex_info_of_list(List<Vertex_info> set)//memo at me: lern Templates... c++ AND c#
    {
        Vertex_info answer = set[Random.Range(0, set.Count - 1)];
        return answer;
    }
}

class Vertex_info
{
    public List<Vertex_info> m_neighbors;
    public Vector3Int m_position;
    public bool m_start;
    public bool m_end;

    public Vertex_info(Vector3Int position_, bool start_, bool end_)
    {
        m_neighbors = new List<Vertex_info>();
        m_position = position_;

        m_start = start_;
        m_end = end_;
    }
    //--Getters--
    //dosn´t make sense
    //--Setters--
    //dosn´t make sense
}

class Cell
{
    // Positional Variables
    //Origon of the Macro Cell
    Vector3Int m_origon;
    //Dimmension of Tiles in der Cell xyz
    Vector3Int m_size_cell;
    //Min Max of Vertex in a Cell
    Vector2Int m_amound_vertex_min_max;
    //entry from the cell bevor in this cell
    Vector3Int m_start;
    //exit form this cell in the next
    Vector3Int m_end;
    //amound vertex of the generatishen
    int m_amound_vertex;
    //list of Vertex
    List<Vertex_info> m_vertex_cell;

    // Default Constructor
    public Cell(Vector3Int origon_, Vector3Int size_cell_, Vector3Int start_, Vector3Int end_, Vector2Int amound_vertex_min_max_)
    {
        //if Z is needed: commend m_origon.z = 0; and m_tile.z = 0;
        //Ini Variables
        m_origon = origon_;
        m_origon.z = 0;

        m_size_cell = size_cell_;
        m_size_cell.z = 0;

        m_start = start_;

        m_end = end_;

        m_amound_vertex_min_max = amound_vertex_min_max_;

        m_amound_vertex = Random.Range(m_amound_vertex_min_max.x, m_amound_vertex_min_max.y);

        m_vertex_cell = new List<Vertex_info>();

        //add Start
        m_vertex_cell.Add(new Vertex_info(m_start, true, false));

        //place the vertex in the Cell
        for (int i = 0; i < m_amound_vertex; i++)
        {
            //generating random XYZ
            int rand_X = Random.Range(m_origon.x + 1, m_size_cell.x + m_origon.x - 1);
            int rand_Y = Random.Range(m_origon.y + 1, m_size_cell.y + m_origon.y - 1);
            //int rand_Z = Random.Range(m_origon.z, m_tile.z + m_origon.z); //in case of other use
            int rand_Z = m_origon.z;

            Vector3Int position = new Vector3Int(rand_X, rand_Y, rand_Z);

            m_vertex_cell.Add(new Vertex_info(position, false, false));
        }

        //add End
        m_vertex_cell.Add(new Vertex_info(m_end, false, true));

        //generateing Waymesh
        int last_vertex = 0;
        int new_vertex = 1;

        for (int i = 2; i < m_vertex_cell.Count; i++)
        {
            m_vertex_cell[last_vertex].m_neighbors.Add(m_vertex_cell[new_vertex]);
            m_vertex_cell[new_vertex].m_neighbors.Add(m_vertex_cell[last_vertex]);

            last_vertex = new_vertex;
            new_vertex++;
        }
        m_vertex_cell[m_vertex_cell.Count - 1].m_neighbors.Add(m_vertex_cell[last_vertex]);
        m_vertex_cell[last_vertex].m_neighbors.Add(m_vertex_cell[m_vertex_cell.Count - 1]);

    }

    //--Getters--
    public Vector2Int amound_vertex_min_maxsetter_getter()
    {
        return m_amound_vertex_min_max;
    }
    public Vector3Int origon_getter()
    {
        return m_origon;
    }
    public Vector3Int size_cell_getter()
    {
        return m_size_cell;
    }
    public Vector3Int start_getter()
    {
        return m_start;
    }
    public Vector3Int end_getter()
    {
        return m_end;
    }
    public List<Vertex_info> vertex_cell_getter()
    {
        return m_vertex_cell;
    }
    //--Setters--
    //dosn´t make sense
}

class Map_abstracter
{

    Vector2Int m_size_cell_array;

    Vector3Int m_size_cell;

    Vector2Int m_amound_vertex_min_max;

    List<Cell> map_cells;

    int m_amound_tiels;

    public Map_abstracter(Vector2Int m_size_cell_array_, Vector3Int m_size_cell_, Vector2Int m_amound_vertex_min_max_)
    {
        //Ini Variables
        m_size_cell_array = m_size_cell_array_;
        m_size_cell = m_size_cell_;
        m_amound_vertex_min_max = m_amound_vertex_min_max_;

        m_amound_tiels = m_size_cell_array.x * m_size_cell_array.y * m_size_cell.x * m_size_cell.y;

        Toolbeld tools = new Toolbeld();

        //Working Variables
        List<Vertex_info> room_map = new List<Vertex_info>();
        map_cells = new List<Cell>();

        Vector3Int currend_origon;
        Vector3Int currend_size_cell = m_size_cell;
        Vector3Int currend_start;
        Vector3Int currend_end;

        Vector2Int currend_amound_vertex_min_max = m_amound_vertex_min_max; //option for fariation?

        // --KILL-- List<Vertex_info> nav_mesh = new List<Vertex_info>();

        //generating cells
        //if Z is an option add for loop  
        for (int i = 0; i < m_size_cell_array.y; i++)//y
        {
            for (int j = 0; j < m_size_cell_array.x; j++)//x
            {
                //define currend_origon
                //memo at me Mathf.RoundToInt is doof... by .5 it gives back the even number
                currend_origon = new Vector3Int(m_size_cell.x * j, m_size_cell.y * i, m_size_cell.z);

                //define start and define end
               
                Vector3Int nord = new Vector3Int(   currend_origon.x + Mathf.RoundToInt(m_size_cell.x / 2),
                                                    currend_origon.y,
                                                    0);

                Vector3Int east = new Vector3Int(   currend_origon.x + m_size_cell.x,
                                                    currend_origon.y + Mathf.RoundToInt(m_size_cell.y / 2),
                                                    0);

                Vector3Int south = new Vector3Int(  currend_origon.x + Mathf.RoundToInt(m_size_cell.x / 2),
                                                    currend_origon.y + m_size_cell.y,
                                                    0);

                Vector3Int west = new Vector3Int(   currend_origon.x,
                                                    currend_origon.y + Mathf.RoundToInt(m_size_cell.y / 2),
                                                    0);

                //start uopper left corner
                if (j == 0 && i == 0)
                {
                    currend_start = new Vector3Int(0, 0, 0);

                    currend_end = east;
                }
                //all in the "middle"
                else if (j < m_size_cell_array.x - 1 && j > 0)
                {
                    currend_start = west;

                    currend_end = east;
                }
                //all on the right and even
                else if (j == m_size_cell_array.x - 1 && i % 2 == 0)
                {
                    currend_start = west;

                    currend_end = south;
                }
                // all on the right and odd 
                else if (j == m_size_cell_array.x - 1 && i % 2 != 0)
                {
                    currend_start = nord;

                    currend_end = west;
                }
                //all on the left and even
                else if (j == 0 && i % 2 == 0)
                {
                    currend_start = nord;

                    currend_end = east;
                }
                // all on the left and odd 
                else if (j == 0 && i % 2 != 0)
                {
                    currend_start = east;

                    currend_end = south;
                }
                // 0 0 if fail
                else
                {
                    currend_start = new Vector3Int(0, 0, 0);

                    currend_end = new Vector3Int(0, 0, 0);
                }
                //fill the Cell map
                map_cells.Add(new Cell(currend_origon, currend_size_cell, currend_start, currend_end, currend_amound_vertex_min_max));
            }

        }
    }

    //--Getters--
    public Vector2Int amound_vertex_min_maxsetter_getter()
    {
        return m_amound_vertex_min_max;
    }
    public Vector2Int size_cell_array_getter()
    {
        return m_size_cell_array;
    }
    public Vector3Int size_cell_getter()
    {
        return m_size_cell;
    }
    public List<Cell> map_cells_getter()
    {
        return map_cells;
    }

    //--Setters--
    public void amound_vertex_min_maxsetter_setter(Vector2Int m_amound_vertex_min_max_)
    {
        m_amound_vertex_min_max = m_amound_vertex_min_max_;
    }
    public void size_cell_array_setter(Vector2Int size_cell_array_)
    {
        m_size_cell_array = size_cell_array_;
    }
    public void size_cell_setter(Vector3Int m_size_cell_)
    {
        m_size_cell = m_size_cell_;
    }
}

class Map_builder
{
    string m_flor_symbol = "+";
    Vector2Int m_amound_vertex_min_max;
    Vector2Int m_size_cell_array;
    Vector3Int m_size_cell;

    List<Cell> m_cell_map;
    List<string> m_string_map;



    public Map_builder(Vector2Int m_size_cell_array_, Vector3Int m_size_cell_, Vector2Int m_amound_vertex_min_max_)
    {
        //Ini Variables
        m_size_cell_array = m_size_cell_array_;
        m_size_cell = m_size_cell_;
        m_amound_vertex_min_max = m_amound_vertex_min_max_;

        Map_abstracter abst_map = new Map_abstracter(m_size_cell_array, m_size_cell, m_amound_vertex_min_max);
        m_cell_map = abst_map.map_cells_getter();

        //build ini map
        m_string_map = build_string_map();



    }
    //--Getters--
    public Vector2Int amound_vertex_min_maxsetter_getter()
    {
        return m_amound_vertex_min_max;
    }
    public Vector2Int size_cell_array_getter()
    {
        return m_size_cell_array;
    }
    public Vector3Int size_cell_getter()
    {
        return m_size_cell;
    }
    public List<string> string_map_getter()
    {
        return m_string_map;
    }

    //--Setters--
    public void amound_vertex_min_maxsetter_setter(Vector2Int m_amound_vertex_min_max_)
    {
        m_amound_vertex_min_max = m_amound_vertex_min_max_;
    }
    public void size_cell_array_setter(Vector2Int size_cell_array_)
    {
        m_size_cell_array = size_cell_array_;
    }
    public void size_cell_setter(Vector3Int m_size_cell_)
    {
        m_size_cell = m_size_cell_;
    }

    //additional Funcions
    public List<string> build_string_map() //memo at me: we need a better way to save maps...
    {
        List<string> currend_string_map = new List<string>();
        //build a blank map
        for (int y = 0; y < m_size_cell_array.y * m_size_cell.y; y++)
        {
            for (int x = 0; x < m_size_cell_array.x * m_size_cell.x; x++)
            {
                currend_string_map.Add(" ");
            }

        }

        //fill map whit ways
        for (int array_y = 0; array_y < m_size_cell_array.y; array_y++)
        {
            for (int array_x = 0; array_x < m_size_cell_array.x; array_x++)
            {
                Cell currend_cell = m_cell_map[array_x + array_y * m_size_cell_array.x];
                int amound_of_points = currend_cell.vertex_cell_getter().Count;
                Vertex_info last_vertex = new Vertex_info(new Vector3Int(-1, -1, -1), false, false);

                for (int i = 0; i < amound_of_points; i++)
                {
                    Vertex_info currend_vertex = currend_cell.vertex_cell_getter()[i];
                    Vector3Int currend_position = currend_vertex.m_position;

                    for (int j = 0; j < currend_vertex.m_neighbors.Count; j++)
                    {
                        Vector3Int next_position = currend_vertex.m_neighbors[j].m_position;

                        if (next_position != last_vertex.m_position)
                        {
                            //filling the way left and rigth
                            while (currend_position.x != next_position.x)
                            {
                                currend_string_map[currend_position.x + currend_position.y * m_size_cell.x * m_size_cell_array.x] = m_flor_symbol;
                                //it go´s left
                                if ((next_position.x - currend_position.x) < 0)
                                {
                                    currend_position.x -= 1;
                                }
                                //it go´s rigth
                                else if ((next_position.x - currend_position.x) > 0)
                                {
                                    currend_position.x += 1;
                                }
                            }
                            //filling the way up and down
                            while (currend_position.y != next_position.y)
                            {
                                currend_string_map[currend_position.x + currend_position.y * m_size_cell.x * m_size_cell_array.x] = m_flor_symbol;
                                //it go´s down
                                if ((next_position.y - currend_position.y) < 0)
                                {
                                    currend_position.y -= 1;
                                }
                                //it go´s up 
                                else if ((next_position.y - currend_position.y) > 0)
                                {
                                    currend_position.y += 1;
                                }
                            }

                            last_vertex = currend_vertex;
                        }

                    
                    }
                }
            }

        }
        return currend_string_map;
    }
}



public class LevelLayoutGenerator : LevelLayoutInitializer {

    // NOTE: Level coordinate (x = 0, y = 0) is the bottom left corner of the map.

    public Vector2Int m_size_cell_array_ = new Vector2Int(2, 2);
    public Vector3Int m_size_cell_ = new Vector3Int(10, 10, 0);
    public Vector2Int m_amound_vertex_min_max_ = new Vector2Int(3, 6);
    List<string> currend_map;

    public override LevelLayout ProvideLevelLayout()
    {
        /*
        // BEGIN TEST - create simple level
        string layoutString =
            " + " +
            "+++" +
            "+ +";
        int levelWidth = 3;
        LevelLayout result = new LevelLayout(layoutString, levelWidth);
        // END TEST - create simple level
        */

        Map_builder builder = new Map_builder(m_size_cell_array_, m_size_cell_, m_amound_vertex_min_max_);
        currend_map = builder.string_map_getter();
        string layoutString = "";
        for (int i = 0; i < currend_map.Count; i++)
        {
            layoutString += currend_map[i];
        }

        LevelLayout result = new LevelLayout(layoutString, m_size_cell_array_.x * m_size_cell_.x);

        return result;
    }
}
