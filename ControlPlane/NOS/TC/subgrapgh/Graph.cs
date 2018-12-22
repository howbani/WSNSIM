
using MiniSDN.Dataplane;
using MiniSDN.Dataplane.PacketRouter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSDN.ControlPlane.NOS.TC.subgrapgh
{
    public class Graph
    {
        /// <summary>
        /// convert from Sensor To Vertex
        /// </summary>
        /// <param name="NetWork"></param>
        /// <returns></returns>
        public static List<Vertex> ConvertNodeToVertex(List<Sensor> NetWork)
        {
            // ini 
            List<Vertex> graph = new List<Vertex>(NetWork.Count);
            for (int i = 0; i < NetWork.Count; i++)
            {
                Vertex parVer = new Vertex(); // create new ver.
                parVer.ID = -1; // ini val.
                graph.Add(parVer);
            }
            // build:
            foreach (Sensor parSen in NetWork)
            {
                if (graph[parSen.ID].ID == -1)
                {
                    Vertex parVer = graph[parSen.ID];
                    parVer.ID = parSen.ID;
                    parVer.H_value = parSen.HopsToSink;
                    foreach (NeighborsTableEntry childSen in parSen.NeighborsTable)
                    {
                        if (graph[childSen.ID].ID == -1)
                        {

                            Vertex childVer = graph[childSen.ID];
                            childVer.ID = childSen.ID;
                            childVer.H_value = childSen.H;
                            graph[parSen.ID].Adjacent.Add(graph[childSen.ID]);

                            if (graph[childSen.ID].H_value < graph[parSen.ID].H_value)
                            {
                                graph[parSen.ID].Candidates.Add(graph[childSen.ID]);
                            }
                        }
                        else
                        {
                            graph[parSen.ID].Adjacent.Add(graph[childSen.ID]);
                            if (graph[childSen.ID].H_value < graph[parSen.ID].H_value)
                            {
                                graph[parSen.ID].Candidates.Add(graph[childSen.ID]);
                            }
                        }
                    }
                }
                else
                {
                    foreach (NeighborsTableEntry childSen in parSen.NeighborsTable)
                    {
                        if (graph[childSen.ID].ID == -1)
                        {

                            Vertex childVer = graph[childSen.ID];
                            childVer.ID = childSen.ID;
                            childVer.H_value = childSen.H;
                            graph[parSen.ID].Adjacent.Add(graph[childSen.ID]);

                            if (graph[childSen.ID].H_value < graph[parSen.ID].H_value)
                            {
                                graph[parSen.ID].Candidates.Add(graph[childSen.ID]);
                            }
                        }
                        else
                        {
                            graph[parSen.ID].Adjacent.Add(graph[childSen.ID]);

                            if (graph[childSen.ID].H_value < graph[parSen.ID].H_value)
                            {
                                graph[parSen.ID].Candidates.Add(graph[childSen.ID]);
                            }
                        }
                    }
                }
            }
            return graph;
        }
    }
}
