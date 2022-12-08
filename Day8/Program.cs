// Part 1
var paths = File.ReadAllLines("data.txt")
	.Select(x => x.Select(x => int.Parse(x.ToString())))
	.SelectMany((x, i) => x.Select((x0, i0) => (point: (x: i, y: i0), value: x0)))
	.SelectMany(x => new [] { 'x', 'y' }, (x, y) => (dim: y, item: x))
	.GroupBy(x => (x.dim, x.dim == 'x' ? x.item.point.x : x.item.point.y))
	.Select(x => x.Select(x => x.item))
	.SelectMany(x => new [] { x, x.Reverse() });

var trees0 = paths.Aggregate(new List<(int x, int y)>(), (items, x) =>
{
	x.Aggregate(-1, (max, x) =>
	{
			if (x.value > max)
			{
				items.Add(x.point);
				max = x.value;
			}
			return max;
	});
	return items;
})
.Distinct()
.Count();

Console.WriteLine(trees0);

// Part 2
var maxScore = paths
	.SelectMany(x => x.Select(x0 => (x0.point, x0.value, path: x)))
	.GroupBy(x => x.point)
	.Aggregate(0, (max, paths) =>
	{
		var groupScore = paths.Aggregate(1, (pathScore, p) =>
		{
			var count = 0;
			foreach (var tree in p.path.SkipWhile(x => x.point != p.point).Skip(1))
			{
				count++;
				if (tree.value >= p.value)
				{
					break;
				}
			}
			return pathScore * count;
		});
		return Math.Max(groupScore, max);
	});

Console.WriteLine(maxScore);