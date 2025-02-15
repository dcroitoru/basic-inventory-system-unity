namespace GDS.Core {

    public static class Shapes {
        public static readonly int[,] Rect1x1 = new int[1, 1] { { 1 } };
        public static readonly int[,] Rect1x2 = new int[2, 1] { { 1 }, { 1 }, };
        public static readonly int[,] Rect1x3 = new int[3, 1] { { 1 }, { 1 }, { 1 }, };
        public static readonly int[,] Rect1x4 = new int[4, 1] { { 1 }, { 1 }, { 1 }, { 1 }, };
        public static readonly int[,] Rect2x1 = new int[1, 2] { { 1, 1 }, };
        public static readonly int[,] Rect2x2 = new int[2, 2] { { 1, 1 }, { 1, 1 } };
        public static readonly int[,] Rect2x3 = new int[3, 2] { { 1, 1 }, { 1, 1 }, { 1, 1 } };
        public static readonly int[,] Rect3x1 = new int[1, 3] { { 1, 1, 1 }, };
        public static readonly int[,] Rect3x2 = new int[2, 3] { { 1, 1, 1 }, { 1, 1, 1 }, };
        public static readonly int[,] Rect3x3 = new int[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        public static readonly int[,] Rect3x4 = new int[4, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        public static readonly int[,] Rect3x5 = new int[5, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        public static readonly int[,] Rect4x1 = new int[1, 4] { { 1, 1, 1, 1 }, };
        public static readonly int[,] Rect4x2 = new int[2, 4] { { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, };
        public static readonly int[,] Rect4x3 = new int[3, 4] { { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, };
        public static readonly int[,] Rect5x3 = new int[3, 5] { { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, };

        // TODO: consider turning this into a dictionary
        // Why use a dictionary?

        // Faster lookups (O(1) instead of O(n) for switch).
        // Easier to extend without modifying code structure.
        // private static readonly Dictionary<Size, int[,]> _shapes = new() {
        //     [Sizes.Size1x1] = Rect1x1,
        //     [Sizes.Size1x2] = Rect1x2,
        //     [Sizes.Size1x3] = Rect1x3
        // };
        // public static int[,] GetShape(Size size) => _shapes.TryGetValue(size, out var shape) ? shape : new int[0, 0];


        public static int[,] GetShape(Size size) => size switch {
            Size s when s == Sizes.Size1x1 => Rect1x1,
            Size s when s == Sizes.Size1x2 => Rect1x2,
            Size s when s == Sizes.Size1x3 => Rect1x3,
            Size s when s == Sizes.Size1x4 => Rect1x4,
            Size s when s == Sizes.Size2x1 => Rect2x1,
            Size s when s == Sizes.Size2x2 => Rect2x2,
            Size s when s == Sizes.Size2x3 => Rect2x3,
            Size s when s == Sizes.Size3x1 => Rect3x1,
            Size s when s == Sizes.Size3x2 => Rect3x2,
            Size s when s == Sizes.Size3x3 => Rect3x3,
            Size s when s == Sizes.Size4x1 => Rect4x1,
            Size s when s == Sizes.Size4x2 => Rect4x2,
            Size s when s == Sizes.Size4x3 => Rect4x3,
            Size s when s == Sizes.Size5x3 => Rect5x3,
            _ => Rect1x1
        };
    }



}