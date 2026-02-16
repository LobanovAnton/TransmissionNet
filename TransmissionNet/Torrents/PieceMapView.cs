namespace TransmissionNet.Torrents;

public class PieceMapView : GraphicsView
{
    private const int CellSize = 8;
    private const int CellSpacing = 1;

    public static readonly BindableProperty PiecesProperty =
        BindableProperty.Create(nameof(Pieces), typeof(byte[]), typeof(PieceMapView), Array.Empty<byte>(),
            propertyChanged: OnPiecesChanged);

    public static readonly BindableProperty BeginPieceProperty =
        BindableProperty.Create(nameof(BeginPiece), typeof(int), typeof(PieceMapView), 0,
            propertyChanged: OnPiecesChanged);

    public static readonly BindableProperty PieceCountProperty =
        BindableProperty.Create(nameof(PieceCount), typeof(int), typeof(PieceMapView), 0,
            propertyChanged: OnPiecesChanged);

    public int[] Availability
    {
        get;
        set;
    } = [];

    public byte[] Pieces
    {
        get => (byte[])GetValue(PiecesProperty);
        set => SetValue(PiecesProperty, value);
    }

    public int BeginPiece
    {
        get => (int)GetValue(BeginPieceProperty);
        set => SetValue(BeginPieceProperty, value);
    }

    public int PieceCount
    {
        get => (int)GetValue(PieceCountProperty);
        set => SetValue(PieceCountProperty, value);
    }

    public PieceMapView()
    {
        Drawable = new PieceMapDrawable(this);
    }

    private static void OnPiecesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PieceMapView view)
        {
            view.UpdateHeight(view.Width);
            view.Invalidate();
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateHeight(width);
    }

    private void UpdateHeight(double width)
    {
        if (PieceCount <= 0 || width <= 0)
            return;

        int step = CellSize + CellSpacing;
        int cols = Math.Max(1, (int)(width / step));
        int rows = (int)Math.Ceiling((double)PieceCount / cols);

        HeightRequest = rows * step;
    }

    private class PieceMapDrawable(PieceMapView view) : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            byte[] pieces = view.Pieces;
            int beginPiece = view.BeginPiece;
            int pieceCount = view.PieceCount;

            if (pieces.Length == 0 || pieceCount <= 0)
                return;

            int step = CellSize + CellSpacing;
            int cols = Math.Max(1, (int)(dirtyRect.Width / step));

            Color downloadedColor = Application.Current?.RequestedTheme == AppTheme.Dark
                ? Colors.LimeGreen
                : Colors.Green;
            Color missingColor = Application.Current?.RequestedTheme == AppTheme.Dark
                ? Colors.DarkGray
                : Colors.LightGray;

            for (int i = 0; i < pieceCount; i++)
            {
                int col = i % cols;
                int row = i / cols;
                
                if (view.Availability[i] == 0)
                    canvas.FillColor = Colors.DarkRed;
                else
                {
                    int pieceIndex = beginPiece + i;
                    int byteIndex = pieceIndex / 8;
                    int bitIndex = 7 - (pieceIndex & 7);

                    bool isDownloaded = byteIndex < pieces.Length && (pieces[byteIndex] & (1 << bitIndex)) != 0;
                    
                    canvas.FillColor = isDownloaded ? downloadedColor : missingColor;
                }
                canvas.FillRectangle(col * step, row * step, CellSize, CellSize);
            }
        }
    }
}
