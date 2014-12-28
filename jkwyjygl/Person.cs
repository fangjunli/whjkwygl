using System;
using System.ComponentModel;

namespace jkwyjygl {
 
    class levellist
    {
        private string LevelID;
        private string LevelNo;


        public levellist(string lid,string lno) 
        {
            LevelID = lid;
            LevelNo = lno;
        }

        public string levelid
        {
            get { return LevelID; }
            set { LevelID = value; }
        }

        public string levelno
        {
            get { return LevelNo; }
            set { LevelNo = value; }
        }

        public static string findIDbyNo(string no,BindingList<levellist> bl)
        {
            for (int ii = 0; ii < bl.Count; ii++)
            {
                if (bl[ii].levelno == no) return bl[ii].levelid;
            }

            return "";
        }
  
    }
}