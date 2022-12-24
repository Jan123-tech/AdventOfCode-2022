var points = File.ReadAllLines("data.txt")
	.SelectMany((x, i) => x
		.Select((x, i) => (x, i))
		.Where(x => x.x == '#')
		.Select(x => new Point(x.i, i)))
		.ToList();

var xMax = points.Max(x => x.x);
var yMax = points.Max(x => x.y);
var padding = 15;

var bufferService = new BufferService();
var buffer = bufferService.GetBuffer(xMax + padding*2, yMax + padding*2);

var movesIndex = 0;

var moves = new Point[][] 
{
	new []
	{
		new Point(-1, -1),
		new Point(0, -1),
		new Point(1, -1)
	},
	new []
	{
		new Point(-1, 1),
		new Point(0, 1),
		new Point(1, 1)
	},
	new []
	{
		new Point(-1, -1),
		new Point(-1, 0),
		new Point(-1, 1)
	},
	new []
	{
		new Point(1, -1),
		new Point(1, 0),
		new Point(1, 1)
	}
};

var allMoves = moves.SelectMany(x => x.Select(x => x)).ToList();

var endPoints = Enumerable.Range(1, 1000).Aggregate(points, (a, x) => 
{
	Console.WriteLine($"Round: {x}");
	var newPoints = GetNewPoints(a, moves);

	//Output(newPoints, x);
	return newPoints.ToList();
})
.ToList();

var endXMax = endPoints.Max(x => x.x);
var endYMax = endPoints.Max(x => x.y);
var endXMin = endPoints.Min(x => x.x);
var endYMin = endPoints.Min(x => x.y);

var space = ((endXMax - endXMin) + 1) * ((endYMax - endYMin) + 1);
var answer = space - endPoints.Count();

Console.WriteLine(answer);

IList<Point> GetNewPoints(IEnumerable<Point> elfs, Point[][] moves)
{
	var m = moves[movesIndex];

	var indexedAllElfs = elfs.ToHashSet();

	var elfsToMove = elfs
		.Select((point, index) => (point, index))
		.Where(x =>
		{
			var allMoves0 = allMoves.Select(x0 => new Point(x0.x + x.point.x, x0.y + x.point.y));
			var elfInMoveLocation = allMoves0.Any(x => indexedAllElfs.Contains(x));

			return elfInMoveLocation;
		})
		.ToList();

	if (elfsToMove.Count() == 0)
	{
		throw new Exception("Finished");
	}

	var newPoints0 = elfsToMove.Select(x =>
	{
		var newPoint = GetNewDirection(x.point, movesIndex, moves, indexedAllElfs);
		return (point: newPoint, index: x.index);
	});

	var pointGroups = newPoints0.GroupBy(x => x.point, x => x.index);

	var indexedPointsNonMovers = pointGroups.Where(x => x.Count() > 1).SelectMany(x => x).ToHashSet();
	var indexedElfsToMove = elfsToMove.Select(x => x.index).ToHashSet();
	var indexedNewPoints0 = newPoints0.ToDictionary(x => x.index, x => x.point);

	var newPoints1 = elfs.Select((x, i) =>
		indexedElfsToMove.Contains(i) ? 
			indexedPointsNonMovers.Contains(i) ?
				x :
				indexedNewPoints0[i] :
			x).ToList();
			
	movesIndex += 1;
	if (movesIndex == moves.Length)
		movesIndex = 0;

	return newPoints1;
}

Point GetNewDirection(Point current, int startIndex, Point[][] moves, HashSet<Point> points)
{
	var index = startIndex;
	while (true)
	{
		var m = moves[index];

		var empty = m.Select(x => new Point(current.x + x.x, current.y + x.y)).All(x => !points.Contains(x));

		if (empty)
			return new Point(current.x + m[1].x, current.y + m[1].y);

		index++;
		if (index == moves.Length)
			index = 0;
		if (index == startIndex)
			return current;
	}
}

void Output(IEnumerable<Point> points, int round)
{
	bufferService.ClearBuffer(buffer);
	Console.WriteLine($"Round: {round}");
	foreach (var p in points)
	{
		buffer[p.x + padding, p.y + padding] = '#';
	}
	bufferService.Output(buffer);
}