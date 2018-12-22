using MiniSDN.Dataplane;
using MiniSDN.Dataplane.NOS;
using System.Collections.Generic;

namespace MiniSDN.ControlPlane.NOS.Visualizating
{
    public class DistenctPath
    {
        public int SourceID { get; set; }
        public int ID { get; set; }
        public string PathString { get; set; }
        public int Hops { get; set; }
        public double Multiplicity { get; set; } //  //the number of times an element belongs to the multiset is the multiplicity of that member.
        public double SumProb { set; get; } // the sum of all paths. ( the path has diffrent prob per time)
        public double AverageProb { get { return SumProb / Multiplicity; } } // for path.

    }

    public class ClassfyPathsPerSource
    {
        public int SourceID { get; set; }
        public List<DistenctPath> DistinctPathsForThisSource = new List<DistenctPath>();
    } 

    public  class DisPathConter
    {
        private DistenctPath isFound(string path, List<DistenctPath> disPathsList)
        {
            foreach (DistenctPath dispath in disPathsList)
            {
                if (path == dispath.PathString) { return dispath; }
            }
            return null;
        }

        public ClassfyPathsPerSource FindbySource( int sid, List<ClassfyPathsPerSource> clssess)
        {
            foreach(ClassfyPathsPerSource cls in clssess) { if (cls.SourceID == sid) return cls;  }
            return null;
        }


        /// <summary>
        /// classy the distnt paths per source.
        /// </summary>
        /// <returns></returns>
        public List<ClassfyPathsPerSource> ClassyfyDistinctPathsPerSources()
        {
            List<ClassfyPathsPerSource> clssess = new List<ClassfyPathsPerSource>();
            List<DistenctPath> disPaths = FindTheDistinctPaths();
            foreach(DistenctPath p in disPaths)
            {
                ClassfyPathsPerSource oldclass = FindbySource(p.SourceID, clssess);
                if(oldclass==null)
                {
                    ClassfyPathsPerSource newclass = new ClassfyPathsPerSource();
                    newclass.SourceID = p.SourceID;
                    newclass.DistinctPathsForThisSource.Add(p);
                    clssess.Add(newclass);
                }
                else
                {
                    oldclass.DistinctPathsForThisSource.Add(p);
                }
            }
            return clssess;
        }

        /// <summary>
        /// find the paths.
        /// </summary>
        /// <returns></returns>
        public List<DistenctPath> FindTheDistinctPaths()
        {
            List<DistenctPath> disPathsList = new List<DistenctPath>();
            Sensor sink = PublicParamerters.SinkNode;
            if (sink != null)
            {
                foreach (Packet pl1 in PublicParamerters.FinishedRoutedPackets)
                {
                    DistenctPath oldRecord = isFound(pl1.Path, disPathsList);
                    if (oldRecord == null)
                    {
                        DistenctPath newPath = new DistenctPath();
                        newPath.SourceID = pl1.Source.ID;
                        newPath.PathString = pl1.Path;
                      //  newPath.SumProb = pl1.PathLinksQualityEstimator;
                        newPath.ID = disPathsList.Count + 1;
                        newPath.Hops = pl1.Hops;
                        newPath.Multiplicity = 1;
                        disPathsList.Add(newPath);
                    }
                    else
                    {
                        oldRecord.Multiplicity += 1;
                      //  oldRecord.SumProb += pl1.PathLinksQualityEstimator;
                    }
                }
            }
            return disPathsList;
        }
    }
}
