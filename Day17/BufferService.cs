public interface IBuffer 
{
  char[,] GetBuffer(int width, int height);
  void Copy(char[,] src, char[,] target, int xOffset, int yOffSet, char? charOverride = null);
  void Copy(string src, char[,] target, int xOffset, int yOffSet);
  char[,] Create(string src);
}

public class BufferService : IBuffer
{
  public char[,] Create(string src)
  {
    var srcStrArr = src.Replace("\r\n", "\n").Split("\n");

    var buffer = GetBuffer(srcStrArr.Select(x => x.Length).Max(), srcStrArr.Length);

    for (var y = 0; y < srcStrArr.Length; y++)
    {
      var xs =  srcStrArr[y].ToCharArray();

      for (var x = 0; x < xs.Length; x++)
        buffer[x, y] = xs[x];
    }

    return buffer;
  }

  public void Copy(string src, char[,] target, int xOffset, int yOffSet)
  {
    var s = Create(src);
    Copy(s, target, xOffset, yOffSet);
  }

  public void Copy(char[,] src, char[,] target, int xOffset, int yOffSet, char? charOverride = null)
  {
    for (var x = 0; x < src.GetLength(0); x++)
      for (var y = 0; y < src.GetLength(1); y++)
      {
        var c = src[x, y];
        if (c != '.')
         target[x + xOffset, y + yOffSet] = charOverride ?? c;
      }
  }

  public void RenderChar(char[,] buffer, Point? p, char c, int offset, int dim)
  {
    var x = p.x + offset;
    var y = p.y + offset;
    if (x >= 0 && y >= 0 && x < dim && y < dim)
      buffer[x, y] = c;
  }

  public char[,] GetBuffer(int width, int height)
  {
    var buffer = new char[width, height];
    ClearBuffer(buffer);
    return buffer;
  }

  void ClearBuffer(char[,] buffer)
  {
    for (var x = 0; x < buffer.GetLength(0); x++)
      for (var y = 0; y < buffer.GetLength(1); y++)
        buffer[x, y] = '.';
  }

  public void Output(char[,] buffer, int? xLower = null, int? xUpper = null, int? yLower = null, int? yUpper = null)
  {
    if (xLower == null) xLower = 0;
    if (xUpper == null) xUpper = buffer.GetLength(0);
    if (yLower == null) yLower = 0;
    if (yUpper == null) yUpper = buffer.GetLength(1);

    var numDigits = 3;

    for (var y = 0; y < numDigits; y++)
    {
      Console.Write("   ");
      for (var x = xLower.Value-1; x < xUpper; x++)
      {
        var digits = x.ToString().ToCharArray().Reverse().ToArray();
        var digitIndex = numDigits-1-y;
        Console.Write(x % 4 == 0 ? digits.Length > digitIndex ? digits[digitIndex] : ' ' : ' ');
      }
      Console.Write("\n");
    }

    for (var y = yUpper.Value - 1; y >= yLower.Value; y--)
    {
      var axis = y.ToString();
      var axisPadded = $"{(axis.Length == 1 ? "  " : axis.Length == 2 ? " " : "")}{axis} ";
      Console.Write(axisPadded);

      for (var x = xLower.Value; x < xUpper.Value; x++)
      {
        Console.Write(buffer[x, y]);
      }
      Console.Write("\n");
    }
  }
}

public record Point(int x, int y);