#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GDS.Core {
    public enum Direction { N, E, S, W }
    public record Window(Pos Pos, Size Size);
    public record MatrixWindow(Window Window, int[,] Matrix, Direction Direction);
    public record GridWindow(Pos Pos, Size Size, int[,] Shape);
    public static class MathUtil {

        public static IEnumerable<Pos> CreateGrid(Size size) => Enumerable.Range(0, size.H).SelectMany(x => Enumerable.Range(0, size.W), (y, x) => new Pos(x, y));

        public static IEnumerable<Pos> CreateGrid(Size size, Pos pos) => CreateGrid(size).Select(MathUtil.Translate(pos));

        public static IEnumerable<Pos> CreateWindow(Size size, Pos pos) => CreateGrid(size, pos);

        public static bool OutOfBounds(Vector3 pos, float w, float h, float x = 0, float y = 0) => pos.x >= w || pos.y >= h || pos.x <= x || pos.y <= y;

        public static Func<Pos, Pos> Translate(this Pos pos) => (Pos translateBy) => new Pos(pos.X + translateBy.X, pos.Y + translateBy.Y);

        public static Pos Translate(this Pos pos, Pos offset) => new Pos(pos.X + offset.X, pos.Y + offset.Y);

        public static Size Scale(this Size size, int factor) => new(size.W * factor, size.H * factor);

        public static bool WindowInBounds(Pos windowPos, Size windowSize, Size boundsSize)
            => windowPos.X <= boundsSize.W - windowSize.W
            && windowPos.Y <= boundsSize.H - windowSize.H;


        public static int Sum(this int[,] matrix) => matrix.Cast<int>().Sum();

        public static int Area(this Size size) => size.W * size.H;

        public static T Get<T>(this T[,] matrix, Pos pos) {
            var (h, w) = matrix.GetLength2D();
            if (pos.Y < 0 || pos.X < 0 || pos.Y >= h || pos.X >= w) { throw new Exception($"Pos out of Matrix bounds!!! {pos}"); }
            return matrix[pos.Y, pos.X];
        }

        public static T[,] CreateMatrix<T>(Size size, T value) {
            var (columns, rows) = size;
            T[,] matrix = new T[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++) matrix[i, j] = value;
            return matrix;
        }

        public static T[,] CreateMatrix<T>(Size size, Func<int, int, T> CreateFn) {
            var (columns, rows) = size;
            T[,] matrix = new T[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++) matrix[i, j] = CreateFn(i, j);
            return matrix;
        }

        public static string Print<T, U>(T[,] matrix, Func<int, int, U> item) {
            var str = "";
            var (rows, cols) = matrix.GetLength2D();
            for (var j = 0; j < cols; j++) {
                var s = "------";
                str += s + "\t";
            }
            str += "\n";

            for (var i = 0; i < rows; i++) {
                str += "|";
                for (var j = 0; j < cols; j++) {
                    var s = " " + item(i, j)!.ToString();
                    if (s.Length > 8) s = s.Substring(0, 8);
                    if (s.Length < 8) s = s.PadRight(7, ' ');
                    str += s + "\t|";
                }
                str += "\n";
                for (var j = 0; j < cols; j++) {
                    var s = "------";
                    str += s + "\t";
                }
                str += "\n";
            }

            return str;
        }

        public static string Print(int[,] matrix) => Print(matrix, (i, j) => matrix[i, j]);

        public static (int Height, int Width) GetLength2D<T>(this T[,] matrix) => (matrix.GetLength(0), matrix.GetLength(1));

        public static List<Pos> GetPositions(int[,] matrix, Func<int, bool> predicate) {
            var positions = new List<Pos>() { };
            var (h, w) = matrix.GetLength2D();
            for (var i = 0; i < h; i++)
                for (var j = 0; j < w; j++)
                    if (predicate(matrix[i, j])) positions.Add(new Pos(j, i));
            return positions;
        }

        public static List<Pos> GetOverlappingPositions(int[,] matrix) => GetPositions(matrix, (v) => v > 1);

        public static Pos? FindPositionForItem(this int[,] matrix, int[,] shapeMask) {
            var (h, w) = matrix.GetLength2D();
            var (shapeHeight, shapeWidth) = shapeMask.GetLength2D();
            for (var i = 0; i < h - shapeHeight + 1; i++) {
                for (var j = 0; j < w - shapeWidth + 1; j++) {
                    bool canFit = true;
                    // Console.WriteLine($"checking {i},{j}");

                    for (var k = 0; k < shapeHeight; k++) {
                        for (var l = 0; l < shapeWidth; l++) {
                            // Console.WriteLine($"\tchecking {k},{l}: {shapeMask[k, l]}, sum: {shapeMask[k, l] + matrix[i + k, j + l]}");
                            if (shapeMask[k, l] + matrix[i + k, j + l] > 1) { canFit = false; break; }
                        }
                        if (!canFit) break;
                    }
                    if (canFit) return new Pos(j, i);
                }
            }

            return null;
        }

        public static Pos? FindPositionForItem(this int[,] matrix, Size size) {
            var (h, w) = matrix.GetLength2D();
            var (itemWidth, itemHeight) = size;
            for (var i = 0; i < h - itemHeight + 1; i++) {
                for (var j = 0; j < w - itemWidth + 1; j++) {
                    bool canFit = true;
                    for (var k = 0; k < itemHeight; k++) {
                        for (var l = 0; l < itemWidth; l++) {
                            if (matrix[i + k, j + l] > 0) { canFit = false; break; }
                        }
                        if (!canFit) break;
                    }
                    if (canFit) return new Pos(j, i);
                }
            }

            return null;
        }

        public static MatrixWindow CreateMatrixWindow(Window window) => new MatrixWindow(window, MathUtil.CreateMatrix(window.Size, 1), Direction.N);

        public static MatrixWindow CreateShapeWindow(Pos pos, int[,] shapeMatrix, Direction direction) {
            var computedMatrix = shapeMatrix.Rotate(direction);
            var size = new Size(computedMatrix.GetLength(1), computedMatrix.GetLength(0));
            return new MatrixWindow(new Window(pos, size), computedMatrix, direction);
        }

        public static MatrixWindow ComputedMatrixWindow(MatrixWindow window, int[,] sourceMatrix) {
            var (cols, rows) = window.Window.Size;
            var (x, y) = window.Window.Pos;
            var (h, w) = (sourceMatrix.GetLength(0), sourceMatrix.GetLength(1));
            int[,] computed = new int[rows, cols];
            if (x + cols > w) return new MatrixWindow(window.Window, computed, 0);
            if (y + rows > h) return new MatrixWindow(window.Window, computed, 0);
            for (var i = 0; i < rows; i++) {
                for (var j = 0; j < cols; j++) {
                    if (window.Matrix[i, j] == 0) {
                        computed[i, j] = 0;
                        continue;
                    }
                    computed[i, j] = window.Matrix[i, j] + sourceMatrix[i + y, j + x];
                }
            }

            return new MatrixWindow(window.Window, computed, window.Direction);
        }


        public static int[,] ComputedSourceMatrix(MatrixWindow window, int[,] sourceMatrix) {
            var computed = CloneMatrix(sourceMatrix);
            var (h, w) = (computed.GetLength(0), computed.GetLength(1));
            var (cols, rows) = window.Window.Size;
            var (x, y) = window.Window.Pos;
            if (x + cols > w) return computed;
            if (y + rows > h) return computed;
            for (var i = 0; i < rows; i++) {
                for (var j = 0; j < cols; j++) {
                    computed[i + y, j + x] = window.Matrix[i, j] + sourceMatrix[i + y, j + x];
                }
            }

            return computed;
        }

        public static int[,] ComputedShape(Pos pos, int[,] shape, int[,] occupancy) {
            var (x, y) = pos;
            var (rows, cols) = shape.GetLength2D(); ;
            var (h, w) = occupancy.GetLength2D();
            int[,] computed = new int[rows, cols];
            if (x + cols > w) return shape;
            if (y + rows > h) return shape;

            for (var i = 0; i < rows; i++) {
                for (var j = 0; j < cols; j++) {
                    if (shape[i, j] == 0) {
                        computed[i, j] = 0;
                        continue;
                    }
                    computed[i, j] = shape[i, j] + occupancy[i + y, j + x];
                }
            }

            return computed;
        }

        public static int[,] ComputedOccupancy(Pos pos, int[,] shape, int[,] occupancy) {
            var (x, y) = pos;
            var (rows, cols) = shape.GetLength2D(); ;
            var (h, w) = occupancy.GetLength2D();
            if (x + cols > w) return shape;
            if (y + rows > h) return shape;

            int[,] computed = CloneMatrix(occupancy);
            for (var i = 0; i < rows; i++) {
                for (var j = 0; j < cols; j++) {
                    computed[i + y, j + x] = shape[i, j] + occupancy[i + y, j + x];
                }
            }

            return computed;
        }

        public static int[,] CloneMatrix(int[,] source) {
            var (rows, cols) = source.GetLength2D();
            int[,] clone = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    clone[i, j] = source[i, j];
            return clone;
        }

        public static int[,] Rotate(this int[,] matrix, Direction direction) => direction switch {
            Direction.W => matrix.Rotate90(),
            Direction.S => matrix.Rotate180(),
            Direction.E => matrix.Rotate270(),
            _ => matrix
        };

        public static int[,] Rotate180(this int[,] matrix) {
            var (h, w) = matrix.GetLength2D();
            int[,] rotated = new int[h, w];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    rotated[h - 1 - i, w - 1 - j] = matrix[i, j];
            return rotated;
        }

        public static int[,] Rotate270(this int[,] matrix) {
            var (h, w) = matrix.GetLength2D();
            int[,] rotated = new int[w, h];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    rotated[j, h - 1 - i] = matrix[i, j];
            return rotated;
        }

        public static int[,] Rotate90(this int[,] matrix) {
            var (h, w) = matrix.GetLength2D();
            int[,] rotated = new int[w, h];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    rotated[w - 1 - j, i] = matrix[i, j];
            return rotated;
        }

        public static Size Rotate(this Size size) => new Size(size.H, size.W);

        public static Direction Rotate(this Direction direction) => direction switch {
            Direction.N => Direction.E,
            Direction.E => Direction.S,
            Direction.S => Direction.W,
            Direction.W => Direction.N,
            _ => Direction.N
        };

        // TODO: Is this a disabled slot check?
        public static bool ContainsHigher(int[,] computedMatrix, int[,] shapeMatrix, int value) {
            // TODO: validate matrices here?
            bool containsHigher = false;
            var (h, w) = computedMatrix.GetLength2D();
            for (int i = 0; i < h; i++) {
                for (int j = 0; j < w; j++) {
                    if (shapeMatrix[i, j] == 0) continue;
                    if (computedMatrix[i, j] > value) {
                        containsHigher = true;
                        break;
                    }
                }
                if (containsHigher)
                    break;
            }

            return containsHigher;
        }

        public static Pos ScreenToCell(Vector2 screenPos, int cellSize) => new Pos((int)Math.Floor(screenPos.x / cellSize), (int)Math.Floor(screenPos.y / cellSize));


    }
}