using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEditor;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Assets.Scripts.SUMOImporter.NetFileComponents;
using Assets.Scripts;
using UnityEngine;
using System.Linq;
using EasyRoads3Dv3;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class SumoNetLoader 
{
    static GameObject network;

    public static Dictionary<string, NetFileJunction> junctions;
    public static Dictionary<string, NetFileLane> lanes;
    public static Dictionary<string, NetFileEdge> edges;
    public static Dictionary<string, Shape> shapes;

    public static List<Vector3[]> polygons;

    static string sumoFilesPath;
    static string sumoBinPath;

    static float xmin;
    static float xmax;
    static float ymin;
    static float ymax;

    static Terrain Terrain;
    static TerrainData terrainData;

    static float numPlants;

    static float scaleLength = 2;
    static float scaleWidth = 2;

    static float meshScaleX = 3.3f;
    static float uvScaleV = 5;
    static float uvScaleU = 1;

    static float junctionHeight = 0.01f;
    static float trafficLightDistance = 2f;
    static float minLengthForStreetLamp = 12;
    static float streeLampDistance = 6f;

    static Boolean grassEnabled = true;
    static Boolean treesEnabled = true;

    static string[] plants = { "tree01", "tree02", "tree03", "tree04", "bush01", "bush02", "bush03", "bush04", "bush05", "bush06" };
   
    public static void parseXMLfiles(string sumoPath)
    {
        sumoFilesPath = sumoPath;
        lanes = new Dictionary<string, NetFileLane>();
        edges = new Dictionary<string, NetFileEdge>();
        junctions = new Dictionary<string, NetFileJunction>();
        shapes = new Dictionary<string, Shape>();

        network = new GameObject("StreetNetwork");

        parseJunctions(sumoFilesPath);
        parseEdges(sumoFilesPath);
    }

    public static void parseJunctions(string sumoFilesPath)
    {
        //network = new GameObject("StreetNetwork");

        string netFilePath = sumoFilesPath + "/osm.net.xml";
        string shapesFilePath = sumoFilesPath + "/osm.poly.xml";

        XmlReaderSettings xmlReaderSetting = new XmlReaderSettings();
        xmlReaderSetting.IgnoreComments = true;
        xmlReaderSetting.IgnoreWhitespace = true;
        using (XmlReader reader = XmlReader.Create(netFilePath, xmlReaderSetting))
        {
            //reader.ReadToFollowing("tlLogic");
            reader.ReadToFollowing("junction");
            //while (reader.Read() && !reader.EOF)
            do
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToString())
                    {
                        case "junction":
                            string junctionId = "null";
                            if (reader.HasAttributes)
                            {
                                if (reader.GetAttribute("type") != "internal")
                                {
                                    try
                                    {
                                        float z = float.Parse(reader.GetAttribute("z"));
                                    }
                                    catch (ArgumentNullException)
                                    {
                                        UnityEngine.Debug.LogWarning("not found error: junction attribute - z ");
                                    }
                                    finally
                                    {
                                        float z = 0f;
                                        junctionId = reader.GetAttribute("id");
                                        junctionTypeType type = (junctionTypeType)Enum.Parse(typeof(junctionTypeType), reader.GetAttribute("type"));
                                        float x = float.Parse(reader.GetAttribute("x"));
                                        float y = float.Parse(reader.GetAttribute("y"));
                                        string incLanes = reader.GetAttribute("incLanes");
                                        string junctionShape = reader.GetAttribute("shape");

                                        NetFileJunction junction = new NetFileJunction(junctionId, type, x, y, z, incLanes, junctionShape);

                                        if (!junctions.ContainsKey(junction.id))
                                        {
                                            junctions.Add(junction.id, junction);
                                        }
                                    }

                                }
                            }
                            break;

                        default:
                            reader.Skip();
                            break;
                    } // end of switch statement
                } // end of if statement
            } while (reader.Read() && !reader.EOF); // end of while statement
        }

    } // end of method

    public static void parseEdges(string sumoFilesPath)
    {
        string netFilePath = sumoFilesPath + "/osm.net.xml";
        string shapesFilePath = sumoFilesPath + "/osm.poly.xml";

        XmlReaderSettings xmlReaderSetting = new XmlReaderSettings();
        xmlReaderSetting.IgnoreComments = true;
        xmlReaderSetting.IgnoreWhitespace = true;
        using (XmlReader reader = XmlReader.Create(netFilePath, xmlReaderSetting))
        {
            reader.ReadToFollowing("net");
            while (reader.Read() && !reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToString())
                    {
                        case "location":
                            if (reader.HasAttributes)
                            {
                                string boundary = reader.GetAttribute("convBoundary");
                                string[] boundaries = boundary.Split(',');
                                xmin = float.Parse(boundaries[0]);
                                ymin = float.Parse(boundaries[1]);
                                xmax = float.Parse(boundaries[2]);
                                ymax = float.Parse(boundaries[3]);
                                UnityEngine.Debug.Log("xmin: " + xmin + " ymin: " + ymin + " xmax: " + xmax + " ymax: " + ymax);
                            }
                            break;
                        case "edge":
                            string edgeId = "null";
                            string laneId = "null";
                            if (reader.HasAttributes)
                            {
                                if (reader.GetAttribute("function") != "internal")
                                {
                                    try
                                    {
                                        string edgeShape = reader.GetAttribute("shape");
                                    }
                                    catch (KeyNotFoundException)
                                    {
                                        UnityEngine.Debug.LogWarning("not found error: edge attribute - shape ");
                                    }
                                    finally
                                    {
                                        string edgeShape = "null";
                                        edgeId = reader.GetAttribute("id");
                                        string from = reader.GetAttribute("from");
                                        string to = reader.GetAttribute("to");
                                        string priority = reader.GetAttribute("priority");

                                        NetFileEdge edge = new NetFileEdge(edgeId, from, to, priority, edgeShape);
                                        if (!edges.ContainsKey(edgeId))
                                        {
                                            edges.Add(edgeId, edge);
                                        }

                                        XmlReader innerReader = reader.ReadSubtree();
                                        while (innerReader.Read())
                                        {
                                            switch (innerReader.Name.ToString())
                                            {
                                                case "lane":
                                                    if (innerReader.HasAttributes)
                                                    {
                                                        laneId = innerReader.GetAttribute("id");
                                                        string index = innerReader.GetAttribute("index");
                                                        float speed = float.Parse(innerReader.GetAttribute("speed"));
                                                        float length = float.Parse(innerReader.GetAttribute("length"));
                                                        string laneShape = innerReader.GetAttribute("shape");
                                                        edges[edgeId].addLane(laneId, index, speed, length, laneShape);
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        //UnityEngine.Debug.Log("edge : " + edgeId + " lane: " + laneId);
                                        }
                                        innerReader.Close();
                                    }
                                }
                            }
                            break;
                        case "junction":
                            reader.Skip();
                            break;
                        default:
                            reader.Skip();
                            break;
                    } // end of switch statement
                } // end of if statement
            } // end of while statement
        }
    } // end of method

    public static void drawStreetNetwork()
    {
        polygons = new List<Vector3[]>();

        bool linearOption = true;

        int laneCounter = 0;
        int streetLightCounter = 0;


        // (1) Draw all Edges ------------------------------------
        //MonoBehaviour.print("Inserting 3d Streets");
        UnityEngine.Debug.Log("Inserting 3d Streets");

        foreach (NetFileEdge e in edges.Values)
        {
            int edgeCounter = 0;
            GameObject spline = new GameObject("StreetSegment_" + laneCounter++);
            spline.transform.SetParent(network.transform);

            Spline splineObject = spline.AddComponent<Spline>();

            if (linearOption)
                splineObject.interpolationMode = Spline.InterpolationMode.Linear;
            else
                splineObject.interpolationMode = Spline.InterpolationMode.BSpline;

            foreach (NetFileLane l in e.getLanes())
            {
                foreach (double[] coordPair in l.shape)
                {
                    // Add Node
                    GameObject splineNode = new GameObject("Node_" + edgeCounter++);
                    splineNode.transform.SetParent(spline.transform);
                    SplineNode splineNodeObject = splineNode.AddComponent<SplineNode>();
                    splineNode.transform.position = new Vector3((float)coordPair[0]- xmin, 0, (float)coordPair[1]-ymin);
                    splineObject.splineNodesArray.Add(splineNodeObject);
                }

                // Add meshes
                Material material = AssetDatabase.LoadAssetAtPath<Material>(PathConstants.pathRoadMaterial);
                // --------- 20.10.13 ---------//
                material.shader = Shader.Find("Standard");
                material.SetFloat("_Glossiness", 0f);
                /////////////////////////////////
                MeshRenderer mRenderer = mRenderer = spline.GetComponent<MeshRenderer>();
                if (mRenderer == null)
                {
                    mRenderer = spline.AddComponent<MeshRenderer>();
                }
                mRenderer.material = material;
                // --------- 20.10.13 ---------//
                mRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mRenderer.receiveShadows = false;
                /////////////////////////////////


                SplineMesh sMesh = spline.AddComponent<SplineMesh>();
                sMesh.spline = splineObject;
                sMesh.baseMesh = AssetDatabase.LoadAssetAtPath<Mesh>(PathConstants.pathSuperSplinesBox);
                sMesh.startBaseMesh = AssetDatabase.LoadAssetAtPath<Mesh>(PathConstants.pathSuperSplinesBox);
                sMesh.endBaseMesh = AssetDatabase.LoadAssetAtPath<Mesh>(PathConstants.pathSuperSplinesBox);
                sMesh.uvScale = new Vector2(uvScaleU, uvScaleV);
                sMesh.xyScale = new Vector2(meshScaleX, 0);
                //sMesh.segmentCount = 500;
                //sMesh.segmentCount = 500 * 4;
                sMesh.segmentCount = 50;


                // (1.1) Add Lanes to polygon list for tree placement check
                for (int i = 0; i < l.shape.Count - 1; i++)
                {
                    double length = Math.Sqrt(Math.Pow(l.shape[i][0]-xmin - (l.shape[i + 1][0]-xmin), 2) + Math.Pow(l.shape[i][1]-ymin - (l.shape[i + 1][1]-ymin), 2));
                    // Calc the position (in line with the lane)
                    float x1 = (float)l.shape[i][0]-xmin;
                    float y1 = (float)l.shape[i][1]-ymin;
                    float x2 = (float)l.shape[i + 1][0] - xmin;
                    float y2 = (float)l.shape[i + 1][1] - ymin;
                    double Dx = x2 - x1;
                    double Dy = y2 - y1;
                    double D = Math.Sqrt(Dx * Dx + Dy * Dy);
                    double W = 10;
                    Dx = 0.5 * W * Dx / D;
                    Dy = 0.5 * W * Dy / D;
                    Vector3[] polygon = new Vector3[] { new Vector3((float)(x1 - Dy), 0, (float)(y1 + Dx)),
                                                    new Vector3((float)(x1 + Dy), 0, (float)(y1 - Dx)),
                                                    new Vector3((float)(x2 + Dy), 0, (float)(y2 - Dx)),
                                                    new Vector3((float)(x2 - Dy), 0, (float)(y2 + Dx)) };
                    polygons.Add(polygon);


                    // (2) Add Street Lamps (only if long enough)
                    if (length >= minLengthForStreetLamp)
                    {
                        float angle = Mathf.Atan2(y2 - y1, x2 - x1) * 180 / Mathf.PI;

                        // Allway located at the middle of a street
                        double ratioRotPoint = 0.5;
                        double ratio = 0.5 + streeLampDistance / length;

                        float xDest = (float)((1 - ratio) * x1 + ratio * x2);
                        float yDest = (float)((1 - ratio) * y1 + ratio * y2);

                        float xRotDest = (float)((1 - ratioRotPoint) * x1 + ratioRotPoint * x2);
                        float yRotDest = (float)((1 - ratioRotPoint) * y1 + ratioRotPoint * y2);

#if streetLamp
                        GameObject streetLampPrefab = AssetDatabase.LoadMainAssetAtPath(PathConstants.pathLaterne) as GameObject;
                        GameObject streetLamp = GameObject.Instantiate(streetLampPrefab, new Vector3(xDest, 0, yDest), Quaternion.Euler(new Vector3(0, 0, 0)));
                        streetLamp.name = "StreetLight_" + streetLightCounter++;
                        streetLamp.transform.SetParent(network.transform);
                        streetLamp.transform.RotateAround(new Vector3(xRotDest, 0, yRotDest), Vector3.up, -90.0f);
                        streetLamp.transform.Rotate(Vector3.up, -angle);
#endif
                    }
                }

            }


        }

        // (3) Draw all Junction areas ------------------------------------
        MonoBehaviour.print("Inserting 3d Junctions");

        int junctionCounter = 0;
        foreach (NetFileJunction j in junctions.Values)
        {
            List<int> indices = new List<int>();

            Vector2[] vertices2D = new Vector2[j.shape.Count];
            for (int i = 0; i < j.shape.Count; i++)
            {
                vertices2D[i] = new Vector3((float)(j.shape[i])[0] - xmin, (float)(j.shape[i])[1] - ymin);
            }

            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(vertices2D);
            List<int> bottomIndices = new List<int>(tr.Triangulate());
            indices.AddRange(bottomIndices);


            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[vertices2D.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
            }

            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Bounds bounds = mesh.bounds;
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x / bounds.size.x, vertices[i].z / bounds.size.z);
            }
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Set up game object with mesh;
            GameObject junction3D = new GameObject("junction_" + junctionCounter++);
            //MeshRenderer r = (MeshRenderer)junction3D.AddComponent(typeof(MeshRenderer));
            //--- 2020 10 08 ---//
            MeshRenderer r = junction3D.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            // ----------------//
            //Material material = Resources.Load<Material>(PathConstants.pathJunctionMaterial);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(PathConstants.pathJunctionMaterial);
            r.material = material;
            //--- 2020 10 13 ---//
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;
            // ----------------//
            MeshFilter filter = junction3D.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = mesh;
            junction3D.transform.SetParent(network.transform);

            // (3.1) Add junctions to polygon list for tree placement check
            polygons.Add(vertices);
        }

        // (4) Draw Traffic Lights
#if trafficLight
        MonoBehaviour.print("Inserting 3d Traffic Lights");

        foreach (NetFileJunction j in junctions.Values)
        {
            if (j.type == junctionTypeType.traffic_light)
            {
                int index = 0;
                foreach (NetFileLane l in j.incLanes)
                {
                    // Calc the position (in line with the lane)
                    float x1 = (float)l.shape[0][0] - xmin;
                    float y1 = (float)l.shape[0][1] - ymin;
                    float x2 = (float)l.shape[1][0] - xmin;
                    float y2 = (float)l.shape[1][1] - ymin;
                    float length = (float)Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2));
                    float angle = Mathf.Atan2(y2 - y1, x2 - x1) * 180 / Mathf.PI;

                    double ratio = (length - trafficLightDistance) / length;

                    float xDest = (float)((1 - ratio) * x1 + ratio * x2);
                    float yDest = (float)((1 - ratio) * y1 + ratio * y2);

                    // Insert the 3d object, rotate from lane 90° to the right side and then orientate the traffic light towards the vehicles
                    //--- 2020 10 08 ---//
                    //GameObject trafficLightPrefab = AssetDatabase.LoadMainAssetAtPath(PathConstants.pathTrafficLight) as GameObject;
                    GameObject trafficLightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PathConstants.pathTrafficLight);
                    // ----------------//
                    GameObject trafficLight = GameObject.Instantiate(trafficLightPrefab, new Vector3(xDest, 0, yDest), Quaternion.Euler(new Vector3(0, 0, 0)));
                    trafficLight.name = "TrafficLight_" + j.id;
                    trafficLight.transform.SetParent(network.transform);
                    trafficLight.transform.RotateAround(new Vector3(x2, 0, y2), Vector3.up, -90.0f);
                    trafficLight.transform.Rotate(Vector3.up, -angle);

                    // Insert traffic light index as empty GameObject into traffic light
                    GameObject TLindex = new GameObject("index");
                    GameObject TLindexVal = new GameObject(Convert.ToString(index++));
                    TLindexVal.transform.SetParent(TLindex.transform);
                    TLindex.transform.SetParent(trafficLight.transform);
                }
            }
        }
#endif
    }
}
