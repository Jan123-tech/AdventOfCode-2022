var map0 = File.ReadAllLines("data.txt")
	.TakeWhile(x => !string.IsNullOrEmpty(x));

var maxLength = map0.Select(x => x.Length).Max();

var map1 = map0.Select(x => x.Length == maxLength ? x : x.PadRight(maxLength, ' ')).ToList();

var points = new char[maxLength, map1.Count()];

for (var r = 0; r < map1.Count(); r++)
	for (var c = 0; c < maxLength; c++)
		points[c, r] = map1[r][c];

var directions0 = File.ReadAllLines("data.txt")
	.SkipWhile(x => !string.IsNullOrEmpty(x))
	.Skip(1)
	.First();

var directions = new List<(char direction, int count)>();

void CreateDirection(string text) => 
	directions.Add((text.First(), int.Parse(new String(text.Skip(1).ToArray()))));

var sb = new System.Text.StringBuilder();

foreach (var c in $"R{directions0}")
{
	if (c == 'L' || c == 'R' && sb.Length > 0)
	{
		CreateDirection(sb.ToString());
		sb.Clear();
	}

	sb.Append(c);
}
CreateDirection(sb.ToString());

var dirV = new Point[] { new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };

var position = new Point(map1[0].Select((x, i) => (x, i)).First(x => x.x != ' ').i, 0);
var dir = dirV.Last();
points[position.x, position.y] = '>';

foreach (var d in directions)
{
	dir = GetNextDirection(dir, d.direction, dirV);

	for (var i = 0; i < d.count; i ++)
	{
		var pointPeek = GetNextPoint(position, dir, d.count);
		if (points[pointPeek.x, pointPeek.y] == '#')
			continue;

		position = pointPeek;
		points[position.x, position.y] = dir.x == 1 ? '>' : dir.x == -1 ? '<' : dir.y == 1 ? 'v' : '^';
	}
}

Console.WriteLine(1000 * (position.y + 1) + 4 * (position.x + 1) + dirV.Select((x, i) => (x, i)).First(x => x.x == dir).i);

Point GetNextDirection(Point current, char d, Point[] dirVs)
{
	var index = dirVs.Select((x, i) => (x, i)).First(x => x.x == current).i;
	var newIndex = index + (d == 'R' ? 1 : -1);
	if (newIndex < 0)
		newIndex = dirVs.Length - 1;
	else if (newIndex == dirVs.Length)
		newIndex = 0;
	return dirVs[newIndex];
}

Point GetNextPoint(Point current, Point dir, int count)
{
	var newPoint = GetNewPoint(current, dir, points!);

	while (points![newPoint.x, newPoint.y] == ' ')
	{
		newPoint = GetNewPoint(newPoint, dir, points);
	}

  return newPoint!;

	static Point GetNewPoint(Point current, Point dir, char[,] points)
  {
    var newPoint = new Point(current.x + dir.x, current.y + dir.y);
		newPoint = LimitAxises(newPoint, dir, points);
		return newPoint;
  }

	static Point LimitAxises(Point current, Point dir, char[,] points)
	{
		var x = LimitAxis(current.x, dir.x, points, 0);
		var y = LimitAxis(current.y, dir.y, points, 1);
		return new Point(x, y);
	}

  static int LimitAxis(int value, int dir, char[,] points, int dim)
  {
		var maxIndex = points.GetLength(dim) - 1;
    if (value > maxIndex)
      value = 0;
    else if (value < 0)
      value = maxIndex;
    return value;
  }
}

var bufferService = new BufferService();

bufferService.Output(points);