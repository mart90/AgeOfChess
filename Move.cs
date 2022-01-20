using System;

namespace AgeOfChess
{
    class Move
    {
        public int? SourceSquareX { get; set; }
        public int? SourceSquareY { get; set; }

        public int DestinationSquareX { get; set; }
        public int DestinationSquareY { get; set; }

        public string PiecePlaced { get; set; }
        public string ObjectCaptured { get; set; }

        public string SourceSquareStr => SourceSquareX != null ? $"{XToLetter(SourceSquareX.Value)}{SourceSquareY.Value}" : null;
        public string DestinationSquareStr => $"{XToLetter(DestinationSquareX)}{DestinationSquareY}";

        public Move(string sourceSquare, string destinationSquare, string piecePlaced = null, string objectCaptured = null)
        {
            (DestinationSquareX, DestinationSquareY) = SquareStringToXY(destinationSquare);

            if (sourceSquare != null)
            {
                (SourceSquareX, SourceSquareY) = SquareStringToXY(sourceSquare);
            }

            PiecePlaced = piecePlaced;
            ObjectCaptured = objectCaptured;
        }

        public Move(Square sourceSquare, Square destinationSquare, string piecePlaced = null, string objectCaptured = null)
        {
            if (sourceSquare != null)
            {
                SourceSquareX = sourceSquare.X;
                SourceSquareY = sourceSquare.Y;
            }

            DestinationSquareX = destinationSquare.X;
            DestinationSquareY = destinationSquare.Y;

            PiecePlaced = piecePlaced;
            ObjectCaptured = objectCaptured;
        }

        public Move(string notation)
        {
            string sourceSquare = null;
            string destinationSquare = null;

            if (!notation.Contains("x") && !notation.Contains("-"))
            {
                // Placed piece
                destinationSquare = notation[0..^1];
                PiecePlaced = notation[^1..];
            }
            else if (notation.Contains("x"))
            {
                sourceSquare = notation.Split('x')[0];
                destinationSquare = notation.Split('x')[1];
            }
            else if (notation.Contains("-"))
            {
                sourceSquare = notation.Split('-')[0];
                destinationSquare = notation.Split('-')[1];
            }

            if (sourceSquare != null)
            {
                (SourceSquareX, SourceSquareY) = SquareStringToXY(sourceSquare);
            }

            (DestinationSquareX, DestinationSquareY) = SquareStringToXY(destinationSquare);
        }

        public string ToNotation()
        {
            if (PiecePlaced != null)
            {
                return $"{XToLetter(DestinationSquareX)}{DestinationSquareY}{PiecePlaced}";
            }

            string connector = ObjectCaptured != null ? "x" : "-";

            return $"{XToLetter(SourceSquareX.Value)}{SourceSquareY.Value}{connector}{XToLetter(DestinationSquareX)}{DestinationSquareY}";
        }

        public static string ObjectTypeToStringId(Type placeableObjectType)
        {
            if (placeableObjectType == typeof(Treasure)) return "t";
            else if (placeableObjectType == typeof(Queen)) return "q";
            else if (placeableObjectType == typeof(Rook)) return "r";
            else if (placeableObjectType == typeof(Bishop)) return "b";
            else if (placeableObjectType == typeof(Knight)) return "n";
            else if (placeableObjectType == typeof(Pawn)) return "p";

            else throw new NotImplementedException();
        }

        public static string ObjectToStringId(PlaceableObject obj)
        {
            if (obj is Treasure) return "t";
            else if (obj is Queen) return "q";
            else if (obj is Rook) return "r";
            else if (obj is Bishop) return "b";
            else if (obj is Knight) return "n";
            else if (obj is Pawn) return "p";

            else throw new NotImplementedException();
        }

        private static (int x, int y) SquareStringToXY(string squareString)
        {
            int x = LetterToX(squareString[0]);
            int y = int.Parse(squareString[1..]);

            return (x, y);
        }

        private static string XToLetter(int x)
        {
            return ((char)(x + 96)).ToString();
        }

        private static int LetterToX(char letter)
        {
            return letter - 96;
        }
    }
}
