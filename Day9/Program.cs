var moves = File.ReadAllLines("data.txt")
	.Select(x => CreateMove(x.First(),
		int.Parse(x.Substring(x.LastIndexOf(" ") + 1))))
	.SelectMany(x => Enumerable.Range(1, x.iterations).Select(x0 => x.impulse));

var acc = moves.Aggregate(
	(nodes: Enumerable.Range(1, 10).Select(x => new Position(0, 0)),
		positions: new List<Position>()), (acc, move) =>
		{
			var newNodes = MoveTail(new [] { acc.nodes.First().Move(move) }.Concat(acc.nodes.Skip(1)));
			acc.positions.Add(newNodes.Last());
			//Output(newNodes, acc.positions);
			return (newNodes, acc.positions);
		});
			
Console.WriteLine(acc.positions.Distinct().Count());

static IEnumerable<Position> MoveTail(IEnumerable<Position> nodes)
{
  return nodes.Count() == 1 ?
		nodes :
		MoveTailInternal();

  IEnumerable<Position> MoveTailInternal()
  {
    var head = nodes.First();
    var tail = nodes.Skip(1).First();

    var xDist = head.x - tail.x;
    var yDist = head.y - tail.y;
    var dist = Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));

    return new[] { head }.Concat(
        MoveTail(new[] { dist > 1 ? CreateNode() : tail }
          .Concat(nodes.Skip(2)).ToArray()));

    Position CreateNode()
    {
      var distanceToMakeUp = dist - 1;
      var sin = yDist / dist;
      var cos = xDist / dist;
      var y = distanceToMakeUp * sin;
      var x = distanceToMakeUp * cos;
      var i = new Impulse((int)Math.Round(x, 0), (int)Math.Round(y, 0));
      var node = tail.Move(i);
      return node;
    }
  }
}

void Output(IEnumerable<Position> nodes, IEnumerable<Position> positions)
{
	var sb = new System.Text.StringBuilder();
	Display(sb, nodes.ToArray(), positions);
	Console.Clear();
	Console.WriteLine(sb);
	Console.ReadKey();
}
	
void Display(System.Text.StringBuilder sb, Position[] nodes, IEnumerable<Position> positions)
{
	int cols = 50, rows = 30;
	for (var j = rows; j >= -rows; j--)
	{
		for (var i = -cols; i < cols; i++)
		{
			var p = new Position(i, j);
			string? c = null;
			for (var a = nodes.Length - 1; a >= 0; a--)
			{
				if (nodes[a] == p) c = a == 0 ? "H" : a.ToString();
			}
			c = c ?? (j == 0 && i == 0 ? "s" : positions.Contains(p) ? "#" : ".");
			sb.Append(c);
		}
		sb.Append("\n");
	}
}

MoveAggregate CreateMove(char c, int iterations) => c switch
{
	'R' => new MoveAggregate(new Impulse(1, 0), iterations),
	'L' => new MoveAggregate(new Impulse(-1, 0), iterations),
	'U' => new MoveAggregate(new Impulse(0, 1), iterations),
	'D' => new MoveAggregate(new Impulse(0, -1), iterations),
	_ => throw new ArgumentException()
};

record Impulse(int x, int y);
record MoveAggregate(Impulse impulse, int iterations);
record Position(int x, int y)
{
	public Position Move(Impulse v) =>
		new Position(x + v.x, y + v.y);
}