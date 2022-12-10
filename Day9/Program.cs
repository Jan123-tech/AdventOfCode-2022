var moves = File.ReadAllLines("data.txt")
	.Select(x => GetMove(x.First(),
		int.Parse(x.Substring(x.LastIndexOf(" ") + 1))));

var nodes = Enumerable.Range(1, 10).Select(x => new Position(0, 0)).ToArray();
var positions = new List<Position>();

foreach (var v in moves.SelectMany(x => Enumerable.Range(1, x.iterations).Select(x0 => x.vector)))
{
		nodes[0] = nodes[0].Move(v);
		MoveTail();
		Output();
		positions.Add(nodes.Last());
}

Console.WriteLine(positions.Distinct().Count());

void MoveTail()
{
	for (var i = 0; i < nodes.Length - 1; i++)
	{
		var h = nodes[i];
		var t = nodes[i+1];

		var xDist = h.x - t.x;
		var yDist = h.y - t.y;
		var dist = Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));

		if (dist > 1)
		{
			var distanceToMakeUp = dist - 1;
			var sin = yDist / dist;
			var cos = xDist / dist;
			var y = distanceToMakeUp * sin;
			var x = distanceToMakeUp * cos;
			nodes[i+1] = t.Move(new Vector((int)Math.Round(x, 0), (int)Math.Round(y, 0)));
		}
	}
}
	
Move GetMove(char c, int iterations) => c switch
{
		'R' => new Move(new Vector(1, 0), iterations),
		'L' => new Move(new Vector(-1, 0), iterations),
		'U' => new Move(new Vector(0, 1), iterations),
		'D' => new Move(new Vector(0, -1), iterations),
		_ => throw new ArgumentException()
};

void Output()
{
		var sb = new System.Text.StringBuilder();
		Display(sb);
		Console.Clear();
		Console.WriteLine(sb);
		Console.ReadKey();
}
	
void Display(System.Text.StringBuilder sb)
{
	int cols = 50, rows = 30;

	for (var j = rows; j >= -rows; j--)
	{
		for (var i = -cols; i < cols; i++)
		{
			var p = new Position(i, j);
			string? c = null;
			for (int a = nodes.Length - 1; a > 0; a--)
			{
				if (nodes[a] == p) c = a.ToString();
			}
			c = c ?? (nodes?[0] == p ? "H" : j == 0 && i == 0 ? "s" : ".");
			sb.Append(c);
		}
		sb.Append("\n");
	}
	sb.Append("\n");
}

record Vector(int x, int y);
record Move(Vector vector, int iterations);
record Position(int x, int y)
{
	public Position Move(Vector v) =>
		new Position(x + v.x, y + v.y);
}