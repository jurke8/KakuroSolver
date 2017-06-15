using KakuroSolver.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Tesseract;

namespace KakuroSolver.Helpers
{
    public class PictureHelper
    {
        private static List<Bitmap> kakuroSquares = new List<Bitmap>();
        private static int nRows = 0;
        private static int nCols = 0;


        public List<PictureCell> ReadFromImage(Bitmap originalImage, int nR, int nC)
        {
            kakuroSquares.Clear();
            nRows = nR;
            nCols = nC;

            //Copy original image
            Bitmap binaryImage = (Bitmap)originalImage.Clone();

            //Image binarization
            binaryImage = ToBlackAndWhite(binaryImage, true);

            //binaryImage.Save(AppDomain.CurrentDomain.BaseDirectory + @"\images\binary.png");

            //Reduce original to rectangle
            originalImage = ResizeToRectangle(binaryImage, originalImage);

            //Reduce binary to rectangle
            binaryImage = ResizeToRectangle(binaryImage, binaryImage);

            //binaryImage.Save(AppDomain.CurrentDomain.BaseDirectory + @"\images\binaryResized.png");
            //originalImage.Save(AppDomain.CurrentDomain.BaseDirectory + @"\images\originalResized.png");

            List<PictureCell> cells = new List<PictureCell>();

            //Find borders in original image
            FindBorders(originalImage, cells);

            //Slice kakuro in squares
            SliceKakuro(binaryImage, cells);


            //Slice every square and read text from small rectangles
            for (int i = 0; i < kakuroSquares.Count; i++)
            {
                if (cells.ElementAt(i).IsBorder)
                {
                    var smallRectangles = SliceSquare(kakuroSquares.ElementAt(i), i);

                    cells.ElementAt(i).VerticalSum = ReadTextFromImage(smallRectangles.ElementAt(0));
                    cells.ElementAt(i).HorizontalSum = ReadTextFromImage(smallRectangles.ElementAt(1));
                }
            }
            return cells;
        }
        public static void SliceKakuro(Bitmap originalImage, List<PictureCell> cells)
        {
            var sqrWidth = Convert.ToInt32(Math.Round(((decimal)originalImage.Width / (decimal)nCols), 0));
            var sqrHeight = Convert.ToInt32(Math.Round(((decimal)originalImage.Height / (decimal)nRows), 0));

            int processed = 0;
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    try
                    {
                        if (cells.ElementAt(processed).IsBorder)
                        {
                            Rectangle rect = new Rectangle(sqrWidth * j, sqrHeight * i, sqrWidth, sqrHeight);

                            if (sqrWidth * (j + 1) > originalImage.Width)
                            {
                                rect = new Rectangle(sqrWidth * j, sqrHeight * i, originalImage.Width - sqrWidth * j - 1, sqrHeight);
                            }
                            if (sqrHeight * (i + 1) > originalImage.Height)
                            {
                                rect = new Rectangle(sqrWidth * j, sqrHeight * i, sqrWidth, originalImage.Height - sqrHeight * i - 1);

                                if (sqrWidth * (j + 1) > originalImage.Width)
                                {
                                    rect = new Rectangle(sqrWidth * j, sqrHeight * i, originalImage.Width - sqrWidth * j - 1, originalImage.Height - sqrHeight * i - 1);
                                }
                            }
                            Bitmap cutted = originalImage.Clone(rect, originalImage.PixelFormat);
                            var resized = ResizeImage(cutted, cutted.Width * 2, cutted.Height * 2);
                            kakuroSquares.Add(resized);
                            //cutted.Save(AppDomain.CurrentDomain.BaseDirectory + @"\images\" + processed + ".png");
                        }
                        else
                        {
                            kakuroSquares.Add(null);
                        }
                        ++processed;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
            }
        }
        public static List<Bitmap> SliceSquare(Bitmap originalImage, int itemNumber)
        {
            var halfSqrWidth = (originalImage.Width / 2);
            var halfSqrHeight = (originalImage.Height / 2);

            Rectangle bottomLeftRect = new Rectangle(halfSqrWidth / 3, halfSqrHeight + Convert.ToInt32(halfSqrHeight / 7), halfSqrWidth - halfSqrWidth / 3, halfSqrHeight - Convert.ToInt32(halfSqrHeight / 2.2));
            Rectangle topRightRect = new Rectangle(halfSqrWidth, halfSqrHeight / 4, halfSqrWidth - halfSqrWidth / 4, halfSqrHeight / 2 + halfSqrHeight / 10);
            Bitmap bottomLeftCutted = originalImage.Clone(bottomLeftRect, originalImage.PixelFormat);
            Bitmap topRightCutted = originalImage.Clone(topRightRect, originalImage.PixelFormat);

            //var bottomLeftIndex = 1001 + itemNumber;
            //var topRightIndex = 2001 + itemNumber;
            //bottomLeftCutted.Save(AppDomain.CurrentDomain.BaseDirectory + @"\images\" + bottomLeftIndex + ".png");
            //topRightCutted.Save(AppDomain.CurrentDomain.BaseDirectory + @"\images\" + topRightIndex + ".png");

            var returnList = new List<Bitmap>();
            returnList.Add(bottomLeftCutted);
            returnList.Add(topRightCutted);
            return returnList;
        }
        public static string ReadTextFromImage(Bitmap image)
        {
            var dataPath = AppDomain.CurrentDomain.BaseDirectory + @"tessdata";

            //Creating the tesseract OCR engine with English as the language
            using (var tEngine = new TesseractEngine(dataPath, "eng", EngineMode.Default))
            {
                //Process the specified image
                using (var page = tEngine.Process(image, Tesseract.PageSegMode.SingleWord))
                {
                    tEngine.SetVariable("tessedit_char_whitelist", "0123456789");

                    //Gets the image's content as plain text
                    var text = page.GetText();

                    try
                    {
                        var value = Convert.ToInt32(text);
                        if (value < 3 || value > 45)
                        {
                            text = "?";
                        }
                        else
                        {
                            text = value.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        text = "";
                    }
                    return text;
                }
            }
        }
        public static Bitmap ToBlackAndWhite(Bitmap image, bool invert)
        {
            int rgb;
            Color c;
            List<Color> colors = new List<Color>();
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    c = image.GetPixel(x, y);
                    colors.Add(c);
                    rgb = (int)((c.R + c.G + c.B) / 3);
                    if (invert)
                    {
                        rgb = rgb > 128 ? 0 : 255;
                    }
                    else
                    {
                        rgb = rgb < 128 ? 0 : 255;
                    }
                    image.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return image;
        }
        public static Bitmap ResizeImage(Bitmap image, int newWidth, int newHeight)
        {

            Bitmap temp = (Bitmap)image;

            Bitmap bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat);

            double nWidthFactor = (double)temp.Width / (double)newWidth;
            double nHeightFactor = (double)temp.Height / (double)newHeight;

            double fx, fy, nx, ny;
            int cx, cy, fr_x, fr_y;
            Color color1 = new Color();
            Color color2 = new Color();
            Color color3 = new Color();
            Color color4 = new Color();
            byte nRed, nGreen, nBlue;

            byte bp1, bp2;

            for (int x = 0; x < bmap.Width; ++x)
            {
                for (int y = 0; y < bmap.Height; ++y)
                {

                    fr_x = (int)Math.Floor(x * nWidthFactor);
                    fr_y = (int)Math.Floor(y * nHeightFactor);
                    cx = fr_x + 1;
                    if (cx >= temp.Width) cx = fr_x;
                    cy = fr_y + 1;
                    if (cy >= temp.Height) cy = fr_y;
                    fx = x * nWidthFactor - fr_x;
                    fy = y * nHeightFactor - fr_y;
                    nx = 1.0 - fx;
                    ny = 1.0 - fy;

                    color1 = temp.GetPixel(fr_x, fr_y);
                    color2 = temp.GetPixel(cx, fr_y);
                    color3 = temp.GetPixel(fr_x, cy);
                    color4 = temp.GetPixel(cx, cy);

                    // Blue
                    bp1 = (byte)(nx * color1.B + fx * color2.B);

                    bp2 = (byte)(nx * color3.B + fx * color4.B);

                    nBlue = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                    // Green
                    bp1 = (byte)(nx * color1.G + fx * color2.G);

                    bp2 = (byte)(nx * color3.G + fx * color4.G);

                    nGreen = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                    // Red
                    bp1 = (byte)(nx * color1.R + fx * color2.R);

                    bp2 = (byte)(nx * color3.R + fx * color4.R);

                    nRed = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                    bmap.SetPixel(x, y, System.Drawing.Color.FromArgb(255, nRed, nGreen, nBlue));
                }
            }

            return bmap;
        }
        public static Bitmap ResizeToRectangle(Bitmap binaryImage, Bitmap imageToReduce)
        {
            Color c;
            int leftMargin = 0, rightMargin = 0, topMargin = 0, bottomMargin = 0;
            #region FindingMargins
            for (int x = 0; x < binaryImage.Width; x++)
            {
                c = binaryImage.GetPixel(x, binaryImage.Height / 2);
                if ("ffffffff".Equals(c.Name))
                {
                    leftMargin = x + 1;
                    break;
                }
            }
            for (int x = binaryImage.Width - 1; x > 0; x--)
            {
                c = binaryImage.GetPixel(x, binaryImage.Height / 2);
                if ("ffffffff".Equals(c.Name))
                {
                    rightMargin = x - 1;
                    break;
                }
            }
            for (int y = 0; y < binaryImage.Height; y++)
            {
                c = binaryImage.GetPixel(binaryImage.Width / 2, y);
                if ("ffffffff".Equals(c.Name))
                {
                    topMargin = y + 1;
                    break;
                }
            }
            for (int y = binaryImage.Height - 1; y > 0; y--)
            {
                c = binaryImage.GetPixel(binaryImage.Width / 2, y);
                if ("ffffffff".Equals(c.Name))
                {
                    bottomMargin = y - 1;
                    break;
                }
            }
            #endregion

            Rectangle rect = new Rectangle(leftMargin, topMargin, rightMargin - leftMargin, bottomMargin - topMargin);

            return imageToReduce.Clone(rect, imageToReduce.PixelFormat);
        }
        public static List<PictureCell> FindBorders(Bitmap image, List<PictureCell> cells)
        {
            var sqrWidth = (image.Width / nCols);
            var sqrHeight = (image.Height / nRows);

            Color c;
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    var pictureCell = new PictureCell();

                    c = image.GetPixel(10 + sqrWidth * j, sqrHeight / 2 + sqrHeight * i);
                    if ("ffffffff".Equals(c.Name))
                    {
                        pictureCell.IsBorder = false;
                        cells.Add(pictureCell);
                    }
                    else
                    {
                        pictureCell.IsBorder = true;
                        cells.Add(pictureCell);
                    }
                }
            }
            return cells;
        }
        public List<PictureCell> GetBorder(int rows, int colums)
        {
            var cells = new List<PictureCell>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < colums; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        var cell = new PictureCell() { IsBorder=true};
                        cells.Add(cell);
                    }
                    else
                    {
                        var cell = new PictureCell() { IsBorder = false };
                        cells.Add(cell);
                    }
                }
            }
            return cells;
        }
    }
}