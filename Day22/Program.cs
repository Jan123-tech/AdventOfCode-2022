using System.Diagnostics;

var part1 = false;

var map0 = File.ReadAllLines("data.txt")
	.TakeWhile(x => !string.IsNullOrEmpty(x));

var maxLength = map0.Select(x => x.Length).Max();
var cellSize = maxLength/3;

Console.WriteLine($"Max width: {maxLength}");
Console.WriteLine($"Cell size: {cellSize}");

var map1 = map0.Select(x => x.Length == maxLength ? x : x.PadRight(maxLength, ' ')).ToList();

var points = new char[maxLength, map1.Count];

Console.WriteLine($"Max height: {map1.Count}");

for (var r = 0; r < map1.Count; r++)
	for (var c = 0; c < maxLength; c++)
		points[c, r] = map1[r][c];

var directions0 = File.ReadAllLines("data.txt")
	.SkipWhile(x => !string.IsNullOrEmpty(x))
	.Skip(1)
	.First();

var directions = new List<(char direction, int count)>();

void CreateDirection(string text) => 
	directions.Add((text.First(), int.Parse(new string([.. text.Skip(1)]))));

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

foreach (var (direction, count) in directions)
{
	dir = GetNextDirection(dir, direction, dirV);

	for (var i = 0; i < count; i ++)
	{
		var positionPeek = GetNextPoint(position, dir, count);

		if (points[positionPeek.point.x, positionPeek.point.y] == '#')
			continue;

		position = positionPeek.point;
		dir = positionPeek.dir;
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

(Point point, Point dir) GetNextPoint(Point current, Point dir, int count)
{
	var newPoint = new Point(current.x + dir.x, current.y + dir.y);

	var xOffset = current.x % cellSize;
	var yOffset = current.y % cellSize;

	return part1 ?
		newPoint switch
		{
			(<100, -1) => (new Point(newPoint.x, 149), dir),
			(_, -1) => (new Point(newPoint.x, 49), dir),
			(150, _) => (new Point(50, newPoint.y), dir),
			(>=100, 50) when dir.y == 1 => (new Point(newPoint.x, 0), dir),
			(100, >= 50 and <100) => (new Point(50, newPoint.y), dir),
			(100, >=100) => (new Point(0, newPoint.y), dir),
			(>=50, 150) when dir.y == 1 => (new Point(newPoint.x, 0), dir),
			(50, >=150) => (new Point(0, newPoint.y), dir),
			(_, 200) => (new Point(newPoint.x, 100), dir),
			(-1, >=150) => (new Point(49, newPoint.y), dir),
			(-1, _) => (new Point(99, newPoint.y), dir),
			(<50, 99) when dir.y == -1 => (new Point(newPoint.x, 199), dir),
			(49, >=50 and <100) => (new Point(99, newPoint.y), dir),
			(49, <50) => (new Point(149, newPoint.y), dir),
			_ => (newPoint, dir)
		} :
		newPoint switch
		{
			(<100, -1) => (new Point(0, 150 + xOffset), new Point(1, 0)),
			(_, -1) => (new Point(0 + xOffset, 199), new Point(0, -1)),
			(150, _) => (new Point(99, 149 - yOffset), new Point(-1, 0)),
			(>=100, 50) when dir.y == 1 => (new Point(99, 50 + xOffset), new Point(-1, 0)),
			(100, >= 50 and <100) => (new Point(100 + yOffset, 49), new Point(0, -1)),
			(100, >=100) => (new Point(149, 49 - yOffset), new Point(-1, 0)),
			(>=50, 150) when dir.y == 1 => (new Point(49, 150 + xOffset), new Point(-1, 0)),
			(50, >=150) => (new Point(50 + yOffset, 149), new Point(0, -1)),
			(_, 200) => (new Point(100 + xOffset, 0), new Point(0, 1)),
			(-1, >=150) => (new Point(50 + yOffset, 0), new Point(0, 1)),
			(-1, _) => (new Point(50, 49 - yOffset), new Point(1, 0)),
			(<50, 99) when dir.y == -1 => (new Point(50, 50 + xOffset), new Point(1, 0)),
			(49, >=50 and <100) => (new Point(0 + yOffset, 100), new Point(0, 1)),
			(49, <50) => (new Point(0, 149 - yOffset), new Point(1, 0)),
			_ => (newPoint, dir)
		};
}

var bufferService = new BufferService();

bufferService.Output(points);