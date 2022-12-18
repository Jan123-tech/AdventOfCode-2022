var rows = File.ReadAllLines("data.txt");
var map = ParseMap(rows);

IList<Path> Step(IList<Path> paths, Path path, PathPoint cell)
{
	var cellValue = map[cell.x, cell.y];

	if (cellValue == 'E')
	{
		path.IsComplete = true;
		return paths;
	}

	path.points.Add(cell);

  var nextPoints0 = new List<PathPoint>
  {
    new (cell.x + 1, cell.y, '>'), new (cell.x - 1, cell.y, '<'),
    new (cell.x, cell.y + 1, 'v'), new (cell.x, cell.y - 1, '^')
  };

	var nextPoints = nextPoints0;
  var nextPoints1 = nextPoints.Where(x => x.x >= 0 && x.x < map.GetLength(0) && x.y >= 0 && x.y < map.GetLength(1));
  var nextPoints2 = nextPoints1.Where(x => !path.visited.Contains(x));
	var nextPoints3 = nextPoints2.Where(x =>
	{
		var newCellValue = map[x.x, x.y];

		var isS = cellValue == 'S' && path.points.Count() == 1;
		var isok = Math.Abs(newCellValue - cellValue) < 2;
		var isEnd = cellValue == 'z' && newCellValue == 'E';
		
		return   isok || isEnd || isS;
	})
  .ToList();

	var pathPoints = path.points.Select(x => new PathPoint(x.x, x.y, x.direction)).ToList();

  for (int i = 0; i < nextPoints3.Count; i++)
	{
		if (i > 0)
		{
			//return paths;
		}
    var p = nextPoints3[i];

		Path path0 = path;

		if (i > 0)
		{
			var newPath = new Path(pathPoints.Select(x => new PathPoint(x.x, x.y, x.direction)).ToList(),
				new HashSet<PathPoint>());
			paths.Add(newPath);
			path0 = newPath;
		}

		if (path0.points.Count() > 0)
		{
			var last = path0.points[path0.points.Count() - 1];
			path0.points[path0.points.Count() - 1] = new PathPoint(last.x, last.y, p.direction);
		}

		Step(paths, path0, p);
  }

  return paths;
}

var origPath = new Path(new List<PathPoint>(), new HashSet<PathPoint>());
var paths = Step(new List<Path>() { origPath },
	origPath,
	new PathPoint(0, 20, 'S'));



var complete = paths.Where(p => p.IsComplete).GroupBy(x => x.points.Count()).OrderBy(x => x.Key)
.Take(1)
	.SelectMany(x => x);
var path = complete.FirstOrDefault();

Console.WriteLine(path?.points.Count());

foreach (var p in complete)
{
	foreach (var x in p.points)
	{
		Console.Write(map[x.x, x.y]);
	}

	Console.Write("\n");
}

foreach (var c in complete)
{
Output(c);
Console.Write("\n");
}

void Output(Path? path)
{
  for (var i = 0; i < map.GetLength(1); i++)
  {
    for (var j = 0; j < map.GetLength(0); j++)
    {
			var pathPoint = path?.points.FirstOrDefault(p => p.x == j && p.y == i)?.direction;

			var c = pathPoint ?? ((map[j,i] == 'E') ? 'E' : '.');
      Console.Write(c);
    }
    Console.Write("\n");
  }
}

static char[,] ParseMap(string[] rows)
{
  var map = new char[rows[0].Length, rows.Length];

  for (int r = 0; r < rows.Length; r++)
  {
    var cells = rows[r].ToCharArray();

    for (int c = 0; c < cells.Length; c++)
    {
      map[c, r] = cells[c];
    }
  }

  return map;
}


record Path(IList<PathPoint> points, HashSet<PathPoint> visited)
{
	public bool IsComplete { get; set; } = false;
}
record PathPoint(int x, int y, char direction);