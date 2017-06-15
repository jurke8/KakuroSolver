using KakuroSolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KakuroSolver.Helpers
{
    public class Algorithm
    {
        public static int nRowsGlobal, nColsGlobal;
        public List<List<Cell>> GetResult(List<PictureCell> cells, int nRows, int nCols)
        {
            nRowsGlobal = nRows;
            nColsGlobal = nCols;
            List<List<Cell>> Sums = CreateCellList();
            Sums = ReadFromBoard(Sums, cells);
            //if (Sums[0][0].Value == -2)
            //    return Sums; //ako zbroj vertikalnih i horizontalnih suma nije jednak


            FindAllPossibleNumbers(Sums);       //FirstIteration
            int i = 0;
            while (i < 15)
            {
                Iterate(Sums);
                CheckMaxMinSums(Sums);
                Iterate(Sums);
                FindPossibleNumbers(Sums);
                ClearPossibleNumbers(Sums);
                i++;                            //broj iteracija
                if (IsBoardValid(Sums))
                {
                    //WriteToBoard(Sums);         //ispis na ploču kad riješi sve
                    return Sums;
                }
            }
            //WriteToBoard(Sums);
            return Sums;
        }

        private List<List<Cell>> CreateCellList()
        {
            List<List<Cell>> cellList = new List<List<Cell>>();
            for (int i = 0; i < nRowsGlobal; i++)
            {
                var nestedList = new List<Cell>();

                for (int j = 0; j < nColsGlobal; j++)
                {
                    var nestedCell = new Cell { RowNumber = i, ColumnNumber = j };
                    nestedList.Add(nestedCell);
                }
                cellList.Add(nestedList);
            }
            return cellList;
        }

        private List<List<Cell>> ReadFromBoard(List<List<Cell>> cellList, List<PictureCell> cells)
        {
            int totalVerticalSum = 0;
            int totalhorizontalSum = 0;
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {
                    if (cells.ElementAt(rowNumber * nColsGlobal + columnNumber).IsBorder)
                    {
                        cellList[rowNumber][columnNumber].Border =true;
                        cellList[rowNumber][columnNumber].HorizontalSum = Convert.ToInt32(cells.ElementAt(rowNumber * nColsGlobal + columnNumber).HorizontalSum);
                        cellList[rowNumber][columnNumber].VerticalSum = Convert.ToInt32(cells.ElementAt(rowNumber * nColsGlobal + columnNumber).VerticalSum);
                        cellList[rowNumber][columnNumber].PossibleNumbers.Clear();
                        cellList[rowNumber][columnNumber].AllPossibleNumbers.Clear();
                        cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Clear();
                        cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Clear();
                    }
                }
            }
            if (totalVerticalSum != totalhorizontalSum)
            {
                cellList[0][0].Value = -2;
                return cellList;
            }
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {
                    try
                    {
                        if (cellList[rowNumber][columnNumber].Border)
                        {
                            for (int i = columnNumber + 1; i < nColsGlobal; i++)                //set horizontal lengths of border
                            {
                                if (!cellList[rowNumber][i].Border)
                                {
                                    cellList[rowNumber][columnNumber].HorizontalLength++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int j = rowNumber + 1; j < nRowsGlobal; j++)
                            {
                                if (!cellList[j][columnNumber].Border)
                                {
                                    cellList[rowNumber][columnNumber].VerticalLength++;     //set vertical lengths of border
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (cellList[rowNumber][columnNumber].HorizontalLength > 9 || cellList[rowNumber][columnNumber].VerticalLength > 9)
                            {
                                cellList[0][0].Value = -2;
                                return cellList;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return cellList;
                    }
                }
            }
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {
                    try
                    {
                        if (cellList[rowNumber][columnNumber].Border)
                        {
                            for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                            {
                                //set horizontal sum and length of all elements in border row
                                cellList[rowNumber][i].HorizontalSum = cellList[rowNumber][i].VirtualHorizontalSum = cellList[rowNumber][columnNumber].HorizontalSum;
                                cellList[rowNumber][i].HorizontalLength = cellList[rowNumber][i].VirtualHorizontalLength = cellList[rowNumber][columnNumber].HorizontalLength;
                            }
                            for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                            {
                                //set vertical sum and length of all elements in border column
                                cellList[j][columnNumber].VerticalSum = cellList[j][columnNumber].VirtualVerticalSum = cellList[rowNumber][columnNumber].VerticalSum;
                                cellList[j][columnNumber].VerticalLength = cellList[j][columnNumber].VirtualVerticalLength = cellList[rowNumber][columnNumber].VerticalLength;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return cellList;
                    }
                }
            }
            return cellList;
        }
        private List<List<Cell>> Iterate(List<List<Cell>> cellList)
        {
            ListToValue(cellList);
            var restrictedHorizontalNumbers = new List<int>();
            var restrictedVerticalNumbers = new List<int>();
            //Remove certain (100%) value from other possible numbers in that row / column

            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {
                    try
                    {

                        if (!cellList[rowNumber][columnNumber].Border && !cellList[rowNumber][columnNumber].Locked)
                        {
                            cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Clear();
                            cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.AddRange(cellList[rowNumber][columnNumber].PossibleNumbers);
                            cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Clear();
                            cellList[rowNumber][columnNumber].PossibleVerticalNumbers.AddRange(cellList[rowNumber][columnNumber].PossibleNumbers);

                            for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                            {
                                if (!cellList[rowNumber][i].Locked)
                                {
                                    foreach (var number in cellList[rowNumber][i].PossibleNumbers)
                                    {
                                        if (!cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Contains(number))
                                        {
                                            cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Add(number);
                                        }
                                    }
                                }
                                else
                                {
                                    restrictedHorizontalNumbers.Add(cellList[rowNumber][i].Value);
                                }
                            }
                            for (int i = columnNumber - 1; i > 0 && !cellList[rowNumber][i].Border; i--)
                            {
                                if (!cellList[rowNumber][i].Locked)
                                {
                                    foreach (var number in cellList[rowNumber][i].PossibleNumbers)
                                    {
                                        if (!cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Contains(number))
                                        {
                                            cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Add(number);
                                        }
                                    }
                                }
                                else
                                {
                                    restrictedHorizontalNumbers.Add(cellList[rowNumber][i].Value);
                                }
                            }
                            for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                            {
                                if (!cellList[j][columnNumber].Locked)
                                {
                                    foreach (var number in cellList[j][columnNumber].PossibleNumbers)
                                    {
                                        if (!cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Contains(number))
                                        {
                                            cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Add(number);
                                        }
                                    }
                                }
                                else
                                {
                                    restrictedVerticalNumbers.Add(cellList[j][columnNumber].Value);
                                }

                            }
                            for (int j = rowNumber - 1; j > 0 && !cellList[j][columnNumber].Border; j--)
                            {
                                if (!cellList[j][columnNumber].Locked)
                                {
                                    foreach (var number in cellList[j][columnNumber].PossibleNumbers)
                                    {
                                        if (!cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Contains(number))
                                        {
                                            cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Add(number);
                                        }
                                    }
                                }
                                else
                                {
                                    restrictedVerticalNumbers.Add(cellList[j][columnNumber].Value);
                                }
                            }
                            foreach (var number in restrictedHorizontalNumbers)
                            {
                                if (cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Contains(number))
                                {
                                    cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Remove(number);
                                }
                            }
                            restrictedHorizontalNumbers.Clear();
                            foreach (var number in restrictedVerticalNumbers)
                            {
                                if (cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Contains(number))
                                {
                                    cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Remove(number);
                                }
                            }
                            restrictedVerticalNumbers.Clear();
                        }


                        if (!cellList[rowNumber][columnNumber].Border && cellList[rowNumber][columnNumber].Locked && !cellList[rowNumber][columnNumber].Iterated)
                        {
                            for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                            {
                                if (!cellList[rowNumber][i].Locked)
                                {
                                    cellList[rowNumber][i].VirtualHorizontalSum = cellList[rowNumber][i].VirtualHorizontalSum - cellList[rowNumber][columnNumber].Value;
                                    cellList[rowNumber][i].VirtualHorizontalLength--;
                                }
                            }
                            for (int i = columnNumber - 1; i > 0 && !cellList[rowNumber][i].Border; i--)
                            {
                                if (!cellList[rowNumber][i].Locked)
                                {
                                    cellList[rowNumber][i].VirtualHorizontalSum = cellList[rowNumber][i].VirtualHorizontalSum - cellList[rowNumber][columnNumber].Value;
                                    cellList[rowNumber][i].VirtualHorizontalLength--;
                                }
                            }
                            for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                            {
                                if (!cellList[j][columnNumber].Locked)
                                {
                                    cellList[j][columnNumber].VirtualVerticalSum = cellList[j][columnNumber].VirtualVerticalSum - cellList[rowNumber][columnNumber].Value;
                                    cellList[j][columnNumber].VirtualVerticalLength--;
                                }

                            }
                            for (int j = rowNumber - 1; j > 0 && !cellList[j][columnNumber].Border; j--)
                            {
                                if (!cellList[j][columnNumber].Locked)
                                {
                                    cellList[j][columnNumber].VirtualVerticalSum = cellList[j][columnNumber].VirtualVerticalSum - cellList[rowNumber][columnNumber].Value;
                                    cellList[j][columnNumber].VirtualVerticalLength--;
                                }

                            }
                            cellList[rowNumber][columnNumber].Iterated = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    ListToValue(cellList);
                }
            }
            return cellList;
        }
        private void ListToValue(List<List<Cell>> cellList)
        {

            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {

                    if (cellList[rowNumber][columnNumber].PossibleNumbers.Count == 1)
                    {
                        cellList[rowNumber][columnNumber].Value = cellList[rowNumber][columnNumber].PossibleNumbers[0];
                        cellList[rowNumber][columnNumber].Locked = true;
                        if (cellList[rowNumber][columnNumber].CertainNumbersInColumn.Contains(cellList[rowNumber][columnNumber].Value) || cellList[rowNumber][columnNumber].CertainNumbersInRow.Contains(cellList[rowNumber][columnNumber].Value))
                        {
                            RemoveFromCertainNumbers(cellList, rowNumber, columnNumber, cellList[rowNumber][columnNumber].Value);
                        }
                    }
                }
            }
        }
        private void ClearPossibleNumbers(List<List<Cell>> cellList)
        {
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {
                    {
                        if (cellList[rowNumber][columnNumber].Value != -1)
                        {
                            cellList[rowNumber][columnNumber].Locked = true;
                            cellList[rowNumber][columnNumber].PossibleNumbers.Clear();
                            cellList[rowNumber][columnNumber].PossibleNumbers.Add(cellList[rowNumber][columnNumber].Value);
                        }
                    }
                }
            }
        }
        private void WriteToBoard(List<List<Cell>> cellList)
        {
            //int rowNumber;
            //int columnNumber;
            //foreach (TextBox c in this.table.Controls)
            //{
            //    rowNumber = this.table.GetRow(c);
            //    columnNumber = this.table.GetColumn(c);
            //    if (!cellList[rowNumber][columnNumber].Border)
            //    {
            //        c.Text = "";
            //        if (cellList[rowNumber][columnNumber].Value != -1)
            //        {
            //            c.Text = cellList[rowNumber][columnNumber].Value.ToString();
            //            c.BackColor = Color.LightGreen;
            //            c.Font = new Font("Arial", 24);
            //            c.TextAlign = HorizontalAlignment.Center;
            //        }
            //        else
            //        {
            //            c.BackColor = Color.LightBlue;
            //            c.Font = new Font(Font.OriginalFontName, 9, FontStyle.Bold);
            //            c.TextAlign = HorizontalAlignment.Right;
            //            for (int i = 1; i < 10; i++)
            //            {
            //                if (cellList[rowNumber][columnNumber].PossibleNumbers.Contains(i))
            //                {
            //                    c.Text += i.ToString() + "  ";
            //                }
            //                else
            //                {
            //                    c.Text += "     ";
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        c.BackColor = Color.Gray;
            //    }
            //}
        }
        private bool IsBoardValid(List<List<Cell>> cellList)
        {
            List<int> values = new List<int>();
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {


                    if (!cellList[rowNumber][columnNumber].Border)
                    {
                        if (cellList[rowNumber][columnNumber].Value == 0 || cellList[rowNumber][columnNumber].Value == -1)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                        {
                            if (values.Contains(cellList[rowNumber][i].Value))
                            {
                                return false;
                            }
                            else
                            {
                                values.Add(cellList[rowNumber][i].Value);
                            }
                        }
                        values.Clear();
                        for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                        {
                            if (values.Contains(cellList[j][columnNumber].Value))
                            {
                                return false;
                            }
                            else
                            {
                                values.Add(cellList[j][columnNumber].Value);
                            }
                        }
                        values.Clear();
                    }
                }
            }
            return true;
        }

        public List<List<int>> GetAllCombinations(List<int> allPossibleNumbers, int arrayLength, int arraySum)
        {
            var allPossibleSolutions = new List<List<int>>();
            var possibleSolutions = new List<List<int>>();
            int setCounter = Convert.ToInt32(Math.Pow(2, allPossibleNumbers.Count()));
            for (int number = 1; number < setCounter; number++)
            {
                var nestedList = new List<int>();
                for (int j = 0; j < allPossibleNumbers.Count(); j++)
                {
                    var binaryNumber = 1 << j;
                    if ((number & binaryNumber) == binaryNumber) { nestedList.Add(allPossibleNumbers[j]); }
                }
                allPossibleSolutions.Add(nestedList);
            }
            foreach (List<int> possibleNumbers in allPossibleSolutions)
            {
                if (arrayLength == possibleNumbers.Count())
                {
                    int currentSum = 0;
                    foreach (int number in possibleNumbers)
                    {
                        currentSum += number;
                    }
                    if (currentSum == arraySum)
                    {
                        possibleSolutions.Add(possibleNumbers);
                    }
                }
            }
            return possibleSolutions;
        }
        private static List<int> FindNumbers(List<List<int>> allPossibleCombinations)
        {
            var possibleNumbers = new List<int>();
            foreach (var list in allPossibleCombinations)
            {
                foreach (var number in list)
                {
                    if (!possibleNumbers.Contains(number))
                    {
                        possibleNumbers.Add(number);
                    }
                }
            }
            possibleNumbers.Sort();
            return possibleNumbers;
        }
        private static List<int> FindSectionNumbers(List<int> VerticalNumbers, List<int> HorizontalNumbers)
        {
            var possibleNumbers = new List<int>();

            foreach (var number in VerticalNumbers)
            {
                if (!possibleNumbers.Contains(number) && HorizontalNumbers.Contains(number))
                {
                    possibleNumbers.Add(number);
                }
            }
            possibleNumbers.Sort();
            return possibleNumbers;
        }
        private void FindAllPossibleNumbers(List<List<Cell>> cellList)
        {
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {
                    try
                    {
                        if (!cellList[rowNumber][columnNumber].Locked && !cellList[rowNumber][columnNumber].Border)
                        {
                            if (cellList[rowNumber][columnNumber].AllPossibleNumbers.Count >= cellList[rowNumber][columnNumber].HorizontalLength)
                            {
                                cellList[rowNumber][columnNumber].PossibleHorizontalNumbers = FindNumbers(GetAllCombinations(cellList[rowNumber][columnNumber].AllPossibleNumbers, cellList[rowNumber][columnNumber].HorizontalLength, cellList[rowNumber][columnNumber].HorizontalSum));
                            }
                            if (cellList[rowNumber][columnNumber].AllPossibleNumbers.Count >= cellList[rowNumber][columnNumber].VerticalLength)
                            {
                                cellList[rowNumber][columnNumber].PossibleVerticalNumbers = FindNumbers(GetAllCombinations(cellList[rowNumber][columnNumber].AllPossibleNumbers, cellList[rowNumber][columnNumber].VerticalLength, cellList[rowNumber][columnNumber].VerticalSum));
                            }
                            cellList[rowNumber][columnNumber].AllPossibleNumbers = FindSectionNumbers(cellList[rowNumber][columnNumber].PossibleHorizontalNumbers, cellList[rowNumber][columnNumber].PossibleVerticalNumbers);
                        }
                        cellList[rowNumber][columnNumber].PossibleNumbers = cellList[rowNumber][columnNumber].AllPossibleNumbers;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            ListToValue(cellList);
        }
        private void CheckMaxMinSums(List<List<Cell>> cellList)
        {
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {
                    var numbersToRemove = new List<int>();
                    if (!cellList[rowNumber][columnNumber].Border && !cellList[rowNumber][columnNumber].Locked && !cellList[rowNumber][columnNumber].Iterated)
                    {
                        try
                        {
                            if (maxSums[cellList[rowNumber][columnNumber].VirtualHorizontalLength] == cellList[rowNumber][columnNumber].VirtualHorizontalSum)
                            {
                                foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                {
                                    if (!maxSumNumbers[cellList[rowNumber][columnNumber].VirtualHorizontalLength].Contains(number) && !numbersToRemove.Contains(number))
                                    {
                                        numbersToRemove.Add(number);
                                    }
                                }
                            }
                            if (maxSums[cellList[rowNumber][columnNumber].VirtualVerticalLength] == cellList[rowNumber][columnNumber].VirtualVerticalSum)
                            {
                                foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                {
                                    if (!maxSumNumbers[cellList[rowNumber][columnNumber].VirtualVerticalLength].Contains(number) && !numbersToRemove.Contains(number))
                                    {
                                        numbersToRemove.Add(number);
                                    }
                                }
                            }
                            if (minSums[cellList[rowNumber][columnNumber].VirtualHorizontalLength] == cellList[rowNumber][columnNumber].VirtualHorizontalSum)
                            {
                                foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                {
                                    if (!minSumNumbers[cellList[rowNumber][columnNumber].VirtualHorizontalLength].Contains(number) && !numbersToRemove.Contains(number))
                                    {
                                        numbersToRemove.Add(number);
                                    }
                                }
                            }
                            if (minSums[cellList[rowNumber][columnNumber].VirtualVerticalLength] == cellList[rowNumber][columnNumber].VirtualVerticalSum)
                            {
                                foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                {
                                    if (!minSumNumbers[cellList[rowNumber][columnNumber].VirtualVerticalLength].Contains(number) && !numbersToRemove.Contains(number))
                                    {
                                        numbersToRemove.Add(number);
                                    }
                                }
                            }
                            foreach (var number in numbersToRemove)
                            {
                                cellList[rowNumber][columnNumber].PossibleNumbers.Remove(number);
                            }
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
            }
            ListToValue(cellList);
        }
        private void FindPossibleNumbers(List<List<Cell>> cellList)
        {
            var restrictedHorizontalNumbers = new List<int>();
            var restrictedVerticalNumbers = new List<int>();
            List<int> virtualPossibleHorizontalNumbers = new List<int>();
            List<int> virtualPossibleVerticalNumbers = new List<int>();
            List<int> allVirtualPossibleHorizontalNumbers = new List<int>();
            List<int> allVirtualPossibleVerticalNumbers = new List<int>();
            List<int> numbersToRemove = new List<int>();
            int horizontalCounter = 1;
            int verticalCounter = 1;
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)      //Remove certain (100%) value from other possible numbers in that row / column
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {

                    try
                    {
                        if (!cellList[rowNumber][columnNumber].Border && !cellList[rowNumber][columnNumber].Locked)
                        {
                            var listOfCombinationsForRemove = new List<List<int>>();

                            if (cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Count >= cellList[rowNumber][columnNumber].VirtualHorizontalLength)
                            {
                                cellList[rowNumber][columnNumber].AllHorizontalCombinations = GetAllCombinations(cellList[rowNumber][columnNumber].PossibleHorizontalNumbers, cellList[rowNumber][columnNumber].VirtualHorizontalLength, cellList[rowNumber][columnNumber].VirtualHorizontalSum);
                                foreach (var combination in cellList[rowNumber][columnNumber].AllHorizontalCombinations)
                                {
                                    foreach (var number in cellList[rowNumber][columnNumber].CertainNumbersInRow)
                                    {
                                        if (!combination.Contains(number))
                                        {
                                            listOfCombinationsForRemove.Add(combination);
                                        }
                                    }

                                }
                                foreach (var combination in listOfCombinationsForRemove)
                                {
                                    cellList[rowNumber][columnNumber].AllHorizontalCombinations.Remove(combination);
                                }
                                cellList[rowNumber][columnNumber].PossibleHorizontalNumbers = FindNumbers(cellList[rowNumber][columnNumber].AllHorizontalCombinations);
                            }

                            if (cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Count >= cellList[rowNumber][columnNumber].VirtualVerticalLength)
                            {
                                cellList[rowNumber][columnNumber].AllVerticalCombinations = GetAllCombinations(cellList[rowNumber][columnNumber].PossibleVerticalNumbers, cellList[rowNumber][columnNumber].VirtualVerticalLength, cellList[rowNumber][columnNumber].VirtualVerticalSum);
                                listOfCombinationsForRemove.Clear();

                                foreach (var combination in cellList[rowNumber][columnNumber].AllVerticalCombinations)
                                {
                                    foreach (var number in cellList[rowNumber][columnNumber].CertainNumbersInColumn)
                                    {
                                        if (!combination.Contains(number))
                                        {
                                            listOfCombinationsForRemove.Add(combination);
                                        }
                                    }
                                }

                                foreach (var combination in listOfCombinationsForRemove)
                                {
                                    cellList[rowNumber][columnNumber].AllVerticalCombinations.Remove(combination);
                                }
                                cellList[rowNumber][columnNumber].PossibleVerticalNumbers = FindNumbers(cellList[rowNumber][columnNumber].AllVerticalCombinations);
                            }
                            cellList[rowNumber][columnNumber].PossibleNumbers = FindSectionNumbers(cellList[rowNumber][columnNumber].PossibleHorizontalNumbers, cellList[rowNumber][columnNumber].PossibleVerticalNumbers);
                            if (cellList[rowNumber][columnNumber].AllHorizontalCombinations != null && cellList[rowNumber][columnNumber].AllHorizontalCombinations.Count == 1)
                            {
                                foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                {
                                    bool counter = false;

                                    for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                                    {
                                        if (!cellList[rowNumber][i].Locked)
                                        {
                                            if (cellList[rowNumber][i].PossibleNumbers.Contains(number))
                                            {
                                                counter = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!counter)
                                    {
                                        for (int i = columnNumber - 1; i > 0 && !cellList[rowNumber][i].Border; i--)
                                        {
                                            if (!cellList[rowNumber][i].Locked)
                                            {
                                                if (cellList[rowNumber][i].PossibleNumbers.Contains(number))
                                                {
                                                    counter = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (!counter)
                                    {
                                        cellList[rowNumber][columnNumber].Value = number;
                                        cellList[rowNumber][columnNumber].PossibleNumbers.Clear();
                                        cellList[rowNumber][columnNumber].PossibleNumbers.Add(cellList[rowNumber][columnNumber].Value);
                                        cellList[rowNumber][columnNumber].Locked = true;
                                        if (cellList[rowNumber][columnNumber].CertainNumbersInColumn.Contains(cellList[rowNumber][columnNumber].Value) || cellList[rowNumber][columnNumber].CertainNumbersInRow.Contains(cellList[rowNumber][columnNumber].Value))
                                        {
                                            RemoveFromCertainNumbers(cellList, rowNumber, columnNumber, cellList[rowNumber][columnNumber].Value);
                                        }
                                        break;
                                    }
                                }
                            }
                            if (cellList[rowNumber][columnNumber].AllVerticalCombinations != null && cellList[rowNumber][columnNumber].AllVerticalCombinations.Count == 1)
                            {
                                foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                {
                                    bool counter = false;

                                    for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                                    {
                                        if (!cellList[j][columnNumber].Locked)
                                        {
                                            if (cellList[j][columnNumber].PossibleNumbers.Contains(number))
                                            {
                                                counter = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!counter)
                                    {
                                        for (int j = rowNumber - 1; j > 0 && !cellList[j][columnNumber].Border; j--)
                                        {
                                            if (!cellList[j][columnNumber].Locked)
                                            {
                                                if (cellList[j][columnNumber].PossibleNumbers.Contains(number))
                                                {
                                                    counter = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (!counter)
                                    {
                                        cellList[rowNumber][columnNumber].Value = number;
                                        cellList[rowNumber][columnNumber].PossibleNumbers.Clear();
                                        cellList[rowNumber][columnNumber].PossibleNumbers.Add(cellList[rowNumber][columnNumber].Value);
                                        cellList[rowNumber][columnNumber].Locked = true;
                                        if (cellList[rowNumber][columnNumber].CertainNumbersInColumn.Contains(cellList[rowNumber][columnNumber].Value) || cellList[rowNumber][columnNumber].CertainNumbersInRow.Contains(cellList[rowNumber][columnNumber].Value))
                                        {
                                            RemoveFromCertainNumbers(cellList, rowNumber, columnNumber, cellList[rowNumber][columnNumber].Value);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    ListToValue(cellList);
                    Iterate(cellList);
                }
            }
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)     //removing numbers from possible numbers
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {
                    try
                    {
                        if (!cellList[rowNumber][columnNumber].Locked)
                        {
                            if (cellList[rowNumber][columnNumber].VirtualHorizontalLength == 2)      //removing numbers from possible numbers
                            {
                                cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Clear();
                                if ((columnNumber + 1) < nColsGlobal && !cellList[rowNumber][columnNumber + 1].Locked && !cellList[rowNumber][columnNumber + 1].Border) //element after
                                {
                                    cellList[rowNumber][columnNumber + 1].PossibleHorizontalNumbers.Clear();
                                    foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                    {
                                        cellList[rowNumber][columnNumber + 1].PossibleHorizontalNumbers.Add(cellList[rowNumber][columnNumber].VirtualHorizontalSum - number);
                                    }
                                    foreach (var number in cellList[rowNumber][columnNumber + 1].PossibleNumbers)
                                    {
                                        cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Add(cellList[rowNumber][columnNumber].VirtualHorizontalSum - number);
                                    }
                                    cellList[rowNumber][columnNumber].PossibleNumbers = FindSectionNumbers(cellList[rowNumber][columnNumber].PossibleNumbers, cellList[rowNumber][columnNumber].PossibleHorizontalNumbers);
                                    cellList[rowNumber][columnNumber + 1].PossibleNumbers = FindSectionNumbers(cellList[rowNumber][columnNumber + 1].PossibleNumbers, cellList[rowNumber][columnNumber + 1].PossibleHorizontalNumbers);
                                }
                                else if (!cellList[rowNumber][columnNumber - 1].Locked && !cellList[rowNumber][columnNumber - 1].Border) //element before 
                                {
                                    cellList[rowNumber][columnNumber - 1].PossibleHorizontalNumbers.Clear();
                                    foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                    {
                                        cellList[rowNumber][columnNumber - 1].PossibleHorizontalNumbers.Add(cellList[rowNumber][columnNumber].VirtualHorizontalSum - number);
                                    }
                                    foreach (var number in cellList[rowNumber][columnNumber - 1].PossibleNumbers)
                                    {
                                        cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Add(cellList[rowNumber][columnNumber].VirtualHorizontalSum - number);
                                    }
                                    cellList[rowNumber][columnNumber].PossibleNumbers = FindSectionNumbers(cellList[rowNumber][columnNumber].PossibleNumbers, cellList[rowNumber][columnNumber].PossibleHorizontalNumbers);
                                    cellList[rowNumber][columnNumber - 1].PossibleNumbers = FindSectionNumbers(cellList[rowNumber][columnNumber - 1].PossibleNumbers, cellList[rowNumber][columnNumber - 1].PossibleHorizontalNumbers);
                                }

                            }
                            if (cellList[rowNumber][columnNumber].VirtualVerticalLength == 2)      //removing numbers from possible numbers
                            {
                                cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Clear();
                                if ((rowNumber + 1) < nRowsGlobal && !cellList[rowNumber + 1][columnNumber].Locked && !cellList[rowNumber + 1][columnNumber].Border) //element after
                                {
                                    cellList[rowNumber + 1][columnNumber].PossibleVerticalNumbers.Clear();
                                    foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                    {
                                        cellList[rowNumber + 1][columnNumber].PossibleVerticalNumbers.Add(cellList[rowNumber][columnNumber].VirtualVerticalSum - number);
                                    }
                                    foreach (var number in cellList[rowNumber + 1][columnNumber].PossibleNumbers)
                                    {
                                        cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Add(cellList[rowNumber + 1][columnNumber].VirtualVerticalSum - number);
                                    }
                                    cellList[rowNumber][columnNumber].PossibleNumbers = FindSectionNumbers(cellList[rowNumber][columnNumber].PossibleNumbers, cellList[rowNumber][columnNumber].PossibleVerticalNumbers);
                                    cellList[rowNumber + 1][columnNumber].PossibleNumbers = FindSectionNumbers(cellList[rowNumber + 1][columnNumber].PossibleNumbers, cellList[rowNumber + 1][columnNumber].PossibleVerticalNumbers);
                                }
                                else if (!cellList[rowNumber - 1][columnNumber].Locked && !cellList[rowNumber - 1][columnNumber].Border) //element after
                                {
                                    cellList[rowNumber - 1][columnNumber].PossibleVerticalNumbers.Clear();
                                    foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                    {
                                        cellList[rowNumber - 1][columnNumber].PossibleVerticalNumbers.Add(cellList[rowNumber][columnNumber].VirtualVerticalSum - number);
                                    }
                                    foreach (var number in cellList[rowNumber - 1][columnNumber].PossibleNumbers)
                                    {
                                        cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Add(cellList[rowNumber - 1][columnNumber].VirtualVerticalSum - number);
                                    }
                                    cellList[rowNumber][columnNumber].PossibleNumbers = FindSectionNumbers(cellList[rowNumber][columnNumber].PossibleNumbers, cellList[rowNumber][columnNumber].PossibleVerticalNumbers);
                                    cellList[rowNumber - 1][columnNumber].PossibleNumbers = FindSectionNumbers(cellList[rowNumber - 1][columnNumber].PossibleNumbers, cellList[rowNumber - 1][columnNumber].PossibleVerticalNumbers);
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {
                        return;
                    }
                    ListToValue(cellList);
                    Iterate(cellList);

                }
            }
            //virtualPossibleHorizontalNumbers.Clear();
            //virtualPossibleVerticalNumbers.Clear();
            //allVirtualPossibleHorizontalNumbers.Clear();
            //allVirtualPossibleVerticalNumbers.Clear();
            //numbersToRemove.Clear();
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)     //removing numbers from possible numbers    OP FUNCTION
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {

                    try
                    {
                        if (!cellList[rowNumber][columnNumber].Locked && !cellList[rowNumber][columnNumber].Border && cellList[rowNumber][columnNumber].VirtualVerticalLength == 1)
                        {
                            cellList[rowNumber][columnNumber].Value = cellList[rowNumber][columnNumber].VirtualVerticalSum;
                            cellList[rowNumber][columnNumber].PossibleNumbers.Clear();
                            cellList[rowNumber][columnNumber].PossibleNumbers.Add(cellList[rowNumber][columnNumber].Value);
                            cellList[rowNumber][columnNumber].Locked = true;
                        }
                        if (!cellList[rowNumber][columnNumber].Locked && !cellList[rowNumber][columnNumber].Border && cellList[rowNumber][columnNumber].VirtualHorizontalLength == 1)
                        {
                            cellList[rowNumber][columnNumber].Value = cellList[rowNumber][columnNumber].VirtualHorizontalSum;
                            cellList[rowNumber][columnNumber].PossibleNumbers.Clear();
                            cellList[rowNumber][columnNumber].PossibleNumbers.Add(cellList[rowNumber][columnNumber].Value);
                            cellList[rowNumber][columnNumber].Locked = true;
                        }
                        if (!cellList[rowNumber][columnNumber].Locked && !cellList[rowNumber][columnNumber].Border)
                        {

                            cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Clear();
                            cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Clear();
                            virtualPossibleHorizontalNumbers.Clear();
                            virtualPossibleVerticalNumbers.Clear();
                            allVirtualPossibleHorizontalNumbers.Clear();
                            allVirtualPossibleVerticalNumbers.Clear();
                            numbersToRemove.Clear();
                            foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)               //za svaki broj od possible numbersa
                            {
                                for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                                {
                                    if (!cellList[rowNumber][i].Locked)
                                    {
                                        foreach (var item in cellList[rowNumber][i].PossibleNumbers)
                                        {
                                            if (!cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Contains(item) && (number != item))
                                            {
                                                cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Add(item);
                                            }
                                        }
                                    }
                                }
                                for (int i = columnNumber - 1; i > 0 && !cellList[rowNumber][i].Border; i--)
                                {
                                    if (!cellList[rowNumber][i].Locked)
                                    {
                                        foreach (var item in cellList[rowNumber][i].PossibleNumbers)
                                        {
                                            if (!cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Contains(item) && (number != item))
                                            {
                                                cellList[rowNumber][columnNumber].PossibleHorizontalNumbers.Add(item);
                                            }
                                        }
                                    }
                                }

                                for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                                {
                                    if (!cellList[j][columnNumber].Locked)
                                    {
                                        foreach (var item in cellList[j][columnNumber].PossibleNumbers)
                                        {
                                            if (!cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Contains(item) && (number != item))
                                            {
                                                cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Add(item);
                                            }
                                        }
                                    }
                                }
                                for (int j = rowNumber - 1; j > 0 && !cellList[j][columnNumber].Border; j--)
                                {
                                    if (!cellList[j][columnNumber].Locked)
                                    {
                                        foreach (var item in cellList[j][columnNumber].PossibleNumbers)
                                        {
                                            if (!cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Contains(item) && (number != item))
                                            {
                                                cellList[rowNumber][columnNumber].PossibleVerticalNumbers.Add(item);
                                            }
                                        }
                                    }
                                }

                                virtualPossibleHorizontalNumbers = FindNumbers(GetAllCombinations(cellList[rowNumber][columnNumber].PossibleHorizontalNumbers, cellList[rowNumber][columnNumber].VirtualHorizontalLength - 1, cellList[rowNumber][columnNumber].VirtualHorizontalSum - number));
                                virtualPossibleVerticalNumbers = FindNumbers(GetAllCombinations(cellList[rowNumber][columnNumber].PossibleVerticalNumbers, cellList[rowNumber][columnNumber].VirtualVerticalLength - 1, cellList[rowNumber][columnNumber].VirtualVerticalSum - number));

                                if (virtualPossibleHorizontalNumbers.Count == 0 || virtualPossibleVerticalNumbers.Count == 0)
                                {
                                    numbersToRemove.Add(number);
                                }
                                foreach (var item in virtualPossibleHorizontalNumbers)
                                {
                                    if (!allVirtualPossibleHorizontalNumbers.Contains(item))
                                    {
                                        allVirtualPossibleHorizontalNumbers.Add(item);
                                    }
                                }
                                foreach (var item in virtualPossibleVerticalNumbers)
                                {
                                    if (!allVirtualPossibleVerticalNumbers.Contains(item))
                                    {
                                        allVirtualPossibleVerticalNumbers.Add(item);
                                    }
                                }
                            }           //gotov foreach od possible numbersa

                            foreach (var number in numbersToRemove)         //brise sve possible number za koje nema kombinacije
                            {
                                if (cellList[rowNumber][columnNumber].PossibleNumbers.Contains(number))
                                {
                                    cellList[rowNumber][columnNumber].PossibleNumbers.Remove(number);
                                }
                            }

                            for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                            {
                                if (!cellList[rowNumber][i].Locked)
                                {
                                    cellList[rowNumber][i].PossibleNumbers = FindSectionNumbers(cellList[rowNumber][i].PossibleNumbers, allVirtualPossibleHorizontalNumbers);
                                }
                            }
                            for (int i = columnNumber - 1; i > 0 && !cellList[rowNumber][i].Border; i--)
                            {
                                if (!cellList[rowNumber][i].Locked)
                                {
                                    cellList[rowNumber][i].PossibleNumbers = FindSectionNumbers(cellList[rowNumber][i].PossibleNumbers, allVirtualPossibleHorizontalNumbers);

                                }
                            }

                            for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                            {
                                if (!cellList[j][columnNumber].Locked)
                                {
                                    cellList[j][columnNumber].PossibleNumbers = FindSectionNumbers(cellList[j][columnNumber].PossibleNumbers, allVirtualPossibleVerticalNumbers);
                                }
                            }
                            for (int j = rowNumber - 1; j > 0 && !cellList[j][columnNumber].Border; j--)
                            {
                                if (!cellList[j][columnNumber].Locked)
                                {
                                    cellList[j][columnNumber].PossibleNumbers = FindSectionNumbers(cellList[j][columnNumber].PossibleNumbers, allVirtualPossibleVerticalNumbers);
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {

                        return;
                    }
                    ListToValue(cellList);
                    Iterate(cellList);

                }
            }
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)      //removing numbers from possible numbers if same possible numbers exist in multiple cells
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {

                    horizontalCounter = 1;
                    verticalCounter = 1;
                    if (!cellList[rowNumber][columnNumber].Locked)
                    {
                        try
                        {
                            for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                            {
                                if (!cellList[rowNumber][i].Locked && cellList[rowNumber][i].PossibleNumbers.SequenceEqual(cellList[rowNumber][columnNumber].PossibleNumbers))
                                {
                                    horizontalCounter++;
                                }
                            }
                            for (int i = columnNumber - 1; i > 0 && !cellList[rowNumber][i].Border; i--)
                            {
                                if (!cellList[rowNumber][i].Locked && cellList[rowNumber][i].PossibleNumbers.SequenceEqual(cellList[rowNumber][columnNumber].PossibleNumbers))
                                {
                                    horizontalCounter++;
                                }
                            }

                            for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                            {
                                if (!cellList[j][columnNumber].Locked && cellList[j][columnNumber].PossibleNumbers.SequenceEqual(cellList[rowNumber][columnNumber].PossibleNumbers))
                                {
                                    verticalCounter++;
                                }
                            }
                            for (int j = rowNumber - 1; j > 0 && !cellList[j][columnNumber].Border; j--)
                            {
                                if (!cellList[j][columnNumber].Locked && cellList[j][columnNumber].PossibleNumbers.SequenceEqual(cellList[rowNumber][columnNumber].PossibleNumbers))
                                {
                                    verticalCounter++;
                                }
                            }
                            if (horizontalCounter == cellList[rowNumber][columnNumber].PossibleNumbers.Count)
                            {
                                for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                                {
                                    if (!cellList[rowNumber][i].Locked)
                                    {
                                        foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                        {
                                            if (!cellList[rowNumber][i].CertainNumbersInRow.Contains(number))
                                            {
                                                cellList[rowNumber][i].CertainNumbersInRow.Add(number);
                                            }
                                        }
                                        if (!cellList[rowNumber][i].PossibleNumbers.SequenceEqual(cellList[rowNumber][columnNumber].PossibleNumbers))
                                        {
                                            foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                            {
                                                if (cellList[rowNumber][i].PossibleNumbers.Contains(number))
                                                {
                                                    cellList[rowNumber][i].PossibleNumbers.Remove(number);
                                                }
                                            }
                                        }
                                    }
                                }
                                for (int i = columnNumber - 1; i > 0 && !cellList[rowNumber][i].Border; i--)
                                {
                                    if (!cellList[rowNumber][i].Locked)
                                    {
                                        foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                        {
                                            if (!cellList[rowNumber][i].CertainNumbersInRow.Contains(number))
                                            {
                                                cellList[rowNumber][i].CertainNumbersInRow.Add(number);
                                            }
                                        }
                                        if (!cellList[rowNumber][i].PossibleNumbers.SequenceEqual(cellList[rowNumber][columnNumber].PossibleNumbers))
                                        {
                                            foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                            {
                                                if (cellList[rowNumber][i].PossibleNumbers.Contains(number))
                                                {
                                                    cellList[rowNumber][i].PossibleNumbers.Remove(number);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (verticalCounter == cellList[rowNumber][columnNumber].PossibleNumbers.Count)
                            {
                                for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                                {

                                    if (!cellList[j][columnNumber].Locked)
                                    {
                                        foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                        {
                                            if (!cellList[j][columnNumber].CertainNumbersInColumn.Contains(number))
                                            {
                                                cellList[j][columnNumber].CertainNumbersInColumn.Add(number);
                                            }
                                        }
                                        if (!cellList[j][columnNumber].PossibleNumbers.SequenceEqual(cellList[rowNumber][columnNumber].PossibleNumbers))
                                        {
                                            foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                            {
                                                if (cellList[j][columnNumber].PossibleNumbers.Contains(number))
                                                {
                                                    cellList[j][columnNumber].PossibleNumbers.Remove(number);
                                                }
                                            }
                                        }

                                    }
                                }
                                for (int j = rowNumber - 1; j > 0 && !cellList[j][columnNumber].Border; j--)
                                {
                                    if (!cellList[j][columnNumber].Locked)
                                    {
                                        foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                        {
                                            if (!cellList[j][columnNumber].CertainNumbersInColumn.Contains(number))
                                            {
                                                cellList[j][columnNumber].CertainNumbersInColumn.Add(number);
                                            }
                                        }
                                        if (!cellList[j][columnNumber].PossibleNumbers.SequenceEqual(cellList[rowNumber][columnNumber].PossibleNumbers))
                                        {
                                            foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                            {
                                                if (cellList[j][columnNumber].PossibleNumbers.Contains(number))
                                                {
                                                    cellList[j][columnNumber].PossibleNumbers.Remove(number);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                            return;
                        }
                        ListToValue(cellList);
                        Iterate(cellList);
                    }

                }
            }
            for (int rowNumber = 0; rowNumber < nRowsGlobal; rowNumber++)       //Remove certain (100%) value from other possible numbers in that row / column
            {
                for (int columnNumber = 0; columnNumber < nColsGlobal; columnNumber++)
                {
                    try
                    {
                        if (!cellList[rowNumber][columnNumber].Locked && !cellList[rowNumber][columnNumber].Border)
                        {
                            if (cellList[rowNumber][columnNumber].AllHorizontalCombinations != null && cellList[rowNumber][columnNumber].AllHorizontalCombinations.Count == 1)
                            {
                                foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                {
                                    bool counter = false;

                                    for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
                                    {
                                        if (!cellList[rowNumber][i].Locked)
                                        {
                                            if (cellList[rowNumber][i].PossibleNumbers.Contains(number))
                                            {
                                                counter = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (!restrictedHorizontalNumbers.Contains(cellList[rowNumber][i].Value))
                                            {
                                                restrictedHorizontalNumbers.Add(cellList[rowNumber][i].Value);
                                            }
                                        }
                                    }
                                    for (int i = columnNumber - 1; i > 0 && !cellList[rowNumber][i].Border; i--)
                                    {
                                        if (!cellList[rowNumber][i].Locked)
                                        {
                                            if (cellList[rowNumber][i].PossibleNumbers.Contains(number))
                                            {
                                                counter = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (!restrictedHorizontalNumbers.Contains(cellList[rowNumber][i].Value))
                                            {
                                                restrictedHorizontalNumbers.Add(cellList[rowNumber][i].Value);
                                            }
                                        }
                                    }
                                    if (!counter && !restrictedHorizontalNumbers.Contains(number))
                                    {
                                        cellList[rowNumber][columnNumber].Value = number;
                                        cellList[rowNumber][columnNumber].PossibleNumbers.Clear();
                                        cellList[rowNumber][columnNumber].PossibleNumbers.Add(cellList[rowNumber][columnNumber].Value);
                                        cellList[rowNumber][columnNumber].Locked = true;
                                        restrictedHorizontalNumbers.Clear();
                                        if (cellList[rowNumber][columnNumber].CertainNumbersInColumn.Contains(cellList[rowNumber][columnNumber].Value) || cellList[rowNumber][columnNumber].CertainNumbersInRow.Contains(cellList[rowNumber][columnNumber].Value))
                                        {
                                            RemoveFromCertainNumbers(cellList, rowNumber, columnNumber, cellList[rowNumber][columnNumber].Value);
                                        }
                                        break;
                                    }
                                    restrictedHorizontalNumbers.Clear();
                                }
                            }
                            if (cellList[rowNumber][columnNumber].AllVerticalCombinations != null && cellList[rowNumber][columnNumber].AllVerticalCombinations.Count == 1)
                            {
                                foreach (var number in cellList[rowNumber][columnNumber].PossibleNumbers)
                                {
                                    bool counter = false;

                                    for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
                                    {
                                        if (!cellList[j][columnNumber].Locked)
                                        {
                                            if (cellList[j][columnNumber].PossibleNumbers.Contains(number))
                                            {
                                                counter = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (!restrictedVerticalNumbers.Contains(cellList[j][columnNumber].Value))
                                            {
                                                restrictedVerticalNumbers.Add(cellList[j][columnNumber].Value);
                                            }
                                        }
                                    }
                                    for (int j = rowNumber - 1; j > 0 && !cellList[j][columnNumber].Border; j--)
                                    {
                                        if (!cellList[j][columnNumber].Locked)
                                        {
                                            if (cellList[j][columnNumber].PossibleNumbers.Contains(number))
                                            {
                                                counter = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (!restrictedVerticalNumbers.Contains(cellList[j][columnNumber].Value))
                                            {
                                                restrictedVerticalNumbers.Add(cellList[j][columnNumber].Value);
                                            }
                                        }
                                    }
                                    if (!counter && !restrictedVerticalNumbers.Contains(number))
                                    {
                                        cellList[rowNumber][columnNumber].Value = number;
                                        cellList[rowNumber][columnNumber].PossibleNumbers.Clear();
                                        cellList[rowNumber][columnNumber].PossibleNumbers.Add(cellList[rowNumber][columnNumber].Value);
                                        cellList[rowNumber][columnNumber].Locked = true;
                                        restrictedVerticalNumbers.Clear();
                                        if (cellList[rowNumber][columnNumber].CertainNumbersInColumn.Contains(cellList[rowNumber][columnNumber].Value) || cellList[rowNumber][columnNumber].CertainNumbersInRow.Contains(cellList[rowNumber][columnNumber].Value))
                                        {
                                            RemoveFromCertainNumbers(cellList, rowNumber, columnNumber, cellList[rowNumber][columnNumber].Value);
                                        }
                                        break;
                                    }
                                    restrictedVerticalNumbers.Clear();
                                }
                            }
                        }


                    }

                    catch (Exception)
                    {
                        return;
                    }
                    ListToValue(cellList);
                    Iterate(cellList);
                }
            }
        }
        private void ClearFields()
        {
            //foreach (Control c in this.table.Controls)
            //{
            //    if (!c.Text.Contains("\\"))
            //    {
            //        c.Text = "";
            //        c.BackColor = Color.LightBlue;
            //    }
            //    else
            //    {
            //        c.Text = "\\";
            //    }
            //}
        }
        private void RemoveFromCertainNumbers(List<List<Cell>> cellList, int rowNumber, int columnNumber, int value)
        {
            //for (int i = columnNumber + 1; i < nColsGlobal && !cellList[rowNumber][i].Border; i++)
            //{
            //    if (!cellList[rowNumber][i].Locked && cellList[rowNumber][i].CertainNumbersInRow.Contains(value))
            //    {
            //        cellList[rowNumber][i].CertainNumbersInRow.Remove(value);
            //    }
            //}
            //for (int i = columnNumber - 1; i > 0 && !cellList[rowNumber][i].Border; i--)
            //{
            //    if (!cellList[rowNumber][i].Locked && cellList[rowNumber][i].CertainNumbersInRow.Contains(value))
            //    {
            //        cellList[rowNumber][i].CertainNumbersInRow.Remove(value);
            //    }
            //}
            //for (int j = rowNumber + 1; j < nRowsGlobal && !cellList[j][columnNumber].Border; j++)
            //{
            //    if (!cellList[j][columnNumber].Locked && cellList[j][columnNumber].CertainNumbersInColumn.Contains(value))
            //    {
            //        cellList[j][columnNumber].CertainNumbersInColumn.Remove(value);
            //    }
            //}
            //for (int j = rowNumber - 1; j > 0 && !cellList[j][columnNumber].Border; j--)
            //{
            //    if (!cellList[j][columnNumber].Locked && cellList[j][columnNumber].CertainNumbersInColumn.Contains(value))
            //    {
            //        cellList[j][columnNumber].CertainNumbersInColumn.Remove(value);
            //    }
            //}
        }

        List<List<int>> maxSumNumbers = new List<List<int>>() {
        new List<int>{0},
        new List<int>{9},
        new List<int>{9,8},
        new List<int>{9,8,7},
        new List<int>{9,8,7,6},
        new List<int>{9,8,7,6,5},
        new List<int>{9,8,7,6,5,4},
        new List<int>{9,8,7,6,5,4,3},
        new List<int>{9,8,7,6,5,4,3,2},
        new List<int>{9,8,7,6,5,4,3,2,1},
        };
        List<List<int>> minSumNumbers = new List<List<int>>() {
        new List<int>{0},
        new List<int>{1},
        new List<int>{1,2},
        new List<int>{1,2,3},
        new List<int>{1,2,3,4},
        new List<int>{1,2,3,4,5},
        new List<int>{1,2,3,4,5,6},
        new List<int>{1,2,3,4,5,6,7},
        new List<int>{1,2,3,4,5,6,7,8},
        new List<int>{1,2,3,4,5,6,7,8,9},
        };
        List<int> maxSums = new List<int>() { 0, 9, 17, 24, 30, 35, 39, 42, 44, 45 };
        List<int> minSums = new List<int>() { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45 };

    }
}