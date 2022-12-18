var point0 = System.IO.File.ReadAllLines("data.txt")
	.Select(x => x.Split(",")
		.Select(x => int.Parse(x)))
	.Select(x => (x: x.First(), y: x.Skip(1).First(), z: x.Last()));

var maxX = point0.Max(x => x.x);
var maxY = point0.Max(x => x.y);
var maxZ = point0.Max(x => x.z);

var points = new bool[maxX+1, maxY+1, maxZ+1];

for (var x = 0; x < points.GetLength(0); x++)
	for (var y = 0; y < points.GetLength(1); y++)
		for (var z = 0; z < points.GetLength(2); z++)
			points[x, y, z] = false;

foreach (var p in point0)
	points[p.x, p.y, p.z] = true;

var spacePoints = new HashSet<Point>();
for (var x = 0; x < points.GetLength(0); x++)
	for (var y = 0; y < points.GetLength(1); y++)
		for (var z = 0; z < points.GetLength(2); z++)
		{
			if (points[x, y, z])
				continue;

			spacePoints.Add(new Point(x, y, z));
		}

var moves = new []
{
	new Point(1, 0, 0),
	new Point(-1, 0, 0),
	new Point(0, 1, 0),
	new Point(0, -1, 0),
	new Point(0, 0, 1),
	new Point(0, 0, -1),
};

var spacePointsWithNeighbours = spacePoints.Select(p =>
{
	var neighbours = moves
		.Select(x0 => new Point(p.x + x0.x, p.y + x0.y, p.z + x0.z))
		.Where(x0 =>
			x0.x >= 0 && x0.x < points.GetLength(0) && 
			x0.y >= 0 && x0.y < points.GetLength(1) &&
			x0.z >= 0 && x0.z < points.GetLength(2))
		.Where(x0 => !points[x0.x, x0.y, x0.z])
		.ToList();

	return (p, neighbours);
})
.ToHashSet();

var spaces = new HashSet<HashSet<Point>>();

foreach (var p in spacePointsWithNeighbours)
{
	var space = spaces.FirstOrDefault(x => x.Contains(p.p));
	if (space == null)
	{
		space = new HashSet<Point>();
		spaces.Add(space);
	}

	Connect(space, p);
}

void Connect(ICollection<Point> visited, (Point point, IList<Point> neighbours) point)
{
	if (visited.Contains(point.point))
		return;

	visited.Add(point.point);

	foreach (var p in point.neighbours)
	{
		var pWithNeighbours = spacePointsWithNeighbours.First(x => x.p == p);
		Connect(visited, pWithNeighbours);
	}
}

var internalSpacePoints = spaces.Where(x => !x.Contains(new Point(0,0,0))).SelectMany(x => x.Select(x => x)).ToHashSet();

var count = 0;
for (var x = 0; x < points.GetLength(0); x++)
	for (var y = 0; y < points.GetLength(1); y++)
		for (var z = 0; z < points.GetLength(2); z++)
		{
			if (!points[x, y, z])
				continue;

			var count0 = moves
				.Select(x0 => new Point(x + x0.x, y + x0.y, z + x0.z))
				.Where(x0 =>
					x0.x >= 0 && x0.x < points.GetLength(0) && 
					x0.y >= 0 && x0.y < points.GetLength(1) &&
					x0.z >= 0 && x0.z < points.GetLength(2))
				.Where(x0 => points[x0.x, x0.y, x0.z] ||
					internalSpacePoints.Contains(new Point(x0.x, x0.y, x0.z)))
				.Count();

			count += 6 - count0;
		}
			
Console.WriteLine(count);

record Point(int x, int y, int z);