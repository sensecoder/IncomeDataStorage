using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncomeDataStorage
{
    public class JournalViewParam
    {
        private bool reverseSort = false;
        public bool ReverseSort
        {
            get
            {
                return reverseSort;
            }
        }

        private Sorting sortedBy;
        public Sorting SortedBy
        {
            get
            {
                return sortedBy;
            }
            set
            {
                reverseSort = !reverseSort;
                sortedBy = value;
            }
        }

        public DateFilter FilterBy { get; set; }
    }
}
