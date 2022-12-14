var paths = System.IO.File.ReadAllLines("data.txt")
	.Select(x => x.Split(" -> ", StringSplitOptions.RemoveEmptyEntries)
		.Select(x => x.Split(",")
			.Select(x => int.Parse(x)))
		.Select(x => new Point(x.First(), x.Last())))
	.Select(x => x);

var map = new char[5001,180];
var mapLowerBoundx = 385;//435;
var mapUpperBoundx = 5000;//435;
var mapUpperBoundy = 180;
var mapLowerBoundy = 0;

var lowestPlatform = paths.Max(x => x.Max(x => x.y)) + 2;
paths = paths.Append(new List<Point> { new Point(0, lowestPlatform), new Point(map.GetLength(0)-2, lowestPlatform) });

ClearMap();
map[500,0] = '+';

foreach (var p in paths.Select(p => p.ToArray()))
{
	var x = p.First().x;
	var y = p.First().y;
	map[x,y] = '#';

	for (var i = 1; i < p.Count(); i++)
	{
		var newPoint = p[i];
		var xD = newPoint.x - x;
		var yD = newPoint.y - y;

		if (xD != 0 && yD != 0)
			throw new ArgumentException();

		var diff = xD == 0 ? yD : xD;
		var direction = diff < 0 ? -1 : 1;
		var absDiff = Math.Abs(diff);

		for (var j = 0; j < absDiff; j++)
		{
			if (xD == 0)
				y += direction;
			else
				x += direction;
				
			map[x,y] = '#';
		}
	}
}

var count = 0;

while (true)
{
	if (Move(new(500, 0)))
		count++;
	else
		break;
}

Output();

Console.WriteLine($"Count: {count}");

bool Move(Point p)
{
	if (map[p.x, p.y] == 'o')
		return false;

	var newY = p.y + 1;

	if (newY == mapUpperBoundy)
		return false;

	var newPoint = new [] { 0, -1, 1 }
		.Select(x => new Point(p.x + x, newY))
		.Where(x => map[x.x, x.y] == '.')
		.FirstOrDefault();

	map[p.x, p.y] = newPoint == null ? 'o' : '.' ;		

	if (newPoint == null)
		return true;
	else
		return Move(newPoint);
}

void ClearMap()
{
	for (var x = 0; x < map.GetLength(0); x++)
		for (var y = 0; y < map.GetLength(1); y++)
			map[x, y] = '.';
}

void Output()
{
	for (var y = 0; y < 3; y++)
	{
		Console.Write("   ");
		for (var x = mapLowerBoundx-1; x < mapUpperBoundx; x++)
		{
			Console.Write(x % 4 == 0 ? x.ToString().ToCharArray()[y] : ' ');
		}
		Console.Write("\n");
	}

	for (var y = mapLowerBoundy; y < mapUpperBoundy; y++)
	{
		var axis = y.ToString();
		var axisPadded = $"{(axis.Length == 1 ? "  " : axis.Length == 2 ? " " : "")}{axis} ";
		Console.Write(axisPadded);

		for (var x = mapLowerBoundx; x < mapUpperBoundx; x++)
		{
			Console.Write(map[x, y]);
		}
		Console.Write("\n");
	}
}

record Point(int x, int y);
