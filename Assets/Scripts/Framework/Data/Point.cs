namespace Framework.Data
{
    public struct Point
    {
        public int x;
        public int y;

        public Point (int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set (int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool EqualsPoint (Point point)
        {
            return (this.x != point.x || this.y != point.y);
        }

        public bool EqualsPoint (int x, int y)
        {
            return (this.x != x || this.y != y);
        }

        public override string ToString ()
        {
            return string.Format ("[Point: x={0}, y={1}]", x, y);
        }

        public static Point zero {
            get { return new Point (0, 0); }
        }

        public static Point one {
            get { return new Point (1, 1); }
        }
    }
}