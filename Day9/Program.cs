var moves = File.ReadAllLines("data.txt")
	.Select(x => GetMove(x.First(),
		int.Parse(x.Substring(x.LastIndexOf(" ") + 1))))
	.SelectMany(x => Enumerable.Range(1, x.iterations).Select(x0 => x.vector));

var nodes = Enumerable.Range(1, 10).Select(x => new Position(0, 0)).ToArray();
var positions = new List<Position>();

foreach (var v in moves)
{
		nodes = MoveTail(new [] { nodes[0].Move(v) }.Concat(nodes.Skip(1)).ToArray());
		positions.Add(nodes[nodes.Length-1]);
		//Output(nodes, positions);
}

Console.WriteLine(positions.Distinct().Count());

static Position[] MoveTail(Position[] nodes)
{
	if (nodes.Length == 1)
		return nodes;

	var h = nodes[0];
	var t = nodes[1];

	var xDist = h.x - t.x;
	var yDist = h.y - t.y;
	var dist = Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));

	return new [] { h }.Concat(
			MoveTail(new [] { dist > 1 ? CalculateNewNode() : t }
				.Concat(nodes.Skip(2))
					.ToArray()))
						.ToArray();

	Position CalculateNewNode()
  {
    var distanceToMakeUp = dist - 1;
    var sin = yDist / dist;
    var cos = xDist / dist;
    var y = distanceToMakeUp * sin;
    var x = distanceToMakeUp * cos;
    var newNode = t.Move(new Vector((int)Math.Round(x, 0), (int)Math.Round(y, 0)));
    return newNode;
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

void Output(Position[] nodes, List<Position> positions)
{
		var sb = new System.Text.StringBuilder();
		Display(sb, nodes, positions);
		Console.Clear();
		Console.WriteLine(sb);
		Console.ReadKey();
}
	
void Display(System.Text.StringBuilder sb, Position[] nodes, List<Position> positions)
{
	int cols = 50, rows = 30;
	for (var j = rows; j >= -rows; j--)
	{
		for (var i = -cols; i < cols; i++)
		{
			var p = new Position(i, j);
			string? c = null;
			for (int a = nodes.Length - 1; a >= 0; a--)
			{
				if (nodes[a] == p) c = a == 0 ? "H" : a.ToString();
			}
			c = c ?? (j == 0 && i == 0 ? "s" : positions.Contains(p) ? "#" : ".");
			sb.Append(c);
		}
		sb.Append("\n");
	}
}

record Vector(int x, int y);
record Move(Vector vector, int iterations);
record Position(int x, int y)
{
	public Position Move(Vector v) =>
		new Position(x + v.x, y + v.y);
}