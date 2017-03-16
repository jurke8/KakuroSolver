using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KakuroHelper
{
    [Serializable]
    public class Cell
    {
        public int VerticalSum { get; set; }
        public int HorizontalSum { get; set; }
        public int VerticalLength { get; set; }
        public int HorizontalLength { get; set; }
        public int VirtualVerticalSum { get; set; }
        public int VirtualHorizontalSum { get; set; }
        public int VirtualVerticalLength { get; set; }
        public int VirtualHorizontalLength { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public List<int> PossibleHorizontalNumbers { get; set; }
        public List<int> PossibleVerticalNumbers { get; set; }
        public List<List<int>> AllHorizontalCombinations { get; set; }
        public List<List<int>> AllVerticalCombinations { get; set; }
        public List<int> CertainNumbersInRow { get; set; }
        public List<int> CertainNumbersInColumn { get; set; }
        public List<int> AllPossibleNumbers { get; set; }
        public List<int> PossibleNumbers { get; set; }
        public int Value { get; set; }
        public bool Border { get; set; }
        public bool Locked { get; set; }
        public bool Iterated { get; set; }
        public Cell()
        {
            VerticalSum = -1;
            HorizontalSum = -1;
            VirtualVerticalSum = -1;
            VirtualHorizontalSum = -1;
            PossibleHorizontalNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            PossibleVerticalNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            PossibleNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            AllPossibleNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Value = -1;
            CertainNumbersInRow = new List<int>();
            CertainNumbersInColumn = new List<int>();
        }
    }
}
