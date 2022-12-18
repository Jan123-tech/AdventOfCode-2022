var paths = System.IO.File.ReadAllLines("data.txt")
	.Select(x => x.Split(":")
		.Select(x0 => x0.Split(",")
			.Select(x1 => x1.Substring(x1.LastIndexOf("=")+1))
			.Select(x2 => int.Parse(x2)))
		.Select(x3 => new Point(x3.First(), x3.Last())));

var sensors = paths.Select(x4 => (sensor: x4.First(), beacon: x4.Last()))
	.Select(x => (x.sensor, x.beacon,
		radius:(
				Math.Abs(x.beacon.x - x.sensor.x) +
				Math.Abs(x.beacon.y - x.sensor.y))));

var max = 4000000;

bool Contains((int l, int u) range0, (int l, int u) range1) =>
  (range0.l >= range1.l || Math.Abs(range0.l - range1.l) == 1)
		&& (range0.u <= range1.u || Math.Abs(range0.u - range1.u) == 1);

bool Overlap((int l, int u) range0, (int l, int u) range1) =>
  (range0.u >= range1.l || Math.Abs(range0.u - range1.l) == 1) &&
		(range0.l <= range1.u || Math.Abs(range0.l - range1.u) == 1);

bool IsOverlap((int l, int u) range0, (int l, int u) range1) =>
    Contains(range0, range1) || Contains(range1, range0) ||
        Overlap(range0, range1) || Overlap(range1, range0);

IList<(int l, int u)> GetNumOfInValidPositions(int row)
{
	var points = sensors.Where(x => row >= x.sensor.y - x.radius && row <= x.sensor.y + x.radius);
	var points0 = points.Select(x => (sensor: x, diff: Math.Abs(x.sensor.y - row)));
	var points1 = points0.Select(x => (sensor: x.sensor, num: x.sensor.radius - x.diff));
	var point2 = points1.Select(x => (sensor: x.sensor, range: (l: x.sensor.sensor.x - x.num, u: x.sensor.sensor.x + x.num)));
	var point3 = point2.Select(x => (sensor: x.sensor, range: (l: Math.Max(x.range.l, 0), u: Math.Min(x.range.u, max))));

	var ranges = point3.Skip(1).Aggregate(new List<(int l, int u)> { point3.First().range }, (a, r0) =>
	{
		var nonOverlaps = a.Where(x => !IsOverlap(x, r0.range));
		var overlaps = a.Where(x => IsOverlap(x, r0.range));

		var newRange = overlaps.Any() ?
			overlaps.Skip(1).Append(r0.range).Aggregate(overlaps.First(), (a, r) =>
				(Math.Min(a.l, r.l), Math.Max(a.u, r.u))) :
			r0.range;

		return new [] { newRange }.Concat(nonOverlaps).ToList();
	});

	return ranges;
}

var rows = new IList<(int l, int u)>[max];
for (var r = 0; r < max; r++)
{
	rows[r] = GetNumOfInValidPositions(r);
}

var rowWithGap = rows.Select((x, i) => (x, i)).First(x => x.x.Count() > 1).i;
var ranges = rows[rowWithGap];
var x = Enumerable.Range(0, max).First(x => !ranges.Any(r => r.l <= x && r.u >= x));

Console.WriteLine($"x: {x} y: {rowWithGap}");

var points = sensors.Select(x =>
{
	var segment = DrawSegment(x.radius);
	var segment2 = MirrorBuffer(segment, true);
	var segment4 = MirrorBuffer(segment2, false);

	var points = GetPoints(segment4, x.radius, x.radius);
	var pointsTranslated = points.Select(x0 => new Point(x0.x + x.sensor.x, x0.y + x.sensor.y));

	return (sensor: x, points: pointsTranslated);
});

var dim = 21;
var buffer = new char[dim,dim];
var offset = 0;
ClearBuffer(buffer);

foreach (var sensor in points)
{
	foreach (var p in sensor.points)
  {
    RenderChar(p, '#');
  }
}
foreach (var s in sensors)
{
	RenderChar(s.sensor, 'S');
	RenderChar(s.beacon, 'B');
}
void RenderChar(Point? p, char c)
{
  var x = p.x + offset;
  var y = p.y + offset;
  if (x >= 0 && y >= 0 && x < dim && y < dim)
    buffer[x, y] = c;
}

//Output(buffer);

IEnumerable<Point> GetPoints(char[,] buffer, int xOffset, int yOffset)
{
	var list = new List<Point>();

		for (var x = 0; x < buffer.GetLength(0); x++)
		for (var y = 0; y < buffer.GetLength(1); y++)
		{
			var val = buffer[x, y];
			if (val != '.')
				list.Add(new Point(x - xOffset, y - yOffset));
		}
		return list;
}

char[,] MirrorBuffer(char[,] src, bool vertical)
{
	var xDimSrc = src.GetLength(0);
	var yDimSrc = src.GetLength(1);
	var xDim = vertical ? xDimSrc : xDimSrc * 2 - 1;
	var yDim = vertical ? yDimSrc * 2 - 1 : yDimSrc;
	var buffer = GetBuffer(xDim, yDim);
	for (var x = 0; x < xDim; x++)
		for (var y = 0; y < yDim; y++)
		{
			var xSrc = vertical ?
				x :
				x < xDimSrc ?
					xDimSrc - x - 1 :
					x - xDimSrc + 1;

			var ySrc = vertical ?
				y < yDimSrc ?
					yDimSrc - y - 1 :
					y - yDimSrc + 1 :
				y;

			buffer[x, y] = src[xSrc, ySrc];
		}
	return buffer;
}

char[,] GetBuffer(int width, int height)
{
	var buffer = new char[width, height];
	ClearBuffer(buffer);
	return buffer;
}

char[,] DrawSegment(int radius)
{
	var buffer = GetBuffer(radius+1, radius+1);
	while (true)
	{
		if (!Move(new(0, radius)))
			break;
	}
	return buffer;

	bool Move(Point p)
	{
		if (buffer[p.x, p.y] == '#')
			return false;

		var newY = p.y - 1;

		var newPoint = newY >= 0 ? new [] { 0, 1 }
			.Select(x => new Point(p.x + x, newY))
			.Where(x => buffer[x.x, x.y] == '.')
			.FirstOrDefault() : null;

		buffer[p.x, p.y] = newPoint == null ? '#' : '.' ;		

		if (newPoint == null)
			return true;
		else
			return Move(newPoint);
	}
}

void ClearBuffer(char[,] buffer)
{
	for (var x = 0; x < buffer.GetLength(0); x++)
		for (var y = 0; y < buffer.GetLength(1); y++)
			buffer[x, y] = '.';
}

void Output(char[,] buffer, int? xLower = null, int? xUpper = null, int? yLower = null, int? yUpper = null)
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

	for (var y = yLower.Value; y < yUpper.Value; y++)
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

record Point(int x, int y);