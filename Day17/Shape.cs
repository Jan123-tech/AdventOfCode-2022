class Shape
{
	public char[,] Bitmap;
	public int x;
	public int y;

  public int StartOffset;

	public int Height =>
		Bitmap.GetLength(1);

	public Shape(char[,] bitmap, int x, int y, int startOffset)
	{
			Bitmap = bitmap;
			this.x = x;
			this.y = y;
      StartOffset = startOffset;
	}
}