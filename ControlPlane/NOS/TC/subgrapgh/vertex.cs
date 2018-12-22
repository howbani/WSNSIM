using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSDN.ControlPlane.NOS.TC.subgrapgh
{
    public class Vertex
    {
        public int ID { get; set; }
        public int H_value { get; set; }
        public List<Vertex> Adjacent = new List<Vertex>();
        public string AdjacentString
        {
            get
            {
                string str = ID + "<";
                foreach(Vertex ver in Adjacent)
                {
                    str += ver.ID + ",";
                }
                string sub = str.Substring(0, str.Length - 1);
                sub += ">";
                return sub;
            }
        }

        public List<Vertex> Candidates = new List<Vertex>();
        public string CandidatesString 
        {
            get
            {
                string str = ID + "<";
                foreach (Vertex ver in Candidates)
                {
                    str += ver.ID + ",";
                }
                string sub = str.Substring(0, str.Length - 1);
                sub += ">";
                return sub;
            }
        }
    }
}
